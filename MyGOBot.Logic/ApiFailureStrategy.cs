using MyGOBot.Logic.Logging;
using POGOProtos.Networking.Envelopes;
using PokemonGo.RocketAPI;
using PokemonGo.RocketAPI.Exceptions;
using PokemonGo.RocketAPI.Extensions;
using System;
using System.Threading.Tasks;

namespace MyGOBot.Logic {

	public class ApiFailureStrategy : IApiFailureStrategy {

		private Client client;
		private int _retryCount;

		public Client Client {
			get { return client; }
			set { client = value; }
		}
		
		public ApiFailureStrategy() { }

		public ApiFailureStrategy(Client client) {
			this.client = client;
		}

		public async Task<ApiOperation> HandleApiFailure() {
			if (_retryCount == 11)
				return ApiOperation.Abort;

			await Task.Delay(500);
			_retryCount++;

			if (_retryCount % 5 == 0) {
				await DoLogin();
			}

			return ApiOperation.Retry;
		}

		public void HandleApiSuccess() {
			_retryCount = 0;
		}

		private async Task DoLogin() {
			try {
				await client.Login.DoLogin();
			} catch (AggregateException ae) {
				throw ae.Flatten().InnerException;
			} catch (LoginFailedException) {
				Logger.Write("Login failed, the credentials you entered for your Pokemon GO account are invalid");
				Logger.Write("Trying again in 1 second...");

				await Task.Delay(1000);
			} catch (AccessTokenExpiredException) {
				Logger.Write("Access token expired");
				Logger.Write("Trying again in 1 second...");
				
				await Task.Delay(1000);
			} catch (PtcOfflineException) {
				Logger.Write("PTC servers appear to be offline");
				Logger.Write("Trying again in 15 seconds...");

				await Task.Delay(15000);
			} catch (InvalidResponseException) {
				Logger.Write("Invalid response received from Niantic's servers");
				Logger.Write("Trying again in 5 seconds...");

				await Task.Delay(5000);
			} catch (Exception ex) {
				throw ex.InnerException;
			}
		}
		public void HandleApiSuccess(RequestEnvelope request, ResponseEnvelope response) {
			_retryCount = 0;
		}

		public async Task<ApiOperation> HandleApiFailure(RequestEnvelope request, ResponseEnvelope response) {
			if (_retryCount == 11)
				return ApiOperation.Abort;

			await Task.Delay(500);
			_retryCount++;

			if (_retryCount % 5 == 0) {
				try {
					await DoLogin();
				} catch (PtcOfflineException) {
					await Task.Delay(20000);
				} catch (AccessTokenExpiredException) {
					await Task.Delay(2000);
				} catch (Exception ex) when (ex is InvalidResponseException || ex is TaskCanceledException) {
					await Task.Delay(1000);
				}
			}

			return ApiOperation.Retry;
		}
	}

}

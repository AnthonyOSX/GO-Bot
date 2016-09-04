using GMap.NET;
using GO_Bot.Internals;
using GO_Bot.Models;
using MyGOBot.Logic;
using Newtonsoft.Json.Linq;
using NLog;
using POGOProtos.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Xceed.Wpf.Toolkit.Primitives;
using CheckListBox = Xceed.Wpf.Toolkit.CheckListBox;
using Logger = NLog.Logger;

namespace GO_Bot.Views {

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		private static Logger logger = LogManager.GetCurrentClassLogger();
		private static ListBoxWriter listBoxWriter;
		private static BackgroundLoopingTask backgroundTask;
		private static Random random = new Random();

		public static MainWindow Instance { get; private set; }
		internal MainWindowModel Model { get; private set; }

		//private DateTime breakTime;

		private Logic logic;

		public MainWindow() {
			InitializeComponent();
			Instance = this;
			Model = Settings.MainWindowModel;
			DataContext = Model;
			backgroundTask = new BackgroundLoopingTask(() => {
				bool b = Task.Run(async () => {
					//if (Model.EnableBreaks && DateTime.Now > breakTime) {
					//	int breakMinutes = RandomizeBreakMinutes(Model.BreakForMinutes);
						
					//	logger.Info($"Randomized your break time to {breakMinutes} minutes");
					//	logger.Info($"Breaking, will continue after the break time is up at {DateTime.Now.AddMinutes(breakMinutes).ToString(@"yyyy-MM-dd hh\:mm\:ss tt")}...");
					//	await Task.Delay(breakMinutes * 1000);
						
					//	breakTime = DateTime.Now.AddMinutes(Model.BreakAfterMinutes);
					//	logger.Info($"Done, will break again at {breakTime.ToString(@"yyyy-MM-dd hh\:mm\:ss tt")}");
					//}

					try {
						MyGOBot.Logic.Logging.Logger.SetLogger(new NLogLogger());
						await logic.Execute();
					} catch (Exception e) {
						logger.Error(e);
					}
					
					await Task.Delay(5000);

					return true;
				}).Result;
			});
			SetupUI();
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e) {
			
			// need to load this crap manually
			
			foreach (PokemonId id in Enum.GetValues(typeof(PokemonId))) {
				if (Model.PokemonsToEvolve.Contains(id)) {
					clbPokemonToEvolve.SelectedItems.Add(id.ToString());
				}

				if (Model.PokemonsToTransfer.Contains(id)) {
					clbPokemonToTransfer.SelectedItems.Add(id.ToString());
				}

				if (Model.PokemonsToCatch.Contains(id)) {
					clbPokemonToCatch.SelectedItems.Add(id.ToString());
				}
			}
			
			clbPokemonToEvolve.ItemSelectionChanged += clbPokemonToEvolve_ItemSelectionChanged;
			clbPokemonToTransfer.ItemSelectionChanged += clbPokemonToTransfer_ItemSelectionChanged;
			clbPokemonToCatch.ItemSelectionChanged += clbPokemonToCatch_ItemSelectionChanged;

			//try {
			//	txtPassword.Password = Model.LoginPassword = Model.LoginPassword.Unprotect();
			//} catch {
			//	txtPassword.Password = String.Empty;
			//	Model.LoginPassword = String.Empty;
			//}

			logger.Info("Welcome to MyGOBot!");
			gMap.Position = new PointLatLng(Model.DefaultLatitude, Model.DefaultLongitude);
			gMap.DragButton = MouseButton.Left;
			gMap.CenterCrossPen = new Pen(new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/MyGOBot;component/Resources/pokemon_ball_16x16.png", UriKind.Absolute))), 16);
			await UpdateProvider.CheckForUpdates("https://mygobot.org/api/version");
		}

		private void Window_Closing(object sender, CancelEventArgs e) {
			GeneralSettings settings = Settings.GeneralSettings;

			settings.WindowLeft = Left;
			settings.WindowTop = Top;
			settings.WindowWidth = Width;
			settings.WindowHeight = Height;

			//try {
			//	Model.LoginPassword = txtPassword.Password.Protect();
			//} catch {
			//	Model.LoginPassword = String.Empty;
			//}

			Settings.Save();
			Environment.Exit(0);
		}

		private async void btnStartStop_Click(object sender, RoutedEventArgs e) {
			try {
				Mouse.OverrideCursor = Cursors.Wait;

				bool start = btnStartStop.Content.ToString().ToLower().Contains("start");

				if (start) {
					logic = new Logic(Model);
					logic._client.Player.UpdatePositionEvent += async (lat, lng, alt) => await gMap.SafeAccessAsync(g => g.Position = new PointLatLng(lat, lng));
					Model.Statistics = logic._stats;
					//Model.LoginPassword = txtPassword.Password;
					await backgroundTask.Start();

					//if (Model.EnableBreaks) {
					//	breakTime = DateTime.Now.AddMinutes(RandomizeBreakMinutes(Model.BreakAfterMinutes));
					//	logger.Info($"Will be breaking at {breakTime.ToString(@"yyyy-MM-dd hh\:mm\:ss tt")}");
					//}

					Model.Status = "Running";
				} else {
					await backgroundTask.Stop();
					Model.Status = null;
				}

				btnStartStop.IsEnabled = false;
				btnStartStop.Content = (start) ? "Close the program to stop" : "Start";
			} catch (Exception ex) {
				logger.Error(ex);
			} finally {
				Mouse.OverrideCursor = null;
			}
		}

		private void btnOpenLogsDirectory_Click(object sender, RoutedEventArgs e) {
			Process.Start("explorer.exe", ApplicationEnvironment.LogsDirectory());
		}

		private async void btnLogin_Click(object sender, RoutedEventArgs e) {
			await Task.Run(async () => {
				try {
					await Application.Current.Dispatcher.InvokeAsync(() => {
						busyIndicator.IsBusy = true;
					});

					//SSLValidator.OnValidateCertificate = (s, certificate, chain, errors) => {
					//	Console.WriteLine(certificate.Issuer);
					//	return true;
					//};
					//SSLValidator.OverrideValidation();

					string token = String.Empty;

					await txtToken.SafeAccessAsync((txt) => token = txt.Text);

					HttpResponseMessage message = await HttpProvider.Post("https://mygobot.org/api/auth", new Dictionary<string, string>() {
						{"token",  token }
					});

					if (message.IsSuccessStatusCode) {
						JObject jObject = JObject.Parse(await message.Content.ReadAsStringAsync());

						if (jObject.Count > 0) {
							User user = jObject.ToObject<User>();

							if (user.Banned == 1) {
								logger.Info("Your account has been banned");

								goto bottom;
							}

							switch (user.AccountType) {
								case AccountType.Default:
									logger.Info("You must either activate your trial or purchase the bot to continue using");

									goto bottom;
								case AccountType.TrialExpired:
									logger.Info("Your trial has expired! Please purchase the bot to continue using");

									goto bottom;
								case AccountType.TrialActive:
									logger.Info($"Your trial is active until {user.Trial?.AddHours(3).ToLocalTime().ToString(@"yyyy-MM-dd hh\:mm\:ss tt")}");

									break;
								case AccountType.Purchased:
									logger.Info("Premium account authenticated");

									break;
							}

							Model.User = user;
							Model.IsLoggedIn = true;
							logger.Info($"Successful login, welcome back {user.Name}");
						} else {
							logger.Info("Invalid authentication token (you need to register at mygobot.org to receive one)");
						}
					} else {
						logger.Error($"Failed to authenticate ({(int)message.StatusCode} {message.StatusCode})");
						logger.Info("Please try again later");
					}

					bottom:;
				} catch (Exception ex) {
					logger.Error(ex, $"Unhandled exception while attempting to authenticate ({ex.InnerException.Message})");
				} finally {
					//SSLValidator.RestoreValidation();
					await Application.Current.Dispatcher.InvokeAsync(() => {
						busyIndicator.IsBusy = false;
					});
				}
			});
		}

		private void hlUserName_Click(object sender, RoutedEventArgs e) {
			if (MethodProvider.DisplayMessage("Logout", "Would you like to logout?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
				Model.IsLoggedIn = false;
			}
		}
		
		private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
			Process.Start(e.Uri.ToString());
		}

		private void SetupUI() {
			GeneralSettings settings = Settings.GeneralSettings;
			listBoxWriter = new ListBoxWriter(lbLog);

			Title = $"MyGOBot v{Assembly.GetExecutingAssembly().GetName().Version.ToString(3)} {((App.Constants.IsBetaRelease) ? "(BETA)" : String.Empty)}";
			Console.SetOut(listBoxWriter);
			Console.SetError(listBoxWriter);

			// TODO weird problem with null exception in this method (I think at least?), fix later
			if (settings == null) {
				return;
			}
			
#if !DEBUG
			if (settings.WindowLeft > 0 && settings.WindowTop > 0) {
				WindowStartupLocation = WindowStartupLocation.Manual;
				Left = settings.WindowLeft;
				Top = settings.WindowTop;
				Width = settings.WindowWidth;
				Height = settings.WindowHeight;
			}
#endif
		}

		//private void btnSelectDeselectAll_Click(object sender, RoutedEventArgs e) {
		//	Button btn = sender as Button;
		//	CheckListBox clb = ((btn.Parent as StackPanel).Parent as StackPanel).Children[1] as CheckListBox; // omg why
		//	bool deselect = (btn.Content as DependencyObject).FindVisualDescendant<TextBlock>().Text.ToLower().Contains("deselect");

		//	logger.Info(deselect);

		//	foreach (PokemonId id in Enum.GetValues(typeof(PokemonId))) {
		//		if (deselect) {
		//			clb.SelectedItems.Remove(id.ToString());
		//		} else {
		//			clb.SelectedItems.Add(id.ToString());
		//		}
		//	}
		//}

		private void clbPokemonToCatch_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e) {
			string value = e.Item as string;

			if (String.IsNullOrEmpty(value)) {
				return;
			}

			PokemonId pokemonId = PokemonIdFromString(value);

			if (e.IsSelected) {
				Model.PokemonsToCatch.Add(pokemonId);
			} else {
				Model.PokemonsToCatch.Remove(pokemonId);
			}
		}

		private void clbPokemonToTransfer_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e) {
			string value = e.Item as string;

			if (String.IsNullOrEmpty(value)) {
				return;
			}

			PokemonId pokemonId = PokemonIdFromString(value);

			if (e.IsSelected) {
				Model.PokemonsToTransfer.Add(pokemonId);
			} else {
				Model.PokemonsToTransfer.Remove(pokemonId);
			}
		}

		private void clbPokemonToEvolve_ItemSelectionChanged(object sender, ItemSelectionChangedEventArgs e) {
			string value = e.Item as string;

			if (String.IsNullOrEmpty(value)) {
				return;
			}

			PokemonId pokemonId = PokemonIdFromString(value);

			if (e.IsSelected) {
				Model.PokemonsToEvolve.Add(pokemonId);
			} else {
				Model.PokemonsToEvolve.Remove(pokemonId);
			}
		}

		private PokemonId PokemonIdFromString(string value) {
			return (PokemonId)Enum.Parse(typeof(PokemonId), value, true);
		}

		private int RandomizeBreakMinutes(int setBreak) {
			return random.Next(Math.Max(setBreak - 10, 1), Math.Max(setBreak + 10, 5));
		}

		private void btnRefreshUserPaths_Click(object sender, RoutedEventArgs e) {

		}

		private void btnManageUserPaths_Click(object sender, RoutedEventArgs e) {

		}
	}

}

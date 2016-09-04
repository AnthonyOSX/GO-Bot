using System.Net;
using System.Net.Security;

namespace GO_Bot.Internals {

	internal static class SSLValidator {

		public static RemoteCertificateValidationCallback OnValidateCertificate { get; set; }

		private static RemoteCertificateValidationCallback defaultCallback;
		
		public static void OverrideValidation() {
			defaultCallback = ServicePointManager.ServerCertificateValidationCallback;
			ServicePointManager.ServerCertificateValidationCallback = OnValidateCertificate;
		}

		public static void RestoreValidation() {
			ServicePointManager.ServerCertificateValidationCallback = defaultCallback;
		}

	}

}

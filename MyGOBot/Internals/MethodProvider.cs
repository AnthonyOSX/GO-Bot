using System;
using System.Windows;
using System.Windows.Input;

namespace GO_Bot.Internals {

	internal class MethodProvider {

		public static void DisplayInformation(string message) {
			DisplayMessage("Information", message, MessageBoxButton.OK, MessageBoxImage.Information);
		}

		public static void DisplayWarning(string message) {
			DisplayMessage("Warning", message, MessageBoxButton.OK, MessageBoxImage.Warning);
		}

		public static void DisplayError(string message) {
			DisplayMessage("Error", message, MessageBoxButton.OK, MessageBoxImage.Error);
		}

		public static MessageBoxResult DisplayMessage(string title, string message, MessageBoxButton button, MessageBoxImage icon) {
			return MessageBox.Show(message, title, button, icon);
		}

		public static void WithWaitCursor(Action action) {
			try {
				Mouse.OverrideCursor = Cursors.Wait;
				action();
			} catch {
				throw;
			} finally {
				Mouse.OverrideCursor = null;
			}
		}

	}

}

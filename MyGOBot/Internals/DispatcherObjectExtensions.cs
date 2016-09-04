using System;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GO_Bot.Internals {

	internal static class DispatcherObjectExtensions {
		
		public static Task SafeAccessAsync<T>(this T dispatcherObject, Action<T> action) where T : DispatcherObject {
			return Task.Run(async () => {
				Dispatcher dispatcher = dispatcherObject.Dispatcher;

				if (dispatcher.CheckAccess()) {
					action(dispatcherObject);
				} else {
					await dispatcher.BeginInvoke(action, DispatcherPriority.Normal, dispatcherObject);
				}
			});
		}

	}

}

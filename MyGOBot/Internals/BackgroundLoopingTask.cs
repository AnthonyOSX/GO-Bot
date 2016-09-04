using System;
using System.Threading;
using System.Threading.Tasks;

namespace GO_Bot.Internals {

	internal class BackgroundLoopingTask {

		private AutoResetEvent are;
		private Task task;
		private volatile bool running;

		public Action Action { get; set; }
		public TaskCreationOptions CreationOptions { get; set; } = TaskCreationOptions.LongRunning;
		public int MillisecondsDelay { get; set; } = 10;
		public bool Running { get { return running; } }

		public BackgroundLoopingTask(Action action) {
			Action = action;
			are = new AutoResetEvent(false);
		}

		public Task Start() {
			return Task.Run(() => {
				if (running) {
					return;
				}
				
				running = true;
				task = Task.Factory.StartNew(() => {
					while (!are.WaitOne(MillisecondsDelay)) {
						Action();
					}

					running = false;
				}, CreationOptions);
			});
		}

		public Task Stop() {
			return Task.Run(() => {
				if (!running) {
					return;
				}

				are.Set();
				task.Wait();
			});
		}

	}

}

using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace GO_Bot.Internals {

	internal class ListBoxWriter : TextWriter {

		private ListBox listBox;
		private StringBuilder content = new StringBuilder();

		public ListBoxWriter(ListBox listBox) {
			this.listBox = listBox;
		}

		public override void Write(char value) {
			base.Write(value);

			content.Append(value);

			if (value == '\n') {
				if (listBox.Dispatcher.CheckAccess()) {
					WriteLogMessage();
				} else {
					listBox.Dispatcher.Invoke(new Action(() => {
						WriteLogMessage();
					}));
				}

				content = new StringBuilder();
			}
		}

		public override Encoding Encoding {
			get { return Encoding.UTF8; }
		}

		public void WriteLogMessage() {
			listBox.Items.Add(new ListBoxMessage(Regex.Replace(content.ToString(), @"\r\n?|\n", "")));

			for (int i = 0; listBox.Items.Count > 500; i--) {
				listBox.Items.RemoveAt(i);
			}

			listBox.ScrollToBottom();
		}

	}

}

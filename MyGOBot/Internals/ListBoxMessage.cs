using System.Windows.Media;

namespace GO_Bot.Internals {

	class ListBoxMessage {

		public string Text { get; protected set; }
		public Brush Color { get; protected set; }

		public ListBoxMessage(string text) : this(text, Brushes.Black) { }

		public ListBoxMessage(string text, Brush color) {
			Text = text;
			Color = color;
		}

	}

}

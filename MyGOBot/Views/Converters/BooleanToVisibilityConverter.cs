using System.Windows;

namespace GO_Bot.Views.Converters {

	internal sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility> {

		public BooleanToVisibilityConverter() :
			base(Visibility.Visible, Visibility.Collapsed) { }

	}

}

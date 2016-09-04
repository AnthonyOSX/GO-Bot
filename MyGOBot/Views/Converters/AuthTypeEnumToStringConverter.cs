using PokemonGo.RocketAPI.Enums;
using System;
using System.Globalization;
using System.Windows.Data;

namespace GO_Bot.Views.Converters {

	internal class AuthTypeEnumToStringConverter : IValueConverter {

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			return value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			return (AuthType)Enum.Parse(typeof(AuthType), value.ToString(), true);
		}

	}

}

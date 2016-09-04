using System.ComponentModel;

namespace GO_Bot.Internals {

	internal class CheckedListItem<T> : INotifyPropertyChanged {

		public event PropertyChangedEventHandler PropertyChanged;

		private bool isChecked;
		private T item;

		public CheckedListItem() { }

		public CheckedListItem(T item, bool isChecked = false) {
			this.item = item;
			this.isChecked = isChecked;
		}

		public T Item {
			get { return item; }
			set {
				item = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item"));
			}
		}
		
		public bool IsChecked {
			get { return isChecked; }
			set {
				isChecked = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsChecked"));
			}
		}

	}

}

using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace GO_Bot.Internals {

	public static class DependencyObjectExtensions {

		public static T FindLogicalAncestor<T>(this DependencyObject dependencyObject) where T : class {
			DependencyObject target = dependencyObject;

			do {
				target = LogicalTreeHelper.GetParent(target);
			} while (target != null && !(target is T));

			return target as T;
		}

		public static T FindLogicalDescendant<T>(this DependencyObject dependencyObject) where T : class {
			DependencyObject target = dependencyObject;

			do {
				target = LogicalTreeHelper.GetChildren(target).OfType<DependencyObject>().FirstOrDefault();
			} while (target != null && !(target is T));

			return target as T;
		}

		public static T FindVisualAncestor<T>(this DependencyObject dependencyObject) where T : class {
			DependencyObject target = dependencyObject;

			do {
				target = VisualTreeHelper.GetParent(target);
			} while (target != null && !(target is T));

			return target as T;
		}

		public static T FindVisualDescendant<T>(this DependencyObject dependencyObject) where T : DependencyObject {
			T foundChild = null;
			int childrenCount = VisualTreeHelper.GetChildrenCount(dependencyObject);

			for (int i = 0; i < childrenCount; i++) {
				DependencyObject child = VisualTreeHelper.GetChild(dependencyObject, i);
				T childType = child as T;

				if (childType == null) {
					foundChild = child.FindVisualDescendant<T>();

					if (foundChild != null) {
						break;
					}
				} else {
					foundChild = (T)child;

					break;
				}
			}

			return foundChild;
		}

		


		//public static T FindVisualDescendant<T>(this DependencyObject dependencyObject) where T : class {
		//	DependencyObject target = dependencyObject;

		//	do {
		//		target = VisualTreeHelper.GetChild(target);
		//	} while (target != null && !(target is T));

		//	return target as T;
		//}

	}

}

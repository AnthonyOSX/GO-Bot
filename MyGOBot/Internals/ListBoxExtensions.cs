using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace GO_Bot.Internals {

	internal static class ListBoxExtensions {

		public static void ScrollToBottom(this ListBox source) {
			try {
				ListBoxAutomationPeer svAutomation = (ListBoxAutomationPeer)UIElementAutomationPeer.CreatePeerForElement(source);
				IScrollProvider scrollInterface = (IScrollProvider)svAutomation.GetPattern(PatternInterface.Scroll);
				ScrollAmount scrollVertical = ScrollAmount.LargeIncrement;
				ScrollAmount scrollHorizontal = ScrollAmount.NoAmount;

				// If the vertical scroller is not available, the operation cannot be performed, which will raise an exception. 
				if (scrollInterface.VerticallyScrollable) {
					scrollInterface.Scroll(scrollHorizontal, scrollVertical);
				}
			} catch { }
		}

	}

}

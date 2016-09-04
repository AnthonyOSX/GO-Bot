using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace GO_Bot.Internals {

	internal static class StringExtensions {

		//public static string Protect(this string plainText, DataProtectionScope scope = DataProtectionScope.LocalMachine, byte[] entropy = null) {
		//	if (plainText == null) {
		//		throw new ArgumentNullException("plainText");
		//	}

		//	return Convert.ToBase64String(ProtectedData.Protect(Encoding.UTF8.GetBytes(plainText), entropy, scope));
		//}

		//public static string Unprotect(this string encryptedText, DataProtectionScope scope = DataProtectionScope.LocalMachine, byte[] entropy = null) {
		//	if (encryptedText == null) {
		//		throw new ArgumentNullException("encryptedText");
		//	}

		//	return Encoding.UTF8.GetString(ProtectedData.Unprotect(Convert.FromBase64String(encryptedText), entropy, scope));
		//}

		public static string SanitizeFileName(this string source) {
			return String.Join("_", source.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
		}
		
		//public static string GetSummedFriendlyNameOfItemAwardList(IEnumerable<FortSearchResponse.Types.ItemAward> items) {
		//	var enumerable = items as IList<FortSearchResponse.Types.ItemAward> ?? items.ToList();

		//	if (!enumerable.Any())
		//		return string.Empty;

		//	return
		//		enumerable.GroupBy(i => i.ItemId)
		//				  .Select(kvp => new { ItemName = kvp.Key.ToString(), Amount = kvp.Sum(x => x.ItemCount) })
		//				  .Select(y => $"{y.Amount} x {y.ItemName}")
		//				  .Aggregate((a, b) => $"{a}, {b}");
		//}

	}

}

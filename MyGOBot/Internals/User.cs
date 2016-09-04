using Newtonsoft.Json;
using System;

namespace GO_Bot.Internals {

	internal class User {

		[JsonProperty(PropertyName = "id")]
		public int Id { get; set; }

		[JsonProperty(PropertyName = "name")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "email")]
		public string Email { get; set; }

		[JsonProperty(PropertyName = "banned")]
		public int Banned { get; set; }

		[JsonProperty(PropertyName = "created_at")]
		public DateTime? CreatedAt { get; set; }

		[JsonProperty(PropertyName = "updated_at")]
		public DateTime? UpdatedAt { get; set; }

		[JsonProperty(PropertyName = "purchased")]
		public int Purchased { get; set; }

		[JsonProperty(PropertyName = "purchased_at")]
		public DateTime? PurchasedAt { get; set; }

		[JsonProperty(PropertyName = "trial")]
		public DateTime? Trial { get; set; }

		[JsonProperty(PropertyName = "account_type")]
		public AccountType AccountType { get; set; }

	}

}

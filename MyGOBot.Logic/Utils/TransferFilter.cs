using POGOProtos.Enums;
using System.Collections.Generic;

namespace MyGOBot.Logic.Utils {

	public class TransferFilter {
		public TransferFilter() {
		}

		public TransferFilter(int keepMinCp, float keepMinIvPercentage, int keepMinDuplicatePokemon, List<PokemonMove> moves = null) {
			KeepMinCp = keepMinCp;
			KeepMinIvPercentage = keepMinIvPercentage;
			KeepMinDuplicatePokemon = keepMinDuplicatePokemon;
			Moves = moves ?? new List<PokemonMove>();
		}

		public int KeepMinCp { get; set; }
		public float KeepMinIvPercentage { get; set; }
		public int KeepMinDuplicatePokemon { get; set; }
		public List<PokemonMove> Moves { get; set; }
	}

}

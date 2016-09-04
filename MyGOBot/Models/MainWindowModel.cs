using Newtonsoft.Json;
using PokemonGo.RocketAPI;
using System.Diagnostics;
using System.Timers;
using PokemonGo.RocketAPI.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using GO_Bot.Internals;
using System.Threading.Tasks;
using GMap.NET.MapProviders;
using GMap.NET;
using System.IO;
using POGOProtos.Enums;
using MyGOBot.Logic.Utils;
using POGOProtos.Inventory.Item;

namespace GO_Bot.Models {

	internal class MainWindowModel : BindableBase, ISettings {

		private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

		private PerformanceCounter cpuCounter;
		private PerformanceCounter memoryCounter;
		private float cpuUsage;
		private float memoryUsage;
		private string status;
		private int selectedTabIndex;
		private bool isLoggedIn;
		private User user;
		private string loginToken = String.Empty;
		private string loginUsername = String.Empty;
		private string loginPassword = String.Empty;
		private AuthType authType = AuthType.Ptc;
		private double latitude = 34.073620;
		private double longitude = -118.400352;
		private double altitude = 10;
		private double keepMinIvPercentage = 95;
		private int keepMinCp = 1000;
		private double walkingSpeedKmHr = 15;
		private bool evolvePokemon = false;
		private bool transferDuplicatePokemon = true;
		private int delayBetweenPokemonCatch = 5000;
		private string googleRefreshToken = String.Empty;
		private bool prioritizeIVOverCP = false;
		private int maxTravelDistanceInMeters = 1000;
		private bool useLuckyEggsWhileEvolving = false;
		private bool evolveAllPokemonAboveIV = false;
		private double evolveAboveIVValue = 95;
		private bool onlyFarmPokestops;
		private bool useGpxPathing;
		private string gpxFile = String.Empty;
		private List<PokemonId> pokemonsToEvolve = new List<PokemonId>(Enum.GetValues(typeof(PokemonId)).OfType<PokemonId>());
		private List<PokemonId> pokemonsToTransfer = new List<PokemonId>(Enum.GetValues(typeof(PokemonId)).OfType<PokemonId>());
		private List<PokemonId> pokemonsToCatch = new List<PokemonId>(Enum.GetValues(typeof(PokemonId)).OfType<PokemonId>());
		private bool enableBreaks;
		private int breakAfterMinutes = 60;
		private int breakForMinutes = 20;
		private Statistics statistics;

		// item limit
		private int pokeBallCount = 50;
		private int greatBallCount = 50;
		private int ultraBallCount = 50;
		private int masterBallCount = 50;
		private int potionCount = 50;
		private int superPotionCount = 50;
		private int hyperPotionCount = 50;
		private int maxPotionCount = 50;
		private int reviveCount = 50;
		private int maxReviveCount = 50;
		private int razzBerryCount = 50;
		//private int blukBerryCount = 10;
		//private int nanabBerryCount = 10;
		//private int weparBerryCount = 10;
		//private int pinapBerryCount = 10;
		
		public MainWindowModel() {
			Task.Run(() => {
				Timer performanceTimer = new Timer();

				if (PerformanceCounterCategory.CounterExists("% Processor Time", "Process")) {
					cpuCounter = new PerformanceCounter("Process", "% Processor Time", Process.GetCurrentProcess().ProcessName);
				} else {
					logger.Error("Cannot display CPU usage (counter does not exist on your system)");
				}

				if (PerformanceCounterCategory.CounterExists("Working Set", "Process")) {
					memoryCounter = new PerformanceCounter("Process", "Working Set", Process.GetCurrentProcess().ProcessName);
				} else {
					logger.Error("Cannot display RAM usage (counter does not exist on your system)");
				}
				
				performanceTimer.Interval = 2000;
				performanceTimer.Elapsed += (s, e) => {
					if (cpuCounter != null) {
						CpuUsage = cpuCounter.NextValue();
						cpuCounter.NextValue();
					}

					if (memoryCounter != null) {
						MemoryUsage = memoryCounter.NextValue();
						memoryCounter.NextValue();
					}
				};
				performanceTimer.Start();
			});
			
			GMaps.Instance.Mode = AccessMode.ServerOnly;
		}
		
		[JsonIgnore]
		public string Status {
			get { return status; }
			set { SetProperty(ref status, value); }
		}

		[JsonIgnore]
		public float CpuUsage {
			get { return cpuUsage; }
			set { SetProperty(ref cpuUsage, value); }
		}

		[JsonIgnore]
		public float MemoryUsage {
			get { return memoryUsage; }
			set { SetProperty(ref memoryUsage, value); }
		}

		[JsonIgnore]
		public int SelectedTabIndex {
			get { return selectedTabIndex; }
			set { SetProperty(ref selectedTabIndex, value); }
		}

		[JsonIgnore]
		public bool IsLoggedIn {
			get { return isLoggedIn; }
			set {
				if (SetProperty(ref isLoggedIn, value)) {
					SelectedTabIndex = (isLoggedIn) ? 1 : 0;

					if (!isLoggedIn) {
						User = null;
					}
				}
			}
		}

		[JsonIgnore]
		public User User {
			get { return user; }
			set { SetProperty(ref user, value); }
		}

		public string LoginToken {
			get { return loginToken; }
			set { SetProperty(ref loginToken, value); }
		}

		public AuthType AuthType {
			get { return authType; }
			set { SetProperty(ref authType, value); }
		}

		public double DefaultLatitude {
			get { return latitude; }
			set { SetProperty(ref latitude, value); }
		}

		public double DefaultLongitude {
			get { return longitude; }
			set { SetProperty(ref longitude, value); }
		}

		public double DefaultAltitude {
			get { return altitude; }
			set { SetProperty(ref altitude, value); }
		}

		[JsonIgnore]
		public string GoogleRefreshToken {
			get { return googleRefreshToken; }
			set { SetProperty(ref googleRefreshToken, value); }
		}

		public string LoginPassword {
			get { return loginPassword; }
			set { SetProperty(ref loginPassword, value); }
		}

		public string LoginUsername {
			get { return loginUsername; }
			set { SetProperty(ref loginUsername, value); }
		}

		public double KeepMinIVPercentage {
			get { return keepMinIvPercentage; }
			set { SetProperty(ref keepMinIvPercentage, value); }
		}

		public int KeepMinCP {
			get { return keepMinCp; }
			set { SetProperty(ref keepMinCp, value); }
		}

		public double WalkingSpeedInKilometerPerHour {
			get { return walkingSpeedKmHr; }
			set { SetProperty(ref walkingSpeedKmHr, value); }
		}

		public bool EvolveAllPokemonWithEnoughCandy {
			get { return evolvePokemon; }
			set { SetProperty(ref evolvePokemon, value); }
		}

		public bool TransferDuplicatePokemon {
			get { return transferDuplicatePokemon; }
			set { SetProperty(ref transferDuplicatePokemon, value); }
		}

		public int DelayBetweenPokemonCatch {
			get { return delayBetweenPokemonCatch; }
			set { SetProperty(ref delayBetweenPokemonCatch, value); }
		}

		[JsonIgnore]
		public bool UsePokemonToNotCatchFilter {
			get { return true; }
		}

		[JsonIgnore]
		public int KeepMinDuplicatePokemon {
			get { return 1; }
		}
		
		public int PokeBallCount {
			get { return pokeBallCount; }
			set { SetProperty(ref pokeBallCount, value); }
		}

		public int GreatBallCount {
			get { return greatBallCount; }
			set { SetProperty(ref greatBallCount, value); }
		}

		public int UltraBallCount {
			get { return ultraBallCount; }
			set { SetProperty(ref ultraBallCount, value); }
		}

		public int MasterBallCount {
			get { return masterBallCount; }
			set { SetProperty(ref masterBallCount, value); }
		}

		public int PotionCount {
			get { return potionCount; }
			set { SetProperty(ref potionCount, value); }
		}

		public int SuperPotionCount {
			get { return superPotionCount; }
			set { SetProperty(ref superPotionCount, value); }
		}

		public int HyperPotionCount {
			get { return hyperPotionCount; }
			set { SetProperty(ref hyperPotionCount, value); }
		}

		public int MaxPotionCount {
			get { return maxPotionCount; }
			set { SetProperty(ref maxPotionCount, value); }
		}

		public int ReviveCount {
			get { return reviveCount; }
			set { SetProperty(ref reviveCount, value); }
		}

		public int MaxReviveCount {
			get { return maxReviveCount; }
			set { SetProperty(ref maxReviveCount, value); }
		}
		
		public int RazzBerryCount {
			get { return razzBerryCount; }
			set { SetProperty(ref razzBerryCount, value); }
		}

		//public int BlukBerryCount {
		//	get { return blukBerryCount; }
		//	set { SetProperty(ref blukBerryCount, value); }
		//}

		//public int NanabBerryCount {
		//	get { return nanabBerryCount; }
		//	set { SetProperty(ref nanabBerryCount, value); }
		//}

		//public int WeparBerryCount {
		//	get { return weparBerryCount; }
		//	set { SetProperty(ref weparBerryCount, value); }
		//}

		//public int PinapBerryCount {
		//	get { return pinapBerryCount; }
		//	set { SetProperty(ref pinapBerryCount, value); }
		//}

		[JsonIgnore]
		public List<KeyValuePair<ItemId, int>> ItemRecycleFilter {
			get {
				return new List<KeyValuePair<ItemId, int>>(new[] {
					new KeyValuePair<ItemId, int>(ItemId.ItemUnknown, 0),
					new KeyValuePair<ItemId, int>(ItemId.ItemPokeBall, PokeBallCount),
					new KeyValuePair<ItemId, int>(ItemId.ItemGreatBall, GreatBallCount),
					new KeyValuePair<ItemId, int>(ItemId.ItemUltraBall, UltraBallCount),
					new KeyValuePair<ItemId, int>(ItemId.ItemMasterBall, MasterBallCount),
					new KeyValuePair<ItemId, int>(ItemId.ItemPotion, PotionCount),
					new KeyValuePair<ItemId, int>(ItemId.ItemSuperPotion, SuperPotionCount),
					new KeyValuePair<ItemId, int>(ItemId.ItemHyperPotion, HyperPotionCount),
					new KeyValuePair<ItemId, int>(ItemId.ItemMaxPotion, MaxPotionCount),
					new KeyValuePair<ItemId, int>(ItemId.ItemRevive, ReviveCount),
					new KeyValuePair<ItemId, int>(ItemId.ItemMaxRevive, MaxReviveCount),
					new KeyValuePair<ItemId, int>(ItemId.ItemLuckyEgg, 200),
					new KeyValuePair<ItemId, int>(ItemId.ItemIncenseOrdinary, 100),
					new KeyValuePair<ItemId, int>(ItemId.ItemIncenseSpicy, 100),
					new KeyValuePair<ItemId, int>(ItemId.ItemIncenseCool, 100),
					new KeyValuePair<ItemId, int>(ItemId.ItemIncenseFloral, 100),
					new KeyValuePair<ItemId, int>(ItemId.ItemTroyDisk, 100),
					new KeyValuePair<ItemId, int>(ItemId.ItemXAttack, 100),
					new KeyValuePair<ItemId, int>(ItemId.ItemXDefense, 100),
					new KeyValuePair<ItemId, int>(ItemId.ItemXMiracle, 100),
					new KeyValuePair<ItemId, int>(ItemId.ItemRazzBerry, RazzBerryCount),
					new KeyValuePair<ItemId, int>(ItemId.ItemBlukBerry, 100),
					new KeyValuePair<ItemId, int>(ItemId.ItemNanabBerry, 100),
					new KeyValuePair<ItemId, int>(ItemId.ItemWeparBerry, 100),
					new KeyValuePair<ItemId, int>(ItemId.ItemPinapBerry, 100),
					new KeyValuePair<ItemId, int>(ItemId.ItemSpecialCamera, 100),
					new KeyValuePair<ItemId, int>(ItemId.ItemIncubatorBasicUnlimited, 100),
					new KeyValuePair<ItemId, int>(ItemId.ItemIncubatorBasic, 100),
					new KeyValuePair<ItemId, int>(ItemId.ItemPokemonStorageUpgrade, 100),
					new KeyValuePair<ItemId, int>(ItemId.ItemItemStorageUpgrade, 100)
				});
			}
		}
		
		public List<PokemonId> PokemonsToEvolve {
			get { return pokemonsToEvolve; }
			set { SetProperty(ref pokemonsToEvolve, value); }
		}
		
		public List<PokemonId> PokemonsToTransfer {
			get { return pokemonsToTransfer; }
			set { SetProperty(ref pokemonsToTransfer, value); }
		}
		
		public List<PokemonId> PokemonsToCatch {
			get { return pokemonsToCatch; }
			set { SetProperty(ref pokemonsToCatch, value); }
		}

		public bool PrioritizeIVOverCP {
			get { return prioritizeIVOverCP; }
			set { SetProperty(ref prioritizeIVOverCP, value); }
		}

		public int MaxTravelDistanceInMeters {
			get { return maxTravelDistanceInMeters; }
			set { SetProperty(ref maxTravelDistanceInMeters, value); }
		}

		public bool UseLuckyEggsWhileEvolving {
			get { return useLuckyEggsWhileEvolving; }
			set { SetProperty(ref useLuckyEggsWhileEvolving, value); }
		}

		public bool EvolveAllPokemonAboveIV {
			get { return evolveAllPokemonAboveIV; }
			set { SetProperty(ref evolveAllPokemonAboveIV, value); }
		}

		public double EvolveAboveIVValue {
			get { return evolveAboveIVValue; }
			set { SetProperty(ref evolveAboveIVValue, value); }
		}

		public bool OnlyFarmPokestops {
			get { return onlyFarmPokestops; }
			set { SetProperty(ref onlyFarmPokestops, value); }
		}
		
		public bool UseGPXPathing {
			get { return useGpxPathing; }
			set { SetProperty(ref useGpxPathing, value); }
		}
		
		public string GPXFile {
			get { return gpxFile; }
			set {
				string fileName = Path.GetFileName(value ?? String.Empty)?.SanitizeFileName() ?? String.Empty;

				if (!String.IsNullOrEmpty(fileName) && !fileName.TrimEnd().EndsWith(".gpx")) {
					fileName += ".gpx";
				}

				fileName = fileName.SanitizeFileName();
				SetProperty(ref gpxFile, fileName);
			}
		}

		public bool EnableBreaks {
			get { return enableBreaks; }
			set { SetProperty(ref enableBreaks, value); }
		}

		public int BreakAfterMinutes {
			get { return breakAfterMinutes; }
			set { SetProperty(ref breakAfterMinutes, value); }
		}

		public int BreakForMinutes {
			get { return breakForMinutes; }
			set { SetProperty(ref breakForMinutes, value); }
		}

		[JsonIgnore]
		public Statistics Statistics {
			get { return statistics; }
			set { SetProperty(ref statistics, value); }
		}

		// GMap

		[JsonIgnore]
		public GMapProvider GMapProvider {
			get { return GoogleMapProvider.Instance; }
		}
		
	}

}

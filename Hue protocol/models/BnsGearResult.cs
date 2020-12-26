using System;
using Newtonsoft.Json;

namespace HueProtocol.models
{
    public partial class BnsGearResult
    {
        [JsonProperty("weaponName")]
        public string WeaponName { get; set; }

        [JsonProperty("gem1")]
        public string Gem1 { get; set; }

        [JsonProperty("gem2")]
        public string Gem2 { get; set; }

        [JsonProperty("gem3")]
        public string Gem3 { get; set; }

        [JsonProperty("gem4")]
        public string Gem4 { get; set; }

        [JsonProperty("gem5")]
        public string Gem5 { get; set; }

        [JsonProperty("gem6")]
        public string Gem6 { get; set; }

        [JsonProperty("gem7")]
        public string Gem7 { get; set; }

        [JsonProperty("gem8")]
        public string Gem8 { get; set; }

        [JsonProperty("ringName")]
        public string RingName { get; set; }

        [JsonProperty("earringName")]
        public string EarringName { get; set; }

        [JsonProperty("necklaceName")]
        public string NecklaceName { get; set; }

        [JsonProperty("braceletName")]
        public string BraceletName { get; set; }

        [JsonProperty("beltName")]
        public string BeltName { get; set; }

        [JsonProperty("soulName")]
        public string SoulName { get; set; }

        [JsonProperty("soulName2")]
        public string SoulName2 { get; set; }

        [JsonProperty("petAuraName")]
        public string PetAuraName { get; set; }

        [JsonProperty("talismanName")]
        public string TalismanName { get; set; }

        [JsonProperty("soulBadgeName")]
        public string SoulBadgeName { get; set; }

        [JsonProperty("mysticBadgeName")]
        public string MysticBadgeName { get; set; }

        [JsonProperty("outfitName")]
        public string OutfitName { get; set; }

        [JsonProperty("clothesAccessoryName")]
        public string ClothesAccessoryName { get; set; }

        [JsonProperty("hairName")]
        public string HairName { get; set; }

        [JsonProperty("faceDecorationName")]
        public string FaceDecorationName { get; set; }

        [JsonProperty("gloves")]
        public string Gloves { get; set; }

        [JsonProperty("soulshield1")]
        public string Soulshield1 { get; set; }

        [JsonProperty("soulshield2")]
        public string Soulshield2 { get; set; }

        [JsonProperty("soulshield3")]
        public string Soulshield3 { get; set; }

        [JsonProperty("soulshield4")]
        public string Soulshield4 { get; set; }

        [JsonProperty("soulshield5")]
        public string Soulshield5 { get; set; }

        [JsonProperty("soulshield6")]
        public string Soulshield6 { get; set; }

        [JsonProperty("soulshield7")]
        public string Soulshield7 { get; set; }

        [JsonProperty("soulshield8")]
        public string Soulshield8 { get; set; }

        [JsonProperty("accountName")]
        public string AccountName { get; set; }

        [JsonProperty("characterName")]
        public string CharacterName { get; set; }

        [JsonProperty("playerClass")]
        public string PlayerClass { get; set; }

        [JsonProperty("playerLevel")]
        public long PlayerLevel { get; set; }

        [JsonProperty("playerLevelHM")]
        public long PlayerLevelHm { get; set; }

        [JsonProperty("server")]
        public string Server { get; set; }

        [JsonProperty("faction")]
        public string Faction { get; set; }

        [JsonProperty("factionRank")]
        public string FactionRank { get; set; }

        [JsonProperty("guild")]
        public string Guild { get; set; }

        [JsonProperty("characterImg")]
        public Uri CharacterImg { get; set; }

        [JsonProperty("tournamentTotalGames")]
        public long TournamentTotalGames { get; set; }

        [JsonProperty("tournamentTotalWins")]
        public long TournamentTotalWins { get; set; }

        [JsonProperty("tournamentSoloPoints")]
        public long TournamentSoloPoints { get; set; }

        [JsonProperty("tournamentSoloWins")]
        public long TournamentSoloWins { get; set; }

        [JsonProperty("tournamentSoloTier")]
        public string TournamentSoloTier { get; set; }

        [JsonProperty("tournamentTagPoints")]
        public long TournamentTagPoints { get; set; }

        [JsonProperty("tournamentTagWins")]
        public long TournamentTagWins { get; set; }

        [JsonProperty("tournamentTagTier")]
        public string TournamentTagTier { get; set; }

        [JsonProperty("otherNames")]
        public string[] OtherNames { get; set; }

        [JsonProperty("ap")]
        public long Ap { get; set; }

        [JsonProperty("ap_PvP")]
        public long ApPvP { get; set; }

        [JsonProperty("ap_boss")]
        public long ApBoss { get; set; }

        [JsonProperty("piercing")]
        public long Piercing { get; set; }

        [JsonProperty("piercingDefRate")]
        public double PiercingDefRate { get; set; }

        [JsonProperty("piercingBlockRate")]
        public double PiercingBlockRate { get; set; }

        [JsonProperty("accuracy")]
        public long Accuracy { get; set; }

        [JsonProperty("accuracyRate")]
        public double AccuracyRate { get; set; }

        [JsonProperty("crit")]
        public long Crit { get; set; }

        [JsonProperty("critRate")]
        public double CritRate { get; set; }

        [JsonProperty("critDamage")]
        public long CritDamage { get; set; }

        [JsonProperty("critDamageRate")]
        public double CritDamageRate { get; set; }

        [JsonProperty("extraDmg")]
        public long ExtraDmg { get; set; }

        [JsonProperty("extraDmgRate")]
        public long ExtraDmgRate { get; set; }

        [JsonProperty("mystic")]
        public long Mystic { get; set; }

        [JsonProperty("mysticRate")]
        public double MysticRate { get; set; }

        [JsonProperty("flame")]
        public long Flame { get; set; }

        [JsonProperty("flameRate")]
        public long FlameRate { get; set; }

        [JsonProperty("frost")]
        public long Frost { get; set; }

        [JsonProperty("frostRate")]
        public long FrostRate { get; set; }

        [JsonProperty("wind")]
        public long Wind { get; set; }

        [JsonProperty("windRate")]
        public long WindRate { get; set; }

        [JsonProperty("earth")]
        public long Earth { get; set; }

        [JsonProperty("earthRate")]
        public long EarthRate { get; set; }

        [JsonProperty("lightning")]
        public long Lightning { get; set; }

        [JsonProperty("lightningRate")]
        public long LightningRate { get; set; }

        [JsonProperty("shadow")]
        public long Shadow { get; set; }

        [JsonProperty("shadowRate")]
        public long ShadowRate { get; set; }

        [JsonProperty("debuff")]
        public long Debuff { get; set; }

        [JsonProperty("debuffRate")]
        public long DebuffRate { get; set; }

        [JsonProperty("hp")]
        public long Hp { get; set; }

        [JsonProperty("defence")]
        public long Defence { get; set; }

        [JsonProperty("defenceDmgReduction")]
        public double DefenceDmgReduction { get; set; }

        [JsonProperty("defence_PvP")]
        public long DefencePvP { get; set; }

        [JsonProperty("defenceDmgRate_PvP")]
        public double DefenceDmgRatePvP { get; set; }

        [JsonProperty("defence_boss")]
        public long DefenceBoss { get; set; }

        [JsonProperty("defenceDmgRate_boss")]
        public double DefenceDmgRateBoss { get; set; }

        [JsonProperty("evasion")]
        public long Evasion { get; set; }

        [JsonProperty("evasionRate")]
        public double EvasionRate { get; set; }

        [JsonProperty("evasionCounterRate")]
        public double EvasionCounterRate { get; set; }

        [JsonProperty("block")]
        public long Block { get; set; }

        [JsonProperty("blockDmgReduction")]
        public double BlockDmgReduction { get; set; }

        [JsonProperty("blockImproveRate")]
        public double BlockImproveRate { get; set; }

        [JsonProperty("blockRate")]
        public double BlockRate { get; set; }

        [JsonProperty("critDef")]
        public long CritDef { get; set; }

        [JsonProperty("critDefRate")]
        public double CritDefRate { get; set; }

        [JsonProperty("critDmgReduction")]
        public double CritDmgReduction { get; set; }

        [JsonProperty("dmgReduction")]
        public long DmgReduction { get; set; }

        [JsonProperty("dmgReductionRate")]
        public long DmgReductionRate { get; set; }

        [JsonProperty("regenOutOfCombat")]
        public long RegenOutOfCombat { get; set; }

        [JsonProperty("regenInCombat")]
        public long RegenInCombat { get; set; }

        [JsonProperty("recovery")]
        public long Recovery { get; set; }

        [JsonProperty("recoveryRate")]
        public double RecoveryRate { get; set; }

        [JsonProperty("debuffDef")]
        public long DebuffDef { get; set; }

        [JsonProperty("debuffDefRate")]
        public long DebuffDefRate { get; set; }

        [JsonProperty("HMAttackPoint")]
        public long HmAttackPoint { get; set; }

        [JsonProperty("HMDefencePoint")]
        public long HmDefencePoint { get; set; }

        [JsonProperty("HMAP")]
        public long Hmap { get; set; }

        [JsonProperty("HMAttackValue")]
        public long HmAttackValue { get; set; }

        [JsonProperty("HMHP")]
        public long Hmhp { get; set; }

        [JsonProperty("HMDefenceValue")]
        public long HmDefenceValue { get; set; }

        [JsonProperty("baseAP")]
        public long BaseAp { get; set; }

        [JsonProperty("baseHP")]
        public long BaseHp { get; set; }

        [JsonProperty("style")]
        public string Style { get; set; }

        [JsonProperty("bnsToolsPvEScore")]
        public long BnsToolsPvEScore { get; set; }

        [JsonProperty("bnsToolsPvPScore")]
        public long BnsToolsPvPScore { get; set; }

        [JsonProperty("region")]
        public string Region { get; set; }
    }
}


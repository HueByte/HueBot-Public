using System.Text.Json.Serialization;
using System;

namespace HueProtocol.Services.APIs.Results
{
    public struct BnsGearResult
    {
        [JsonPropertyName("weaponName")]
        public string WeaponName { get; set; }

        [JsonPropertyName("gem1")]
        public string Gem1 { get; set; }

        [JsonPropertyName("gem2")]
        public string Gem2 { get; set; }

        [JsonPropertyName("gem3")]
        public string Gem3 { get; set; }

        [JsonPropertyName("gem4")]
        public string Gem4 { get; set; }

        [JsonPropertyName("gem5")]
        public string Gem5 { get; set; }

        [JsonPropertyName("gem6")]
        public string Gem6 { get; set; }

        [JsonPropertyName("gem7")]
        public string Gem7 { get; set; }

        [JsonPropertyName("gem8")]
        public string Gem8 { get; set; }

        [JsonPropertyName("ringName")]
        public string RingName { get; set; }

        [JsonPropertyName("earringName")]
        public string EarringName { get; set; }

        [JsonPropertyName("necklaceName")]
        public string NecklaceName { get; set; }

        [JsonPropertyName("braceletName")]
        public string BraceletName { get; set; }

        [JsonPropertyName("beltName")]
        public string BeltName { get; set; }

        [JsonPropertyName("soulName")]
        public string SoulName { get; set; }

        [JsonPropertyName("soulName2")]
        public string SoulName2 { get; set; }

        [JsonPropertyName("petAuraName")]
        public string PetAuraName { get; set; }

        [JsonPropertyName("talismanName")]
        public string TalismanName { get; set; }

        [JsonPropertyName("soulBadgeName")]
        public string SoulBadgeName { get; set; }

        [JsonPropertyName("mysticBadgeName")]
        public string MysticBadgeName { get; set; }

        [JsonPropertyName("outfitName")]
        public string OutfitName { get; set; }

        [JsonPropertyName("clothesAccessoryName")]
        public string ClothesAccessoryName { get; set; }

        [JsonPropertyName("hairName")]
        public string HairName { get; set; }

        [JsonPropertyName("faceDecorationName")]
        public string FaceDecorationName { get; set; }

        [JsonPropertyName("gloves")]
        public string Gloves { get; set; }

        [JsonPropertyName("soulshield1")]
        public string Soulshield1 { get; set; }

        [JsonPropertyName("soulshield2")]
        public string Soulshield2 { get; set; }

        [JsonPropertyName("soulshield3")]
        public string Soulshield3 { get; set; }

        [JsonPropertyName("soulshield4")]
        public string Soulshield4 { get; set; }

        [JsonPropertyName("soulshield5")]
        public string Soulshield5 { get; set; }

        [JsonPropertyName("soulshield6")]
        public string Soulshield6 { get; set; }

        [JsonPropertyName("soulshield7")]
        public string Soulshield7 { get; set; }

        [JsonPropertyName("soulshield8")]
        public string Soulshield8 { get; set; }

        [JsonPropertyName("accountName")]
        public string AccountName { get; set; }

        [JsonPropertyName("characterName")]
        public string CharacterName { get; set; }

        [JsonPropertyName("playerClass")]
        public string PlayerClass { get; set; }

        [JsonPropertyName("playerLevel")]
        public long PlayerLevel { get; set; }

        [JsonPropertyName("playerLevelHM")]
        public long PlayerLevelHm { get; set; }

        [JsonPropertyName("server")]
        public string Server { get; set; }

        [JsonPropertyName("faction")]
        public string Faction { get; set; }

        [JsonPropertyName("factionRank")]
        public string FactionRank { get; set; }

        [JsonPropertyName("guild")]
        public string Guild { get; set; }

        [JsonPropertyName("characterImg")]
        public Uri CharacterImg { get; set; }

        [JsonPropertyName("tournamentTotalGames")]
        public long TournamentTotalGames { get; set; }

        [JsonPropertyName("tournamentTotalWins")]
        public long TournamentTotalWins { get; set; }

        [JsonPropertyName("tournamentSoloPoints")]
        public long TournamentSoloPoints { get; set; }

        [JsonPropertyName("tournamentSoloWins")]
        public long TournamentSoloWins { get; set; }

        [JsonPropertyName("tournamentSoloTier")]
        public string TournamentSoloTier { get; set; }

        [JsonPropertyName("tournamentTagPoints")]
        public long TournamentTagPoints { get; set; }

        [JsonPropertyName("tournamentTagWins")]
        public long TournamentTagWins { get; set; }

        [JsonPropertyName("tournamentTagTier")]
        public string TournamentTagTier { get; set; }

        [JsonPropertyName("otherNames")]
        public string[] OtherNames { get; set; }

        [JsonPropertyName("ap")]
        public long Ap { get; set; }

        [JsonPropertyName("ap_PvP")]
        public long ApPvP { get; set; }

        [JsonPropertyName("ap_boss")]
        public long ApBoss { get; set; }

        [JsonPropertyName("piercing")]
        public long Piercing { get; set; }

        [JsonPropertyName("piercingDefRate")]
        public double PiercingDefRate { get; set; }

        [JsonPropertyName("piercingBlockRate")]
        public double PiercingBlockRate { get; set; }

        [JsonPropertyName("accuracy")]
        public long Accuracy { get; set; }

        [JsonPropertyName("accuracyRate")]
        public double AccuracyRate { get; set; }

        [JsonPropertyName("crit")]
        public long Crit { get; set; }

        [JsonPropertyName("critRate")]
        public double CritRate { get; set; }

        [JsonPropertyName("critDamage")]
        public long CritDamage { get; set; }

        [JsonPropertyName("critDamageRate")]
        public double CritDamageRate { get; set; }

        [JsonPropertyName("extraDmg")]
        public long ExtraDmg { get; set; }

        [JsonPropertyName("extraDmgRate")]
        public long ExtraDmgRate { get; set; }

        [JsonPropertyName("mystic")]
        public long Mystic { get; set; }

        [JsonPropertyName("mysticRate")]
        public double MysticRate { get; set; }

        [JsonPropertyName("flame")]
        public long Flame { get; set; }

        [JsonPropertyName("flameRate")]
        public long FlameRate { get; set; }

        [JsonPropertyName("frost")]
        public long Frost { get; set; }

        [JsonPropertyName("frostRate")]
        public long FrostRate { get; set; }

        [JsonPropertyName("wind")]
        public long Wind { get; set; }

        [JsonPropertyName("windRate")]
        public long WindRate { get; set; }

        [JsonPropertyName("earth")]
        public long Earth { get; set; }

        [JsonPropertyName("earthRate")]
        public long EarthRate { get; set; }

        [JsonPropertyName("lightning")]
        public long Lightning { get; set; }

        [JsonPropertyName("lightningRate")]
        public long LightningRate { get; set; }

        [JsonPropertyName("shadow")]
        public long Shadow { get; set; }

        [JsonPropertyName("shadowRate")]
        public long ShadowRate { get; set; }

        [JsonPropertyName("debuff")]
        public long Debuff { get; set; }

        [JsonPropertyName("debuffRate")]
        public long DebuffRate { get; set; }

        [JsonPropertyName("hp")]
        public long Hp { get; set; }

        [JsonPropertyName("defence")]
        public long Defence { get; set; }

        [JsonPropertyName("defenceDmgReduction")]
        public double DefenceDmgReduction { get; set; }

        [JsonPropertyName("defence_PvP")]
        public long DefencePvP { get; set; }

        [JsonPropertyName("defenceDmgRate_PvP")]
        public double DefenceDmgRatePvP { get; set; }

        [JsonPropertyName("defence_boss")]
        public long DefenceBoss { get; set; }

        [JsonPropertyName("defenceDmgRate_boss")]
        public double DefenceDmgRateBoss { get; set; }

        [JsonPropertyName("evasion")]
        public long Evasion { get; set; }

        [JsonPropertyName("evasionRate")]
        public double EvasionRate { get; set; }

        [JsonPropertyName("evasionCounterRate")]
        public double EvasionCounterRate { get; set; }

        [JsonPropertyName("block")]
        public long Block { get; set; }

        [JsonPropertyName("blockDmgReduction")]
        public double BlockDmgReduction { get; set; }

        [JsonPropertyName("blockImproveRate")]
        public double BlockImproveRate { get; set; }

        [JsonPropertyName("blockRate")]
        public double BlockRate { get; set; }

        [JsonPropertyName("critDef")]
        public long CritDef { get; set; }

        [JsonPropertyName("critDefRate")]
        public double CritDefRate { get; set; }

        [JsonPropertyName("critDmgReduction")]
        public double CritDmgReduction { get; set; }

        [JsonPropertyName("dmgReduction")]
        public long DmgReduction { get; set; }

        [JsonPropertyName("dmgReductionRate")]
        public long DmgReductionRate { get; set; }

        [JsonPropertyName("regenOutOfCombat")]
        public long RegenOutOfCombat { get; set; }

        [JsonPropertyName("regenInCombat")]
        public long RegenInCombat { get; set; }

        [JsonPropertyName("recovery")]
        public long Recovery { get; set; }

        [JsonPropertyName("recoveryRate")]
        public double RecoveryRate { get; set; }

        [JsonPropertyName("debuffDef")]
        public long DebuffDef { get; set; }

        [JsonPropertyName("debuffDefRate")]
        public long DebuffDefRate { get; set; }

        [JsonPropertyName("HMAttackPoint")]
        public long HmAttackPoint { get; set; }

        [JsonPropertyName("HMDefencePoint")]
        public long HmDefencePoint { get; set; }

        [JsonPropertyName("HMAP")]
        public long Hmap { get; set; }

        [JsonPropertyName("HMAttackValue")]
        public long HmAttackValue { get; set; }

        [JsonPropertyName("HMHP")]
        public long Hmhp { get; set; }

        [JsonPropertyName("HMDefenceValue")]
        public long HmDefenceValue { get; set; }

        [JsonPropertyName("baseAP")]
        public long BaseAp { get; set; }

        [JsonPropertyName("baseHP")]
        public long BaseHp { get; set; }

        [JsonPropertyName("style")]
        public string Style { get; set; }

        [JsonPropertyName("bnsToolsPvEScore")]
        public long BnsToolsPvEScore { get; set; }

        [JsonPropertyName("bnsToolsPvPScore")]
        public long BnsToolsPvPScore { get; set; }

        [JsonPropertyName("region")]
        public string Region { get; set; }
    }
}
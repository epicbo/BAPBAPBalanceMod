using HarmonyLib.Tools;
using MelonLoader;

[assembly: MelonInfo(typeof(BAPBAPBalanceMod.Core), "BAPBAPBalanceMod", "0.1.0", "episkbo", null)]
[assembly: MelonGame("gg.bapbap", "BAPBAP")]

namespace BAPBAPBalanceMod
{
    public class Core : MelonMod
    {

        public static MelonLogger.Instance Log;

        public override void OnInitializeMelon()
        {
            Log = LoggerInstance;
            BalanceConfig.InitConfig();
            HarmonyFileLog.Enabled = true;
        }
    }

    public class BalanceConfig
    {

        private readonly static Dictionary<string, Dictionary<string, MelonPreferences_Category>> CfgFiles = new();

        private const string FILE_PATH = "UserData/BalanceMod/";

        public const string GENERAL = "general";
        public const string AUGMENTS = "augments";
        public const string CHARACTERS = "characters";

        private static void addConfig<T>(string file, string category, string identifier, T defaultValue, string description)
        {

            Dictionary<string, MelonPreferences_Category> categories;
            if (!CfgFiles.ContainsKey(file))
            {
                categories = new();
                CfgFiles.TryAdd(file, categories);
            } else
            {
                CfgFiles.TryGetValue(file, out categories);
            }

            MelonPreferences_Category cat;
            if (!categories.ContainsKey(category))
            {
                cat = MelonPreferences.CreateCategory(category);
                cat.SetFilePath(FILE_PATH+file+".cfg");
                categories.TryAdd(category, cat);
            } else
            {
                categories.TryGetValue(category, out cat);
            }

            if (defaultValue is float f)
            {
                // Cursed workaround to prevent floating point precision issues in the .cfg file
                cat.CreateEntry<double>(identifier, Double.Parse(f.ToString()), identifier, description);
            } else
            {
                cat.CreateEntry(identifier, defaultValue, identifier, description);
            }

            
        }

        public static T get<T>(string file, string category, string identifier)
        {

            bool fileExists = CfgFiles.TryGetValue(file, out var f);

            if (!fileExists)
                Core.Log.Error($"Invalid configuration file '{file}'");

            bool categoryExists = f.TryGetValue(category, out var cat);

            if (!categoryExists)
                Core.Log.Error($"Invalid configuration category '{category}'");

            if (typeof(T) == typeof(float))
            {
                // We store floats as double to prevent floating point precision issues, so here we have to convert them back. Not the prettiest solution
                return (T)(object)(float)cat.GetEntry<double>(identifier).Value;
            } else
            {
                return cat.GetEntry<T>(identifier).Value;
            }
        }

        public static void InitConfig()
        {
            Core.Log.Msg("Loading balance configuration");

            Directory.CreateDirectory(FILE_PATH);

            // ======================== MOD SETTINGS ========================

            addConfig<bool>(GENERAL, "General", "EnableBalanceTweaks", true, "Enable changing the stats of characters, abilities, augments and more");
            addConfig<bool>(GENERAL, "General", "EnableHiddenAugments", true, "Add more augments to the game");

            // ====================== GENERIC AUGMENTS ========================

            addConfig<int>(AUGMENTS, "Augment_Generic_Health", "MaxLevel", 2, "Maximum level of the Health augment");
            addConfig<int>(AUGMENTS, "Augment_Generic_Health", "BonusHpBase", 350, "Max HP gained from the first level of the augment");
            addConfig<int>(AUGMENTS, "Augment_Generic_Health", "BonusHpPerLevel", 350, "Max HP gained from subsequent levels of the augment");
            
            addConfig<int>(AUGMENTS, "Augment_Generic_MinorHealth", "MaxLevel", 5, "Maximum level of this augment");
            addConfig<int>(AUGMENTS, "Augment_Generic_MinorHealth", "BonusHpBase", 60, "Max HP gained from the first level of the augment");
            addConfig<int>(AUGMENTS, "Augment_Generic_MinorHealth", "BonusHpPerLevel", 60, "Max HP gained from subsequent levels of the augment");

            addConfig<int>(AUGMENTS, "Augment_Generic_AttackSpeed", "MaxLevel", 3, "Maximum level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_AttackSpeed", "BonusAttackSpeedBase", 0.4f, "Attack speed gained from the first level of the augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_AttackSpeed", "BonusAttackSpeedPerLevel", 0.4f, "Attack speed gained from subsequent levels of the augment");

            addConfig<int>(AUGMENTS, "Augment_Generic_MinorAttackSpeed", "MaxLevel", 5, "Maximum level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_MinorAttackSpeed", "BonusAttackSpeedBase", 0.07f, "Attack speed gained from the first level of the augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_MinorAttackSpeed", "BonusAttackSpeedPerLevel", 0.07f, "Attack speed gained from subsequent levels of the augment");

            addConfig<int>(AUGMENTS, "Augment_Generic_Damage", "MaxLevel", 2, "Maximum level of this augment");
            addConfig<int>(AUGMENTS, "Augment_Generic_Damage", "BonusDamageBase", 60, "Damage gained from the first level of the augment");
            addConfig<int>(AUGMENTS, "Augment_Generic_Damage", "BonusDamagePerLevel", 60, "Damage gained from subsequent levels of the augment");

            addConfig<int>(AUGMENTS, "Augment_Generic_MinorDamage", "MaxLevel", 5, "Maximum level of this augment");
            addConfig<int>(AUGMENTS, "Augment_Generic_MinorDamage", "BonusDamageBase", 15, "Damage gained from the first level of the augment");
            addConfig<int>(AUGMENTS, "Augment_Generic_MinorDamage", "BonusDamagePerLevel", 15, "Damage gained from subsequent level of the augment");

            addConfig<int>(AUGMENTS, "Augment_Generic_CooldownReduction", "MaxLevel", 1, "Maximum level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_CooldownReduction", "BonusCooldownReductionBase", 0.6f, "Cooldown reduction gained from the first level of the augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_CooldownReduction", "BonusCooldownReductionPerLevel", 0.4f, "Cooldown reduction gained from subsequent level of the augment");

            addConfig<int>(AUGMENTS, "Augment_Generic_CritChance", "MaxLevel", 1, "Maximum level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_CritChance", "BonusCritChanceBase", 0.35f, "Crit chance gained from the first level of the augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_CritChance", "BonusCritChancePerLevel", 0.25f, "Crit chance gained from subsequent level of the augment");

            addConfig<int>(AUGMENTS, "Augment_Generic_CritDamage", "MaxLevel", 2, "Maximum level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_CritDamage", "BonusCritChanceBase", 0.15f, "Crit chance gained from the first level of the augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_CritDamage", "BonusCritDamageBase", 0.35f, "Crit damage gained from the first level of the augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_CritDamage", "BonusCritChancePerLevel", 0.15f, "Crit chance gained from subsequent level of the augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_CritDamage", "BonusCritDamagePerLevel", 0.35f, "Crit damage gained from subsequent level of the augment");

            addConfig<int>(AUGMENTS, "Augment_Generic_Lifesteal", "MaxLevel", 2, "Maximum level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_Lifesteal", "BonusLifestealBase", 0.25f, "Lifesteal gained from the first level of the augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_Lifesteal", "BonusLifestealPerLevel", 0.20f, "Lifesteal gained from subsequent level of the augment");

            addConfig<int>(AUGMENTS, "Augment_Generic_MovementSpeed", "MaxLevel", 1, "Maximum level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_MovementSpeed", "BonusMovespeedBase", 0.4f, "Movement speed gained from the first level of the augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_MovementSpeed", "BonusMovespeedPerLevel", 0.3f, "Movement speed gained from subsequent level of the augment");

            // TODO fix bugged cooldown scaling 
            addConfig<int>(AUGMENTS, "Augment_Generic_StickyBomb", "MaxLevel", 2, "Maximum level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_StickyBomb", "CooldownBase", 7, "Cooldown of the bomb");
            addConfig<int>(AUGMENTS, "Augment_Generic_StickyBomb", "DamageBase", 250, "Bomb damage for the first level of the augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_StickyBomb", "DamageScalingBase", 0.8f, "Damage scaling of the bomb for the first level of the augment");
            addConfig<int>(AUGMENTS, "Augment_Generic_StickyBomb", "DamagePerLevel", 120, "Damage increase for each additional level of the augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_StickyBomb", "DamageScalingPerLevel", 0.1f, "Damage scaling increase for each additional level of the augment");

            addConfig<int>(AUGMENTS, "Augment_Generic_FireWave", "MaxLevel", 1, "Maximum level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_FireWave", "Speed", 16, "Speed of the projectile");
            addConfig<float>(AUGMENTS, "Augment_Generic_FireWave", "Duration", 0.45f, "How long the projectile exists. Range = Speed * Duration");
            addConfig<float>(AUGMENTS, "Augment_Generic_FireWave", "CooldownBase", 4.5f, "Cooldown for the first level of the augment");
            addConfig<int>(AUGMENTS, "Augment_Generic_FireWave", "DamageBase", 220, "Wave damage for the first level of the augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_FireWave", "DamageScalingBase", 0.75f, "Damage scaling for the first level of the augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_FireWave", "CooldownPerLevel", 0, "Cooldown increase for each additional level of the augment, can be negative");
            addConfig<int>(AUGMENTS, "Augment_Generic_FireWave", "DamagePerLevel", 110, "Damage increase for each additional level of the augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_FireWave", "DamageScalingPerLevel", 0.0f, "Damage scaling increase for each additional level of the augment");

            addConfig<int>(AUGMENTS, "Augment_Generic_FireWaveCooldown", "MaxLevel", 1, "Maximum level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_FireWaveCooldown", "CooldownBase", -2f, "Cooldown increase for the first level of this augment. Should be a negative number");
            addConfig<float>(AUGMENTS, "Augment_Generic_FireWaveCooldown", "CooldownPerLevel", -1f, "Cooldown increase for each additional level of this augment. Should be a negative number");

            addConfig<int>(AUGMENTS, "Augment_Generic_FireWaveBurn", "MaxLevel", 1, "Maximum level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_FireWaveBurn", "BurnMultiplier", 1, "Burn damage multiplier from fire wave. Default burn damage is 20 damage twice per second");
            addConfig<float>(AUGMENTS, "Augment_Generic_FireWaveBurn", "DurationBase", 4, "Base burn duration");
            addConfig<float>(AUGMENTS, "Augment_Generic_FireWaveBurn", "DurationPerLevel", 4, "Additional burn duration per level of this augment");

            addConfig<int>(AUGMENTS, "Augment_Generic_FireWaveRange", "MaxLevel", 1, "Maximum level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_FireWaveRange", "DurationIncreaseBase", 0.5f, "Additional life time of the fire wave projectile for the first level of the augment. Longer life time means longer range");
            addConfig<float>(AUGMENTS, "Augment_Generic_FireWaveRange", "DurationIncreasePerLevel", 0.5f, "Additional life time of the fire wave projectile for each additional level of the augment. Longer life time means longer range");

            addConfig<int>(AUGMENTS, "Augment_Generic_HealthRegen", "MaxLevel", 1, "Maximum level of this augment");
            addConfig<int>(AUGMENTS, "Augment_Generic_HealthRegen", "FlatHealthRegenBase", 0, "Flat health regeneration gained per second for the first level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_HealthRegen", "PercentHealthRegenBase", 0.04f, "Percentage of max health gained per second for the first level of this augment");
            addConfig<int>(AUGMENTS, "Augment_Generic_HealthRegen", "FlatHealthRegenPerLevel", 0, "Flat health regeneration gained per second for each additional level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_HealthRegen", "PercentHealthRegenPerLevel", 0.04f, "Percentage of max health gained per second for each additional level of this augment");

            // TODO figure out how to modify the -25 dmg and +150 hp
            addConfig<int>(AUGMENTS, "Augment_Generic_Thorns", "MaxLevel", 1, "Maximum level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_Thorns", "DamageReturnedBase", 0.2f, "Percent of damage returned to attackers for the first level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_Thorns", "DamageReturnedPerLevel", 0.15f, "Percent of damage returned for each additional level of this augment");

            addConfig<int>(AUGMENTS, "Augment_Generic_LifeForPower", "MaxLevel", 1, "Maximum level of this augment");
            addConfig<int>(AUGMENTS, "Augment_Generic_LifeForPower", "BonusHealthBase", 1000, "Bonus health gained for the first level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_LifeForPower", "BonusPercentDamageBase", -0.33f, "Percent damage gained for the first level of this augment");
            addConfig<int>(AUGMENTS, "Augment_Generic_LifeForPower", "BonusHealthPerLevel", 1000, "Bonus health gained for each additional level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_LifeForPower", "BonusPercentDamagePerLevel", -0.33f, "Percent damage gained for each additional level of this augment");

            addConfig<int>(AUGMENTS, "Augment_Generic_SlowButSteady", "MaxLevel", 1, "Maximum level of this augment");
            addConfig<int>(AUGMENTS, "Augment_Generic_SlowButSteady", "BonusHealthBase", 600, "Bonus health gained for the first level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_SlowButSteady", "BonusMovespeedBase", -0.5f, "Movement speed gained from the first level of the augment");
            addConfig<int>(AUGMENTS, "Augment_Generic_SlowButSteady", "BonusHealthPerLevel", 600, "Bonus health gained for each additional level of this augment");
            addConfig<float>(AUGMENTS, "Augment_Generic_SlowButSteady", "BonusMovespeedPerLevel", -0.5f, "Movement speed gained for each additional level of this augment");

            // ======================= ANNA ====================================

            addConfig<int>(CHARACTERS, "Character_Anna", "BaseHp", 1200, "Starting HP of Anna");
            addConfig<float>(CHARACTERS, "Character_Anna", "RunSpeed", 4.1f, "Base movement speed of Anna");
            addConfig<float>(CHARACTERS, "Character_Anna", "WalkSpeed", 3.1f, "Base movement speed of Anna when attacking");

            addConfig<float>(CHARACTERS, "Character_Anna_Basic", "CastTime", 0.075f, "Cast time of the ability");
            addConfig<float>(CHARACTERS, "Character_Anna_Basic", "Cooldown", 0.725f, "Cooldown time of the ability");
            addConfig<int>(CHARACTERS, "Character_Anna_Basic", "Damage", 50, "Damage of the ability");
            addConfig<float>(CHARACTERS, "Character_Anna_Basic", "DamageScaling", 0.21f, "A multiplier on bonus damage gained to this ability");
            addConfig<bool>(CHARACTERS, "Character_Anna_Basic", "EnableCrits", true, "Whether this ability can crit");
            addConfig<int>(CHARACTERS, "Character_Anna_Basic", "MaxBullets", 4, "Number of bullets per burst");
            addConfig<float>(CHARACTERS, "Character_Anna_Basic", "Speed", 35.4f, "Speed of the projectile");
            addConfig<float>(CHARACTERS, "Character_Anna_Basic", "Spread", 3, "Random spread of the projectiles (in radians?)");
            addConfig<float>(CHARACTERS, "Character_Anna_Basic", "Duration", 0.23f, "How long the projectile is alive. Range = Speed * Duration");

            addConfig<float>(CHARACTERS, "Character_Anna_Special", "CastTime", 0.1f, "Cast time of the ability");
            addConfig<float>(CHARACTERS, "Character_Anna_Special", "Cooldown", 7.5f, "Cooldown time of the ability");
            addConfig<int>(CHARACTERS, "Character_Anna_Special", "Damage", 85, "Damage of the ability");
            addConfig<float>(CHARACTERS, "Character_Anna_Special", "DamageScaling", 0.3f, "A multiplier on bonus damage gained to this ability");
            addConfig<bool>(CHARACTERS, "Character_Anna_Special", "EnableCrits", false, "Whether this ability can crit");
            addConfig<int>(CHARACTERS, "Character_Anna_Special", "Bullets", 3, "Number of bullets fired");
            addConfig<float>(CHARACTERS, "Character_Anna_Special", "Speed", 24f, "Speed of the projectile");
            addConfig<float>(CHARACTERS, "Character_Anna_Special", "Spread", 20, "Random spread of the projectiles in degrees");
            addConfig<float>(CHARACTERS, "Character_Anna_Special", "Duration", 0.22f, "How long the projectile is alive. Range = Speed * Duration");

            addConfig<float>(CHARACTERS, "Character_Anna_Tactical", "CastTime", 0, "Cast time of the ability");
            addConfig<float>(CHARACTERS, "Character_Anna_Tactical", "Cooldown", 6, "Cooldown time of the ability");
            addConfig<float>(CHARACTERS, "Character_Anna_Tactical", "DashSpeed", 20f, "How fast the character travels while dashing");
            addConfig<float>(CHARACTERS, "Character_Anna_Tactical", "DashTime", 0.05f, "How long the dash lasts. Distance traveled = DashTime * DashSpeed");

            addConfig<float>(CHARACTERS, "Character_Anna_Ultimate", "AreaOfEffectRadius", 0.1f, "Size of the damaging area");
            addConfig<float>(CHARACTERS, "Character_Anna_Ultimate", "CastTime", 0.1f, "Cast time of the ability");
            addConfig<float>(CHARACTERS, "Character_Anna_Ultimate", "Cooldown", 30f, "Cooldown time of the ability");
            addConfig<int>(CHARACTERS, "Character_Anna_Ultimate", "Damage", 110, "Damage of the ability per tick");
            addConfig<float>(CHARACTERS, "Character_Anna_Ultimate", "DamageRate", 0.18f, "Time between each damage tick");
            addConfig<float>(CHARACTERS, "Character_Anna_Ultimate", "DamageScaling", 0.5f, "A multiplier on bonus damage gained to this ability");
            addConfig<float>(CHARACTERS, "Character_Anna_Ultimate", "Duration", 2, "How long the ability lasts");
            addConfig<bool>(CHARACTERS, "Character_Anna_Ultimate", "EnableCrits", false, "Whether this ability can crit");

            // ======================== CHUCK ==================================

            addConfig<int>(CHARACTERS, "Character_Chuck", "BaseHp", 1600, "Starting HP of Chuck");
            addConfig<float>(CHARACTERS, "Character_Chuck", "RunSpeed", 4.3f, "Base movement speed of Chuck");
            addConfig<float>(CHARACTERS, "Character_Chuck", "WalkSpeed", 3.5f, "Base movement speed of Chuck when attacking");

            // Commented ones doesn't seem to do anything
            //addConfig<float>(CHARACTERS, "Character_Chuck_Basic", "CastTime1", 0.3f, "Cast time of the first punch in the sequence");
            //addConfig<float>(CHARACTERS, "Character_Chuck_Basic", "CastTime2", 0.3f, "Cast time of the second punch in the sequence");
            //addConfig<float>(CHARACTERS, "Character_Chuck_Basic", "CastTime3", 0.3f, "Cast time of the third punch in the sequence");
            //addConfig<float>(CHARACTERS, "Character_Chuck_Basic", "ComboResetTime", 0.5f, "Time before the combo resets if not attacking");
            addConfig<float>(CHARACTERS, "Character_Chuck_Basic", "Cooldown", 0.3f, "Cooldown time of the ability");
            addConfig<int>(CHARACTERS, "Character_Chuck_Basic", "Damage1", 145, "Damage of the first punch in the sequence");
            addConfig<int>(CHARACTERS, "Character_Chuck_Basic", "Damage2", 145, "Damage of the second punch in the sequence");
            addConfig<int>(CHARACTERS, "Character_Chuck_Basic", "Damage3", 145, "Damage of the third punch in the sequence");
            addConfig<float>(CHARACTERS, "Character_Chuck_Basic", "DamageScaling1", 0.45f, "A multiplier on bonus damage gained to the first punch in the sequence");
            addConfig<float>(CHARACTERS, "Character_Chuck_Basic", "DamageScaling2", 0.45f, "A multiplier on bonus damage gained to the second punch in the sequence");
            addConfig<float>(CHARACTERS, "Character_Chuck_Basic", "DamageScaling3", 0.45f, "A multiplier on bonus damage gained to the third punch in the sequence");
            addConfig<bool>(CHARACTERS, "Character_Chuck_Basic", "EnableCrits", true, "Whether this ability can crit");

            addConfig<float>(CHARACTERS, "Character_Chuck_Special", "CastTime", 0.4f, "Cast time of the ability");
            addConfig<float>(CHARACTERS, "Character_Chuck_Special", "Cooldown", 8f, "Cooldown time of the ability");
            addConfig<int>(CHARACTERS, "Character_Chuck_Special", "Damage", 240, "Damage of the ability");
            addConfig<int>(CHARACTERS, "Character_Chuck_Special", "DamagePerStack", 0, "Damage bonus per stack gained from Chuck's basic attack");
            addConfig<float>(CHARACTERS, "Character_Chuck_Special", "DamagePerStackScaling", 0, "Damage scaling on the bonus damage per stack gained from Chuck's basic attack");
            addConfig<float>(CHARACTERS, "Character_Chuck_Special", "DamageScaling", 0.8f, "A multiplier on bonus damage gained to this ability");
            addConfig<bool>(CHARACTERS, "Character_Chuck_Special", "EnableCrits", false, "Whether this ability can crit");
            addConfig<int>(CHARACTERS, "Character_Chuck_Special", "MaxStacks", 3, "Maximum number of stacks that can be gained from Chuck's basic attack. Must be between 1 and 5.");
            addConfig<float>(CHARACTERS, "Character_Chuck_Special", "SlowDuration", 0.5f, "How long the slow lasts for (in seconds)");
            addConfig<float>(CHARACTERS, "Character_Chuck_Special", "SlowAmountPerStack", 0.2f, "How slowed targets will be per stack");

            addConfig<float>(CHARACTERS, "Character_Chuck_Tactical", "AreaOfEffectRadius", 2.85f, "How large the area of effect is");
            addConfig<float>(CHARACTERS, "Character_Chuck_Tactical", "CastTime", 0.1f, "Cast time of the ability");
            addConfig<float>(CHARACTERS, "Character_Chuck_Tactical", "Cooldown", 11, "Cooldown time of the ability");
            addConfig<int>(CHARACTERS, "Character_Chuck_Tactical", "Damage", 250, "Damage of the ability");
            addConfig<float>(CHARACTERS, "Character_Chuck_Tactical", "DamageScaling", 0.8f, "Damage of the ability");
            addConfig<float>(CHARACTERS, "Character_Chuck_Tactical", "Distance", 7f, "How far Chuck can jump");
            addConfig<bool>(CHARACTERS, "Character_Chuck_Tactical", "EnableCrits", false, "Whether this ability can crit");
            //addConfig<float>(CHARACTERS, "Character_Chuck_Tactical", "JumpTime", 0.8f, "How long the jump lasts for"); // Changing this one seems to break the ability in weird ways
            addConfig<float>(CHARACTERS, "Character_Chuck_Tactical", "UltimateAreaOfEffectRadiusIncrease", 0.4f, "How much the area of effect radius is increased when Chuck's ultimate ability is active");
            addConfig<float>(CHARACTERS, "Character_Chuck_Tactical", "UltimateDistance", 7f, "How far Chuck can jump when Chuck's ultimate ability is active");

            addConfig<float>(CHARACTERS, "Character_Chuck_Ultimate", "CastTime", 0.33f, "Cast time of the ability");
            addConfig<float>(CHARACTERS, "Character_Chuck_Ultimate", "Cooldown", 35f, "Cooldown time of the ability");
            addConfig<float>(CHARACTERS, "Character_Chuck_Ultimate", "DamagePercentIncrease", 0.4f, "Percent damage increase while the ability is active");
            addConfig<float>(CHARACTERS, "Character_Chuck_Ultimate", "Duration", 12, "How long the ability lasts");
            addConfig<float>(CHARACTERS, "Character_Chuck_Ultimate", "HpRegenPercent", 0.04f, "How many percent of Chuck's maximum hp he regenerates per second while the ability is active");
            addConfig<float>(CHARACTERS, "Character_Chuck_Ultimate", "MovementSpeedIncrease", 0.7f, "Damage of the ability per tick");
            addConfig<float>(CHARACTERS, "Character_Chuck_Ultimate", "PercentDamageReduction", 0.15f, "Damage reduction gained while the ability is active");



            // ======================= KITSU ====================================
            addConfig<int>(CHARACTERS, "Character_Kitsu", "BaseHp", 1400, "Starting HP of Kitsu");
            addConfig<float>(CHARACTERS, "Character_Kitsu", "RunSpeed", 4.0f, "Base movement speed of Kitsu");
            addConfig<float>(CHARACTERS, "Character_Kitsu", "WalkSpeed", 0.8f, "Base movement speed of Kitsu when attacking");

            addConfig<float>(CHARACTERS, "Character_Kitsu_Basic", "CastTime", 0.2f, "Cast time of the ability");
            addConfig<float>(CHARACTERS, "Character_Kitsu_Basic", "Cooldown", 0.7f, "Cooldown time of the ability");
            addConfig<int>(CHARACTERS, "Character_Kitsu_Basic", "Damage", 200, "Damage of the ability");
            addConfig<float>(CHARACTERS, "Character_Kitsu_Basic", "DamageScaling", 0.6f, "A multiplier on bonus damage gained to this ability");
            addConfig<bool>(CHARACTERS, "Character_Kitsu_Basic", "EnableCrits", true, "Whether this ability can crit");
            addConfig<float>(CHARACTERS, "Character_Kitsu_Basic", "Speed", 30.0f, "Speed of the projectile");
            addConfig<float>(CHARACTERS, "Character_Kitsu_Basic", "Duration", 0.32f, "How long the projectile is alive. Range = Speed * Duration");

            // Ability radius and hitbox radius are disabled due to the indicator not updating
            //BALANCE_CHARACTER_KITSU_SPECIAL_ABILITY_RADIUS = CFG.Bind<float>("Character_Kitsu_Special", "Range", 10, "Range of Kitsu's special ability");
            addConfig<float>(CHARACTERS, "Character_Kitsu_Special", "CastTime", 0.3f, "Cast time of Kitsu's special attack");
            addConfig<float>(CHARACTERS, "Character_Kitsu_Special", "Cooldown", 0.65f, "Cooldown of Kitsu's special attack");
            addConfig<int>(CHARACTERS, "Character_Kitsu_Special", "Damage", 60, "Damage per hit");
            addConfig<float>(CHARACTERS, "Character_Kitsu_Special", "DamageRate", 0.5f, "How often the damage is dealt (time between pulses in seconds)");
            addConfig<float>(CHARACTERS, "Character_Kitsu_Special", "DamageScaling", 0.36f, "A multiplier on bonus damage gained to this ability");
            addConfig<bool>(CHARACTERS, "Character_Kitsu_Special", "EnableCrits", false, "Whether this ability can crit");
            //BALANCE_CHARACTER_KITSU_SPECIAL_HITBOX_RADIUS = CFG.Bind<float>("Character_Kitsu_Special", "AreaOfEffectRadius", 2, "Size of the area that deals damage");
            addConfig<float>(CHARACTERS, "Character_Kitsu_Special", "Duration", 2.0f, "How long the effect lasts");

            addConfig<float>(CHARACTERS, "Character_Kitsu_Tactical", "CastTime", 0.1f, "Cast time of the ability");
            addConfig<float>(CHARACTERS, "Character_Kitsu_Tactical", "Cooldown", 11.5f, "Cooldown of the ability");
            addConfig<int>(CHARACTERS, "Character_Kitsu_Tactical", "Damage", 80, "Damage of the ability");
            addConfig<float>(CHARACTERS, "Character_Kitsu_Tactical", "DamageScaling", 0.25f, "A multiplier on bonus damage gained to this ability");
            addConfig<bool>(CHARACTERS, "Character_Kitsu_Tactical", "EnableCrits", false, "Whether this ability can crit");
            addConfig<float>(CHARACTERS, "Character_Kitsu_Tactical", "AreaOfEffectRadius", 1.8f, "Size of area in front of Kitsu that deals damage");
            addConfig<float>(CHARACTERS, "Character_Kitsu_Tactical", "JumpDistance", 5, "How far back Kitsu jumps");

            addConfig<float>(CHARACTERS, "Character_Kitsu_Ultimate", "CastTime", 10, "Cast time of the ability");
            addConfig<float>(CHARACTERS, "Character_Kitsu_Ultimate", "Cooldown", 35, "Cooldown of the ability");
            addConfig<int>(CHARACTERS, "Character_Kitsu_Ultimate", "Damage", 500, "Damage of the ability");
            addConfig<float>(CHARACTERS, "Character_Kitsu_Ultimate", "DamageScaling", 1.25f, "A multiplier on bonus damage gained to this ability");
            addConfig<bool>(CHARACTERS, "Character_Kitsu_Ultimate", "EnableCrits", false, "Whether this ability can crit");
            addConfig<float>(CHARACTERS, "Character_Kitsu_Ultimate", "Speed", 21, "Speed of the projectile");
            addConfig<float>(CHARACTERS, "Character_Kitsu_Ultimate", "Duration", 1.7f, "How long the projectile is alive. Range = Speed * Duration");
            
        }
    }
}
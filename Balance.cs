
using HarmonyLib;
using Il2CppBAPBAP.Entities;
using Il2CppBAPBAP.Items;
using Il2CppBAPBAP.Local;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine.TextCore.Text;
using static Il2CppBAPBAP.Entities.Augment.AugmentConfiguration;

namespace BAPBAPBalanceMod
{

    [HarmonyPatch(typeof(PassiveManager), nameof(PassiveManager.PreAwake))]
    public class BalancePassivesInitializationPatch
    {

        public static Il2CppReferenceArray<TieredConfig<TierStatsBase>.Level<TierStatsBase>> CreateLevelsForStatBuff(int maxLevels, ItemStat[] statsBase, ItemStat[] statsIncrement)
        {
            maxLevels = Math.Min(Math.Max(1, maxLevels), 20);

            var newLevelsArray = new Il2CppReferenceArray<TieredConfig<TierStatsBase>.Level<TierStatsBase>>(maxLevels);

            // First level
            var level = new TieredConfig<TierStatsBase>.Level<TierStatsBase>();

            level.tiers = new Il2CppReferenceArray<TierStatsBase>(1);
            level.tiers[0] = new TierStatsBase();
            level.tiers[0].stats = new Il2CppStructArray<ItemStat>(statsBase);
            newLevelsArray[0] = level;

            // Remaining levels
            for (int i = 0; i < maxLevels - 1; i++)
            {
                level = new TieredConfig<TierStatsBase>.Level<TierStatsBase>();

                level.tiers = new Il2CppReferenceArray<TierStatsBase>(1);
                level.tiers[0] = new TierStatsBase();
                level.tiers[0].stats = new Il2CppStructArray<ItemStat>(statsIncrement);

                newLevelsArray[i + 1] = level;
            }

            return newLevelsArray;

        }

        public static Il2CppReferenceArray<TieredConfig<TTierStats>.Level<TTierStats>> CreateLevelsForSpecific<TTierStats>(
            int maxLevels,
            TTierStats tierStatsBase,
            TTierStats tierStatsIncrement)
        where TTierStats : Il2CppObjectBase
        {
            maxLevels = Math.Min(Math.Max(1, maxLevels), 20);

            var newLevelsArray = new Il2CppReferenceArray<TieredConfig<TTierStats>.Level<TTierStats>>(maxLevels);

            // First level
            var level = new TieredConfig<TTierStats>.Level<TTierStats>();

            level.tiers = new Il2CppReferenceArray<TTierStats>(1);
            level.tiers[0] = tierStatsBase;

            newLevelsArray[0] = level;

            // Remaining levels
            for (int i = 0; i < maxLevels - 1; i++)
            {
                level = new TieredConfig<TTierStats>.Level<TTierStats>();
                level.tiers = new Il2CppReferenceArray<TTierStats>(1);
                level.tiers[0] = tierStatsIncrement;

                newLevelsArray[i + 1] = level;
            }

            return newLevelsArray;
        }

        public static void Postfix(PassiveManager __instance)
        {
            Core.Log.Msg("Applying balance patch for augments...");

            // ================================================ Augments ================================================== //
            /*
            P_SpawnEntity_SO slimeBossAug = __instance.library.passives[(int)AugmentID.P_SPAWNENTITY_SLIMEBOSS].Cast<P_SpawnEntity_SO>();
            SLIME_SPAWN = slimeBossAug.configuration.entityToSpawn.gameObject;

            // Sticky Bomb
            P_OnUse_StickyBomb_SO stickyBombAug = __instance.library.passives[(int)AugmentID.P_STICKYBOMB].Cast<P_OnUse_StickyBomb_SO>();
            var tierStatsBaseStickybomb = new P_OnUse_StickyBomb.Config.TierStats
            {
                baseCooldown = Configuration.BALANCE_AUGMENT_GENERIC_STICKYBOMB_COOLDOWN.Value,
                damage = Configuration.BALANCE_AUGMENT_GENERIC_STICKYBOMB_DAMAGE_BASE.Value,
                damageScaling = Configuration.BALANCE_AUGMENT_GENERIC_STICKYBOMB_DAMAGE_SCALING_BASE.Value
            };
            var tierStatsIncrementStickybomb = new P_OnUse_StickyBomb.Config.TierStats
            {
                baseCooldown = -Configuration.BALANCE_AUGMENT_GENERIC_STICKYBOMB_COOLDOWN.Value,
                damage = Configuration.BALANCE_AUGMENT_GENERIC_STICKYBOMB_DAMAGE_INCREMENT.Value,
                damageScaling = Configuration.BALANCE_AUGMENT_GENERIC_STICKYBOMB_DAMAGE_SCALING_INCREMENT.Value
            };
            stickyBombAug.configuration.levelStats.levels = CreateLevelsForSpecific(Configuration.BALANCE_AUGMENT_GENERIC_STICKYBOMB_MAXLEVEL.Value, tierStatsBaseStickybomb, tierStatsIncrementStickybomb);

            // Firewave
            P_OnUse_SpawnFireWave_SO firewaveAug = __instance.library.passives[(int)AugmentID.P_FIREWAVE].Cast<P_OnUse_SpawnFireWave_SO>();
            firewaveAug.configuration.speed = Configuration.BALANCE_AUGMENT_GENERIC_FIREWAVE_SPEED.Value;
            firewaveAug.configuration.ttl = Configuration.BALANCE_AUGMENT_GENERIC_FIREWAVE_TTL.Value;
            var tierStatsBaseFirewave = new P_OnUse_SpawnFireWave.Config.TierStats
            {
                baseCooldown = Configuration.BALANCE_AUGMENT_GENERIC_FIREWAVE_COOLDOWN_BASE.Value,
                damage = Configuration.BALANCE_AUGMENT_GENERIC_FIREWAVE_DAMAGE_BASE.Value,
                damageScaling = Configuration.BALANCE_AUGMENT_GENERIC_FIREWAVE_DAMAGE_SCALING_BASE.Value
            };
            var tierStatsIncrementFirewave = new P_OnUse_SpawnFireWave.Config.TierStats
            {
                baseCooldown = Configuration.BALANCE_AUGMENT_GENERIC_FIREWAVE_COOLDOWN_INCREMENT.Value,
                damage = Configuration.BALANCE_AUGMENT_GENERIC_FIREWAVE_DAMAGE_INCREMENT.Value,
                damageScaling = Configuration.BALANCE_AUGMENT_GENERIC_FIREWAVE_DAMAGE_SCALING_INCREMENT.Value
            };
            firewaveAug.configuration.levelStats.levels = CreateLevelsForSpecific(Configuration.BALANCE_AUGMENT_GENERIC_FIREWAVE_MAXLEVEL.Value, tierStatsBaseFirewave, tierStatsIncrementFirewave);

            // Firewave cooldown reduction
            P_Firewave_Cooldown_SO firewaveCooldownAug = __instance.library.passives[(int)AugmentID.P_FIREWAVE_COOLDOWN].Cast<P_Firewave_Cooldown_SO>();
            var tierStatsBaseFirewaveCdr = new P_Firewave_Cooldown.Config.TierStats
            {
                cooldown = Configuration.BALANCE_AUGMENT_GENERIC_FIREWAVECOOLDOWN_BASE.Value
            };
            var tierStatsIncrementFirewaveCdr = new P_Firewave_Cooldown.Config.TierStats
            {
                cooldown = Configuration.BALANCE_AUGMENT_GENERIC_FIREWAVECOOLDOWN_INCREMENT.Value
            };
            firewaveCooldownAug.configuration.levelStats.levels = CreateLevelsForSpecific(Configuration.BALANCE_AUGMENT_GENERIC_FIREWAVECOOLDOWN_MAXLEVEL.Value, tierStatsBaseFirewaveCdr, tierStatsIncrementFirewaveCdr);

            // Firewave burn
            P_Firewave_Burn_SO firewaveBurnAug = __instance.library.passives[(int)AugmentID.P_FIREWAVE_BURN].Cast<P_Firewave_Burn_SO>();
            firewaveBurnAug.configuration.statusEffects.multiplier = Configuration.BALANCE_AUGMENT_GENERIC_FIREWAVEBURN_BURN_MULTIPLIER.Value;
            //SE_Burn_SO burnSE = firewaveBurnAug.configuration.statusEffects.statusEffect.Cast<SE_Burn_SO>();
            var tierStatsBaseFirewaveBurn = new P_Firewave_Burn.Config.TierStats
            {
                duration = Configuration.BALANCE_AUGMENT_GENERIC_FIREWAVEBURN_DURATION_BASE.Value,
            };
            var tierStatsIncrementFirewaveBurn = new P_Firewave_Burn.Config.TierStats
            {
                duration = Configuration.BALANCE_AUGMENT_GENERIC_FIREWAVEBURN_DURATION_INCREMENT.Value,
            };
            firewaveBurnAug.configuration.levelStats.levels = CreateLevelsForSpecific(Configuration.BALANCE_AUGMENT_GENERIC_FIREWAVEBURN_MAXLEVEL.Value, tierStatsBaseFirewaveBurn, tierStatsIncrementFirewaveBurn);

            // Firewave range increase
            P_Firewave_Range_SO firewaveRangeAug = __instance.library.passives[(int)AugmentID.P_FIREWAVE_RANGE].Cast<P_Firewave_Range_SO>();
            var tierStatsBaseFirewaveRange = new P_Firewave_Range.Config.TierStats
            {
                ttl = Configuration.BALANCE_AUGMENT_GENERIC_FIREWAVERANGE_BASE.Value,
            };
            var tierStatsIncrementFirewaveRange = new P_Firewave_Range.Config.TierStats
            {
                ttl = Configuration.BALANCE_AUGMENT_GENERIC_FIREWAVERANGE_INCREMENT.Value,
            };
            firewaveRangeAug.configuration.levelStats.levels = CreateLevelsForSpecific(Configuration.BALANCE_AUGMENT_GENERIC_FIREWAVERANGE_MAXLEVEL.Value, tierStatsBaseFirewaveRange, tierStatsIncrementFirewaveRange);

            // Hp regen
            P_Buff_HpRegen_SO hpRegenAug = __instance.library.passives[(int)AugmentID.P_BUFF_HPREGEN].Cast<P_Buff_HpRegen_SO>();
            var tierStatsBaseHpRegen = new P_Buff_HpRegen.Config.TierStats
            {
                flatHpAmount = Configuration.BALANCE_AUGMENT_GENERIC_HPREGEN_FLAT_BASE.Value,
                percentHpAmount = Configuration.BALANCE_AUGMENT_GENERIC_HPREGEN_PERCENT_BASE.Value,
            };
            var tierStatsIncrementHpRegen = new P_Buff_HpRegen.Config.TierStats
            {
                flatHpAmount = Configuration.BALANCE_AUGMENT_GENERIC_HPREGEN_FLAT_INCREMENT.Value,
                percentHpAmount = Configuration.BALANCE_AUGMENT_GENERIC_HPREGEN_PERCENT_INCREMENT.Value,
            };
            hpRegenAug.configuration.levelStats.levels = CreateLevelsForSpecific(Configuration.BALANCE_AUGMENT_GENERIC_HPREGEN_MAXLEVEL.Value, tierStatsBaseHpRegen, tierStatsIncrementHpRegen);

            // Thorns
            // TODO I haven't figured out how to modify the flat -25 dmg and +150 hp.
            P_Thorns_SO thornsAug = __instance.library.passives[(int)AugmentID.P_THORNS].Cast<P_Thorns_SO>();

            var tierStatsBaseThorns = new P_Thorns.Config.TierStats
            {
                thornsAmount = Configuration.BALANCE_AUGMENT_GENERIC_THORNS_BASE.Value,
            };
            var tierStatsIncremenThorns = new P_Thorns.Config.TierStats
            {
                thornsAmount = Configuration.BALANCE_AUGMENT_GENERIC_THORNS_INCREMENT.Value,
            };
            thornsAug.configuration.levelStats.levels = CreateLevelsForSpecific(Configuration.BALANCE_AUGMENT_GENERIC_THORNS_MAXLEVEL.Value, tierStatsBaseThorns, tierStatsIncremenThorns);

            // Gain HP lose Dmg
            P_Buff_HP_LoseDmg_SO gainHpLoseDmgAug = __instance.library.passives[(int)AugmentID.P_BUFF_HP_LOSEDMG].Cast<P_Buff_HP_LoseDmg_SO>();
            var tierStatsBasegainHpLoseDmg = new P_Buff_HP_LoseDmg.Config.TierStats
            {
                damagePercent = Configuration.BALANCE_AUGMENT_GENERIC_GAINHPLOSEDAMAGE_DAMAGE_BASE.Value,
                healthAmount = Configuration.BALANCE_AUGMENT_GENERIC_GAINHPLOSEDAMAGE_HP_BASE.Value,
            };
            var tierStatsIncremengainHpLoseDmg = new P_Buff_HP_LoseDmg.Config.TierStats
            {
                damagePercent = Configuration.BALANCE_AUGMENT_GENERIC_GAINHPLOSEDAMAGE_DAMAGE_INCREMENT.Value,
                healthAmount = Configuration.BALANCE_AUGMENT_GENERIC_GAINHPLOSEDAMAGE_HP_INCREMENT.Value,
            };
            gainHpLoseDmgAug.configuration.levelStats.levels = CreateLevelsForSpecific(Configuration.BALANCE_AUGMENT_GENERIC_GAINHPLOSEDAMAGE_MAXLEVEL.Value, tierStatsBasegainHpLoseDmg, tierStatsIncremengainHpLoseDmg);

            // Gain HP lose movespeed
            P_Buff_HP_LoseMS_SO gainHpLoseMsAug = __instance.library.passives[(int)AugmentID.P_BUFF_HP_LOSEMS].Cast<P_Buff_HP_LoseMS_SO>();
            var tierStatsBasegainHpLoseMs = new P_Buff_HP_LoseMS.Config.TierStats
            {
                movementSpeed = Configuration.BALANCE_AUGMENT_GENERIC_GAINHPLOSEMOVESPEED_MOVESPEED_BASE.Value,
                healthAmount = Configuration.BALANCE_AUGMENT_GENERIC_GAINHPLOSEMOVESPEED_HP_BASE.Value,
            };
            var tierStatsIncremengainHpLoseMs = new P_Buff_HP_LoseMS.Config.TierStats
            {
                movementSpeed = Configuration.BALANCE_AUGMENT_GENERIC_GAINHPLOSEMOVESPEED_MOVESPEED_INCREMENT.Value,
                healthAmount = Configuration.BALANCE_AUGMENT_GENERIC_GAINHPLOSEMOVESPEED_HP_INCREMENT.Value,
            };
            gainHpLoseMsAug.configuration.levelStats.levels = CreateLevelsForSpecific(Configuration.BALANCE_AUGMENT_GENERIC_GAINHPLOSEMOVESPEED_MAXLEVEL.Value, tierStatsBasegainHpLoseMs, tierStatsIncremengainHpLoseMs);
            /*
            P_FIREBALL = 157,
            P_ONHIT_LIGHTNING_SPRINT = 175,
            P_ONHIT_LIGHTNINGSTRIKE_ABILITIES = 177,
            P_ONHIT_LIGHTNINGSTRIKE_LMB = 179,
            P_BLOOD_EXTRADMG = 191,
            P_ONCD_BLEED = 203,
            P_GIVE_ITEM_CHAINS = 208,
            P_GIVE_ITEM_DASH = 209,
            P_GIVE_ITEM_KNOCKBACK = 213,
            P_GIVE_ITEM_PROTECTZONE = 214,
            P_GIVE_ITEM_REBOOT = 215,
            P_SPAWNENTITY_SLIMEBOSS = 217,
            P_BERSERK = 230,
            P_BUFF_WALKINGBOOTS = 238,
            P_CHARAUGMENT_CHUCK_TITANSTRADEOFF = 239,
            P_DAMAGING_BLOW = 252,
            P_EXPLOSIVECHARM = 263,
            P_FIREYCAPE = 267,
            P_GAINSIZEBYHP_GIGANTICSIZE = 271,
            P_GRAVITATIONALORBIT_GIGANTICSIZE = 274,
            P_HEALINGSKILLS = 278,
            P_HITTYWITTY = 282,
            P_JELLYSPLASH = 286,
            P_KAMIKAZE = 290,
            P_LMBFORULT = 293,
            P_LUCKYDANCE = 295,
            P_METALULT = 299,
            P_ONUSE_DMG = 303,
            P_ONUSE_LMB_PUSH_SPEED = 305,
            P_ONUSE_ULT_DMG = 307,
            P_PRIMARYCHARGING = 315,
            P_PROTECTIVEBUBBLE = 318,
            P_RAPIDFIRE = 322,
            P_RECHARNING_SHIELD = 324,
            P_SPEEDYWEEDY = 342,
            P_STYLEFACTOR = 347,
            P_TAKEDOWN_TONIC = 350,
            P_TOXICTEMPO = 353,
            P_ULTIMATEHITTYWITTY = 356,
            P_VENGEANCE = 361,
            P_WATERSHIELD = 362,
            P_WORKBENCH = 365,
            P_ZOMBIETURRETS = 369,

             */
            
            // Health
            P_AugmentStats_SO hpStatAug = __instance.library.passives[(int)AugmentID.P_STAT_HP].Cast<P_AugmentStats_SO>();
            hpStatAug.configuration.levelStats.levels = CreateLevelsForStatBuff(2, [new ItemStat(Stats.Hp, 350)], [new ItemStat(Stats.Hp, 350)]);

            // Health (Minor)
            P_AugmentStats_SO hpStatMinorAug = __instance.library.passives[(int)AugmentID.P_STAT_MINOR_HP].Cast<P_AugmentStats_SO>();
            hpStatMinorAug.configuration.levelStats.levels = CreateLevelsForStatBuff(10, [new ItemStat(Stats.Hp, 60)], [new ItemStat(Stats.Hp, 60)]);

            // Attack speed
            P_AugmentStats_SO atkSpeedStatAug = __instance.library.passives[(int)AugmentID.P_STAT_ATTACKSPEED].Cast<P_AugmentStats_SO>();
            atkSpeedStatAug.configuration.levelStats.levels = CreateLevelsForStatBuff(3, [new ItemStat(Stats.AtkSpeed, 0.4f)], [new ItemStat(Stats.AtkSpeed, 0.4f)]);

            // Attack speed (Minor)
            P_AugmentStats_SO atkSpeedStatMinorAug = __instance.library.passives[(int)AugmentID.P_STAT_MINOR_ATTACKSPEED].Cast<P_AugmentStats_SO>();
            atkSpeedStatMinorAug.configuration.levelStats.levels = CreateLevelsForStatBuff(10, [new ItemStat(Stats.AtkSpeed, 0.07f)], [new ItemStat(Stats.AtkSpeed, 0.07f)]);

            // Damage
            P_AugmentStats_SO dmgStatAug = __instance.library.passives[(int)AugmentID.P_STAT_DAMAGE].Cast<P_AugmentStats_SO>();
            dmgStatAug.configuration.levelStats.levels = CreateLevelsForStatBuff(2, [new ItemStat(Stats.Dmg, 60)], [new ItemStat(Stats.Dmg, 60)]);

            // Damage (Minor)
            P_AugmentStats_SO dmgStatMinorAug = __instance.library.passives[(int)AugmentID.P_STAT_MINOR_DAMAGE].Cast<P_AugmentStats_SO>();
            dmgStatMinorAug.configuration.levelStats.levels = CreateLevelsForStatBuff(10, [new ItemStat(Stats.Dmg, 15)], [new ItemStat(Stats.Dmg, 15)]);

            // Cooldown Reduction
            P_AugmentStats_SO cdrAug = __instance.library.passives[(int)AugmentID.P_STAT_CDR].Cast<P_AugmentStats_SO>();
            cdrAug.configuration.levelStats.levels = CreateLevelsForStatBuff(1, [new ItemStat(Stats.CooldownReduction, 0.6f)], [new ItemStat(Stats.CooldownReduction, 0.4f)]);

            // Crit chance
            P_AugmentStats_SO critChanceAug = __instance.library.passives[(int)AugmentID.P_STAT_CRITCHANCE].Cast<P_AugmentStats_SO>();
            critChanceAug.configuration.levelStats.levels = CreateLevelsForStatBuff(1, [new ItemStat(Stats.CritChance, 0.35f)], [new ItemStat(Stats.CritChance, 0.35f)]);

            // Crit damage
            P_AugmentStats_SO critDmgAug = __instance.library.passives[(int)AugmentID.P_STAT_CRITDAMAGE].Cast<P_AugmentStats_SO>();
            critDmgAug.configuration.levelStats.levels = CreateLevelsForStatBuff(2, [new ItemStat(Il2CppBAPBAP.Items.Stats.CritChance, 0.15f), new ItemStat(Stats.CritDmg, 0.35f)], [new ItemStat(Stats.CritChance, 0.15f), new ItemStat(Stats.CritDmg, 0.35f)]);

            // Damage reduction (unused?)
            P_AugmentStats_SO dmgRedAug = __instance.library.passives[(int)AugmentID.P_STAT_DAMAGEREDUCTION].Cast<P_AugmentStats_SO>();
            dmgRedAug.configuration.levelStats.levels = CreateLevelsForStatBuff(3, [new ItemStat(Il2CppBAPBAP.Items.Stats.Defense, 0.06f)], [new ItemStat(Stats.Defense, 0.05f)]);

            // Lifesteal
            P_AugmentStats_SO lifestealAug = __instance.library.passives[(int)AugmentID.P_STAT_LIFESTEAL].Cast<P_AugmentStats_SO>();
            lifestealAug.configuration.levelStats.levels = CreateLevelsForStatBuff(2, [new ItemStat(Il2CppBAPBAP.Items.Stats.Lifesteal, 0.25f)], [new ItemStat(Stats.Lifesteal, 0.20f)]);

            // Movespeed
            P_AugmentStats_SO movespeedAug = __instance.library.passives[(int)AugmentID.P_STAT_MOVESPEED].Cast<P_AugmentStats_SO>();
            movespeedAug.configuration.levelStats.levels = CreateLevelsForStatBuff(1, [new ItemStat(Il2CppBAPBAP.Items.Stats.MoveSpeed, 0.40f)], [new ItemStat(Stats.MoveSpeed, 0.30f)]);


            /* 
             * 
            P_CHARAUGMENT_ANNA_DASHOVERDRIVE = 16,
            P_CHARAUGMENT_ANNA_DIEDIEDIE = 17,
            P_CHARAUGMENT_ANNA_MACHINEGUN = 20,
            P_CHARAUGMENT_ANNA_PRECISIONSHOT = 21,
            P_ONHIT_LMB_RESETBULLETS = 22,

            P_CHARAUGMENT_BISHOP_HPSTACKS = 24,
            P_CHARAUGMENT_BISHOP_MOUTHRANGE = 25,
            P_CHARAUGMENT_BISHOP_MUNCH = 26,
            P_CHARAUGMENT_BISHOP_TETHERCOUNTER = 28,

            P_CHARAUGMENT_CHUCK_CHUCKRESET = 29,
            P_CHARAUGMENT_CHUCK_DUCKYJUMP = 30,
            P_CHARAUGMENT_CHUCK_CHUCK_HUG = 31,
            P_CHARAUGMENT_CHUCK_LOCKERSLAM = 32,
            P_CHARAUGMENT_CHUCK_RAIDBOSS = 34,

            P_CHARAUGMENT_EVE_EXPLOSIVEICEBLOCK = 36,
            P_CHARAUGMENT_EVE_FREEZINGHEAL = 37,
            P_CHARAUGMENT_EVE_ICEBLOCKSLIDE = 38,
            P_CHARAUGMENT_EVE_ICESKATING = 39,

            P_CHARAUGMENT_FROGGY_BIGSWAMP = 40,
            P_CHARAUGMENT_FROGGY_DASHEXECUTE = 41,
            P_CHARAUGMENT_FROGGY_HEALINGHOP = 42,
            P_CHARAUGMENT_FROGGY_HEALTHYLEAP = 43,
            P_CHARAUGMENT_FROGGY_HEALTHYTONGUE = 44,
            P_CHARAUGMENT_FROGGY_HIPPITYHOP = 45,
            P_CHARAUGMENT_FROGGY_LICKYWICKY = 46,
            P_CHARAUGMENT_FROGGY_TONGUEPULL = 48,

            P_CHARAUGMENT_JIRO_CONCUSS = 51,
            P_CHARAUGMENT_JIRO_DEFLECTINGKICK = 52,
            P_CHARAUGMENT_JIRO_DOORDASH = 53,
            P_CHARAUGMENT_JIRO_LMBDASH = 56,
            P_CHARAUGMENT_JIRO_TASTYFOOD = 57,

            P_CHARAUGMENT_KAT_AGGROCATS = 58,
            P_CHARAUGMENT_KAT_POLYMORPHHEAL = 59,
            P_CHARAUGMENT_KAT_QRECAST = 60,
            P_CHARAUGMENT_KAT_ULTHEALINGCATS = 61,

            P_CHARAUGMENT_KIDDO_AUTOSHIELD = 62,
            P_CHARAUGMENT_KIDDO_BLAZINGCRATOR = 63,
            P_CHARAUGMENT_KIDDO_CALAMITYMETEOR = 64,
            P_CHARAUGMENT_KIDDO_METEORSHOWER = 66,
            P_CHARAUGMENT_KIDDO_RISINGFLAME = 67,
            P_CHARAUGMENT_KIDDO_THORNSHIELD = 68,

            P_CHARAUGMENT_BOUNCEPROJECTILE = 75,
            P_CHARAUGMENT_BOUNCEPROJECTILE_KITSU_ULT = 76,
            P_CHARAUGMENT_EXPLOSIVE_JUMP = 77,
            P_CHARAUGMENT_EXTRAMILEAGE = 78,
            P_CHARAUGMENT_FOXTRAP = 79,
            P_CHARAUGMENT_KITSU_CHARGE_PULSAR = 81,
            P_CHARAUGMENT_KITSU_TRILUNARBARRAGE = 84,
            P_CHARAUGMENT_KITSU_VANISH = 86,
            P_CHARAUGMENT_KITSU_ULTTRAIL = 85,
            P_CHARAUGMENT_SCATTERSHOT_1 = 88,
            P_CHARAUGMENT_SCATTERSHOT_2 = 89,
            P_CHARAUGMENT_ULTRAVIOLET = 93,

            P_DAMAGINGATKSPEED = 94, // Stop hitting yourself?
            P_ONTAKEDMG_QDMG = 96, // Slingshot thingy?

            P_CHARAUGMENT_ROCKY_BOULDERING = 97,
            P_CHARAUGMENT_ROCKY_CHARGEDSHOT = 98,
            P_CHARAUGMENT_ROCKY_MUSCLE = 99,
            P_CHARAUGMENT_ROCKY_RECAST = 100,
            P_ONCD_MOUNTAINSHIELD = 101,

            P_AURA_SPAWN_JELLYFISH = 102,
            P_CHARAUGMENT_SASHIMI_BOUNCEROLL = 104,
            P_CHARAUGMENT_SASHIMI_EARTHQUAKE = 106,
            P_CHARAUGMENT_SASHIMI_ONCD_BUBBLEMODULE = 108,
            P_CHARAUGMENT_SASHIMI_OVERCHARGE = 109,
            P_CHARAUGMENT_SASHIMI_SHIELDROLL = 110,
            P_CHARAUGMENT_SASHIMI_SLAMSTUN = 111,
            P_CHARAUGMENT_SASHIMI_TELEPORTATIONMODULE = 112,

            P_CHARAUGMENT_SKINNY_360SCYTHE = 113,
            P_CHARAUGMENT_SKINNY_BIGSCYTHE = 114,
            P_CHARAUGMENT_SKINNY_LONGCONAMBUSH = 116,
            P_CHARAUGMENT_SKINNY_RESTEALTH = 117,
            P_CHARAUGMENT_SKINNY_SEISMICSLASH = 119,
            P_CHARAUGMENT_SKINNY_SLEEP = 120,
            P_CHARAUGMENT_SKINNY_SNEAK = 121,

            P_CHARAUGMENT_SOFIA_BURNINGDASH = 122,
            P_CHARAUGMENT_SOFIA_COMBUSTION = 123,
            P_CHARAUGMENT_SOFIA_FIREYCOUNTER = 125,
            P_CHARAUGMENT_SOFIA_FLAMEPORTAL = 126,

            P_CHARAUGMENT_TEEVEE_CLONE_DURATION = 130,
            P_CHARAUGMENT_TEEVEE_CLONEAOE = 132,
            P_CHARAUGMENT_TEEVEE_EXTRASWAP = 134,
            P_CHARAUGMENT_TEEVEE_EXTRACLONE = 141,

            P_CHARAUGMENT_ZOOK_CHARGEDSHOT = 143,
            P_CHARAUGMENT_ZOOK_MINETURRET = 146,
            P_CHARAUGMENT_ZOOK_MOREMINES = 147,
            P_CHARAUGMENT_ZOOK_PERMAHOMING = 149,

        

       
            P_CHARAUGMENT_EVE_FROSTDETONATION = 397,
            P_CHARAUGMENT_EVE_LONGSHOT = 398,
            P_CHARAUGMENT_FROGGY_EXTENDOTONGUE = 399,
            P_CHARAUGMENT_KAT_TELEPORTATIONMODULE = 400,
            P_CHARAUGMENT_SOFIA_SUPERCHARGEDPROJECTILE = 402,
            P_GIVE_ITEM_SWAPPER = 404,
            P_SPAWNENTITY_TANK = 405,
            P_DOUBLERANDOM = 407,
            P_DRONENATOR = 408,
            P_ONDAMAGE_NOODLECOMBO = 410,
            P_ONDAMAGE_THOUSANDCUTS = 412,
            P_ONTIMER_INVULNERABLE = 415,
            P_PUSHANDPULL = 417,
            P_Stat_NegativeCDR = 418
            */

            // ====================== Hidden augments ====================

            // TODO add balancing for hidden augments

            Core.Log.Msg("Done applying balance patch for augments");
        }
    }


    [HarmonyPatch(typeof(AugmentManager), nameof(AugmentManager.PreAwake))]
    public class BalanceAugmentInitializationPatch
    {

        private static List<int> genericIdsToAdd = new List<int>();
        private static List<PassiveSO> wildcardIdsToAdd = new List<PassiveSO>();
        private static List<int> allIdsToAdd = new List<int>();

        public static void addHiddenAugment(AugmentManager manager, int id, bool isGeneric, bool isWildcard)
        {
            //TODO dont be lazy and make a new array for each element added...
            /*
            var augment = BalancePassivesInitializationPatch.PASSIVE_MANAGER_INSTANCE.library.passives[id];

            var augmentTree = manager.startingAugmentTrees[0].MemberwiseClone().Cast<AugmentManager.AugmentTree>();

            augmentTree.childrenAugments = new Il2CppReferenceArray<AugmentManager.DependencyAugment>(0);
            augmentTree.augment = augment;

            manager.allAugmentIds = Util.AppendIntArray(manager.allAugmentIds, id);
            manager.genericAugmentIds = Util.AppendIntArray(manager.genericAugmentIds, id);

            manager.startingAugmentTrees = Util.AppendReferenceArray(manager.startingAugmentTrees, augmentTree);

            /*
            allIdsToAdd.Add(id);


            if (isGeneric)
                genericIdsToAdd.Add(id);

            if (isWildcard)
                wildcardIdsToAdd.Add(augment);*/
        }

        public static void Postfix(AugmentManager __instance)
        {

            
            // =================== Hidden augments ================= //

            // TODO add hidden augments?
            
            /*
            addHiddenAugment(__instance, (int)AugmentID.P_DAMAGECAP, true, true);
            addHiddenAugment(__instance, (int)AugmentID.P_FIREWAVE_NOABILITIES, true, true); 
            addHiddenAugment(__instance, (int)AugmentID.P_BLOOD_LIFESTEAL, true, true); 
            addHiddenAugment(__instance, (int)AugmentID.P_GIVE_ITEM_HPBONUS, true, true); 
            addHiddenAugment(__instance, (int)AugmentID.P_GIVE_ITEM_VIRUS, true, true); 
            addHiddenAugment(__instance, (int)AugmentID.P_ATMASBAG, true, true); 
            addHiddenAugment(__instance, (int)AugmentID.P_AURA_ORBIT, true, true); 
            addHiddenAugment(__instance, (int)AugmentID.P_CRITICALABILITY, true, true); 
            addHiddenAugment(__instance, (int)AugmentID.P_CRITSHIELD, true, true); 
            addHiddenAugment(__instance, (int)AugmentID.P_EXECUTIONER, true, true); 
            addHiddenAugment(__instance, (int)AugmentID.P_FRONTDAMAGEMITIGATE, true, true); 
            addHiddenAugment(__instance, (int)AugmentID.P_HEALINGRAIN, true, true); 
            addHiddenAugment(__instance, (int)AugmentID.P_HEAVYHITTER, true, true); 
            addHiddenAugment(__instance, (int)AugmentID.P_KNOCKBACKPILLED, true, true);
            addHiddenAugment(__instance, (int)AugmentID.P_RAGINGSPIRIT, true, true);
            addHiddenAugment(__instance, (int)AugmentID.P_SKULLSHIELD, true, true);
            addHiddenAugment(__instance, (int)AugmentID.P_TIMED_DMG, true, true);
            addHiddenAugment(__instance, (int)AugmentID.P_WINSOOSBLADE, true, true); 
            addHiddenAugment(__instance, (int)AugmentID.P_XTRABULK, true, true);
            addHiddenAugment(__instance, (int)AugmentID.P_XTRAXTRABULK, true, true);
            addHiddenAugment(__instance, (int)AugmentID.P_STAT_DAMAGEREDUCTION, true, true);
            */
        }
    }

    [HarmonyPatch(typeof(CharAbilities), nameof(CharAbilities.Start))]
    public class CharAbilitiesInitializationPatch
    {
        public static void Postfix(CharAbilities __instance)
        {
            Core.Log.Msg("Applying balance patch to characters and abilities...");

            CharacterID characterId = (CharacterID)__instance.entityManager.charId;
            EntityManager entityManager = __instance.entityManager;
            EntityMovement movement = entityManager.charMove;
            CharHurtbox hurtbox = entityManager.charHurtbox;

            if (characterId == CharacterID.KITSU)
            {
                hurtbox.baseHp = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Kitsu", "BaseHp");
                movement.baseWalkSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu", "WalkSpeed");
                movement.baseRunSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu", "RunSpeed");

                ArrowAbility kitsuBasicAbility = __instance.abilities[0].Cast<ArrowAbility>();
                ChargedArrowsAbility kitsuSpecialAbility = __instance.abilities[1].Cast<ChargedArrowsAbility>();
                RecoilArrowAbility kitsuTacticalAbility = __instance.abilities[2].Cast<RecoilArrowAbility>();
                ArrowMissileAbility kitsuUltimateAbility = __instance.abilities[3].Cast<ArrowMissileAbility>();

                kitsuBasicAbility.baseCooldownTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Basic", "Cooldown");
                kitsuBasicAbility.castingTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Basic", "CastTime");
                kitsuBasicAbility.damage = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Kitsu_Basic", "Damage");
                kitsuBasicAbility.damageScaling = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Basic", "DamageScaling");
                kitsuBasicAbility.enableCritDmg = BalanceConfig.get<bool>(BalanceConfig.CHARACTERS, "Character_Kitsu_Basic", "EnableCrits");
                kitsuBasicAbility.speed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Basic", "Speed");
                kitsuBasicAbility.ttl = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Basic", "Duration");

                kitsuSpecialAbility.baseCooldownTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Special", "Cooldown");
                kitsuSpecialAbility.castingTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Special", "CastTime");
                kitsuSpecialAbility.damage = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Kitsu_Special", "Damage");
                kitsuSpecialAbility.damageRate = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Special", "DamageRate");
                kitsuSpecialAbility.damageScaling = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Special", "DamageScaling");
                kitsuSpecialAbility.enableCritDmg = BalanceConfig.get<bool>(BalanceConfig.CHARACTERS, "Character_Kitsu_Special", "EnableCrits");
                kitsuSpecialAbility.ttl = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Special", "Duration");
                // Modifying area of effect does not change the indicator. I don't know how to fix that yet.
                //kitsuSpecialAbility.indicatorBaseHalfScale = new Vector2(Configuration.BALANCE_CHARACTER_KITSU_SPECIAL_ABILITY_RADIUS.Value, Configuration.BALANCE_CHARACTER_KITSU_SPECIAL_ABILITY_RADIUS.Value);
                //kitsuSpecialAbility.indicatorHalfScale = new Vector2(Configuration.BALANCE_CHARACTER_KITSU_SPECIAL_HITBOX_RADIUS.Value, Configuration.BALANCE_CHARACTER_KITSU_SPECIAL_HITBOX_RADIUS.Value);
                //kitsuSpecialAbility.hitboxRadius = Configuration.BALANCE_CHARACTER_KITSU_SPECIAL_HITBOX_RADIUS.Value;
                //kitsuSpecialAbility.indicatorPrefab.transform.localScale = new Vector3(10, 10, 10);
                //kitsuSpecialAbility.indicatorMaxDistance = Configuration.BALANCE_CHARACTER_KITSU_SPECIAL_ABILITY_RADIUS.Value;
                //kitsuSpecialAbility.abilityRadius = Configuration.BALANCE_CHARACTER_KITSU_SPECIAL_ABILITY_RADIUS.Value;

                kitsuTacticalAbility.baseCooldownTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Tactical", "Cooldown");
                kitsuTacticalAbility.castingTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Tactical", "CastTime");
                kitsuTacticalAbility.damage = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Kitsu_Tactical", "Damage");
                kitsuTacticalAbility.damageScaling = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Tactical", "DamageScaling");
                kitsuTacticalAbility.enableCritDmg = BalanceConfig.get<bool>(BalanceConfig.CHARACTERS, "Character_Kitsu_Tactical", "EnableCrits");
                kitsuTacticalAbility.hitboxRadius = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Tactical", "AreaOfEffectRadius");
                kitsuTacticalAbility.jumpDistance = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Tactical", "JumpDistance");

                kitsuUltimateAbility.baseCooldownTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Ultimate", "Cooldown");
                kitsuUltimateAbility.castingTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Ultimate", "CastTime"); //TODO I don't think this works?
                kitsuUltimateAbility.damage = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Kitsu_Ultimate", "Damage");
                kitsuUltimateAbility.damageScaling = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Ultimate", "DamageScaling");
                kitsuUltimateAbility.enableCritDmg = BalanceConfig.get<bool>(BalanceConfig.CHARACTERS, "Character_Kitsu_Ultimate", "EnableCrits");
                kitsuUltimateAbility.speed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Ultimate", "Speed");
                kitsuUltimateAbility.ttl = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kitsu_Ultimate", "Duration");
            }
            else if (characterId == CharacterID.ANNA) 
            {
                
                hurtbox.baseHp = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Anna", "BaseHp");
                movement.baseWalkSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna", "WalkSpeed");
                movement.baseRunSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna", "RunSpeed");

                RunNGunAbility annaBasicAbility = __instance.abilities[0].Cast<RunNGunAbility>();
                ShotgunAbility annaSpecialAbility = __instance.abilities[1].Cast<ShotgunAbility>();
                DashAbility annaTacticalAbility = __instance.abilities[2].Cast<DashAbility>();
                TornadoAbility annaUltimateAbility = __instance.abilities[3].Cast<TornadoAbility>();

                annaBasicAbility.castingTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Basic", "CastTime");
                annaBasicAbility.baseCooldownTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Basic", "Cooldown");
                annaBasicAbility.damage = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Anna_Basic", "Damage");
                annaBasicAbility.damageScaling = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Basic", "DamageScaling");
                annaBasicAbility.enableCritDmg = BalanceConfig.get<bool>(BalanceConfig.CHARACTERS, "Character_Anna_Basic", "EnableCrits");
                annaBasicAbility.maxBullets = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Anna_Basic", "MaxBullets");
                annaBasicAbility.speed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Basic", "Speed");
                annaBasicAbility.spread = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Basic", "Spread");
                annaBasicAbility.ttl = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Basic", "Duration");

                annaSpecialAbility.castingTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Special", "CastTime");
                annaSpecialAbility.baseCooldownTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Special", "Cooldown");
                annaSpecialAbility.damage = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Anna_Special", "Damage");
                annaSpecialAbility.damageScaling = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Special", "DamageScaling");
                annaSpecialAbility.enableCritDmg = BalanceConfig.get<bool>(BalanceConfig.CHARACTERS, "Character_Anna_Special", "EnableCrits");
                annaSpecialAbility.bullets = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Anna_Special", "Bullets");
                annaSpecialAbility.speed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Special", "Speed");
                annaSpecialAbility.angleSpread = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Special", "Spread");
                annaSpecialAbility.ttl = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Special", "Duration");

                annaTacticalAbility.castingTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Tactical", "CastTime");
                annaTacticalAbility.baseCooldownTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Tactical", "Cooldown");
                annaTacticalAbility.dashSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Tactical", "DashSpeed");
                annaTacticalAbility.dashTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Tactical", "DashTime");

                annaUltimateAbility.castingTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Ultimate", "CastTime");
                annaUltimateAbility.baseCooldownTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Ultimate", "Cooldown");
                annaUltimateAbility.damage = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Anna_Ultimate", "Damage");
                annaUltimateAbility.damageScaling = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Ultimate", "DamageScaling");
                annaUltimateAbility.enableCritDmg = BalanceConfig.get<bool>(BalanceConfig.CHARACTERS, "Character_Anna_Ultimate", "EnableCrits");
                annaUltimateAbility.damageRate = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Ultimate", "DamageRate");
                annaUltimateAbility.areaRadius = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Ultimate", "AreaOfEffectRadius");
                annaUltimateAbility.tornadoTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Anna_Ultimate", "Duration");

             }
            else if (characterId == CharacterID.CHUCK)
            {
                
                hurtbox.baseHp = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Chuck", "BaseHp");
                movement.baseWalkSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck", "WalkSpeed");
                movement.baseRunSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck", "RunSpeed");

                PunchSequenceAbility chuckBasicAbility = __instance.abilities[0].Cast<PunchSequenceAbility>();
                HeavyPunchAbility chuckSpecialAbility = __instance.abilities[1].Cast<HeavyPunchAbility>();
                JumpPoundAbility chuckTacticalAbility = __instance.abilities[2].Cast<JumpPoundAbility>();
                RageAbility chuckUltimateAbility = __instance.abilities[3].Cast<RageAbility>();


                // Cooldown and cast time does not seem to do anything
                //chuckBasicAbility.baseCooldownTime = BalanceConfig.get<float>("Character_Chuck_Basic", "Cooldown");
                chuckBasicAbility.damage1 = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Chuck_Basic", "Damage1");
                chuckBasicAbility.damage2 = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Chuck_Basic", "Damage2");
                chuckBasicAbility.damage3 = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Chuck_Basic", "Damage3");
                chuckBasicAbility.damage1Scaling = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Basic", "DamageScaling1");
                chuckBasicAbility.damage2Scaling = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Basic", "DamageScaling2");
                chuckBasicAbility.damage3Scaling = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Basic", "DamageScaling3");
                chuckBasicAbility.enableCritDmg = BalanceConfig.get<bool>(BalanceConfig.CHARACTERS, "Character_Chuck_Basic", "EnableCrits");

                chuckSpecialAbility.castingTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Special", "CastTime");
                chuckSpecialAbility.baseCooldownTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Special", "Cooldown");
                chuckSpecialAbility.damage = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Chuck_Special", "Damage");
                //chuckSpecialAbility.damagePerStack = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Chuck_Special", "DamagePerStack");
                //chuckSpecialAbility.damagePerStackScaling = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Special", "DamagePerStackScaling");
                chuckSpecialAbility.damageScaling = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Special", "DamageScaling"); ;
                chuckSpecialAbility.enableCritDmg = BalanceConfig.get<bool>(BalanceConfig.CHARACTERS, "Character_Chuck_Special", "EnableCrits");
                chuckSpecialAbility.maxStacks = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Chuck_Special", "MaxStacks");
                chuckSpecialAbility.slowDuration = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Special", "SlowDuration");
                chuckSpecialAbility.slowPerStack = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Special", "SlowAmountPerStack");

                // The visual indicator doesn't match the hitboxRadius
                chuckTacticalAbility.hitboxRadius = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Tactical", "AreaOfEffectRadius");
                chuckTacticalAbility.castingTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Tactical", "CastTime");
                chuckTacticalAbility.baseCooldownTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Tactical", "Cooldown");
                chuckTacticalAbility.damage = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Chuck_Tactical", "Damage");
                chuckTacticalAbility.damageScaling = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Tactical", "DamageScaling");
                chuckTacticalAbility.maxJumpDistance = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Tactical", "Distance");
                chuckTacticalAbility.rageMaxJumpDistance = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Tactical", "UltimateDistance");
                chuckTacticalAbility.rageHitboxRadiusIncrease = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Tactical", "UltimateAreaOfEffectRadiusIncrease");;
                chuckTacticalAbility.enableCritDmg = BalanceConfig.get<bool>(BalanceConfig.CHARACTERS, "Character_Chuck_Tactical", "EnableCrits"); ;

                chuckUltimateAbility.castingTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Ultimate", "CastTime");
                chuckUltimateAbility.baseCooldownTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Ultimate", "Cooldown"); ;
                chuckUltimateAbility.rageDmgPercentIncrease = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Ultimate", "DamagePercentIncrease"); ;
                chuckUltimateAbility.rageDmgReduction = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Ultimate", "PercentDamageReduction"); ;
                chuckUltimateAbility.rageHpRegenPercent = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Ultimate", "HpRegenPercent"); ;
                chuckUltimateAbility.rageSpeedIncrease = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Ultimate", "MovementSpeedIncrease"); ;
                chuckUltimateAbility.rageTime = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Chuck_Ultimate", "Duration"); ;
                /*
                Core.Log.Msg($"hp={hurtbox.baseHp}, walk={movement.baseWalkSpeed}, run={movement.baseRunSpeed}");
                
                Core.Log.Msg($"Basic ability castTime1={chuckBasicAbility.castingTime1},castTime2={chuckBasicAbility.castingTime2},castTime3={chuckBasicAbility.castingTime3},cooldown={chuckBasicAbility.baseCooldownTime},comboResetTime={chuckBasicAbility.comboResetTime},cooldown1={chuckBasicAbility.cooldownTime1},cooldown2={chuckBasicAbility.cooldownTime2},dmg1={chuckBasicAbility.damage1},dmg1Scaling={chuckBasicAbility.damage1Scaling},dmg2={chuckBasicAbility.damage2},dmg2Scaling={chuckBasicAbility.damage2Scaling},dmg3={chuckBasicAbility.damage3},dmg3Scaling={chuckBasicAbility.damage3Scaling},ttl1={chuckBasicAbility.ttl1},ttl2={chuckBasicAbility.ttl2},ttl3={chuckBasicAbility.ttl3}");
                Core.Log.Msg($"special ability dmg={chuckSpecialAbility.damage},dmgscale={chuckSpecialAbility.damageScaling},cooldown={chuckSpecialAbility.baseCooldownTime},casttime={chuckSpecialAbility.castingTime},dmgPerStack={chuckSpecialAbility.damagePerStack},dmgPerStackScaling={chuckSpecialAbility.damagePerStackScaling},maxStacks={chuckSpecialAbility.maxStacks},slowDuration={chuckSpecialAbility.slowDuration},slowPerStack={chuckSpecialAbility.slowPerStack}");
                Core.Log.Msg($"Tactical ability dmg={chuckTacticalAbility.damage}, dmgscale={chuckTacticalAbility.damageScaling},cooldown={chuckTacticalAbility.baseCooldownTime},casttime={chuckTacticalAbility.castingTime},hitboxRadius={chuckTacticalAbility.hitboxRadius}, jumpTime={chuckTacticalAbility.jumpTime},maxJumpDistance={chuckTacticalAbility.maxJumpDistance},rageHitboxRadiusIncrease={chuckTacticalAbility.rageHitboxRadiusIncrease}, rageMaxJumpdist={chuckTacticalAbility.rageMaxJumpDistance},ttl={chuckTacticalAbility.ttl},");
                Core.Log.Msg($"Ultimate ability hpRegen={chuckUltimateAbility.hpRegenAmount},pushRadius={chuckUltimateAbility.pushRadius},rageDmgPercentIncrease={chuckUltimateAbility.rageDmgPercentIncrease},rageDmgReduct={chuckUltimateAbility.rageDmgReduction},rageHpRegenPercent={chuckUltimateAbility.rageHpRegenPercent},rageSpeed={chuckUltimateAbility.rageSpeedIncrease},rageTime={chuckUltimateAbility.rageTime},cooldown={chuckUltimateAbility.baseCooldownTime},castTime={chuckUltimateAbility.castingTime}");
                */

            }
            else if (characterId == CharacterID.SASHIMI)
            {
                hurtbox.baseHp = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Sashimi", "BaseHp");
                movement.baseWalkSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Sashimi", "WalkSpeed");
                movement.baseRunSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Sashimi", "RunSpeed");

                WidePunchAbility sashimiBasicAbility = __instance.abilities[0].Cast<WidePunchAbility>();
                StunClapAbility sashimiSpecialAbility = __instance.abilities[1].Cast<StunClapAbility>();
                ChargeImpulseAbility sashimiTacticalAbility = __instance.abilities[2].Cast<ChargeImpulseAbility>();
                JumpDropAbility sashimiUltimateAbility = __instance.abilities[3].Cast<JumpDropAbility>();

            }
            else if (characterId == CharacterID.KIDDO)
            {
                hurtbox.baseHp = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Kiddo", "BaseHp");
                movement.baseWalkSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kiddo", "WalkSpeed");
                movement.baseRunSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kiddo", "RunSpeed");

                FireAttackAbility kiddoBasicAbility = __instance.abilities[0].Cast<FireAttackAbility>();
                FireStormAbility kiddoSpecialAbility = __instance.abilities[1].Cast<FireStormAbility>();
                FireShieldAbility kiddoTacticalAbility = __instance.abilities[2].Cast<FireShieldAbility>();
                FireMeteoriteAbility kiddoUltimateAbility = __instance.abilities[3].Cast<FireMeteoriteAbility>();

            }
            else if (characterId == CharacterID.ZOOK)
            {
                hurtbox.baseHp = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Zook", "BaseHp");
                movement.baseWalkSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Zook", "WalkSpeed");
                movement.baseRunSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Zook", "RunSpeed");

                RocketAbility zookBasicAbility = __instance.abilities[0].Cast<RocketAbility>();
                FollowRocketAbility zookSpecialAbility = __instance.abilities[1].Cast<FollowRocketAbility>();
                RocketJumpAbility zookTacticalAbility = __instance.abilities[2].Cast<RocketJumpAbility>();
                MineFieldAbility zookUltimateAbility = __instance.abilities[3].Cast<MineFieldAbility>();

            }
            else if (characterId == CharacterID.SKINNY)
            {
                hurtbox.baseHp = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Skinny", "BaseHp");
                movement.baseWalkSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Skinny", "WalkSpeed");
                movement.baseRunSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Skinny", "RunSpeed");

                AssassinMeleeAbility skinnyBasicAbility = __instance.abilities[0].Cast<AssassinMeleeAbility>();
                AssassinScytheHeavyAbility skinnySpecialAbility = __instance.abilities[1].Cast<AssassinScytheHeavyAbility>();
                InvisibleEscapeAbility skinnyTacticalAbility = __instance.abilities[2].Cast<InvisibleEscapeAbility>();
                FatalBlowAbility skinnyUltimateAbility = __instance.abilities[3].Cast<FatalBlowAbility>();

            }
            else if (characterId == CharacterID.FROGGY)
            {
                hurtbox.baseHp = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Froggy", "BaseHp");
                movement.baseWalkSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Froggy", "WalkSpeed");
                movement.baseRunSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Froggy", "RunSpeed");

                FroggyMeleeAbility froggyBasicAbility = __instance.abilities[0].Cast<FroggyMeleeAbility>();
                TongueJumpAbility froggySpecialAbility = __instance.abilities[1].Cast<TongueJumpAbility>();
                FroggyLeapAbility froggyTacticalAbility = __instance.abilities[2].Cast<FroggyLeapAbility>();
                StabBlinkAbility froggyUltimateAbility = __instance.abilities[3].Cast<StabBlinkAbility>();

            }
            else if (characterId == CharacterID.TEEVEE)
            {
                hurtbox.baseHp = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Teevee", "BaseHp");
                movement.baseWalkSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Teevee", "WalkSpeed");
                movement.baseRunSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Teevee", "RunSpeed");

                DigitalProjectileAbility teeveeBasicAbility = __instance.abilities[0].Cast<DigitalProjectileAbility>();
                HeavyDigitalBeamAbility teeveeSpecialAbility = __instance.abilities[1].Cast<HeavyDigitalBeamAbility>();
                DigitalCloneAbility teeveeTacticalAbility = __instance.abilities[2].Cast<DigitalCloneAbility>();
                DigitalCloneUpgradeAbility teeveeUltimateAbility = __instance.abilities[3].Cast<DigitalCloneUpgradeAbility>();


            }
            else if (characterId == CharacterID.SOFIA)
            {
                hurtbox.baseHp = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Sofia", "BaseHp");
                movement.baseWalkSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Sofia", "WalkSpeed");
                movement.baseRunSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Sofia", "RunSpeed");

                KatanaMeleeAbility sofiaBasicAbility = __instance.abilities[0].Cast<KatanaMeleeAbility>();
                ParryAbility sofiaSpecialAbility = __instance.abilities[1].Cast<ParryAbility>();
                FireyEmpoweredDashAbility sofiaTacticalAbility = __instance.abilities[2].Cast<FireyEmpoweredDashAbility>();
                FireyChargedProjectileAbility sofiaUltimateAbility = __instance.abilities[3].Cast<FireyChargedProjectileAbility>();

            }
            else if (characterId == CharacterID.JIRO)
            {
                hurtbox.baseHp = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Jiro", "BaseHp");
                movement.baseWalkSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Jiro", "WalkSpeed");
                movement.baseRunSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Jiro", "RunSpeed");

                JiroPunchAbility jiroBasicAbility = __instance.abilities[0].Cast<JiroPunchAbility>();
                JiroJumpKickAbility jiroSpecialAbility = __instance.abilities[1].Cast<JiroJumpKickAbility>();
                JiroDashKickAbility jiroTacticalAbility = __instance.abilities[2].Cast<JiroDashKickAbility>();
                JiroPushKickAbility jiroUltimateAbility = __instance.abilities[3].Cast<JiroPushKickAbility>();

            }
            else if (characterId == CharacterID.BISHOP)
            {
                hurtbox.baseHp = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Bishop", "BaseHp");
                movement.baseWalkSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Bishop", "WalkSpeed");
                movement.baseRunSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Bishop", "RunSpeed");

                SpriestTetherAbility bishopBasicAbility = __instance.abilities[0].Cast<SpriestTetherAbility>();
                SpriestSnareAbility bishopSpecialAbility = __instance.abilities[1].Cast<SpriestSnareAbility>();
                SpriestDisperseAbility bishopTacticalAbility = __instance.abilities[2].Cast<SpriestDisperseAbility>();
                SpriestExpungeAbility bishopUltimateAbility = __instance.abilities[3].Cast<SpriestExpungeAbility>();

            }
            else if (characterId == CharacterID.EVE)
            {
                hurtbox.baseHp = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Eve", "BaseHp");
                movement.baseWalkSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Eve", "WalkSpeed");
                movement.baseRunSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Eve", "RunSpeed");

                EveShardAbility eveBasicAbility = __instance.abilities[0].Cast<EveShardAbility>();
                EveSteadyShot eveSpecilAbility = __instance.abilities[1].Cast<EveSteadyShot>();
                EveIceBlockAbility eveTacticalAbility = __instance.abilities[2].Cast<EveIceBlockAbility>();
                EveFreezeAbility eveUltimateAbility = __instance.abilities[3].Cast<EveFreezeAbility>();

            }
            else if (characterId == CharacterID.KAT)
            {
                hurtbox.baseHp = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Kat", "BaseHp");
                movement.baseWalkSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kat", "WalkSpeed");
                movement.baseRunSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Kat", "RunSpeed");

                CatThrowAbility katBasicAbility = __instance.abilities[0].Cast<CatThrowAbility>();
                CatStompAbility katSpecialAbility = __instance.abilities[1].Cast<CatStompAbility>();
                CatPolymorphAbility katTacticalAbility = __instance.abilities[2].Cast<CatPolymorphAbility>();
                CatMissileAbility katMissileAbility = __instance.abilities[3].Cast<CatMissileAbility>();

            }
            else if (characterId == CharacterID.ROCKY)
            {
                hurtbox.baseHp = BalanceConfig.get<int>(BalanceConfig.CHARACTERS, "Character_Rocky", "BaseHp");
                movement.baseWalkSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Rocky", "WalkSpeed");
                movement.baseRunSpeed = BalanceConfig.get<float>(BalanceConfig.CHARACTERS, "Character_Rocky", "RunSpeed");

                RockyPunchAbility rockyBasicAbility = __instance.abilities[0].Cast<RockyPunchAbility>();
                RockyBoulderAbility rockySpecialAbility = __instance.abilities[1].Cast<RockyBoulderAbility>();
                RockyRuptureAbility rockyTacticalAbility = __instance.abilities[2].Cast<RockyRuptureAbility>();
                RockySmashAbility rockyUltimateAbility = __instance.abilities[3].Cast<RockySmashAbility>();

            }


            Core.Log.Msg("Done applying balance patch to characters and abilities");
        }
    }
}
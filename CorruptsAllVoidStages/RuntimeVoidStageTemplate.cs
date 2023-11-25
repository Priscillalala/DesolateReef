using System;
using UnityEngine;
using BepInEx;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using RoR2;
using HG;
using UnityEngine.SceneManagement;
using System.Linq;
using RoR2.Navigation;
using AK.Wwise;
using UnityEngine.Rendering.PostProcessing;
using RoR2.ExpansionManagement;
using System.Collections.Generic;
using R2API;

namespace CorruptsAllVoidStages
{
    public class RuntimeVoidStageTemplate
    {
        public PostProcessProfile ppSceneVoidStageNew;
        public Material matVoidOverhangNew;
        public Material matVoidCrystalNew;
        public DccsPool dpMonsters;
        public DirectorCardCategorySelection dccsMonsters;
        public DirectorCardCategorySelection dccsMonstersDLC1;
        public DccsPool dpInteractables;
        public DirectorCardCategorySelection dccsInteractables;
        public DirectorCardCategorySelection dccsInteractablesDLC1;

        public RuntimeVoidStageTemplate(Scene voidStage)
        {
            #region postprocessing
            PostProcessProfile ppSceneVoidStage = Addressables.LoadAssetAsync<PostProcessProfile>("RoR2/DLC1/Common/Void/ppSceneVoidStage.asset").WaitForCompletion();
            ppSceneVoidStageNew = ScriptableObject.CreateInstance<PostProcessProfile>();
            ppSceneVoidStageNew.name = "ppSceneVoidStageNew";
            foreach (PostProcessEffectSettings settings in ppSceneVoidStage.settings)
            {
                ppSceneVoidStageNew.AddSettings(UnityEngine.Object.Instantiate(settings));
            }
            if (ppSceneVoidStageNew.TryGetSettings(out RampFog rampFog))
            {
                rampFog.fogZero.Override(-0.001f);
                rampFog.fogOne.Override(0.06f);
                rampFog.fogColorStart.Override(new Color32(61, 63, 82, 0));
                rampFog.fogColorMid.Override(new Color32(66, 67, 101, 100));
                rampFog.fogColorEnd.Override(new Color32(34, 21, 56, 255));
                rampFog.skyboxStrength.Override(0.05f);
            }
            if (ppSceneVoidStageNew.TryGetSettings(out Vignette vignette))
            {
                vignette.intensity.Override(0.4f);
            }
            #endregion
            #region terrain
            matVoidOverhangNew = new Material(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/voidstage/matVoidOverhang.mat").WaitForCompletion());
            matVoidOverhangNew.SetTexture("_SnowTex", Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/Props/texSand1.png").WaitForCompletion());

            matVoidCrystalNew = new Material(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/voidstage/matVoidCrystal.mat").WaitForCompletion());
            matVoidCrystalNew.SetColor("_Color", Color.white);
            matVoidCrystalNew.SetTexture("_MainTex", Addressables.LoadAssetAsync<Texture>("RoR2/DLC1/sulfurpools/texSPCoralEmi.png").WaitForCompletion());
            matVoidCrystalNew.SetFloat("_RampInfo", 0);
            #endregion
            ExpansionDef DLC1 = Addressables.LoadAssetAsync<ExpansionDef>("RoR2/DLC1/Common/DLC1.asset").WaitForCompletion();
            #region dccsMonsters
            CharacterSpawnCard cscUnderwaterLemurian = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            cscUnderwaterLemurian.name = "cscUnderwaterLemurian";
            cscUnderwaterLemurian.prefab = Assets.UnderwaterLemurianMaster;
            cscUnderwaterLemurian.sendOverNetwork = true;
            cscUnderwaterLemurian.hullSize = HullClassification.Human;
            cscUnderwaterLemurian.nodeGraphType = MapNodeGroup.GraphType.Ground;
            cscUnderwaterLemurian.requiredFlags = NodeFlags.None;
            cscUnderwaterLemurian.forbiddenFlags = NodeFlags.NoCharacterSpawn;
            cscUnderwaterLemurian.directorCreditCost = 8;
            cscUnderwaterLemurian.occupyPosition = false;
            dccsMonsters = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();
            dccsMonsters.name = "dccsVoidStageMonstersNew";
            dccsMonsters.categories = new[]
            {
                new DirectorCardCategorySelection.Category
                {
                    name = "Basic Monsters",
                    selectionWeight = 3f,
                    cards = new[]
                    {
                        new DirectorCard 
                        {
                            spawnCard = cscUnderwaterLemurian,
                            selectionWeight = 2,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Wisp/cscLesserWisp.asset").WaitForCompletion(),
                            selectionWeight = 2,
                            preventOverhead = true,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Jellyfish/cscJellyfish.asset").WaitForCompletion(),
                            selectionWeight = 2,
                            preventOverhead = true,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/HermitCrab/cscHermitCrab.asset").WaitForCompletion(),
                            selectionWeight = 1,
                            spawnDistance = DirectorCore.MonsterSpawnDistance.Far,
                        },
                    },
                },
                new DirectorCardCategorySelection.Category
                {
                    name = "Minibosses",
                    selectionWeight = 2f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Golem/cscGolemSandy.asset").WaitForCompletion(),
                            selectionWeight = 1,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/GreaterWisp/cscGreaterWisp.asset").WaitForCompletion(),
                            selectionWeight = 1,
                            preventOverhead = true,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/MiniMushroom/cscMiniMushroom.asset").WaitForCompletion(),
                            selectionWeight = 1,
                        },
                    },
                },
                new DirectorCardCategorySelection.Category
                {
                    name = "Champions",
                    selectionWeight = 2f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Titan/cscTitanGooLake.asset").WaitForCompletion(),
                            selectionWeight = 1,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Vagrant/cscVagrant.asset").WaitForCompletion(),
                            selectionWeight = 1,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/MagmaWorm/cscMagmaWorm.asset").WaitForCompletion(),
                            selectionWeight = 1,
                        },
                    },
                },
                new DirectorCardCategorySelection.Category
                {
                    name = "Special",
                    selectionWeight = 1f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Scav/cscScav.asset").WaitForCompletion(),
                            selectionWeight = 1,
                            minimumStageCompletions = 5,
                        },
                    },
                },
            };
            #endregion
            #region dccsMonstersDLC1
            dccsMonstersDLC1 = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();
            dccsMonstersDLC1.name = "dccsVoidStageMonstersDLC1New";
            dccsMonstersDLC1.categories = new[]
            {
                new DirectorCardCategorySelection.Category
                {
                    name = "Basic Monsters",
                    selectionWeight = 3f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Wisp/cscLesserWisp.asset").WaitForCompletion(),
                            selectionWeight = 2,
                            preventOverhead = true,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Jellyfish/cscJellyfish.asset").WaitForCompletion(),
                            selectionWeight = 2,
                            preventOverhead = true,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/HermitCrab/cscHermitCrab.asset").WaitForCompletion(),
                            selectionWeight = 1,
                            spawnDistance = DirectorCore.MonsterSpawnDistance.Far,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/DLC1/AcidLarva/cscAcidLarva.asset").WaitForCompletion(),
                            selectionWeight = 2,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/DLC1/MajorAndMinorConstruct/cscMinorConstruct.asset").WaitForCompletion(),
                            selectionWeight = 2,
                            preventOverhead = true,
                            minimumStageCompletions = 3,
                        },
                    },
                },
                new DirectorCardCategorySelection.Category
                {
                    name = "Minibosses",
                    selectionWeight = 2f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Golem/cscGolemSandy.asset").WaitForCompletion(),
                            selectionWeight = 1,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/GreaterWisp/cscGreaterWisp.asset").WaitForCompletion(),
                            selectionWeight = 1,
                            preventOverhead = true,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/MiniMushroom/cscMiniMushroom.asset").WaitForCompletion(),
                            selectionWeight = 1,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/DLC1/Gup/cscGupBody.asset").WaitForCompletion(),
                            selectionWeight = 1,
                        },
                    },
                },
                new DirectorCardCategorySelection.Category
                {
                    name = "Champions",
                    selectionWeight = 2f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Titan/cscTitanGooLake.asset").WaitForCompletion(),
                            selectionWeight = 1,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Vagrant/cscVagrant.asset").WaitForCompletion(),
                            selectionWeight = 1,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/MagmaWorm/cscMagmaWorm.asset").WaitForCompletion(),
                            selectionWeight = 1,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/DLC1/MajorAndMinorConstruct/cscMegaConstruct.asset").WaitForCompletion(),
                            selectionWeight = 1,
                            preventOverhead = true,
                            minimumStageCompletions = 3,
                        },
                    },
                },
                new DirectorCardCategorySelection.Category
                {
                    name = "Special",
                    selectionWeight = 1f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Scav/cscScav.asset").WaitForCompletion(),
                            selectionWeight = 1,
                            minimumStageCompletions = 5,
                        },
                    },
                },
            };
            #endregion
            #region dpMonsters
            dpMonsters = ScriptableObject.CreateInstance<DccsPool>();
            dpMonsters.name = "dpVoidStageMonstersNew";
            dpMonsters.poolCategories = new[]
            {
                new DccsPool.Category
                {
                    name = "Standard",
                    categoryWeight = 0.98f,
                    alwaysIncluded = Array.Empty<DccsPool.PoolEntry>(),
                    includedIfConditionsMet = new[]
                    {
                        new DccsPool.ConditionalPoolEntry
                        {
                            dccs = dccsMonstersDLC1,
                            weight = 1f,
                            requiredExpansions = new[] { DLC1 },
                        },
                    },
                    includedIfNoConditionsMet = new[]
                    {
                        new DccsPool.PoolEntry
                        {
                            dccs = dccsMonsters,
                            weight = 1f,
                        },
                    },
                },
                new DccsPool.Category
                {
                    name = "Family",
                    categoryWeight = 0.02f,
                    alwaysIncluded = Array.Empty<DccsPool.PoolEntry>(),
                    includedIfConditionsMet = new[]
                    {
                        new DccsPool.ConditionalPoolEntry
                        {
                            dccs = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/Common/dccsGolemFamily.asset").WaitForCompletion(),
                            weight = 3f,
                            requiredExpansions = Array.Empty<ExpansionDef>(),
                        },
                        new DccsPool.ConditionalPoolEntry
                        {
                            dccs = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/Common/dccsGupFamily.asset").WaitForCompletion(),
                            weight = 2f,
                            requiredExpansions = new[] { DLC1 },
                        },
                        new DccsPool.ConditionalPoolEntry
                        {
                            dccs = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/Common/dccsImpFamily.asset").WaitForCompletion(),
                            weight = 1f,
                            requiredExpansions = Array.Empty<ExpansionDef>(),
                        },
                        new DccsPool.ConditionalPoolEntry
                        {
                            dccs = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/Base/Common/dccsJellyfishFamily.asset").WaitForCompletion(),
                            weight = 3f,
                            requiredExpansions = Array.Empty<ExpansionDef>(),
                        },
                    },
                    includedIfNoConditionsMet = Array.Empty<DccsPool.PoolEntry>(),
                },
                new DccsPool.Category
                {
                    name = "VoidInvasion",
                    categoryWeight = 0.02f,
                    alwaysIncluded = Array.Empty<DccsPool.PoolEntry>(),
                    includedIfConditionsMet = new[]
                    {
                        new DccsPool.ConditionalPoolEntry 
                        {
                            dccs = Addressables.LoadAssetAsync<DirectorCardCategorySelection>("RoR2/DLC1/Common/dccsVoidFamily.asset").WaitForCompletion(),
                            weight = 1f,
                            requiredExpansions = new[] { DLC1 },
                        },
                    },
                    includedIfNoConditionsMet = Array.Empty<DccsPool.PoolEntry>(),
                },
            };
            #endregion
            #region dccsInteractables
            dccsInteractables = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();
            dccsInteractables.name = "dccsVoidStageInteractablesNew";
            dccsInteractables.categories = new[]
            {
                new DirectorCardCategorySelection.Category
                {
                    name = "Chests",
                    selectionWeight = 45f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Chest1/iscChest1.asset").WaitForCompletion(),
                            selectionWeight = 24,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Chest2/iscChest2.asset").WaitForCompletion(),
                            selectionWeight = 4,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/EquipmentBarrel/iscEquipmentBarrel.asset").WaitForCompletion(),
                            selectionWeight = 2,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/TripleShop/iscTripleShop.asset").WaitForCompletion(),
                            selectionWeight = 8,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/LunarChest/iscLunarChest.asset").WaitForCompletion(),
                            selectionWeight = 1,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/TripleShopLarge/iscTripleShopLarge.asset").WaitForCompletion(),
                            selectionWeight = 1,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/CategoryChest/iscCategoryChestDamage.asset").WaitForCompletion(),
                            selectionWeight = 2,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/CategoryChest/iscCategoryChestHealing.asset").WaitForCompletion(),
                            selectionWeight = 2,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/CategoryChest/iscCategoryChestUtility.asset").WaitForCompletion(),
                            selectionWeight = 2,
                        },
                    },
                },
                new DirectorCardCategorySelection.Category
                {
                    name = "Barrels",
                    selectionWeight = 10f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Barrel1/iscBarrel1.asset").WaitForCompletion(),
                            selectionWeight = 10,
                        },
                    },
                },
                new DirectorCardCategorySelection.Category
                {
                    name = "Shrines",
                    selectionWeight = 8f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/ShrineChance/iscShrineChance.asset").WaitForCompletion(),
                            selectionWeight = 40,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/ShrineCombat/iscShrineCombat.asset").WaitForCompletion(),
                            selectionWeight = 20,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/ShrineBoss/iscShrineBoss.asset").WaitForCompletion(),
                            selectionWeight = 10,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/ShrineCleanse/iscShrineCleanse.asset").WaitForCompletion(),
                            selectionWeight = 3,
                        },
                    },
                },
                new DirectorCardCategorySelection.Category
                {
                    name = "Drones",
                    selectionWeight = 3f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Drones/iscBrokenEmergencyDrone.asset").WaitForCompletion(),
                            selectionWeight = 3,
                        },
                    },
                },
                new DirectorCardCategorySelection.Category
                {
                    name = "Misc",
                    selectionWeight = 6f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Drones/iscBrokenTurret1.asset").WaitForCompletion(),
                            selectionWeight = 6,
                        },
                    },
                },
                new DirectorCardCategorySelection.Category
                {
                    name = "Rare",
                    selectionWeight = 0.4f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Chest1StealthedVariant/iscChest1Stealthed.asset").WaitForCompletion(),
                            selectionWeight = 6,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/RadarTower/iscRadarTower.asset").WaitForCompletion(),
                            selectionWeight = 2,
                            forbiddenUnlockableDef = Addressables.LoadAssetAsync<UnlockableDef>("RoR2/DLC1/voidstage/Logs.Stages.voidstage.asset").WaitForCompletion()
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/ShrineGoldshoresAccess/iscShrineGoldshoresAccess.asset").WaitForCompletion(),
                            selectionWeight = 2,
                            minimumStageCompletions = 1,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/GoldChest/iscGoldChest.asset").WaitForCompletion(),
                            selectionWeight = 2,
                            minimumStageCompletions = 3,
                        },
                    },
                },
                new DirectorCardCategorySelection.Category
                {
                    name = "Duplicator",
                    selectionWeight = 8f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Duplicator/iscDuplicator.asset").WaitForCompletion(),
                            selectionWeight = 30,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/DuplicatorLarge/iscDuplicatorLarge.asset").WaitForCompletion(),
                            selectionWeight = 6,
                            minimumStageCompletions = 1,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/DuplicatorMilitary/iscDuplicatorMilitary.asset").WaitForCompletion(),
                            selectionWeight = 1,
                            minimumStageCompletions = 4,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/DuplicatorWild/iscDuplicatorWild.asset").WaitForCompletion(),
                            selectionWeight = 2,
                            minimumStageCompletions = 1,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Scrapper/iscScrapper.asset").WaitForCompletion(),
                            selectionWeight = 12,
                        },
                    },
                },
            };
            #endregion
            #region dccsInteractablesDLC1
            dccsInteractablesDLC1 = ScriptableObject.CreateInstance<DirectorCardCategorySelection>();
            dccsInteractablesDLC1.name = "dccsVoidStageInteractablesDLC1New";
            dccsInteractablesDLC1.categories = new[]
            {
                new DirectorCardCategorySelection.Category
                {
                    name = "Chests",
                    selectionWeight = 45f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Chest1/iscChest1.asset").WaitForCompletion(),
                            selectionWeight = 240,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Chest2/iscChest2.asset").WaitForCompletion(),
                            selectionWeight = 37,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/EquipmentBarrel/iscEquipmentBarrel.asset").WaitForCompletion(),
                            selectionWeight = 20,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/TripleShop/iscTripleShop.asset").WaitForCompletion(),
                            selectionWeight = 80,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/LunarChest/iscLunarChest.asset").WaitForCompletion(),
                            selectionWeight = 10,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/TripleShopLarge/iscTripleShopLarge.asset").WaitForCompletion(),
                            selectionWeight = 10,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/CategoryChest/iscCategoryChestDamage.asset").WaitForCompletion(),
                            selectionWeight = 20,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/CategoryChest/iscCategoryChestHealing.asset").WaitForCompletion(),
                            selectionWeight = 20,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/CategoryChest/iscCategoryChestUtility.asset").WaitForCompletion(),
                            selectionWeight = 20,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/DLC1/CategoryChest2/iscCategoryChest2Healing.asset").WaitForCompletion(),
                            selectionWeight = 3,
                        },
                    },
                },
                new DirectorCardCategorySelection.Category
                {
                    name = "Barrels",
                    selectionWeight = 10f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Barrel1/iscBarrel1.asset").WaitForCompletion(),
                            selectionWeight = 10,
                        },
                    },
                },
                new DirectorCardCategorySelection.Category
                {
                    name = "Shrines",
                    selectionWeight = 8f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/ShrineChance/iscShrineChance.asset").WaitForCompletion(),
                            selectionWeight = 40,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/ShrineCombat/iscShrineCombat.asset").WaitForCompletion(),
                            selectionWeight = 20,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/ShrineBoss/iscShrineBoss.asset").WaitForCompletion(),
                            selectionWeight = 10,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/ShrineCleanse/iscShrineCleanse.asset").WaitForCompletion(),
                            selectionWeight = 3,
                        },
                    },
                },
                new DirectorCardCategorySelection.Category
                {
                    name = "Drones",
                    selectionWeight = 3f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Drones/iscBrokenEmergencyDrone.asset").WaitForCompletion(),
                            selectionWeight = 3,
                        },
                    },
                },
                new DirectorCardCategorySelection.Category
                {
                    name = "Misc",
                    selectionWeight = 6f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Drones/iscBrokenTurret1.asset").WaitForCompletion(),
                            selectionWeight = 6,
                        },
                    },
                },
                new DirectorCardCategorySelection.Category
                {
                    name = "Rare",
                    selectionWeight = 0.4f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Chest1StealthedVariant/iscChest1Stealthed.asset").WaitForCompletion(),
                            selectionWeight = 6,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/RadarTower/iscRadarTower.asset").WaitForCompletion(),
                            selectionWeight = 2,
                            forbiddenUnlockableDef = Addressables.LoadAssetAsync<UnlockableDef>("RoR2/DLC1/voidstage/Logs.Stages.voidstage.asset").WaitForCompletion()
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/ShrineGoldshoresAccess/iscShrineGoldshoresAccess.asset").WaitForCompletion(),
                            selectionWeight = 2,
                            minimumStageCompletions = 1,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/GoldChest/iscGoldChest.asset").WaitForCompletion(),
                            selectionWeight = 2,
                            minimumStageCompletions = 3,
                        },
                    },
                },
                new DirectorCardCategorySelection.Category
                {
                    name = "Duplicator",
                    selectionWeight = 8f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Duplicator/iscDuplicator.asset").WaitForCompletion(),
                            selectionWeight = 30,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/DuplicatorLarge/iscDuplicatorLarge.asset").WaitForCompletion(),
                            selectionWeight = 6,
                            minimumStageCompletions = 1,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/DuplicatorMilitary/iscDuplicatorMilitary.asset").WaitForCompletion(),
                            selectionWeight = 1,
                            minimumStageCompletions = 4,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/DuplicatorWild/iscDuplicatorWild.asset").WaitForCompletion(),
                            selectionWeight = 2,
                            minimumStageCompletions = 1,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Scrapper/iscScrapper.asset").WaitForCompletion(),
                            selectionWeight = 12,
                        },
                    },
                },
                new DirectorCardCategorySelection.Category
                {
                    name = "Void Stuff",
                    selectionWeight = 3f,
                    cards = new[]
                    {
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/DLC1/VoidChest/iscVoidChest.asset").WaitForCompletion(),
                            selectionWeight = 15,
                        },
                        new DirectorCard
                        {
                            spawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/DLC1/VoidCamp/iscVoidCamp.asset").WaitForCompletion(),
                            selectionWeight = 7,
                            minimumStageCompletions = 1,
                        },
                    },
                },
            };
            #endregion
            #region dpInteractables
            dpInteractables = ScriptableObject.CreateInstance<DccsPool>();
            dpInteractables.name = "dpVoidStageMonstersNew";
            dpInteractables.poolCategories = new[]
            {
                new DccsPool.Category
                {
                    name = "Standard",
                    categoryWeight = 1f,
                    alwaysIncluded = Array.Empty<DccsPool.PoolEntry>(),
                    includedIfConditionsMet = new[]
                    {
                        new DccsPool.ConditionalPoolEntry
                        {
                            dccs = dccsInteractablesDLC1,
                            weight = 1f,
                            requiredExpansions = new[] { DLC1 },
                        },
                    },
                    includedIfNoConditionsMet = new[]
                    {
                        new DccsPool.PoolEntry
                        {
                            dccs = dccsInteractables,
                            weight = 1f,
                        },
                    },
                },
            };
            #endregion
        }

        public void Apply(Scene voidStage)
        {
            Physics.gravity = Vector3.down * 20f;
            Dictionary<string, GameObject> rootObjects = new Dictionary<string, GameObject>();
            foreach (GameObject rootObject in voidStage.GetRootGameObjects())
            {
                rootObjects[rootObject.name] = rootObject;
            }
            if (rootObjects.TryGetValue("SceneInfo", out GameObject sceneInfo))
            {
                if (sceneInfo.TryGetComponent(out ClassicStageInfo classicStageInfo))
                {
                    classicStageInfo.monsterDccsPool = dpMonsters;
                    classicStageInfo.interactableDccsPool = dpInteractables;
                    classicStageInfo.sceneDirectorInteractibleCredits = 280;
                    classicStageInfo.sceneDirectorMonsterCredits = 100;
                    classicStageInfo.monsterCategories = dccsMonsters;
                    classicStageInfo.interactableCategories = dccsInteractables;
                }
                if (sceneInfo.TryGetComponent(out AkAmbient ambient))
                {
                    ambient.data.ObjectReference = Addressables.LoadAssetAsync<WwiseObjectReference>("Wwise/3CC8F911-9CA7-416B-A6E9-9748E4CEA659.asset").WaitForCompletion();
                }
            }
            if (rootObjects.TryGetValue("MissionController", out GameObject missionController))
            {
                if (missionController.TryGetComponent(out VoidStageMissionController voidStageMissionController))
                {
                    voidStageMissionController.enabled = false;
                }
                if (missionController.TryGetComponent(out GenericObjectiveProvider objectiveProvider))
                {
                    objectiveProvider.enabled = false;
                }
            }
            if (rootObjects.TryGetValue("Director", out GameObject director))
            {
                if (director.TryGetComponent(out SceneDirector sceneDirector))
                {
                    sceneDirector.teleporterSpawnCard = Addressables.LoadAssetAsync<SpawnCard>("RoR2/Base/Teleporters/iscTeleporter.asset").WaitForCompletion();
                }
                foreach (CombatDirector combatDirector in director.GetComponents<CombatDirector>())
                {
                    combatDirector.teamIndex = TeamIndex.Monster;
                }
            }
            Material matVoidCrystal = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/voidstage/matVoidCrystal.mat").WaitForCompletion();
            if (rootObjects.TryGetValue("HOLDER: Terrain", out GameObject terrain))
            {
                if (terrain.transform.TryFind("OLDTERRAIN", out Transform oldTerrain))
                {
                    oldTerrain.gameObject.SetActive(true);
                    foreach (Transform child in oldTerrain.AllChildren())
                    {
                        child.gameObject.SetActive(false);
                    }
                    if (oldTerrain.transform.TryFind("meshVoidOutterTerrain", out Transform outerTerrainMesh))
                    {
                        outerTerrainMesh.gameObject.SetActive(true);
                        outerTerrainMesh.localScale *= 1.3f;
                    }
                    if (oldTerrain.transform.TryFind("meshVoidDistantProp", out Transform distantPropsMesh))
                    {
                        distantPropsMesh.gameObject.SetActive(true);
                        distantPropsMesh.localScale *= 1.5f;
                    }
                    if (oldTerrain.transform.TryFind("meshVoidButress", out Transform voidButress))
                    {
                        voidButress.gameObject.SetActive(true);
                        Material matVoidTrim = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/voidstage/matVoidTrim.mat").WaitForCompletion();
                        
                        void CreateButressFace(Vector3 localPosition, Vector3 localEulerAngles, Vector2 scale)
                        {
                            GameObject face = GameObject.CreatePrimitive(PrimitiveType.Plane);
                            face.layer = LayerIndex.world.intVal;
                            face.GetComponent<MeshRenderer>().sharedMaterial = matVoidTrim;
                            face.transform.parent = voidButress;
                            face.transform.localPosition = localPosition;
                            face.transform.localEulerAngles = localEulerAngles;
                            face.transform.localScale = new Vector3(scale.x, 1, scale.y);
                        }

                        CreateButressFace(new Vector3(-167.61f, -206.7f, 83.8f), new Vector3(337, 25, 310), new Vector2(0.595f, 0.57f));
                        CreateButressFace(new Vector3(-172f, -220.6f, 81f), new Vector3(353, 355.3f, 335), new Vector2(0.7f, 0.7f));
                        CreateButressFace(new Vector3(-168.8f, -236.4f, 68.4f), new Vector3(0, 315, 345), new Vector2(0.7f, 0.7f));
                        CreateButressFace(new Vector3(-133.4f, -251.5f, 37f), new Vector3(345, 11, 13), new Vector2(0.7f, 0.7f));
                    }
                    if (oldTerrain.transform.TryFind("meshVoidCenterStatue", out Transform voidCenterStatue))
                    {
                        voidCenterStatue.gameObject.SetActive(true);
                    }
                    if (oldTerrain.transform.TryFind("meshVoidCenterStatue (1)", out Transform voidCenterStatue1))
                    {
                        voidCenterStatue1.gameObject.SetActive(true);
                    }
                    if (oldTerrain.transform.TryFind("meshVoidCenterStatue (2)", out Transform voidCenterStatue2))
                    {
                        voidCenterStatue2.gameObject.SetActive(true);
                    }
                }
                if (terrain.transform.TryFind("Revamped Hopoo Terrain", out Transform revampedTerrain))
                {
                    Material matVoidTerrain = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/voidstage/matVoidTerrain.mat").WaitForCompletion();
                    Material matVoidMetalTrimGrassyVertexColorsOnly = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/voidstage/matVoidMetalTrimGrassyVertexColorsOnly.mat").WaitForCompletion();
                    Material matVoidFoam = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/voidstage/matVoidFoam.mat").WaitForCompletion();
                    foreach (MeshRenderer meshRenderer in revampedTerrain.GetComponentsInChildren<MeshRenderer>(false))
                    {
                        
                        if (meshRenderer.gameObject.name == "mdlVoidWatcher")
                        {
                            meshRenderer.gameObject.SetActive(false);
                        }
                        else if (meshRenderer.gameObject.name.StartsWith("mdlVoidWater"))
                        {
                            meshRenderer.gameObject.SetActive(false);
                        }
                        else if (meshRenderer.sharedMaterial == matVoidFoam)
                        {
                            meshRenderer.gameObject.SetActive(false);
                        }
                        else if (Array.IndexOf(meshRenderer.sharedMaterials, matVoidFoam) > 0)
                        {
                            meshRenderer.SetSharedMaterials(meshRenderer.sharedMaterials.Where(x => x != matVoidFoam).ToList());
                        }
                        else if (meshRenderer.gameObject.name == "meshVoidBubbleCoral")
                        {
                            if (meshRenderer.transform.parent.TryFind("VoidBubble", out Transform voidBubble))
                            {
                                voidBubble.Find("Point Light")?.gameObject.SetActive(false);
                                voidBubble.Find("Bubble")?.gameObject.SetActive(true);
                            }
                        }
                        else if (meshRenderer.gameObject.name == "meshVoidOverhangBottom (2)")
                        {
                            meshRenderer.sharedMaterial = matVoidOverhangNew;
                            if (meshRenderer.transform.TryFind("VoidBubble/Point Light", out Transform pointLight) && pointLight.TryGetComponent(out Light light))
                            {
                                light.color = new Color32(201, 206, 221, 255);
                            }
                        }
                        else if (meshRenderer.sharedMaterial == matVoidTerrain)
                        {
                            meshRenderer.sharedMaterial = Assets.matVoidTerrainNew;
                        }
                        else if (meshRenderer.sharedMaterial == matVoidMetalTrimGrassyVertexColorsOnly)
                        {
                            meshRenderer.sharedMaterial = Assets.matVoidMetalOvergrown;
                        }
                        else if (meshRenderer.sharedMaterial == matVoidCrystal)
                        {
                            meshRenderer.sharedMaterial = matVoidCrystalNew;
                        }
                    }
                    if (revampedTerrain.transform.TryFind("Islands", out Transform islands))
                    {
                        islands.transform.Find("mdlVoidStageIslandMainCore/mdlVoidTerrainMain/mdlVoidMetalSpiralWalkway")?.gameObject.SetActive(false);
                        if (islands.transform.TryFind("mdlVoidStageIslandNE", out Transform mdlVoidStageIslandNE))
                        {
                            mdlVoidStageIslandNE.transform.Find("mdlVoidTerrainNE/mdlVoidSupportPlatformFloating")?.gameObject.SetActive(false);
                            mdlVoidStageIslandNE.transform.Find("mdlVoidTerrainNE/mdlVoidSupportPlatform.002/mdlVoidRepairsPlatformWalls.001")?.gameObject.SetActive(false);
                            mdlVoidStageIslandNE.transform.Find("mdlVoidTerrainNE/mdlGravityBoostPath.001")?.gameObject.SetActive(false);
                            mdlVoidStageIslandNE.transform.Find("mdlVoidTerrainNE/mdlGravityCollarGenericBase.001")?.gameObject.SetActive(false);
                        }
                        if (islands.transform.TryFind("mdlVoidStageIslandSE", out Transform mdlVoidStageIslandSE))
                        {
                            mdlVoidStageIslandSE.transform.Find("mdlVoidTerrainSE/mdlVoidArchEntry")?.gameObject.SetActive(false);
                            mdlVoidStageIslandSE.transform.Find("mdlVoidTerrainSE/mdlVoidRepairsPlatform")?.gameObject.SetActive(false);
                        }
                        if (islands.transform.TryFind("mdlVoidStageIslandNW", out Transform mdlVoidStageIslandNW))
                        {
                            mdlVoidStageIslandNW.transform.Find("mdlVoidTerrainNW/mdlGravityCollarGenericBase")?.gameObject.SetActive(false);
                        }
                        if (islands.transform.TryFind("mdlVoidStageIslandEastSmall", out Transform mdlVoidStageIslandEastSmall))
                        {
                            mdlVoidStageIslandEastSmall.transform.Find("mdlGravityShrineEnd")?.gameObject.SetActive(false);
                        }
                        if (islands.transform.TryFind("mdlVoidStageIslandSouthSmall", out Transform mdlVoidStageIslandSouthSmall))
                        {
                            mdlVoidStageIslandSouthSmall.transform.Find("mdlVoidTerrainSouthSmall/mdlVoidSupportPlatform/Point Light Void Support Platform")?.gameObject.SetActive(false);
                            mdlVoidStageIslandSouthSmall.transform.Find("mdlVoidTerrainSouthSmall/mdlVoidSupportPlatform/Point Light Void Support Platform (1)")?.gameObject.SetActive(false);
                            mdlVoidStageIslandSouthSmall.transform.Find("mdlVoidTerrainSouthSmall/mdlVoidSupportPlatform.001")?.gameObject.SetActive(false);
                        }
                    }
                }
            }
            if (rootObjects.TryGetValue("HOLDER: Pebbles & Small Doodads", out GameObject pebblesAndSmallDoodads))
            {
                foreach (Transform child in pebblesAndSmallDoodads.transform.AllChildren())
                {
                    if (child.TryGetComponent(out MeshRenderer meshRenderer) && meshRenderer.sharedMaterial == matVoidCrystal)
                    {
                        meshRenderer.sharedMaterial = matVoidCrystalNew;
                    }
                }
            }
            if (rootObjects.TryGetValue("Weather, Void Stage", out GameObject weather))
            {
                weather.transform.Find("HOLDER: FX")?.gameObject.SetActive(false);
                weather.transform.Find("HOLDER: Cloud Floor")?.gameObject.SetActive(false);
                if (weather.transform.TryFind("HOLDER: Skybox Kelp Trees", out Transform kelp))
                {
                    kelp.gameObject.SetActive(true);
                    kelp.transform.localPosition = new Vector3(100, 0, 0);
                    kelp.transform.localEulerAngles = new Vector3(0, 340, 90);
                    kelp.transform.localScale = Vector3.one * 2f;
                }
                if (weather.transform.TryFind("HOLDER: Lights/PP + Amb", out Transform postProcessing))
                {
                    if (postProcessing.TryGetComponent(out PostProcessVolume postProcessVolume))
                    {
                        postProcessVolume.sharedProfile = ppSceneVoidStageNew;
                        postProcessVolume.profile = null;
                    }
                    if (postProcessing.TryGetComponent(out SetAmbientLight setAmbientLight))
                    {
                        setAmbientLight.skyboxMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/arena/matSkyboxArena.mat").WaitForCompletion();
                        setAmbientLight.ambientSkyColor = new Color32(78, 70, 106, 255);
                    }
                }
                if (weather.transform.TryFind("HOLDER: Lights/Directional Light", out Transform directionalLight) && directionalLight.TryGetComponent(out Light light))
                {
                    light.color = new Color32(142, 196, 107, 255);
                    light.intensity = 1;
                    light.shadows = LightShadows.None;
                    directionalLight.localPosition = new Vector3(0f, 50f, 0f);
                    directionalLight.localEulerAngles = new Vector3(50f, 90f, 120f);
                }
            }
            if (rootObjects.TryGetValue("HOLDER: Props", out GameObject props))
            {
                props.SetActive(false);
            }
            if (rootObjects.TryGetValue("HOLDER: AsteroidField", out GameObject asteroidField))
            {
                asteroidField.SetActive(false);
            }
            if (rootObjects.TryGetValue("LogPickup", out GameObject logPickup))
            {
                UnityEngine.Object.Destroy(logPickup);
            }
            if (rootObjects.TryGetValue("HOLDER: Geysers", out GameObject geysers))
            {
                GameObject geyserPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/Props/Geyser.prefab").WaitForCompletion();
                Transform[] children = geysers.transform.AllChildren().ToArray();
                Vector3?[] geyserLocalPositionOverrides = new Vector3?[Math.Max(children.Length, 7)];
                (Vector3, float)?[] geyserJumpVelocityOverrides = new (Vector3, float)?[Math.Max(children.Length, 7)];

                geyserLocalPositionOverrides[0] = new Vector3(-215f, 30.3f, -74f);
                geyserLocalPositionOverrides[2] = new Vector3(-201f, 14.8f, -136.17f);
                geyserLocalPositionOverrides[3] = new Vector3(-128.39f, 29.7f, -166.13f);
                geyserLocalPositionOverrides[4] = new Vector3(152.317f, 6.8f, -177.4871f);
                geyserLocalPositionOverrides[5] = new Vector3(182.99f, 2.209999f, -105.39f);

                geyserJumpVelocityOverrides[0] = (new Vector3(6.4f, 22.5f, -27.0f), 2.86f);
                geyserJumpVelocityOverrides[1] = (new Vector3(-3.3f, 33.5f, -30.7f), 3.19f);
                geyserJumpVelocityOverrides[2] = (new Vector3(-6.0f, 27.0f, 38.5f), 2.10f);
                geyserJumpVelocityOverrides[3] = (new Vector3(25.0f, 23.1f, 20.1f), 3.03f);
                geyserJumpVelocityOverrides[4] = (new Vector3(16.4f, 21.2f, 41.0f), 2.37f);
                geyserJumpVelocityOverrides[5] = (new Vector3(-17.4f, 25.1f, -38.3f), 2.21f);
                geyserJumpVelocityOverrides[6] = (new Vector3(-13.1f, 32.9f, -19.5f), 2.59f);

                for (int i = 0; i < children.Length; i++)
                {
                    Transform child = children[i];
                    if (child.gameObject.activeSelf)
                    {
                        JumpVolume oldJumpVolume = child.GetComponentInChildren<JumpVolume>();
                        if (oldJumpVolume)
                        {
                            GameObject geyserInstance = UnityEngine.Object.Instantiate(geyserPrefab, geysers.transform);
                            geyserInstance.transform.SetPositionAndRotation(geyserLocalPositionOverrides[i] ?? child.position, child.rotation);
                            JumpVolume newJumpVolume = geyserInstance.GetComponentInChildren<JumpVolume>();
                            if (newJumpVolume)
                            {
                                newJumpVolume.jumpVelocity = geyserJumpVelocityOverrides[i]?.Item1 ?? oldJumpVolume.jumpVelocity;
                                newJumpVolume.time = geyserJumpVelocityOverrides[i]?.Item2 ?? oldJumpVolume.time;
                                newJumpVolume.targetElevationTransform = oldJumpVolume.targetElevationTransform;
                            }
                        }
                        child.gameObject.SetActive(false);
                    }
                }

                {
                    GameObject geyserInstance = UnityEngine.Object.Instantiate(geyserPrefab, geysers.transform);
                    geyserInstance.transform.SetPositionAndRotation(new Vector3(154f, 39.2f, -131f), Quaternion.Euler(270, 0, 0));
                    JumpVolume newJumpVolume = geyserInstance.GetComponentInChildren<JumpVolume>();
                    if (newJumpVolume)
                    {
                        newJumpVolume.jumpVelocity = new Vector3(-26.3f, 23.5f, 28.3f);
                        newJumpVolume.time = 2.63f;
                    }
                }
            }

            static void InstantiateCoral(Vector3 position, Vector3 eulerAngles)
            {
                GameObject instance = UnityEngine.Object.Instantiate(Assets.VoidCoralLit, position, Quaternion.Euler(eulerAngles));
                instance.transform.localScale = Vector3.one * 2f;
            }

            InstantiateCoral(new Vector3(-224f, 26.7f, -52f), new Vector3(280, 0, 0));
            InstantiateCoral(new Vector3(-10f, 26.9f, -20f), new Vector3(270, 40, 0));
            InstantiateCoral(new Vector3(-124f, 25.4f, -10f), new Vector3(270, 20, 0));
            InstantiateCoral(new Vector3(-178f, 17.7f, 185f), new Vector3(270, 60, 0));
            InstantiateCoral(new Vector3(-75f, 17.6f, 215f), new Vector3(270, 0, 0));
            InstantiateCoral(new Vector3(87f, -12f, 12f), new Vector3(270, 310, 0));
            InstantiateCoral(new Vector3(54f, 14f, -235f), new Vector3(270, 0, 0));
            InstantiateCoral(new Vector3(61f, 7.1f, -98f), new Vector3(270, 90, 0));
        }
    }
}
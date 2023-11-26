using System;
using UnityEngine;
using BepInEx;
using System.Security;
using System.Security.Permissions;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using RoR2;
using HG;
using UnityEngine.SceneManagement;
using RoR2.Navigation;
using RoR2.EntitlementManagement;
using RoR2.ContentManagement;
using System.Collections;
using RoR2.ExpansionManagement;
using UnityEngine.Networking;
using System.Threading.Tasks;
using R2API;
using System.Linq;
using UnityEngine.ResourceManagement;
using System.Collections.Generic;

[module: UnverifiableCode]
#pragma warning disable
[assembly: SecurityPermission(System.Security.Permissions.SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore
[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace CorruptsAllVoidStages
{
    [BepInPlugin("com.groovesalad.DesolateReef", "DesolateReef", "1.0.0")]
    public class CorruptsAllVoidStages : BaseUnityPlugin, IContentPackProvider
    {
        public bool disableOcclusionCulling;
        public AssetBundleCreateRequest desolateReefAssets;
        public ContentPack contentPack;
        public RuntimeVoidStageTemplate voidStageTemplate;

        public string identifier => "groovesalad.CorruptsAllVoidStages";

        public void Awake()
        {
            disableOcclusionCulling = Config.Bind("Optimization", "Disable Occlusion Culling", true, "Disables occlusion culling on Desolate Reef. Enabling culling will slightly improve performance but may cause flickering in certain areas of the map.").Value;

            desolateReefAssets = AssetBundle.LoadFromFileAsync(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Info.Location), "desolatereefassets"));
            
            contentPack = new ContentPack
            {
                identifier = identifier
            };

            Language.Init();

            ContentManager.collectContentPackProviders += add => add(this);
            On.RoR2.RuleDef.FromExpansion += RuleDef_FromExpansion;
            On.RoR2.RuleDef.AvailableChoiceCount += RuleDef_AvailableChoiceCount;
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            Run.onRunStartGlobal += Run_onRunStartGlobal;
            if (disableOcclusionCulling)
            {
                CameraRigController.onCameraEnableGlobal += CameraRigController_onCameraEnableGlobal;
            }
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            static AsyncOperationHandle CreateAddressableOp<TObject>(object key, out AsyncOperationHandle<TObject> op) => op = Addressables.LoadAssetAsync<TObject>(key);

            var groupOp = Addressables.ResourceManager.CreateGenericGroupOperation(new List<AsyncOperationHandle>
            {
                CreateAddressableOp<EntitlementDef>("RoR2/DLC1/Common/entitlementDLC1.asset", out var entitlementDLC1),
                CreateAddressableOp<SceneDef>("RoR2/DLC1/voidstage/voidstage.asset", out var voidstage),
                CreateAddressableOp<SceneCollection>("RoR2/Base/SceneGroups/sgStage2.asset", out var sgStage2),
                CreateAddressableOp<SceneCollection>("RoR2/Base/SceneGroups/sgStage3.asset", out var sgStage3),
                CreateAddressableOp<MusicTrackDef>("RoR2/DLC1/Common/muGameplayDLC1_06.asset", out var soundlessDepths),
                CreateAddressableOp<MusicTrackDef>("RoR2/Base/Common/muSong05.asset", out var thermodynamicEquilibrium),
                CreateAddressableOp<Material>("RoR2/Base/bazaar/matBazaarSeerGolemplains.mat", out var matBazaarSeerGolemplains),
                CreateAddressableOp<Material>("RoR2/DLC1/voidstage/matVoidTerrain.mat", out var matVoidTerrain),
                CreateAddressableOp<Texture>("RoR2/Base/Common/Props/texSand1.png", out var texSand1),
                CreateAddressableOp<Material>("RoR2/DLC1/voidstage/matVoidMetalTrimGrassyVertexColorsOnly.mat", out var matVoidMetalTrimGrassyVertexColorsOnly),
                CreateAddressableOp<Texture>("RoR2/DLC1/voidstage/texVoidMoss.tga", out var texVoidMoss),
                CreateAddressableOp<GameObject>("RoR2/DLC1/sulfurpools/SPCoralMDLit.prefab", out var SPCoralMDLit),
                CreateAddressableOp<Texture>("RoR2/DLC1/Common/ColorRamps/texRampVoidFlatCoral.png", out var texRampVoidFlatCoral),
                CreateAddressableOp<GameObject>("RoR2/DLC1/voidstage/VoidStageDiorama.prefab", out var VoidStageDiorama),
                CreateAddressableOp<Material>("RoR2/DLC1/voidstage/matVoidTrim.mat", out var matVoidTrim),
                CreateAddressableOp<GameObject>("RoR2/Base/Lemurian/LemurianMaster.prefab", out var LemurianMaster),
                CreateAddressableOp<NodeGraph>("RoR2/DLC1/voidstage/voidstage_GroundNodeGraph.asset", out var voidstage_GroundNodeGraph),
                CreateAddressableOp<NodeGraph>("RoR2/DLC1/voidstage/voidstage_AirNodeGraph.asset", out var voidstage_AirNodeGraph),
                CreateAddressableOp<UnlockableDef>("RoR2/DLC1/voidstage/Logs.Stages.voidstage.asset", out var Logs_Stages_voidstage),
            }, true);

            yield return desolateReefAssets;
            var texDesolateReefPreview = desolateReefAssets.assetBundle.LoadAssetAsync<Sprite>("texDesolateReefPreview");
            var texDesolateReefSeerPreview = desolateReefAssets.assetBundle.LoadAssetAsync<Sprite>("texDesolateReefSeerPreview");

            while (!groupOp.IsDone)
            {
                args.ReportProgress(groupOp.PercentComplete);
                yield return null;
            }

            Assets.SetNewVoidStageHiddenExpansion(entitlementDLC1.Result);

            voidstage.Result.requiredExpansion = Assets.NewVoidStageHiddenExpansion;
            voidstage.Result.sceneType = SceneType.Stage;
            voidstage.Result.blockOrbitalSkills = false;
            voidstage.Result.destinationsGroup = sgStage3.Result;
            ArrayUtils.ArrayAppend(ref sgStage2.Result._sceneEntries, new SceneCollection.SceneEntry
            {
                sceneDef = voidstage.Result,
                weight = 1f,
            });
            voidstage.Result.stageOrder = 2;
            voidstage.Result.validForRandomSelection = true;
            voidstage.Result.mainTrack = soundlessDepths.Result;
            voidstage.Result.bossTrack = thermodynamicEquilibrium.Result;
            voidstage.Result.nameToken = "GS_MAP_DESOLATEREEF_TITLE";
            voidstage.Result.subtitleToken = "GS_MAP_DESOLATEREEF_SUBTITLE";
            voidstage.Result.loreToken = "GS_MAP_DESOLATEREEF_LORE";
            voidstage.Result.portalSelectionMessageString = "GS_BAZAAR_SEER_DESOLATEREEF";

            Assets.SetMatVoidTerrainNew(matVoidTerrain.Result, texSand1.Result);
            Assets.SetMatVoidMetalOvergrown(matVoidMetalTrimGrassyVertexColorsOnly.Result, texSand1.Result, texVoidMoss.Result);
            Assets.SetVoidCoralLit(SPCoralMDLit.Result, texRampVoidFlatCoral.Result, texVoidMoss.Result);

            voidstage.Result.dioramaPrefab = PrefabAPI.InstantiateClone(VoidStageDiorama.Result, "DesolateReefDiorama", false);
            if (voidstage.Result.dioramaPrefab.TryGetComponent(out ModelPanelParameters modelPanelParameters))
            {
                modelPanelParameters.minDistance = 20f;
                modelPanelParameters.maxDistance = 200f;
            }
            if (voidstage.Result.dioramaPrefab.transform.TryFind("Holder", out Transform holder))
            {
                holder.transform.localScale = Vector3.one * 4f;
                holder.transform.Find("FX")?.gameObject.SetActive(false);
                holder.transform.Find("Lights")?.gameObject.SetActive(false);
                holder.transform.Find("VoidWhale")?.gameObject.SetActive(false);
                if (holder.TryFind("mdlVoidStageDiorama/mdlVoidStageDioramaBase", out Transform mdlVoidStageDioramaBase))
                {
                    mdlVoidStageDioramaBase.transform.Find("dioramawater")?.gameObject.SetActive(false);
                    mdlVoidStageDioramaBase.transform.Find("mdlVoidMetalSpiralWalkway.001/mdlPortalFrame.001")?.gameObject.SetActive(false);
                    mdlVoidStageDioramaBase.transform.Find("mdlVoidSupportPlatform.004")?.gameObject.SetActive(false);
                    mdlVoidStageDioramaBase.transform.Find("mdlVoidEastPlatform.001/Pipe")?.gameObject.SetActive(false);
                    if (mdlVoidStageDioramaBase.transform.TryFind("mdlVoidTerrainSouthSmallRoof.001", out Transform mdlVoidTerrainSouthSmallRoof))
                    {
                        if (mdlVoidTerrainSouthSmallRoof.TryGetComponent(out MeshRenderer meshRenderer))
                        {
                            meshRenderer.sharedMaterial = Assets.matVoidTerrainNew;
                        }
                        if (mdlVoidTerrainSouthSmallRoof.TryFind("mdlVoidRepairsPlatform.001", out Transform mdlVoidRepairsPlatform))
                        {
                            if (mdlVoidRepairsPlatform.TryGetComponent(out MeshRenderer platformMeshRenderer))
                            {
                                platformMeshRenderer.enabled = false;
                            }
                            foreach (Transform child in mdlVoidRepairsPlatform.AllChildren())
                            {
                                child.gameObject.SetActive(false);
                            }
                            if (mdlVoidRepairsPlatform.TryFind("mdlVoidArchEntry.002", out Transform mdlVoidArchEntry))
                            {
                                mdlVoidArchEntry.gameObject.SetActive(true);
                                if (mdlVoidArchEntry.TryGetComponent(out MeshRenderer archMeshRenderer))
                                {
                                    archMeshRenderer.sharedMaterial = matVoidTrim.Result;
                                }
                                mdlVoidArchEntry.Find("mdlVoidArchEntry.003")?.gameObject.SetActive(false);
                                mdlVoidArchEntry.localScale = Vector3.one * 3f;
                            }
                        }
                    }
                    if (mdlVoidStageDioramaBase.transform.TryFind("mdlVoidEastPlatform.001", out Transform mdlVoidEastPlatform))
                    {
                        if (mdlVoidEastPlatform.TryGetComponent(out MeshRenderer meshRenderer))
                        {
                            meshRenderer.sharedMaterial = Assets.matVoidMetalOvergrown;
                        }
                    }
                    if (mdlVoidStageDioramaBase.transform.TryFind("mdlVoidMetalSpiralWalkway.001", out Transform mdlVoidMetalSpiralWalkway))
                    {
                        if (mdlVoidMetalSpiralWalkway.TryGetComponent(out MeshRenderer meshRenderer))
                        {
                            meshRenderer.sharedMaterial = Assets.matVoidMetalOvergrown;
                        }
                    }
                    if (mdlVoidStageDioramaBase.TryGetComponent(out MeshRenderer baseMeshRenderer))
                    {
                        baseMeshRenderer.sharedMaterial = Assets.matVoidMetalOvergrown;
                    }
                }
                holder.transform.Find("Grass/CrabFoam1Prop (15)")?.gameObject.SetActive(false);
                holder.transform.Find("Grass/CrabFoam1Prop (16)")?.gameObject.SetActive(false);
                holder.transform.Find("Grass/CrabFoam1Prop (17)")?.gameObject.SetActive(false);
                holder.transform.Find("Grass/CrabFoam1Prop (18)")?.gameObject.SetActive(false);
                if (holder.transform.TryFind("Grass/VoidFanCoral (1)", out Transform voidFanCoral))
                {
                    voidFanCoral.localScale = Vector3.one * 0.25f;
                }
                if (holder.transform.TryFind("Grass/VoidBubbleCoral (1)", out Transform voidBubbleCoral))
                {
                    voidBubbleCoral.localScale = Vector3.one * 0.25f;
                    voidBubbleCoral.Find("VoidBubble/Point Light")?.gameObject.SetActive(false);
                    voidBubbleCoral.Find("VoidBubble/Bubble")?.gameObject.SetActive(true);
                }
                GameObject largeVoidCoralLit = Instantiate(Assets.VoidCoralLit, holder);
                largeVoidCoralLit.transform.localPosition = new Vector3(0f, 0.8f, 0f);
                largeVoidCoralLit.transform.localScale = Vector3.one * 1.5f;
                GameObject smallVoidCoralLit = Instantiate(Assets.VoidCoralLit, holder);
                smallVoidCoralLit.transform.localPosition = new Vector3(-0.7f, 0.9f, -3.7f);
                smallVoidCoralLit.transform.localEulerAngles = new Vector3(270, 310, 0);
                smallVoidCoralLit.transform.localScale = Vector3.one * 0.8f;
            }

            Assets.SetUnderwaterLemurianMaster(LemurianMaster.Result);

            IZone[] disabledGroundNodeZones = new IZone[]
            {
                // first stairs section
                new SimpleBoxZone
                {
                    cornerA = new Vector3(-45, 29, -26),
                    cornerB = new Vector3(45, 68, -95),
                },
                // rest of stairs
                new SimpleRadialZone
                {
                    center = new Vector2(2.425146f, -12.55224f),
                    height = new RangeFloat { min = 37.74302f, max = 130f },
                    radius = 100f,
                },
                // random nodes under spirals
                new SimpleSphereZone
                {
                    center = new Vector3(31.51681f, 9.069029f, -83.28874f),
                    radius = 4,
                },
                new SimpleSphereZone
                {
                    center = new Vector3(-54.34208f, -3.969529f, 27.88097f),
                    radius = 4,
                },
                // top section of island with platform removed
                new SimpleBoxZone
                {
                    cornerA = new Vector3(-30f, 22f, 214f),
                    cornerB = new Vector3(-220.7236f, 42.97554f, 400f),
                },
                // part of the top section is fake
                new SimpleRadialZone
                {
                    center = new Vector2(-184, 209),
                    height = new RangeFloat { min = 25f, max = 50f },
                    radius = 15f,
                },
                // random node that used to be on platform
                new SimpleSphereZone
                {
                    center = new Vector3(-111.0189f, 19.61561f, 197.9515f),
                    radius = 4,
                },
                // leading to top section
                new SimpleBoxZone
                {
                    cornerA = new Vector3(-112.1319f, 20.08289f, 201f),
                    cornerB = new Vector3(-140.7579f, 28f, 217f),
                },
                // out in front of a gesyer
                new SimpleSphereZone
                {
                    center = new Vector3(157.7814f, 6.52061f, -173.4296f),
                    radius = 4,
                },
            };
            IZone[] disabledAirNodeZones = new IZone[]
            {
                // rough estimate of air tunnel
                new SimpleBoxZone
                {
                    cornerA = new Vector3(-274.8925f, 142.8943f, 129.8717f),
                    cornerB = new Vector3(-52.02304f, 103.5345f, -60.19645f),
                },
                // nodes inside lower spiral
                new SimpleBoxZone
                {
                    cornerA = new Vector3(-63.34568f, 42.65285f, 72.64758f),
                    cornerB = new Vector3(43.69559f, 4.374349f, 44.70821f),
                },
                // nodes inside middle spiral
                new SimpleSphereZone
                {
                    center = new Vector3(48.7455f, 23.94464f, -70.06884f),
                    radius = 15,
                },
            };

            byte disabledGroundeNodeGateIndex = voidstage_GroundNodeGraph.Result.RegisterGateName("groovesalad.disabledNodes");
            for (int i = 0; i < voidstage_GroundNodeGraph.Result.nodes.Length; i++)
            {
                Vector3 position = voidstage_GroundNodeGraph.Result.nodes[i].position;
                if (disabledGroundNodeZones.Any(x => x.IsInBounds(position)))
                {
                    voidstage_GroundNodeGraph.Result.nodes[i].gateIndex = disabledGroundeNodeGateIndex;
                }
            }
            byte disabledAirNodeGateIndex = voidstage_AirNodeGraph.Result.RegisterGateName("groovesalad.disabledNodes");
            for (int i = 0; i < voidstage_AirNodeGraph.Result.nodes.Length; i++)
            {
                Vector3 position = voidstage_AirNodeGraph.Result.nodes[i].position;
                if (disabledAirNodeZones.Any(x => x.IsInBounds(position)))
                {
                    voidstage_AirNodeGraph.Result.nodes[i].gateIndex = disabledAirNodeGateIndex;
                }
            }
            Logs_Stages_voidstage.Result.nameToken = "GS_UNLOCKABLE_LOG_STAGES_DESOLATEREEF";   

            yield return texDesolateReefPreview;
            yield return texDesolateReefSeerPreview;
            voidstage.Result.previewTexture = (texDesolateReefPreview.asset as Sprite).texture;
            voidstage.Result.portalMaterial = new Material(matBazaarSeerGolemplains.Result);
            voidstage.Result.portalMaterial.SetTexture("_MainTex", (texDesolateReefSeerPreview.asset as Sprite).texture);

            desolateReefAssets.assetBundle.Unload(false);

            contentPack.expansionDefs.Add(new[]
            {
                Assets.NewVoidStageHiddenExpansion,
            });
            contentPack.masterPrefabs.Add(new[]
            {
                Assets.UnderwaterLemurianMaster,
            });
        }

        public IEnumerator GenerateContentPackAsync(GetContentPackAsyncArgs args)
        {
            ContentPack.Copy(contentPack, args.output);
            yield break;
        }

        public IEnumerator FinalizeAsync(FinalizeAsyncArgs args)
        {
            yield break;
        }

        private RuleDef RuleDef_FromExpansion(On.RoR2.RuleDef.orig_FromExpansion orig, ExpansionDef expansionDef)
        {
            RuleDef ruleDef = orig(expansionDef);
            if (expansionDef == Assets.NewVoidStageHiddenExpansion)
            {
                ruleDef.forceLobbyDisplay = false;
            }
            return ruleDef;
        }

        private int RuleDef_AvailableChoiceCount(On.RoR2.RuleDef.orig_AvailableChoiceCount orig, RuleDef self, RuleChoiceMask availability)
        {
            int result = orig(self, availability);
            if (self.globalName == "Expansions.groovesalad.NewVoidStageHiddenExpansion")
            {
                return 0;
            }
            return result;
        }

        private void SceneManager_activeSceneChanged(Scene oldScene, Scene newScene)
        {
            if (newScene.name == "voidstage")
            {
                (voidStageTemplate ??= new RuntimeVoidStageTemplate(newScene)).Apply(newScene);
            }
        }

        private void Run_onRunStartGlobal(Run run)
        {
            if (NetworkServer.active)
            {
                run.SetEventFlag("NoVoidStage");
            }
        }

        private void CameraRigController_onCameraEnableGlobal(CameraRigController cameraRigController)
        {
            if (SceneCatalog.mostRecentSceneDef?.cachedName == "voidstage" && cameraRigController.sceneCam)
            {
                cameraRigController.sceneCam.useOcclusionCulling = false;
            }
        }
    }
}
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

[module: UnverifiableCode]
#pragma warning disable
[assembly: SecurityPermission(System.Security.Permissions.SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore
[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace CorruptsAllVoidStages
{
    [BepInPlugin("com.groovesalad.CorruptsAllVoidStages", "CorruptsAllVoidStages", "1.0.0")]
    public class CorruptsAllVoidStages : BaseUnityPlugin, IContentPackProvider
    {
        public RuntimeVoidStageTemplate voidStageTemplate;
        public ContentPack contentPack;
        public ExpansionDef newVoidStageHiddenExpansion;

        public string identifier => "groovesalad.CorruptsAllVoidStages";

        public void Awake()
        {
            Language.Init();

            contentPack = new ContentPack
            {
                identifier = identifier
            };

            _ = LoadAsync();
        }

        public async Task LoadAsync()
        {
            var entitlementDLC1 = Addressables.LoadAssetAsync<EntitlementDef>("RoR2/DLC1/Common/entitlementDLC1.asset");
            var voidStage = Addressables.LoadAssetAsync<SceneDef>("RoR2/DLC1/voidstage/voidstage.asset");
            var sgStage2 = Addressables.LoadAssetAsync<SceneCollection>("RoR2/Base/SceneGroups/sgStage2.asset");
            var sgStage3 = Addressables.LoadAssetAsync<SceneCollection>("RoR2/Base/SceneGroups/sgStage3.asset");
            var soundlessDepths = Addressables.LoadAssetAsync<MusicTrackDef>("RoR2/DLC1/Common/muGameplayDLC1_06.asset");
            var thermodynamicEquilibrium = Addressables.LoadAssetAsync<MusicTrackDef>("RoR2/Base/Common/muSong05.asset");
            var matBazaarSeerGolemplains = Addressables.LoadAssetAsync<Material>("RoR2/Base/bazaar/matBazaarSeerGolemplains.mat");
            var matVoidTerrain = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/voidstage/matVoidTerrain.mat");
            var texSand1 = Addressables.LoadAssetAsync<Texture>("RoR2/Base/Common/Props/texSand1.png");
            var matVoidMetalTrimGrassyVertexColorsOnly = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/voidstage/matVoidMetalTrimGrassyVertexColorsOnly.mat");
            var texVoidMoss = Addressables.LoadAssetAsync<Texture>("RoR2/DLC1/voidstage/texVoidMoss.tga");
            var SPCoralMDLit = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/sulfurpools/SPCoralMDLit.prefab");
            var texRampVoidFlatCoral = Addressables.LoadAssetAsync<Texture>("RoR2/DLC1/Common/ColorRamps/texRampVoidFlatCoral.png");
            var VoidStageDiorama = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/voidstage/VoidStageDiorama.prefab");
            var matVoidTrim = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/voidstage/matVoidTrim.mat");
            var LemurianMaster = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianMaster.prefab");

            AssetBundle desolateReefAssets = AssetBundle.LoadFromFile(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Info.Location), "desolatereefassets"));

            newVoidStageHiddenExpansion = ScriptableObject.CreateInstance<ExpansionDef>();
            newVoidStageHiddenExpansion.name = "groovesalad.NewVoidStage";

            newVoidStageHiddenExpansion.requiredEntitlement = await entitlementDLC1;
            newVoidStageHiddenExpansion.nameToken = string.Empty;
            newVoidStageHiddenExpansion.descriptionToken = string.Empty;

            await voidStage;
            voidStage.Result.requiredExpansion = newVoidStageHiddenExpansion;
            voidStage.Result.sceneType = SceneType.Stage;
            voidStage.Result.blockOrbitalSkills = false;
            voidStage.Result.destinationsGroup = await sgStage3;
            await sgStage2;
            ArrayUtils.ArrayAppend(ref sgStage2.Result._sceneEntries, new SceneCollection.SceneEntry
            {
                sceneDef = voidStage.Result,
                weight = 1f,
            });
            voidStage.Result.stageOrder = 2;
            voidStage.Result.validForRandomSelection = true;
            voidStage.Result.mainTrack = await soundlessDepths;
            voidStage.Result.bossTrack = await thermodynamicEquilibrium;
            voidStage.Result.nameToken = "GS_MAP_DESOLATEREEF_TITLE";
            voidStage.Result.subtitleToken = "GS_MAP_DESOLATEREEF_SUBTITLE";
            voidStage.Result.loreToken = "GS_MAP_DESOLATEREEF_LORE";
            voidStage.Result.portalSelectionMessageString = "GS_BAZAAR_SEER_DESOLATEREEF";

            await matVoidTerrain;
            await texSand1;
            CommonAssets.SetMatVoidTerrainNew(matVoidTerrain.Result, texSand1.Result);
            await matVoidMetalTrimGrassyVertexColorsOnly;
            await texVoidMoss;
            CommonAssets.SetMatVoidMetalOvergrown(matVoidMetalTrimGrassyVertexColorsOnly.Result, texSand1.Result, texVoidMoss.Result);

            await SPCoralMDLit;
            await texRampVoidFlatCoral;
            CommonAssets.SetVoidCoralLit(SPCoralMDLit.Result, texRampVoidFlatCoral.Result, texVoidMoss.Result);

            voidStage.Result.dioramaPrefab = PrefabAPI.InstantiateClone(await VoidStageDiorama, "DesolateReefDiorama", false);
            if (voidStage.Result.dioramaPrefab.TryGetComponent(out ModelPanelParameters modelPanelParameters))
            {
                modelPanelParameters.minDistance = 20f;
                modelPanelParameters.maxDistance = 200f;
            }
            if (voidStage.Result.dioramaPrefab.transform.TryFind("Holder", out Transform holder))
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
                            meshRenderer.sharedMaterial = CommonAssets.matVoidTerrainNew;
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
                                    archMeshRenderer.sharedMaterial = await matVoidTrim;
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
                            meshRenderer.sharedMaterial = CommonAssets.matVoidMetalOvergrown;
                        }
                    }
                    if (mdlVoidStageDioramaBase.transform.TryFind("mdlVoidMetalSpiralWalkway.001", out Transform mdlVoidMetalSpiralWalkway))
                    {
                        if (mdlVoidMetalSpiralWalkway.TryGetComponent(out MeshRenderer meshRenderer))
                        {
                            meshRenderer.sharedMaterial = CommonAssets.matVoidMetalOvergrown;
                        }
                    }
                    if (mdlVoidStageDioramaBase.TryGetComponent(out MeshRenderer baseMeshRenderer)) 
                    {
                        baseMeshRenderer.sharedMaterial = CommonAssets.matVoidMetalOvergrown;
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
                GameObject largeVoidCoralLit = Instantiate(CommonAssets.voidCoralLit, holder);
                largeVoidCoralLit.transform.localPosition = new Vector3(0f, 0.8f, 0f);
                largeVoidCoralLit.transform.localScale = Vector3.one * 1.5f;
                GameObject smallVoidCoralLit = Instantiate(CommonAssets.voidCoralLit, holder);
                smallVoidCoralLit.transform.localPosition = new Vector3(-0.7f, 0.9f, -3.7f);
                smallVoidCoralLit.transform.localEulerAngles = new Vector3(270, 310, 0);
                smallVoidCoralLit.transform.localScale = Vector3.one * 0.8f;
            }

            await LemurianMaster;
            CommonAssets.SetUnderwaterLemurianMaster(LemurianMaster.Result);

            voidStage.Result.previewTexture = desolateReefAssets.LoadAsset<Sprite>("texDesolateReefPreview").texture;
            voidStage.Result.portalMaterial = new Material(await matBazaarSeerGolemplains);
            voidStage.Result.portalMaterial.SetTexture("_MainTex", desolateReefAssets.LoadAsset<Sprite>("texDesolateReefSeerPreview").texture);

            desolateReefAssets.Unload(false);
            entitlementDLC1.Release();
            voidStage.Release();
            sgStage2.Release();
            sgStage3.Release();
            soundlessDepths.Release();
            thermodynamicEquilibrium.Release();
            matBazaarSeerGolemplains.Release();
            matVoidTerrain.Release();
            texSand1.Release();
            matVoidMetalTrimGrassyVertexColorsOnly.Release();
            texVoidMoss.Release();
            SPCoralMDLit.Release();
            texRampVoidFlatCoral.Release();
            VoidStageDiorama.Release();
            matVoidTrim.Release();
            LemurianMaster.Release();
        }

        public void OnEnable()
        {
            ContentManager.collectContentPackProviders += ContentManager_collectContentPackProviders;
            On.RoR2.RuleDef.FromExpansion += RuleDef_FromExpansion;
            On.RoR2.RuleDef.AvailableChoiceCount += RuleDef_AvailableChoiceCount;
            SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
            SceneDirector.onPrePopulateSceneServer += SceneDirector_onPrePopulateSceneServer;
            Run.onRunStartGlobal += Run_onRunStartGlobal;
        }

        public void OnDisable()
        {
            ContentManager.collectContentPackProviders -= ContentManager_collectContentPackProviders;
            On.RoR2.RuleDef.FromExpansion -= RuleDef_FromExpansion;
            On.RoR2.RuleDef.AvailableChoiceCount -= RuleDef_AvailableChoiceCount;
            SceneManager.activeSceneChanged -= SceneManager_activeSceneChanged;
            SceneDirector.onPrePopulateSceneServer -= SceneDirector_onPrePopulateSceneServer;
            Run.onRunStartGlobal -= Run_onRunStartGlobal;
        }

        private void ContentManager_collectContentPackProviders(ContentManager.AddContentPackProviderDelegate addContentPackProvider)
        {
            addContentPackProvider(this);
        }

        private RuleDef RuleDef_FromExpansion(On.RoR2.RuleDef.orig_FromExpansion orig, ExpansionDef expansionDef)
        {
            RuleDef ruleDef = orig(expansionDef);
            if (expansionDef == newVoidStageHiddenExpansion)
            {
                ruleDef.forceLobbyDisplay = false;
            }
            return ruleDef;
        }

        private int RuleDef_AvailableChoiceCount(On.RoR2.RuleDef.orig_AvailableChoiceCount orig, RuleDef self, RuleChoiceMask availability)
        {
            int result = orig(self, availability);
            if (self.globalName == "Expansions.groovesalad.NewVoidStage")
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

        private void SceneDirector_onPrePopulateSceneServer(SceneDirector sceneDirector)
        {
            if (SceneCatalog.mostRecentSceneDef.cachedName == "voidstage" && voidStageTemplate != null)
            {
                voidStageTemplate.OnPrePopulateSceneServer(sceneDirector);
            }
        }

        private void Run_onRunStartGlobal(Run run)
        {
            if (NetworkServer.active)
            {
                run.SetEventFlag("NoVoidStage");
            }
        }

        public IEnumerator LoadStaticContentAsync(LoadStaticContentAsyncArgs args)
        {
            contentPack.expansionDefs.Add(new[]
            {
                newVoidStageHiddenExpansion,
            });
            contentPack.masterPrefabs.Add(new[]
            {
                CommonAssets.underwaterLemurianMaster,
            });
            yield break;
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
    }
}
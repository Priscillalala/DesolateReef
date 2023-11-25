using System;
using UnityEngine;
using BepInEx;
using RoR2;
using HG;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using R2API;
using RoR2.CharacterAI;
using System.Linq;
using RoR2.ExpansionManagement;
using RoR2.EntitlementManagement;

namespace CorruptsAllVoidStages
{
    public static class Assets
    {
        public static ExpansionDef NewVoidStageHiddenExpansion { get; private set; }
        public static Material matVoidTerrainNew { get; private set; }
        public static Material matVoidMetalOvergrown { get; private set; }
        public static GameObject VoidCoralLit { get; private set; }
        public static GameObject UnderwaterLemurianMaster { get; private set; }

        public static void SetNewVoidStageHiddenExpansion(EntitlementDef entitlementDLC1)
        {
            NewVoidStageHiddenExpansion = ScriptableObject.CreateInstance<ExpansionDef>();
            NewVoidStageHiddenExpansion.name = "groovesalad.NewVoidStageHiddenExpansion";
            NewVoidStageHiddenExpansion.requiredEntitlement = entitlementDLC1;
            NewVoidStageHiddenExpansion.nameToken = string.Empty;
            NewVoidStageHiddenExpansion.descriptionToken = string.Empty;
        }

        public static void SetMatVoidTerrainNew(Material matVoidTerrain, Texture texSand1)
        {
            matVoidTerrainNew = new Material(matVoidTerrain);
            matVoidTerrainNew.SetTexture("_RedChannelTopTex", texSand1);
            matVoidTerrainNew.SetTextureScale("_RedChannelTopTex", new Vector2(2f, 2f));
            matVoidTerrainNew.SetFloat("_RedChannelBias", 2.45f);
            matVoidTerrainNew.SetFloat("_GreenChannelBias", 0.037f);
        }

        public static void SetMatVoidMetalOvergrown(Material matVoidMetalTrimGrassyVertexColorsOnly, Texture texSand1, Texture texVoidMoss)
        {
            matVoidMetalOvergrown = new Material(matVoidMetalTrimGrassyVertexColorsOnly);
            matVoidMetalOvergrown.SetColor("_Color", new Color32(155, 155, 155, 255));
            matVoidMetalOvergrown.SetFloat("_SpecularStrength", 0.06365327f);
            matVoidMetalOvergrown.SetFloat("_SpecularExponent", 0.7722783f);
            matVoidMetalOvergrown.SetTexture("_SnowTex", texSand1);
            matVoidMetalOvergrown.SetFloat("_SnowBias", 0.05808783f);
            matVoidMetalOvergrown.SetFloat("_Depth", 0.5279048f);
            matVoidMetalOvergrown.EnableKeyword("DIRTON");
            matVoidMetalOvergrown.SetTexture("_DirtTex", texVoidMoss);
            matVoidMetalOvergrown.SetFloat("_DirtBias", 0.5306497f);
        }

        public static void SetVoidCoralLit(GameObject SPCoralMDLit, Texture texRampVoidFlatCoral, Texture texVoidMoss)
        {
            VoidCoralLit = PrefabAPI.InstantiateClone(SPCoralMDLit, "VoidCoralLit", false);
            Light light = VoidCoralLit.GetComponentInChildren<Light>();
            if (light)
            {
                light.intensity = 4f;
                light.range = 15f;
            }
            VoidCoralLit.transform.Find("meshSPCoralString")?.gameObject.SetActive(false);
            if (VoidCoralLit.TryGetComponent(out MeshRenderer meshRenderer))
            {
                meshRenderer.sharedMaterial = new Material(meshRenderer.sharedMaterial);
                meshRenderer.sharedMaterial.SetFloat("_EmPower", 0.5f);
                meshRenderer.sharedMaterial.SetColor("_Color", Color.white);
                meshRenderer.sharedMaterial.SetTexture("_MainTex", texRampVoidFlatCoral);
                meshRenderer.sharedMaterial.SetTexture("_BlueChannelTex", texVoidMoss);
            }
        }

        public static void SetUnderwaterLemurianMaster(GameObject LemurianMaster)
        {
            UnderwaterLemurianMaster = PrefabAPI.InstantiateClone(LemurianMaster, "UnderwaterLemurianMaster", true);
            AISkillDriver[] skillDrivers = UnderwaterLemurianMaster.GetComponents<AISkillDriver>();
            AISkillDriver strafeAndShoot = skillDrivers.FirstOrDefault(x => x.customName == "StrafeAndShoot");
            if (strafeAndShoot)
            {
                UnityEngine.Object.DestroyImmediate(strafeAndShoot);
            }
            AISkillDriver strafeIdley = skillDrivers.FirstOrDefault(x => x.customName == "StrafeIdley");
            if (strafeIdley)
            {
                UnityEngine.Object.DestroyImmediate(strafeIdley);
            }
        }
    }
}
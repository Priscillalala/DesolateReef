using System;
using UnityEngine;
using BepInEx;
using RoR2;
using HG;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using R2API;

namespace CorruptsAllVoidStages
{
    public static class Language
    {
        public static void Init()
        {
            LanguageAPI.Add(new Dictionary<string, string>
            {
                { "GS_MAP_DESOLATEREEF_TITLE", "Desolate Reef" },
                { "GS_MAP_DESOLATEREEF_SUBTITLE", "Spiritual Congregation" },
                //{ "GS_MAP_DESOLATEREEF_LORE", "" },
                { "GS_BAZAAR_SEER_DESOLATEREEF", "<style=cWorldEvent>You dream of oceanic depths.</style>" },
            });
        }
    }
}
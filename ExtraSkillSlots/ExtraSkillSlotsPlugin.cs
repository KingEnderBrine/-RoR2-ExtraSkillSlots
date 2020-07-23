using BepInEx;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using R2API;
using Rewired.Data;
using SimpleJSON;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ExtraSkillSlots
{
    [BepInDependency("com.iDeathHD.FixedSplitscreen", BepInDependency.DependencyFlags.SoftDependency)]
    
    [BepInDependency("com.KingEnderBrine.ScrollableLobbyUI")]
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.KingEnderBrine.ExtraSkillSlots", "Extra Skill Slots", "1.0.3")]

    public class ExtraSkillSlotsPlugin : BaseUnityPlugin
    {
        private static string ExecutingDirectory { get; } = Assembly.GetExecutingAssembly().Location.Replace("\\ExtraSkillSlots.dll", "");
        
        private void Awake()
        {
            RegisterLanguage();

            //Because FixedSplitScreen completely override LoadoutPanelController.FromSkillSlot need to override it again
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.iDeathHD.FixedSplitscreen"))
            {
                RegisterFixedSplitScreenHook();
            }

            //Add actions to RoR2.InputCatalog
            ExtraInputs.AddActionsToInputCatalog();
            
            //Hook to method with some rewired initialization (or not? Anyway it works) to add custom actions
            var userDataInit = typeof(UserData).GetMethod("KFIfLMJhIpfzcbhqEXHpaKpGsgeZ", BindingFlags.NonPublic | BindingFlags.Instance);
            new Hook(userDataInit, (Action<Action<UserData>, UserData>)ExtraInputs.AddCustomActions);

            //Adding custom actions to Settings
            On.RoR2.UI.SettingsPanelController.Start += UIHooks.SettingsPanelControllerStart;

            //Adding custom skill slots to HUD
            On.RoR2.UI.HUD.Awake += UIHooks.HUDAwake;

            //Applying overrides to SkillLocator to use extra skills
            On.RoR2.SkillLocator.GetSkill += ExtraSkillLocator.GetSkillOverrideHook;
            On.RoR2.SkillLocator.FindSkillSlot += ExtraSkillLocator.FindSkillSlotOverrideHook;
            IL.RoR2.SkillLocator.ApplyAmmoPack += ExtraSkillLocator.ApplyAmmoPackILHook;

            //Adding custom InputBankTest
            On.RoR2.InputBankTest.Awake += (orig, self) =>
            {
                orig(self);
                self.gameObject.AddComponent<ExtraInputBankTest>();
            };
            On.RoR2.InputBankTest.CheckAnyButtonDown += ExtraInputBankTest.CheckAnyButtonDownOverrideHook;

            //Adding custom GenericCharacterMain
            On.EntityStates.GenericCharacterMain.OnEnter += (orig, self) =>
            {
                ExtraGenericCharacterMain.Add(self);
                orig(self);
            };
            On.EntityStates.GenericCharacterMain.OnExit += (orig, self) =>
            {
                orig(self);
                ExtraGenericCharacterMain.Remove(self);
            };

            //Applying overrides to GenericCharacterMain to be able to use extra skills
            On.EntityStates.GenericCharacterMain.PerformInputs += ExtraGenericCharacterMain.PerformInputsOverrideHook;
            On.EntityStates.GenericCharacterMain.GatherInputs += ExtraGenericCharacterMain.GatherInputsOverrideHook;

            //Adding custom PlayerCharacterMasterController
            On.RoR2.PlayerCharacterMasterController.Awake += (orig, self) =>
            {
                orig(self);
                self.gameObject.AddComponent<ExtraPlayerCharacterMasterController>();
            };

            //Applying Brainstalks cooldown effect to extra skills
            IL.RoR2.CharacterBody.RecalculateStats += ExtraCharacterBody.RecalculateStatsILHook;

            //Fixing getting extra skill slots for UI
            IL.RoR2.UI.LoadoutPanelController.Row.FromSkillSlot += UIHooks.LoadoutPanelControllerFromSkillSlot;
        }

        private static void RegisterLanguage()
        {
            var flag = false;
            foreach (var file in Directory.GetFiles(ExecutingDirectory, "ess_*.json", SearchOption.AllDirectories))
            {
                flag = true;
                var languageToken = Regex.Match(file, ".+ess_(?<lang>[a-zA-Z]+).json\\Z").Groups["lang"].Value;
                var tokens = JSON.Parse(File.ReadAllText(file));

                if (languageToken == "en")
                {
                    foreach (var key in tokens.Keys)
                    {
                        LanguageAPI.Add(key, tokens[key].Value);
                    }
                }
                foreach (var key in tokens.Keys)
                {
                    LanguageAPI.Add(key, tokens[key].Value, languageToken);
                }
            }
            if (!flag)
            {
                Debug.LogWarning("Localizaiton files not found");
            }
        }

        //Applying same hook as for vanilla
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void RegisterFixedSplitScreenHook()
        {
            var fixCorrectlySaveToRightProfile = typeof(FixedSplitscreen.FixedSplitscreen).GetMethod("FixCorrectlySaveToRightProfile", BindingFlags.NonPublic | BindingFlags.Static);
            MonoMod.RuntimeDetour.HookGen.HookEndpointManager.Modify(fixCorrectlySaveToRightProfile, (Action<ILContext>)UIHooks.FixedSplitScreenFromSkillSlot);
        }
    }
}
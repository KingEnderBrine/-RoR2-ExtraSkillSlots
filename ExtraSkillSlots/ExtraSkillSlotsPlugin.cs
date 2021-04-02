using BepInEx;
using EntityStates;
using MonoMod.RuntimeDetour;
using MonoMod.RuntimeDetour.HookGen;
using Rewired.Data;
using RoR2;
using System;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: R2API.Utils.ManualNetworkRegistration]
[assembly: EnigmaticThunder.Util.ManualNetworkRegistration]
namespace ExtraSkillSlots
{
    [BepInDependency("com.KingEnderBrine.ScrollableLobbyUI")]
    [BepInPlugin(GUID, Name, Version)]

    public class ExtraSkillSlotsPlugin : BaseUnityPlugin
    {
        public const string GUID = "com.KingEnderBrine.ExtraSkillSlots";
        public const string Name = "Extra Skill Slots";
        public const string Version = "1.3.1";

        private void Awake()
        {
            //Add actions to RoR2.InputCatalog
            ExtraInputs.AddActionsToInputCatalog();
            
            //Hook to method with some rewired initialization (or not? Anyway it works) to add custom actions
            var userDataInit = typeof(UserData).GetMethod("KFIfLMJhIpfzcbhqEXHpaKpGsgeZ", BindingFlags.NonPublic | BindingFlags.Instance);
            HookEndpointManager.Add(userDataInit, (Action<Action<UserData>, UserData>)ExtraInputs.AddCustomActions);

            //Adding custom actions to Settings
            On.RoR2.UI.SettingsPanelController.Start += UIHooks.SettingsPanelControllerStart;

            //Adding custom skill slots to HUD
            On.RoR2.UI.HUD.Awake += UIHooks.HUDAwake;

            //Applying overrides to SkillLocator to use extra skills
            On.RoR2.SkillLocator.GetSkill += ExtraSkillLocator.GetSkillOverrideHook;
            On.RoR2.SkillLocator.FindSkillSlot += ExtraSkillLocator.FindSkillSlotOverrideHook;

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

            //Applying override to BaseSkillState
            IL.EntityStates.SkillStateMethods.IsKeyDownAuthority += ExtraBaseSkillState.IsKeyDownAuthorityILHook;

            On.EntityStates.BaseState.OnEnter += (orig, self) =>
            {
                if (self is BaseSkillState selfSkillState)
                {
                    ExtraBaseSkillState.Add(selfSkillState);
                }
                orig(self);
            };

            var baseSkillStateOnExit = typeof(BaseSkillState).GetMethod("OnExit", BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            new Hook(baseSkillStateOnExit, new Action<Action<BaseSkillState>, BaseSkillState>((orig, self) => {
                orig(self);
                ExtraBaseSkillState.Remove(self);
            }));

            //Adding custom PlayerCharacterMasterController
            On.RoR2.PlayerCharacterMasterController.Awake += (orig, self) =>
            {
                orig(self);
                self.gameObject.AddComponent<ExtraPlayerCharacterMasterController>();
            };
            On.RoR2.PlayerCharacterMasterController.SetBody += ExtraPlayerCharacterMasterController.SetBodyOverrideHook;

            //Applying Brainstalks and Purity cooldown effect to extra skills
            IL.RoR2.CharacterBody.RecalculateStats += ExtraCharacterBody.RecalculateStatsILHook;

            //Fixing getting extra skill slots for UI
            IL.RoR2.UI.LoadoutPanelController.Row.FromSkillSlot += UIHooks.LoadoutPanelControllerFromSkillSlot;

            On.RoR2.Language.LoadStrings += LanguageConsts.OnLoadStrings;

            NetworkModCompatibilityHelper.networkModList = NetworkModCompatibilityHelper.networkModList.Append($"{GUID};{Version}");
            RoR2Application.isModded = true;
        }
    }
}
using BepInEx;
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
        public const string Version = "1.3.2";

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

            On.RoR2.InputBankTest.CheckAnyButtonDown += ExtraInputBankTest.CheckAnyButtonDownOverrideHook;

            //Applying overrides to GenericCharacterMain to be able to use extra skills
            On.EntityStates.GenericCharacterMain.PerformInputs += ExtraGenericCharacterMain.PerformInputsOverrideHook;
            On.EntityStates.GenericCharacterMain.GatherInputs += ExtraGenericCharacterMain.GatherInputsOverrideHook;

            //Applying override to BaseSkillState
            IL.EntityStates.SkillStateMethods.IsKeyDownAuthority += ExtraBaseSkillState.IsKeyDownAuthorityILHook;

            //Adding custom PlayerCharacterMasterController
            On.RoR2.PlayerCharacterMasterController.Awake += ExtraPlayerCharacterMasterController.AwakeHook;
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
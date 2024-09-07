using BepInEx;
using BepInEx.Logging;
using MonoMod.RuntimeDetour.HookGen;
using Rewired.Data;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;

[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: AssemblyVersion(ExtraSkillSlots.ExtraSkillSlotsPlugin.Version)]
namespace ExtraSkillSlots
{
    [BepInDependency("com.KingEnderBrine.ScrollableLobbyUI")]
    [BepInPlugin(GUID, Name, Version)]
    public class ExtraSkillSlotsPlugin : BaseUnityPlugin
    {
        public const string GUID = "com.KingEnderBrine.ExtraSkillSlots";
        public const string Name = "Extra Skill Slots";
        public const string Version = "1.6.2";

        internal static ExtraSkillSlotsPlugin Instance { get; private set; }
        internal static ManualLogSource InstanceLogger => Instance?.Logger;

        private void Start()
        {
            Instance = this;

            //Add actions to RoR2.InputCatalog
            ExtraInputs.AddActionsToInputCatalog();
            
            //Hook to method with some rewired initialization (or not? Anyway it works) to add custom actions
            var userDataInit = typeof(UserData).GetMethod(nameof(UserData.gLOOAxUFAvrvUufkVjaYyZoeLbLE), BindingFlags.NonPublic | BindingFlags.Instance);
            HookEndpointManager.Add(userDataInit, ExtraInputs.AddCustomActions);

            //Adding default keybindings
            On.RoR2.UserProfile.LoadDefaultProfile += ExtraInputs.OnLoadDefaultProfile;
            On.RoR2.SaveSystem.LoadUserProfiles += ExtraInputs.OnLoadUserProfiles;

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

            Language.collectLanguageRootFolders += CollectLanguageRootFolders;

            NetworkModCompatibilityHelper.networkModList = NetworkModCompatibilityHelper.networkModList.Append($"{GUID};{Version}");
        }

        private void CollectLanguageRootFolders(List<string> folders)
        {
            folders.Add(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Info.Location), "Language"));
        }
    }
}
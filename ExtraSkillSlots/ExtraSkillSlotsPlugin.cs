using BepInEx;
using BepInEx.Logging;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using HarmonyLib;

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
        public const string Version = "1.5.1";

        internal static ExtraSkillSlotsPlugin Instance { get; private set; }
        internal static ManualLogSource InstanceLogger => Instance?.Logger;

        private void Start()
        {
            Instance = this;

            //Add actions to RoR2.InputCatalog
            ExtraInputs.AddActionsToInputCatalog();
            
            new Harmony(Info.Metadata.GUID).PatchAll();

            Language.collectLanguageRootFolders += CollectLanguageRootFolders;

            NetworkModCompatibilityHelper.networkModList = NetworkModCompatibilityHelper.networkModList.Append($"{GUID};{Version}");
        }

        private void CollectLanguageRootFolders(List<string> folders)
        {
            folders.Add(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Info.Location), "Language"));
        }
    }
}
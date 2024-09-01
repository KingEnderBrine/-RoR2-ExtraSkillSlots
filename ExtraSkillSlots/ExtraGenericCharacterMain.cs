using EntityStates;
using RoR2;
using System.Runtime.CompilerServices;
using HarmonyLib;

namespace ExtraSkillSlots
{
    [HarmonyPatch]
    internal class ExtraGenericCharacterMain
    {
        private static readonly ConditionalWeakTable<GenericCharacterMain, ExtraGenericCharacterMain> instances = new ConditionalWeakTable<GenericCharacterMain, ExtraGenericCharacterMain>();
        
        public bool extraSkill1InputReceived;
        public bool extraSkill2InputReceived;
        public bool extraSkill3InputReceived;
        public bool extraSkill4InputReceived;

        public ExtraSkillLocator ExtraSkillLocator { get; private set; }
        public ExtraInputBankTest ExtraInputBankTest { get; private set; }

        private ExtraGenericCharacterMain() { }

        [HarmonyPostfix, HarmonyPatch(typeof(GenericCharacterMain), nameof(GenericCharacterMain.PerformInputs))]
        internal static void PerformInputsOverrideHook(GenericCharacterMain __instance)
        {
            if (!__instance.isAuthority)
            {
                return;
            }

            if (!instances.TryGetValue(__instance, out var extraGenericCharacterMain))
            {
                instances.Add(__instance, extraGenericCharacterMain = new ExtraGenericCharacterMain
                {
                    ExtraSkillLocator = __instance.outer.GetComponent<ExtraSkillLocator>(),
                    ExtraInputBankTest = __instance.outer.GetComponent<ExtraInputBankTest>()
                });
            }

            var extraSkillLocator = extraGenericCharacterMain.ExtraSkillLocator;
            var extraInputBankTest = extraGenericCharacterMain.ExtraInputBankTest;

            if (extraSkillLocator && extraInputBankTest && extraGenericCharacterMain != null)
            {
                HandleSkill(extraSkillLocator.extraFirst, ref extraGenericCharacterMain.extraSkill1InputReceived, extraInputBankTest.extraSkill1.justPressed);
                HandleSkill(extraSkillLocator.extraSecond, ref extraGenericCharacterMain.extraSkill2InputReceived, extraInputBankTest.extraSkill2.justPressed);
                HandleSkill(extraSkillLocator.extraThird, ref extraGenericCharacterMain.extraSkill3InputReceived, extraInputBankTest.extraSkill3.justPressed);
                HandleSkill(extraSkillLocator.extraFourth, ref extraGenericCharacterMain.extraSkill4InputReceived, extraInputBankTest.extraSkill4.justPressed);
            }

            void HandleSkill(GenericSkill skillSlot, ref bool inputReceived, bool justPressed)
            {
                bool flag = inputReceived;
                inputReceived = false;
                if (!skillSlot || !justPressed && (!flag || skillSlot.mustKeyPress) || !__instance.CanExecuteSkill(skillSlot))
                    return;
                skillSlot.ExecuteIfReady();
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(GenericCharacterMain), nameof(GenericCharacterMain.GatherInputs))]
        internal static void GatherInputsOverrideHook(GenericCharacterMain __instance)
        {
            if (!instances.TryGetValue(__instance, out var extraGenericCharacterMain))
            {
                instances.Add(__instance, extraGenericCharacterMain = new ExtraGenericCharacterMain
                {
                    ExtraSkillLocator = __instance.outer.GetComponent<ExtraSkillLocator>(),
                    ExtraInputBankTest = __instance.outer.GetComponent<ExtraInputBankTest>()
                });
            }

            var extraInputBankTest = extraGenericCharacterMain.ExtraInputBankTest;
            if (!extraInputBankTest)
            {
                return;
            }

            extraGenericCharacterMain.extraSkill1InputReceived |= extraInputBankTest.extraSkill1.down;
            extraGenericCharacterMain.extraSkill2InputReceived |= extraInputBankTest.extraSkill2.down;
            extraGenericCharacterMain.extraSkill3InputReceived |= extraInputBankTest.extraSkill3.down;
            extraGenericCharacterMain.extraSkill4InputReceived |= extraInputBankTest.extraSkill4.down;
        }
    }
}

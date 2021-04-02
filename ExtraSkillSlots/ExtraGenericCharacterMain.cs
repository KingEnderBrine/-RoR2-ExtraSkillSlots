using EntityStates;
using RoR2;
using System.Runtime.CompilerServices;

namespace ExtraSkillSlots
{
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

        internal static void PerformInputsOverrideHook(On.EntityStates.GenericCharacterMain.orig_PerformInputs orig, GenericCharacterMain self)
        {
            orig(self);

            if (!self.isAuthority)
            {
                return;
            }

            if (!instances.TryGetValue(self, out var extraGenericCharacterMain))
            {
                instances.Add(self, extraGenericCharacterMain = new ExtraGenericCharacterMain
                {
                    ExtraSkillLocator = self.outer.GetComponent<ExtraSkillLocator>(),
                    ExtraInputBankTest = self.outer.GetComponent<ExtraInputBankTest>()
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
                if (!skillSlot || !justPressed && (!flag || skillSlot.mustKeyPress) || !self.CanExecuteSkill(skillSlot))
                    return;
                skillSlot.ExecuteIfReady();
            }
        }

        internal static void GatherInputsOverrideHook(On.EntityStates.GenericCharacterMain.orig_GatherInputs orig, GenericCharacterMain self)
        {
            orig(self);

            if (!instances.TryGetValue(self, out var extraGenericCharacterMain))
            {
                instances.Add(self, extraGenericCharacterMain = new ExtraGenericCharacterMain
                {
                    ExtraSkillLocator = self.outer.GetComponent<ExtraSkillLocator>(),
                    ExtraInputBankTest = self.outer.GetComponent<ExtraInputBankTest>()
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

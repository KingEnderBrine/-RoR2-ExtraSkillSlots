using EntityStates;
using RoR2;
using RoR2BepInExPack.Utilities;

namespace ExtraSkillSlots
{
    internal class ExtraGenericCharacterMain
    {
        private static readonly FixedConditionalWeakTable<GenericCharacterMain, ExtraGenericCharacterMain> instances = new FixedConditionalWeakTable<GenericCharacterMain, ExtraGenericCharacterMain>();
        
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

            if (extraSkillLocator && extraInputBankTest)
            {
                self.HandleSkill(extraSkillLocator.extraFirst, ref extraInputBankTest.extraSkill1);
                self.HandleSkill(extraSkillLocator.extraSecond, ref extraInputBankTest.extraSkill2);
                self.HandleSkill(extraSkillLocator.extraThird, ref extraInputBankTest.extraSkill3);
                self.HandleSkill(extraSkillLocator.extraFourth, ref extraInputBankTest.extraSkill4);
            }
        }
    }
}

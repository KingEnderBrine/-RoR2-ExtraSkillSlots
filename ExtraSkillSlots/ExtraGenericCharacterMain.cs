using EntityStates;
using R2API.Utils;
using RoR2;
using System.Collections.Generic;
using System.Reflection;

namespace ExtraSkillSlots
{
    internal class ExtraGenericCharacterMain
    {
        private static readonly PropertyInfo isAuthorityProperty = typeof(GenericCharacterMain).GetProperty("isAuthority", BindingFlags.NonPublic | BindingFlags.Instance);

        public bool extraSkill1InputReceived;
        public bool extraSkill2InputReceived;
        public bool extraSkill3InputReceived;
        public bool extraSkill4InputReceived;

        private static readonly Dictionary<GenericCharacterMain, ExtraGenericCharacterMain> instances = new Dictionary<GenericCharacterMain, ExtraGenericCharacterMain>();

        private ExtraGenericCharacterMain() { }

        internal static ExtraGenericCharacterMain Add(GenericCharacterMain genericCharacterMain)
        {
            return instances[genericCharacterMain] = new ExtraGenericCharacterMain();
        }

        internal static void Remove(GenericCharacterMain genericCharacterMain)
        {
            instances.Remove(genericCharacterMain);
        }

        internal static ExtraGenericCharacterMain Get(GenericCharacterMain genericCharacterMain)
        {
            if (instances.TryGetValue(genericCharacterMain, out var extraGenericCharacterMain))
            {
                return extraGenericCharacterMain;
            }
            return null;
        }

        internal static void PerformInputsOverrideHook(On.EntityStates.GenericCharacterMain.orig_PerformInputs orig, GenericCharacterMain self)
        {
            orig(self);

            if (!(bool)isAuthorityProperty.GetValue(self))
            {
                return;
            }

            var extraSkillLocator = self.outer.GetComponent<ExtraSkillLocator>();
            var extraInputBankTest = self.outer.GetComponent<ExtraInputBankTest>();
            var extraGenericCharacterMain = Get(self);

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
                if (!skillSlot || !justPressed && (!flag || skillSlot.mustKeyPress) || !self.InvokeMethod<bool>("CanExecuteSkill", skillSlot))
                    return;
                skillSlot.ExecuteIfReady();
            }
        }

        internal static void GatherInputsOverrideHook(On.EntityStates.GenericCharacterMain.orig_GatherInputs orig, GenericCharacterMain self)
        {
            orig(self);

            var extraInputBankTest = self.outer.GetComponent<ExtraInputBankTest>();
            var extraGenericCharacterMain = Get(self);
            if (!extraInputBankTest || extraGenericCharacterMain == null)
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

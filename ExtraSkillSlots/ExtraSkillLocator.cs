using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;

namespace ExtraSkillSlots
{
    /// <summary>
    /// Just a container for extra skill slots
    /// </summary>
    [RequireComponent(typeof(SkillLocator))]
    [DisallowMultipleComponent]
    public class ExtraSkillLocator : MonoBehaviour
    {
        public GenericSkill extraFirst;
        public GenericSkill extraSecond;
        public GenericSkill extraThird;
        public GenericSkill extraFourth;

        private void Awake()
        {
            if (!GetComponent<ExtraInputBankTest>())
            {
                gameObject.AddComponent<ExtraInputBankTest>();
            }
        }

        internal static GenericSkill GetSkillOverrideHook(On.RoR2.SkillLocator.orig_GetSkill orig, SkillLocator self, SkillSlot skillSlot)
        {
            var extraSkillLocator = self.GetComponent<ExtraSkillLocator>();
            if (!extraSkillLocator)
            {
                return orig(self, skillSlot);
            }

            return (ExtraSkillSlot)skillSlot switch
            {
                ExtraSkillSlot.ExtraFirst => extraSkillLocator.extraFirst,
                ExtraSkillSlot.ExtraSecond => extraSkillLocator.extraSecond,
                ExtraSkillSlot.ExtraThird => extraSkillLocator.extraThird,
                ExtraSkillSlot.ExtraFourth => extraSkillLocator.extraFourth,
                _ => orig(self, skillSlot)
            };
        }

        internal static SkillSlot FindSkillSlotOverrideHook(On.RoR2.SkillLocator.orig_FindSkillSlot orig, SkillLocator self, GenericSkill skillComponent)
        {
            var extraSkillLocator = self.GetComponent<ExtraSkillLocator>();

            if (!extraSkillLocator)
            {
                return orig(self, skillComponent);
            }

            if (!skillComponent)
            {
                return SkillSlot.None;
            }

            if (skillComponent == extraSkillLocator.extraFirst)
            {
                return (SkillSlot)ExtraSkillSlot.ExtraFirst;
            }
            if (skillComponent == extraSkillLocator.extraSecond)
            {
                return (SkillSlot)ExtraSkillSlot.ExtraSecond;
            }
            if (skillComponent == extraSkillLocator.extraThird)
            {
                return (SkillSlot)ExtraSkillSlot.ExtraThird;
            }
            if (skillComponent == extraSkillLocator.extraFourth)
            {
                return (SkillSlot)ExtraSkillSlot.ExtraFourth;
            }

            return orig(self, skillComponent);
        }
    }
}

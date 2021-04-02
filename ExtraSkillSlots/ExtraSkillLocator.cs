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
    public class ExtraSkillLocator : MonoBehaviour
    {
        public GenericSkill extraFirst;
        public GenericSkill extraSecond;
        public GenericSkill extraThird;
        public GenericSkill extraFourth;

        internal static GenericSkill GetSkillOverrideHook(On.RoR2.SkillLocator.orig_GetSkill orig, SkillLocator self, SkillSlot skillSlot)
        {
            var extraSkillLocator = self.GetComponent<ExtraSkillLocator>();
            if (extraSkillLocator)
            {
                if (skillSlot == ExtraSkillSlot.ExtraFirst)
                {
                    return extraSkillLocator.extraFirst;
                }

                if (skillSlot == ExtraSkillSlot.ExtraSecond)
                {
                    return extraSkillLocator.extraSecond;
                }

                if (skillSlot == ExtraSkillSlot.ExtraThird)
                {
                    return extraSkillLocator.extraThird;
                }

                if (skillSlot == ExtraSkillSlot.ExtraFourth)
                {
                    return extraSkillLocator.extraFourth;
                }
            }

            return orig(self, skillSlot);
        }

        internal static SkillSlot FindSkillSlotOverrideHook(On.RoR2.SkillLocator.orig_FindSkillSlot orig, SkillLocator self, GenericSkill skillComponent)
        {
            var extraSkillLocator = self.GetComponent<ExtraSkillLocator>();

            if (!skillComponent)
            {
                return SkillSlot.None;
            }

            if (extraSkillLocator)
            {
                if (skillComponent == extraSkillLocator.extraFirst)
                {
                    return ExtraSkillSlot.ExtraFirst;
                }
                if (skillComponent == extraSkillLocator.extraSecond)
                {
                    return ExtraSkillSlot.ExtraSecond;
                }
                if (skillComponent == extraSkillLocator.extraThird)
                {
                    return ExtraSkillSlot.ExtraThird;
                }
                if (skillComponent == extraSkillLocator.extraFourth)
                {
                    return ExtraSkillSlot.ExtraFourth;
                }
            }

            return orig(self, skillComponent);
        }
    }
}

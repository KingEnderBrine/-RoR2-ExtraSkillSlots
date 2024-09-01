using RoR2;
using HarmonyLib;
using UnityEngine;

namespace ExtraSkillSlots
{
    /// <summary>
    /// Just a container for extra skill slots
    /// </summary>
    [RequireComponent(typeof(SkillLocator))]
    [DisallowMultipleComponent]
    [HarmonyPatch]
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

        [HarmonyPrefix, HarmonyPatch(typeof(SkillLocator), nameof(SkillLocator.GetSkill))]
        internal static bool GetSkillOverrideHook(SkillLocator __instance, SkillSlot skillSlot, ref GenericSkill __result)
        {
            var extraSkillLocator = __instance.GetComponent<ExtraSkillLocator>();
            if (!extraSkillLocator)
            {
                return true;
            }

            var skill = (ExtraSkillSlot)skillSlot switch
            {
                ExtraSkillSlot.ExtraFirst => extraSkillLocator.extraFirst,
                ExtraSkillSlot.ExtraSecond => extraSkillLocator.extraSecond,
                ExtraSkillSlot.ExtraThird => extraSkillLocator.extraThird,
                ExtraSkillSlot.ExtraFourth => extraSkillLocator.extraFourth,
                _ => null
            };
            if (!skill) return true;
            __result = skill;
            return false;
        }

        [HarmonyPrefix, HarmonyPatch(typeof(SkillLocator), nameof(SkillLocator.FindSkillSlot))]
        internal static bool FindSkillSlotOverrideHook(SkillLocator __instance, ref SkillSlot __result, GenericSkill skillComponent)
        {
            var extraSkillLocator = __instance.GetComponent<ExtraSkillLocator>();

            if (!extraSkillLocator)
            {
                return true;
            }

            if (!skillComponent)
            {
                __result = SkillSlot.None;
                return false;
            }

            if (skillComponent == extraSkillLocator.extraFirst)
            {
                __result = (SkillSlot)ExtraSkillSlot.ExtraFirst;
                return false;
            }
            if (skillComponent == extraSkillLocator.extraSecond)
            {
                __result = (SkillSlot)ExtraSkillSlot.ExtraSecond;
                return false;
            }
            if (skillComponent == extraSkillLocator.extraThird)
            {
                __result = (SkillSlot)ExtraSkillSlot.ExtraThird;
                return false;
            }
            if (skillComponent == extraSkillLocator.extraFourth)
            {
                __result = (SkillSlot)ExtraSkillSlot.ExtraFourth;
                return false;
            }

            return true;
        }
    }
}

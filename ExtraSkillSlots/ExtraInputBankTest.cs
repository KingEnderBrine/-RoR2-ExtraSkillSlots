using HarmonyLib;
using RoR2;
using UnityEngine;

namespace ExtraSkillSlots
{
    [RequireComponent(typeof(InputBankTest))]
    [DisallowMultipleComponent]
    [HarmonyPatch]
    public class ExtraInputBankTest : MonoBehaviour
    {
        public InputBankTest.ButtonState extraSkill1;
        public InputBankTest.ButtonState extraSkill2;
        public InputBankTest.ButtonState extraSkill3;
        public InputBankTest.ButtonState extraSkill4;

        [HarmonyPostfix, HarmonyPatch(typeof(InputBankTest), nameof(InputBankTest.CheckAnyButtonDown))]
        internal static void CheckAnyButtonDownOverrideHook(InputBankTest __instance, ref bool __result)
        {
            var extraInputBankTest = __instance.GetComponent<ExtraInputBankTest>();
            __result |= extraInputBankTest &&
                        (extraInputBankTest.extraSkill1.down ||
                         extraInputBankTest.extraSkill2.down ||
                         extraInputBankTest.extraSkill3.down ||
                         extraInputBankTest.extraSkill4.down);
        }
    }
}
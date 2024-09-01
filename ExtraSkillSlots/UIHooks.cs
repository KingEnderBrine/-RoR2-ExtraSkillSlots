using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.UI;
using System;
using HarmonyLib;
using UnityEngine;

namespace ExtraSkillSlots
{
    [HarmonyPatch]
    internal static class UIHooks
    {
        [HarmonyPostfix, HarmonyPatch(typeof(HUD), nameof(HUD.Awake))]
        internal static void HUDAwake(HUD __instance)
        {
            __instance.gameObject.AddComponent<ExtraHud>();
        }

        #region SettingsPanelController
        [HarmonyPostfix, HarmonyPatch(typeof(SettingsPanelController), nameof(SettingsPanelController.Start))]
        internal static void SettingsPanelControllerStart(SettingsPanelController __instance)
        {
            if (__instance.name == "SettingsSubPanel, Controls (M&KB)" || __instance.name == "SettingsSubPanel, Controls (Gamepad)")
            {
                var jumpBindingTransform = __instance.transform.Find("Scroll View/Viewport/VerticalLayout/SettingsEntryButton, Binding (Jump)");

                AddActionBindingToSettings(RewiredAction.FirstExtraSkill.Name, jumpBindingTransform);
                AddActionBindingToSettings(RewiredAction.SecondExtraSkill.Name, jumpBindingTransform);
                AddActionBindingToSettings(RewiredAction.ThirdExtraSkill.Name, jumpBindingTransform);
                AddActionBindingToSettings(RewiredAction.FourthExtraSkill.Name, jumpBindingTransform);
            }
        }

        internal static void AddActionBindingToSettings(string actionName, Transform buttonToCopy)
        {
            var inputBindingObject = GameObject.Instantiate(buttonToCopy, buttonToCopy.parent);
            var inputBindingControl = inputBindingObject.GetComponent<InputBindingControl>();
            inputBindingControl.actionName = actionName;
            //Usualy calling awake is bad as it's supposed to be called only by unity.
            //But in this case it is necessary to apply "actionName" change.
            inputBindingControl.Awake();
        }
        #endregion

        [HarmonyILManipulator, HarmonyPatch(typeof(LoadoutPanelController.Row), nameof(LoadoutPanelController.Row.FromSkillSlot))]
        internal static void LoadoutPanelControllerFromSkillSlot(ILContext il)
        {
            var c = new ILCursor(il);

            int skillSlotIndex = -1;
            int titleTokenIndex = -1;
            int addWIPIconsIndex = -1;

            c.GotoNext(
                x => x.MatchCallOrCallvirt<SkillLocator>(nameof(SkillLocator.FindSkillSlot)),
                x => x.MatchStloc(out skillSlotIndex));

            c.GotoNext(
                x => x.MatchLdstr(out _),
                x => x.MatchStloc(out titleTokenIndex));

            c.GotoNext(
                x => x.MatchLdcI4(out _),
                x => x.MatchStloc(out addWIPIconsIndex));

            c.GotoNext(
                x => x.MatchNewobj<ArgumentOutOfRangeException>(),
                x => x.MatchThrow());

            //Replacing throwing exception by default to call custom delegate 
            c.Index += 1;
            c.Remove();
            c.Previous.OpCode = OpCodes.Nop;
            c.Previous.Operand = null;

            c.Emit(OpCodes.Ldloc, skillSlotIndex);
            c.Emit(OpCodes.Call, typeof(UIHooks).GetMethod(nameof(GetSkillSlotName), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static));
            c.Emit(OpCodes.Stloc, titleTokenIndex);
            c.Emit(OpCodes.Ldc_I4_0);
            c.Emit(OpCodes.Stloc, addWIPIconsIndex);
        }

        private static string GetSkillSlotName(SkillSlot skillSlot)
        {
            return (ExtraSkillSlot)skillSlot switch
            {
                ExtraSkillSlot.ExtraFirst => LanguageConsts.EXTRA_SKILL_SLOTS_FIRST_EXTRA_SKILL,
                ExtraSkillSlot.ExtraSecond => LanguageConsts.EXTRA_SKILL_SLOTS_SECOND_EXTRA_SKILL,
                ExtraSkillSlot.ExtraThird => LanguageConsts.EXTRA_SKILL_SLOTS_THIRD_EXTRA_SKILL,
                ExtraSkillSlot.ExtraFourth => LanguageConsts.EXTRA_SKILL_SLOTS_FOURTH_EXTRA_SKILL,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}

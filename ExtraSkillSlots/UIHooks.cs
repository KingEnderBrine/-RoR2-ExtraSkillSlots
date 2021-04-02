using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using RoR2.UI;
using System;
using UnityEngine;

namespace ExtraSkillSlots
{
    internal static class UIHooks
    {
        internal static void HUDAwake(On.RoR2.UI.HUD.orig_Awake orig, HUD self)
        {
            orig(self);

            self.gameObject.AddComponent<ExtraHud>();
        }

        #region SettingsPanelController
        internal static void SettingsPanelControllerStart(On.RoR2.UI.SettingsPanelController.orig_Start orig, RoR2.UI.SettingsPanelController self)
        {
            orig(self);

            if (self.name == "SettingsSubPanel, Controls (M&KB)" || self.name == "SettingsSubPanel, Controls (Gamepad)")
            {
                var jumpBindingTransform = self.transform.Find("Scroll View/Viewport/VerticalLayout/SettingsEntryButton, Binding (Jump)");

                AddActionBindingToSettings(RewiredActions.FirstExtraSkillName, jumpBindingTransform);
                AddActionBindingToSettings(RewiredActions.SecondExtraSkillName, jumpBindingTransform);
                AddActionBindingToSettings(RewiredActions.ThirdExtraSkillName, jumpBindingTransform);
                AddActionBindingToSettings(RewiredActions.FourthExtraSkillName, jumpBindingTransform);
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

        internal static void LoadoutPanelControllerFromSkillSlot(ILContext il)
        {
            var c = new ILCursor(il);

            c.GotoNext(
                x => x.MatchNewobj<ArgumentOutOfRangeException>(),
                x => x.MatchThrow());

            //Replacing throwing exception by default to call custom delegate 
            c.Index += 1;
            c.Remove();
            c.Previous.OpCode = OpCodes.Nop;
            c.Previous.Operand = null;

            c.Emit(OpCodes.Ldloc, 4);
            c.Emit(OpCodes.Call, typeof(UIHooks).GetMethod(nameof(GetSkillSlotName), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static));
            c.Emit(OpCodes.Stloc, 2);
            c.Emit(OpCodes.Ldc_I4_0);
            c.Emit(OpCodes.Stloc, 3);
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

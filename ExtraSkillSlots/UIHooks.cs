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
        #region HUD
        internal static void HUDAwake(On.RoR2.UI.HUD.orig_Awake orig, HUD self)
        {
            orig(self);

            var extraHud = self.gameObject.AddComponent<ExtraHud>();

            var skillsContainer = self.transform.Find("MainContainer").Find("MainUIArea").Find("BottomRightCluster").Find("Scaler");

            var firstSkill = CopyUISkillSlot(1, RewiredActions.FirstExtraSkillName, skillsContainer);
            var secondSkill = CopyUISkillSlot(2, RewiredActions.SecondExtraSkillName, skillsContainer);
            var thirdSkill = CopyUISkillSlot(3, RewiredActions.ThirdExtraSkillName, skillsContainer);
            var fourthSkill = CopyUISkillSlot(4, RewiredActions.FourthExtraSkillName, skillsContainer);

            extraHud.extraSkillIconFirst = firstSkill.GetComponent<SkillIcon>();
            extraHud.extraSkillIconSecond = secondSkill.GetComponent<SkillIcon>();
            extraHud.extraSkillIconThird = thirdSkill.GetComponent<SkillIcon>();
            extraHud.extraSkillIconFourth = fourthSkill.GetComponent<SkillIcon>();
        }

        internal static Transform CopyUISkillSlot(int slot, string actionName, Transform skillsContainer)
        {
            var skill = skillsContainer.Find($"Skill{slot}Root");
            var skillCopy = GameObject.Instantiate(skill, skill.parent);

            //Lift up copy
            var skillCopyRectTransform = skillCopy.GetComponent<RectTransform>();
            skillCopyRectTransform.anchorMin = new Vector2(1, 2.15F);
            skillCopyRectTransform.anchorMax = new Vector2(1, 2.15F);

            //Changing visual input binding
            var skillKeyText = skillCopy.Find("SkillBackgroundPanel").Find("SkillKeyText");
            var inputBindingDisplayController = skillKeyText.GetComponent<InputBindingDisplayController>();
            inputBindingDisplayController.actionName = actionName;

            return skillCopy;
        }
        #endregion

        #region SettingsPanelController
        internal static void SettingsPanelControllerStart(On.RoR2.UI.SettingsPanelController.orig_Start orig, RoR2.UI.SettingsPanelController self)
        {
            orig(self);

            if (self.name == "SettingsSubPanel, Controls (M&KB)" || self.name == "SettingsSubPanel, Controls (Gamepad)")
            {
                var jumpBindingTransform = self.transform.Find("Scroll View").Find("Viewport").Find("VerticalLayout").Find("SettingsEntryButton, Binding (Jump)");

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
            c.EmitDelegate<Func<SkillSlot, string>>((skillSlot) =>
            {
                if (skillSlot == ExtraSkillSlot.ExtraFirst)
                {
                    return LanguageConsts.EXTRA_SKILL_SLOTS_FIRST_EXTRA_SKILL;
                }
                if (skillSlot == ExtraSkillSlot.ExtraSecond)
                {
                    return LanguageConsts.EXTRA_SKILL_SLOTS_SECOND_EXTRA_SKILL;
                }
                if (skillSlot == ExtraSkillSlot.ExtraThird)
                {
                    return LanguageConsts.EXTRA_SKILL_SLOTS_THIRD_EXTRA_SKILL;
                }
                if (skillSlot == ExtraSkillSlot.ExtraFourth)
                {
                    return LanguageConsts.EXTRA_SKILL_SLOTS_FOURTH_EXTRA_SKILL;
                }

                throw new ArgumentOutOfRangeException();
            });
            c.Emit(OpCodes.Stloc, 2);
            c.Emit(OpCodes.Ldc_I4_0);
            c.Emit(OpCodes.Stloc, 3);
        }
    }
}

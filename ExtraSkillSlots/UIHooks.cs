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

        internal static void CharacterSelectControllerRebuildLocal(ILContext il)
        {
            var c = new ILCursor(il);
            var arrayIndex = -1;
            var elementIndex = -1;
            var indexIndex = -1;
            var removeFromIndex = -1;
            var removeCount = -1;
            var arrayElementUsageIndex = -1;
            Instruction loopEndInstuction = null;

            //Removing Where call because we will filter inside the loop instead
            c.GotoNext(MoveType.After, x => x.MatchCallOrCallvirt(typeof(GameObject).GetMethod(nameof(GameObject.GetComponents), Array.Empty<Type>()).MakeGenericMethod(new[] { typeof(GenericSkill) })));
            removeFromIndex = c.Index;
            c.GotoNext(x => x.MatchStloc(out arrayIndex));
            removeCount = c.Index - removeFromIndex;
            c.Index = removeFromIndex;
            c.RemoveRange(removeCount);

            //Find storing of an array element to a local variable
            c.GotoNext(MoveType.After,
                x => x.MatchLdloc(arrayIndex),
                x => x.MatchLdloc(out indexIndex),
                x => x.MatchLdelemRef(),
                x => x.MatchStloc(out elementIndex));
            arrayElementUsageIndex = c.Index;

            //Find loop end to go to it when our condition is true
            c.GotoNext(
                x => x.MatchLdloc(indexIndex),
                x => x.MatchLdcI4(1),
                x => x.MatchAdd(),
                x => x.MatchStloc(indexIndex));
            loopEndInstuction = c.Next;
            c.Index = arrayElementUsageIndex;

            //Emit condition that was in Where
            c.Emit(OpCodes.Ldloc, elementIndex);
            c.Emit(OpCodes.Ldfld, typeof(GenericSkill).GetField(nameof(GenericSkill.hideInCharacterSelect)));
            c.Emit(OpCodes.Brtrue, loopEndInstuction);
        }
    }
}

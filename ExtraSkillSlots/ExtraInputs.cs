using R2API.Utils;
using Rewired;
using Rewired.Data;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ExtraSkillSlots
{
    internal static class ExtraInputs
    {
        internal static void AddActionsToInputCatalog()
        {
            //Using reflection because "ActionAxisPair" is private
            var actionAxisPairConstructor = typeof(InputCatalog).GetNestedType("ActionAxisPair", BindingFlags.NonPublic)?.GetConstructor(new Type[] { typeof(string), typeof(AxisRange) });

            var extraActionAxisPairFirst = actionAxisPairConstructor.Invoke(new object[] { RewiredActions.FirstExtraSkillName, AxisRange.Full });
            var extraActionAxisPairSecond = actionAxisPairConstructor.Invoke(new object[] { RewiredActions.SecondExtraSkillName, AxisRange.Full });
            var extraActionAxisPairThird = actionAxisPairConstructor.Invoke(new object[] { RewiredActions.ThirdExtraSkillName, AxisRange.Full });
            var extraActionAxisPairFourth = actionAxisPairConstructor.Invoke(new object[] { RewiredActions.FourthExtraSkillName, AxisRange.Full });

            var actionToToken = typeof(InputCatalog).GetField("actionToToken", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);

            actionToToken.InvokeMethod("Add", new object[] { extraActionAxisPairFirst, LanguageConsts.EXTRA_SKILL_SLOTS_FIRST_EXTRA_SKILL });
            actionToToken.InvokeMethod("Add", new object[] { extraActionAxisPairSecond, LanguageConsts.EXTRA_SKILL_SLOTS_SECOND_EXTRA_SKILL });
            actionToToken.InvokeMethod("Add", new object[] { extraActionAxisPairThird, LanguageConsts.EXTRA_SKILL_SLOTS_THIRD_EXTRA_SKILL });
            actionToToken.InvokeMethod("Add", new object[] { extraActionAxisPairFourth, LanguageConsts.EXTRA_SKILL_SLOTS_FOURTH_EXTRA_SKILL });
        }

        internal static void AddCustomActions(Action<UserData> orig, UserData self)
        {
            var firstAction = CreateInputAction(RewiredActions.FirstExtraSkill, RewiredActions.FirstExtraSkillName);
            var secondAction = CreateInputAction(RewiredActions.SecondExtraSkill, RewiredActions.SecondExtraSkillName);
            var thirdAction = CreateInputAction(RewiredActions.ThirdExtraSkill, RewiredActions.ThirdExtraSkillName);
            var fourthAction = CreateInputAction(RewiredActions.FourthExtraSkill, RewiredActions.FourthExtraSkillName);

            var actions = self.GetFieldValue<List<InputAction>>("actions");

            actions?.Add(firstAction);
            actions?.Add(secondAction);
            actions?.Add(thirdAction);
            actions?.Add(fourthAction);

            orig(self);
        }

        internal static InputAction CreateInputAction(int id, string name, InputActionType type = InputActionType.Button)
        {
            var action = new InputAction();

            action.SetFieldValue("_id", id);
            action.SetFieldValue("_name", name);
            action.SetFieldValue("_type", type);
            action.SetFieldValue("_descriptiveName", name);
            action.SetFieldValue("_behaviorId", 0);
            action.SetFieldValue("_userAssignable", true);
            action.SetFieldValue("_categoryId", 0);

            return action;
        }
    }
}

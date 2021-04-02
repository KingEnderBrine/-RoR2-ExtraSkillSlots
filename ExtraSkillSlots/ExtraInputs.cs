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
            var extraActionAxisPairFirst = new InputCatalog.ActionAxisPair(RewiredActions.FirstExtraSkillName, AxisRange.Full);
            var extraActionAxisPairSecond = new InputCatalog.ActionAxisPair(RewiredActions.SecondExtraSkillName, AxisRange.Full);
            var extraActionAxisPairThird = new InputCatalog.ActionAxisPair(RewiredActions.ThirdExtraSkillName, AxisRange.Full);
            var extraActionAxisPairFourth = new InputCatalog.ActionAxisPair(RewiredActions.FourthExtraSkillName, AxisRange.Full);

            InputCatalog.actionToToken.Add(extraActionAxisPairFirst, LanguageConsts.EXTRA_SKILL_SLOTS_FIRST_EXTRA_SKILL);
            InputCatalog.actionToToken.Add(extraActionAxisPairSecond, LanguageConsts.EXTRA_SKILL_SLOTS_SECOND_EXTRA_SKILL);
            InputCatalog.actionToToken.Add(extraActionAxisPairThird, LanguageConsts.EXTRA_SKILL_SLOTS_THIRD_EXTRA_SKILL);
            InputCatalog.actionToToken.Add(extraActionAxisPairFourth, LanguageConsts.EXTRA_SKILL_SLOTS_FOURTH_EXTRA_SKILL);
        }

        internal static void AddCustomActions(Action<UserData> orig, UserData self)
        {
            var firstAction = CreateInputAction(RewiredActions.FirstExtraSkill, RewiredActions.FirstExtraSkillName);
            var secondAction = CreateInputAction(RewiredActions.SecondExtraSkill, RewiredActions.SecondExtraSkillName);
            var thirdAction = CreateInputAction(RewiredActions.ThirdExtraSkill, RewiredActions.ThirdExtraSkillName);
            var fourthAction = CreateInputAction(RewiredActions.FourthExtraSkill, RewiredActions.FourthExtraSkillName);

            var actions = typeof(UserData).GetField("actions", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(self) as List<InputAction>;

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

        private static void SetFieldValue<T>(this T obj, string fieldName, object value)
        {
            typeof(T).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).SetValue(obj, value);
        }
    }
}

using Rewired;
using Rewired.Data;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExtraSkillSlots
{
    internal static class ExtraInputs
    {
        internal static void AddActionsToInputCatalog()
        {
            InputCatalog.actionToToken[RewiredAction.FirstExtraSkill] = RewiredAction.FirstExtraSkill.DisplayToken;
            InputCatalog.actionToToken[RewiredAction.SecondExtraSkill] = RewiredAction.SecondExtraSkill.DisplayToken;
            InputCatalog.actionToToken[RewiredAction.ThirdExtraSkill] = RewiredAction.ThirdExtraSkill.DisplayToken;
            InputCatalog.actionToToken[RewiredAction.FourthExtraSkill] = RewiredAction.FourthExtraSkill.DisplayToken;
        }

        internal static void AddCustomActions(Action<UserData> orig, UserData self)
        {
            self.actions?.Add(RewiredAction.FirstExtraSkill);
            self.actions?.Add(RewiredAction.SecondExtraSkill);
            self.actions?.Add(RewiredAction.ThirdExtraSkill);
            self.actions?.Add(RewiredAction.FourthExtraSkill);

            orig(self);
        }

        internal static void OnLoadUserProfiles(On.RoR2.UserProfile.orig_LoadUserProfiles orig)
        {
            orig();

            foreach (var (name, userProfile) in UserProfile.loadedUserProfiles)
            {
                try
                {
                    AddMissingBingings(userProfile);
                    userProfile.RequestSave();
                }
                catch(Exception e)
                {
                    ExtraSkillSlotsPlugin.InstanceLogger.LogWarning($"Failed to add default bindings to '{name}' profile");
                    ExtraSkillSlotsPlugin.InstanceLogger.LogError(e);
                }
            }
        }

        internal static void OnLoadDefaultProfile(On.RoR2.UserProfile.orig_LoadDefaultProfile orig)
        {
            orig();

            try
            {
                AddMissingBingings(UserProfile.defaultProfile);
            }
            catch (Exception e)
            {
                ExtraSkillSlotsPlugin.InstanceLogger.LogWarning($"Failed to add default bindings to default profile");
                ExtraSkillSlotsPlugin.InstanceLogger.LogError(e);
            }
        }

        private static void AddMissingBingings(UserProfile userProfile)
        {
            AddActionMaps(RewiredAction.FirstExtraSkill, userProfile);
            AddActionMaps(RewiredAction.SecondExtraSkill, userProfile);
            AddActionMaps(RewiredAction.ThirdExtraSkill, userProfile);
            AddActionMaps(RewiredAction.FourthExtraSkill, userProfile);
        }

        private static void AddActionMaps(RewiredAction action, UserProfile userProfile)
        {
            if (userProfile.joystickMap.AllMaps.All(map => map.actionId != action.ActionId))
            {
                userProfile.joystickMap.AddElementMap(new ActionElementMap(action.ActionId, ControllerElementType.Button, action.DefaultJoystickKey, Pole.Positive, AxisRange.Full));
            }

            if (userProfile.keyboardMap.AllMaps.All(map => map.actionId != action.ActionId))
            {
                userProfile.keyboardMap.AddElementMap(new ActionElementMap(action.ActionId, ControllerElementType.Button, Pole.Positive, action.DefaultKeyboardKey, ModifierKey.None, ModifierKey.None, ModifierKey.None));
            }
        }

        private static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
        {
            key = pair.Key;
            value = pair.Value;
        }
    }
}

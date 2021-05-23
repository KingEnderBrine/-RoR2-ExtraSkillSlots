using Rewired;
using Rewired.Data;
using Rewired.Data.Mapping;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

            var joystickMap = self.joystickMaps?.FirstOrDefault();
            var keyboardMap = self.keyboardMaps?.FirstOrDefault();
            
            FillActionMaps(RewiredAction.FirstExtraSkill, keyboardMap, joystickMap);
            FillActionMaps(RewiredAction.SecondExtraSkill, keyboardMap, joystickMap);
            FillActionMaps(RewiredAction.ThirdExtraSkill, keyboardMap, joystickMap);
            FillActionMaps(RewiredAction.FourthExtraSkill, keyboardMap, joystickMap);

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

        private static void FillActionMaps(RewiredAction action, ControllerMap_Editor keyboardMap, ControllerMap_Editor joystickMap)
        {
            if (joystickMap != null && joystickMap.actionElementMaps.All(map => map.actionId != action.ActionId))
            {
                joystickMap.actionElementMaps.Add(action.DefaultJoystickMap);
            }

            if (keyboardMap != null && keyboardMap.actionElementMaps.All(map => map.actionId != action.ActionId))
            {
                keyboardMap.actionElementMaps.Add(action.DefaultKeyboardMap);
            }
        }

        private static void AddActionMaps(RewiredAction action, UserProfile userProfile)
        {
            if (userProfile.joystickMap.AllMaps.All(map => map.actionId != action.ActionId))
            {
                userProfile.joystickMap.AddElementMap(action.DefaultJoystickMap);
                action.DefaultJoystickMap.GPKKIyqEPFnzbZebduZEdqINbaj(userProfile.joystickMap);
            }

            if (userProfile.keyboardMap.AllMaps.All(map => map.actionId != action.ActionId))
            {
                userProfile.keyboardMap.AddElementMap(action.DefaultKeyboardMap);
                action.DefaultKeyboardMap.GPKKIyqEPFnzbZebduZEdqINbaj(userProfile.keyboardMap);
            }
        }

        private static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
        {
            key = pair.Key;
            value = pair.Value;
        }
    }
}

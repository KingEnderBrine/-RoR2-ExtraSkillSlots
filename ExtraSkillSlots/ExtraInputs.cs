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
            InputCatalog.actionToToken[RewiredAction.ExtraModifier] = RewiredAction.ExtraModifier.DisplayToken;
        }

        internal static void AddCustomActions(Action<UserData> orig, UserData self)
        {
            self.mapCategories?.Add(RewiredAction.ExtraSkillsCategory);
            self.actions?.Add(RewiredAction.FirstExtraSkill);
            self.actions?.Add(RewiredAction.SecondExtraSkill);
            self.actions?.Add(RewiredAction.ThirdExtraSkill);
            self.actions?.Add(RewiredAction.FourthExtraSkill);
            self.actions?.Add(RewiredAction.ExtraModifier);

            FillActionMaps(RewiredAction.FirstExtraSkill, self);
            FillActionMaps(RewiredAction.SecondExtraSkill, self);
            FillActionMaps(RewiredAction.ThirdExtraSkill, self);
            FillActionMaps(RewiredAction.FourthExtraSkill, self);
            FillActionMaps(RewiredAction.ExtraModifier, self);

            orig(self);
        }

        internal static void OnLoadUserProfiles(On.RoR2.SaveSystem.orig_LoadUserProfiles orig, SaveSystem self)
        {
            orig(self);

            foreach (var (name, userProfile) in self.loadedUserProfiles)
            {
                try
                {
                    AddMissingBindings(userProfile);
                    userProfile.RequestEventualSave();
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
                AddMissingBindings(UserProfile.defaultProfile);
            }
            catch (Exception e)
            {
                ExtraSkillSlotsPlugin.InstanceLogger.LogWarning($"Failed to add default bindings to default profile");
                ExtraSkillSlotsPlugin.InstanceLogger.LogError(e);
            }
        }

        internal static void OnFillDefaultJoystickMaps(On.RoR2.UserProfile.orig_FillDefaultJoystickMaps orig, UserProfile self)
        {
            orig(self);
            AddMissingBindings(self);
        }

        internal static bool InputMapperHelperDoActionElementMapsHaveOverlappingElementOrAction(On.RoR2.InputMapperHelper.orig_DoActionElementMapsHaveOverlappingElementOrAction orig, Rewired.ActionElementMap a, Rewired.ActionElementMap b)
        {
            if ((a.controllerMap is JoystickMap && a.actionId >= RewiredAction.FirstExtraSkill.ActionId && a.actionId <= RewiredAction.FourthExtraSkill.ActionId) !=
                (b.controllerMap is JoystickMap && b.actionId >= RewiredAction.FirstExtraSkill.ActionId && b.actionId <= RewiredAction.FourthExtraSkill.ActionId))
            {
                return false;
            }

            return orig(a, b);
        }

        private static void AddMissingBindings(UserProfile userProfile)
        {
            AddActionMaps(RewiredAction.FirstExtraSkill, userProfile);
            AddActionMaps(RewiredAction.SecondExtraSkill, userProfile);
            AddActionMaps(RewiredAction.ThirdExtraSkill, userProfile);
            AddActionMaps(RewiredAction.FourthExtraSkill, userProfile);
            AddActionMaps(RewiredAction.ExtraModifier, userProfile);
        }

        private static void FillActionMaps(RewiredAction action, UserData userData)
        {
            foreach (var joystickMap in userData.joystickMaps)
            {
                if (joystickMap.categoryId == 0 && joystickMap.actionElementMaps.All(map => map.actionId != action.ActionId))
                {
                    joystickMap.actionElementMaps.Add(action.DefaultJoystickMap);
                }
            }

            foreach (var keyboardMap in userData.keyboardMaps)
            {
                if (keyboardMap.categoryId == 0 && keyboardMap.actionElementMaps.All(map => map.actionId != action.ActionId))
                {
                    keyboardMap.actionElementMaps.Add(action.DefaultKeyboardMap);
                }
            }
        }

        private static void AddActionMaps(RewiredAction action, UserProfile userProfile)
        {
            foreach (var (_, map) in userProfile.HardwareJoystickMaps2)
            {
                if (map.AllMaps.All(map => map.actionId != action.ActionId))
                {
                    map.CreateElementMap(action.DefaultJoystickMap.actionId, action.DefaultJoystickMap.axisContribution, action.DefaultJoystickMap.elementIdentifierId, action.DefaultJoystickMap.elementType, action.DefaultJoystickMap.axisRange, action.DefaultJoystickMap.invert);
                    action.DefaultJoystickMap.cVYzDXVDMNXvRMrklCVdVyeXGAlK(map);
                }
            }

            if (userProfile.keyboardMap.AllMaps.All(map => map.actionId != action.ActionId))
            {
                userProfile.keyboardMap.CreateElementMap(action.DefaultKeyboardMap.actionId, action.DefaultKeyboardMap.axisContribution, action.DefaultKeyboardMap.elementIdentifierId, action.DefaultJoystickMap.elementType, action.DefaultJoystickMap.axisRange, action.DefaultJoystickMap.invert, out var resultMap);
                resultMap._keyboardKeyCode = action.DefaultKeyboardKey;
                action.DefaultKeyboardMap.cVYzDXVDMNXvRMrklCVdVyeXGAlK(userProfile.keyboardMap);
            }
        }
    }
}

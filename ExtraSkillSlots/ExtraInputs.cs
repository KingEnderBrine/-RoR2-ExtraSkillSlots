using Rewired;
using Rewired.Data;
using Rewired.Data.Mapping;
using RoR2;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace ExtraSkillSlots
{
    internal static class ExtraInputs
    {
        [HarmonyPatch]
        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        internal static class RewiredPatch
        {
            internal static MethodBase TargetMethod()
            {
                return AccessTools.FirstMethod(typeof(ReInput), method =>
                {
                    var parameters = method.GetParameters();
                    return parameters.Any(param => param.ParameterType == typeof(InputManager_Base)) && parameters.Any(param => param.ParameterType == typeof(UserData));
                });
            }
            
            internal static void Prefix(object[] __args)
            {
                var self = (UserData) __args.First(obj => obj is UserData);
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
            }
        }

        internal static void AddActionsToInputCatalog()
        {
            InputCatalog.actionToToken[RewiredAction.FirstExtraSkill] = RewiredAction.FirstExtraSkill.DisplayToken;
            InputCatalog.actionToToken[RewiredAction.SecondExtraSkill] = RewiredAction.SecondExtraSkill.DisplayToken;
            InputCatalog.actionToToken[RewiredAction.ThirdExtraSkill] = RewiredAction.ThirdExtraSkill.DisplayToken;
            InputCatalog.actionToToken[RewiredAction.FourthExtraSkill] = RewiredAction.FourthExtraSkill.DisplayToken;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(SaveSystem), nameof(SaveSystem.LoadUserProfiles))]
        internal static void OnLoadUserProfiles(SaveSystem __instance)
        {
            foreach (var (name, userProfile) in __instance.loadedUserProfiles)
            {
                try
                {
                    AddMissingBingings(userProfile);
                    userProfile.RequestEventualSave();
                }
                catch (Exception e)
                {
                    ExtraSkillSlotsPlugin.InstanceLogger.LogWarning(
                        $"Failed to add default bindings to '{name}' profile");
                    ExtraSkillSlotsPlugin.InstanceLogger.LogError(e);
                }
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(UserProfile), nameof(UserProfile.LoadDefaultProfile))]
        internal static void OnLoadDefaultProfile()
        {
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

        private static void FillActionMaps(RewiredAction action, ControllerMap_Editor keyboardMap,
            ControllerMap_Editor joystickMap)
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
                userProfile.joystickMap.CreateElementMap(action.DefaultJoystickMap.actionId,
                    action.DefaultJoystickMap.axisContribution, action.DefaultJoystickMap.elementIdentifierId,
                    action.DefaultJoystickMap.elementType, action.DefaultJoystickMap.axisRange,
                    action.DefaultJoystickMap.invert);
                AddToMap(action.DefaultJoystickMap, userProfile.joystickMap);
            }

            if (userProfile.keyboardMap.AllMaps.All(map => map.actionId != action.ActionId))
            {
                userProfile.keyboardMap.CreateElementMap(action.DefaultKeyboardMap.actionId,
                    action.DefaultKeyboardMap.axisContribution, action.DefaultKeyboardMap.elementIdentifierId,
                    action.DefaultJoystickMap.elementType, action.DefaultJoystickMap.axisRange,
                    action.DefaultJoystickMap.invert, out var resultMap);
                resultMap._keyboardKeyCode = action.DefaultKeyboardKey;
                AddToMap(action.DefaultKeyboardMap, userProfile.keyboardMap);
            }
        }

        private static MethodInfo _addToMap;

        private static void AddToMap(ActionElementMap actionDefaultJoystickMap, ControllerMap userProfileJoystickMap)
        {
            _addToMap ??= AccessTools.FirstMethod(typeof(ActionElementMap),
                method => method.GetParameters()
                    .All(param => typeof(ControllerMap).IsAssignableFrom(param.ParameterType)));
            _addToMap.Invoke(actionDefaultJoystickMap, new object[] { userProfileJoystickMap });
        }

        private static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key,
            out TValue value)
        {
            key = pair.Key;
            value = pair.Value;
        }
    }
}
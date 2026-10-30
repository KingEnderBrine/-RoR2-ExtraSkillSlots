using System;
using Rewired;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.Networking;

namespace ExtraSkillSlots
{
    internal class ExtraPlayerCharacterMasterController : NetworkBehaviour
    {
        private PlayerCharacterMasterController playerCharacterMasterController;
        private ExtraInputBankTest extraInputBankTest;
        private PreButtons<PlayerCharacterMasterController.ButtonBuffer> buttonBuffers;
        private PreButtons<int> buttonCollisions;
        private static PlayerCharacterMasterController.ButtonBuffer bufferDiscard;

        public void Awake()
        {
            playerCharacterMasterController = GetComponent<PlayerCharacterMasterController>();
            InputBindingDisplayController.onBindingsChanged += RebuildCollisions;
        }

        public void Start()
        {
            RebuildCollisions();
        }

        public void OnDestroy()
        {
            InputBindingDisplayController.onBindingsChanged -= RebuildCollisions;
        }

        public void Update()
        {
            if (!playerCharacterMasterController.hasEffectiveAuthority || !playerCharacterMasterController.networkUser)
            {
                return;
            }

            var networkUser = playerCharacterMasterController.networkUser;
            var inputPlayer = networkUser.inputPlayer;
            buttonBuffers.extraSkill1.PushValue(inputPlayer.GetButton(RewiredAction.FirstExtraSkill.ActionId));
            buttonBuffers.extraSkill2.PushValue(inputPlayer.GetButton(RewiredAction.SecondExtraSkill.ActionId));
            buttonBuffers.extraSkill3.PushValue(inputPlayer.GetButton(RewiredAction.ThirdExtraSkill.ActionId));
            buttonBuffers.extraSkill4.PushValue(inputPlayer.GetButton(RewiredAction.FourthExtraSkill.ActionId));

            if (networkUser.localUser?.isUIFocused ?? false)
            {
                SuppressAllExtraButtons();
            }
            else if (networkUser.localUser.eventSystem?.currentInputSource == MPEventSystem.InputSource.Gamepad)
            {
                if (inputPlayer.GetButton(RewiredAction.ExtraModifier.ActionId))
                {
                    HandleButtonSuppression(ref buttonBuffers.extraSkill1, ref GetVanillaButtonBuffer(buttonCollisions.extraSkill1));
                    HandleButtonSuppression(ref buttonBuffers.extraSkill2, ref GetVanillaButtonBuffer(buttonCollisions.extraSkill2));
                    HandleButtonSuppression(ref buttonBuffers.extraSkill3, ref GetVanillaButtonBuffer(buttonCollisions.extraSkill3));
                    HandleButtonSuppression(ref buttonBuffers.extraSkill4, ref GetVanillaButtonBuffer(buttonCollisions.extraSkill4));
                }
                else
                {
                    HandleButtonSuppression(ref GetVanillaButtonBuffer(buttonCollisions.extraSkill1), ref buttonBuffers.extraSkill1);
                    HandleButtonSuppression(ref GetVanillaButtonBuffer(buttonCollisions.extraSkill2), ref buttonBuffers.extraSkill2);
                    HandleButtonSuppression(ref GetVanillaButtonBuffer(buttonCollisions.extraSkill3), ref buttonBuffers.extraSkill3);
                    HandleButtonSuppression(ref GetVanillaButtonBuffer(buttonCollisions.extraSkill4), ref buttonBuffers.extraSkill4);
                }
            }
        }

        private void HandleButtonSuppression(ref PlayerCharacterMasterController.ButtonBuffer currentBuffer, ref PlayerCharacterMasterController.ButtonBuffer previousBuffer)
        {
            if (previousBuffer.previousPollReportedValue)
            {
                currentBuffer.SuppressUntilReleased();
            }
            else
            {
                previousBuffer.SuppressUntilReleased();
            }
        }

        private void SuppressAllExtraButtons()
        {
            buttonBuffers.extraSkill1.SuppressUntilReleased();
            buttonBuffers.extraSkill2.SuppressUntilReleased();
            buttonBuffers.extraSkill3.SuppressUntilReleased();
            buttonBuffers.extraSkill4.SuppressUntilReleased();
        }

        public void FixedUpdate()
        {
            if (!extraInputBankTest || !playerCharacterMasterController.hasEffectiveAuthority)
            {
                return;
            }

            var pollResults = new PreButtons<PlayerCharacterMasterController.ButtonBuffer.PollResult>
            {
                extraSkill1 = buttonBuffers.extraSkill1.Poll(),
                extraSkill2 = buttonBuffers.extraSkill2.Poll(),
                extraSkill3 = buttonBuffers.extraSkill3.Poll(),
                extraSkill4 = buttonBuffers.extraSkill4.Poll(),
            };

            if (!PlayerCharacterMasterController.CanSendBodyInput(playerCharacterMasterController.networkUser, out _, out _, out _, out var onlyAllowMovement) || onlyAllowMovement)
            {
                pollResults = default;
            }

            extraInputBankTest.extraSkill1.PushState(pollResults.extraSkill1.value);
            extraInputBankTest.extraSkill2.PushState(pollResults.extraSkill2.value);
            extraInputBankTest.extraSkill3.PushState(pollResults.extraSkill3.value);
            extraInputBankTest.extraSkill4.PushState(pollResults.extraSkill4.value);
        }

        internal static void SetBodyOverrideHook(On.RoR2.PlayerCharacterMasterController.orig_SetBody orig, PlayerCharacterMasterController self,  GameObject newBody)
        {
            orig(self, newBody);

            var extraMaster = self.GetComponent<ExtraPlayerCharacterMasterController>();
            if (!extraMaster)
            {
                return;
            }

            extraMaster.extraInputBankTest = self.body ? self.body.GetComponent<ExtraInputBankTest>() : null;
        }

        internal static void AwakeHook(On.RoR2.PlayerCharacterMasterController.orig_Awake orig, PlayerCharacterMasterController self)
        {
            orig(self);
            self.gameObject.AddComponent<ExtraPlayerCharacterMasterController>();
        }

        public ref PlayerCharacterMasterController.ButtonBuffer GetVanillaButtonBuffer(int index)
        {
            switch (index)
            {
                case 0: return ref playerCharacterMasterController.buttonBuffers.skill1;
                case 1: return ref playerCharacterMasterController.buttonBuffers.skill2;
                case 2: return ref playerCharacterMasterController.buttonBuffers.skill3;
                case 3: return ref playerCharacterMasterController.buttonBuffers.skill4;
                case 4: return ref playerCharacterMasterController.buttonBuffers.interact;
                case 5: return ref playerCharacterMasterController.buttonBuffers.sprint;
                case 6: return ref playerCharacterMasterController.buttonBuffers.activateEquipment;
                case 7: return ref playerCharacterMasterController.buttonBuffers.jump;
                case 8: return ref playerCharacterMasterController.buttonBuffers.ping;
                case 9: return ref playerCharacterMasterController.buttonBuffers.activateKeyItem;
                case 10: return ref playerCharacterMasterController.buttonBuffers.keyItemNextElement;
                case 11: return ref playerCharacterMasterController.buttonBuffers.keyItemPrevElement;
                default: return ref bufferDiscard;
            }
        }

        private void RebuildCollisions()
        {
            if (!playerCharacterMasterController || !playerCharacterMasterController.networkUser || playerCharacterMasterController.networkUser.localUser == null)
            {
                ResetCollisions();
                return;
            }

            var eventSystem = playerCharacterMasterController.networkUser.localUser.eventSystem;
            if (!eventSystem)
            {
                ResetCollisions();
                return;
            }

            var glyphs = new string[]
            {
                GetGlyphString(eventSystem, RewiredConsts.Action.PrimarySkill),
                GetGlyphString(eventSystem, RewiredConsts.Action.SecondarySkill),
                GetGlyphString(eventSystem, RewiredConsts.Action.UtilitySkill),
                GetGlyphString(eventSystem, RewiredConsts.Action.SpecialSkill),
                GetGlyphString(eventSystem, RewiredConsts.Action.Interact),
                GetGlyphString(eventSystem, RewiredConsts.Action.Sprint),
                GetGlyphString(eventSystem, RewiredConsts.Action.Equipment),
                GetGlyphString(eventSystem, RewiredConsts.Action.Jump),
                GetGlyphString(eventSystem, RewiredConsts.Action.Ping),
                GetGlyphString(eventSystem, RewiredConsts.Action.UseKeyItem),
                GetGlyphString(eventSystem, RewiredConsts.Action.KeyItemChangeElement, AxisRange.Positive),
                GetGlyphString(eventSystem, RewiredConsts.Action.KeyItemChangeElement, AxisRange.Negative),
            };

            buttonCollisions.extraSkill1 = glyphs.IndexOf(GetGlyphString(eventSystem, RewiredAction.FirstExtraSkill.ActionId));
            buttonCollisions.extraSkill2 = glyphs.IndexOf(GetGlyphString(eventSystem, RewiredAction.SecondExtraSkill.ActionId));
            buttonCollisions.extraSkill3 = glyphs.IndexOf(GetGlyphString(eventSystem, RewiredAction.ThirdExtraSkill.ActionId));
            buttonCollisions.extraSkill4 = glyphs.IndexOf(GetGlyphString(eventSystem, RewiredAction.FourthExtraSkill.ActionId));
        }

        private void ResetCollisions()
        {
            buttonCollisions.extraSkill1 = -1;
            buttonCollisions.extraSkill2 = -1;
            buttonCollisions.extraSkill3 = -1;
            buttonCollisions.extraSkill4 = -1;
        }

        private string GetGlyphString(MPEventSystem eventSystem, int actionId, AxisRange axisRange = AxisRange.Full)
        {
            return Glyphs.GetGlyphString(eventSystem, ReInput.mapping.GetAction(actionId).name, axisRange);
        }

        private struct PreButtons<T>
        {
            public T extraSkill1;
            public T extraSkill2;
            public T extraSkill3;
            public T extraSkill4;
        }
    }
}

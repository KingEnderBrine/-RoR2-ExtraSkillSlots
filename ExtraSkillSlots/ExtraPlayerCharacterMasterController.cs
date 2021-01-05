using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace ExtraSkillSlots
{
    internal class ExtraPlayerCharacterMasterController : NetworkBehaviour
    {
        private PlayerCharacterMasterController playerCharacterMasterController;
        private ExtraInputBankTest extraInputBankTest;

        public void Awake()
        {
            playerCharacterMasterController = GetComponent<PlayerCharacterMasterController>();
        }

        public void FixedUpdate()
        {
            if (!extraInputBankTest || !playerCharacterMasterController.hasEffectiveAuthority || !extraInputBankTest)
            {
                return;
            }

            var skill1State = false;
            var skill2State = false;
            var skill3State = false;
            var skill4State = false;

            if (PlayerCharacterMasterController.CanSendBodyInput(playerCharacterMasterController.networkUser, out _, out var inputPlayer, out _))
            {
                skill1State = inputPlayer.GetButton(RewiredActions.FirstExtraSkill);
                skill2State = inputPlayer.GetButton(RewiredActions.SecondExtraSkill);
                skill3State = inputPlayer.GetButton(RewiredActions.ThirdExtraSkill);
                skill4State = inputPlayer.GetButton(RewiredActions.FourthExtraSkill);
            }

            extraInputBankTest.extraSkill1.PushState(skill1State);
            extraInputBankTest.extraSkill2.PushState(skill2State);
            extraInputBankTest.extraSkill3.PushState(skill3State);
            extraInputBankTest.extraSkill4.PushState(skill4State);
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
    }
}

using R2API.Utils;
using Rewired;
using RoR2;
using System.Reflection;
using UnityEngine.Networking;

namespace ExtraSkillSlots
{
    internal class ExtraPlayerCharacterMasterController : NetworkBehaviour
    {
        private PlayerCharacterMasterController playerCharacterMasterController;
        private static readonly MethodInfo canSendBodyInputMethod = typeof(PlayerCharacterMasterController).GetMethod("CanSendBodyInput", BindingFlags.NonPublic | BindingFlags.Static);

        public void Awake()
        {
            playerCharacterMasterController = GetComponent<PlayerCharacterMasterController>();
        }

        public void FixedUpdate()
        {
            var body = playerCharacterMasterController.GetFieldValue<CharacterBody>("body");
            if (!body)
            {
                return;
            }

            var extraInputBankTest = body.GetComponent<ExtraInputBankTest>();
            if (!playerCharacterMasterController.hasEffectiveAuthority || !extraInputBankTest)
            {
                return;
            }

            var skill1State = false;
            var skill2State = false;
            var skill3State = false;
            var skill4State = false;

            LocalUser localUser = null;
            Player inputPlayer = null;
            CameraRigController cameraRigController = null;
            var args = new object[]
            {
                playerCharacterMasterController.networkUser,
                localUser,
                inputPlayer,
                cameraRigController
            };

            if ((bool)canSendBodyInputMethod.Invoke(null, args))
            {
                inputPlayer = args[2] as Player;

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
    }
}

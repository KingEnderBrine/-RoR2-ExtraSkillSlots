using HarmonyLib;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace ExtraSkillSlots
{
    [HarmonyPatch]
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
        
            if (PlayerCharacterMasterController.CanSendBodyInput(playerCharacterMasterController.networkUser, out _, out var inputPlayer, out _, out _))
            {
                skill1State = inputPlayer.GetButton(RewiredAction.FirstExtraSkill.ActionId);
                skill2State = inputPlayer.GetButton(RewiredAction.SecondExtraSkill.ActionId);
                skill3State = inputPlayer.GetButton(RewiredAction.ThirdExtraSkill.ActionId);
                skill4State = inputPlayer.GetButton(RewiredAction.FourthExtraSkill.ActionId);
            }

            extraInputBankTest.extraSkill1.PushState(skill1State);
            extraInputBankTest.extraSkill2.PushState(skill2State);
            extraInputBankTest.extraSkill3.PushState(skill3State);
            extraInputBankTest.extraSkill4.PushState(skill4State);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerCharacterMasterController), nameof(PlayerCharacterMasterController.SetBody))]
        internal static void SetBodyOverrideHook(PlayerCharacterMasterController __instance, GameObject newBody)
        {
            var extraMaster = __instance.GetComponent<ExtraPlayerCharacterMasterController>();
            if (!extraMaster)
            {
                return;
            }

            extraMaster.extraInputBankTest = __instance.body ? __instance.body.GetComponent<ExtraInputBankTest>() : null;
        }
        
        [HarmonyPostfix, HarmonyPatch(typeof(PlayerCharacterMasterController), nameof(PlayerCharacterMasterController.Awake))]
        internal static void AwakeHook(PlayerCharacterMasterController __instance)
        {
            __instance.gameObject.AddComponent<ExtraPlayerCharacterMasterController>();
        }
    }
}

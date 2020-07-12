using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ExtraSkillSlots
{
    [RequireComponent(typeof(HUD))]
    internal class ExtraHud : MonoBehaviour
    {
        private HUD hud;
        public SkillIcon extraSkillIconFirst;
        public SkillIcon extraSkillIconSecond;
        public SkillIcon extraSkillIconThird;
        public SkillIcon extraSkillIconFourth;

        public void Awake()
        {
            hud = GetComponent<HUD>();
        }

        public void Update()
        {
            if (hud.targetBodyObject)
            {
                var component = hud.targetBodyObject.GetComponent<ExtraSkillLocator>();
                if (component)
                {
                    var masterController = hud.targetMaster ? hud.targetMaster.GetComponent<PlayerCharacterMasterController>() : null;
                    
                    if (extraSkillIconFirst)
                    {
                        extraSkillIconFirst.gameObject.SetActive(component.extraFirst);
                        extraSkillIconFirst.targetSkillSlot = ExtraSkillSlot.ExtraFirst;
                        extraSkillIconFirst.targetSkill = component.extraFirst;
                        extraSkillIconFirst.playerCharacterMasterController = masterController;
                    }
                    if (extraSkillIconSecond)
                    {
                        extraSkillIconSecond.gameObject.SetActive(component.extraSecond);
                        extraSkillIconSecond.targetSkillSlot = ExtraSkillSlot.ExtraSecond;
                        extraSkillIconSecond.targetSkill = component.extraSecond;
                        extraSkillIconSecond.playerCharacterMasterController = masterController;
                    }
                    if (extraSkillIconThird)
                    {
                        extraSkillIconThird.gameObject.SetActive(component.extraThird);
                        extraSkillIconThird.targetSkillSlot = ExtraSkillSlot.ExtraThird;
                        extraSkillIconThird.targetSkill = component.extraThird;
                        extraSkillIconThird.playerCharacterMasterController = masterController;
                    }
                    if (extraSkillIconFourth)
                    {
                        extraSkillIconFourth.gameObject.SetActive(component.extraFourth);
                        extraSkillIconFourth.targetSkillSlot = ExtraSkillSlot.ExtraFourth;
                        extraSkillIconFourth.targetSkill = component.extraFourth;
                        extraSkillIconFourth.playerCharacterMasterController = masterController;
                    }
                }
                else
                {
                    if (extraSkillIconFirst)
                    {
                        extraSkillIconFirst.gameObject.SetActive(false);
                    }
                    if (extraSkillIconSecond)
                    {
                        extraSkillIconSecond.gameObject.SetActive(false);
                    }
                    if (extraSkillIconThird)
                    {
                        extraSkillIconThird.gameObject.SetActive(false);
                    }
                    if (extraSkillIconFourth)
                    {
                        extraSkillIconFourth.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}

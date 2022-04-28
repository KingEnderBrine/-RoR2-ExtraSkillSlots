using RoR2;
using RoR2.UI;
using System;
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

        private void Awake()
        {
            hud = GetComponent<HUD>();
            var skillsContainer = hud.skillIcons;

            extraSkillIconFirst = CopyUISkillSlot(RewiredAction.FirstExtraSkill.Name, skillsContainer[0]);
            extraSkillIconSecond = CopyUISkillSlot(RewiredAction.SecondExtraSkill.Name, skillsContainer[1]);
            extraSkillIconThird = CopyUISkillSlot(RewiredAction.ThirdExtraSkill.Name, skillsContainer[2]);
            extraSkillIconFourth = CopyUISkillSlot(RewiredAction.FourthExtraSkill.Name, skillsContainer[3]);
        }

        internal static SkillIcon CopyUISkillSlot(string actionName, SkillIcon skillsContainer)
        {
            var skill = skillsContainer.gameObject.transform;
            var skillCopy = Instantiate(skill, skill.parent);

            //Lift up copy
            var skillCopyRectTransform = skillCopy.GetComponent<RectTransform>();
            skillCopyRectTransform.anchorMin = new Vector2(1, 2.5F);
            skillCopyRectTransform.anchorMax = new Vector2(1, 2.5F);

            //Changing visual input binding
            var inputBindingDisplayController = skillCopy.GetComponentInChildren<InputBindingDisplayController>();
            inputBindingDisplayController.actionName = actionName;

            return skillCopy.GetComponent<SkillIcon>();
        }

        private void Update()
        {
            if (!hud.targetBodyObject)
            {
                return;
            }
            var extraSkillLocator = hud.targetBodyObject.GetComponent<ExtraSkillLocator>();
            if (extraSkillLocator)
            {
                var masterController = hud.targetMaster ? hud.targetMaster.playerCharacterMasterController : null;

                if (extraSkillIconFirst)
                {
                    extraSkillIconFirst.gameObject.SetActive(ShouldShow(extraSkillLocator.extraFirst));
                    extraSkillIconFirst.targetSkillSlot = (SkillSlot)ExtraSkillSlot.ExtraFirst;
                    extraSkillIconFirst.targetSkill = extraSkillLocator.extraFirst;
                    extraSkillIconFirst.playerCharacterMasterController = masterController;
                }
                if (extraSkillIconSecond)
                {
                    extraSkillIconSecond.gameObject.SetActive(ShouldShow(extraSkillLocator.extraSecond));
                    extraSkillIconSecond.targetSkillSlot = (SkillSlot)ExtraSkillSlot.ExtraSecond;
                    extraSkillIconSecond.targetSkill = extraSkillLocator.extraSecond;
                    extraSkillIconSecond.playerCharacterMasterController = masterController;
                }
                if (extraSkillIconThird)
                {
                    extraSkillIconThird.gameObject.SetActive(ShouldShow(extraSkillLocator.extraThird));
                    extraSkillIconThird.targetSkillSlot = (SkillSlot)ExtraSkillSlot.ExtraThird;
                    extraSkillIconThird.targetSkill = extraSkillLocator.extraThird;
                    extraSkillIconThird.playerCharacterMasterController = masterController;
                }
                if (extraSkillIconFourth)
                {
                    extraSkillIconFourth.gameObject.SetActive(ShouldShow(extraSkillLocator.extraFourth));
                    extraSkillIconFourth.targetSkillSlot = (SkillSlot)ExtraSkillSlot.ExtraFourth;
                    extraSkillIconFourth.targetSkill = extraSkillLocator.extraFourth;
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

        private bool ShouldShow(GenericSkill skill)
        {
            return skill && skill.skillDef && !skill.skillDef.skillName.Equals("Disabled", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

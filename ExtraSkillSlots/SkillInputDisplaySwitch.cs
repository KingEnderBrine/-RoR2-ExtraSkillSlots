using System;
using System.Collections.Generic;
using System.Text;
using RoR2.UI;
using UnityEngine;

namespace ExtraSkillSlots
{
    public class SkillInputDisplaySwitch : MonoBehaviour
    {
        public GameObject modifierGO;
        private MPEventSystem.InputSource lastSource = (MPEventSystem.InputSource)(-1);
        private MPEventSystemLocator eventSystemLocator;

        public void Awake()
        {
            eventSystemLocator = GetComponent<MPEventSystemLocator>();
            ExtraSkillSlotsPlugin.ModifierButtonBehavior.SettingChanged += ModifierButtonBehaviorSettingChanged;
        }

        public void OnDestroy()
        {
            ExtraSkillSlotsPlugin.ModifierButtonBehavior.SettingChanged -= ModifierButtonBehaviorSettingChanged;
        }

        private void ModifierButtonBehaviorSettingChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        public void Update()
        {
            if (!eventSystemLocator.eventSystem)
            {
                return;
            }

            var currentSource = eventSystemLocator.eventSystem.currentInputSource;
            if (lastSource != currentSource)
            {
                lastSource = currentSource;
                Refresh();
            }
        }

        private void Refresh()
        {
            if (lastSource == MPEventSystem.InputSource.MouseAndKeyboard || ExtraSkillSlotsPlugin.ModifierButtonBehavior.Value == ModifierButtonBehavior.Ignore)
            {
                modifierGO.SetActive(false);
                transform.localScale = Vector3.one;
            }
            else if (lastSource == MPEventSystem.InputSource.Gamepad)
            {
                modifierGO.SetActive(true);
                transform.localScale = new Vector3(0.7F, 0.7F, 0.7F);
            }
        }
    }
}

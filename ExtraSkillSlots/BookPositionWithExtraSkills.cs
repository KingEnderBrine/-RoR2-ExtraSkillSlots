using System;
using System.Collections.Generic;
using System.Text;
using RoR2;
using RoR2.UI;
using UnityEngine;

namespace ExtraSkillSlots
{
    public class BookPositionWithExtraSkills : MonoBehaviour
    {
        private CursedBookExtraDisplay display;
        private KeyItemIconExtraDisplay displayIcon;
        private Vector2 origPivot;
        private Vector2 origPosition;

        public void Awake()
        {
            display = GetComponent<CursedBookExtraDisplay>();
            displayIcon = GetComponent<KeyItemIconExtraDisplay>();

            var holder = display.spellSlotHolder;
            origPivot = holder.pivot;
            origPosition = holder.anchoredPosition;

            Refresh(displayIcon.bodyBehavior?.body);
            displayIcon.onBodyBehaviorDiscovered += OnBodyBehaviorDiscovered;
            displayIcon.onBodyBehaviorLost += OnBodyBehaviorLost;
        }

        public void OnDestroy()
        {
            displayIcon.onBodyBehaviorDiscovered -= OnBodyBehaviorDiscovered;
            displayIcon.onBodyBehaviorLost -= OnBodyBehaviorLost;
        }

        private void OnBodyBehaviorDiscovered(RoR2.Items.BaseKeyItemBodyBehavior behavior)
        {
            Refresh(behavior.body);
        }

        private void OnBodyBehaviorLost(RoR2.Items.BaseKeyItemBodyBehavior behavior)
        {
            Refresh(null);
        }

        private void Refresh(CharacterBody body)
        {
            var holder = display.spellSlotHolder;
            if (body && body.GetComponent<ExtraSkillLocator>())
            {
                holder.pivot = new Vector2(1, holder.pivot.y);
                holder.anchoredPosition = new Vector2(20, holder.anchoredPosition.y);
            }
            else
            {
                holder.pivot = origPivot;
                holder.anchoredPosition = origPosition;
            }
        }
    }
}

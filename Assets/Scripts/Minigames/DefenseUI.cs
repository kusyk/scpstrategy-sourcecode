using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Minigames
{
    public class DefenseUI : MinigameUI
    {
        public Image safetyBar;

        public override void UpdateUI()
        {
            if(DefenseMinigame.MinigameInstance == null || safetyBar == null)
                return;

            float targetFill = (float)DefenseMinigame.MinigameInstance.currentStrength / DefenseMinigame.MinigameInstance.GetRealInitialStrenght;
            safetyBar.fillAmount = Mathf.MoveTowards(safetyBar.fillAmount, targetFill, Time.deltaTime);
        }

        private void Update()
        {
            UpdateUI();
        }
    }
}
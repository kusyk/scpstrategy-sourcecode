using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Minigames
{
    public class WavesUI : MinigameUI
    {
        public Color standardTextColor;
        public Color successTextColor;
        public List<WaveLabels> buttonsLabels = new List<WaveLabels>();
        

        public void ChangeWave(string changeInfo)
        {
            if (changeInfo.Length != 3)
            {
                Debug.Log("Wrong button info!");
                return;
            }
            char[] info = changeInfo.ToUpper().ToCharArray();
            WavesMinigame.ChangeLineSettings(info);

            UpdateUI();
        }

        public override void UpdateUI()
        {
            if(buttonsLabels.Count != WavesMinigame.MinigameInstance.waves.Count)
            {
                Debug.LogWarning("Different buttons and waves counts");
                return;
            }

            for (int i = 0; i < buttonsLabels.Count; i++)
            {
                buttonsLabels[i].heightLabel.text = WavesMinigame.MinigameInstance.waves[i].playerLineSettings.x.ToString();
                buttonsLabels[i].widthLabel.text = WavesMinigame.MinigameInstance.waves[i].playerLineSettings.y.ToString();
                buttonsLabels[i].shiftLabel.text = WavesMinigame.MinigameInstance.waves[i].playerLineSettings.z.ToString();

                bool lineMatch = WavesMinigame.MinigameInstance.CompareLines(i);

                buttonsLabels[i].heightLabel.color = lineMatch ? successTextColor: standardTextColor;
                buttonsLabels[i].widthLabel.color = lineMatch ? successTextColor : standardTextColor;
                buttonsLabels[i].shiftLabel.color = lineMatch ? successTextColor : standardTextColor;
            }
        }

        [Serializable]
        public class WaveLabels
        {
            public TextMeshProUGUI heightLabel;
            public TextMeshProUGUI widthLabel;
            public TextMeshProUGUI shiftLabel;
        }
    }
}
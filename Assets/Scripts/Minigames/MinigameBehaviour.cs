using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minigames
{
    public class MinigameBehaviour : MonoBehaviour
    {
        public bool resultOnTimeout = false;
        [SerializeField] private float standardDuration = 30;

        public float GetMinigameDuration
        {
            get
            {
                return standardDuration;
            }
        }

        public void FinishMinigame(bool result)
        {
            MinigamesController.Instance.FinishMinigame(result);
        }
    }
}
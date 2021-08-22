using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minigames
{
    public class DefenseMinigameAnomaly : MonoBehaviour
    {
        public float initialScale = 1.2f;
        public int initialStrength = 3;
        public float scallingSppeed = 3;

        private int currentStrength;
        private bool destroing = false;

        private void Start()
        {
            currentStrength = initialStrength;
        }

        private void Update()
        {
            float newScale = Mathf.MoveTowards(transform.localScale.x, initialScale / initialStrength * currentStrength, Time.deltaTime * scallingSppeed);
            transform.localScale = new Vector3(newScale, newScale, newScale);

            if(transform.localPosition.x > 0)
            {
                transform.localPosition = new Vector3(transform.localPosition.x - Time.deltaTime * DefenseMinigame.MinigameInstance.anomalySpeed, 0, 0);

                if (!destroing && transform.localPosition.x < DefenseMinigame.MinigameInstance.anomalyAttackDistance)
                {
                    DefenseMinigame.MinigameInstance.AttackCenter();
                    currentStrength = 0;
                    destroing = true;
                    Destroy(transform.parent.gameObject, 1);
                }
            }
            else
            {
                Destroy(transform.parent.gameObject);
            }

        }

        private void OnMouseDown()
        {
            currentStrength--;

            if (currentStrength > 0)
                return;
            
            currentStrength = 0;

            if (destroing)
                return;
            
            destroing = true;
            Destroy(transform.parent.gameObject, 1);

            if (!MinigamesController.Instance.showingResult)
                SoundSystem.Instance.PlayActionSound();
        }
    }

}
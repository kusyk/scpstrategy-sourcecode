using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minigames
{
    public class DefenseMinigame : MinigameBehaviour
    {
        private static DefenseMinigame minigameInstance;

        [Header("Settings")]
        public float anomalySpawnDelay = 3;
        public float anomalySpawnDistance = 10;
        public float anomalySpeed = 1;
        public float anomalyAttackDistance = 2;

        [Space]
        public int standardInitialStrength = 5;
        public int currentStrength;

        [Header("Objects")]
        public Transform centerNode;
        public Transform originalAnomaly;
        public Transform anomaliesHolder;


        private float delayTimer = 0;
        private bool isActive = true;

        
        public static DefenseMinigame MinigameInstance
        {
            get
            {
                if (minigameInstance == null)
                {
                    UnityEngine.Debug.LogError("minigameInstance does not exist");
                }

                return minigameInstance;
            }
        }

        public int GetRealInitialStrenght
        {
            get
            {
                if(SkillsManager.Instance.GetSkillProgress("def2") == 1f)
                    return standardInitialStrength * 2;

                return standardInitialStrength;
            }
        }

        public void AttackCenter()
        {
            if (!isActive)
                return;

            currentStrength--;

            if(currentStrength <= 0)
            {
                currentStrength = 0;
                isActive = false;
                base.FinishMinigame(false);
            }

            SoundSystem.Instance.PlayAttackSound();
        } 

        private void Awake()
        {
            minigameInstance = this;
            PrepareMinigame();
        }

        private void PrepareMinigame()
        {
            currentStrength = GetRealInitialStrenght;
            originalAnomaly.gameObject.SetActive(false);

            for (int i = 0; i < anomaliesHolder.childCount; i++)
            {
                Destroy(anomaliesHolder.GetChild(0).gameObject);
            }
        }

        void Update()
        {
            delayTimer -= Time.deltaTime;

            if (delayTimer <= 0)
            {
                delayTimer = anomalySpawnDelay;
                SpawnNewAnomaly();
            }

            if (MinigamesController.Instance.timer <= 0 && isActive)
                isActive = false;
        }

        private void SpawnNewAnomaly()
        {
            Transform newAnomaly = Instantiate(originalAnomaly.gameObject, anomaliesHolder).transform;
            newAnomaly.eulerAngles = new Vector3(0, Random.Range(0f, 360f), 0);
            newAnomaly.gameObject.SetActive(true);
            newAnomaly.GetChild(0).localPosition = new Vector3(anomalySpawnDistance, 0, 0);
        }
    }
}
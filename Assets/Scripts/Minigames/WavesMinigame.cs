 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minigames
{
    public class WavesMinigame : MinigameBehaviour
    {
        private static WavesMinigame minigameInstance;

        public int linesResolution = 20;

        [Header("Settings")]
        public Wave.Settings rangeMin;
        public Wave.Settings change;

        [Header("Waves")]
        public List<Wave> waves = new List<Wave>();

        [Header("Time Wave")]
        public int timeWaveResolution = 3;
        public Wave timeWave;

        private int doneLines = 0;

        public static WavesMinigame MinigameInstance
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
        
        private void Awake()
        {
            minigameInstance = this;
            RandomizeLines();
        }

        public static void ChangeLineSettings(char[] info)
        {
            if (MinigameInstance == null)
                return;

            int lineIndex = int.Parse(info[0].ToString());
            int changeInt = info[2] == 'P' ? 1 : -1;

            //UnityEngine.Debug.Log("ChangeLineSettings( " + new string(info) + " )\n" + lineIndex + ", " + info[1] + ", " + changeInt);

            if (MinigameInstance.CompareLines(lineIndex))
                return;

            switch (info[1])
            {
                case 'A':
                    minigameInstance.waves[lineIndex].playerLineSettings.x += changeInt;
                    break;
                case 'B':
                    minigameInstance.waves[lineIndex].playerLineSettings.y += changeInt;
                    break;
                case 'C':
                    minigameInstance.waves[lineIndex].playerLineSettings.z += changeInt;
                    break;
            }

            minigameInstance.waves[lineIndex].playerLineSettings.Clamp(Vector3Int.one, Vector3Int.one * 5);

            minigameInstance.CheckAnswers();
            minigameInstance.CheckAndPlaySound();
        }

        private void RandomizeLines()
        {
            for (int i = 0; i < waves.Count; i++)
            {
                waves[i].speed = Random.Range(rangeMin.speed, change.speed);

                Vector2Int heightRandom = GetRandomValues1to5;
                waves[i].bgLineSettings.x = heightRandom.x;
                waves[i].playerLineSettings.x = heightRandom.y;

                Vector2Int widthRandom = GetRandomValues1to5;
                waves[i].bgLineSettings.y = widthRandom.x;
                waves[i].playerLineSettings.y = widthRandom.y;

                Vector2Int shiftRandom = GetRandomValues1to5;
                waves[i].bgLineSettings.z = shiftRandom.x;
                waves[i].playerLineSettings.z = shiftRandom.y;

                if (SkillsManager.Instance.GetSkillProgress("waves2") == 1f)
                    waves[i].playerLineSettings.y = widthRandom.x;
            }
        }

        /// <summary>
        /// Returns two different and random integers in range from 1 to 5 
        /// </summary>
        private Vector2Int GetRandomValues1to5
        {
            get
            {
                int bgValue = Random.Range(1, 6);
                int playerValue;
                do
                {
                    playerValue = Random.Range(1, 6);
                } while (bgValue == playerValue);

                return new Vector2Int(bgValue, playerValue);
            }
        }

        private void Update()
        {
            UpdateAllLines();
        }

        private void UpdateAllLines()
        {
            //game waves
            for (int i = 0; i < waves.Count; i++)
            {
                if (waves[i].bgLine != null)
                    UpdateLine(waves[i].bgLine, linesResolution, waves[i].lineLenght, waves[i].speed, waves[i].bgLineSettings);

                if (waves[i].playerLine != null)
                    UpdateLine(waves[i].playerLine, linesResolution, waves[i].lineLenght, waves[i].speed, waves[i].playerLineSettings);
            }

            //time waves
            UpdateLine(timeWave.bgLine, timeWaveResolution, timeWave.lineLenght, MinigamesController.GetTimerPositive * timeWave.speed, timeWave.bgLineSettings);
            UpdateLine(timeWave.playerLine, timeWaveResolution, timeWave.lineLenght, MinigamesController.GetTimerPositive * timeWave.speed, timeWave.playerLineSettings);
        }

        private void UpdateLine(LineRenderer line, int lineResolution , float lineLenght, float speed, Vector3Int settings)
        {
            line.positionCount = (int)(lineLenght * lineResolution);

            for (int i = 0; i < line.positionCount; i++)
            {
                float x = (float)i / (float)lineResolution - lineLenght * 0.5f;
                float height = rangeMin.height + settings.x * change.height;
                float width = rangeMin.width + (6 - settings.y) * change.width;
                float horizontalShift = rangeMin.horizontalShift + (6 - settings.z) * change.horizontalShift;

                line.SetPosition(i, new Vector3(x, 0, Mathf.Sin(x * width + Time.timeSinceLevelLoad * speed + horizontalShift) * height));
            }
        }

        public bool CompareLines(int lineIndex)
        {
            if (minigameInstance.waves[lineIndex].bgLineSettings == minigameInstance.waves[lineIndex].playerLineSettings)
                return true;
            else
                return false;
        }

        private void CheckAnswers()
        {
            for (int i = 0; i < waves.Count; i++)
            {
                if (!CompareLines(i))
                {
                    return;
                }
            }

            base.FinishMinigame(true);
        }

        private void CheckAndPlaySound()
        {
            int newDoneLines = 0;

            for (int i = 0; i < waves.Count; i++)
            {
                if (CompareLines(i))
                {
                    newDoneLines++;
                }
            }

            if(newDoneLines > doneLines)
            {
                doneLines = newDoneLines;
                SoundSystem.Instance.PlayActionSound();
            }
        }


        [System.Serializable]
        public class Wave
        {
            public LineRenderer bgLine;
            public LineRenderer playerLine;

            [Space]
            public float lineLenght = 10f;
            [Space]
            public float speed = 10f;
            [Space]
            public Vector3Int bgLineSettings;
            [Space]
            public Vector3Int playerLineSettings;



            [System.Serializable]
            public class Settings
            {
                public float speed = 1f;
                public float height = 1f;
                public float width = 1f;
                public float horizontalShift = 1f;
            }
        }
    }
}
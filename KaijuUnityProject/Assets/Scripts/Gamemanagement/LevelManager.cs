using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Management
{
    public class LevelManager : MonoBehaviour
    {
        /// <summary>
        /// The static instance of the GameManager in the scene.
        /// </summary>
        public static LevelManager Instance { get; private set; } = null;

        public UnityEvent UpdateLevelEvent = new UnityEvent();

        #region Variables
        private ActiveLevel activeLevel;
        #endregion

        #region Basic Functions
        private void Awake()
        {
            //Check if instance is assigned
            if (Instance == null) Instance = this;
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
        }
        #endregion

        #region Public Functions
        public void LoadLevel(int levelId)
        {
            activeLevel = new ActiveLevel(LevelList.GetLevelById(levelId));
        }
        public void LoadLevel(string levelTitle)
        {
            activeLevel = new ActiveLevel(LevelList.GetLevelByTitle(levelTitle));
        }
        #endregion

        #region Getters
        public int GetActiveLevelID()
        {
            return activeLevel.id;
        }
        public int GetNextLevelID()
        {
            return activeLevel.nextLevelId;
        }
        public int GetActiveLevelDifficulty()
        {
            return activeLevel.difficulty;
        }
        public string GetActiveLevelTitle()
        {
            return activeLevel.title;
        }
        public int GetActiveLevelWidth()
        {
            return activeLevel.width;
        }
        public int GetActiveLevelHeight()
        {
            return activeLevel.height;
        }
        public int GetActiveLevelTime()
        {
            return activeLevel.time;
        }
        public ActiveLevel GetActiveLevel()
        {
            return activeLevel;
        }
        #endregion
    }

    public class ActiveLevel
    {
        public int id;
        public int nextLevelId;
        public int difficulty;
        public string title;
        public int time;
        public int width;
        public int height;
        public List<Task> tasks;

        public ActiveLevel()
        {
            id = -1;
            nextLevelId = -1;
            difficulty = -1;
            title = "Undefined Level";
            time = 10;
            width = 1;
            height = 1;
            tasks = new List<Task>();
        }

        public ActiveLevel(Level level)
        {
            id = level.Id;
            title = level.Title;
            nextLevelId = level.NextLevelId;
            difficulty = level.Difficulty;
            time = level.Time;
            width = level.Width;
            height = level.Height;
            tasks = new List<Task>(level.GetTasks());
        }
    }
}

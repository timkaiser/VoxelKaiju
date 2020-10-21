using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Management
{
    [Serializable]
    public class Levels
    {
        [SerializeField]
        private Level[] levels = new Level[0];

        /// <summary>
        /// get the list of  all Levels on the JSON file
        /// </summary>
        public ReadOnlyCollection<Level> GetLevels
        {
            get
            {
                return Array.AsReadOnly<Level>(levels);
            }
        }
    }

    [Serializable]
    public class Level
    {
        [SerializeField]
        private int id = -1;
        [SerializeField]
        private int nextLevelId = 1;
        [SerializeField]
        private string title = "Undefined Level";
        [SerializeField]
        private int difficulty = -1;
        [SerializeField]
        private int time = 0;
        [SerializeField]
        private int width = 5;
        [SerializeField]
        private int height = 5;
        [SerializeField]
        private Task[] tasks = new Task[0];

        public int Id { get { return id; } }
        public int NextLevelId { get { return nextLevelId; } }
        public string Title { get { return title; } }
        public int Difficulty { get { return difficulty; } }
        public int Time { get { return time; } }
        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public Task[] Tasks { get { return tasks; } }
        public ReadOnlyCollection<Task> GetTasks() { return Array.AsReadOnly<Task>(tasks); }
    }
}

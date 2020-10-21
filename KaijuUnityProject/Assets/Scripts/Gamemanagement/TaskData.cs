using System;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Management
{
    public enum TaskEvents
    {
        None = 0,
        DestroyedObject = 1,
        GrownToHeight = 2,
        EnteredArea = 3
    }

    [Serializable]
    public class Task
    {
        [SerializeField]
        private int id = -1;
        [SerializeField]
        private int nextTaskId = 1;
        [SerializeField]
        private string title = "Undefined Task";
        [SerializeField]
        string description = "";
        [SerializeField]
        int[] requiresEnemies = new int[0];
        [SerializeField]
        int[] requiresDestructibles = new int[0];
        [SerializeField]
        int[] requiresInput = new int[0];
        [SerializeField]
        private int requiresHighscorePoints = 0;

        #region Getters
        public int TaskId { get { return id; } }
        public string TaskTitle { get { return title; } }
        public int NextTaskId { get { return nextTaskId; } }
        public int[] RequiresEnemies { get { return requiresEnemies; } }
        public int[] RequiresDestructibles { get { return requiresDestructibles; } }
        public int[] RequiresInput { get { return requiresInput;  } }
        public string Description { get { return description; } }
        public int RequiresHighscorePoints { get { return requiresHighscorePoints; } }
        #endregion

        #region JsonToData
        public EnemyData.EnemyType[] GetRequiredEnemyTypes()
        {
            return Array.ConvertAll(requiresEnemies, value => (EnemyData.EnemyType) value);
        }
        public DestructibleData.DestructibleType[] GetRequiredDestructibleTypes()
        {
            return Array.ConvertAll(requiresDestructibles, value => (DestructibleData.DestructibleType) value);
        }
        public ActionData.ActionType[] GetRequiredInputTypes()
        {
            return Array.ConvertAll(requiresInput, value => (ActionData.ActionType) value);
        }
        #endregion
    }
}

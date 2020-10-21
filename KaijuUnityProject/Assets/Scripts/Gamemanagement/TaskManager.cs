using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Management
{
    public class TaskManager : MonoBehaviour
    {
        /// <summary>
        /// The static instance of the GameManager in the scene.
        /// </summary>
        public static TaskManager Instance { get; private set; } = null;

        public UnityEvent NewTaskEvent = new UnityEvent();
        public UnityEvent DestroyedObjectEvent = new UnityEvent();
        public UnityEvent PerformedActionEvent = new UnityEvent();

        #region Variables
        private ActiveTask activeTask = new ActiveTask();
        private List<Task> tasks = new List<Task>();
        private int taskCount = 0;
        private bool[] completedTasks = new bool[0];
        private List<DestructibleData.DestructibleType> totalDestroyedObjects = new List<DestructibleData.DestructibleType>();
        private List<EnemyData.EnemyType> totalDestroyedEnemies = new List<EnemyData.EnemyType>();

        /// <summary>
        /// The reference to the text object of the in-game task description.
        /// </summary>
        private Text uiTaskDescription;
        /// <summary>
        /// The reference to the text object of the in-game task target count.
        /// </summary>
        private Text uiTargetCount;
        /// <summary>
        /// The reference to the check icon of the in-game task.
        /// </summary>
        private GameObject uiCheckIcon;
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

        private void Start()
        {
            Transform uiTaskTransform = UIManager.Instance.GetHudPanel.transform.Find("TaskDescriptionBackground");
            uiTaskDescription = uiTaskTransform.Find("TaskDescription").GetComponent<Text>();
            uiTargetCount = uiTaskTransform.Find("TargetCount").GetComponent<Text>();
            uiCheckIcon = uiTaskTransform.Find("CheckIcon").gameObject;
            uiCheckIcon.SetActive(false);
            EmptyUi();
        }

        public void Init(ActiveLevel level)
        {
            tasks = level.tasks;
            taskCount = tasks.Count;
            if (taskCount > 0) activeTask = new ActiveTask(tasks[0]);
            else activeTask = new ActiveTask();
            completedTasks = new bool[taskCount];
            UiUpdateTaskDescription();
            UiUpdateTargetCount();
        }
        #endregion

        #region Getters
        public int TaskCount { get { return taskCount; } }
        public int GetActiveTaskId()
        {
            return activeTask.taskId;
        }
        public int GetNextTaskId()
        {
            return activeTask.nextTaskId;
        }
        public string GetActiveTaskTitle()
        {
            return activeTask.taskTitle;
        }
        public string GetActiveTaskDescription()
        {
            return activeTask.description;
        }
        #endregion

        #region Private Functions
        private IEnumerator ManageActiveTask()
        {
            Debug.Log(activeTask.description + " COMPLETED");           
                           
            if (taskCount > activeTask.taskId + 1 && activeTask.nextTaskId != -1)
            {
                uiCheckIcon.SetActive(true);
                SoundManager.Instance.PlayEFX("Task_3");

                yield return new WaitForSeconds(2f);

                uiCheckIcon.SetActive(false);
                activeTask = new ActiveTask(tasks[activeTask.nextTaskId]);                
                UiUpdateTaskDescription();
                UiUpdateTargetCount();

                NewTaskEvent?.Invoke();
            }
            else
            {
                EmptyUi();
                GameManager.Instance.FinishGame(true);
            }
        }
        #endregion

        #region UiFunctions

        /// <summary>
        /// Updates the text of the HUD with the current task description.
        /// </summary>
        private void UiUpdateTaskDescription()
        {
            uiTaskDescription.text = activeTask.description;
        }

        /// <summary>
        /// Updates the text of the HUD with the current amount of cleared targets.
        /// </summary>
        private void UiUpdateTargetCount()
        {
            uiTargetCount.text = "[" + (activeTask.totalTargetCount - activeTask.GetCurrentTargetCount()) + " / " + activeTask.totalTargetCount + "]";
        }

        private void EmptyUi()
        {
            uiTaskDescription.text = "";
            uiTargetCount.text = "";
        }
        #endregion

        #region Register Functions
        public void RegisterDestroyedObject(DestructibleData.DestructibleType type)
        {
            totalDestroyedObjects.Add(type);
            DestroyedObjectEvent?.Invoke();
            if (activeTask.requiresDestructibles.Remove(type))
            {
                UpdateTask();
            }
        }

        public void RegisterDestroyedEnemy(EnemyData.EnemyType type)
        {
            totalDestroyedEnemies.Add(type);
            DestroyedObjectEvent?.Invoke();
            if (activeTask.requiresEnemies.Remove(type))
            {
                UpdateTask();
            }
        }

        public void RegisterPerformedAction(ActionData.ActionType type)
        {
            PerformedActionEvent?.Invoke();
            if (activeTask.requiresAction.Remove(type))
            {
                UpdateTask();
            }
        }

        public void RegisterAquiredPoints(int points)
        {
            if (activeTask.requiresHighscorePoints > 0)
            {
                activeTask.requiresHighscorePoints = Mathf.Clamp(activeTask.requiresHighscorePoints - points, 0, int.MaxValue);
                UpdateTask();
            }
        }

        private void UpdateTask()
        {          
            UiUpdateTargetCount();
            if (activeTask.IsFinished())
            {
                completedTasks[activeTask.taskId] = true;
                StartCoroutine(ManageActiveTask());
            }
            else
            {
                int targetsCleared = activeTask.totalTargetCount - activeTask.GetCurrentTargetCount();
                if (targetsCleared == 1) SoundManager.Instance.PlayEFX("Task_1");
                else if (targetsCleared == 2) SoundManager.Instance.PlayEFX("Task_2");
            }
        }
        #endregion
    }

    public class ActiveTask
    {
        public int taskId;
        public int nextTaskId;
        public string taskTitle;
        public string description;
        public int totalTargetCount;
        public List<EnemyData.EnemyType> requiresEnemies;
        public List<DestructibleData.DestructibleType> requiresDestructibles;
        public List<ActionData.ActionType> requiresAction;
        public int requiresHighscorePoints;

        public ActiveTask()
        {
            taskId = -1;
            nextTaskId = -1;
            taskTitle = "Undefined Task";
            description = "Undefined Task";
            totalTargetCount = -1;
            requiresEnemies = new List<EnemyData.EnemyType>();
            requiresDestructibles = new List<DestructibleData.DestructibleType>();
            requiresAction = new List<ActionData.ActionType>();
            requiresHighscorePoints = 0;
        }
        public ActiveTask(Task t)
        {
            taskId = t.TaskId;
            nextTaskId = t.NextTaskId;
            taskTitle = t.TaskTitle;
            description = t.Description;
            requiresEnemies = new List<EnemyData.EnemyType>(t.GetRequiredEnemyTypes());
            requiresDestructibles = new List<DestructibleData.DestructibleType>(t.GetRequiredDestructibleTypes());
            requiresAction = new List<ActionData.ActionType>(t.GetRequiredInputTypes());
            requiresHighscorePoints = t.RequiresHighscorePoints;
            totalTargetCount = GetCurrentTargetCount();
            requiresHighscorePoints = Mathf.Clamp(requiresHighscorePoints - GameManager.Instance.GetComponent<Highscore>().GetScore(), 0, int.MaxValue);
        }

        public bool IsFinished()
        {
            return requiresDestructibles.Count + requiresEnemies.Count + requiresAction.Count + requiresHighscorePoints == 0;
        }
        
        public int GetCurrentTargetCount()
        {
            return requiresDestructibles.Count + requiresEnemies.Count + requiresAction.Count + requiresHighscorePoints;
        }
    }
}

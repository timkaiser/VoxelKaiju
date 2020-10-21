using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Management
{
    public static class LevelList
    {
        public static int size = 0;
        public static Levels levels;

        public static void Init()
        {
            string jsonFile = Resources.Load<TextAsset>("JSONData/LevelList").text;
            levels = JsonUtility.FromJson<Levels>(jsonFile);
            size = levels.GetLevels.Count;
            //Console Log
            Debug.Log("[Management] Loaded Levels: " + size);
        }

        #region Getters
        public static Level GetLevelById(int levelId)
        {
            for (int i = 0; i < size; i++)
            {
                if (levels.GetLevels[i].Id == levelId)
                {
                    return levels.GetLevels[i];
                }
            }
            return new Level();
        }
        public static Level GetLevelByTitle(string levelTitle)
        {
            for (int i = 0; i < size; i++)
            {
                if (levels.GetLevels[i].Title == levelTitle)
                {
                    return levels.GetLevels[i];
                }
            }
            return new Level();
        }
        #endregion
    }
}

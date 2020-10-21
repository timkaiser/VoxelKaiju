using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DebugCommands
{
    /// <summary>
    /// class that handels any debug commands for the gameplay side of the game
    /// </summary>
    public class GameplayDebugCommands : MonoBehaviour
    {
        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        void Start()
        {
            Tools.Console.RegisterCommand(Tools.Console.Category.Gameplay, "Test", CommandTest);
        }

        private void CommandTest(object[] param)
        {
            Debug.Log("Test");
        }

        private void OnApplicationQuit()
        {
            //reset global settings if nessesary
        }
    }
}

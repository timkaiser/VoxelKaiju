using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    /// <summary>
    /// Manager class for the debug menu
    /// </summary>
    public class DebugMenuManager : MonoBehaviour
    {
        /// <summary>
        /// Marks whether the debug menu is active or not
        /// </summary>
        private bool isActive;

        /// <summary>
        /// Console input field
        /// </summary>
        public InputField Console;

        /// <summary>
        /// Initialisation
        /// </summary>
        public static void Init()
        {
            if(Config.GetBool(Config.Type.Tools, "Debug.Menu"))
            {
                GameObject go = Instantiate(Resources.Load<GameObject>("Tools.Debug.UI"));
                go.transform.name = "Tools.Debug.UI";
                DontDestroyOnLoad(go);
                go.GetComponent<Canvas>().enabled = false;
            }
        }

        //Handle input
        private void Update()
        {
            //Toggle the menu
            if (Input.GetKeyDown(KeyCode.F1))
            {
                //Swap state
                isActive = !isActive;

                if (isActive)
                {
                    Console.ActivateInputField();
                    //Console.Select();
                }
                else
                {
                    Console.text = string.Empty;
                    Console.DeactivateInputField();
                }

                //Activate the canvas
                GetComponent<Canvas>().enabled = isActive;
            }

            //Don't do stuff if not active
            if (!isActive)
                return;
        }
    }
}
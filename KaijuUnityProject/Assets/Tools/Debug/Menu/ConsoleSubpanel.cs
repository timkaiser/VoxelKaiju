using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tools
{
    /// <summary>
    /// Subpanel 
    /// </summary>
    public class ConsoleSubpanel : Subpanel
    {
        [Header("Command Line")]
        /// <summary>
        /// The command input field
        /// </summary>
        public InputField Field;
        /// <summary>
        /// The auto completion text
        /// </summary>
        public Text Autocomplete;

        //Check for tab input
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!Autocomplete.text.Equals(string.Empty))
                {
                    Field.text = Autocomplete.text;
                    Field.caretPosition = Field.text.Length;
                }
            }
        }

        /// <summary>
        /// Check for new autocomplete results
        /// </summary>
        public void CheckForAutocomplete()
        {
            if (!Field.text.Equals(string.Empty))
            {
                Autocomplete.text = Console.CheckForAutocomplete(Field.text);
            }
            else
            {
                Autocomplete.text = string.Empty;
            }
        }

        /// <summary>
        /// Submit the command
        /// </summary>
        public void Submit()
        {
            if (!Field.text.Equals(string.Empty))
            {
                bool result = Console.Execute(Field.text);
                if (result)
                {
                    //Clear input field
                    Field.text = string.Empty;
                    Autocomplete.text = string.Empty;
                    Field.ActivateInputField();
                }
            }
        }
    }
}
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace Tools
{
    /// <summary>
    /// Debug menu log subpanel manager
    /// </summary>
    public static class Console
    {
        /// <summary>
        /// Delegate function for the command
        /// </summary>
        /// <param name="parameters">Parameters for the delegate function</param>
        public delegate void Command(params object[] parameters);

        //Stores all commands
        private static List<Tuple<string, Command>> commands;
        //Stores all commands in lower case. Used for auto completion tests
        private static List<Tuple<string, Command>> commandsToLower;
        /// <summary>
        /// Used to prevent category duplicates due to typos
        /// </summary>
        public enum Category
        {
            Debug,
            Gameplay
        }

        /// <summary>
        /// Registers a new command
        /// </summary>
        /// <param name="category">The command category</param>
        /// <param name="command">The command string</param>
        /// <param name="function">The function to call on command</param>
        public static void RegisterCommand(Category category, string command, Command function)
        {
            if (commands == null)
                commands = new List<Tuple<string, Command>>();
            if (commandsToLower == null)
                commandsToLower = new List<Tuple<string, Command>>();
            bool exists = false;
            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].Item1.Equals(category.ToString() + "." + command))
                {
                    Debug.LogWarning("Overwriting command duplicate: \n" + category.ToString() + "." + command);
                    commands[i] = new Tuple<string, Command>(category.ToString() + "." + command, function);
                    commandsToLower[i] = new Tuple<string, Command>((category.ToString() + "." + command).ToLower(), function);
                    exists = true;
                    break;
                }
            }
            if (!exists)
            {
                commands.Add(new Tuple<string, Command>(category.ToString() + "." + command, function));
                commandsToLower.Add(new Tuple<string, Command>((category.ToString() + "." + command).ToLower(), function));
            }
        }

        /// <summary>
        /// De-registers an existing command
        /// </summary>
        /// <param name="category">The command category</param>
        /// <param name="command">The command string</param>
        public static void DeregisterCommand(Category category, string command)
        {
            //Make sure the command exists
            for (int i = 0; i < commands.Count; i++)
            {
                if (commands[i].Item1.Equals(category.ToString() + "." + command))
                {
                    commands.RemoveAt(i);
                    commandsToLower.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Checks if a command exists that starts with s
        /// </summary>
        /// <param name="s">The string to check</param>
        /// <returns>The full command for the given start</returns>
        public static string CheckForAutocomplete(string s)
        {
            if (commands == null)
                return "";

            int numCommands = commands.Count;
            List<string> result = new List<string>();
            s = s.ToLower();
            for (int i = 0; i < numCommands; i++)
            {
                if (commandsToLower[i].Item1.StartsWith(s, StringComparison.Ordinal))
                {
                    result.Add(commands[i].Item1);
                }
            }
            if (result.Count == 0)
            {
                return string.Empty;
            }
            else if (result.Count == 1)
            {
                return result[0] + " ";
            }
            else
            {
                string combined = result[0];
                for (int i = 1; i < result.Count; i++)
                {
                    //Buffer the current combination
                    string buffer = combined;
                    //Reset the current combination
                    combined = string.Empty;
                    //Get the length of the shorter word
                    int compareUntil = 0;
                    if (result[i].Length < buffer.Length)
                    {
                        compareUntil = result[i].Length;
                    }
                    else
                    {
                        compareUntil = buffer.Length;
                    }

                    //Iterate through it
                    for (int j = 0; j < compareUntil; j++)
                    {
                        if (buffer[j] == result[i][j])
                        {
                            combined += buffer[j];
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                return combined;
            }
        }

        /// <summary>
        /// Tries to execute a command
        /// </summary>
        /// <param name="s">The command to execute</param>
        /// <returns>Success or not</returns>
        public static bool Execute(string s)
        {
            int numCommands = commands.Count;
            string[] commandSplit = s.Split(' ');
            for (int i = 0; i < numCommands; i++)
            {
                if (commands[i].Item1.Equals(commandSplit[0]))
                {
                    object[] parameters = new object[commandSplit.Length - 1];
                    if (commandSplit.Length == 1)
                    {
                        commands[i].Item2.Invoke();
                    }
                    else if(commandSplit.Length == 2)
                    {
                        commands[i].Item2.Invoke(commandSplit[1]);
                    }
                    else
                    {
                        for (int j = 1; j < commandSplit.Length; j++)
                        {
                            UnityEngine.Debug.Log(commandSplit[j]);
                            parameters[j - 1] = commandSplit[j];
                        }
                        commands[i].Item2.Invoke(parameters);
                    }
                    
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Generic parse function which assures that there is always a type being returned
        /// </summary>
        /// <typeparam name="T">Type to convert to</typeparam>
        /// <param name="input">The input object to parse</param>
        /// <param name="result">The parsed output</param>
        /// <returns>True if parsed successfully</returns>
        private static bool Parse<T>(object input, out T result)
        {
            //Check if type is already correct
            if (input is T)
            {
                result = (T)input;
                return true;
            }

            //Try converting or return the default value
            try
            {
                result = (T)Convert.ChangeType(input, typeof(T));
                return true;
            }
            catch (Exception)
            {
                result = default(T);
                return false;
            }
        }

        /// <summary>
        /// Generic parse function which assures that there is always a type being returned
        /// </summary>
        /// <typeparam name="T0">Type of parameter 0</typeparam>
        /// <param name="input">Input objects</param>
        /// <param name="result0">Parsed parameter 0</param>
        /// <returns>True of successfully parsed all objects</returns>
        public static bool Parse<T0>(object[] input, out T0 result0)
        {
            result0 = default(T0);

            if (!Parse<T0>(input[0], out result0))
                return false;

            return true;
        }

        /// <summary>
        /// Generic parse function which assures that there is always a type being returned
        /// </summary>
        /// <typeparam name="T0">Type of parameter 0</typeparam>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <param name="input">Input objects</param>
        /// <param name="result0">Parsed parameter 0</param>
        /// <param name="result1">Parsed parameter 1</param>
        /// <returns>True of successfully parsed all objects</returns>
        public static bool Parse<T0, T1>(object[] input, out T0 result0, out T1 result1)
        {
            result0 = default(T0);
            result1 = default(T1);

            if (!Parse<T0>(input[0], out result0))
                return false;
            if (!Parse<T1>(input[1], out result1))
                return false;

            return true;
        }

        /// <summary>
        /// Generic parse function which assures that there is always a type being returned
        /// </summary>
        /// <typeparam name="T0">Type of parameter 0</typeparam>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <typeparam name="T2">Type of parameter 2</typeparam>
        /// <param name="input">Input objects</param>
        /// <param name="result0">Parsed parameter 0</param>
        /// <param name="result1">Parsed parameter 1</param>
        /// <param name="result2">Parsed parameter 2</param>
        /// <returns>True of successfully parsed all objects</returns>
        public static bool Parse<T0, T1, T2>(object[] input, out T0 result0, out T1 result1, out T2 result2)
        {
            result0 = default(T0);
            result1 = default(T1);
            result2 = default(T2);

            if (!Parse<T0>(input[0], out result0))
                return false;
            if (!Parse<T1>(input[1], out result1))
                return false;
            if (!Parse<T2>(input[2], out result2))
                return false;

            return true;
        }

        /// <summary>
        /// Generic parse function which assures that there is always a type being returned
        /// </summary>
        /// <typeparam name="T0">Type of parameter 0</typeparam>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <typeparam name="T2">Type of parameter 2</typeparam>
        /// <typeparam name="T3">Type of parameter 3</typeparam>
        /// <param name="input">Input objects</param>
        /// <param name="result0">Parsed parameter 0</param>
        /// <param name="result1">Parsed parameter 1</param>
        /// <param name="result2">Parsed parameter 2</param>
        /// <param name="result3">Parsed parameter 3</param>
        /// <returns>True of successfully parsed all objects</returns>
        public static bool Parse<T0, T1, T2, T3>(object[] input, out T0 result0, out T1 result1, out T2 result2, out T3 result3)
        {
            result0 = default(T0);
            result1 = default(T1);
            result2 = default(T2);
            result3 = default(T3);

            if (!Parse<T0>(input[0], out result0))
                return false;
            if (!Parse<T1>(input[1], out result1))
                return false;
            if (!Parse<T2>(input[2], out result2))
                return false;
            if (!Parse<T3>(input[3], out result3))
                return false;

            return true;
        }
        /// <summary>
        /// Generic parse function which assures that there is always a type being returned
        /// </summary>
        /// <typeparam name="T0">Type of parameter 0</typeparam>
        /// <typeparam name="T1">Type of parameter 1</typeparam>
        /// <typeparam name="T2">Type of parameter 2</typeparam>
        /// <typeparam name="T3">Type of parameter 3</typeparam>
        /// <typeparam name="T4">Type of parameter 4</typeparam>
        /// <param name="input">Input objects</param>
        /// <param name="result0">Parsed parameter 0</param>
        /// <param name="result1">Parsed parameter 1</param>
        /// <param name="result2">Parsed parameter 2</param>
        /// <param name="result3">Parsed parameter 3</param>
        /// <param name="result4">Parsed parameter 4</param>
        /// <returns>True of successfully parsed all objects</returns>
        public static bool Parse<T0, T1, T2, T3, T4>(object[] input, out T0 result0, out T1 result1, out T2 result2, out T3 result3, out T4 result4)
        {
            result0 = default(T0);
            result1 = default(T1);
            result2 = default(T2);
            result3 = default(T3);
            result4 = default(T4);

            if (!Parse<T0>(input[0], out result0))
                return false;
            if (!Parse<T1>(input[1], out result1))
                return false;
            if (!Parse<T2>(input[2], out result2))
                return false;
            if (!Parse<T3>(input[3], out result3))
                return false;
            if (!Parse<T4>(input[4], out result4))
                return false;

            return true;
        }
    }
}
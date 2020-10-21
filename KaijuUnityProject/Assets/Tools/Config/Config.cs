using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization;
using LitJson;
using UnityEngine;

/// <summary>
/// Config file parser class (runtime)
/// </summary>
public static class Config
{
    /// <summary>
    /// Enum containing all the possible config files
    /// </summary>
    public enum Type
    {
        Debug,
        Gameplay,
        Physics,
        Enemies,
        Rendering,
        Scenemanager,
        Tools,
        UI,
        Generation
    }
    
    //Loaded json data
    private static JsonData[] json;

    /// <summary>
    /// Load the files
    /// </summary>
    public static void Init()
    {
        //Get the enum value names to read the corresponding file
        string[] typeNames = Enum.GetNames(typeof(Type));
        //Setup the json data array
        json = new JsonData[typeNames.Length];

        //Load all json files
        for(int i = 0; i < typeNames.Length; i++)
        {
            string filePath = Application.dataPath + "/StreamingAssets/Config/" + typeNames[i] + ".config";
            //Load file content as string
            string fileContent = string.Empty;
            string[] lines = File.ReadAllLines(filePath);

            //Filter comments
            foreach (string line in lines)
            {
                if (!line.Contains("//"))
                {
                    fileContent += line;
                }
            }
            
            //Parse file content to json
            //Update culture during parsing to prevent localization issues
            CultureInfo prevCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            json[i] = JsonMapper.ToObject(fileContent);
            CultureInfo.CurrentCulture = prevCulture;
        }
    }
    
    /// <summary>
    /// Save the config file
    /// </summary>
    /// <param name="type">Type of the config file to save</param>
    private static void Save(Type type)
    {
        //Parse json to file content
        //Update culture during parsing to prevent localization issues
        CultureInfo prevCulture = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        string fileContent = JsonMapper.ToJson(json[(int)type]);
        CultureInfo.CurrentCulture = prevCulture;
        
        string filePath = Application.dataPath + "/StreamingAssets/Config/" + Enum.GetName(typeof(Type), (int)type) + ".config";

        File.WriteAllText(filePath, fileContent);
    }

    /// <summary>
    /// Check whether the config system is initialised or not
    /// </summary>
    /// <returns>True if the system is fully initialised</returns>
    public static bool IsInitalized()
    {
        if (json == null)
            return false;
        else
            return true;
    }

    /// <summary>
    /// Gets a list of all keys
    /// </summary>
    /// <param name="type">The config file type to get the keys for</param>
    /// <returns>List of config file keys as string</returns>
    public static List<string> GetAllKeys(Type type)
    {
        return json[(int)type].Keys.ToList<string>();
    }

    /// <summary>
    /// Check if a key exists in the config
    /// </summary>
    /// <param name="type">The config file type to get the keys for</param>
    /// <param name="key">Key to check</param>
    /// <returns>True if key exists in config</returns>
    public static bool HasKey(Type type, string key)
    {
        if (json == null)
            Init();
        
        if (json[(int)type].ContainsKey(key))
            return true;
        else
            return false;
    }

    /// <summary>
    /// Deletes a key and its value from the config file
    /// </summary>
    /// <param name="type">The config file type to delete the key for</param>
    /// <param name="key">Key to delete</param>
    public static void DeleteKey(Type type, string key)
    {
        //Make sure the key exists
        if(HasKey(type, key))
        {
            json[(int)type].Remove(key);
            Save(type);
        }
    }

    /// <summary>
    /// Changes the value of a key
    /// </summary>
    /// <param name="type">The config type the key is a part of</param>
    /// <param name="oldKey">The old name of the key</param>
    /// <param name="newKey">The new name of the key</param>
    public static void ChangeKey(Type type, string oldKey, string newKey)
    {
        //Make sure the old key exists and the new one doesn't
        if (!HasKey(type, oldKey) || HasKey(type, newKey))
        {
            return;
        }

        //Search for the key and replace it
        for(int i = 0; i < json[(int)type].Keys.Count; i++)
        {
            //Get key and value of the current pair
            string key = json[(int)type].Keys.ElementAt(i);
            JsonData value = json[(int)type][key];
            //Remove the pair
            if (HasKey(type, key))
            {
                json[(int)type].Remove(key);
            }
            //Replace the key if it's the desired one
            if (key.Equals(oldKey))
                key = newKey;
            //Move the pair to the end of the list
            json[(int)type][key] = value;
        }
        Save(type);
    }

    /// <summary>
    /// Adds a new key/int value pair
    /// </summary>
    /// <param name="type">The config file type to add the value to</param>
    /// <param name="key">The key of the new value</param>
    /// <param name="value">The value for the new key</param>
    public static void AddInt(Type type, string key, int value)
    {
        //Make sure the key is unique
        string keyAdapted = key;
        int counter = 0;
        while(HasKey(type, keyAdapted))
        {
            counter++;
            keyAdapted = key + "(" + counter + ")";
        }
        
        //Add the pair and save
        json[(int)type][keyAdapted] = value;
        Save(type);
    }

    /// <summary>
    /// Adds a new key/float value pair
    /// </summary>
    /// <param name="type">The config file type to add the value to</param>
    /// <param name="key">The key of the new value</param>
    /// <param name="value">The value for the new key</param>
    public static void AddFloat(Type type, string key, float value)
    {
        //Make sure the key is unique
        string keyAdapted = key;
        int counter = 0;
        while (HasKey(type, keyAdapted))
        {
            counter++;
            keyAdapted = key + "(" + counter + ")";
        }

        //Add the pair and save
        json[(int)type][keyAdapted] = value;
        Save(type);
    }

    /// <summary>
    /// Adds a new key/Vector3 value pair
    /// </summary>
    /// <param name="type">The config file type to add the value to</param>
    /// <param name="key">The key of the new value</param>
    /// <param name="value">The value for the new key</param>
    public static void AddVector3(Type type, string key, Vector3 value)
    {
        //Make sure the key is unique
        string keyAdapted = key;
        int counter = 0;
        while (HasKey(type, keyAdapted))
        {
            counter++;
            keyAdapted = key + "(" + counter + ")";
        }

        //Add the pair and save
        json[(int)type][keyAdapted] = "vec3_" + value.x + "_" + value.y + "_" + value.z;
        Save(type);
    }


    /// <summary>
    /// Adds a new key/bool value pair
    /// </summary>
    /// <param name="type">The config file type to add the value to</param>
    /// <param name="key">The key of the new value</param>
    /// <param name="value">The value for the new key</param>
    public static void AddBool(Type type, string key, bool value)
    {
        //Make sure the key is unique
        string keyAdapted = key;
        int counter = 0;
        while (HasKey(type, keyAdapted))
        {
            counter++;
            keyAdapted = key + "(" + counter + ")";
        }

        //Add the pair and save
        json[(int)type][keyAdapted] = value;
        Save(type);
    }

    /// <summary>
    /// Adds a new key/string value pair
    /// </summary>
    /// <param name="type">The config file type to add the value to</param>
    /// <param name="key">The key of the new value</param>
    /// <param name="value">The value for the new key</param>
    public static void AddString(Type type, string key, string value)
    {
        //Make sure the key is unique
        string keyAdapted = key;
        int counter = 0;
        while (HasKey(type, keyAdapted))
        {
            counter++;
            keyAdapted = key + "(" + counter + ")";
        }

        //Add the pair and save
        json[(int)type][keyAdapted] = value;
        Save(type);
    }

    /// <summary>
    /// Get a json data object from the config
    /// </summary>
    /// <param name="type">The config file type to get the keys for</param>
    /// <param name="key">The key to get the value for</param>
    /// <returns>The value for the key</returns>
    public static JsonData GetJsonData(Type type, string key)
    {
        if (!HasKey(type, key))
        {
            Debug.LogWarning("Config data for key: " + key + " not available!");
        }

        return json[(int)type][key];
    }

    /// <summary>
    /// Get an int from the config
    /// </summary>
    /// <param name="type">The config file type to get the keys for</param>
    /// <param name="key">The key to get the value for</param>
    /// <returns>The value for the key</returns>
    public static int GetInt(Type type, string key)
    {
        if (!HasKey(type, key))
        {
            Debug.LogWarning("Config data for key: " + key + " not available!");
        }

        try
        {
            return int.Parse(json[(int)type][key].ToString());
        }
        catch(Exception e)
        {
            Debug.LogError("Unable to parse config data to int: " + e);
            return 0;
        }
    }

    /// <summary>
    /// Get a float from the config
    /// </summary>
    /// <param name="type">The config file type to get the keys for</param>
    /// <param name="key">The key to get the value for</param>
    /// <returns>The value for the key</returns>
    public static float GetFloat(Type type, string key)
    {
        if (!HasKey(type, key))
        {
            Debug.LogWarning("Config data for key: " + key + " not available!");
        }

        try
        {
            return float.Parse(json[(int)type][key].ToString());
        }
        catch (Exception e)
        {
            Debug.LogError("Unable to parse config data to float: " + e);
            return 0.0f;
        }
    }

    /// <summary>
    /// Get a Vector3 from the config
    /// </summary>
    /// <param name="type">The config file type to get the keys for</param>
    /// <param name="key">The key to get the value for</param>
    /// <returns>The value for the key</returns>
    public static Vector3 GetVector3(Type type, string key)
    {
        if (!HasKey(type, key))
        {
            Debug.LogWarning("Config data for key: " + key + " not available!");
        }

        try
        {
            string val = json[(int)type][key].ToString();
            string[] values = val.Split('_');
            Vector3 converted = new Vector3(float.Parse(values[1]), float.Parse((values[2])), float.Parse(values[3]));
            return converted;
        }
        catch (Exception e)
        {
            Debug.LogError("Unable to parse config data to Vector3: " + e);
            return Vector3.zero;
        }
    }

    /// <summary>
    /// Get a string from the config
    /// </summary>
    /// <param name="type">The config file type to get the keys for</param>
    /// <param name="key">The key to get the value for</param>
    /// <returns>The value for the key</returns>
    public static string GetString(Type type, string key)
    {
        if (!HasKey(type, key))
        {
            Debug.LogWarning("Config data for key: " + key + " not available!");
            return string.Empty;
        }

        return json[(int)type][key].ToString();
    }

    /// <summary>
    /// Get a bool from the config
    /// </summary>
    /// <param name="type">The config file type to get the keys for</param>
    /// <param name="key">The key to get the value for</param>
    /// <returns>The value for the key</returns>
    public static bool GetBool(Type type, string key)
    { 
        if (!HasKey(type, key))
        {
            Debug.LogWarning("Config data for key: " + key + " not available!");
        }

        try
        {
            return bool.Parse(json[(int)type][key].ToString());
        }
        catch (Exception e)
        {
            Debug.LogError("Unable to parse config data to float: " + e);
            return false;
        }
    }

    /// <summary>
    /// Set an int in the config
    /// </summary>
    /// <param name="type">The config file type</param>
    /// <param name="key">The key to set the value for</param>
    /// <param name="value">The value to set</param>
    public static void SetInt(Type type, string key, int value)
    {
        //Make sure the key exists
        if (!HasKey(type, key))
        {
            Debug.LogWarning("Config data for key: " + key + " not available!");
            return;
        }

        //Adapt the value and save
        json[(int)type][key] = value;
        Save(type);
    }

    /// <summary>
    /// Set a float in the config
    /// </summary>
    /// <param name="type">The config file type</param>
    /// <param name="key">The key to set the value for</param>
    /// <param name="value">The value to set</param>
    public static void SetFloat(Type type, string key, float value)
    {
        //Make sure the key exists
        if (!HasKey(type, key))
        {
            Debug.LogWarning("Config data for key: " + key + " not available!");
            return;
        }

        //Adapt the value and save
        json[(int)type][key] = value;
        Save(type);
    }

    /// <summary>
    /// Set a Vector3 in the config
    /// </summary>
    /// <param name="type">The config file type</param>
    /// <param name="key">The key to set the value for</param>
    /// <param name="value">The value to set</param>
    public static void SetVector3(Type type, string key, Vector3 value)
    {
        //Make sure the key exists
        if (!HasKey(type, key))
        {
            Debug.LogWarning("Config data for key: " + key + " not available!");
            return;
        }

        //Adapt the value and save
        json[(int)type][key] = "vec3_" + value.x + "_" + value.y + "_" + value.z;
        Save(type);
    }

    /// <summary>
    /// Set a bool in the config
    /// </summary>
    /// <param name="type">The config file type</param>
    /// <param name="key">The key to set the value for</param>
    /// <param name="value">The value to set</param>
    public static void SetBool(Type type, string key, bool value)
    {
        //Make sure the key exists
        if (!HasKey(type, key))
        {
            Debug.LogWarning("Config data for key: " + key + " not available!");
            return;
        }

        //Adapt the value and save
        json[(int)type][key] = value;
        Save(type);
    }

    /// <summary>
    /// Set a string in the config
    /// </summary>
    /// <param name="type">The config file type</param>
    /// <param name="key">The key to set the value for</param>
    /// <param name="value">The value to set</param>
    public static void SetString(Type type, string key, string value)
    {
        //Make sure the key exists
        if (!HasKey(type, key))
        {
            Debug.LogWarning("Config data for key: " + key + " not available!");
            return;
        }

        //Adapt the value and save
        json[(int)type][key] = value;
        Save(type);
    }
}

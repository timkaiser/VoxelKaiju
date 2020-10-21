using UnityEngine;
using UnityEditor;
using LitJson;
using System;
using System.Collections.Generic;

/// <summary>
/// Config Editor Window
/// </summary>
public class ConfigEditor : EditorWindow
{
    //The currently selected config category
    private int category;

    //Skin
    private GUISkin skin;

    //Colours
    private Color backgroundColor;
    private Color backgroundColor2;

    //Background textures
    private Texture2D backgroundTexture;
    private Texture2D backgroundTexture2;

    //Scroll pos
    private Vector2 scrollPos;
    private int offset;

    /// <summary>
    /// Opens and/or gets the window
    /// </summary>
    [MenuItem("Tools/Config Editor")]
    static void Init()
    {
        //Get an existing open window or open a new one
        ConfigEditor window = (ConfigEditor)GetWindow(typeof(ConfigEditor));
        window.titleContent = new GUIContent("Config Editor");
        window.Show();
    }

    /// <summary>
    /// Reload on Project change
    /// </summary>
    private void OnProjectChange()
    {
        Config.Init();
    }

    /// <summary>
    /// Reload on Project change
    /// </summary>
    private void OnEnable()
    {
        Config.Init();
    }

    private void InitResources()
    {
        //Get the skin
        skin = EditorGUIUtility.Load("EditorUI/ConfigEditorSkin.guiskin") as GUISkin;

        //Setup Colours
        backgroundColor = new Color(225.0f/255.0f, 225.0f / 255.0f, 225.0f / 255.0f);
        backgroundColor2 = new Color(79.0f / 255.0f, 82.0f / 255.0f, 89.0f / 255.0f);
        //Setup Textures
        backgroundTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        backgroundTexture.SetPixel(0, 0, backgroundColor);
        backgroundTexture.Apply();
        backgroundTexture2 = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        backgroundTexture2.SetPixel(0, 0, backgroundColor2);
        backgroundTexture2.Apply();

        //Set initial scrollPos
        scrollPos = Vector2.zero;

        //Set minimum window size
        minSize = new Vector2(400, 200);
    }

    /// <summary>
    /// Draw the window contents
    /// </summary>
    private void OnGUI()
    {
        //Deselect on mouse click
        if (Event.current.type == EventType.MouseDown)
            GUI.FocusControl(null);

        //Make sure the config backend is initialised
        if (!Config.IsInitalized())
            Config.Init();

        //Make sure the resources are initialised
        if (backgroundTexture == null || skin == null)
            InitResources();

        //Draw Background
        GUI.DrawTexture(new Rect(0, 0, position.width, position.height), backgroundTexture, ScaleMode.StretchToFill);

        //Draw Category dropdown
        GUILayout.BeginArea(new Rect(0, 0, position.width, 30));
        EditorGUI.BeginChangeCheck();
        category = EditorGUILayout.Popup(category, Enum.GetNames(typeof(Config.Type)), skin.FindStyle("Dropdown"));
        if (EditorGUI.EndChangeCheck())
        {
            scrollPos = Vector2.zero;
        }
        GUILayout.EndArea();

        //Draw titles
        GUILayout.BeginArea(new Rect(0, 40, (position.width - 24) / 2, 20));
        EditorGUILayout.LabelField("Key", skin.FindStyle("TableTitle"));
        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect((position.width - 34) / 2 + 2, 40, (position.width - 24) / 2, 20));
        EditorGUILayout.LabelField("Value", skin.FindStyle("TableTitle"));
        GUILayout.EndArea();

        //Get all json keys for the category
        List<string> keys = Config.GetAllKeys((Config.Type)category);

        //Draw the scrollbar
        GUISkin skinBuffer = GUI.skin;
        GUI.skin = skin;
        scrollPos = GUI.BeginScrollView(
            new Rect(0, 60, position.width - 3, position.height - 120),
            scrollPos,
            new Rect(0, 0, position.width - 16, (keys.Count + offset) * 22),
            false,
            true,
            skin.horizontalScrollbar,
            skin.FindStyle("Scrollbar"));
        GUI.skin = skinBuffer;

        //Draw all key/value pairs
        offset = 0;
        for (int i = 0; i < keys.Count; i++)
        {
            bool isVec3 = false;
            string key = keys[i];
            JsonData data = Config.GetJsonData((Config.Type)category, key);

            //Key
            GUILayout.BeginArea(new Rect(0, (i + offset) * 22, (position.width - 34) / 2, 20));
            string keyValue;
            EditorGUI.BeginChangeCheck();
            keyValue = EditorGUILayout.DelayedTextField(key, skin.FindStyle("KeyValue"));
            if (EditorGUI.EndChangeCheck())
            {
                Config.ChangeKey((Config.Type)category, key, keyValue);
            }
            GUILayout.EndArea();
            //Value
            if (data.IsInt)
            {
                GUILayout.BeginArea(new Rect((position.width - 34) / 2 + 2, (i + offset) * 22, (position.width - 34) / 2, 20));
                int value;
                EditorGUI.BeginChangeCheck();
                value = EditorGUILayout.DelayedIntField(Config.GetInt((Config.Type)category, keyValue), skin.FindStyle("KeyValue"));
                if (EditorGUI.EndChangeCheck())
                {
                    Config.SetInt((Config.Type)category, keyValue, value);
                }
                GUILayout.EndArea();
            }
            else if (data.IsDouble)
            {
                GUILayout.BeginArea(new Rect((position.width - 34) / 2 + 2, (i + offset) * 22, (position.width - 34) / 2, 20));
                float value;
                EditorGUI.BeginChangeCheck();
                value = EditorGUILayout.DelayedFloatField(Config.GetFloat((Config.Type)category, keyValue), skin.FindStyle("KeyValue"));
                if (EditorGUI.EndChangeCheck())
                {
                    Config.SetFloat((Config.Type)category, keyValue, value);
                }
                GUILayout.EndArea();
            }
            else if (data.IsBoolean)
            {
                GUILayout.BeginArea(new Rect((position.width - 34) / 2 + 2, (i + offset) * 22, (position.width - 34) / 2, 20));
                bool value = Config.GetBool((Config.Type)category, keyValue);
                EditorGUI.BeginChangeCheck();
                if (value)
                {
                    if(GUILayout.Button(string.Empty, skin.FindStyle("KeyValueBoolChecked")))
                    {
                        value = false;
                    }
                }
                else
                {
                    if (GUILayout.Button(string.Empty, skin.FindStyle("KeyValueBoolUnchecked")))
                    {
                        value = true;
                    }
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Config.SetBool((Config.Type)category, keyValue, value);
                }
                GUILayout.EndArea();
            }
            else if (data.IsString)
            {
                string value = Config.GetString((Config.Type)category, keyValue);
                if (!value.StartsWith("vec3"))
                {
                    GUILayout.BeginArea(new Rect((position.width - 34) / 2 + 2, (i + offset) * 22, (position.width - 34) / 2, 20));
                    EditorGUI.BeginChangeCheck();
                    value = EditorGUILayout.DelayedTextField(value, skin.FindStyle("KeyValue"));
                    if (EditorGUI.EndChangeCheck())
                    {
                        Config.SetString((Config.Type)category, keyValue, value);
                    }
                    GUILayout.EndArea();
                }
                else
                {
                    Vector3 val= Config.GetVector3((Config.Type)category, keyValue);
                    EditorGUI.BeginChangeCheck();
                    GUILayout.BeginArea(new Rect((position.width - 34) / 2 + 2, (i + offset) * 22, (position.width - 34) / 2, 20));
                    val.x = EditorGUILayout.DelayedFloatField(val.x, skin.FindStyle("KeyValue"));
                    GUILayout.EndArea();
                    GUILayout.BeginArea(new Rect((position.width - 34) / 2 + 2, (i + offset) * 22 + 22, (position.width - 34) / 2, 20));
                    val.y = EditorGUILayout.DelayedFloatField(val.y, skin.FindStyle("KeyValue"));
                    GUILayout.EndArea();
                    GUILayout.BeginArea(new Rect((position.width - 34) / 2 + 2, (i + offset) * 22 + 44, (position.width - 34) / 2, 20));
                    val.z = EditorGUILayout.DelayedFloatField(val.z, skin.FindStyle("KeyValue"));
                    GUILayout.EndArea();
                    offset += 2;
                    isVec3 = true;
                    if (EditorGUI.EndChangeCheck())
                    {
                        Config.SetVector3((Config.Type)category, keyValue, val);
                    }
                }
            }
            else
            {
                Debug.LogError("Error in config file: " + Enum.GetName(typeof(Config.Type), category) + ". " +
                    "The value for the key: " + key + " couldn't be parsed!");
            }

            //Draw the delete button for the key
            if (isVec3)
            {
                GUILayout.BeginArea(new Rect(position.width - 30, (i + offset - 2) * 22, 20, 20));
            }
            else
            {
                GUILayout.BeginArea(new Rect(position.width - 30, (i + offset) * 22, 20, 20));
            }
            if (GUILayout.Button("X", skin.FindStyle("Delete")))
            {
                Config.DeleteKey((Config.Type)category, key);
            }
            GUILayout.EndArea();
        }
        GUI.EndScrollView();

        //Draw the buttons for adding new pairs
        GUILayout.BeginArea(new Rect(0, position.height - 50, position.width, 30));
        GUI.DrawTexture(new Rect(0, 0, position.width, position.height), backgroundTexture2, ScaleMode.StretchToFill);
        GUILayout.BeginArea(new Rect(0, 0, position.width / 5 - 1, 30));
        if (GUILayout.Button("Add Int", skin.FindStyle("ButtonAdd")))
        {
            Config.AddInt((Config.Type)category, "NewIntKey", 0);
        }
        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect(position.width / 5 + 1, 0, position.width / 5 - 2, 30));
        if (GUILayout.Button("Add Float", skin.FindStyle("ButtonAdd")))
        {
            Config.AddFloat((Config.Type)category, "NewFloatKey", 0.0f);
        }
        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect(2 * position.width / 5 + 1, 0, position.width / 5 - 2, 30));
        if (GUILayout.Button("Add Bool", skin.FindStyle("ButtonAdd")))
        {
            Config.AddBool((Config.Type)category, "NewBoolKey", false);
        }
        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect((position.width / 5) * 3 + 1, 0, position.width / 5 - 2, 30));
        if (GUILayout.Button("Add String", skin.FindStyle("ButtonAdd")))
        {
            Config.AddString((Config.Type)category, "NewStringKey", "NewStringValue");
        }
        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect((position.width / 5) * 4 + 1, 0, position.width / 5 - 2, 30));
        if (GUILayout.Button("Add Vector3", skin.FindStyle("ButtonAdd")))
        {
            Config.AddVector3((Config.Type)category, "NewVector3", Vector3.zero);
        }
        GUILayout.EndArea();
        GUILayout.EndArea();

        //Draw the bottom info bar
        GUILayout.BeginArea(new Rect(0, position.height - 20, position.width, 30));
        EditorGUILayout.LabelField("Config Editor v.1.0", skin.FindStyle("BottomLabel"));
        GUILayout.EndArea();
    }
}

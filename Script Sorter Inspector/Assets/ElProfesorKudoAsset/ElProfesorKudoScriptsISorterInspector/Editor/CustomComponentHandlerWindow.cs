using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Collections.Generic;
using UnityEditorInternal;

// This Script is provide by el profesor kudo, check out my youtube channel https://www.youtube.com/@elprofesorkudo
// Fell free to buy me a coffee to support me :) https://buymeacoffee.com/elprofesorkudo

namespace ElProfesorKudoSorterComponent
{
    public class TypeOfHandlerGenerator : EditorWindow
    {
        private bool _toggleIsForceCreateScript = true;
        private bool _toggleIsOverrideScriptableObject = false;
        private bool _toggleIsLoadAutomaticallyPrefabs = true;
        private string _toolTipIsForceCreateScript = "If you check this box It will create the EnumGenerate.cs file in the Scripts folder if it's not already exists";
        private string _toolTipIsLoadAutomaticallyPrefabs = "If you check this box It will automatically retrieve all the prefab from the folder you have had";

        private string _toolTipWriteContent = "If you press this button this will retrieve all the cs file in " + Utils.folderScripsPath + " and write the enum content in the EnumGenerate.cs file and after it will fill the switch content of TypeOfHandlerSwitch.cs with the enum previously created.";
        private string _toolTipCreateScriptableObject = "If you press this button this will create the scriptable object in " + Utils.folderPathScribtableObject + " base on the enum content in EnumGenerate.cs";
        private string _toolTipCleanContent = "If you press this button this will clean the EnumGenerate.cs file and the TypeOfHandlerSwitch.cs file and also the scriptable object create in " + Utils.folderPathScribtableObject;
        private string _toolTipIsOverrideScriptableObject = "If you press this button this will override the existing scriptable object inside " + Utils.folderPathScribtableObject + " and this ll also erased all the type you already add into the list of you gameobject";
        private string _toolTipAddNewPathScripts = "This field allow you to add new path for adding other script that might be somewhere else inside your project";
        private string _toolTipAddNewPathPrefabs = "This field allow you to add new path for adding other prefab that might be somewhere else inside your project";

        private bool _isCompiling = false;
        private string _messageToLog = "";

        private string _inputFieldStringToAddPathScripts = "";
        private List<string> _listPathScript = new List<string>();

        private bool _isUserAllowToInteractWithSortPrefab = false;
        private string _inputFieldStringToAddPathPrefabs = "";
        private List<string> _listPathPrefabs = new List<string>();

        private ReorderableList _reorderableListTypeOfPrefab;
        private List<GameObject> _ListOfPrefabs = new List<GameObject>();

        private ReorderableList _reorderableListTypeOfHandler;
        private List<TypeOfHandler> _listScriptableTypeOfHandler = new List<TypeOfHandler>();

        private Vector2 _scrollPosition;

        [MenuItem("Tools/Type Of Handler")]
        public static void ShowWindow()
        {
            GetWindow<TypeOfHandlerGenerator>("Tools TypeOfHandler");
        }

        [MenuItem("Tools/Clear Editor player Prefs")]
        public static void ClearEditorPrefsNow()
        {
            EditorPrefs.DeleteAll();
            Debug.Log("All EditorPrefs has been deleted");
        }

        private void OnGUI()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            DrawButtonCreation();
            using (new GUILayout.HorizontalScope())
            {
                DrawListOfPathsScripts();
                DrawListOfPathsPrefabs();
            }
            using (new GUILayout.HorizontalScope())
            {
                DrawPrefabListTitle();
                DrawButtonClearPrefab();
                DrawButtonLoadPrefab();
                DrawButtonSortAllPrefab();
            }
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(new GUIContent("Load Autmatically prefab in list", _toolTipIsLoadAutomaticallyPrefabs));
                _toggleIsLoadAutomaticallyPrefabs = EditorGUILayout.Toggle(_toggleIsLoadAutomaticallyPrefabs);
            }
            using (new GUILayout.HorizontalScope())
            {
                _reorderableListTypeOfPrefab.DoLayoutList();
                _reorderableListTypeOfHandler.DoLayoutList();
            }
            EditorGUILayout.EndScrollView();
        }

        #region Path Script
        private void DrawListOfPathsScripts()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(new GUIContent("New Path Scripts", _toolTipAddNewPathScripts), GUILayout.Width(120));
                    _inputFieldStringToAddPathScripts = EditorGUILayout.TextField(_inputFieldStringToAddPathScripts);
                    if (GUILayout.Button("Add", GUILayout.Width(50)))
                    {
                        AddStringPathsScripts();
                    }
                }
                GUILayout.Space(10);
                EditorGUILayout.LabelField("List of Path for Script");
                for (int i = 0; i < _listPathScript.Count; i++)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField(_listPathScript[i]);
                        if (GUILayout.Button("Remove", GUILayout.Width(70)))
                        {
                            RemoveStringPathsScripts(i);
                            break;
                        }
                    }
                }
            }

        }
        private void AddStringPathsScripts()
        {
            if (!string.IsNullOrEmpty(_inputFieldStringToAddPathScripts))
            {
                if (!Directory.Exists(_inputFieldStringToAddPathScripts))
                {
                    Debug.LogWarning("Invalid directory path. of " + _inputFieldStringToAddPathScripts);
                    _inputFieldStringToAddPathScripts = "";
                    return;
                }
                if (!_listPathScript.Contains(_inputFieldStringToAddPathScripts))
                {
                    _listPathScript.Add(_inputFieldStringToAddPathScripts);
                    SaveListStringPathScript();
                }
                else
                {
                    Debug.LogWarning("Path already exists in the list.");
                }
            }
            _inputFieldStringToAddPathScripts = "";
        }
        private void RemoveStringPathsScripts(int index)
        {
            if (index >= 0 && index < _listPathScript.Count)
            {
                string tempPath = _listPathScript[index];
                _listPathScript.RemoveAt(index);
                SaveListStringPathScript();
            }
        }
        private void LoadListStringPathScripts()
        {
            string serializedList = EditorPrefs.GetString("FolderPathsScripts");

            if (!string.IsNullOrEmpty(serializedList))
            {
                _listPathScript = new List<string>(serializedList.Split(';'));
            }
        }
        private void SaveListStringPathScript()
        {
            string serializedList = string.Join(";", _listPathScript.ToArray());
            EditorPrefs.SetString("FolderPathsScripts", serializedList);
        }

        #endregion Path Script

        #region Path Prefab
        private void DrawListOfPathsPrefabs()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                using (new GUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(new GUIContent("New Path prefabs", _toolTipAddNewPathPrefabs), GUILayout.Width(120));
                    _inputFieldStringToAddPathPrefabs = EditorGUILayout.TextField(_inputFieldStringToAddPathPrefabs);
                    if (GUILayout.Button("Add", GUILayout.Width(50)))
                    {
                        AddStringPathsPrefabs();
                    }
                }
                GUILayout.Space(10);
                EditorGUILayout.LabelField("List of Path for prefabs");
                for (int i = 0; i < _listPathPrefabs.Count; i++)
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField(_listPathPrefabs[i]);
                        if (GUILayout.Button("Remove", GUILayout.Width(70)))
                        {
                            RemoveStringPathsPrefabs(i);
                            break;
                        }
                    }
                }
            }
        }
        private void AddStringPathsPrefabs()
        {
            if (!string.IsNullOrEmpty(_inputFieldStringToAddPathPrefabs))
            {
                if (!Directory.Exists(_inputFieldStringToAddPathPrefabs))
                {
                    Debug.LogWarning("Invalid directory path. of " + _inputFieldStringToAddPathPrefabs);
                    _inputFieldStringToAddPathPrefabs = "";
                    return;
                }
                if (!_listPathPrefabs.Contains(_inputFieldStringToAddPathPrefabs))
                {
                    _listPathPrefabs.Add(_inputFieldStringToAddPathPrefabs);
                    SaveListStringPathPrefabs();
                }
                else
                {
                    Debug.LogWarning("Path already exists in the list.");
                }
            }
            _inputFieldStringToAddPathPrefabs = "";
        }
        private void RemoveStringPathsPrefabs(int index)
        {
            if (index >= 0 && index < _listPathPrefabs.Count)
            {
                string tempPath = _listPathPrefabs[index];
                _listPathPrefabs.RemoveAt(index);
                SaveListStringPathPrefabs();
            }
        }
        private void LoadListStringPathPrefabs()
        {
            string serializedList = EditorPrefs.GetString("FolderPathsPrefabs");

            if (!string.IsNullOrEmpty(serializedList))
            {
                _listPathPrefabs = new List<string>(serializedList.Split(';'));
            }
        }
        private void SaveListStringPathPrefabs()
        {
            string serializedList = string.Join(";", _listPathPrefabs.ToArray());
            EditorPrefs.SetString("FolderPathsPrefabs", serializedList);
        }

        #endregion Path Prefab

        private void DrawButtonCreation()
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(new GUIContent("Force Creation EnumGenerate.cs", _toolTipIsForceCreateScript));
                _toggleIsForceCreateScript = EditorGUILayout.Toggle(_toggleIsForceCreateScript);
            }
            EditorGUILayout.LabelField(new GUIContent("Generate Enum and TypeOfHandlerSwitch", _toolTipWriteContent), EditorStyles.boldLabel);
            if (GUILayout.Button("Write content"))
            {
                string enumCode = GenerateTypeWantedEnum();
                WriteEnumGenerateContent(Utils.filePathEnumGenerate, _toggleIsForceCreateScript, enumCode);
                WriteSwitchContentTypeOfHandler(Utils.filePathTypeOfHandler, enumCode, GenerateTypeWantedEnum(true));
                AssetDatabase.Refresh();
                _messageToLog = "Write switch content in file: " + Utils.filePathTypeOfHandler + " done " + "\n" + "Write enum content in file: " + Utils.filePathEnumGenerate + " done";
            }
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(new GUIContent("Generate TypeOfHandler Scriptable Objects", _toolTipCreateScriptableObject), EditorStyles.boldLabel);
            }
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(new GUIContent("Override Scriptable Objects", _toolTipIsOverrideScriptableObject), EditorStyles.boldLabel);
                _toggleIsOverrideScriptableObject = EditorGUILayout.Toggle(_toggleIsOverrideScriptableObject);
            }

            if (GUILayout.Button("Create Scriptable Objects"))
            {
                GenerateTypeOfHandlerScriptableObjects();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            EditorGUILayout.LabelField(new GUIContent("Clean Enum,TypeOfHandlerSwitch,and Scriptable", _toolTipCleanContent), EditorStyles.boldLabel);
            if (GUILayout.Button("Clean All content"))
            {
                CleanEnumGenerateContent(Utils.filePathEnumGenerate);
                CleanSwitchContentTypeOfHandler(Utils.filePathTypeOfHandler);
                DeleteTypeOfHandlerScriptableObjects();
                AssetDatabase.Refresh();
                _messageToLog = "Clean done.";
            }
        }

        #region Enum Function

        private string GenerateTypeWantedEnum(bool withNameSpace = false)
        {
            List<Type> classList = Utils.LoadClassesFromFolder();

            string enumCode = "public enum TypeWanted\n{\n";

            for (int i = 0; i < classList.Count; i++)
            {
                if (i != classList.Count - 1)
                {
                    enumCode += "\t" + (withNameSpace ? classList[i] : classList[i].Name) + ",\n";
                }
                else
                {
                    enumCode += "\t" + (withNameSpace ? classList[i] : classList[i].Name) + "\n";
                }
            }
            enumCode += "}\n";

            return enumCode;
        }
        private void WriteEnumGenerateContent(string filePath, bool forceCreation = true, string enumCode = "")
        {
            if (!File.Exists(filePath) && !forceCreation)
            {
                Debug.LogWarning("File not found, in : " + filePath);
                Debug.LogWarning("Check Force Creation EnumGenerate.cs to force creation");
            }
            else if (!File.Exists(filePath) && forceCreation)
            {
                using (StreamWriter writer = File.CreateText(filePath))
                {
                    writer.WriteLine(enumCode);
                }
                File.WriteAllText(filePath, enumCode);
            }
            else
            {
                File.WriteAllText(filePath, enumCode);
            }

        }
        private void CleanEnumGenerateContent(string filePath)
        {
            if (File.Exists(filePath))
            {
                string enumDeclaration = "public enum TypeWanted\n{\nMonoBehaviour,\nComponent\n}\n";
                File.WriteAllText(filePath, enumDeclaration);
                Debug.LogWarning("Clean file at : " + filePath + " done ");
            }
            else
            {
                Debug.LogWarning("The file doesn't exist: " + filePath);
            }
        }

        #endregion Enum Function

        #region Type Function
        private string GenerateSwitchContent(string enumString, string enumStringWithNameSpace)
        {
            string[] enumNames = enumString
                .Replace("public enum TypeWanted", "")
                .Replace("{", "")
                .Replace("}", "")
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            string[] enumNamesWithNameSpace = enumStringWithNameSpace
                .Replace("public enum TypeWanted", "")
                .Replace("{", "")
                .Replace("}", "")
                .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);


            string switchContent = "switch (typeWanted) {\n";
            for (int i = 0; i < enumNames.Length; i++)
            {
                switchContent += $"\tcase TypeWanted.{enumNames[i].Trim()}:\n\t\treturn typeof({enumNamesWithNameSpace[i].Trim()});\n";
            }
            switchContent += "\tdefault:\n\t\treturn typeof(MonoBehaviour);\n}\n";

            return switchContent;
        }
        private void WriteSwitchContentTypeOfHandler(string filePath, string enumString, string enumStringWithNameSpace)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogWarning("file doesn't exist in : " + filePath + " so we created it !!");
                CreateFileTypeOfHandler(filePath);
            }
            string fileContent = File.ReadAllText(filePath);
            int startIndex = fileContent.IndexOf("switch (typeWanted)");
            int endIndex = fileContent.IndexOf("}", startIndex);

            if (startIndex != -1 && endIndex != -1)
            {
                string switchContent = fileContent.Substring(startIndex, endIndex - startIndex + 1);
                string newSwitchContent = GenerateSwitchContent(enumString, enumStringWithNameSpace);
                fileContent = fileContent.Replace(switchContent, newSwitchContent);
                File.WriteAllText(filePath, fileContent);
            }
            else
            {
                Debug.LogWarning("Switch not found in file: " + filePath);
            }
        }

        private static void CreateFileTypeOfHandler(string filePath)
        {
            string fileContent = @"using System;
using UnityEngine;

[CreateAssetMenu(fileName = ""TypeOfHandler"", menuName = ""El Profesor Kudo Asset/ Scripts Sorter / Scriptable Object / Type Of Handler"")]
public class TypeOfHandler : ScriptableObject
{
    public TypeWanted typeWanted;
    public Type GetBaseTypeToSearch()
    {
        switch (typeWanted)
        {
        }
    }
}";
            File.WriteAllText(filePath, fileContent);
            Debug.Log("File sucess created in :" + filePath);
        }

        private void CleanSwitchContentTypeOfHandler(string filePath)
        {
            if (File.Exists(filePath))
            {
                string fileContent = File.ReadAllText(filePath);
                int startIndex = fileContent.IndexOf("switch (typeWanted)");
                int endIndex = fileContent.IndexOf("}", startIndex);
                if (startIndex != -1 && endIndex != -1)
                {
                    string switchContent = fileContent.Substring(startIndex, endIndex - startIndex + 1);
                    string emptySwitch = "switch (typeWanted) {\n\tdefault:\n\t\treturn typeof(MonoBehaviour);\n}\n";
                    fileContent = fileContent.Replace(switchContent, emptySwitch);
                    File.WriteAllText(filePath, fileContent);
                    Debug.Log("The content of the switch has been clean : " + filePath);
                }
                else
                {
                    Debug.LogWarning("The switch didn't find in : " + filePath);
                }
            }
            else
            {
                Debug.LogWarning("File note found at  : " + filePath);
            }
        }

        #endregion Type Function

        #region Scriptable Function
        private void InitReordableListTypeWanted()
        {
            _reorderableListTypeOfHandler = new ReorderableList(_listScriptableTypeOfHandler, typeof(TypeOfHandler), true, true, true, true);
            _reorderableListTypeOfHandler.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Type Wanted");
            _reorderableListTypeOfHandler.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = _listScriptableTypeOfHandler[index];
                rect.y += 2;
                _listScriptableTypeOfHandler[index] = EditorGUI.ObjectField(rect, GUIContent.none, element, typeof(TypeOfHandler), true) as TypeOfHandler;
            };
        }
        private void SaveTypeOfHandlerList()
        {
            if (_listScriptableTypeOfHandler != null)
            {
                string serializedList = "";
                foreach (TypeOfHandler typeOfHandler in _listScriptableTypeOfHandler)
                {
                    string path = AssetDatabase.GetAssetPath(typeOfHandler);
                    if (!string.IsNullOrEmpty(path))
                    {
                        serializedList += path + ";";
                    }
                }
                EditorPrefs.SetString("TypeofHandlerChoose", serializedList);
            }
        }
        private void LoadTypeOfHandlerList()
        {
            string serializedList = EditorPrefs.GetString("TypeofHandlerChoose", "");
            if (!string.IsNullOrEmpty(serializedList))
            {
                _listScriptableTypeOfHandler = new List<TypeOfHandler>();
                string[] typeOfHandlerPaths = serializedList.Split(';');
                foreach (string path in typeOfHandlerPaths)
                {
                    TypeOfHandler typeOfHandler = AssetDatabase.LoadAssetAtPath<TypeOfHandler>(path);
                    if (typeOfHandler != null)
                    {
                        _listScriptableTypeOfHandler.Add(typeOfHandler);
                    }
                }
            }
        }

        private void GenerateTypeOfHandlerScriptableObjects()
        {
            if (!AssetDatabase.IsValidFolder(Utils.folderPathScribtableObject))
            {
                AssetDatabase.CreateFolder(Utils.parentFolderPathScribtableObject, "ScriptableObjects");
            }

            string[] assetGUIDs = AssetDatabase.FindAssets("t:TypeOfHandler", new string[] { Utils.folderPathScribtableObject });
            if (_toggleIsOverrideScriptableObject)
            {
                foreach (string guid in assetGUIDs)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    AssetDatabase.DeleteAsset(assetPath);
                }
            }
            if (System.Enum.GetValues(typeof(TypeWanted)).Length == 0)
            {
                Debug.LogWarning("No enum found, please generate it first.");
                return;
            }
            foreach (TypeWanted typeWanted in System.Enum.GetValues(typeof(TypeWanted)))
            {
                if (!_toggleIsOverrideScriptableObject)
                {
                    string assetName = typeWanted.ToString() + ".asset";
                    string assetPath = $"{Utils.folderPathScribtableObject}/{assetName}";
                    if (!File.Exists(assetPath))
                    {
                        TypeOfHandler typeOfHandler = ScriptableObject.CreateInstance<TypeOfHandler>();
                        typeOfHandler.typeWanted = typeWanted;
                        AssetDatabase.CreateAsset(typeOfHandler, assetPath);
                    }
                    else
                    {
                        Debug.LogWarning("Asset " + assetName + " already exists. Skipping...");
                    }
                }
                else
                {
                    TypeOfHandler typeOfHandler = ScriptableObject.CreateInstance<TypeOfHandler>();
                    typeOfHandler.typeWanted = typeWanted;
                    string assetPath = $"{Utils.folderPathScribtableObject}/{typeWanted.ToString()}.asset";
                    AssetDatabase.CreateAsset(typeOfHandler, assetPath);
                }

            }
            // use debug here cause Unity not recompiling
            Debug.Log("ScriptableObjects TypeOfHandler generation complete.");
        }
        private void DeleteTypeOfHandlerScriptableObjects()
        {
            if (!AssetDatabase.IsValidFolder(Utils.folderPathScribtableObject))
            {
                Debug.LogWarning("ScriptableObjects folder not found.");
                return;
            }

            string[] assetGUIDs = AssetDatabase.FindAssets("t:TypeOfHandler", new string[] { Utils.folderPathScribtableObject });
            foreach (string guid in assetGUIDs)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                AssetDatabase.DeleteAsset(assetPath);
            }

            AssetDatabase.Refresh();
            Debug.Log("ScriptableObjects TypeOfHandler assets deleted.");
        }

        #endregion Scriptable Function

        #region Sort All Prefab 
        private void InitReordableListPrefab()
        {
            _ListOfPrefabs = new List<GameObject>();
            _reorderableListTypeOfPrefab = new ReorderableList(_ListOfPrefabs, typeof(TypeOfHandler), true, true, true, true);
            _reorderableListTypeOfPrefab.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Prefab List");
            _reorderableListTypeOfPrefab.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                var element = _ListOfPrefabs[index];
                rect.y += 2;
                _ListOfPrefabs[index] = EditorGUI.ObjectField(rect, GUIContent.none, element, typeof(GameObject), true) as GameObject;
            };
        }

        private void DrawPrefabListTitle()
        {
            GUILayout.Label("Prefab List", EditorStyles.boldLabel);
        }
        private void DrawButtonClearPrefab()
        {
            if (GUILayout.Button("Clear Prefabs"))
            {
                _ListOfPrefabs.Clear();
            }
        }

        private void DrawButtonLoadPrefab()
        {
            if (GUILayout.Button("Load Prefabs"))
            {
                if (!_listPathPrefabs.Contains(Utils.folderPrefabsPath))
                {
                    _listPathPrefabs.Add(Utils.folderPrefabsPath);
                }
                LoadPrefabs(_listPathPrefabs);
            }
        }
        private void DrawButtonSortAllPrefab()
        {
            if (GUILayout.Button("SortAllPrefab"))
            {
                _listScriptableTypeOfHandler.RemoveAll(element => element == null);
                _ListOfPrefabs.RemoveAll(element => element == null);
                if (_listScriptableTypeOfHandler == null || _listScriptableTypeOfHandler.Count == 0 || _ListOfPrefabs == null || _ListOfPrefabs.Count == 0)
                {
                    Debug.LogWarning("Please Check your prefab list and type you want to sort make sure there is no empty element or no element at all");
                    return;
                }
                List<Type> listTypeAlreadySort = new List<Type>();
                int numberAlreadySortTypeComponent = 0;
                for (int i = 0; i < _listScriptableTypeOfHandler.Count; i++)
                {
                    foreach (GameObject prefabGameObject in _ListOfPrefabs)
                    {
                        Utils.SortComponent(_listScriptableTypeOfHandler[i].GetBaseTypeToSearch(), ref numberAlreadySortTypeComponent, true, false, true, listTypeAlreadySort, prefabGameObject);
                    }
                    listTypeAlreadySort.Add(_listScriptableTypeOfHandler[i].GetBaseTypeToSearch());
                }
            }
        }
        private void LoadPrefabs(List<string> folderPaths)
        {
            _ListOfPrefabs.Clear();

            foreach (string folderPath in folderPaths)
            {
                string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab", new string[] { folderPath });
                foreach (string prefabPath in prefabPaths)
                {
                    string prefabAssetPath = AssetDatabase.GUIDToAssetPath(prefabPath);
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabAssetPath);
                    if (prefab != null)
                    {
                        _ListOfPrefabs.Add(prefab);
                    }
                }
            }
        }

        #endregion Sort All Prefab

        #region Init

        private void OnEnable()
        {
            EditorApplication.update += () =>
            {
                CheckCompilationStatus(_messageToLog);
            };
            LoadListStringPathScripts();
            LoadListStringPathPrefabs();
            LoadTypeOfHandlerList();
            InitReordableListTypeWanted();
            InitReordableListPrefab();
            _toggleIsLoadAutomaticallyPrefabs = EditorPrefs.GetBool("ToggleIsLoadAutomaticallyPrefabs", false);
            if (_toggleIsLoadAutomaticallyPrefabs)
            {
                LoadPrefabs(_listPathPrefabs);
            }

        }
        private void OnDisable()
        {
            EditorApplication.update -= () =>
         {
             CheckCompilationStatus(_messageToLog);
         };
            SaveListStringPathScript();
            SaveListStringPathPrefabs();
            SaveTypeOfHandlerList();
            EditorPrefs.SetBool("ToggleIsLoadAutomaticallyPrefabs", _toggleIsLoadAutomaticallyPrefabs);
        }
        private void CheckCompilationStatus(string messageToShow)
        {
            if (!EditorApplication.isCompiling && _isCompiling)
            {
                Debug.Log("Compilation is done!");
                Debug.Log(messageToShow);
                _isCompiling = false;
            }
            else if (EditorApplication.isCompiling && !_isCompiling)
            {
                _isCompiling = true;
            }
        }

        #endregion Init
    }
}

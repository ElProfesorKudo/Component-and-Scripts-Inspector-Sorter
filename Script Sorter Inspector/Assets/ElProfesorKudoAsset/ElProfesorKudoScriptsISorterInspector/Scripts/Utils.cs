using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


// This Script is provide by el profesor kudo, check out my youtube channel https://www.youtube.com/@elprofesorkudo
// Fell free to buy me a coffee to support me :) https://buymeacoffee.com/elprofesorkudo

namespace ElProfesorKudoSorterComponent
{
    public static class Utils
    {
        public const string filePathEnumGenerate = "Assets/ElProfesorKudoAsset/ElProfesorKudoScriptsISorterInspector/Scripts/EnumGenerate.cs";
        public const string filePathTypeOfHandler = "Assets/ElProfesorKudoAsset/ElProfesorKudoScriptsISorterInspector/Scripts/TypeOfHandler.cs";
        public const string parentFolderPathScribtableObject = "Assets/ElProfesorKudoAsset/ElProfesorKudoScriptsISorterInspector";
        public const string folderPathScribtableObject = "Assets/ElProfesorKudoAsset/ElProfesorKudoScriptsISorterInspector/ScriptableObjects";
        public const string folderScripsPath = "Assets/ElProfesorKudoAsset/ElProfesorKudoScriptsISorterInspector/Scripts";
        public const string folderPrefabsPath = "Assets/ElProfesorKudoAsset/ElProfesorKudoScriptsISorterInspector/Prefabs";
        public static List<string> listOfFolderPathScripts = new List<string>() { folderScripsPath };
#if UNITY_EDITOR
        public static List<Type> LoadClassesFromFolder(bool addMonoBehaviourDefault = true, bool addComponentDefault = true)
        {
            List<Type> loadedClasses = new List<Type>();
            LoadPathFromEditorPlayerPref();
            foreach (string folderPath in listOfFolderPathScripts)
            {
                if (!Directory.Exists(folderPath))
                {
                    Debug.LogWarning("Folder path does not exist: " + folderPath);
                    continue;
                }
                string[] filePaths = Directory.GetFiles(folderPath, "*.cs", SearchOption.AllDirectories);

                foreach (string filePath in filePaths)
                {
                    UnityEditor.MonoScript script = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.MonoScript>(filePath);

                    if (script != null)
                    {
                        Type classType = script.GetClass();

                        if (classType != null)
                        {
                            loadedClasses.Add(classType);
                        }
                    }
                }
            }
            if (addMonoBehaviourDefault)
            {
                if (!loadedClasses.Contains(typeof(MonoBehaviour)))
                {
                    loadedClasses.Add(typeof(MonoBehaviour));
                }
            }
            if (addComponentDefault)
            {
                if (!loadedClasses.Contains(typeof(Component)))
                {
                    loadedClasses.Add(typeof(Component));
                }
            }
            return loadedClasses;
        }

        private static void LoadPathFromEditorPlayerPref()
        {
            string serializedList = UnityEditor.EditorPrefs.GetString("FolderPathsScripts");
            if (!string.IsNullOrEmpty(serializedList))
            {
                string[] pathsToAdd = serializedList.Split(';');
                foreach (string path in pathsToAdd)
                {
                    if (!listOfFolderPathScripts.Contains(path))
                    {
                        listOfFolderPathScripts.Add(path);
                    }
                    else
                    {
                        Debug.LogWarning("Path already exists in the list: " + path);
                    }
                }
            }
        }

        public static void SortComponent(Type mainBaseTypeSort, ref int numberAlreadySortTypeComponent, bool isAscendent = true, bool isShowFromTheTop = false, bool isSeparateType = true, List<Type> listTypeAlreadySort = null, GameObject go = null)
        {
            Component[] componentsArrayOfGameObject = GetArrayOfComponentInGameObject(go);

            List<Component> listSortTypeComponents = new List<Component>();

            foreach (Component componentToSort in componentsArrayOfGameObject)
            {
                List<Type> listSubClassOfCurrentComponent = new List<Type>();
                listSubClassOfCurrentComponent = Utils.GetSubClassesOf((componentToSort.GetType()), Utils.LoadClassesFromFolder()).ToList();
                if (isSeparateType)
                {
                    if (!listSubClassOfCurrentComponent.Contains(componentToSort.GetType()))
                    {
                        listSubClassOfCurrentComponent.Add(componentToSort.GetType());
                    }
                    if (componentToSort != null && (componentToSort.GetType().IsSubclassOf(mainBaseTypeSort) || componentToSort.GetType() == (mainBaseTypeSort)) &&
                    listTypeAlreadySort != null && !listTypeAlreadySort.Any(t => listSubClassOfCurrentComponent.Contains(t)))
                    {
                        listSortTypeComponents.Add(componentToSort);
                    }
                }
                else
                {
                    if (componentToSort != null && (listSubClassOfCurrentComponent.Contains(mainBaseTypeSort) || componentToSort.GetType() == (mainBaseTypeSort)))
                    {
                        listSortTypeComponents.Add(componentToSort);
                    }
                }

            }
            if (isAscendent)
            {
                listSortTypeComponents.Sort((x, y) => x.GetType().Name.CompareTo(y.GetType().Name));
            }
            else
            {
                listSortTypeComponents.Sort((x, y) => y.GetType().Name.CompareTo(x.GetType().Name));
            }
            if (!isShowFromTheTop)
            {
                for (int i = listSortTypeComponents.Count - 1; i >= 0; i--)
                {
                    int positionToGo = componentsArrayOfGameObject.Length - 1 - (listSortTypeComponents.Count - 1 - i); // Beacuse of the transform component that cannot be moved
                    int currentPosition = GetComponentPositionInInspector(listSortTypeComponents[i], go);
                    int delta = Mathf.Abs(positionToGo - currentPosition);
                    for (int j = 0; j < delta; j++)
                    {
                        if (positionToGo > currentPosition)
                        {
                            UnityEditorInternal.ComponentUtility.MoveComponentDown(listSortTypeComponents[i]);
                        }
                        else if (positionToGo < currentPosition)
                        {
                            UnityEditorInternal.ComponentUtility.MoveComponentUp(listSortTypeComponents[i]);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < listSortTypeComponents.Count; i++)
                {
                    int positionToGo = i + 1 + numberAlreadySortTypeComponent;
                    int currentPosition = GetComponentPositionInInspector(listSortTypeComponents[i], go);
                    int delta = Mathf.Abs(positionToGo - currentPosition);
                    for (int j = 0; j < delta; j++)
                    {
                        if (positionToGo > currentPosition)
                        {
                            UnityEditorInternal.ComponentUtility.MoveComponentDown(listSortTypeComponents[i]);
                        }
                        else if (positionToGo < currentPosition)
                        {
                            UnityEditorInternal.ComponentUtility.MoveComponentUp(listSortTypeComponents[i]);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
            numberAlreadySortTypeComponent += listSortTypeComponents.Count;
        }
        private static int GetComponentPositionInInspector(Component componentToCheck, GameObject go)
        {
            Component[] components = GetArrayOfComponentInGameObject(go);
            int index = -1;

            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == componentToCheck)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public static Component[] GetArrayOfComponentInGameObject(GameObject go)
        {
            return go.GetComponents<Component>();
        }

        public static bool IsTypePartOfSubClass(Type subType, Type mainType)
        {
            return GetSubClassesOf(subType, LoadClassesFromFolder()).ToList().Contains(mainType);
        }
#endif
        public static HashSet<Type> GetSubClassesOf(Type type, List<Type> listTypeInAsset)
        {
            HashSet<Type> hasSubClass = new HashSet<Type>();
            foreach (Type t in listTypeInAsset)
            {
                if (type.IsSubclassOf(t))
                {
                    hasSubClass.Add(t);
                    hasSubClass.UnionWith(GetSubClassesOf(t, listTypeInAsset));
                }
            }
            return hasSubClass;
        }
    }
}

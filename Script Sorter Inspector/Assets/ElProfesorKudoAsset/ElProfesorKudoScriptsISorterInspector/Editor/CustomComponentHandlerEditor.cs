using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// This Script is provide by el profesor kudo, check out my youtube channel https://www.youtube.com/@elprofesorkudo
// Fell free to buy me a coffee to support me :) https://buymeacoffee.com/elprofesorkudo

namespace ElProfesorKudoSorterComponent
{
    [CustomEditor(typeof(CustomComponentHandler))]
    public class CustomComponentHandlerEditor : Editor
    {
        #region Sort Variable

        private bool _toggleIsAlphabeticOrderSort = true;
        private bool _toggleIsShowFromTheTop = false;
        private bool _toggleIsSeparateTypeSort = true;
        private string _toolTipIsAlphabeticOrderSort = "Check this box if you want to sort the component by name.";
        private string _toolTipsIsShowFromTheTop = "If you check the Show from the top the sort ‘ll start after the transform component";
        private string _toolTipIsSeparateTypeSort = "Check this box if you want to sort the component and separate by type if you don't check it ll sort the component script without taking into account the type.";

        #endregion Sort Variable

        #region Search Variable

        private string _searchQuery = "";
        private string _idInputField = "SearchInputField";
        private bool _toggleIsAlphabeticOrderSearch = true;
        private bool _toggleIsCaseSensitive = false;
        private bool _toggleIsExactWord = false;
        private bool _toggleIsSearchContainAllInheritanceInclude = false;
        private bool _toggleIsSearchContainSpecificType = false;
        private bool _toggleIsSpecificTypeAllowInheritance = true;
        private string _toolTipIsAlphabeticOrderSearch = "Check this box if you want to search the component by name.";
        private string _toolTipIsCaseSensitive = "Check this box if you want a case sensitive search.";
        private string _toolTipIsExactWord = "Check this box if you want exact search.";
        private string _toolTipIsSearchContainAllInheritanceInclude = "Check this box if you want to search include all inheritance from the component.\n example : if your search abstract class, and you have a class inherit from it, it will be included.";
        private string _toolTipIsSpecificType = "Check this box if you want include only on a specific type on your search. (ie : AbstractFuse, Monobehaviour, etc)";
        private string _toolTipIsSpecificTypeAllowInheritance = "Check this box if you want to include also inheritance from your specific type)";

        #endregion Search Variable

        private bool _isUserAllowToInteractWithCustomUI = true;
        private CustomComponentHandler _customComponentHandlerScript;
        private List<Type> _listTypeWanted = new List<Type>();

        #region Init Function

        private void OnEnable()
        {
            _customComponentHandlerScript = (CustomComponentHandler)target;
            AddDefaultValueToListTypeOfWanted();
            GetTypeWanted();
            SetListTypeWanted();
        }
        private Type GetTypeWanted()
        {
            if (_customComponentHandlerScript.TypeOfWanted != null)
            {
                return _customComponentHandlerScript.TypeOfWanted.GetBaseTypeToSearch();
            }
            else
            {
                Debug.LogWarning("Please refresh the component to get the list of type wanted.");
                return null;
            }

        }
        private void SetListTypeWanted()
        {
            _listTypeWanted.Clear();
            foreach (TypeOfHandler toh in _customComponentHandlerScript.ListOfTypeOfWanted)
            {
                _listTypeWanted.Add(toh.GetBaseTypeToSearch());
            }
        }

        #endregion Init Function

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawWarningMessageListNull();
            DrawWarningMessageMonoBehaviourFirst();
            EditorGUILayout.Space();
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUI.enabled = _isUserAllowToInteractWithCustomUI;
                using (new GUILayout.HorizontalScope())
                {
                    DrawSearchInputFieldAndButton();
                    DrawCleanSearchButton();
                }
                using (new GUILayout.HorizontalScope())
                {
                    DrawSortBarButton();
                    DrawShuffleBarButton();
                }
                DrawOptionToggle();
            }
            if (string.IsNullOrEmpty(_searchQuery))
            {
                ForceCleanSearch();
            }
            else
            {
                if (!IsFocusIsOnSearchBar())
                {
                    DrawSearchResults(_customComponentHandlerScript);
                }
            }
            if (GUI.changed)
            {
                GetTypeWanted();
                SetListTypeWanted();
                DrawWarningMessageMonoBehaviourFirst();
            }
        }

        #region Common Function
        private void AddDefaultValueToListTypeOfWanted()
        {
            _customComponentHandlerScript.ListOfTypeOfWanted.RemoveAll(element => element == null);
            if (_customComponentHandlerScript.ListOfTypeOfWanted.Count == 0 || _customComponentHandlerScript.ListOfTypeOfWanted == null)
            {
                string assetPath = Utils.folderPathScribtableObject + "/MonoBehaviour.asset";
                TypeOfHandler defaultTypeOfHandler = AssetDatabase.LoadAssetAtPath<TypeOfHandler>(assetPath);

                if (defaultTypeOfHandler != null)
                {
                    _customComponentHandlerScript.ListOfTypeOfWanted.Add(defaultTypeOfHandler);
                }
                else
                {
                    Debug.LogWarning("Default TypeOfHandler asset not found at path: " + assetPath);
                    Debug.LogWarning("Please use Tool type handler to generate at least one scribtable object, also make sure to have at least one enum TypeWanted in " + Utils.filePathEnumGenerate);
                    Debug.LogWarning("Or check the path to see if it correctly writing");
                }
            }

        }
        private void UpdateListSearchComponent()
        {
            _customComponentHandlerScript.SearchComponents(_searchQuery, _toggleIsAlphabeticOrderSearch, _toggleIsSearchContainAllInheritanceInclude || _toggleIsSpecificTypeAllowInheritance, _toggleIsCaseSensitive, _toggleIsExactWord);
        }

        #endregion Common Function

        #region Draw Custom UI
        private void DrawWarningMessageListNull()
        {
            if (_customComponentHandlerScript.ListOfTypeOfWanted.Count == 0 || _customComponentHandlerScript.TypeOfWanted == null)
            {
                EditorGUILayout.Space();
                GUIStyle redStyle = new GUIStyle(EditorStyles.label);
                redStyle.normal.textColor = Color.red;
                redStyle.fontStyle = FontStyle.Bold;
                redStyle.alignment = TextAnchor.MiddleCenter;
                EditorGUILayout.LabelField(" PLEASE MAKE SURE TO HAVE A LIST NOT EMPTY ", redStyle);
                _isUserAllowToInteractWithCustomUI = false;
                DrawButtonSetDefaultValueIntoList();
                EditorGUILayout.Space();
            }
            else
            {
                _isUserAllowToInteractWithCustomUI = true;
            }
        }
        private void DrawButtonSetDefaultValueIntoList()
        {
            if (GUILayout.Button("Set Default Value into list"))
            {
                AddDefaultValueToListTypeOfWanted();
            }
        }
        private void DrawWarningMessageMonoBehaviourFirst()
        {
            if (_customComponentHandlerScript.ListOfTypeOfWanted.Count != 0 && _customComponentHandlerScript.ListOfTypeOfWanted[0].GetBaseTypeToSearch() == typeof(MonoBehaviour))
            {
                EditorGUILayout.Space();
                GUIStyle yellowStyle = new GUIStyle(EditorStyles.label);
                yellowStyle.normal.textColor = Color.yellow;
                yellowStyle.fontStyle = FontStyle.Italic;
                yellowStyle.alignment = TextAnchor.MiddleCenter;
                EditorGUILayout.LabelField(" Warning : If you put Monobehaviour on first item of the list, ", yellowStyle);
                EditorGUILayout.LabelField("when you ll sort , this will not take into account the other class,", yellowStyle);
                EditorGUILayout.LabelField("cause at the end all class heritate from MonoBehaviour", yellowStyle);
                EditorGUILayout.Space();
            }
        }
        private void DrawSearchInputFieldAndButton()
        {
            EditorGUILayout.LabelField("Search", GUILayout.Width(60));

            GUI.SetNextControlName(_idInputField);
            _searchQuery = EditorGUILayout.TextField(_searchQuery);

            if (GUILayout.Button("Search") || Event.current.isKey && Event.current.keyCode == KeyCode.Return)
            {
                UpdateListSearchComponent();
                GUI.FocusControl(null);
            }
        }
        private void DrawCleanSearchButton()
        {
            if (GUILayout.Button("Clean Search "))
            {
                _searchQuery = "";
                _customComponentHandlerScript.CleanSearch();
            }
        }
        private void DrawSortBarButton()
        {
            if (GUILayout.Button("Sort"))
            {
                List<Type> listTypeAlreadySort = new List<Type>();
                int numberAlreadySortTypeComponent = 0;
                for (int i = 0; i < _listTypeWanted.Count; i++)
                {
                    _customComponentHandlerScript.SortComponent(_listTypeWanted[i], ref numberAlreadySortTypeComponent, _toggleIsAlphabeticOrderSort, _toggleIsShowFromTheTop,
                                                                _toggleIsSeparateTypeSort, listTypeAlreadySort);
                    listTypeAlreadySort.Add(_listTypeWanted[i]);
                }
            }
        }
        private void DrawShuffleBarButton()
        {
            if (GUILayout.Button("Shuffle"))
            {
                _customComponentHandlerScript.ShuffleComponent();
            }
        }
        private void DrawOptionToggle()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Option Sort", EditorStyles.boldLabel);
            }

            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(new GUIContent("Alphabetic", _toolTipIsAlphabeticOrderSort), GUILayout.Width(65));
                    _toggleIsAlphabeticOrderSort = EditorGUILayout.Toggle(_toggleIsAlphabeticOrderSort);
                    EditorGUILayout.LabelField(new GUIContent("Show From the top", _toolTipsIsShowFromTheTop), GUILayout.Width(110));
                    _toggleIsShowFromTheTop = EditorGUILayout.Toggle(_toggleIsShowFromTheTop);
                    EditorGUILayout.LabelField(new GUIContent("Separate Type", _toolTipIsSeparateTypeSort), GUILayout.Width(90));
                    _toggleIsSeparateTypeSort = EditorGUILayout.Toggle(_toggleIsSeparateTypeSort);

                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Option search", EditorStyles.boldLabel);
            }
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.LabelField(new GUIContent("Alphabetic", _toolTipIsAlphabeticOrderSearch), GUILayout.Width(65));
                    _toggleIsAlphabeticOrderSearch = EditorGUILayout.Toggle(_toggleIsAlphabeticOrderSearch);
                    EditorGUILayout.LabelField(new GUIContent("Case sensitive", _toolTipIsCaseSensitive), GUILayout.Width(90));
                    _toggleIsCaseSensitive = EditorGUILayout.Toggle(_toggleIsCaseSensitive);
                    EditorGUILayout.LabelField(new GUIContent("Exact Word", _toolTipIsExactWord), GUILayout.Width(70));
                    _toggleIsExactWord = EditorGUILayout.Toggle(_toggleIsExactWord);

                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (!_toggleIsSearchContainSpecificType)
                    {
                        EditorGUILayout.LabelField(new GUIContent("All (InheritanceInclude)", _toolTipIsSearchContainAllInheritanceInclude), GUILayout.Width(130));
                        _toggleIsSearchContainAllInheritanceInclude = EditorGUILayout.Toggle(_toggleIsSearchContainAllInheritanceInclude);
                        _toggleIsSpecificTypeAllowInheritance = false;
                    }

                    if (!_toggleIsSearchContainAllInheritanceInclude)
                    {
                        EditorGUILayout.LabelField(new GUIContent("Specific Type", _toolTipIsSpecificType), GUILayout.Width(90));
                        _toggleIsSearchContainSpecificType = EditorGUILayout.Toggle(_toggleIsSearchContainSpecificType);
                        _toggleIsSearchContainAllInheritanceInclude = false;

                    }
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (_toggleIsSearchContainSpecificType)
                    {
                        DrawDropDownMenuScriptableObject();

                    }
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (_toggleIsSearchContainSpecificType)
                    {
                        EditorGUILayout.LabelField(new GUIContent("Include Inheritance", _toolTipIsSpecificTypeAllowInheritance), GUILayout.Width(110));
                        _toggleIsSpecificTypeAllowInheritance = EditorGUILayout.Toggle(_toggleIsSpecificTypeAllowInheritance);
                    }
                }
            }
            UpdateListSearchComponent();
        }
        private void DrawDropDownMenuScriptableObject()
        {
            List<string> listTypeWantedString = new List<string>();
            for (int i = 0; i < _listTypeWanted.Count; i++)
            {
                listTypeWantedString.Add(_listTypeWanted[i].ToString());
            }

            Rect popupRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
            EditorGUIUtility.labelWidth = 120f;

            int newSelectedIndex = EditorGUI.Popup(popupRect, "Type Selected", _customComponentHandlerScript.IndexDefaultTake, listTypeWantedString.ToArray());

            if (newSelectedIndex != _customComponentHandlerScript.IndexDefaultTake)
            {
                _customComponentHandlerScript.IndexDefaultTake = newSelectedIndex;
                GetTypeWanted();
            }
        }
        #endregion Draw Custom UI

        #region Draw Search Results

        private void DrawSearchResults(CustomComponentHandler componentSearch)
        {
            EditorGUILayout.LabelField("Search Results ", EditorStyles.boldLabel);

            foreach (Component result in componentSearch.SearchListResultsComponent)
            {
                if (_toggleIsSearchContainSpecificType)
                {
                    if (_toggleIsSpecificTypeAllowInheritance)
                    {
                        if (GetTypeWanted() == result.GetType() || result.GetType().IsSubclassOf(GetTypeWanted()))
                        {
                            ColorChangeResult(result);
                            EditorGUILayout.ObjectField("Script", result, GetTypeWanted(), true);
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if (GetTypeWanted() == result.GetType())
                        {
                            ColorChangeResult(result);
                            EditorGUILayout.ObjectField("Script", result, GetTypeWanted(), true);
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    ColorChangeResult(result);
                    // Handle all the cases of the component
                    MonoBehaviour monoBehaviour = result.GetComponent<MonoBehaviour>();
                    if (monoBehaviour == null)
                    {
                        EditorGUILayout.ObjectField("Component", result, typeof(Component), true);
                        //EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(monoBehaviour), typeof(MonoScript), false);
                    }
                }
                // Draw all properties of the component
                SerializedObject serializedObject = new SerializedObject(result);
                SerializedProperty property = serializedObject.GetIterator();
                bool enterChildren = true;
                while (property.NextVisible(enterChildren))
                {
                    EditorGUILayout.PropertyField(property, true);
                    enterChildren = false;
                }

                // Apply modifications
                serializedObject.ApplyModifiedProperties();
            }
        }
        private void ColorChangeResult(Component result)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUI.contentColor = GetLabelStyle(result); // Change the content color to yellow for highlighting
                EditorGUILayout.LabelField(result.GetType().Name, EditorStyles.boldLabel);// Change the content color to yellow for highlighting
                GUI.contentColor = Color.white; // Reset content color to default
                GUIStyle greyStyle = new GUIStyle(EditorStyles.label);
                greyStyle.normal.textColor = Color.grey;
                for (int i = 0; i < 3; i++)
                {

                }
                EditorGUILayout.LabelField(" : " + result.GetType().BaseType.Name, greyStyle);
            }

        }
        private Color GetLabelStyle(Component result)
        {
            if (_toggleIsSearchContainAllInheritanceInclude)
            {
                return Color.blue;
            }
            else
            {
                if (_toggleIsSearchContainSpecificType)
                {
                    if (_toggleIsSpecificTypeAllowInheritance)
                    {
                        if (result.GetType() == GetTypeWanted())
                        {
                            return Color.green;
                        }
                        else if (Utils.IsTypePartOfSubClass(result.GetType(), GetTypeWanted()))
                        {
                            return Color.cyan;
                        }
                        else
                        {
                            return Color.yellow;
                        }
                    }
                    else
                    {
                        return result.GetType() == GetTypeWanted() ? Color.green : Color.yellow;

                    }
                }
                else
                {
                    return Color.yellow;
                }

            }
        }
        private void ForceCleanSearch()
        {
            _searchQuery = "";
            _customComponentHandlerScript.CleanSearch();
        }
        private bool IsFocusIsOnSearchBar()
        {
            return GUI.GetNameOfFocusedControl() == _idInputField;
        }

        #endregion Draw Search Results

    }
}

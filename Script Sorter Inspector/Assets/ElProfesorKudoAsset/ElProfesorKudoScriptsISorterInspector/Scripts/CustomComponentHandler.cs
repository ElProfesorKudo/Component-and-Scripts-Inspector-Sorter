using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


// This Script is provide by el profesor kudo, check out my youtube channel https://www.youtube.com/@elprofesorkudo
// Fell free to buy me a coffee to support me :) https://buymeacoffee.com/elprofesorkudo

namespace ElProfesorKudoSorterComponent
{
    public class CustomComponentHandler : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private List<TypeOfHandler> _listOfTypeOfWanted = new List<TypeOfHandler>();
        private List<Component> _searchListResultsComponent = new List<Component>();
        private int _indexDefaultTake = 0;

        public TypeOfHandler TypeOfWanted { get => SetDefaultTypeOfHandler(_indexDefaultTake); }
        public List<Component> SearchListResultsComponent { get => _searchListResultsComponent; set => _searchListResultsComponent = value; }
        public List<TypeOfHandler> ListOfTypeOfWanted { get => _listOfTypeOfWanted; }
        public int IndexDefaultTake { get => _indexDefaultTake; set => _indexDefaultTake = value; }

        #region Search Functionality

        public void SearchComponents(string searchQuery, bool isAscendent, bool includeAllInheritanceInclude = false, bool caseSensitive = false, bool wholeWord = false)
        {
            _searchListResultsComponent.Clear();
            if (!caseSensitive)
            {
                searchQuery = searchQuery.ToLower();
            }
            foreach (Component component in GetArrayOfComponentInGameObject())
            {
                if (wholeWord)
                {
                    if (includeAllInheritanceInclude)
                    {
                        if (IsComponentExactQuery(component.GetType().Name, searchQuery, caseSensitive) ||
                            IsSubClassComponentExactQuery(component, searchQuery, caseSensitive))
                        {
                            _searchListResultsComponent.Add(component);
                        }
                    }
                    else
                    {
                        if (IsComponentExactQuery(component.GetType().Name, searchQuery, caseSensitive))
                        {
                            _searchListResultsComponent.Add(component);
                        }
                    }

                }
                else
                {
                    if (includeAllInheritanceInclude)
                    {
                        if (IsComponentContainsQuery(component.GetType().Name, searchQuery, caseSensitive) ||
                            IsSubClassComponentContainsQuery(component, searchQuery, caseSensitive))
                        {
                            _searchListResultsComponent.Add(component);
                        }
                    }
                    else
                    {
                        if (IsComponentContainsQuery(component.GetType().Name, searchQuery, caseSensitive))
                        {
                            _searchListResultsComponent.Add(component);
                        }
                    }

                }
            }
            if (isAscendent)
            {
                _searchListResultsComponent.Sort((x, y) => x.GetType().Name.CompareTo(y.GetType().Name));
            }
            else
            {
                _searchListResultsComponent.Sort((x, y) => y.GetType().Name.CompareTo(x.GetType().Name));
            }
        }
        private bool IsComponentContainsQuery(string componentName, string searchQuery, bool caseSensitive = false)
        {
            string elementName = componentName;
            if (!caseSensitive)
            {
                elementName = componentName.ToLower();
            }
            return elementName.Contains(searchQuery);
        }
        private bool IsSubClassComponentContainsQuery(Component component, string searchQuery, bool caseSensitive)
        {
            List<Type> _inheritanceType = new List<Type>();
            _inheritanceType = Utils.GetSubClassesOf(component.GetType(), Utils.LoadClassesFromFolder()).ToList();
            bool isInheritanceTypeContainQuery = false;
            foreach (Type inheritanceType in _inheritanceType)
            {
                if (IsComponentContainsQuery(inheritanceType.ToString(), searchQuery, caseSensitive))
                {
                    isInheritanceTypeContainQuery = true;
                    break;
                }
            }
            return isInheritanceTypeContainQuery;
        }
        private bool IsComponentExactQuery(string componentName, string searchQuery, bool caseSensitive = false)
        {
            string elementName = componentName;
            if (!caseSensitive)
            {
                elementName = componentName.ToLower();
            }
            return elementName.Equals(searchQuery);

        }
        private bool IsSubClassComponentExactQuery(Component component, string searchQuery, bool caseSensitive)
        {
            List<Type> _inheritanceType = new List<Type>();
            _inheritanceType = Utils.GetSubClassesOf(component.GetType(), Utils.LoadClassesFromFolder()).ToList();
            bool isInheritanceTypeContainQuery = false;
            foreach (Type inheritanceType in _inheritanceType)
            {
                if (IsComponentExactQuery(inheritanceType.ToString(), searchQuery, caseSensitive))
                {
                    isInheritanceTypeContainQuery = true;
                    break;
                }
            }
            return isInheritanceTypeContainQuery;
        }
        public void CleanSearch()
        {
            _searchListResultsComponent.Clear();
        }

        #endregion Search Functionality

        #region Sort Functionality
        public void SortComponent(Type mainBaseTypeSort, ref int numberAlreadySortTypeComponent, bool isAscendent = true, bool isShowFromTheTop = false, bool isSeparateType = true, List<Type> listTypeAlreadySort = null)
        {
            Utils.SortComponent(mainBaseTypeSort, ref numberAlreadySortTypeComponent, isAscendent, isShowFromTheTop, isSeparateType, listTypeAlreadySort, gameObject);
        }
        #endregion Sort Functionality

        #region Shuffle Functionality

        public void ShuffleComponent()
        {
            Component[] componentsArray = GetArrayOfComponentInGameObject();
            List<Component> componentList = new List<Component>();
            for (int i = 0; i < componentsArray.Length; i++)
            {
                componentList.Add(componentsArray[i]);
            }
            Shuffle(componentList);
            for (int i = 0; i < componentList.Count; i++)
            {
                int randomNumber = UnityEngine.Random.Range(0, componentList.Count);
                for (int j = 0; j < randomNumber; j++)
                {
                    UnityEditorInternal.ComponentUtility.MoveComponentDown(componentsArray[i]);
                }
            }
        }
        private void Shuffle<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, list.Count);
                T temp = list[i];
                list[i] = list[randomIndex];
                list[randomIndex] = temp;
            }
        }

        #endregion Shuffle Functionality

        #region Common Functionality

        public Component[] GetArrayOfComponentInGameObject()
        {
            return Utils.GetArrayOfComponentInGameObject(gameObject);
        }
        public TypeOfHandler SetDefaultTypeOfHandler(int index)
        {
            if (index > _listOfTypeOfWanted.Count - 1)
            {
                _indexDefaultTake = 0;
                index = 0;
                Debug.LogWarning("You remove one or more of the item in the list via the inspector so the Default type 'll be the first item of the list if this one exist");
            }
            return _listOfTypeOfWanted.Count != 0 ? _listOfTypeOfWanted[index] : null;
        }

        #endregion Common Functionality
#endif
    }
}

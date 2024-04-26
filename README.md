Custom Component Handler

Welcome to the Custom Component Handler documentation! This tool empowers you to efficiently manage and organize components within the Inspector of your Unity game objects. Whether you're dealing with a plethora of components or simply seeking to streamline your workflow, the Custom Component Handler is here to help.
Getting Started

    Installation: Simply download the Custom Component Handler package and import it into your Unity project.

    Opening the Tool: Navigate to Tool -> TypeOfHandler in the Unity Editor to open the Custom Component Handler window.

    Generating Scripts: Click on Write Content to generate necessary scripts, including EnumGenerate.cs and TypeOfHandler.cs.

    Creating Scriptable Objects: Press the Create Scriptable object button to generate scriptable objects in the designated ScriptableObjects folder.

Usage

Once you've completed the setup, follow these steps to effectively utilize the Custom Component Handler:

    Attaching the Handler: Add the CustomComponentHandler.cs component to your game object.

    Sorting Components: Utilize the ListOfTypeOfWanted to specify the types of components you wish to sort. By default, MonoBehaviour is selected. Click Sort to organize components alphabetically.

    Custom Sorting: Add multiple types to the list for more granular sorting. Components will be sorted based on the priority set in the list.

    Search Functionality: Use the search bar to find components based on various criteria such as alphabetic order, case sensitivity, exact word matches, and inheritance inclusion.

    Managing External Scripts/Prefabs: Easily incorporate scripts or prefabs located elsewhere by specifying their folder paths in the designated area.

    Loading Prefabs: Click on Load Prefab to load prefabs from specified paths, enabling you to organize them efficiently.

    Sorting Prefabs by Groups: Ensure your prefab list and type wanted list are populated, then select the desired sorting order and click SortAllPrefab to organize prefabs accordingly.

This is before sorting :
![Capture d’écran 2024-04-26 à 19 13 36](https://github.com/ElProfesorKudo/Component-and-Scripts-Inspector-Sorter/assets/165610217/b9838d5f-91ad-402e-9c31-3ba995df51b7)


This is after sorting
![Capture d’écran 2024-04-26 à 19 20 18](https://github.com/ElProfesorKudo/Component-and-Scripts-Inspector-Sorter/assets/165610217/dc8d61b6-5d15-4acb-be60-3967e79c3c01)

For more precise view check the pdf document I made you can find it here :
https://github.com/ElProfesorKudo/Component-and-Scripts-Inspector-Sorter/tree/main/Script%20Sorter%20Inspector/Assets/ElProfesorKudoAsset/ElProfesorKudoScriptsISorterInspector/Documentation

If you only want the package  to add it to your project you can download it here:
https://github.com/ElProfesorKudo/Component-and-Scripts-Inspector-Sorter/tree/main/Script%20Sorter%20Inspector/ExportPackage


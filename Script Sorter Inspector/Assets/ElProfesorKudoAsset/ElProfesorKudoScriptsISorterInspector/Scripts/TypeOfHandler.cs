using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TypeOfHandler", menuName = "El Profesor Kudo Asset/Scripts Sorter/Scriptable Object/Type Of Handler")]
public class TypeOfHandler : ScriptableObject
{
	public TypeWanted typeWanted;
	public Type GetBaseTypeToSearch()
	{
		switch (typeWanted) {
	default:
		return typeof(MonoBehaviour);
}








	}
}
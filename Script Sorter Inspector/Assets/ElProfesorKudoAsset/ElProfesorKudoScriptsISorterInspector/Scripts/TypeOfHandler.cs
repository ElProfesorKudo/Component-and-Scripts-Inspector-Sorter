using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TypeOfHandler", menuName = "El Profesor Kudo Asset/Scripts Sorter/Scriptable Object/Type Of Handler")]
public class TypeOfHandler : ScriptableObject
{
	public TypeWanted typeWanted;
	public Type GetBaseTypeToSearch()
	{
		switch (typeWanted) {
	case TypeWanted.CustomComponentHandler:
		return typeof(ElProfesorKudoSorterComponent.CustomComponentHandler);
	case TypeWanted.TypeOfHandler:
		return typeof(TypeOfHandler);
	case TypeWanted.Utils:
		return typeof(ElProfesorKudoSorterComponent.Utils);
	case TypeWanted.AbstractElProfesorKudo:
		return typeof(ElProfesorKudoSorterComponent.AbstractElProfesorKudo);
	case TypeWanted.BaseParticleFluidSourceManager:
		return typeof(BaseParticleFluidSourceManager);
	case TypeWanted.ElProfesorKudoClassA:
		return typeof(ElProfesorKudoSorterComponent.ElProfesorKudoClassA);
	case TypeWanted.ElProfesorKudoClassB:
		return typeof(ElProfesorKudoSorterComponent.ElProfesorKudoClassB);
	case TypeWanted.ElProfesorKudoClassC:
		return typeof(ElProfesorKudoSorterComponent.ElProfesorKudoClassC);
	case TypeWanted.ElProfesorKudoClassD:
		return typeof(ElProfesorKudoSorterComponent.ElProfesorKudoClassD);
	case TypeWanted.ElProfesorKudoCobaComponent:
		return typeof(ElProfesorKudoSorterComponent.ElProfesorKudoCobaComponent);
	case TypeWanted.ElProfesorKudoCobaDeformableFixationComponent:
		return typeof(ElProfesorKudoSorterComponent.ElProfesorKudoCobaDeformableFixationComponent);
	case TypeWanted.ElProfesorKudoCobaMaterialComponent:
		return typeof(ElProfesorKudoSorterComponent.ElProfesorKudoCobaMaterialComponent);
	case TypeWanted.ElProfesorKudoGraspedBodyComponent:
		return typeof(ElProfesorKudoSorterComponent.ElProfesorKudoGraspedBodyComponent);
	case TypeWanted.ElProfesorKudoPaintableComponent:
		return typeof(ElProfesorKudoSorterComponent.ElProfesorKudoPaintableComponent);
	case TypeWanted.ElProfesorKudoRenderMeshComponent:
		return typeof(ElProfesorKudoSorterComponent.ElProfesorKudoRenderMeshComponent);
	case TypeWanted.ElProfesorKudoTetMeshComponent:
		return typeof(ElProfesorKudoSorterComponent.ElProfesorKudoTetMeshComponent);
	case TypeWanted.ElProfesorKudoTetMeshDistanceFieldComponent:
		return typeof(ElProfesorKudoSorterComponent.ElProfesorKudoTetMeshDistanceFieldComponent);
	case TypeWanted.ElProfesorKudoTriMeshComponent:
		return typeof(ElProfesorKudoSorterComponent.ElProfesorKudoTriMeshComponent);
	case TypeWanted.OrganHapticsConfigurator:
		return typeof(OrganHapticsConfigurator);
	case TypeWanted.SoftBodyAnatomy:
		return typeof(SoftBodyAnatomy);
	case TypeWanted.SoftBodyCollisionTag:
		return typeof(SoftBodyCollisionTag);
	case TypeWanted.MonoBehaviour:
		return typeof(UnityEngine.MonoBehaviour);
	case TypeWanted.Component:
		return typeof(UnityEngine.Component);
	default:
		return typeof(MonoBehaviour);
}







	}
}
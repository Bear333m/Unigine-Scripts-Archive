using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "c59da2d01b62ff42181fe7e6492cba175b985e2f")]
public class GetCamRayObj : Component
{
	[ShowInEditor]
	[Parameter(Title = "Камера")]
	private PlayerDummy shootingCamera = null;

	[ShowInEditor]
	[ParameterMask(MaskType = ParameterMaskAttribute.TYPE.INTERSECTION)]
	[Parameter(Title = "Mаска пересечения")]
	private int mask = ~0;

	private dvec3 p0, p1;
	private Object selectedObject;
	private Object pastObject;

	void Init()
	{
		// write here code to be called on component initialization
		
	}
	
	void Update()
	{
		selectedObject = GetObject();
		if (selectedObject && selectedObject.GetComponent<ObjectProperties>()) {		//.GetComponents<Component>().Length > 0
			if (selectedObject.GetComponent<ObjectProperties>().isInteractable) {
				SetOutline(selectedObject, 1, new vec3(1.0f, 1.0f, 1.0f)); // подсвечиваем объект
			}
		}
		if (pastObject != selectedObject)		//если мы больше не наведены, выключаем подсветку с последнего объекта
		{
			if (pastObject != null)
			{
				SetOutline(pastObject, 0, new vec3(1.0f, 1.0f, 1.0f));		//выключаем обводку
			}
			pastObject = selectedObject;
		}
	}

	public Object GetObject()
	{
		p0 = shootingCamera.WorldPosition;
		p1 = shootingCamera.WorldPosition + shootingCamera.GetWorldDirection() * 5.0f;		//строим точку в пространстве прибавляя к текущему положения камеры вектор направления камеры домноженный на множитель
		WorldIntersection intersection = new WorldIntersection();
		Object obj = World.GetIntersection(p0, p1, mask, intersection);						//получаем объект, который пересек вектор, выпущенный из нашей камеры
		return obj;
	}

	private void SetOutline(Object gameObject, int isOutline, vec3 color)
	{
		for (var i = 0; i < gameObject.NumSurfaces; i++)			//включаем обводку для всех поверхностей объекта
		{
			gameObject.SetMaterialState("auxiliary", isOutline, i);
			gameObject.SetMaterialParameterFloat3("auxiliary_color", color, i);
		}
	}
}
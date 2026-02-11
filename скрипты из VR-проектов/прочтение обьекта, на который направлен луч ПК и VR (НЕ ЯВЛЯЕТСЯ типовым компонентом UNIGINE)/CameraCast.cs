using System.Collections;
using System.Collections.Generic;
using Unigine;

public class CameraCast {
	private static vec3 p0, p1;
	private static vec3 outlineColor = new vec3(1, 1, 1);
	private static bool drugged;
	private static InputVRController controller = Input.VRControllerRight;
	public static VRPlayer cameraplayer;
	public static GetterModels getterModels;


	public static Object drugObject;
	public static Object selectedTool;
	public static bool renderLine = true;
	public static bool movingAllObject = false;

	// protected override void OnReady()
	// {


	//     if (Input.VRControllerLeft == null) isVRConnected = false;
	//         else
	//     isVRConnected = true;
	// }
	public CameraCast(GetterModels getter)
	{
		getterModels = getter;
		//Node vrPlayerNode = World.GetNodeByName("vr_player");
		Node vrPlayerNode = getterModels.GetPlayer();

		if (vrPlayerNode != null)
		{
			cameraplayer = vrPlayerNode.GetComponent<VRPlayer>();
			if (cameraplayer == null)
				Log.Error("Component VRPlayer ne naiden v vr_player");
		}
	}

	public static void SetDrugged(bool drug)
	{
		drugged = drug;
		if (drug)
		{
			drugObject = GetObject();
		}
		else
		{
			drugObject = null;
		}
	}

	public static Object GetDrugObject()
	{
		return drugObject;
	}

	public static bool GetDrugged()
	{
		return drugged;
	}

	public static void SetOutlineDefault()
	{
		outlineColor = new vec3(1, 1, 1);
	}
	public static void SetOutlineColor(vec3 color)
	{
		outlineColor = color;
	}

	public static void ResetOutline(Object gameObject)
	{
		if (gameObject == null) return;

		for (var i = 0; i < gameObject.NumSurfaces; i++)
		{
			gameObject.SetMaterialState("auxiliary", 0, i);
			gameObject.SetMaterialParameterFloat3("auxiliary_color", outlineColor, i);
		}
	}

	public static void SetOutline(int enabled, Object gameObject)
	{
		if (gameObject == null || gameObject == selectedTool) return;

		for (var i = 0; i < gameObject.NumSurfaces; i++)
		{
			gameObject.SetMaterialState("auxiliary", enabled, i);
			gameObject.SetMaterialParameterFloat3("auxiliary_color", outlineColor, i);
		}
	}

	public static void SetOutlineWithColor(int enabled, Object gameObject, vec3 color)
	{
		for (var i = 0; i < gameObject.NumSurfaces; i++)
		{
			gameObject.SetMaterialState("auxiliary", enabled, i);
			gameObject.SetMaterialParameterFloat3("auxiliary_color", color, i);
		}
	}

	public static void SetOutlineTest(int enabled, Object gameObject)
	{
		for (var i = 0; i < gameObject.NumSurfaces; i++)
		{
			gameObject.SetMaterialState("auxiliary_btw", enabled, i);
			gameObject.SetMaterialParameterFloat3("auxiliary_color", outlineColor, i);
		}
	}

	// private void Update()
	// {
	// 	if (renderLine)
	// 	{
	// 		Visualizer.RenderLine3D(
	// 			Input.VRControllerLeft.GetWorldTransform().GetColumn3(3),
	// 			Input.VRControllerLeft.GetWorldTransform().GetColumn3(3) - Input.VRControllerLeft.GetWorldTransform().GetColumn3(2) * 1000,
	// 			vec4.WHITE
	// 		);

	// 		Visualizer.RenderLine3D(
	// 			Input.VRControllerRight.GetWorldTransform().GetColumn3(3),
	// 			Input.VRControllerRight.GetWorldTransform().GetColumn3(3) - Input.VRControllerRight.GetWorldTransform().GetColumn3(2) * 1000,
	// 			vec4.BLUE
	// 		);
	// 	}
	// }

	public static mat4 GetOldWorldTransform()
	{
		if (controller == null)
			return cameraplayer.Camera.OldWorldTransform;
		else
			return controller.GetWorldTransform();
	}

	public static mat4 GetIWorldTransform()
	{
		if (controller == null)
			return cameraplayer.Camera.IWorldTransform;
		else
			return controller.GetWorldTransform();
	}

	public static Object GetObject()
	{
		
		p0 = cameraplayer.Camera.WorldPosition;
		p1 = cameraplayer.Camera.WorldPosition + cameraplayer.Camera.GetWorldDirection() * 100;
		WorldIntersection intersection = new WorldIntersection();
		Object obj;
		if (controller != null)
		{
			vec3 controllerPosition = controller.GetWorldTransform().GetColumn3(3);
			vec3 controllerDirection = controller.GetWorldTransform().GetColumn3(2);
			obj = World.GetIntersection(controllerPosition, controllerPosition - controllerDirection * 1000, 1, intersection);
		}
		else
			obj = World.GetIntersection(p0, p1, 1, intersection);

		return obj;
	}
}
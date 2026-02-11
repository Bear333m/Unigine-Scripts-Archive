using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "4afaafa227ecc1c708f5044a82a2a2d03bb41cf3")]
public class SelectObject : Component
{

	private Object selectedObject;
	private Object pastObject;
	private vec3 outlineColor = new vec3(1, 1, 1);

	void Init()
	{
		pastObject = null;

		Visualizer.Enabled = true;
	}

	private void ResetPastObject()
	{
		if (pastObject == null || pastObject == selectedObject) return;
		if (pastObject.RootNode.Name != "static_content" && pastObject.RootNode.Name != "toolsNode")
		{
			CameraCast.SetOutlineTest(0, pastObject);
		}

		Log.Message($"Resetting past object: {pastObject.Name}\n");
		CameraCast.SetOutline(0, pastObject);
		Visualizer.Clear();
	}

	private void RenderInfo(string info)
	{
		if (selectedObject.RootNode.Name != "static_content" && selectedObject.RootNode.Name != "toolsNode")
		{
			CameraCast.SetOutlineTest(1, selectedObject);
		}

		CameraCast.SetOutline(1, selectedObject);
		Log.Message($"Setting object: {selectedObject.Name}\n");
		// Visualizer.RenderObjectSurfaceBoundBox(selectedObject, 0, vec4.BLUE, 0.05f);
	}

	private void Update()
	{
		if (Input.VRControllerRight != null)
		{
			if (CameraCast.renderLine)
			{
				// Log.Message(CameraCast.renderLine + "\n");
				Visualizer.RenderLine3D(
					Input.VRControllerLeft.GetWorldTransform().GetColumn3(3),
					Input.VRControllerLeft.GetWorldTransform().GetColumn3(3) - Input.VRControllerLeft.GetWorldTransform().GetColumn3(2) * 1000,
					vec4.WHITE
				);

				Visualizer.RenderLine3D(
					Input.VRControllerRight.GetWorldTransform().GetColumn3(3),
					Input.VRControllerRight.GetWorldTransform().GetColumn3(3) - Input.VRControllerRight.GetWorldTransform().GetColumn3(2) * 1000,
					vec4.BLUE
				);
			}
		}

		if (CameraCast.GetDrugged()) return;

		selectedObject = CameraCast.GetObject();
		if (selectedObject == null) return;

		if (pastObject != selectedObject)
		{
			ResetPastObject();
			pastObject = selectedObject;
		}
		else return;

		var info = $"{selectedObject.Name}";

		if (selectedObject.RootNode.Name == "static_content")
		{
			return;
		}

		RenderInfo(info);
	}
}
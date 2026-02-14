// Unigine C# 2.20.0.1
// from Art Samples — cross section
#region Math Variables
#if UNIGINE_DOUBLE
using Scalar = System.Double;
using Vec2 = Unigine.dvec2;
using Vec3 = Unigine.dvec3;
using Vec4 = Unigine.dvec4;
using Mat4 = Unigine.dmat4;
#else
using Scalar = System.Single;
using Vec2 = Unigine.vec2;
using Vec3 = Unigine.vec3;
using Vec4 = Unigine.vec4;
using Mat4 = Unigine.mat4;
using WorldBoundBox = Unigine.BoundBox;
using WorldBoundSphere = Unigine.BoundSphere;
using WorldBoundFrustum = Unigine.BoundFrustum;
#endif
#endregion

using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters;
using Unigine;

[Component(PropertyGuid = "4155b29b7641da2bf0bb9affb5eb9ec77cdc6126")]
public class CrossSectionMaterialManipulator : Component
{
	[ShowInEditor]
	private Node cross_section_plane = null;
	[ShowInEditor]
	private List<Node> cross_sected_nodes = new List<Node>();

	[ShowInEditor]
	private string material_parameter_position = "plane_position";

	[ShowInEditor]
	private string material_parameter_normal = "plane_normal";

	[ShowInEditor]
	private string material_state_cap_holes = "cap_holes";

	private WidgetVBox mainVBox = null;
	private WidgetSlider posXSlider = null;
	private WidgetLabel posXLabel = null;
	private WidgetSlider posYSlider = null;
	private WidgetLabel posYLabel = null;
	private WidgetSlider posZSlider = null;
	private WidgetLabel posZLabel = null;
	private WidgetSlider rotXSlider = null;
	private WidgetLabel rotXLabel = null;
	private WidgetSlider rotYSlider = null;
	private WidgetLabel rotYLabel = null;
	private WidgetCheckBox fillCheckBox = null;

	private const float min_pos_x = -5.0f;
	private const float min_pos_y = -5.0f;
	private const float min_pos_z = -1.0f;
	private const float max_pos_x = 5.0f;
	private const float max_pos_y = 5.0f;
	private const float max_pos_z = 5.0f;

	private const float min_rot_x = -179.0f;
	private const float min_rot_y = -179.0f;
	private const float max_rot_x = 179.0f;
	private const float max_rot_y = 179.0f;

	private void Init()
	{
		mainVBox = new WidgetVBox(WindowManager.MainWindow.SelfGui);
		mainVBox.SetPosition(10, 10);
		WindowManager.MainWindow.AddChild(mainVBox, Gui.ALIGN_OVERLAP);

		// position

		WidgetGridBox grid = new WidgetGridBox(3, 4, 4);
		mainVBox.AddChild(grid);

		WidgetLabel label = new WidgetLabel("Position:") { FontOutline = 1 };
		grid.AddChild(new WidgetLabel());
		grid.AddChild(label, Gui.ALIGN_CENTER);
		grid.AddChild(new WidgetLabel());

		label = new WidgetLabel("X:") { FontOutline = 1 };
		grid.AddChild(label);

		posXSlider = new WidgetSlider(0, 10000) { Width = 200 };
		grid.AddChild(posXSlider);

		posXLabel = new WidgetLabel("0") { FontOutline = 1, Width = 75 };
		grid.AddChild(posXLabel);

		label = new WidgetLabel("Y:") { FontOutline = 1 };
		grid.AddChild(label);

		posYSlider = new WidgetSlider(0, 10000) { Width = 200 };
		grid.AddChild(posYSlider);

		posYLabel = new WidgetLabel("0") { FontOutline = 1, Width = 75 };
		grid.AddChild(posYLabel);

		label = new WidgetLabel("Z:") { FontOutline = 1 };
		grid.AddChild(label);

		posZSlider = new WidgetSlider(0, 10000) { Width = 200 };
		grid.AddChild(posZSlider);

		posZLabel = new WidgetLabel("0") { FontOutline = 1, Width = 75 };
		grid.AddChild(posZLabel);


		posXSlider.EventChanged.Connect(OnPosChanged);
		posYSlider.EventChanged.Connect(OnPosChanged);
		posZSlider.EventChanged.Connect(OnPosChanged);

		// rotatiton
		label = new WidgetLabel("Rotation:") { FontOutline = 1 };
		grid.AddChild(new WidgetLabel());
		grid.AddChild(label, Gui.ALIGN_CENTER);
		grid.AddChild(new WidgetLabel());

		label = new WidgetLabel("X:") { FontOutline = 1 };
		grid.AddChild(label);

		rotXSlider = new WidgetSlider(0, 10000) { Width = 200 };
		grid.AddChild(rotXSlider);

		rotXLabel = new WidgetLabel("0") { FontOutline = 1, Width = 75 };
		grid.AddChild(rotXLabel);

		label = new WidgetLabel("Y:") { FontOutline = 1 };
		grid.AddChild(label);

		rotYSlider = new WidgetSlider(0, 10000) { Width = 200 };
		grid.AddChild(rotYSlider);

		rotYLabel = new WidgetLabel("0") { FontOutline = 1, Width = 75 };
		grid.AddChild(rotYLabel);

		vec3 pos = new vec3(cross_section_plane.WorldPosition);
		vec3 rot = MathLib.DecomposeRotationXYZ(new mat3(cross_section_plane.WorldTransform));

		posXSlider.Value = (int)(MathLib.Clamp(pos.x - min_pos_x, 0, max_pos_x - min_pos_x) * 10000.0f / (max_pos_x - min_pos_x));
		posYSlider.Value = (int)(MathLib.Clamp(pos.y - min_pos_y, 0, max_pos_y - min_pos_y) * 10000.0f / (max_pos_y - min_pos_y));
		posZSlider.Value = (int)(MathLib.Clamp(pos.z - min_pos_z, 0, max_pos_z - min_pos_z) * 10000.0f / (max_pos_z - min_pos_z));

		rotXSlider.Value = (int)(MathLib.Clamp(rot.x - min_rot_x, 0, max_rot_x - min_rot_x) * 10000.0f / (max_rot_x - min_rot_x));
		rotYSlider.Value = (int)(MathLib.Clamp(rot.y - min_rot_y, 0, max_rot_y - min_rot_y) * 10000.0f / (max_rot_y - min_rot_y));

		rotXSlider.EventChanged.Connect(OnRotChanged);
		rotYSlider.EventChanged.Connect(OnRotChanged);


		fillCheckBox = new WidgetCheckBox() { Checked = true, Text = "Cap Holes" };
		grid.AddChild(fillCheckBox);
		fillCheckBox.EventChanged.Connect(OnFillChanged);

		UpdateLabels();
	}

	private void Update()
	{
		foreach (var node in cross_sected_nodes)
		{
			Object o = node as Object;
			if (o != null)
			{
				for (int i = 0; i < o.NumSurfaces; i++)
				{
					o.SetMaterialParameterFloat3(material_parameter_position, new vec3(cross_section_plane.WorldPosition), i);
					o.SetMaterialParameterFloat3(material_parameter_normal, cross_section_plane.GetWorldDirection(MathLib.AXIS.NZ), i);
				}

			}
		}
	}

	private void Shutdown()
	{
		mainVBox.DeleteLater();
	}
	private void OnFillChanged()
	{
		bool fill = fillCheckBox.Checked;
		foreach (var node in cross_sected_nodes)
		{
			Object o = node as Object;
			if (o != null)
			{
				for (int i = 0; i < o.NumSurfaces; i++)
				{
					o.SetMaterialState(material_state_cap_holes, fill ? 1 : 0, i);
				}
			}
		}
	}
	private void OnPosChanged()
	{
		float x = posXSlider.Value * 0.0001f * (max_pos_x - min_pos_x) + min_pos_x;
		float y = posYSlider.Value * 0.0001f * (max_pos_y - min_pos_y) + min_pos_y;
		float z = posZSlider.Value * 0.0001f * (max_pos_z - min_pos_z) + min_pos_z;
		cross_section_plane.WorldPosition = new vec3(x, y, z);

		UpdateLabels();
	}

	private void OnRotChanged()
	{
		float x = rotXSlider.Value * 0.0001f * (max_rot_x - min_rot_x) + min_rot_x;
		float y = rotYSlider.Value * 0.0001f * (max_rot_y - min_rot_y) + min_rot_y;
		cross_section_plane.SetWorldRotation(new quat(x, y, 0));

		UpdateLabels();
	}

	private void UpdateLabels()
	{
		Vec3 pos = cross_section_plane.WorldPosition;
		vec3 rot = MathLib.DecomposeRotationXYZ(new mat3(cross_section_plane.WorldTransform));

		posXLabel.Text = pos.x.ToString("0.00");
		posYLabel.Text = pos.y.ToString("0.00");
		posZLabel.Text = pos.z.ToString("0.00");

		rotXLabel.Text = rot.x.ToString("0.00");
		rotYLabel.Text = rot.y.ToString("0.00");
	}
}

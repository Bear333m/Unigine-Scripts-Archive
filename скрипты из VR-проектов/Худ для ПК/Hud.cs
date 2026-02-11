using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "e4e9f02eefeac5fc18d0b2fbea493197e48c2644")]
public class Hud : Component
{
	public AssetLink crosshairImage = null;
	public int crosshairSize = 16;

	private WidgetSprite sprite = null;
	private Gui screenGui = null;
	private ivec2 prev_size = new ivec2(0, 0);

	private void Init()
	{
		screenGui = Gui.GetCurrent();

		sprite = new WidgetSprite(screenGui, crosshairImage.AbsolutePath);
		sprite.Width = crosshairSize;
		sprite.Height = crosshairSize;

		screenGui.AddChild(sprite, Gui.ALIGN_CENTER | Gui.ALIGN_OVERLAP);

		sprite.Lifetime = Widget.LIFETIME.WORLD;
	}

	private void Update()
	{
		ivec2 new_size = screenGui.Size;
		if (prev_size != new_size)
		{
			screenGui.RemoveChild(sprite);
			screenGui.AddChild(sprite, Gui.ALIGN_CENTER | Gui.ALIGN_OVERLAP);
		}
		prev_size = new_size;
	}
}


// Unigine C# 2.20.0.1
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "5e9a9ff1ab96632d22a50cb3db81cf91dc416586")]
public class ObjectProperties : Component
{
	[ShowInEditor]
	public bool isInteractable = false;		// Переменная для проверки, нужно ли обводить модель при наведении
	
	void Init()
	{
		// write here code to be called on component initialization
	}
	
	void Update()
	{
		// write here code to be called before updating each render frame
	}
}

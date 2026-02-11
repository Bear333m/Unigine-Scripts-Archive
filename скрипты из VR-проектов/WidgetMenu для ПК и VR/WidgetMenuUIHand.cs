using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "06955d6ec70bcf22a58e9d195db513f1bfa41661")]
public class WidgetMenuUIHand : Component
{
	[ShowInEditor]
	[ParameterFile(Filter = ".ui")]
	private string UiPath;

	[ShowInEditor]
	private DrugObject drugObject = null;

	[ShowInEditor]
	private ObjectGui objectGui;

	private UserInterface ui = null;

	private Gui gui = null;
	private WidgetVBox vbox = null;
	private WidgetVBox detailInfoVBox;
	private WidgetButton initialButton, rayButton,  startAnimationButton, stopAnimationButton, reverseAnimationButton;
	public WidgetButton movingAllButton;
	private WidgetLabel detailNameLabel, detailInfoLabel;
	private AnimationSteps animationSteps;

	private bool renderLine = true;
	public bool movingAllObject = false;

	private PCWidgetMenuUIHand pCmenuUIHand;

	private DescriptionDetail descriptionDetail;

	private void Init()
	{
		pCmenuUIHand = FindComponentInWorld<PCWidgetMenuUIHand>();
		animationSteps = FindComponentInWorld<AnimationSteps>();
		Log.MessageLine("MENU INIT");
		objectGui.MouseMode = ObjectGui.MOUSE_VIRTUAL;
		gui = objectGui.GetGui();
		ui = new UserInterface(gui, UiPath);

		vbox = ui.GetWidget(ui.FindWidget("vbox")) as WidgetVBox;

		initialButton = ui.GetWidget(ui.FindWidget("initialButton")) as WidgetButton;
		initialButton.EventClicked.Connect(InitialButtonClicked);

		rayButton = ui.GetWidget(ui.FindWidget("rayButton")) as WidgetButton;
		rayButton.EventClicked.Connect(RayButtonClicked);

		movingAllButton = ui.GetWidget(ui.FindWidget("movingAll")) as WidgetButton;
		movingAllButton.EventClicked.Connect(movingAllButtonClicked);

		startAnimationButton = ui.GetWidget(ui.FindWidget("startAnimation")) as WidgetButton;
		startAnimationButton.EventClicked.Connect(animationSteps.Play);

		stopAnimationButton = ui.GetWidget(ui.FindWidget("stopAnimation")) as WidgetButton;
		stopAnimationButton.EventClicked.Connect(animationSteps.Stop);

		reverseAnimationButton = ui.GetWidget(ui.FindWidget("reverseAnimation")) as WidgetButton;
		reverseAnimationButton.EventClicked.Connect(animationSteps.PlayReverse);

		detailInfoVBox = ui.GetWidget(ui.FindWidget("detailInfo")) as WidgetVBox;
		detailNameLabel = ui.GetWidget(ui.FindWidget("detailNameLabel")) as WidgetLabel;
		detailInfoLabel = ui.GetWidget(ui.FindWidget("detailInfoLabel")) as WidgetLabel;
		if (detailNameLabel == null)
		{
			Log.Message("Failed to find detailNameLabel in UI\n");
		}
		else
		{
			Log.Message($"detailNameLabel successfully found: {detailNameLabel.Text}\n");
		}

		gui.AddChild(vbox, Gui.ALIGN_EXPAND);

	}

	public void SetDetailName(string name)
	{
		if (detailNameLabel != null && name != null)
		{
			detailNameLabel.Text = name;
		}
		else
		{
			Log.Message($"detailNameLabel is null or name is null. Label: {detailNameLabel != null}, Name: {name}\n");
		}
	}
	
	public void SetDetailInformation(Node nodee)
	{
		if (detailInfoLabel != null && nodee != null)
		{
			descriptionDetail = nodee.GetComponent<DescriptionDetail>();
			if (descriptionDetail!=null)
				detailInfoLabel.Text = descriptionDetail.AddDescription;
			else
				detailInfoLabel.Text = "";
		}
		else
		{
			Log.Message($"detailInfoLabel is null or name is null. Label: {detailInfoLabel != null}, nodee: {nodee}\n");
		}
	}
	
	private void InitialButtonClicked()
	{
		drugObject.AllFullInitialTransform();
	}

	private void RayButtonClicked()
	{
		renderLine = !renderLine;
		if (renderLine)
		{
			CameraCast.renderLine = true;
			rayButton.Text = "Скрыть лучи";
		}
		else
		{
			CameraCast.renderLine = false;
			rayButton.Text = "Показать лучи";
		}
	}

	private void movingAllButtonClicked()
	{
		movingAllObject = !movingAllObject;
		pCmenuUIHand.movingAllObject = movingAllObject;
		if (movingAllObject)
		{
			movingAllButton.Text = "Остановить перемещение всей модели";
			pCmenuUIHand.movingAllButton.Text = "Остановить перемещение всей модели";
			CameraCast.movingAllObject = true;
		}
		else
		{
			movingAllButton.Text = "Переместить всю модель";
			pCmenuUIHand.movingAllButton.Text = "Переместить всю модель";
			CameraCast.movingAllObject = false;
		}
	}



}
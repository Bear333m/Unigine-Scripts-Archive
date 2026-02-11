using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "884bfef6cdbc503ee211ce51f69f1876ce50850c")]
public class PCWidgetMenuUIHand : Component
{
	[ShowInEditor]
	[ParameterFile(Filter = ".ui")]
	private string UiPath;

	[ShowInEditor]
	private DrugObject drugObject = null;

	private UserInterface ui = null;
	private ObjectGui objectGui = null;
	private Gui gui = null;
	private WidgetVBox vbox = null;
	//private WidgetVBox detailInfoVBox;
	private WidgetButton initialButton, rayButton,  startAnimationButton, stopAnimationButton, reverseAnimationButton;
	public WidgetButton movingAllButton;
	private WidgetLabel detailNameLabel, detailInfoLabel;
	private AnimationSteps animationSteps;
	private WidgetEditText detailInfoText;

	private bool renderLine = true;
	public bool movingAllObject = false;
	private WidgetMenuUIHand menuUIHand;

	private DescriptionDetail descriptionDetail;

	private void Init()
	{
		menuUIHand = FindComponentInWorld<WidgetMenuUIHand>();
		animationSteps = FindComponentInWorld<AnimationSteps>();
		Log.MessageLine("MENU INIT");
		objectGui = node as ObjectGui;
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

		//detailInfoVBox = ui.GetWidget(ui.FindWidget("detailInfo")) as WidgetVBox;
		detailNameLabel = ui.GetWidget(ui.FindWidget("detailNameLabel")) as WidgetLabel;
		detailInfoLabel = ui.GetWidget(ui.FindWidget("detailInfoLabel")) as WidgetLabel;
		// detailInfoText = ui.GetWidget(ui.FindWidget("detailInfoText")) as WidgetEditText;
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

	public void SetDetailName(string name)//ЗДЕСЬ ИМЯ, А НАДО ЧТОБЫ НИЖЕ ТЕКСТ ОПИСАНИЕ ДЕТАЛИ
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
			{
				detailInfoLabel.Text = descriptionDetail.AddDescription;
				Log.MessageLine(detailInfoLabel.Width);
				//вот тут if detailInfoLabel.Width >objectgui.screenwidth, то разбиваем hereee на несколько строк до тех пор, пока есть строки больше чем objectgui.screenwidth, и detailInfoLabel.Text = str1 + \n + str2 + \n
			}

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
		if(menuUIHand!=null)
			menuUIHand.movingAllObject = movingAllObject;
		
		if (movingAllObject)
		{
			movingAllButton.Text = "Остановить перемещение всей модели";
			if(menuUIHand!=null)
				if(menuUIHand.movingAllButton!=null)
					menuUIHand.movingAllButton.Text = "Остановить перемещение всей модели";
			CameraCast.movingAllObject = true;
		}
		else
		{
			movingAllButton.Text = "Переместить всю модель";
			if(menuUIHand!=null)
				if(menuUIHand.movingAllButton!=null)
					menuUIHand.movingAllButton.Text = "Переместить всю модель";
			CameraCast.movingAllObject = false;
		}
	}
}
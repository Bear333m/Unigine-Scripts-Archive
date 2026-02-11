// Unigine C# 2.20.0.1
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "6eca770c1093c8bd6b206adc6430c19e3395610e")]
public class MainMenu : Component
{
	[ShowInEditor]
	[ParameterFile(Filter = ".ui")]
	private string filePath = "null";

	//[ShowInEditor]
	//private Testing test;

	[ShowInEditor]
	[ParameterSlider(Min = 0.1f, Max = 10.0f)]
	private float MenuScale = 1;

	private bool isMenuOpened = true;
	private int isFullscreen = 0;

	private UserInterface ui;
	public Widget pMainMenu;
	private WidgetSprite pwButtonWorldMenu1, pwButtonWorldMenu2, pwButtonTesting, pwButtonFullscreen,  pwButtonExit, pwBachground;
	private WidgetDialogMessage MesageBox = new WidgetDialogMessage(Gui.GetCurrent());
	List<Widget> ListOfWidgets = new List<Widget>();


	void Init()
	{
		ui = new UserInterface(Gui.GetCurrent(), filePath);     //получаем интерфейс из пути к файлу, хранящегося в переменной filePath
		ui.Lifetime = Widget.LIFETIME.WORLD;
		System.Console.WriteLine(ui.FindWidget("mainMenu"));
		pMainMenu = ui.GetWidget(ui.FindWidget("mainMenu"));    //получаем виджет из этого файла с названием mainMenu
		pMainMenu.Lifetime = Widget.LIFETIME.WORLD;

		pwButtonWorldMenu1 = (WidgetSprite)ui.GetWidget(ui.FindWidget("BtnWorldMenu1"));			//получаем объект кнопки
		pwButtonWorldMenu1.EventEnter.Connect(ChangeCoverButtonEnter, pwButtonWorldMenu1);		//привязываем обработчик при наведении курсора на кнопку
		pwButtonWorldMenu1.EventLeave.Connect(ChangeCoverButtonLeave, pwButtonWorldMenu1);		//привязываем обработчик при выводе курсора с кнопки
		pwButtonWorldMenu1.EventClicked.Connect(LoadWorldMenu1);									//привязываем обработчик при нажатии на кнопку

		pwButtonWorldMenu2 = (WidgetSprite)ui.GetWidget(ui.FindWidget("BtnWorldMenu2"));
		pwButtonWorldMenu2.EventEnter.Connect(ChangeCoverButtonEnter, pwButtonWorldMenu2);
		pwButtonWorldMenu2.EventLeave.Connect(ChangeCoverButtonLeave, pwButtonWorldMenu2);
		pwButtonWorldMenu2.EventClicked.Connect(LoadWorldMenu2);

		pwButtonTesting = (WidgetSprite)ui.GetWidget(ui.FindWidget("BtnTest"));
		pwButtonTesting.EventEnter.Connect(ChangeCoverButtonEnter, pwButtonTesting);
		pwButtonTesting.EventLeave.Connect(ChangeCoverButtonLeave, pwButtonTesting);
		pwButtonTesting.EventClicked.Connect(StartTesting);

		pwButtonFullscreen = (WidgetSprite)ui.GetWidget(ui.FindWidget("BtnFullscreen"));
		pwButtonFullscreen.EventEnter.Connect(ChangeCoverButtonEnter, pwButtonFullscreen);
		pwButtonFullscreen.EventLeave.Connect(ChangeCoverButtonLeave, pwButtonFullscreen);
		pwButtonFullscreen.EventClicked.Connect(ChangeFullscreen);

		pwButtonExit = (WidgetSprite)ui.GetWidget(ui.FindWidget("BtnExit"));
		pwButtonExit.EventEnter.Connect(ChangeCoverButtonEnter, pwButtonExit);
		pwButtonExit.EventLeave.Connect(ChangeCoverButtonLeave, pwButtonExit);
		pwButtonExit.EventClicked.Connect(ExitTheProgramm);

		pwBachground = (WidgetSprite)ui.GetWidget(ui.FindWidget("Background"));
		Gui.GetCurrent().AddChild(pMainMenu, Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER);     //добавляем полученное из файла главное меню в интерфейс

		EngineWindowViewport ewv = WindowManager.MainWindow;
		ewv.EventResized.Connect(MoveMenuToCenter);           //привязываем обработчик изменения размера окна приложения

		MesageBox.MessageText = "Заглушка для теста";
		MesageBox.Hidden = true;
		MesageBox.GetCancelButton().EventClicked.Connect(ClickMessageBox);
		MesageBox.GetOkButton().EventClicked.Connect(ClickMessageBox);
		Gui.GetCurrent().AddChild(MesageBox, Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER);

		ListOfWidgets.Add(pwButtonWorldMenu1);
		ListOfWidgets.Add(pwButtonWorldMenu2);
		ListOfWidgets.Add(pwButtonTesting);
		ListOfWidgets.Add(pwButtonFullscreen);
		ListOfWidgets.Add(pwButtonExit);
		ListOfWidgets.Add(pwBachground);
		ScaleMenu();
	}


	void Update()
	{
		if (Input.IsKeyDown(Input.KEY.ESC))
		{
			isMenuOpened = !isMenuOpened;
			pMainMenu.Hidden = !pMainMenu.Hidden;
			ToggleCursorMode();
			
		}
		else if (isMenuOpened)			//пока меню открыто, постоянно отключаем захват мышки (для поворота камеры) и включаем курсор, 
		{								//так как иначе при нажатии в пустое место курсор исчезает
			ToggleCursorOn(true);
		}
	}


	private void LoadWorldMenu1()
	{
		EngineWindowViewport ewv = WindowManager.MainWindow;
		ewv.EventResized.Disconnect(MoveMenuToCenter);			//отвязываем обработчик, чтобы не вылетало при изменении окна после перещения в другой мир

		HideMenu();
		Console.Run("world_load menu1");       			 //сценарий Работы

	}

	private void LoadWorldMenu2()
	{
		EngineWindowViewport ewv = WindowManager.MainWindow;
		ewv.EventResized.Disconnect(MoveMenuToCenter);			//отвязываем обработчик, чтобы не вылетало при изменении окна после перещения в другой мир

		HideMenu();
		Console.Run("world_load menu2");
	}

	private void StartTesting()
	{
		isMenuOpened = false;
		pMainMenu.Hidden = true;
		//test.StartTesting();
		MesageBox.Hidden = false;
	}

	private void ChangeFullscreen()
    {
		if (isFullscreen == 0) isFullscreen = 1;
		else isFullscreen = 0;
        Console.Run("main_window_fullscreen " + isFullscreen);
    }

	private void ExitTheProgramm()
	{
		Engine.Quit();
	}



	private void HideMenu()
	{
		isMenuOpened = false;
		pMainMenu.Hidden = true;
	}

	public void ShowMenu()
	{
		isMenuOpened = true;
		pMainMenu.Hidden = false;
	}

	private void ChangeCoverButtonEnter(WidgetSprite widget)			//сдвигаем виджет вниз, так как у нас внизу спрайта кнопки находится наведенное состояние
	{
		widget.SetLayerTexCoord(0, new vec4(0.0f, 0.5f, 1.0f, 1f));
	}

	private void ChangeCoverButtonLeave(WidgetSprite widget)			//сдвигаем виджет вверх, так как у нас вверху спрайта кнопки находится ненаведенное состояние
	{
		widget.SetLayerTexCoord(0, new vec4(0.0f, 0f, 1.0f, 0.5f));
	}
	
	
	private void MoveMenuToCenter()
	{
		var mainWindowSize = WindowManager.MainWindow.Size;
		var px = pwBachground.Width;
		var py = pwBachground.Height;
		pMainMenu.SetPosition(mainWindowSize.x / 2 - px / 2, mainWindowSize.y / 2 - py / 2);	//задаем новые координаты левого верхнего угла меню, чтобы сдвинуть его в центр
	}

	private void ClickMessageBox()
	{
		MesageBox.Hidden = true;
		ToggleCursorOn(false);
	}

	private void ScaleMenu()
	{
		foreach (Widget widget in ListOfWidgets) {
			Log.Message(string.Format("Размеры до: {0} и {1}", widget.Width, widget.Height));
			widget.Height = (int) (widget.Height * MenuScale);
			widget.Width = (int) (widget.Width * MenuScale);
			widget.PositionX = (int) (widget.PositionX * MenuScale);
			widget.PositionY = (int) (widget.PositionY * MenuScale);
		}
		MoveMenuToCenter();
	}

	private void ToggleCursorMode()
    {
        Input.MouseGrab = !Input.MouseGrab;			//меняем параметр захвата мышки на противоположный
		Gui gui;
		gui = Gui.GetCurrent();
		gui.MouseShow = !gui.MouseShow;				//включаем или отключаем отображение курсора
    }

	private void ToggleCursorOn(bool isCursorOn)
    {
        Input.MouseGrab = !isCursorOn;
		Gui gui;
		gui = Gui.GetCurrent();
		gui.MouseShow = isCursorOn;
    }
}

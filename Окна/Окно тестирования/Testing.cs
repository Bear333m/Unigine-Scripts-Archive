// Unigine C# 2.20.0.1
using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;
using System.Linq;
using System.ComponentModel.DataAnnotations;		//для использования SequenceEqual

[Component(PropertyGuid = "bef0e6702a06e36a8e023ef9e558969acf19c83f")]
public class Testing : Component
{
	Json json = new(), jsonQuestions;

	[ShowInEditor]
	//[Parameter(Title = "Json File")]
	[ParameterFile(Filter = ".json")]
	private string jsonFile = "";       //в переменной хранится оносительный путь к файлу


	struct Question
	{
		public string question;
		public List<string> answers;
		public List<int> correctAnswers;
		public string answerType;
	}
	private List<Question> questions = new List<Question>();
	[ShowInEditor]
	[ParameterSlider(Title = "Number of Questions", Min = 1, Max = 22)]
	private int numberOfQuestions = 5;
	[ShowInEditor]
	[Parameter(Title = "Mix the Questions")]
	private bool mixTheQuestions = true;


	private WidgetVBox VBox, VBoxFinal;
	private WidgetSprite background, backgroundFinal;
	private WidgetLabel questionLabel, numberLabel, resultLabel;
	private WidgetButton btn1, btn2, btn3, btn4, nextButton, endButton, closeButton;

	private List<WidgetButton> buttons = new List<WidgetButton>();


	int correctAnswersCount = 0, currentQuestionIndex = 0;
	bool isTestGoing = false;
	List<int> choosenAnswers = new List<int>();

	private Widget TestBox, TestBoxResults;


	void Init()
	{
		LoadQuestions();		//загружаем текст вопросов с ответами из файла
		CreateTestingMenu();	//создаем меню
		CreateTestingFinal();	//создаем окошко с результатами тестирования

		VBox.Hidden = true;
		VBoxFinal.Hidden = true;

		TestBox = VBox;
		TestBoxResults = VBoxFinal;
	}

	void Update()
	{
		if (Input.IsKeyDown(Input.KEY.T))		//для проверок
		{
			StartTesting();
		}
		if (isTestGoing)			//пока меню открыто, при нажатии в пустое будем отключать захват мышки (для поворота камеры) и будем включать курсор, 
		{							//так как иначе при нажатии в пустое место они исчезают
			ToggleCursorOn(true);
		}
	}


	private void LoadQuestions()
	{
		json.Load(jsonFile);     					   //загружаем файл по пути к файлу из переменной jsonFile
		jsonQuestions = json.GetChild("questions");		//получаем объект по ключу questions из файла .json

		for (int i = 0; i < jsonQuestions.GetNumChildren(); i++)
		{
			Json jsonQuestion = jsonQuestions.GetChild(i);          //получаем объект по ключу, который соответствует номеру вопроса, внутри тела объекта questions
			string question = jsonQuestion.Read("question");        //считываем текст из поля question
			string answerType = jsonQuestion.Read("answerType");    //считываем тип ответа из поля answerType
			List<int> correctAnswers = new List<int>();
			jsonQuestion.Read("rightAnswer", correctAnswers);		//считываем номера правильных ответов из поля rightAnswer
			
			List<string> answers = new List<string>();
			Json jsonAnswers = jsonQuestion.GetChild("answers");	//получаем объект по ключу answers из объекта номера вопроса
			answers.Add(jsonAnswers.Read("1"));						//добавляем в массив строк строку из поля с текстом первого ответа
			answers.Add(jsonAnswers.Read("2"));
			answers.Add(jsonAnswers.Read("3"));
			answers.Add(jsonAnswers.Read("4"));

			questions.Add(new Question      //добавляем в список экземпляр структуры Question, с текстом вопроса, списком срок ответов, номером правильного ответа
			{
				question = question,
				answers = answers,
				correctAnswers = correctAnswers,
				answerType = answerType
			});
		}
	}

	private void CreateTestingMenu()
	{
		VBox = new WidgetVBox();
		VBox.Width = 900;
		VBox.Height = 600;

		background = new WidgetSprite();
		background.Width = 900;
		background.Height = 600;
		background.Texture = "data/all scripts/test/gui/background_test.png";
		background.SetPosition(0, 0);

		questionLabel = new WidgetLabel();
		questionLabel.Height = 120;
		questionLabel.Width = 800;
		questionLabel.Text = "Вопрос";
		questionLabel.FontSize = 30;
		questionLabel.SetPosition(50, 50);
		questionLabel.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		questionLabel.FontWrap = 1;

		btn1 = new WidgetButton();
		btn1.Width = 380;
		btn1.Height = 140;
		btn1.Text = "Ответ 1";
		btn1.FontSize = 20;
		btn1.FontWrap = 1;
		btn1.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		btn1.SetPosition(50, 185);
		btn1.ButtonColor = new vec4(0.62f, 0.65f, 0.85f, 1.0f);
		btn1.EventClicked.Connect(ClickAnswer, 1);			//привязываем обработчик нажатия на кнопку для проверки правильности ответа

		btn2 = new WidgetButton();
		btn2.Width = 380;
		btn2.Height = 140;
		btn2.Text = "Ответ 2";
		btn2.FontSize = 20;
		btn2.FontWrap = 1;
		btn2.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		btn2.SetPosition(470, 185);
		btn2.ButtonColor = new vec4(0.62f, 0.65f, 0.85f, 1.0f);
		btn2.EventClicked.Connect(ClickAnswer, 2);			//второй аргумент передается в качестве параметра функции-обработчику при нажатии


		btn3 = new WidgetButton();
		btn3.Width = 380;
		btn3.Height = 140;
		btn3.Text = "Ответ 3";
		btn3.FontSize = 20;
		btn3.FontWrap = 1;
		btn3.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		btn3.SetPosition(50, 350);
		btn3.ButtonColor = new vec4(0.62f, 0.65f, 0.85f, 1.0f);
		btn3.EventClicked.Connect(ClickAnswer, 3);

		btn4 = new WidgetButton();
		btn4.Width = 380;
		btn4.Height = 140;
		btn4.Text = "Ответ 4";
		btn4.FontSize = 20;
		btn4.FontWrap = 1;
		btn4.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		btn4.SetPosition(470, 350);
		btn4.ButtonColor = new vec4(0.62f, 0.65f, 0.85f, 1.0f);
		btn4.EventClicked.Connect(ClickAnswer, 4);

		numberLabel = new WidgetLabel();
		numberLabel.Width = 70;
		numberLabel.Height = 40;
		numberLabel.Text = "1/5";
		numberLabel.FontSize = 30;
		numberLabel.SetPosition(415, 527);
		numberLabel.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);

		nextButton = new WidgetButton();		//Кнопка для проверки и перехода к следующему вопросу
		nextButton.Width = 220;
		nextButton.Height = 70;
		nextButton.Text = "Далее";
		nextButton.FontSize = 25;
		nextButton.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		nextButton.SetPosition(630, 511);
		nextButton.ButtonColor = new vec4(0.62f, 0.65f, 0.85f, 1.0f);
		nextButton.EventClicked.Connect(ClickNextButton);

		closeButton = new WidgetButton();
		closeButton.Width = 220;
		closeButton.Height = 70;
		closeButton.Text = "Закрыть";
		closeButton.FontSize = 25;
		closeButton.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		closeButton.SetPosition(50, 511);
		closeButton.ButtonColor = new vec4(0.62f, 0.65f, 0.85f, 1.0f);
		closeButton.EventClicked.Connect(ClickCloseButton);


		VBox.AddChild(background, Gui.ALIGN_OVERLAP);
		VBox.AddChild(questionLabel, Gui.ALIGN_OVERLAP);
		VBox.AddChild(btn1, Gui.ALIGN_OVERLAP);
		VBox.AddChild(btn2, Gui.ALIGN_OVERLAP);
		VBox.AddChild(btn3, Gui.ALIGN_OVERLAP);
		VBox.AddChild(btn4, Gui.ALIGN_OVERLAP);
		VBox.AddChild(numberLabel, Gui.ALIGN_OVERLAP);
		VBox.AddChild(nextButton, Gui.ALIGN_OVERLAP);
		VBox.AddChild(closeButton, Gui.ALIGN_OVERLAP);

		WindowManager.MainWindow.AddChild(VBox, Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER);

		buttons.Add(btn1);		//добавляем в список кнопку
		buttons.Add(btn2);
		buttons.Add(btn3);
		buttons.Add(btn4);

		nextButton.Enabled = false;
	}


	private void CreateTestingFinal()
	{
		VBoxFinal = new WidgetVBox();
		VBoxFinal.Width = 450;
		VBoxFinal.Height = 250;

		backgroundFinal = new WidgetSprite();
		backgroundFinal.Width = 450;
		backgroundFinal.Height = 250;
		backgroundFinal.Texture = "data/all scripts/test/gui/background_test_results.png";
		backgroundFinal.SetPosition(0, 0);

		resultLabel = new WidgetLabel();
		resultLabel.Height = 120;
		resultLabel.Width = 350;
		resultLabel.Text = "Ваш результат: ";
		resultLabel.FontSize = 25;
		resultLabel.SetPosition(50, 50);
		resultLabel.TextAlign = Gui.ALIGN_CENTER;
		resultLabel.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		resultLabel.FontWrap = 1;

		endButton = new WidgetButton();
		endButton.Width = 220;
		endButton.Height = 70;
		endButton.Text = "Закрыть";
		endButton.FontSize = 25;
		endButton.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		endButton.SetPosition(115, 130);
		endButton.ButtonColor = new vec4(0.62f, 0.65f, 0.85f, 1.0f);
		endButton.EventClicked.Connect(ClickEndButton);

		VBoxFinal.AddChild(backgroundFinal, Gui.ALIGN_OVERLAP);
		VBoxFinal.AddChild(resultLabel, Gui.ALIGN_OVERLAP);
		VBoxFinal.AddChild(endButton, Gui.ALIGN_OVERLAP);

		WindowManager.MainWindow.AddChild(VBoxFinal, Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER);
	}



	public void StartTesting()
	{
		VBox.Hidden = false;
		correctAnswersCount = 0;
		currentQuestionIndex = 0;
		SetBaseButton();
		ShowQuestion();
		nextButton.Hidden = false;

		isTestGoing = true;
		EngineWindowViewport ewv = WindowManager.MainWindow;
		ewv.EventResized.Connect(MoveWindowToCenter);			//привязываем обработчик изменения размеров окна приложения

		nextButton.Text = "Проверить";
		nextButton.Enabled = false;
		if (mixTheQuestions == true) MixQuestions();		//перемешиваем вопросы
		MoveWindowToCenter();								//центрируем окно
	}


	private void SetBaseButton()
	{
		foreach (var button in buttons)		//включаем отображение для каждой кнопки, также задаем им цвет
		{
			button.Enabled = true;
			button.ButtonColor = new vec4(0.62f, 0.65f, 0.85f, 1.0f);
			button.FontColor = new vec4(1, 1, 1, 1);
		}
	}


	private void ShowQuestion()
	{
		choosenAnswers.Clear();		//очищаем список выбранных ответов

		Question question = questions[currentQuestionIndex];	//создаем экземпляр структуры Question и добавляем в нее currentQuestionIndex'овый вопрос из списка вопросов
		questionLabel.Text = question.question;					//записываем в текстовое поле текст текущего вопроса
		if (question.answerType == "single") questionLabel.Text += " Выберите один правильный ответ.";
		else if (question.answerType == "multiple") questionLabel.Text += " Выберите несколько правильных ответов.";
		btn1.Text = question.answers[0];						//записываем на кнопку текст первого ответа из currentQuestionIndex вопроса
		btn2.Text = question.answers[1];
		btn3.Text = question.answers[2];
		btn4.Text = question.answers[3];
		numberLabel.Text = (currentQuestionIndex + 1) + "/" + numberOfQuestions;	//записываем в текстовое поле номер текущего вопроса и суммарное количесво вопросов
	}


	private void ClickAnswer(int choosenButtonNumber)
	{
		int index = choosenButtonNumber - 1;		//делаем из номера кнопки индекс
		if (choosenAnswers.Contains(index))
		{
			choosenAnswers.Remove(index);
			buttons[index].ButtonColor = new vec4(0.62f, 0.65f, 0.85f, 1.0f);       //снимаем выбор с кнопки
		}
		else
		{
			choosenAnswers.Add(index);
			buttons[index].ButtonColor = new vec4(0.54f, 0.35f, 0.59f, 1.0f);       //меняем цвет кнопки при выборе
		}
		nextButton.Enabled = true;
	}
	
	private void ClickNextButton()
	{
		if (nextButton.Text == "Проверить" && currentQuestionIndex == numberOfQuestions - 1)		//Нажали "Проверить" на последнем вопросе, теперь осталось только завершить
		{
			nextButton.Text = "Завершить";
			CheckAnswer();
			ChangeButtonsEnabled(false);
		}
		else if (currentQuestionIndex == numberOfQuestions - 1) {      // Нажали "Завершить", значит вопросы закончились и надо вывести результаты
			ShowResults();
		}
		else if (nextButton.Text == "Проверить")		//Нажали "Проверить", надо проверить выбранные варианты
		{
			nextButton.Text = "Далее";
			CheckAnswer();
			ChangeButtonsEnabled(false);
		}
		else										//Нажали "Далее", надо перейти к следующему вопросу
		{
			nextButton.Enabled = false;
			currentQuestionIndex++;

			nextButton.Text = "Проверить";
			ShowQuestion();
			SetBaseButton();			//возвращаем кнопки в исходное состояние
		}
    }

	private void ShowResults()
    {
        VBoxFinal.Hidden = false;
		VBox.Hidden = true;
		resultLabel.Text = "Ваш результат: " + correctAnswersCount + "/" + numberOfQuestions;
    }

	private void ChangeButtonsEnabled(bool isEnabled)
    {
        foreach (var button in buttons)
		{
			button.Enabled = isEnabled;
		}
    }

	private void CheckAnswer()
	{
		choosenAnswers.Sort();
		if (choosenAnswers.SequenceEqual(questions[currentQuestionIndex].correctAnswers))   //если набор выбранных ответов соответствует правильным
		{
			correctAnswersCount++;
		}
		ShowAnswer();
	}

	private void ShowAnswer()
	{
		List<int> corrAns = questions[currentQuestionIndex].correctAnswers;
		for (int i = 0; i < 4; i++)
		{
			if (corrAns.Contains(i) && choosenAnswers.Contains(i))  //если индекс текущей кнопки есть в списке правильных ответов и выбранных ответов
			{
				buttons[i].ButtonColor = new vec4(0, 0.6, 0, 1);      //то закрашиваем кнопку зеленым
			}
			else if (corrAns.Contains(i) && !choosenAnswers.Contains(i))  //если кнопка не выбрана, а она есть среди правильных 
			{
				buttons[i].ButtonColor = new vec4(0.6, 0.6, 0, 1);      //то закрашиваем кнопку желтым
			}
			else if (!corrAns.Contains(i) && choosenAnswers.Contains(i))  //если кнопка выбрана, а ее нету среди правильных 
			{
				buttons[i].ButtonColor = new vec4(0.8, 0, 0, 1);      //то закрашиваем кнопку красным
			}
		}
	}


	private void ClickCloseButton()
	{
		VBox.Hidden = true;
		HideUI();					//отключаем захват мыши и отвязываем обработчик
		isTestGoing = false;
	}

	private void ClickEndButton()
	{
		VBoxFinal.Hidden = true;
		HideUI();					//отключаем захват мыши и отвязываем обработчик
		isTestGoing = false;
	}



	public bool getActiveTest()
	{
		return isTestGoing;
	}

	private void MoveWindowToCenter()		//сдвигаем окно теста и результатов в центр
	{
		var mainWindowSize = WindowManager.MainWindow.Size;
		var px = TestBox.Width;
		var py = TestBox.Height;
		TestBox.SetPosition(mainWindowSize.x / 2 - px / 2, mainWindowSize.y / 2 - py / 2);

		px = TestBoxResults.Width;
		py = TestBoxResults.Height;
		TestBoxResults.SetPosition(mainWindowSize.x / 2 - px / 2, mainWindowSize.y / 2 - py / 2);
	}

	private void HideUI()
	{
		isTestGoing = false;

		Input.MouseGrab = true;
		Gui gui;
		gui = Gui.GetCurrent();
		gui.MouseShow = false;

		EngineWindowViewport ewv = WindowManager.MainWindow;
		ewv.EventResized.Disconnect(MoveWindowToCenter);			//отвязываем обработчик изменения размеров окна приложения
	}

	private void MixQuestions()
	{
		System.Random rand = new System.Random();
		questions = questions.OrderBy(item => rand.Next()).ToList();
	}

	private void ToggleCursorOn(bool isCursorOn)
    {
        Input.MouseGrab = !isCursorOn;
		Gui gui;
		gui = Gui.GetCurrent();
		gui.MouseShow = isCursorOn;
    }
}

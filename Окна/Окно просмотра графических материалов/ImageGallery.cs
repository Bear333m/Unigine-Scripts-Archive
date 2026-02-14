using Unigine;
using System.Collections.Generic;

[Component(PropertyGuid = "2f48086e821081f8ed8768138e722c21c3939840")]
public class ImageGallery : Component
{
    [ShowInEditor]
    [ParameterAsset(Filter = ".dds", Title = "Чертежи")]
    public List<string> Images = new List<string>();

    private WidgetWindow window;
    private WidgetSprite imageSprite;
    private WidgetLabel infoLabel;
    private WidgetButton btnPrev;
    private WidgetButton btnNext;
    private WidgetButton btnMenu;
    private int currentIndex = 0;
    private bool isVisible = false;

    private const int WINDOW_WIDTH = 500;
    private const int WINDOW_HEIGHT = 360;
    private const int IMAGE_WIDTH = 360;
    private const int IMAGE_HEIGHT = 220;

    void Init()
    {
        CreateWindow();
        UpdateImage();
        UpdateInfo();
    }

    void Update()
    {
        // Открытие/закрытие галереи по клавише G
        if (Input.IsKeyDown(Input.KEY.G))
            ToggleWindow();
    }

    private void CreateWindow()
    {
        Gui gui = Gui.GetCurrent();

        window = new WidgetWindow(gui, "Просмотр изображений");
        window.Width = WINDOW_WIDTH;
        window.Height = WINDOW_HEIGHT;
        window.Hidden = true;
        window.SetPosition(
            (gui.Width - WINDOW_WIDTH) / 2,
            (gui.Height - WINDOW_HEIGHT) / 2
        );

        // Виджет для отображения текстуры изображения
        imageSprite = new WidgetSprite(gui);
        imageSprite.Width = IMAGE_WIDTH;
        imageSprite.Height = IMAGE_HEIGHT;
        imageSprite.SetPosition((WINDOW_WIDTH - IMAGE_WIDTH) / 2, 20);
        window.AddChild(imageSprite);

        // Текст с информацией о текущем изображении
        infoLabel = new WidgetLabel(gui);
        infoLabel.Width = WINDOW_WIDTH - 40;
        infoLabel.Height = 30;
        infoLabel.SetPosition(20, 255);
        infoLabel.TextAlign = Gui.ALIGN_CENTER;
        window.AddChild(infoLabel);

        // Кнопки навигации
        int buttonY = 290;

        btnPrev = new WidgetButton(gui, "Назад");
        btnPrev.Width = 100;
        btnPrev.Height = 30;
        btnPrev.SetPosition(40, buttonY);
        btnPrev.EventClicked.Connect(PrevImage);
        window.AddChild(btnPrev);

        btnNext = new WidgetButton(gui, "Вперед");
        btnNext.Width = 100;
        btnNext.Height = 30;
        btnNext.SetPosition(WINDOW_WIDTH - 140, buttonY);
        btnNext.EventClicked.Connect(NextImage);
        window.AddChild(btnNext);

        btnMenu = new WidgetButton(gui, "Главное меню");
        btnMenu.Width = 160;
        btnMenu.Height = 30;
        btnMenu.SetPosition((WINDOW_WIDTH - 160) / 2, buttonY + 40);
        btnMenu.EventClicked.Connect(ReturnToMenu);
        window.AddChild(btnMenu);

        gui.AddChild(window, Gui.ALIGN_OVERLAP);
    }

    private void UpdateImage()
    {
        if (Images.Count == 0)
        {
            imageSprite.Texture = "";
            return;
        }
        imageSprite.Texture = Images[currentIndex];
    }

    private void UpdateInfo()
    {
        if (Images.Count == 0)
            infoLabel.Text = "Изображения не добавлены";
        else
            infoLabel.Text = $"Изображение {currentIndex + 1} из {Images.Count}";
    }

    private void NextImage()
    {
        if (Images.Count == 0) return;

        currentIndex++;
        if (currentIndex >= Images.Count)
            currentIndex = 0;

        UpdateImage();
        UpdateInfo();
    }

    private void PrevImage()
    {
        if (Images.Count == 0) return;

        currentIndex--;
        if (currentIndex < 0)
            currentIndex = Images.Count - 1;

        UpdateImage();
        UpdateInfo();
    }

    private void ReturnToMenu()
    {
        HideWindow();
        // TODO: Добавить логику возврата в главное меню
    }

    private void ToggleWindow()
    {
        if (isVisible)
            HideWindow();
        else
            ShowWindow();
    }

    private void ShowWindow()
    {
        window.Hidden = false;
        isVisible = true;
        Gui.GetCurrent().MouseShow = true;
        Input.MouseGrab = false;

        UpdateImage();
        UpdateInfo();
    }

    private void HideWindow()
    {
        window.Hidden = true;
        isVisible = false;
        Gui.GetCurrent().MouseShow = false;
        Input.MouseGrab = true;
    }
}
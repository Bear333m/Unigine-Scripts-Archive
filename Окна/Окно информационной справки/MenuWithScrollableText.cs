using Unigine;

[Component(PropertyGuid = "d7d6d9ecd1bb8ccee47ba702999e8191aae8469e")]
public class MenuWithScrollableText : Component
{
    private Gui gui;
    private WidgetWindow window;
    private WidgetScrollBox scroll;
    private WidgetLabel textLabel;
    private WidgetButton quitButton;

	private WidgetEditText textBox;
    private bool uiBuilt = false;

    [Parameter] public string Title = "Техника безопасности";
    [Parameter] public string LongText =
        "Длинный текст...\n\n" +
        "Строка 1\nСтрока 2\nСтрока 3\n...\n" +
        "Ещё строки...\nЕщё строки...\n";

    void Update()
    {
        if (!World.IsLoaded)
            return;

        if (!uiBuilt)
        {
            gui = Gui.GetCurrent();
            if (gui == null) return;

            BuildUI();
            uiBuilt = true;
            window.Hidden = true; // стартуем скрытым
        }
    }

    public void Show()
    {
        if (!uiBuilt || window == null) return;
        window.Hidden = false;
    }

    public void Hide()
    {
        if (!uiBuilt || window == null) return;
        window.Hidden = true;
    }

    public bool IsReady()
    {
        return uiBuilt && window != null;
    }

    private void BuildUI()
{
    window = new WidgetWindow(gui, Title);
    window.Width = 520;
    window.Height = 420;

    var vbox = new WidgetVBox(gui);
    vbox.SetPadding(12, 12, 12, 12);
    vbox.SetSpace(8, 8);

    scroll = new WidgetScrollBox(gui);
    scroll.Width = 480;
    scroll.Height = 320;
    scroll.VScrollEnabled = true;

    // ВАЖНО: делаем внутренний виджет заведомо выше scroll,
    // тогда вертикальная прокрутка точно появится
    textBox = new WidgetEditText(gui);
    textBox.Width = scroll.Width - 10;
    textBox.Height = 1000;                 // <- ключевой момент
    textBox.Text = LongText;

    // Если в твоей версии есть такие свойства/методы — включи:
    // textBox.Multiline = true;
    // textBox.ReadOnly = true;

    scroll.AddChild(textBox, Gui.ALIGN_EXPAND);

    quitButton = new WidgetButton(gui, "Выход");
    quitButton.EventClicked.Connect(() => Engine.Quit());

    vbox.AddChild(scroll, Gui.ALIGN_EXPAND);
    vbox.AddChild(quitButton, Gui.ALIGN_RIGHT);

    window.AddChild(vbox, Gui.ALIGN_EXPAND);
    gui.AddChild(window, Gui.ALIGN_CENTER);
}


    void Shutdown()
    {
        if (uiBuilt && gui != null && window != null)
            gui.RemoveChild(window);

        window = null; scroll = null; textLabel = null; quitButton = null; gui = null;
        uiBuilt = false;
    }
}

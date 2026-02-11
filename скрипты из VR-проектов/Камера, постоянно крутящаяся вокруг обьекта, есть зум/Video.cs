using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "5b070674ee03654e4e37d71b661893a2129bcb43")]
public class Video : Component
{
    [ShowInEditor]
    public PlayerPersecutor playerPersecutor; // ← заменили тип

    [ShowInEditor]
    public Node persecutorTarget; // ← объект, вокруг которого вращаемся

    private WidgetSpriteViewport camera;
    public int posX = 40;
    public int posY = 140;

    private float angle = 0.0f;      // текущий угол вращения (в радианах)
    public float radius = 5.0f;      // радиус орбиты
    public float rotationSpeed = 30.0f; // градусов в секунду

    [ShowInEditor] private ObjectGui gui1;

    void Init()
    {
        Gui gui = gui1.GetGui();
        camera = new WidgetSpriteViewport(gui, gui1.ScreenWidth, gui1.ScreenHeight);

        if (playerPersecutor != null && persecutorTarget != null)
        {
            UpdatePersecutorPosition();
            camera.SetCamera(playerPersecutor.Camera);
        }
        gui.AddChild(camera, Gui.ALIGN_OVERLAP);
    }

    void Update()
    {
        if (playerPersecutor == null || persecutorTarget == null) return;
        angle += rotationSpeed * Game.IFps / 57.3f;
        if (Input.IsKeyPressed(Input.KEY.N))
            radius += 1.0f * Game.IFps;
        if (Input.IsKeyPressed(Input.KEY.M))
            radius -= 1.0f * Game.IFps;
        UpdatePersecutorPosition();
        camera.SetCamera(playerPersecutor.Camera);
    }

    void UpdatePersecutorPosition()
    {
        vec3 targetPos = persecutorTarget.WorldPosition;
        double x = targetPos.x + radius * Math.Sin(angle);
        double y = targetPos.y + radius * Math.Cos(angle);
        double z = targetPos.z;
        playerPersecutor.Position = new vec3(x, y, z);
        playerPersecutor.Target = persecutorTarget;
    }
}


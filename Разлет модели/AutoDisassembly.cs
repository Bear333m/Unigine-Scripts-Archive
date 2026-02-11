// Unigine C# 2.20.0.1
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "e8b3206c052fd3efbdaf3151af07ce1a58cbe762")]
public class Inspection : Component
{
    [Parameter]
    public float ScaleFactorY = 1.0f;
    public float ScaleFactorX = 1.0f;
    public float ScaleFactorZ = 1.0f;

    [Parameter]
    public float AnimationDuration = 1.0f;

    [Parameter]
    public float HoldDuration = 10.0f;

    private List<Node> children = new List<Node>();
    private List<vec3> startPositions = new List<vec3>();
    private List<vec3> targetPositions = new List<vec3>();

    private float elapsedTime = 0.0f;
    private float holdTimer = 0.0f;

    private enum State
    {
        Idle,
        AnimatingForward,
        WaitingToRestore,
        AnimatingBack
    }
    private State currentState = State.Idle;

    void Update()
    {
        // Ручной запуск анимации вперёд по Y
        if (Input.IsKeyDown(Input.KEY.Y) && currentState == State.Idle && !Unigine.Console.Active)
        {
            StartForwardAnimation();
        }

        switch (currentState)
        {
            case State.AnimatingForward:
                AnimateToTargets();
                break;

            case State.WaitingToRestore:
                holdTimer += Engine.IFps;
                if (holdTimer >= HoldDuration)
                {
                    StartRestoreAnimation();
                }
                break;

            case State.AnimatingBack:
                AnimateToStarts();
                break;
        }
    }

    void StartForwardAnimation()
    {
        Node parent = node;
        if (parent == null) return;

        children.Clear();
        startPositions.Clear();
        targetPositions.Clear();

        for (int i = 0; i < parent.NumChildren; i++)
        {
            Node child = parent.GetChild(i);
            if (child != null)
            {
                vec3 pos = (vec3)child.Position;
                children.Add(child);
                startPositions.Add(pos);
                targetPositions.Add(new vec3(pos.x * ScaleFactorX, pos.y * ScaleFactorY, pos.z * ScaleFactorZ));
            }
        }

        if (children.Count == 0) return;

        elapsedTime = 0.0f;
        currentState = State.AnimatingForward;
    }

    void AnimateToTargets()
    {
        elapsedTime += Engine.IFps;
        float t = elapsedTime / AnimationDuration;
        if (t > 1.0f) t = 1.0f;

        float easedT = 1.0f - (1.0f - t) * (1.0f - t);

        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] != null)
            {
                vec3 pos = startPositions[i] + (targetPositions[i] - startPositions[i]) * easedT;
                children[i].Position = pos;
            }
        }

        if (t >= 1.0f)
        {
            // Зафиксируем позиции
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] != null)
                    children[i].Position = targetPositions[i];
            }

            // Переходим к ожиданию
            holdTimer = 0.0f;
            currentState = State.WaitingToRestore;
        }
    }

    void StartRestoreAnimation()
    {
        elapsedTime = 0.0f;
        currentState = State.AnimatingBack;
    }

    void AnimateToStarts()
    {
        elapsedTime += Engine.IFps;
        float t = elapsedTime / AnimationDuration;
        if (t > 1.0f) t = 1.0f;

        float easedT = 1.0f - (1.0f - t) * (1.0f - t);

        for (int i = 0; i < children.Count; i++)
        {
            if (children[i] != null)
            {
                vec3 pos = targetPositions[i] + (startPositions[i] - targetPositions[i]) * easedT;
                children[i].Position = pos;
            }
        }

        if (t >= 1.0f)
        {
            for (int i = 0; i < children.Count; i++)
            {
                if (children[i] != null)
                    children[i].Position = startPositions[i];
            }

            currentState = State.Idle;
        }
    }
}

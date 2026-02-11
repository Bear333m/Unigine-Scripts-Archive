using System;
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Serializable]
public class AnimationStep
{
    [ShowInEditor]
    public ObjectMeshStatic TargetObject;

    [ShowInEditor]
    public vec3 TargetPosition = vec3.ZERO;

    [ShowInEditor]
    [ParameterSlider(Min = 0.1f, Max = 10.0f, Title = "Скорость перемещения")]
    public float Speed = 1.0f;

    [ShowInEditor]
    [ParameterSlider(Min = 0.0f, Max = 30.0f, Title = "Задержка перед стартом (сек)")]
    public float StartDelay = 0.0f;
}

[Component(PropertyGuid = "dcb58ec642dc9f18152a45bbe036eae83c21b2be")]
public class AnimationSteps : Component
{
    [ShowInEditor]
    public AnimationStep[] Steps = new AnimationStep[0];

    private List<ActiveAnimation> activeAnimations = new();
    private bool isPlaying = false;
    private bool isReverse = false; // флаг направления
    private float startTime;

    // Сохраняем начальные позиции для обратного воспроизведения
    private Dictionary<ObjectMeshStatic, vec3> initialPositions = new();

    private class ActiveAnimation
    {
        public ObjectMeshStatic obj;
        public vec3 startPos;
        public vec3 endPos;
        public float speed;
        public float delay;
        public bool completed = false;
        public bool started = false;
    }

    void Init()
    {
        StopAllAnimations();
        CacheInitialPositions(); // Сохраняем стартовые позиции при инициализации
    }

    void Update()
    {
        if (!isPlaying) return;

        float currentTime = Game.Time - startTime;

        foreach (var anim in activeAnimations)
        {
            if (!anim.started && currentTime >= anim.delay)
            {
                anim.started = true;
                anim.startPos = anim.obj.WorldPosition;
            }

            if (!anim.started || anim.completed) continue;

            float progress = (currentTime - anim.delay) * anim.speed;
            float distance = (anim.endPos - anim.startPos).Length;

            if (distance < 0.001f)
            {
                anim.obj.WorldPosition = anim.endPos;
                anim.completed = true;
                continue;
            }

            vec3 direction = (anim.endPos - anim.startPos) / distance;
            float traveled = progress;

            if (traveled >= distance)
            {
                anim.obj.WorldPosition = anim.endPos;
                anim.completed = true;
            }
            else
            {
                anim.obj.WorldPosition = anim.startPos + direction * traveled;
            }
        }

        bool allCompleted = true;
        foreach (var anim in activeAnimations)
        {
            if (!anim.completed)
            {
                allCompleted = false;
                break;
            }
        }

        if (allCompleted)
        {
            isPlaying = false;
            OnAnimationComplete();
        }
    }

    // Сохраняем начальные позиции всех объектов из Steps
    private void CacheInitialPositions()
    {
        initialPositions.Clear();
        foreach (var step in Steps)
        {
            if (step.TargetObject != null)
            {
                initialPositions[step.TargetObject] = step.TargetObject.WorldPosition;
            }
        }
    }

    // Воспроизведение вперёд
    public void Play()
    {
        if (isPlaying) return;

        PrepareAnimations(forward: true);
        isReverse = false;
        isPlaying = true;
        startTime = Game.Time;
    }

    // Воспроизведение в обратном направлении
    public void PlayReverse()
    {
        if (isPlaying) return;

        PrepareAnimations(forward: false);
        isReverse = true;
        isPlaying = true;
        startTime = Game.Time;
    }

    // Подготовка списка анимаций (вперёд или назад)
    private void PrepareAnimations(bool forward)
    {
        activeAnimations.Clear();

        if (forward)
        {
            // Прямое воспроизведение: обходим шаги в прямом порядке
            for (int i = 0; i < Steps.Length; i++)
            {
                var step = Steps[i];
                if (step.TargetObject == null) continue;

                step.TargetObject.Enabled = true;

                vec3 startPos = step.TargetObject.WorldPosition;
                vec3 endPos = step.TargetPosition;

                activeAnimations.Add(new ActiveAnimation
                {
                    obj = step.TargetObject,
                    startPos = startPos,
                    endPos = endPos,
                    speed = step.Speed,
                    delay = step.StartDelay,
                    completed = false,
                    started = false
                });
            }
        }
        else
        {
            // Обратное воспроизведение: обходим шаги в обратном порядке
            // Находим максимальную задержку
            float maxDelay = 0;
            for (int j = 0; j < Steps.Length; j++)
            {
                if (Steps[j].StartDelay > maxDelay)
                    maxDelay = Steps[j].StartDelay;
            }

            for (int i = Steps.Length - 1; i >= 0; i--)
            {
                var step = Steps[i];
                if (step.TargetObject == null) continue;

                step.TargetObject.Enabled = true;

                vec3 startPos = step.TargetObject.WorldPosition;
                vec3 endPos = initialPositions.GetValueOrDefault(step.TargetObject, startPos);

                // Переворачиваем задержку: первый в обратке (последний в прямой) — без задержки
                float adjustedDelay = maxDelay - step.StartDelay;

                activeAnimations.Add(new ActiveAnimation
                {
                    obj = step.TargetObject,
                    startPos = startPos,
                    endPos = endPos,
                    speed = step.Speed,
                    delay = adjustedDelay,
                    completed = false,
                    started = false
                });
            }
        }
    }

    public void Stop()
    {
        isPlaying = false;
        activeAnimations.Clear();
    }

    public void StopAllAnimations()
    {
        Stop();
    }

    private void OnAnimationComplete()
    {
        Log.Message(isReverse ? "Обратная анимация завершена." : "Прямая анимация завершена.");
    }

    public bool IsPlaying => isPlaying;
}
using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "563b2478e23e04738397d0a438111062b8e981ca")]
public class CameraCast : Component
{
    [ShowInEditor]
    [Parameter(Title = "Камера")]
    private PlayerSpectator shootingCamera = null;

    [ShowInEditor]
    [ParameterMask(MaskType = ParameterMaskAttribute.TYPE.INTERSECTION)]
    [Parameter(Title = "Mаска пересечения")]
    private int mask = 1;

    [ShowInEditor]
    [Parameter(Title = "Режим подсветки")]
    private OutlineMode outlineMode = OutlineMode.Fixed;

    // ================== НАСТРОЙКИ FIXED ==================
    [ShowInEditor]
    [ParameterColor(Title = "Цвет обводки")]
    [ParameterCondition(nameof(outlineMode), (int)OutlineMode.Fixed)]
    private vec4 outlineColor = new vec4();
    // ================== НАСТРОЙКИ SWITCHING ==================
    [ShowInEditor]
    [ParameterColor(Title = "Цвета для переключения")]
    [ParameterCondition(nameof(outlineMode), (int)OutlineMode.Switching)]
    private List<vec4> switchColors = new List<vec4>();
    
    [ShowInEditor]
    [ParameterSlider(Title = "Интервал смены (сек)", Min = 0.1f, Max = 5.0f)]
    [ParameterCondition(nameof(outlineMode), (int)OutlineMode.Switching)]
    private float switchInterval = 1.0f;

    // ================== НАСТРОЙКИ SMOOTH ================== 
    [ShowInEditor]
    [ParameterColor(Title = "Цвета для переливания")]
    [ParameterCondition(nameof(outlineMode), (int)OutlineMode.Smooth)]
    private List<vec4> smoothColors = new List<vec4>();
    
    [ShowInEditor]
    [ParameterSlider(Title = "Скорость переливания", Min = 0.1f, Max = 5.0f)]
    [ParameterCondition(nameof(outlineMode), (int)OutlineMode.Smooth)]
    private float smoothSpeed = 1.0f;

    private dvec3 p0, p1;
    private Object selectedObject;
    private Object pastObject;

    // Переменные для Switching режима
    private float switchTimer = 0.0f;
    private int switchColorIndex = 0;
    private vec4 currentSwitchColor;
    private bool switchingModeAvailable = false;
    
    // Переменные для Smooth режима
    private float smoothTimer = 0.0f;
    private vec4 currentSmoothColor;
    private bool smoothModeAvailable = false;

    private enum OutlineMode
    {
        [Parameter(Title = "Фиксированный цвет")]
        Fixed,
        
        [Parameter(Title = "Переключение цветов")]
        Switching,
        
        [Parameter(Title = "Плавное переливание")]
        Smooth
    }
    
    void Init()
    {
        UpdateModeAvailability();
        
        if (switchColors.Count > 0)
        {
            currentSwitchColor = switchColors[0];
        }
    }

    void Update()
    {
        UpdateModeAvailability();
        
        CorrectModeIfNeeded();
        
        UpdateSwitchingMode();
        
        UpdateSmoothMode();
        
        selectedObject = GetObject();
        if (selectedObject)
        {
            vec4 colorToUse = GetCurrentColor();
            SetOutline(selectedObject, 1, colorToUse);
        }
        
        if (pastObject != selectedObject)
        {
            if (pastObject != null)
            {
                vec4 colorToUse = GetCurrentColor();
                SetOutline(pastObject, 0, colorToUse);
            }
            pastObject = selectedObject;
        }
    }
    
    private void UpdateModeAvailability()
    {
        switchingModeAvailable = (switchColors.Count > 0);
        smoothModeAvailable = (smoothColors.Count >= 2);
    }
    
    private void CorrectModeIfNeeded()
    {
        if (!switchingModeAvailable && outlineMode == OutlineMode.Switching)
        {
            outlineMode = OutlineMode.Fixed;
            Log.Message("Режим переключения цветов недоступен: список цветов пуст. Переключено на фиксированный цвет.\n");
        }
        
        if (!smoothModeAvailable && outlineMode == OutlineMode.Smooth)
        {
            outlineMode = OutlineMode.Fixed;
            Log.Message("Режим плавного переливания недоступен: нужно минимум 2 цвета. Переключено на фиксированный цвет.\n");
        }
    }
    
    private void UpdateSwitchingMode()
    {
        if (outlineMode == OutlineMode.Switching && switchColors.Count > 0)
        {
            switchTimer += Game.IFps;
            if (switchTimer >= switchInterval)
            {
                switchTimer = 0.0f;
                switchColorIndex = (switchColorIndex + 1) % switchColors.Count;
                currentSwitchColor = switchColors[switchColorIndex];
            }
        }
    }
    
    private void UpdateSmoothMode()
    {
        if (outlineMode == OutlineMode.Smooth && smoothColors.Count >= 2)
        {
            smoothTimer += Game.IFps * smoothSpeed;
            
            // Плавное переливание по кругу через все цвета
            float totalSegments = smoothColors.Count;
            float loopTime = smoothTimer % totalSegments;
            
            int colorA = (int)loopTime;
            int colorB = (colorA + 1) % smoothColors.Count;
            float lerpValue = loopTime - colorA;
            
            currentSmoothColor = MathLib.Lerp(
                smoothColors[colorA], 
                smoothColors[colorB], 
                lerpValue);
        }
    }
    
    private vec4 GetCurrentColor()
    {
        if (outlineMode == OutlineMode.Fixed)
        {
            return outlineColor;
        }
        else if (outlineMode == OutlineMode.Switching)
        {
            if (switchColors.Count > 0)
                return currentSwitchColor;
            else
                return outlineColor;
        }
        else
        {
            if (smoothColors.Count >= 2)
                return currentSmoothColor;
            else
                return outlineColor;
        }
    }

    public Object GetObject()
    {
        p0 = shootingCamera.WorldPosition;
        p1 = shootingCamera.WorldPosition + shootingCamera.GetWorldDirection() * 5.0f;
        WorldIntersection intersection = new WorldIntersection();
        Object obj = World.GetIntersection(p0, p1, mask, intersection);
        return obj;
    }

    private void SetOutline(Object gameObject, int isOutline, vec4 color)
    {
        for (var i = 0; i < gameObject.NumSurfaces; i++)
        {
            gameObject.SetMaterialState("auxiliary", isOutline, i);
            gameObject.SetMaterialParameterFloat4("auxiliary_color", color, i);
        }
    }
}
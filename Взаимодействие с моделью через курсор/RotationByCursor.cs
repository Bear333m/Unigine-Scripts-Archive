using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "c2620f1044f124571c4a06b6c36b16107f4e13c0")]
public class ObjectRotator : Component
{
    [ShowInEditor]
    [ParameterSlider(Title = "Скорость вращения")]
    public float rotationSpeed = 1.5f;

    [ShowInEditor]
    [ParameterMask(MaskType = ParameterMaskAttribute.TYPE.INTERSECTION)]
    [Parameter(Title = "Маска выбора")]
    private int selectionMask = 1;

    private Node selectedNode = null;
    
    private bool isRotating = false;
    private ivec2 lastMousePos;

	bool isCursorToggledOn = false;

    void Init()
    {
        // Инициализация не требуется
    }

    void Update()
    {
        // Выбор объекта по левой кнопке мыши
        if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT))
        {
            TrySelectObject();
        }
        
        // Начало вращения объекта по правой кнопке мыши
        if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.RIGHT))
        {
            if (selectedNode != null)
            {
                StartRotation();
            }
        }
        
        // Вращение объекта при зажатой правой кнопке
        if (isRotating && Input.IsMouseButtonPressed(Input.MOUSE_BUTTON.RIGHT))
        {
            RotateObject();
        }
        else if (Input.IsMouseButtonUp(Input.MOUSE_BUTTON.RIGHT))
        {
            StopRotation();
        }
        
        // Сброс выбора по ESC
        if (Input.IsKeyPressed(Input.KEY.ESC))
        {
            ResetSelection();
        }
        
        // Сброс вращения объекта по R
        if (Input.IsKeyPressed(Input.KEY.R))
        {
            ResetObjectRotation();
        }
        
        // Визуализация выбранного объекта
        if (selectedNode != null)
        {
            Visualizer.RenderSphere(0.5f, selectedNode.WorldTransform, new vec4(0, 1, 0, 0.5f));
        }

		if (Input.IsKeyDown(Input.KEY.G))
		{
			isCursorToggledOn = !isCursorToggledOn;
			ToggleCursorMode();
			
		}
		else if (isCursorToggledOn)			//пока меню открыто, постоянно отключаем захват мышки (для поворота камеры) и включаем курсор, 
		{								//так как иначе при нажатии в пустое место курсор исчезает
			ToggleCursorOn(true);
		}
    }

    void StartRotation()
    {
        isRotating = true;
        lastMousePos = Input.MousePosition;
        
        // Можно также захватить мышь для более плавного вращения
        Input.MouseHandle = Input.MOUSE_HANDLE.SOFT;
    }

    void RotateObject()
    {
        if (selectedNode == null) return;
        
        ivec2 currentMousePos = Input.MousePosition;
        ivec2 delta = currentMousePos - lastMousePos;
        
        // Вращаем объект
        RotateNode(selectedNode, delta);
        
        lastMousePos = currentMousePos;
    }

    void RotateNode(Node node, ivec2 mouseDelta)
    {
        if (node == null) return;
        
        // Получаем текущее вращение объекта
        quat currentRotation = node.GetRotation();
        
        // Вращаем объект с помощью кватернионов
        // Создаем кватернионы для вращения вокруг осей X и Y
        float angleX = mouseDelta.y * rotationSpeed * -0.001f;
        float angleY = -mouseDelta.x * rotationSpeed * -0.001f;
        
        // Создаем кватернионы вращения
        quat rotationX = new quat(new vec3(1, 0, 0), angleX);
        quat rotationY = new quat(new vec3(0, 1, 0), angleY);
        
        // Комбинируем вращения (сначала Y, потом X)
        quat newRotation = rotationY * rotationX * currentRotation;
        
        // Применяем вращение к объекту
        node.SetRotation(newRotation);
    }

    void StopRotation()
    {
        isRotating = false;
        Input.MouseHandle = Input.MOUSE_HANDLE.USER;
    }

    void TrySelectObject()
    {
        // Получаем текущего игрока (камеру)
        Player player = Game.Player;
        if (player == null) return;
        
        // Получаем направление луча из позиции мыши
        // В Unigine нужно использовать методы WorldBoundBox или другие подходы
        // Для простоты используем World.GetIntersection с лучом из камеры
        
        //dvec3 p0 = player.WorldPosition;
        dvec3 direction;
        
        // Если это PlayerSpectator, используем его направление
        if (player is PlayerSpectator spectator)
        {
            direction = spectator.GetDirection();
        }
        else
        {
            // Иначе используем текущее направление игрока
            direction = new dvec3(player.GetDirection());
        }

		EngineWindow main_window = WindowManager.MainWindow;
		ivec2 main_size = main_window.Size;
		dvec3 p0, p1;
		int mouse_x = Input.MousePosition.x - main_window.Position.x;
		int mouse_y = Input.MousePosition.y - main_window.Position.y;
		//Camera camera = Game.Player.Camera;
		//direction = camera.GetDirectionFromScreen(1, 1);
		player.GetDirectionFromScreen(out p0, out p1, mouse_x, mouse_y, 0, 0, main_size.x, main_size.y);
        

        //dvec3 p1 = p0 + direction * 1000.0f;
		direction = p1 - p0;
		p1 = p0 + direction * 10.0f;
        
        // Проверяем пересечение с объектами
        // В Unigine для получения пересеченного узла используем WorldIntersectionNormal
        WorldIntersectionNormal intersection = new WorldIntersectionNormal();
        Node hitNode = World.GetIntersection(p0, p1, selectionMask, intersection);
		//GameIntersection intersection = new GameIntersection();
		//Node hitNode = Game.GetIntersection(p0, p1, 1.5f, 1, intersection);
		Log.Message($"Выбран объект: {p1}\n");
        

        if (hitNode != null)
        {
            selectedNode = hitNode;
            Log.Message($"Выбран объект: {hitNode.Name}\n");
        }
        else
        {
            // Снимаем выделение если кликнули в пустоту
            if (selectedNode != null)
            {
                Log.Message("Снято выделение\n");
                selectedNode = null;
            }
        }
    }

    void ResetSelection()
    {
        if (selectedNode != null)
        {
            Log.Message("Выделение сброшено\n");
        }
        
        selectedNode = null;
        isRotating = false;
        Input.MouseHandle = Input.MOUSE_HANDLE.USER;
    }

    void ResetObjectRotation()
    {
        if (selectedNode != null)
        {
            // Сбрасываем вращение к нулевому (кватернион без вращения)
            selectedNode.SetRotation(new quat(0, 0, 0));
            Log.Message("Вращение объекта сброшено\n");
        }
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

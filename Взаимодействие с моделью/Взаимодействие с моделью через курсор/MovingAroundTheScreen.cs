using System;
using System.Collections.Generic;
using Unigine;

namespace UnigineApp
{
    [Component(PropertyGuid = "7d8712d613d2eb60b0e4a247838fc20acb896e83")]
    public class MovingAroundTheScreen : Component
    {
        // ================= Настройки ====================
        [Parameter(Group = "Settings")] 
        public int InteractionMask = 1;
        
        [Parameter(Group = "Settings")] 
        public double MaxDistance = 10.0;
        
        [Parameter(Group = "Settings")] 
        public Input.MOUSE_BUTTON MoveButton = Input.MOUSE_BUTTON.RIGHT;

        [Parameter(Group = "Movement")] 
        public bool MoveInCameraPlane = true;
        
        [Parameter(Group = "Movement")] 
        public double MoveSpeed = 5.0;
        
        [Parameter(Group = "Movement")] 
        public double ZoomSpeed = 0.5;
        
        [Parameter(Group = "Movement")] 
        public double MinDistance = 0.05;
        
        [Parameter(Group = "Movement")] 
        public double MaxZoomDistance = 50.0;
        
        [Parameter(Group = "Movement")] 
        public bool LockX, LockY, LockZ;
        
        [Parameter(Group = "Rotation")] 
        public double RotationSpeed = 50;
        
        [Parameter(Group = "Rotation")] 
        public bool InvertRotationY;
        
        [Parameter(Group = "Camera")] 
        public Node Camera;

        [Parameter(Group = "Debug")] 
        public bool DebugMode;

        // ==================== ПОЛЯ ====================
        private WorldIntersection world_intersection;
        
        private MovableObject grabbed_movable_object = null;
        private Node grabbed_node => grabbed_movable_object != null ? grabbed_movable_object.node : null;
        
        private dvec3 grab_offset = dvec3.ZERO;
        private dvec3 grab_point_world = dvec3.ZERO;
        
        private double current_grab_distance = 0.0;
        private bool is_initialized = false;
         
        private bool _wasRotating = false;
        private ivec2 _fixedMousePos = ivec2.ZERO;
        private bool _useFixedMousePos = false;
        private bool _wasShiftHeld = false;

        // 🔹 Для временного родительства к камере
        private Node _originalParent = null;
        private dvec3 _originalWorldPosition = dvec3.ZERO;
        private quat _originalWorldRotation = quat.IDENTITY;
        private bool _isReparented = false;

        // ==================== INIT ====================
        void Init()
        {
            try
            {
                world_intersection = new WorldIntersection();
                is_initialized = true;
                
                if (DebugMode) 
                    Log.Message("✅ [MovingAroundTheScreen] Controller Init() completed.\n");
            }
            catch (Exception e)
            {
                Log.Message("❌ [MovingAroundTheScreen] Init() ERROR: " + e.Message + "\n");
            }
        }

        // ==================== UPDATE ====================
        void Update()
        {
            try
            {
                if (!is_initialized) return;

                Player player = Game.Player;
                if (player == null) return;

                ivec2 mouse_pos = _useFixedMousePos ? _fixedMousePos : Input.MousePosition;
                
                dvec3 player_pos = player.WorldPosition;
                dvec3 cursor_dir = player.GetDirectionFromMainWindow(mouse_pos.x, mouse_pos.y);
                dvec3 ray_end = player_pos + cursor_dir * MaxDistance;

                bool is_button_held = Input.IsMouseButtonPressed(MoveButton);
                bool is_shift_held = Input.IsKeyPressed(Input.KEY.ANY_SHIFT);

                ivec2 mouse_delta_raw = Input.MouseDeltaRaw;
                
                bool is_rotating = is_button_held && is_shift_held;

                // 🔹 Начало вращения — прикрепляем объект к камере
                if (is_rotating && !_wasRotating)
                {
                    _fixedMousePos = Input.MousePosition;
                    _useFixedMousePos = true;
                    ControlsApp.MouseHandle = Input.MOUSE_HANDLE.USER;
                    
                    if (grabbed_node != null && !_isReparented)
                    {
                        ReparentToCamera(grabbed_node, Camera);
                    }
                }
                // 🔹 Конец вращения — открепляем объект от камеры
                else if (!is_rotating && _wasRotating)
                {
                    _useFixedMousePos = false;
                    RestoreNodeToWorld();
                }
                _wasRotating = is_rotating;
                _wasShiftHeld = is_shift_held;

                // Логика движения/вращения ЗАХВАЧЕННОГО объекта
                if (is_button_held && grabbed_movable_object != null)
                {
                    if (!grabbed_movable_object.CanBeMoved)
                    {
                        ReleaseGrab();
                        return;
                    }

                    dvec3 final_position = grabbed_node.WorldPosition;
                    bool position_changed = false;

                    // 🔹 РЕЖИМ ЗУМА
                    double mouse_wheel = Input.MouseWheel;
                    
                    if (MathLib.Abs(mouse_wheel) > 0.01)
                    {
                        dvec3 camera_forward = player.ViewDirection;
                        dvec3 current_offset = grabbed_node.WorldPosition - player_pos;
                        double current_distance = MathLib.Length(current_offset);
                        
                        if (current_grab_distance <= 0.0)
                            current_grab_distance = current_distance;
                        
                        bool zoom_in = mouse_wheel > 0;
                        bool zoom_out = mouse_wheel < 0;
                        bool can_zoom = true;
                        
                        if (zoom_in && current_distance <= MinDistance) can_zoom = false;
                        if (zoom_out && current_distance >= MaxZoomDistance) can_zoom = false;
                        
                        if (can_zoom && zoom_in)
                        {
                            double dot_product = MathLib.Dot(current_offset.Normalized, camera_forward);
                            if (dot_product < 0.1) can_zoom = false;
                        }
                        
                        if (can_zoom)
                        {
                            double zoom_delta = mouse_wheel * ZoomSpeed;
                            current_grab_distance += zoom_delta;
                            current_grab_distance = MathLib.Clamp(current_grab_distance, MinDistance, MaxZoomDistance);
                            position_changed = true;
                            
                            if (DebugMode && Game.Frame % 30 == 0)
                                Log.Message("🔍 Zoom: {0:F2} → {1:F2}\n", current_distance, current_grab_distance);
                        }
                    }

                    // 🔹 Перемещение или вращение
                    if (is_rotating)
                    { 
                        float delta_x = (float)mouse_delta_raw.x;
                        float delta_y = (float)mouse_delta_raw.y;
                        
                        if (InvertRotationY) delta_y = -delta_y;
                        
                        // 🔹 Инверсия по горизонтали для интуитивного управления
                        float rotation_angle_x = (float)(delta_x * RotationSpeed * Game.IFps * MathLib.DEG2RAD);
                        float rotation_angle_y = (float)(delta_y * RotationSpeed * Game.IFps * MathLib.DEG2RAD);
                        
                        quat current_rotation = grabbed_node.GetRotation();
                        quat yaw_rotation = new quat(vec3.FORWARD, rotation_angle_x);
                        quat pitch_rotation = new quat(vec3.RIGHT, rotation_angle_y);
                        quat new_rotation = yaw_rotation * pitch_rotation * current_rotation;
                        
                        grabbed_node.SetRotation(new_rotation);
                    }
                    else
                    {
                        // 🔹 РЕЖИМ ПЕРЕМЕЩЕНИЯ
                        dvec3 new_cursor_dir = player.GetDirectionFromMainWindow(mouse_pos.x, mouse_pos.y);
                        dvec3 new_grab_point;
                         
                        if (MoveInCameraPlane)
                        {
                            dvec3 camera_forward = player.ViewDirection;
                            double denom = MathLib.Dot(new_cursor_dir, camera_forward);
                            if (MathLib.Abs(denom) > 0.001)
                            {
                                double t = current_grab_distance / denom;
                                new_grab_point = player_pos + new_cursor_dir * t;
                            }
                            else
                            {
                                new_grab_point = player_pos + camera_forward * current_grab_distance;
                            }
                        }
                        else
                        {
                            new_grab_point = player_pos + new_cursor_dir * current_grab_distance;
                        }
                        
                        dvec3 new_position = new_grab_point + grab_offset;
                        
                        if (LockX) new_position.x = grabbed_node.WorldPosition.x;
                        if (LockY) new_position.y = grabbed_node.WorldPosition.y;
                        if (LockZ) new_position.z = grabbed_node.WorldPosition.z;
                        
                        dvec3 target_position = new_position;
                        dvec3 current_position = grabbed_node.WorldPosition;
                        double lerp_factor = Math.Min(1.0, MoveSpeed * Game.IFps);
                        final_position = MathLib.Lerp(current_position, target_position, lerp_factor);
                        
                        position_changed = true;
                    }

                    // 🔹 ФИНАЛЬНАЯ ПРОВЕРКА (чтобы не ушло за камеру)
                    if (position_changed && !is_rotating)
                    {
                        dvec3 final_offset = final_position - player_pos;
                        dvec3 camera_forward = player.ViewDirection;
                        double dot_product = MathLib.Dot(final_offset.Normalized, camera_forward);
                        
                        if (dot_product < 0.1)
                        {
                            final_position = player_pos + camera_forward * MinDistance;
                            current_grab_distance = MinDistance;
                        }
                        
                        grabbed_node.WorldPosition = final_position;
                    }
                }
                else if (!is_button_held && grabbed_movable_object != null)
                {
                    ReleaseGrab();
                }

                // 🔹 Захват объекта (Raycast)
                if (is_button_held && grabbed_movable_object == null)
                {
                    Unigine.Object hit_object = World.GetIntersection(
                        player_pos,
                        ray_end,
                        InteractionMask,
                        world_intersection
                    );

                    if (hit_object != null)
                    {
                        Node hit_node = hit_object;
                        MovableObject target_movable = FindMovableComponent(hit_node);
                        
                        if (target_movable != null && target_movable.CanBeMoved)
                        {
                            grabbed_movable_object = target_movable;
                            grab_point_world = world_intersection.Point;
                            grab_offset = grabbed_node.WorldPosition - grab_point_world;
                            current_grab_distance = MathLib.Length(grabbed_node.WorldPosition - player_pos);
                            
                            if (DebugMode)
                                Log.Message("✅ [MovingAroundTheScreen] GRABBED: {0}, distance: {1:F2}\n", grabbed_node.Name, current_grab_distance);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Log.Message("❌ [MovingAroundTheScreen] Update() ERROR: " + e.Message + "\n");
                ReleaseGrab();
            }
        }

        // 🔹 Сброс захвата
        private void ReleaseGrab()
        {
            if (grabbed_movable_object != null)
            {
                grabbed_movable_object = null;
                grab_offset = dvec3.ZERO;
                grab_point_world = dvec3.ZERO;
                current_grab_distance = 0.0;
                _useFixedMousePos = false;
            }
            
            // 🔹 Возвращаем ноду в мир после вращения
            RestoreNodeToWorld();
        }

        private void ReparentToCamera(Node node, Node cameraNode)
        {
            if (node == null || cameraNode == null) return;
            
            // Сохраняем текущие мировые трансформы
            _originalParent = node.Parent;
            _originalWorldPosition = node.WorldPosition;
            _originalWorldRotation = node.GetWorldRotation();
            
            // 🔹 Делаем ноду дочерней к камере через AddWorldChild
            // Это сохраняет мировые координаты при прикреплении
            cameraNode.AddWorldChild(node);
            
            _isReparented = true;
            
            if (DebugMode)
                Log.Message($"🔗 [MovingAroundTheScreen] Reparented to: {cameraNode.Name}\n");
        }

        // 🔹 Возврат ноды в мир
        private void RestoreNodeToWorld()
        {
            if (grabbed_node == null || !_isReparented) return;
            
            // Сохраняем текущие мировые трансформы перед отцепкой
            dvec3 worldPos = grabbed_node.WorldPosition;
            quat worldRot = grabbed_node.GetWorldRotation();
            
            // Возвращаем к оригинальному родителю
            if (_originalParent != null)
            {
                grabbed_node.AddWorldChild(_originalParent);
            }
            else
            {
                // Если оригинального родителя не было — просто отцепляем
                grabbed_node.Parent = null;
            }
            
            // Восстанавливаем мировые трансформы
            grabbed_node.WorldPosition = worldPos;
            grabbed_node.SetWorldRotation(worldRot);
            
            _originalParent = null;
            _originalWorldPosition = dvec3.ZERO;
            _originalWorldRotation = quat.IDENTITY;
            _isReparented = false;
            
            if (DebugMode)
                Log.Message("🔓 [MovingAroundTheScreen] Restored to world\n");
        }

        // ==================== ПОИСК КОМПОНЕНТА ====================
        private MovableObject FindMovableComponent(Node start_node)
        {
            if (start_node == null) return null;
            
            Node current = start_node;
            
            while (current != null)
            {
                MovableObject movable = current.GetComponent<MovableObject>();
                if (movable != null)
                    return movable;
                current = current.Parent;
            }
            
            return null;
        }

        // ==================== SHUTDOWN ====================
        void Shutdown()
        {
            ReleaseGrab();
            if (DebugMode) Log.Message("🔹 [MovingAroundTheScreen] Shutdown()\n");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unigine;
using UnigineApp.data.Code.Annotation.Arrow;
using UnigineApp.data.Code.Auxiliary;
using UnigineApp.data.Code.Light;
using UnigineApp.data.CODE;

namespace UnigineApp.data.Code.Annotation._2DDrawing
{
    internal class DrawingScen
    {
        Intersection intersection = new Intersection();

        AuxiliaryPlane auxiliaryPlane = new AuxiliaryPlane();

        DisplayAnnotationSetting displayAnnotationSetting = new DisplayAnnotationSetting();

        LockedAxlesClass lockedAxlesClass = new LockedAxlesClass();
        public void CreateDrawingScen()
        {

            if (!Storage.AuxiliaryPlane_1_Bool)
            {
                if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT))
                {

                    var (obj, XYZ) = intersection.GetIntersection();

                    auxiliaryPlane.DeleteAuxiliaryPlane();

                    var PositionCamera = Game.Player.Position;
                    dvec3 direction = XYZ - PositionCamera;
                    dvec3 normal = MathLib.Normalize(direction);
                    dmat4 baseRotation = MathLib.LookAt(new dvec3(0, 0, 0), normal, new vec3(0, 1, 0));

                    auxiliaryPlane.CreatAuxiliaryPlane(XYZ, (quat)baseRotation);

                    var sizeWindow = WindowManager.MainWindow.Size;

                    ObjectGui GUI = new ObjectGui(1f, 1f);
                    GUI.Billboard = true;
                    GUI.Background = false;
                    GUI.SetIntersection(true, 0);

                    WidgetSprite widgetSprite = new WidgetSprite();
                    widgetSprite.Texture = Storage.texturePath;

                    Image image = new Image();
                    image.Load(Storage.texturePath);

                    int width = image.Width;
                    int height = image.Height;

                    widgetSprite.Width = 1920;
                    widgetSprite.Height = 1080;

                    Storage.sizePng = new vec2(width / 450, height / 450);

                    GUI.GetGui().AddChild(widgetSprite, Gui.ALIGN_OVERLAP);

                    //GUI.Transform = MathLib.Translate(new vec3(0));
                    GUI.SetPhysicalSize(0.50f, 0.50f);
                    GUI.WorldPosition = XYZ;

                    Storage.IDGui = GUI.ID;

                    Storage.AuxiliaryPlane_1_Bool = true;
                    Storage.PrimitivXY = new vec3(XYZ);
                }
            }
            else
            {

                vec3 start = Storage.PrimitivXY;
                var end = intersection.GetIntersectionList();
                end = lockedAxlesClass.LockedAxles(start, (vec3)end);

                var min = MathLib.Min(start, end);
                var max = MathLib.Max(start, end);
                var size = new vec3(max - min);

                var Node = World.GetNodeByID(Storage.IDGui) as ObjectGui;
                Node.SetPhysicalSize(MathLib.Max(size) * Storage.sizePng.x, MathLib.Max(size) * Storage.sizePng.y);

                if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT))
                {
                    Storage.IDGui = 0;
                    auxiliaryPlane.Zeroing();

                }
            }

        }
    }
}
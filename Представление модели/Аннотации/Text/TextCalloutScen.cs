using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Unigine;
using UnigineApp.data.Code.Annotation.Arrow;
using UnigineApp.data.Code.Auxiliary;
using UnigineApp.data.CODE;

namespace UnigineApp.data.Code.Annotation.Text
{
    internal class TextCalloutScen
    {

        public void CreateTextScen() 
        {

            Intersection intersection = new Intersection();

            AuxiliaryPlane auxiliaryPlane = new AuxiliaryPlane();

            DisplayAnnotationSetting displayAnnotationSetting = new DisplayAnnotationSetting();

            LockedAxlesClass lockedAxlesClass = new LockedAxlesClass();

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

                    WidgetVBox becraund = new WidgetVBox();
                    becraund.Background = 1;
                    becraund.BackgroundColor = vec4.WHITE;
  
                    becraund.SetSpace(1, 1);
                    becraund.Background = 0;

                    GUI.GetGui().AddChild(becraund, Gui.ALIGN_OVERLAP);

                    WidgetLabel inputs = new WidgetLabel();
                    inputs.TextAlign = Gui.ALIGN_CENTER;

                    if (obj)
                    {
                        if (obj.Name != "AuxiliaryPlane_okEn9HiQJNUyo6LuRU8Rcc" &&
                            obj.Name != "TextMaingT5DxmxyMif9tnZkewPlIV" &&
                            obj.Name != "TextT5DxmxyMif9tnZkewPlIV" &&
                            obj.Name != "ArrowMaingT5DxmxyMif9tnZkewPlIV" &&
                            obj.Name != "ArrowT5DxmxyMif9tnZkewPlIV" &&
                            obj.Name != "RulerMain_zMznQZaD1or5hPdCVUVNDn" &&
                            obj.Name != "Ruler_zMznQZaD1or5hPdCVUVNDn")

                        {
                            inputs.Text = obj.Name;
                        }
                        else
                        {
                            inputs.Text = "Текст";
                        }

                    }
                    else
                    {
                        inputs.Text = "Текст";
                    }

                    inputs.FontColor = vec4.BLACK;

                    inputs.FontSize = 300;
                    //inputs.IntersectionEnabled = true;
                    //inputs.SetToolTip("Sdf");

                    int pixelWidth = GUI.GetGui().Width;
                    int pixelHeight = GUI.GetGui().Height;
                    inputs.Width = pixelWidth;
                    inputs.Height = pixelHeight;


                    //
                    becraund.AddChild(inputs, Gui.ALIGN_CENTER | Gui.ALIGN_EXPAND);
                    //GUI.Transform = MathLib.Translate(new vec3(0));
                    GUI.SetPhysicalSize(0.50f, 0.25f);
      

                    Storage.IDGui = GUI.ID;

                    Storage.AuxiliaryPlane_1_Bool = true;
                    Storage.PrimitivXY = new vec3(XYZ);
                }
            }
            else
            {
                vec3 start = Storage.PrimitivXY;
                var (obj, end) = intersection.GetIntersection();

                end = lockedAxlesClass.LockedAxles(start, (vec3)end);

                //var Line_1 = new vec3(end.x - 0.1f, end.y - 0.1f, end.z);
                var playerPos = Game.Player.Position;
                var Line_1 = (vec3)end;

                // По оси X
                if (end.x > playerPos.x)
                    Line_1.x -= 0.1f;
                else if (end.x < playerPos.x)
                    Line_1.x += 0.1f;

                // По оси Y
                if (end.y > playerPos.y)
                    Line_1.y -= 0.1f;
                else if (end.y < playerPos.y)
                    Line_1.y += 0.1f;

                var Line_2 = (vec3)end;

                displayAnnotationSetting.DrawArrow((vec3)start, Line_1, 6, 4, 1, vec4.BLACK);

                displayAnnotationSetting.DrawArrow(Line_1, Line_2, 6, 4, 1, vec4.BLACK);

                //Visualizer.RenderLine3D(start, new vec3(Line_1), vec4.BLACK);
                //Visualizer.RenderLine3D(Line_1, new vec3(Line_2), vec4.BLACK);
                var NodeGui = World.GetNodeByID(Storage.IDGui);

                NodeGui.WorldTransform = MathLib.Translate(end);


                if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT))
                {


                    Mesh mesh = new Mesh();
                    mesh.AddBoxSurface("mesh1", new vec3(0.04f));
                    var dynamicMesh = new ObjectMeshDynamic(mesh);
                    dynamicMesh.SetIntersection(true, 0);
                    dynamicMesh.SetMaxVisibleDistance(0.0000000001f, 0);
                    dynamicMesh.WorldTransform = MathLib.Translate(start);
                    dynamicMesh.Name = "TextMaingT5DxmxyMif9tnZkewPlIV";

                    Mesh mesh2 = new Mesh();
                    mesh2.AddBoxSurface("mesh2", new vec3(0.04f));
                    var dynamicMesh2 = new ObjectMeshDynamic(mesh);
                    dynamicMesh2.SetIntersection(true, 0);

                    dynamicMesh2.SetMaxVisibleDistance(0.0000000001f, 0);


                    //dynamicMesh2.Name = "Arrow_" + dynamicMesh.ID.ToString();
                    dynamicMesh2.Name = "TextT5DxmxyMif9tnZkewPlIV";
                    dynamicMesh.AddChild(dynamicMesh2);

                    Storage.IDGui = 0;
                    NodeGui.WorldTransform = MathLib.Translate(0);
                    dynamicMesh2.AddChild(NodeGui);

                    dynamicMesh2.WorldTransform = MathLib.Translate(end);

                    auxiliaryPlane.Zeroing();

                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unigine;
using UnigineApp.data.Code.Annotation.Arrow;
using UnigineApp.data.Code.Auxiliary;
using UnigineApp.data.CODE;

namespace UnigineApp.data.Code.Annotation.Text
{
    internal class TextCalloutCamera
    {

        Intersection intersection = new Intersection();

        AuxiliaryPlane auxiliaryPlane = new AuxiliaryPlane();

        DisplayAnnotationSetting displayAnnotationSetting = new DisplayAnnotationSetting();

        LockedAxlesClass lockedAxlesClass = new LockedAxlesClass();

        public void CreateTextCamera()
        {


            if (!Storage.AuxiliaryPlane_1_Bool)
            {
                if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT))
                {

                    var (obj, XYZ) = intersection.GetIntersection();
                    auxiliaryPlane.DeleteAuxiliaryPlane();

                    auxiliaryPlane.CreatAuxiliarySphere_2();

                    var plane = World.GetNodeByName("AuxiliaryPlane_okEn9HiQJNUyo6LuRU8Rcc");

                    Game.Player.AddChild(plane);
                    //plane.WorldTransform = Game.Player.WorldTransform + MathLib.Translate(new vec3(0, 1, 0));

                    var sizeWindow = WindowManager.MainWindow.Size;

                    //ObjectGui GUI = new ObjectGui((float)(sizeWindow.x / 6), (float)(sizeWindow.y / 6));

                    ObjectGui GUI = new ObjectGui(1f, 1f);
                    GUI.Billboard = true;
                    GUI.Background = false;


                    //for (int i = 0; i < GUI.NumSurfaces; i++)
                    //{
                    //    GUI.SetIntersection(true, i);
                    //}

                    //GUI.MouseShow = true;
                    //GUI.MouseMode = 0;
                    WidgetVBox becraund = new WidgetVBox();
                    becraund.Background = 1;
                    becraund.BackgroundColor = vec4.WHITE;
                    //becraund.Width = (int)GUI.PhysicalWidth;
                    //becraund.Height = dialog.Size.y - 20;

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
                    GUI.SetPhysicalSize(0.05f, 0.05f);


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

                var playerPos = Game.Player.Position;
                var Line_1 = end;

                // По оси X
                if (end.x > playerPos.x)
                    Line_1.x -= 0.08f;
                else if (end.x < playerPos.x)
                    Line_1.x += 0.08f;

                // По оси Y
                if (end.y > playerPos.y)
                    Line_1.y -= 0.08f;
                else if (end.y < playerPos.y)
                    Line_1.y += 0.08f;

                //// По оси Z
                //if (end.z > playerPos.z)
                //    Line_1.z -= 0.1f;
                //else if (end.z < playerPos.z)
                //    Line_1.z += 0.1f;


                var Line_2 = end;

                Visualizer.RenderLine3D(start, new vec3(Line_1), vec4.BLACK);
                Visualizer.RenderLine3D(Line_1, new vec3(Line_2), vec4.BLACK);
                var NodeGui = World.GetNodeByID(Storage.IDGui);

                NodeGui.WorldTransform = MathLib.Translate(end);


                if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT))
                {


                    Mesh mesh2 = new Mesh();
                    mesh2.AddBoxSurface("mesh2", new vec3(0.04f));
                    var dynamicMesh2 = new ObjectMeshDynamic(mesh2);
                    dynamicMesh2.SetIntersection(true, 0);

                    dynamicMesh2.SetMaxVisibleDistance(0.0000000001f, 0);

   
                    //dynamicMesh2.Name = "Arrow_" + dynamicMesh.ID.ToString();
                    dynamicMesh2.Name = "TextCameraT5DxmxyMif9tnZkewPlIV";
                    Game.Player.AddChild(dynamicMesh2);

                    Storage.IDGui = 0;
                    NodeGui.WorldTransform = MathLib.Translate(0);
                    dynamicMesh2.AddChild(NodeGui);

                    dynamicMesh2.WorldTransform = MathLib.Translate(end);


                    
                    Storage.TextID.Add(dynamicMesh2.ID);
                    Storage.TextThirdCoordinate.Add(start);


                    auxiliaryPlane.Zeroing();

                }
            }


        }





    }
}

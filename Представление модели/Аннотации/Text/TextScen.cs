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
    internal class TextScen
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

                    //var PositionCamera = Game.Player.Position;
                    //dvec3 direction = XYZ - PositionCamera;
                    //dvec3 normal = MathLib.Normalize(direction);
                    //dmat4 baseRotation = MathLib.LookAt(new dvec3(0, 0, 0), normal, new vec3(0, 1, 0));

                    //auxiliaryPlane.CreatAuxiliaryPlane(XYZ, (quat)baseRotation);

                    var sizeWindow = WindowManager.MainWindow.Size;

                    ObjectGui GUI = new ObjectGui(1f, 1f);
                    GUI.Billboard = true;
                    GUI.Background = false;
                    GUI.SetIntersection(true, 0);

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
                    GUI.WorldPosition = XYZ;

                    if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT))
                    {
                        
                        GUI.Name = "TextIvvjn5NBt4j2RCECE02";


                        auxiliaryPlane.Zeroing();

                    }

                }
            }

        }
    }
}
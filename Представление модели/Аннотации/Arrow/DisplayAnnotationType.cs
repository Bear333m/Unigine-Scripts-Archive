using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unigine;
using UnigineApp.data.Code.Annotation._2DDrawing;
using UnigineApp.data.Code.Annotation.Text;
using UnigineApp.data.Code.Auxiliary;
using UnigineApp.data.Code.Light;
using UnigineApp.data.Code.Primitives;
using UnigineApp.data.CODE;

namespace UnigineApp.data.Code.Annotation.Arrow
{
    internal class DisplayAnnotationType
    {
        Intersection intersection = new Intersection();
        Arrow arrow = new Arrow();
        Ruler ruler = new Ruler();
        TextCalloutScen textCalloutScen = new TextCalloutScen();
        TextCalloutCamera textCalloutCamera = new TextCalloutCamera();
        TextScen textScen = new TextScen(); 
        TextCamera textCamera = new TextCamera();
        DrawingScen drawingScen = new DrawingScen();
        Circle circle = new Circle();
        DrawingCamera drawingCamera = new DrawingCamera();

        public void DisplayType()
        {
            if (!Storage.AuxiliarySphereBool)
            {
                Storage.AuxiliarySphereBool = true;
                AuxiliaryPlane auxiliaryPlane = new AuxiliaryPlane();
                auxiliaryPlane.CreatAuxiliarySphere();

            }
            if (!Storage.AuxiliaryPlane_1_Bool )
            {
                var (obj, XYZ) = intersection.GetIntersection();
                Visualizer.RenderPoint3D(XYZ, 0.005f, vec4.GREEN);

            }
            switch (Storage.AnnotationType)
            {

                case 1: //Arrow
                    
                    arrow.CreateArrow();

                    break;

                case 2: //Arrow

                    arrow.CreateArrow();

                    break;

                case 3: //Arrow

                    arrow.CreateArrow();

                    break;

                case 4: //Arrow

                    arrow.CreateArrow();

                    break;

                case 5: //Ruler

                    ruler.CreateRuler();

                    break;

                case 6: //TextCalloutScen

                    textCalloutScen.CreateTextScen();

                    break;

                case 7: //TextCalloutCamera

                    textCalloutCamera.CreateTextCamera();

                    break;

                case 8: //TextScen

                    textScen.CreateTextScen();

                    break;

                case 9: //TextCamera

                    textCamera.CreateTextCamera();

                    break;

                case 10: //DrawingScen

                    drawingScen.CreateDrawingScen();

                    break;

                case 11: //DrawingCamera

                    drawingCamera.CreateDrawingCamera();

                    break;

                case 12: //DrawingCircle

                    circle.CreateCircleScen();

                    break;
            }

        }


        


    }
}
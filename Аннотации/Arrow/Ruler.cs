using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unigine;
using UnigineApp.data.Code.Auxiliary;
using UnigineApp.data.CODE;

namespace UnigineApp.data.Code.Annotation.Arrow
{
    internal class Ruler
    {
        Intersection intersection = new Intersection();

        AuxiliaryPlane auxiliaryPlane = new AuxiliaryPlane();

        DisplayAnnotationSetting displayAnnotationSetting = new DisplayAnnotationSetting();

        LockedAxlesClass lockedAxlesClass = new LockedAxlesClass();
        public void CreateRuler()
        {
            if (!Storage.AuxiliaryPlane_1_Bool)
            {
                if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT))
                {

                    var (obj, XYZ) = intersection.GetIntersection();
                    //auxiliaryPlane.DeleteAuxiliaryPlane();
                    Storage.AuxiliaryPlane_1_Bool = true;
                    Storage.PrimitivXY = new vec3(XYZ);
                }
            }
            else
            {
                if (!Storage.AuxiliaryPlane_2_Bool)
                {
                    vec3 start = Storage.PrimitivXY;
                    var (obj, end) = intersection.GetIntersection();

                    end = lockedAxlesClass.LockedAxles(start, (vec3)end);

                    //Visualizer.RenderLine3D(start, new vec3(end), vec4.GREEN);


                    //var posNum = new vec3((start.x + end.x) / 2, (start.y + end.y) / 2, (start.z + end.z) / 2 + 0.05f);
                    var sizeNum = MathLib.Round(MathLib.Sqrt(MathLib.Pow2(end.x - start.x) + MathLib.Pow2(end.y - start.y) + MathLib.Pow2(end.z - start.z)), 2);

     
                    Visualizer.RenderMessage3D(end, 0, (sizeNum * 100).ToString() + " мм", vec4.WHITE, 0, 25);

                    displayAnnotationSetting.DrawArrow(start, (vec3)end, 1, 1, 1, vec4.BLACK);



                    if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT))
                    {

                        Storage.PrimitivSizeXYZ = (vec3)end;
                        Storage.AuxiliaryPlane_2_Bool = true;

                    }
                }
                else
                {

                    var (obj, endLast) = intersection.GetIntersection();


                    var a = Storage.PrimitivXY;
                    var b = Storage.PrimitivSizeXYZ;
                    var c = (vec3)endLast;

                    // Направляющий вектор AB
                    vec3 vecAB = b - a;

                    // Прямая проходит через точку c в том же направлении, что и AB
                    vec3 direction = vecAB;

                    // Проверка на нулевой вектор
                    float lenSq = MathLib.Length2(direction);
                    //if (lenSq < 1e-6f)
                    //{
                    //    Log.Error("Вектор AB нулевой. Невозможно построить прямую.\n");
                    //    return;
                    //}

                    // Перпендикуляр из A на прямую через C
                    vec3 vecAC = a - c; // важно: от c к a
                    float tA = MathLib.Dot(vecAC, direction) / lenSq;
                    vec3 start = c + direction * tA;

                    // Перпендикуляр из B на ту же прямую
                    vec3 vecBC = b - c;
                    float tB = MathLib.Dot(vecBC, direction) / lenSq;
                    vec3 end = c + direction * tB;



                    var posNum = new vec3((start.x + end.x) / 2, (start.y + end.y) / 2, (start.z + end.z) / 2 );
                    var sizeNum = MathLib.Round(MathLib.Sqrt(MathLib.Pow2(b.x - a.x) + MathLib.Pow2(b.y - a.y) + MathLib.Pow2(b.z - a.z)), 2);

                    Visualizer.RenderMessage3D(posNum, 0, (sizeNum * 100).ToString() + " мм", vec4.WHITE, 0, 25);

                    displayAnnotationSetting.DrawArrow((vec3)start, (vec3)end, 6, 4, 3, vec4.BLACK);

                    Visualizer.RenderLine3D(a, start, vec4.BLACK);
                    Visualizer.RenderLine3D(b, end, vec4.BLACK);

                    if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT))
                    {


                        Mesh mesh = new Mesh();
                        mesh.AddBoxSurface("mesh1", new vec3(1 * 0.01f));
                        var dynamicMesh = new ObjectMeshDynamic(mesh);
                        dynamicMesh.SetIntersection(true, 0);
                        dynamicMesh.SetMaxVisibleDistance(0.0000000001f, 0);
                        dynamicMesh.WorldTransform = MathLib.Translate(Storage.PrimitivXY);
                        dynamicMesh.Name = "RulerMain_zMznQZaD1or5hPdCVUVNDn";

                        Mesh mesh2 = new Mesh();
                        mesh2.AddBoxSurface("mesh2", new vec3(1 * 0.01f));
                        var dynamicMesh2 = new ObjectMeshDynamic(mesh);
                        dynamicMesh2.SetIntersection(true, 0);

                        dynamicMesh2.SetMaxVisibleDistance(0.0000000001f, 0);

                        Storage.RulerID.Add(dynamicMesh.ID);
                        Storage.RulerThirdCoordinate.Add(c);
           

                        //dynamicMesh2.Name = "Arrow_" + dynamicMesh.ID.ToString();
                        dynamicMesh2.Name = "Ruler_zMznQZaD1or5hPdCVUVNDn";
                        dynamicMesh.AddChild(dynamicMesh2);

                        dynamicMesh2.WorldTransform = MathLib.Translate(Storage.PrimitivSizeXYZ);

                        auxiliaryPlane.Zeroing();

                    }
                }
            }


        }


    }
}

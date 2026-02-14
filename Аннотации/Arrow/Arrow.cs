using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unigine;
using UnigineApp.data.Code.Auxiliary;
using UnigineApp.data.Code.GUI;
using UnigineApp.data.Code.Light;
using UnigineApp.data.CODE;

namespace UnigineApp.data.Code.Annotation.Arrow
{
    internal class Arrow
    {
        Intersection intersection = new Intersection();

        AuxiliaryPlane auxiliaryPlane = new AuxiliaryPlane();

        DisplayAnnotationSetting displayAnnotationSetting = new DisplayAnnotationSetting();

        LockedAxlesClass lockedAxlesClass = new LockedAxlesClass();  
        public void CreateArrow()
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
                vec3 start = Storage.PrimitivXY;
                var (obj, end) = intersection.GetIntersection();

                end = lockedAxlesClass.LockedAxles(start, (vec3)end);

                //Visualizer.RenderLine3D(start, new vec3(end), vec4.GREEN);

                int layers = 10;
                float size = 10;

                //var posNum = new vec3((start.x + end.x) / 2, (start.y + end.y) / 2, (start.z + end.z) / 2 + 0.05f);
                //var sizeNum = MathLib.Round(MathLib.Sqrt(MathLib.Pow2(end.x - start.x) + MathLib.Pow2(end.y - start.y) + MathLib.Pow2(end.z - start.z)), 2);

                //Visualizer.RenderMessage3D(posNum, 0, sizeNum.ToString(), vec4.WHITE);

                displayAnnotationSetting.DrawArrow(start, (vec3)end, size, layers, Storage.AnnotationType, vec4.GREEN);



                if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.LEFT))
                {


                    Mesh mesh = new Mesh();
                    mesh.AddBoxSurface("mesh1", new vec3(size * 0.01f));
                    var dynamicMesh = new ObjectMeshDynamic(mesh);
                    dynamicMesh.SetIntersection(true, 0);
                    dynamicMesh.SetMaxVisibleDistance(0.0000000001f, 0);
                    dynamicMesh.WorldTransform = MathLib.Translate(start);
                    dynamicMesh.Name = "ArrowMaingT5DxmxyMif9tnZkewPlIV";

                    Mesh mesh2 = new Mesh();
                    mesh2.AddBoxSurface("mesh2", new vec3(size * 0.01f));
                    var dynamicMesh2 = new ObjectMeshDynamic(mesh);
                    dynamicMesh2.SetIntersection(true, 0);

                    dynamicMesh2.SetMaxVisibleDistance(0.0000000001f, 0);

                    Storage.ArrowID.Add(dynamicMesh.ID);
                    Storage.ArrowType.Add(Storage.AnnotationType);
                    Storage.ArrowSize.Add(size);
                    Storage.ArrowWidth.Add(layers);
                    Storage.ArrowColor.Add(vec4.GREEN);
                    Storage.ArrowNumbers.Add(false);

                    //dynamicMesh2.Name = "Arrow_" + dynamicMesh.ID.ToString();
                    dynamicMesh2.Name = "ArrowgT5DxmxyMif9tnZkewPlIV";
                    dynamicMesh.AddChild(dynamicMesh2);

                    dynamicMesh2.WorldTransform = MathLib.Translate(end);

                    auxiliaryPlane.Zeroing();

                }
            }


        }



        //public void DrawOffsetLines(vec3 start, vec3 end, float offsetStep, int layers)
        //{
        //    // Основная линия
        //    Visualizer.RenderLine3D(start, new vec3(end), vec4.GREEN);

        //    // Направления смещений (в плоскости XY)
        //    vec3[] baseOffsets = new vec3[]
        //    {
        //        new vec3(1, 0, 0),
        //        new vec3(-1, 0, 0),
        //        new vec3(0, 1, 0),
        //        new vec3(0, -1, 0),
        //        new vec3(1, 1, 0),
        //        new vec3(1, -1, 0),
        //        new vec3(-1, 1, 0),
        //        new vec3(-1, -1, 0),
        //    };

        //    // Множители смещения для каждого слоя
        //    for (int i = 1; i <= layers; i++)
        //    {
        //        float currentOffset = offsetStep * i;
        //        foreach (var direction in baseOffsets)
        //        {
        //            vec3 offset = direction * currentOffset;
        //            vec3 newStart = start + offset;
        //            vec3 newEnd = new vec3(end) + offset;
        //            Visualizer.RenderLine3D(newStart, newEnd, vec4.GREEN);
        //        }
        //    }
        //}

        //public void DrawOffsetLines(vec3 start, vec3 end, float offsetStep, int layers)
        //{
        //    // Основная линия
        //    Visualizer.RenderLine3D(start, new vec3(end), vec4.GREEN);

        //    // Проход по квадратной сетке вокруг центра
        //    for (int x = -layers; x <= layers; x++)
        //    {
        //        for (int y = -layers; y <= layers; y++)
        //        {
        //            // Пропускаем центр (основную линию)
        //            if (x == 0 && y == 0)
        //                continue;

        //            vec3 offset = new vec3(x * offsetStep, y * offsetStep, 0);
        //            vec3 newStart = start + offset;
        //            vec3 newEnd = new vec3(end) + offset;
        //            Visualizer.RenderLine3D(newStart, newEnd, vec4.GREEN);
        //        }
        //    }
        //}





    }
}

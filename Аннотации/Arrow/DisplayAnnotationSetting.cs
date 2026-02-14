using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unigine;
using UnigineApp.data.Code.Auxiliary;
using static Unigine.Animations;

namespace UnigineApp.data.Code.Annotation.Arrow
{
    internal class DisplayAnnotationSetting
    {

        public void DisplayArrow()
        {
            List<Node> nodes = new List<Node>();
            World.GetNodesByName("ArrowMaingT5DxmxyMif9tnZkewPlIV", nodes);

            foreach (Node node in nodes)
            {
                var start = node.WorldPosition;
                var end = node.GetChild(0).WorldPosition;

                var Index = Storage.ArrowID.IndexOf(node.ID);

                if (Storage.ArrowNumbers[Index])
                {
                    var posNum = new vec3((start.x + end.x) / 2, (start.y + end.y) / 2, (start.z + end.z) / 2 + 0.05f);
                    var sizeNum = MathLib.Round(MathLib.Sqrt(MathLib.Pow2(end.x - start.x) + MathLib.Pow2(end.y - start.y) + MathLib.Pow2(end.z - start.z)), 2);

                    Visualizer.RenderMessage3D(posNum, 0, (sizeNum * 100).ToString() + " мм", vec4.WHITE, 0, 200);
                }

                DrawArrow((vec3)start, (vec3)end, Storage.ArrowSize[Index], Storage.ArrowWidth[Index], Storage.ArrowType[Index], Storage.ArrowColor[Index]);
            }

            World.GetNodesByName("TextMaingT5DxmxyMif9tnZkewPlIV", nodes);

            foreach (Node node in nodes)
            {
                var start = node.WorldPosition;
                var end = node.GetChild(0).WorldPosition;

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

                //Visualizer.RenderLine3D(start, new vec3(Line_1), vec4.BLACK);
                //Visualizer.RenderLine3D(Line_1, new vec3(Line_2), vec4.BLACK);

                DrawArrow((vec3)start, Line_1, 6, 4, 1, vec4.BLACK);

                DrawArrow(Line_1, Line_2, 6, 4, 1, vec4.BLACK);


            }

            World.GetNodesByName("TextCameraT5DxmxyMif9tnZkewPlIV", nodes);

            foreach (Node node in nodes)
            {
                var end = node.WorldPosition;
                //var end = node.GetChild(0).WorldPosition;
                var index = Storage.TextID.IndexOf(node.ID);
                var start = Storage.TextThirdCoordinate[index];

                var playerPos = Game.Player.Position;
                var Line_1 = (vec3)end;

                // По оси X
                if (end.x > playerPos.x)
                    Line_1.x -= 0.07f;
                else if (end.x < playerPos.x)
                    Line_1.x += 0.07f;

                // По оси Y
                if (end.y > playerPos.y)
                    Line_1.y -= 0.07f;
                else if (end.y < playerPos.y)
                    Line_1.y += 0.07f;

                var Line_2 = (vec3)end;

                Visualizer.RenderLine3D(start, new vec3(Line_1), vec4.BLACK);
                Visualizer.RenderLine3D(Line_1, new vec3(Line_2), vec4.BLACK);


            }

            World.GetNodesByName("RulerMain_zMznQZaD1or5hPdCVUVNDn", nodes);

            foreach (Node node in nodes)
            {
                var a = (vec3)node.WorldPosition;
                var b = (vec3)node.GetChild(0).WorldPosition;

                var Index = Storage.RulerID.IndexOf(node.ID);

                var c = Storage.RulerThirdCoordinate[Index];

                vec3 vecAB = b - a;

                vec3 direction = vecAB;

                float lenSq = MathLib.Length2(direction);
   
                vec3 vecAC = a - c; // важно: от c к a
                float tA = MathLib.Dot(vecAC, direction) / lenSq;
                vec3 start = c + direction * tA;

                // Перпендикуляр из B на ту же прямую
                vec3 vecBC = b - c;
                float tB = MathLib.Dot(vecBC, direction) / lenSq;
                vec3 end = c + direction * tB;



                var posNum = new vec3((start.x + end.x) / 2, (start.y + end.y) / 2, (start.z + end.z) / 2);
                var sizeNum = MathLib.Round(MathLib.Sqrt(MathLib.Pow2(b.x - a.x) + MathLib.Pow2(b.y - a.y) + MathLib.Pow2(b.z - a.z)), 2);

                Visualizer.RenderMessage3D(posNum, 0, (sizeNum * 100).ToString() + " мм", vec4.WHITE, 0, 25);

                DrawArrow((vec3)start, (vec3)end, 6, 4, 3, vec4.BLACK);

                Visualizer.RenderLine3D(a, start, vec4.BLACK);
                Visualizer.RenderLine3D(b, end, vec4.BLACK);

            }
        }

        
        public void DrawArrow(vec3 start, vec3 end, float size, int layers, int type, vec4 color)
        {
            vec3 result = 0; vec3 result2 = 0;
            vec3 direction = MathLib.Normalize(start - (vec3)end);

            switch (type)
            {
                case 1: //ArrowLine
                    size = 0f;

                    break;

                case 2: //ArrowVector
                    
                    result = (vec3)end + direction * 0.01f;
                    Visualizer.RenderVector(result, new vec3(end), color, size);
                    break;

                case 3: //ArrowVector2
                   
                    result = (vec3)end + direction * 0.01f;
                    result2 = start - direction * 0.01f;

                    Visualizer.RenderVector(result, new vec3(end), color, size);
                    Visualizer.RenderVector(result2, new vec3(start), color, size);
                    break;

                case 4: //ArrowVector2Inverted
                  
                    result = (vec3)end - direction * 0.01f;
                    result2 = start + direction * 0.01f;

                    Visualizer.RenderVector(result, new vec3(end), color, size);
                    Visualizer.RenderVector(result2, new vec3(start), color, size);
                    break;
            }


            DrawOffsetLines(start, end, 0.0003f, layers, size * 0.01f, type, color);
        }
        private void DrawOffsetLines(vec3 start, vec3 end, float offsetStep, int layers, float shortenBy, int type, vec4 color )
        {
            vec3 direction = MathLib.Normalize(end - start);
            float fullLength = (end - start).Length;
            float shortenedLength = fullLength;
            float shortenStart = 0f;
            float shortenEnd = 0f;
            int segmentsPerCircle = 32;
            switch (type)
            {
                case 1: 
                    shortenStart = 0f;
                    shortenEnd = 0f;
                    break;

                case 2: // Arrow
                    shortenStart = 0f;
                    shortenEnd = shortenBy;
                    break;

                case 3: // Arrow2
                    shortenStart = shortenBy;
                    shortenEnd = shortenBy;
                    break;

                case 4: // Arrow2inverted
                    shortenStart = -shortenBy;
                    shortenEnd = -shortenBy;
                    break;
            }

            shortenedLength = MathF.Max(0f, fullLength - shortenStart - shortenEnd);

            // Смещённый старт и конец
            vec3 finalStart = start + direction * shortenStart;
            vec3 finalEnd = finalStart + direction * shortenedLength;

            // Основная линия
            Visualizer.RenderLine3D(finalStart, finalEnd, color);

            // Окружности
            for (int i = 1; i <= layers; i++)
            {
                float radius = offsetStep * i;

                for (int s = 0; s < segmentsPerCircle; s++)
                {
                    float angle = (2 * MathF.PI / segmentsPerCircle) * s;

                    float offsetX = MathF.Cos(angle) * radius;
                    float offsetY = MathF.Sin(angle) * radius;

                    vec3 offset = new vec3(offsetX, offsetY, 0);
                    vec3 ringStart = start + offset + direction * shortenStart;
                    vec3 ringEnd = ringStart + direction * shortenedLength;

                    Visualizer.RenderLine3D(ringStart, ringEnd, color);
                }
            }
        }

    }
}

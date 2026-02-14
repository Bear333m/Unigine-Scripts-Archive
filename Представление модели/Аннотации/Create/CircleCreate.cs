using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unigine;

namespace UnigineApp.data.Code.Annotation._2DDrawing
{
    internal class CircleCreate
    {
        public void Circle(vec3 Start, float size) 
        {

            ObjectGui GUI = new ObjectGui(1f, 1f);
            GUI.Billboard = true;
            GUI.Background = false;
            GUI.SetIntersection(true, 0);

            WidgetSprite widgetSprite = new WidgetSprite();
            widgetSprite.Texture = "../data/UI/icon/Circle.png";
            widgetSprite.Width = 1920;
            widgetSprite.Height = 1080;

            GUI.GetGui().AddChild(widgetSprite, Gui.ALIGN_OVERLAP);

            //GUI.Transform = MathLib.Translate(new vec3(0));
            GUI.SetPhysicalSize(size * 2, size * 2);
            GUI.WorldPosition = Start;
        }
    }
}

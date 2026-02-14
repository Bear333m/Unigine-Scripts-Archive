using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unigine;

namespace UnigineApp.data.Code.Annotation.Text
{
    internal class TextScenCreate
    {
        public void TextCamera(vec3 start, string text, int size, vec4 color, int background, vec4 backgroundColor)
        {
            ObjectGui GUI = new ObjectGui(1f, 1f);
            GUI.Billboard = true;
            GUI.Background = false;
            GUI.SetIntersection(true, 0);

            WidgetVBox becraund = new WidgetVBox();
            becraund.BackgroundColor = backgroundColor;


            becraund.SetSpace(1, 1);
            becraund.Background = background;

            GUI.GetGui().AddChild(becraund, Gui.ALIGN_OVERLAP);

            WidgetLabel inputs = new WidgetLabel();
            inputs.TextAlign = Gui.ALIGN_CENTER;

            inputs.FontColor = color;
            inputs.Text = text;
            inputs.FontSize = size;

            int pixelWidth = GUI.GetGui().Width;
            int pixelHeight = GUI.GetGui().Height;
            inputs.Width = pixelWidth;
            inputs.Height = pixelHeight;


            becraund.AddChild(inputs, Gui.ALIGN_CENTER | Gui.ALIGN_EXPAND);
            //GUI.Transform = MathLib.Translate(new vec3(0));
            GUI.SetPhysicalSize(0.05f, 0.05f);
            GUI.WorldPosition = start;

            GUI.Name = "TextIvvjn5NBt4j2RCECE02";

        }
    }
}

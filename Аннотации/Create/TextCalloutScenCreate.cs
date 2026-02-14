using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unigine;
using UnigineApp.data.Code.Auxiliary;

namespace UnigineApp.data.Code.Annotation.Text
{
    internal class TextCalloutScenCreate
    {

        public void TextCalloutScen(vec3 start, vec3 end, string text, int size, vec4 color, int background, vec4 backgroundColor)
        {
            ObjectGui GUI = new ObjectGui(1f, 1f);
            GUI.Billboard = true;
            GUI.Background = false;

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


            GUI.WorldTransform = MathLib.Translate(0);
            dynamicMesh2.AddChild(GUI);

            dynamicMesh2.WorldTransform = MathLib.Translate(end);




        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unigine;
using UnigineApp.data.Code.Annotation.GUI;
using UnigineApp.data.Code.Auxiliary;
using UnigineApp.data.Code.GUI;

namespace UnigineApp.data.Code.Annotation.Arrow
{
    internal class AnnotationProcessing
    {

        public void ProcessingForDialog(Node node)
        {

            string nodeName = node?.Name ?? "";

            string[] parts = nodeName.Split('_');

            string firstPart = parts.Length > 0 ? parts[0] : "";
            string secondPart = parts.Length > 1 ? parts[1] : "";


            if(firstPart == "ArrowgT5DxmxyMif9tnZkewPlIV")
            {
                node = node.Parent;
                nodeName = node?.Name ?? "";
                parts = nodeName.Split('_');

                firstPart = parts.Length > 0 ? parts[0] : "";
                secondPart = parts.Length > 1 ? parts[1] : "";
            }


            if (firstPart == "ArrowMaingT5DxmxyMif9tnZkewPlIV")
            {
              

                //if (!string.IsNullOrEmpty(secondPart) && int.TryParse(secondPart, out int windowId))
                //{
                //    dialog = WindowManager.GetWindowByID((ulong)int.Parse(secondPart)) as EngineWindowViewport;
                //    if (dialog) { dialog.Show(); }

                //}

                //node.Name = firstPart;
              
                    GUIAddArrowSetting gUIAddArrowSetting = new GUIAddArrowSetting();

                    var Index = Storage.ArrowID.IndexOf(node.ID);

                    gUIAddArrowSetting.AddArrow(
                        Index, Storage.ArrowType[Index], Storage.ArrowSize[Index], Storage.ArrowWidth[Index], Storage.ArrowNumbers[Index], Storage.ArrowColor[Index], node
                     );
                
            }

            if (firstPart == "TextT5DxmxyMif9tnZkewPlIV")
            {
                node = node.Parent;
                nodeName = node?.Name ?? "";
                parts = nodeName.Split('_');

                firstPart = parts.Length > 0 ? parts[0] : "";
                secondPart = parts.Length > 1 ? parts[1] : "";
            }


            if (firstPart == "TextMaingT5DxmxyMif9tnZkewPlIV")
            {

                var childrenNode1 = node.GetChild(0);
                var objectGui = childrenNode1.GetChild(0) as ObjectGui;
                var becraund = objectGui.GetGui().GetChild(0) as WidgetVBox;
                var widgetEditText = becraund.GetChild(0) as WidgetLabel;

                GUIAddTextSetting gUIAddTextSetting = new GUIAddTextSetting();

                gUIAddTextSetting.AddText(widgetEditText.Text, widgetEditText.FontSize, widgetEditText, becraund);

                //var mainNode = World.GetNodeByName(node.Name);

            }

            if (firstPart == "TextIvvjn5NBt4j2RCECE02")
            {

                var objectGui = node as ObjectGui;
                var becraund = objectGui.GetGui().GetChild(0) as WidgetVBox;
                var widgetEditText = becraund.GetChild(0) as WidgetLabel;

                GUIAddTextSetting gUIAddTextSetting = new GUIAddTextSetting();

                gUIAddTextSetting.AddText(widgetEditText.Text, widgetEditText.FontSize, widgetEditText, becraund);

                //var mainNode = World.GetNodeByName(node.Name);

            }

            if (firstPart == "TextCameraT5DxmxyMif9tnZkewPlIV")
            {

   
                var objectGui = node.GetChild(0) as ObjectGui;
                var becraund = objectGui.GetGui().GetChild(0) as WidgetVBox;
                var widgetEditText = becraund.GetChild(0) as WidgetLabel;

                GUIAddTextSetting gUIAddTextSetting = new GUIAddTextSetting();

                gUIAddTextSetting.AddText(widgetEditText.Text, widgetEditText.FontSize, widgetEditText, becraund);

                //var mainNode = World.GetNodeByName(node.Name);

            }

        }
    }
}

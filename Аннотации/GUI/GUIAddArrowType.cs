using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unigine;
using UnigineApp.data.Code.Auxiliary;

namespace UnigineApp.data.Code.Annotation.GUI
{
    internal class GUIAddArrow
    {
        EventConnections connections = new EventConnections();
        UpdateTime updateTime = new UpdateTime();
        IconAnimation iconAnimation = new IconAnimation();
        int iconSize = 80;
        int spacing = 5;

        string[] NameMaterial = ["Arrow0", "Arrow1", "Arrow2", "Arrow3"];
        public void Arrow()
        {
            //var GUI = Gui.GetCurrent();
            var dialog = new EngineWindowViewport("Материалы", 600, 700);

            WidgetScrollBox widgetScrollBox = new WidgetScrollBox();
            widgetScrollBox.Width = dialog.Size.x;
            widgetScrollBox.Height = dialog.Size.y - 20;
            widgetScrollBox.Background = 1;
            widgetScrollBox.BackgroundColor = vec4.WHITE;


            dialog.AddChild(widgetScrollBox);

            // Размер иконки + отступы

            int iconFullSize = iconSize + spacing;

            // Расчёт количества колонок на основе ширины окна
            int numColumns = Math.Max(1, (int)(dialog.Size.x / iconFullSize));

            WidgetGridBox widgetGrid = new WidgetGridBox(dialog.Gui, numColumns, spacing, spacing);

            updateTime.StarttMonitoringGridSize(dialog, widgetScrollBox, widgetGrid);

            foreach (var name in NameMaterial)
            {
                WidgetSprite widgetSprite = new WidgetSprite(dialog.Gui);
                widgetSprite.Texture = "../data/UI/icon/Arrow_icon/" + name + ".png";
                widgetSprite.Width = iconSize;
                widgetSprite.Height = iconSize;
                iconAnimation.IconEvent(widgetSprite);

                widgetGrid.AddChild(widgetSprite);
            }

            widgetScrollBox.AddChild(widgetGrid);

            widgetGrid.GetChild(0).EventClicked.Connect(() =>
            {
                Storage.AnnotationType = 1;
            });
            widgetGrid.GetChild(1).EventClicked.Connect(() =>
            {
                Storage.AnnotationType = 2;
            });
            widgetGrid.GetChild(2).EventClicked.Connect(() =>
            {
                Storage.AnnotationType = 3;
            });
            widgetGrid.GetChild(3).EventClicked.Connect(() =>
            {
                Storage.AnnotationType = 4;
            });
            dialog.Show();


        }



    }
}
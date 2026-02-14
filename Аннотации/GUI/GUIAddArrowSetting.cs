using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unigine;
using UnigineApp.data.Code.Auxiliary;
using UnigineApp.data.Code.GUI;

namespace UnigineApp.data.Code.Annotation.GUI
{
    internal class GUIAddArrowSetting
    {
        UpdateTime updateTime = new UpdateTime();

        IconAnimation iconAnimation = new IconAnimation();

        int iconSize = 60;
        int spacing = 4;

        string[] NameMaterial = ["Arrow0", "Arrow1", "Arrow2", "Arrow3"];

        public void AddArrow(int Index, int ArrowType, float ArrowSize, int ArrowWidth, bool ArrowNumbers, vec4 ArrowColor, Node node)
        {
            //var GUI = Gui.GetCurrent();
            var dialog = new EngineWindowViewport("Стрелка", 300, 700);



            WidgetScrollBox widgetScrollBox = new WidgetScrollBox();
            widgetScrollBox.Width = dialog.Size.x;
            widgetScrollBox.Height = dialog.Size.y - 20;
            widgetScrollBox.Background = 1;
            widgetScrollBox.BackgroundColor = vec4.WHITE;


            dialog.AddChild(widgetScrollBox);


            int iconFullSize = iconSize + spacing;

            // Расчёт количества колонок на основе ширины окна
            int numColumns = Math.Max(1, (int)(dialog.Size.x / iconFullSize));

            WidgetGridBox widgetGrid = new WidgetGridBox(dialog.Gui, numColumns, spacing, spacing);

            updateTime.StarttMonitoringGridSize(dialog, widgetScrollBox, widgetGrid);

            foreach (var name in NameMaterial)
            {
                WidgetSprite widgetSprite = new WidgetSprite(dialog.Gui);
                widgetSprite.Texture = "../data/UI/icon/Arrow_icon/" + name + ".png";
                widgetSprite.SetToolTip(name);
                widgetSprite.Width = iconSize;
                widgetSprite.Height = iconSize;
                iconAnimation.IconEvent(widgetSprite);

                widgetGrid.AddChild(widgetSprite);
            }

            widgetScrollBox.AddChild(widgetGrid);



            // Массивы для полей ввода и подписей
            WidgetEditLine[] inputs = new WidgetEditLine[2];
            WidgetLabel[] labels = new WidgetLabel[2];

            // Имена меток
            string[] labelNames = new string[] { "Размер стрелки", "Толщина линии" };

            // Начальные значения для полей ввода
            string[] initialValues = new string[] { ArrowSize.ToString(), ArrowWidth.ToString() };

            // Инициализируем поля ввода и задаем начальные значения
            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = new WidgetEditLine(dialog.Gui);
                inputs[i].Width = 75;
                inputs[i].Height = 20;
                inputs[i].Text = initialValues[i]; // Устанавливаем начальное значение
            }

            // Инициализируем подписи
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i] = new WidgetLabel(dialog.Gui, labelNames[i]);
                labels[i].Width = 75;
                labels[i].Height = 20;
            }

            WidgetButton colorButton = new WidgetButton(dialog.Gui, "Выбор цвета");
            WidgetButton NumbersButton = new WidgetButton(dialog.Gui, "Отображение чисел");
            WidgetButton changeButton = new WidgetButton(dialog.Gui, "Изменить направление");
            //WidgetButton ADD = new WidgetButton(GUI, "Добавить");

            // Функция для создания строки с выравниванием по краям
            WidgetHBox CreateInputRow(WidgetLabel label, WidgetEditLine input)
            {
                WidgetHBox row = new WidgetHBox(dialog.Gui);
                row.AddChild(label, Gui.ALIGN_LEFT);
                row.AddChild(input, Gui.ALIGN_RIGHT);
                return row;
            }


            // Добавляем строки для каждого поля и подписи
            for (int i = 0; i < inputs.Length; i++)
            {
                widgetScrollBox.AddChild(CreateInputRow(labels[i], inputs[i]), Gui.ALIGN_EXPAND);
            }

            // Добавляем элементы в окно
            widgetScrollBox.AddChild(colorButton, Gui.ALIGN_EXPAND);
            widgetScrollBox.AddChild(NumbersButton, Gui.ALIGN_EXPAND);
            widgetScrollBox.AddChild(changeButton, Gui.ALIGN_EXPAND);

            widgetGrid.GetChild(0).EventClicked.Connect(() =>
            {
                Storage.ArrowType[Index] = 1;
            });
            widgetGrid.GetChild(1).EventClicked.Connect(() =>
            {
                Storage.ArrowType[Index] = 2;
            });
            widgetGrid.GetChild(2).EventClicked.Connect(() =>
            {
                Storage.ArrowType[Index] = 3;
            });
            widgetGrid.GetChild(3).EventClicked.Connect(() =>
            {
                Storage.ArrowType[Index] = 4;
            });

            // Обработчик выбора цвета
            colorButton.EventClicked.Connect(async () => {
                GUIAddColor gUIAddColor = new GUIAddColor();
                var Colar = await gUIAddColor.Color();
                Storage.ArrowColor[Index] = Colar;
            });

            NumbersButton.EventClicked.Connect(() => {
                Storage.ArrowNumbers[Index] = !Storage.ArrowNumbers[Index];
            });

            changeButton.EventClicked.Connect(() => {
                var nodeСhild = node.GetChild(0);
                dvec3 positionСhild = nodeСhild.WorldPosition;
                dvec3 positionParent = node.WorldPosition;

                node.WorldTransform = MathLib.Translate(positionСhild);
                nodeСhild.WorldTransform = MathLib.Translate(positionParent);

            });

            // Обработчики изменений значений
            for (int i = 0; i < inputs.Length; i++)
            {
                int index2 = i; // Локальная переменная для замыкания
                inputs[i].EventKeyPressed.Connect(() => {
                    if (float.TryParse(inputs[index2].Text, out float value))
                    {

                        switch (index2)
                        {
                            case 0: // Размер стрелки
                                Storage.ArrowSize[Index] = value;
       

                                break;

                            case 1: // Толщина линии
                                Storage.ArrowWidth[Index] = (int)value;

                                break;

                        }
                       
                    }
                });
            }




            updateTime.StarttMonitoringGridSize(dialog, widgetScrollBox, widgetGrid);

            //var dynamicMesh = World.GetNodeByName("Omni_" + omni.ID.ToString());
            //dynamicMesh.Name = "Omni_" + omni.ID.ToString() + "_" + dialog.ID.ToString();

            dialog.Show();
        }


    }
}
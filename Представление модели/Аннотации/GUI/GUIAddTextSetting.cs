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
    internal class GUIAddTextSetting
    {
        UpdateTime updateTime = new UpdateTime();



        public void AddText(string text, int Size, WidgetLabel widgetEditText, WidgetVBox becraund)
        {
            //var GUI = Gui.GetCurrent();
            var dialog = new EngineWindowViewport("Выноска", 300, 200);



            WidgetVBox widgetVBox = new WidgetVBox();
            widgetVBox.Width = dialog.Size.x;
            widgetVBox.Height = dialog.Size.y - 20;
            widgetVBox.Background = 1;
            widgetVBox.BackgroundColor = vec4.WHITE;


            dialog.AddChild(widgetVBox);


            // Массивы для полей ввода и подписей
            WidgetEditText[] inputs = new WidgetEditText[2];
            WidgetLabel[] labels = new WidgetLabel[2];

            // Имена меток
            string[] labelNames = new string[] { "Текст", "Размер текста" };

            // Начальные значения для полей ввода
            string[] initialValues = new string[] { text, Size.ToString() };

            // Инициализируем поля ввода и задаем начальные значения
            for (int i = 0; i < inputs.Length; i++)
            {
                inputs[i] = new WidgetEditText(dialog.Gui);
                inputs[i].Width = 200;
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
            WidgetButton becraundButton = new WidgetButton(dialog.Gui, "Вкл/Выкл фона");
            WidgetButton colorBecraundButton = new WidgetButton(dialog.Gui, "Выбор цвета фона");

            // Функция для создания строки с выравниванием по краям
            WidgetHBox CreateInputRow(WidgetLabel label, WidgetEditText input)
            {
                WidgetHBox row = new WidgetHBox(dialog.Gui);
                row.AddChild(label, Gui.ALIGN_LEFT);
                row.AddChild(input, Gui.ALIGN_RIGHT);
                return row;
            }


            // Добавляем строки для каждого поля и подписи
            for (int i = 0; i < inputs.Length; i++)
            {
                widgetVBox.AddChild(CreateInputRow(labels[i], inputs[i]), Gui.ALIGN_EXPAND);
            }

            // Добавляем элементы в окно
            widgetVBox.AddChild(colorButton, Gui.ALIGN_EXPAND);
            widgetVBox.AddChild(becraundButton, Gui.ALIGN_EXPAND);
            widgetVBox.AddChild(colorBecraundButton, Gui.ALIGN_EXPAND);


            // Обработчик выбора цвета
            colorButton.EventClicked.Connect(async () => {
                
                GUIAddColor gUIAddColor = new GUIAddColor();
                var Colar = await gUIAddColor.Color();
                widgetEditText.FontColor = Colar;
            });

            becraundButton.EventClicked.Connect(() => {
                if(becraund.Background == 0)
                {
                    becraund.Background = 1;
                }
                else
                {
                    becraund.Background = 0;
                }
            });

            colorBecraundButton.EventClicked.Connect(async () => {

                GUIAddColor gUIAddColor = new GUIAddColor();
                var Colar = await gUIAddColor.Color();
                becraund.BackgroundColor = Colar;
            });




            for (int i = 0; i < inputs.Length; i++)
            {
                int index2 = i; // Локальная переменная для замыкания
                inputs[i].EventKeyPressed.Connect(() =>
                {
                    switch (index2)
                    {
                        case 0: // Текст
                            widgetEditText.Text = inputs[0].Text;
                            break;

                        case 1: // Размер текста
                            if (float.TryParse(inputs[1].Text, out float fontSize))
                            {
                                widgetEditText.FontSize = (int)fontSize;
                            }
                            break;

                            // Добавь другие case, если нужно
                    }
                });
            }



            updateTime.StarttMonitoringdialogSize(dialog, widgetVBox);

            //var dynamicMesh = World.GetNodeByName("Omni_" + omni.ID.ToString());
            //dynamicMesh.Name = "Omni_" + omni.ID.ToString() + "_" + dialog.ID.ToString();

            dialog.Show();
        }


    }
}
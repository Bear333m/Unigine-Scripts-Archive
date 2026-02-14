using System.Collections;
using System.Collections.Generic;
using Unigine;
using UnigineApp.data.Code.Auxiliary;
using UnigineApp.data.Code;
using UnigineApp.data.Code.Annotation.Arrow;
using UnigineApp.data.Code.Annotation.GUI;
using UnigineApp.data.Code.Light;
using UnigineApp.data.CODE;
using Microsoft.VisualBasic;
using UnigineApp.data.Code.Annotation.Text;
using UnigineApp.data.Code.GUI;

[Component(PropertyGuid = "c6eac9e511a364ac1586801673d2fa79ffc9af32")]
public class MAINAnnotation : Component
{
    IconAnimation iconAnimation = new IconAnimation();

    DisplayAnnotationType displayAnnotationTypeSetting = new DisplayAnnotationType();
    DisplayAnnotationSetting displayAnnotationSetting = new DisplayAnnotationSetting();
    GUIAddArrow gUIAddArrow = new GUIAddArrow();
    AuxiliaryPlane auxiliaryPlane = new AuxiliaryPlane();

    Intersection intersection = new Intersection();

    int iconSize = 50;
    int spacing = 15;

    int iconSize2 = 20;
    int spacing2 = 10;

    string[] NameButtonArow = ["Arrow0", "Arrow1", "Arrow2", "Arrow3"];
    string[] NameButton = ["Rule", "TextCamera", "TextCamera2", "TextScen", "TextScen2", "2DCamera", "2D", "Circle"];
    string[] NamesButtons = ["Размер", "Выноска 2D", "Текст 2D", "Выноска 3D", "Текст 3D", "Рисунок 3D", "Рисунок 2D", "Кружок"];

    void Init()
    {
        // write here code to be called on component initialization
        WidgetHBox widgetHBox = FuncController.funcAnnotation;

        WidgetHBox menuHBox = new WidgetHBox(widgetHBox.Gui, spacing, 0);

        GUIText gUIText = new GUIText();


        WidgetVBox grup1VBox = new WidgetVBox(menuHBox.Gui, 0, 0);

        WidgetHBox grup1HBox = new WidgetHBox(menuHBox.Gui, spacing2, 0);
        WidgetHBox grup2HBox = new WidgetHBox(menuHBox.Gui, spacing2, 15);

        WidgetSprite widgetSprite1 = new WidgetSprite(grup1HBox.Gui);
        widgetSprite1.Texture = "../data/UI/icon/MenuAnnotation_icon/" + NameButtonArow[0] + ".png";
        widgetSprite1.Width = iconSize2;
        widgetSprite1.Height = (int)(iconSize2 * 0.85f);
        //widgetSprite.SetToolTip(NameButton2[i]);
        iconAnimation.IconEvent(widgetSprite1);

        WidgetSprite widgetSprite2 = new WidgetSprite(grup1HBox.Gui);
        widgetSprite2.Texture = "../data/UI/icon/MenuAnnotation_icon/" + NameButtonArow[1] + ".png";
        widgetSprite2.Width = iconSize2;
        widgetSprite2.Height = (int)(iconSize2 * 0.85f);
        //widgetSprite.SetToolTip(NameButton2[i]);
        iconAnimation.IconEvent(widgetSprite2);

        WidgetSprite widgetSprite3 = new WidgetSprite(grup2HBox.Gui);
        widgetSprite3.Texture = "../data/UI/icon/MenuAnnotation_icon/" + NameButtonArow[2] + ".png";
        widgetSprite3.Width = iconSize2;
        widgetSprite3.Height = (int)(iconSize2 * 0.85f);
        //widgetSprite.SetToolTip(NameButton2[i]);
        iconAnimation.IconEvent(widgetSprite3);

        WidgetSprite widgetSprite4 = new WidgetSprite(grup2HBox.Gui);
        widgetSprite4.Texture = "../data/UI/icon/MenuAnnotation_icon/" + NameButtonArow[3] + ".png";
        widgetSprite4.Width = iconSize2;
        widgetSprite4.Height = (int)(iconSize2 * 0.85f);
        //widgetSprite.SetToolTip(NameButton2[i]);
        iconAnimation.IconEvent(widgetSprite4);

        WidgetLabel widgetLabel1 = new WidgetLabel(grup1VBox.Gui);
        widgetLabel1.Text = "Стрелки";
        widgetLabel1.FontSize = 20;

        grup1HBox.AddChild(widgetSprite1);
        grup1HBox.AddChild(widgetSprite2);

        grup2HBox.AddChild(widgetSprite3);
        grup2HBox.AddChild(widgetSprite4);

        //grupVBox.AddChild(widgetLabel);
        grup1VBox.AddChild(grup1HBox);
        grup1VBox.AddChild(grup2HBox);
        grup1VBox.AddChild(widgetLabel1);

        menuHBox.AddChild(grup1VBox);



        for (int i = 0; i < NameButton.Length; i++)
        {
            WidgetVBox grupVBox = new WidgetVBox(menuHBox.Gui, 0, 18);

            WidgetSprite widgetSprite = new WidgetSprite(grupVBox.Gui);
            widgetSprite.Texture = "../data/UI/icon/MenuAnnotation_icon/" + NameButton[i] + ".png";
            widgetSprite.Width = iconSize;
            widgetSprite.Height = (int)(iconSize * 0.85f);
            //widgetSprite.SetToolTip(NameButton2[i]);
            iconAnimation.IconEvent(widgetSprite);

            WidgetLabel widgetLabel = new WidgetLabel(grupVBox.Gui);
            widgetLabel.Text = NamesButtons[i];
            widgetLabel.FontSize = 16;
            //widgetLabel.TextAlign = Gui.ALIGN_CENTER;

            grupVBox.AddChild(widgetSprite);
            grupVBox.AddChild(widgetLabel);

            menuHBox.AddChild(grupVBox);
        }

        //                                      
        //  widgetHBox.AddChild(menuHBox, Gui.ALIGN_OVERLAP | Gui.ALIGN_TOP);

        //                                      
        widgetHBox.AddChild(menuHBox, Gui.ALIGN_OVERLAP | Gui.ALIGN_TOP);

        widgetSprite1.EventClicked.Connect(() =>
        {
            Storage.AnnotationType = 1;

        });
        widgetSprite2.EventClicked.Connect(() =>
        {
            Storage.AnnotationType = 2;

        });
        widgetSprite3.EventClicked.Connect(() =>
        {
            Storage.AnnotationType = 3;

        });
        widgetSprite4.EventClicked.Connect(() =>
        {
            Storage.AnnotationType = 4;

        });

        menuHBox.GetChild(1).GetChild(0).EventClicked.Connect(() =>
        {
            Storage.AnnotationType = 5;
        });


        menuHBox.GetChild(2).GetChild(0).EventClicked.Connect(() =>
        {
            Storage.AnnotationType = 7;
        });

        menuHBox.GetChild(3).GetChild(0).EventClicked.Connect(() =>
        {
            Storage.AnnotationType = 9;
        });

        menuHBox.GetChild(4).GetChild(0).EventClicked.Connect(() =>
        {
            Storage.AnnotationType = 6;
        });

        menuHBox.GetChild(5).GetChild(0).EventClicked.Connect(() =>
        {
            Storage.AnnotationType = 8;
        });

        menuHBox.GetChild(6).GetChild(0).EventClicked.Connect(async () =>
        {
            GUIGetPath gUIGetPath = new GUIGetPath();
            string file = await gUIGetPath.Path(".png.jpg");

            if (!string.IsNullOrEmpty(file) || System.IO.File.Exists(file))
            {
                Storage.texturePath = file;

                Storage.AnnotationType = 10;
            }
        });

        menuHBox.GetChild(7).GetChild(0).EventClicked.Connect(async () =>
        {
            GUIGetPath gUIGetPath = new GUIGetPath();
            string file = await gUIGetPath.Path(".png.jpg");

            if (!string.IsNullOrEmpty(file) || System.IO.File.Exists(file))
            {
                Storage.texturePath = file;

                auxiliaryPlane.CreatAuxiliarySphere_2();

                var plane = World.GetNodeByName("AuxiliaryPlane_okEn9HiQJNUyo6LuRU8Rcc");

                Game.Player.AddChild(plane);

                Storage.AnnotationType = 11;
            }
        });



        menuHBox.GetChild(8).GetChild(0).EventClicked.Connect(() =>
        {
            Storage.AnnotationType = 12;
        });

        ControlsApp.MouseHandle = Input.MOUSE_HANDLE.SOFT;

    }

    void Update()
    {

        displayAnnotationSetting.DisplayArrow();

        if (Input.IsMouseButtonDown(Input.MOUSE_BUTTON.DCLICK) && Storage.IDModel > 0)
        {
            var (obj, XYZ) = intersection.GetIntersection();
            AnnotationProcessing annotationProcessing = new AnnotationProcessing();
            annotationProcessing.ProcessingForDialog(obj);
        }

        // write here code to be called before updating each render frame
        if (Storage.AnnotationType != 0)
        {
            displayAnnotationTypeSetting.DisplayType();

            if (Input.IsKeyDown(Input.KEY.ESC))
            {
                AuxiliaryPlane auxiliaryPlane = new AuxiliaryPlane();
                auxiliaryPlane.Zeroing();

            }

        }
    }
}

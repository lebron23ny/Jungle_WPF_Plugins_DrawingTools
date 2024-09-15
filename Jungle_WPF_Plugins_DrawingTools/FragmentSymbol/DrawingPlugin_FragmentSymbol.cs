using System;
using System.Collections.Generic;
using Tekla.Structures.Plugins;
using TSD = Tekla.Structures.Drawing;
using TSDUI = Tekla.Structures.Drawing.UI;
using TSDT = Tekla.Structures.Drawing.Tools;
using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;
using Jungle_WPF_Plugins_DrawingTools.FragmentSymbol.Extension;

namespace Jungle_WPF_Plugins_DrawingTools.FragmentSymbol
{
    public class PluginData
    {
        [StructuresField("SizeC")]
        public int SizeC;

        [StructuresField("HeightFont")]
        public string HeightFont;

        [StructuresField("TextContent")]
        public string TextContent;

        [StructuresField("Direction")]
        public int Direction;
    }


    [Plugin("МИ_Символ_Фрагмента")]
    [PluginUserInterface("Jungle_WPF_Plugins_DrawingTools.FragmentSymbol.MainWindow_FragmentSymbol")]
    class DrawingPlugin_FragmentSymbol : DrawingPluginBase
    {
        PluginData Data { get; set; }
        TSD.DrawingHandler DrawingHandler { get; set; }

        public DrawingPlugin_FragmentSymbol(PluginData data)
        {
            DrawingHandler = new TSD.DrawingHandler();
            this.Data = data;
        }

        public override List<InputDefinition> DefineInput()
        {
            List<InputDefinition> listInputDefinition = new List<InputDefinition>();
            TSDUI.Picker picker = DrawingHandler.GetPicker();
            TSG.Point firstPoint;
            TSG.Point secondPoint;
            TSD.ViewBase view;
            picker.PickTwoPoints("Выберите первую точку", "Выберите вторую точку", out firstPoint, out secondPoint, out view);
            #region вариант 1
            listInputDefinition.Add(TSDT.InputDefinitionFactory.CreateInputDefinition(view, firstPoint));
            listInputDefinition.Add(TSDT.InputDefinitionFactory.CreateInputDefinition(view, secondPoint));
            #endregion
            return listInputDefinition;
        }

        public override bool Run(List<InputDefinition> Input)
        {
            try
            {
                int sizeC = Data.SizeC;
                double hFont = Tools.GetParseDouble(Data.HeightFont);
                string text = Data.TextContent;
                int direction;
                if (Data.Direction < 0)
                    direction = 0;
                else
                    direction = Data.Direction;
                TSD.Drawing drawing = DrawingHandler.GetActiveDrawing();

                Draw_FragmentSymbol(Input, sizeC, hFont, text, direction);
                drawing.CommitChanges();
            }
            catch
            {

            }
            return true;
        }


        private void Draw_FragmentSymbol(List<InputDefinition> Input, 
            int sizeC, 
            double hFont, string text, int direction)
        {
            TSG.Point ptInput1;
            TSG.Point ptInput2;
            if (direction == 0)
            {
                ptInput1 = TSDT.InputDefinitionFactory.GetPoint(Input[0]) as TSG.Point;
                ptInput2 = TSDT.InputDefinitionFactory.GetPoint(Input[1]) as TSG.Point;
            }
            else
            {
                ptInput2 = TSDT.InputDefinitionFactory.GetPoint(Input[0]) as TSG.Point;
                ptInput1 = TSDT.InputDefinitionFactory.GetPoint(Input[1]) as TSG.Point;
            }
            
            TSD.ViewBase viewBase = TSDT.InputDefinitionFactory.GetView(Input[0]) as TSD.ViewBase;
           
            int a = sizeC;
            int b = sizeC;

            double font_height = hFont;
            string textContent = text;

            #region Настройка LineAttributes
            TSD.Line.LineAttributes lineAttributesBlue = new TSD.Line.LineAttributes();
            lineAttributesBlue.Arrowhead.ArrowPosition = TSD.ArrowheadPositions.None;
            lineAttributesBlue.Arrowhead.Head = TSD.ArrowheadTypes.NoArrow;
            lineAttributesBlue.Arrowhead.Height = 2;
            lineAttributesBlue.Arrowhead.Width = 3;

            lineAttributesBlue.Line.Color = TSD.DrawingColors.Blue;
            lineAttributesBlue.Line.Type = TSD.LineTypes.SolidLine;
            #endregion
            #region Настройки ArcAttributes
            TSD.Arc.ArcAttributes arcAttributes = new TSD.Arc.ArcAttributes();
            arcAttributes.Arrowhead.ArrowPosition = TSD.ArrowheadPositions.None;
            arcAttributes.Arrowhead.Head = TSD.ArrowheadTypes.FilledArrow;
            arcAttributes.Arrowhead.Height = 2;
            arcAttributes.Arrowhead.Width = 1;

            arcAttributes.Line.Color = TSD.DrawingColors.Blue;
            arcAttributes.Line.Type = TSD.LineTypes.SolidLine;
            #endregion

            #region Настройки TextAttributes
            string fontName = "GOST 2.304 type A";            
            TSD.FontAttributes fontAttributes = new TSD.FontAttributes();
            fontAttributes.Italic = false;
            fontAttributes.Bold = false;
            fontAttributes.Height = font_height;
            fontAttributes.Name = fontName;
            fontAttributes.Color = TSD.DrawingColors.Black;
            TSD.Text.TextAttributes textAttributes = new TSD.Text.TextAttributes();
            textAttributes.Alignment = TSD.TextAlignment.Center;
            textAttributes.Frame = new TSD.Frame(TSD.FrameTypes.None, TSD.DrawingColors.Black);
            textAttributes.Font = fontAttributes;
            #endregion
            double length = TSG.Distance.PointToPoint(ptInput1, ptInput2);
            TSG.Vector vectorX = new TSG.Vector(
                ptInput2.X - ptInput1.X, 
                ptInput2.Y - ptInput1.Y, 
                ptInput2.Z - ptInput1.Z);
            vectorX.Normalize();
            double angleDeg;
            if (vectorX.X * vectorX.Y < 0)
                angleDeg = -1 * Math.Acos(Math.Abs(vectorX.X)) * 180 / Math.PI;
            else
                angleDeg = Math.Acos(Math.Abs(vectorX.X)) * 180 / Math.PI;

            TSG.Vector vectorZ = new TSG.Vector(0, 0, 1);
            TSG.Vector vectorY = vectorZ.Cross(vectorX);
            TSM.TransformationPlane transformationPlane = new TSM.TransformationPlane(ptInput1, vectorX, vectorY);
            TSG.Matrix matrix = transformationPlane.TransformationMatrixToGlobal;

            TSG.Point pt11 = new TSG.Point(0, 0, 0);
            TSG.Point pt12 = new TSG.Point(a, a, 0);
            TSG.Point centerPt1 = new TSG.Point(a, 0, 0);


            TSG.Point pt21 = new TSG.Point(length / 2 - b, a, 0);
            TSG.Point pt22 = new TSG.Point(length / 2, a + b, 0);
            TSG.Point centerPt2 = new TSG.Point(length / 2 - b, a + b, 0);

            TSG.Point pt31 = pt22;
            TSG.Point pt32 = new TSG.Point(length / 2 + b, a, 0);
            TSG.Point centerPt3 = new TSG.Point(length / 2 + b, a + b, 0);

            TSG.Point pt41 = new TSG.Point(length - a, a, 0);
            TSG.Point pt42 = new TSG.Point(length, 0, 0);
            TSG.Point centerPt4 = new TSG.Point(length - a, 0, 0);

            double height = Tools.GetHeightText(textContent, textAttributes, viewBase);

            TSG.Point insertPTtext = new TSG.Point(length / 2, a + b + height / 2 + 2, 0);
            TSG.Point pt11_global = matrix.Transform(pt11);
            TSG.Point pt12_global = matrix.Transform(pt12);
            TSG.Point centerPt1_global = matrix.Transform(centerPt1);

            TSG.Point pt21_global = matrix.Transform(pt21);
            TSG.Point pt22_global = matrix.Transform(pt22);
            TSG.Point centerPt2_global = matrix.Transform(centerPt2);


            TSG.Point pt31_global = matrix.Transform(pt31);
            TSG.Point pt32_global = matrix.Transform(pt32);
            TSG.Point centerPt3_global = matrix.Transform(centerPt3);

            TSG.Point pt41_global = matrix.Transform(pt41);
            TSG.Point pt42_global = matrix.Transform(pt42);
            TSG.Point centerPt4_global = matrix.Transform(centerPt4);

            TSG.Point ptTextInsert_global = matrix.Transform(insertPTtext);

            TSD.Arc arc1 = new TSD.Arc(viewBase, pt11_global, pt12_global, centerPt1_global, arcAttributes);
            arc1.Insert();

            TSD.Arc arc2 = new TSD.Arc(viewBase, pt22_global, pt21_global, centerPt2_global, arcAttributes);
            arc2.Insert();

            TSD.Arc arc3 = new TSD.Arc(viewBase, pt32_global, pt31_global, centerPt3_global, arcAttributes);
            arc3.Insert();

            TSD.Arc arc4 = new TSD.Arc(viewBase, pt41_global, pt42_global, centerPt4_global, arcAttributes);
            arc4.Insert();


            TSD.Line line1 = new TSD.Line(viewBase, pt12_global, pt21_global, lineAttributesBlue);
            TSD.Line line2 = new TSD.Line(viewBase, pt32_global, pt41_global, lineAttributesBlue);

            line1.Insert();
            line2.Insert();

            textAttributes.Angle = angleDeg;

            TSD.Text textObj = new TSD.Text(viewBase, ptTextInsert_global, textContent, textAttributes);
            textObj.Insert();

        }
    }



}

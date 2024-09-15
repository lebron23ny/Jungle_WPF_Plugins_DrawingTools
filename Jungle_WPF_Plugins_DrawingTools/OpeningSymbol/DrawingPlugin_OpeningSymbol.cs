using System;
using System.Collections.Generic;
using Tekla.Structures.Plugins;
using TSD = Tekla.Structures.Drawing;
using TSDUI = Tekla.Structures.Drawing.UI;
using TSDT = Tekla.Structures.Drawing.Tools;
using TSG = Tekla.Structures.Geometry3d;

namespace Jungle_WPF_Plugins_DrawingTools.OpeningSymbol
{
    public class PluginData
    {
        [StructuresField("IndexType")]
        public int IndexType;

        [StructuresField("IsDrawFrameIndex")]
        public int IsDrawFrameIndex;
    }

    [Plugin("МИ_Символ_проема")]
    [PluginUserInterface("Jungle_WPF_Plugins_DrawingTools.OpeningSymbol.MainWindow_OpeningSymbol")]
    class DrawingPlugin_OpeningSymbol : DrawingPluginBase
    {
        PluginData Data { get; set; }
        TSD.DrawingHandler DrawingHandler { get; set; }

        public DrawingPlugin_OpeningSymbol(PluginData data)
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

                int indexType = Data.IndexType;
                int isDrawFrameIndex = Data.IsDrawFrameIndex;
                if (indexType != 0 && indexType != 1 && indexType != 2 && indexType != 3)
                    indexType = 0;
                if (isDrawFrameIndex != 0 && isDrawFrameIndex != 1)
                    isDrawFrameIndex = 0;
                enumOrintationSymbol orintationSymbol = (enumOrintationSymbol)indexType;
                enumDrawFrame isDrawFrame = (enumDrawFrame)isDrawFrameIndex;
                var drawing = DrawingHandler.GetActiveDrawing();
                DrawSymbol(Input, orintationSymbol, isDrawFrame);

                drawing.CommitChanges();
            }
            catch
            {

            }
            return true;
        }

        private void DrawSymbol(List<InputDefinition> Input, enumOrintationSymbol orintationSymbol, enumDrawFrame isDrawFrame)
        {
            TSD.ViewBase view = TSDT.InputDefinitionFactory.GetView(Input[0]) as TSD.ViewBase;
            TSG.Point firstPoint1 = TSDT.InputDefinitionFactory.GetPoint(Input[0]) as TSG.Point;
            TSG.Point secondPoint1 = TSDT.InputDefinitionFactory.GetPoint(Input[1]) as TSG.Point;

            double xMin = Math.Min(firstPoint1.X, secondPoint1.X);
            double yMin = Math.Min(firstPoint1.Y, secondPoint1.Y);

            double xMax = Math.Max(firstPoint1.X, secondPoint1.X);
            double yMax = Math.Max(firstPoint1.Y, secondPoint1.Y);

            TSG.Point firstPoint = new TSG.Point(xMin, yMin, 0);
            TSG.Point secondPoint = new TSG.Point(xMax, yMax, 0);

            TSG.Point threePoint = new TSG.Point(xMin, yMax, 0);
            TSG.Point fourPoint = new TSG.Point(xMax, yMin, 0);

            

            if (isDrawFrame == enumDrawFrame.yes)
            {
                TSD.Rectangle.RectangleAttributes rectangleAttributes =
                    new TSD.Rectangle.RectangleAttributes("МИ_Символ_проема");
                TSD.Rectangle rectangle = new TSD.Rectangle(view,
                    firstPoint, secondPoint, rectangleAttributes);
                rectangle.Insert();
            }
            double factor = 0.1;
            double dx;
            double dy;
            TSG.Point middlePoint;
            TSD.PointList list = new TSD.PointList();
            if (orintationSymbol == enumOrintationSymbol.left_top)
            {
                dx = (secondPoint.X - firstPoint.X) * factor;
                dy = (secondPoint.Y - firstPoint.Y) * (1 - factor);
                middlePoint = new TSG.Point(firstPoint.X + dx, firstPoint.Y + dy, 0);
                list.Add(firstPoint);
                list.Add(threePoint);
                list.Add(secondPoint);
                list.Add(middlePoint);
            }
            else if (orintationSymbol == enumOrintationSymbol.right_top)
            {
                dx = (secondPoint.X - firstPoint.X) * (1 - factor);
                dy = (secondPoint.Y - firstPoint.Y) * (1 - factor);
                middlePoint = new TSG.Point(firstPoint.X + dx, firstPoint.Y + dy, 0);
                list.Add(threePoint);
                list.Add(secondPoint);
                list.Add(fourPoint);
                list.Add(middlePoint);
            }
            else if (orintationSymbol == enumOrintationSymbol.left_bottom)
            {
                dx = (secondPoint.X - firstPoint.X) * factor;
                dy = (secondPoint.Y - firstPoint.Y) * factor;
                middlePoint = new TSG.Point(firstPoint.X + dx, firstPoint.Y + dy, 0);
                list.Add(threePoint);
                list.Add(firstPoint);
                list.Add(fourPoint);
                list.Add(middlePoint);
            }
            else if (orintationSymbol == enumOrintationSymbol.right_bottom)
            {
                dx = (secondPoint.X - firstPoint.X) * (1 - factor);
                dy = (secondPoint.Y - firstPoint.Y) * factor;
                middlePoint = new TSG.Point(firstPoint.X + dx, firstPoint.Y + dy, 0);
                list.Add(firstPoint);
                list.Add(fourPoint);
                list.Add(secondPoint);
                list.Add(middlePoint);
            }
            TSD.Polygon.PolygonAttributes polygonAttributes =
                new TSD.Polygon.PolygonAttributes("МИ_Символ_проема");
            TSD.Polygon polygon = new TSD.Polygon(view, list, polygonAttributes);
            polygon.Insert();
        }
    }


    public enum enumOrintationSymbol
    {
        left_top = 0,
        right_top = 1,
        left_bottom = 2,
        right_bottom = 3,
    }

    public enum enumDrawFrame
    {
        no = 0,
        yes = 1,
    }

}

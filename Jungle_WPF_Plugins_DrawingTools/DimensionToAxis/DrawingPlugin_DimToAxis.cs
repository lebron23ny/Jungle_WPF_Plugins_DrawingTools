using System;
using System.Collections.Generic;
using Tekla.Structures.Plugins;
using TSD = Tekla.Structures.Drawing;
using TSDUI = Tekla.Structures.Drawing.UI;
using TSDT = Tekla.Structures.Drawing.Tools;
using TSG = Tekla.Structures.Geometry3d;
using TSM = Tekla.Structures.Model;

namespace Jungle_WPF_Plugins_DrawingTools.DimensionToAxis
{
    public class PluginData
    {
        [StructuresField("IndexType")]
        public int IndexType;

        [StructuresField("DistanceDim")]
        public double DistanceDim;

        [StructuresField("NameDimAttr")]
        public string NameDimAttr;

        [StructuresField("NameTextAttr")]
        public string NameTextAttr;

        [StructuresField("LengthAxis")]
        public double LengthAxis;
    }

    [Plugin("МИ_Расстояние_до_оси")]
    [PluginUserInterface("Jungle_WPF_Plugins_DrawingTools.DimensionToAxis.MainWindow_DimToAxis")]
    class DrawingPlugin_DimToAxis : DrawingPluginBase
    {
        public DrawingPlugin_DimToAxis(PluginData data)
        {
            DrawingHandler = new TSD.DrawingHandler();
            Data = data;
        }

        PluginData Data {  get; set; }
        TSD.DrawingHandler DrawingHandler { get; set; }
        public override List<InputDefinition> DefineInput()
        {
            TSM.TransformationPlane transformationPlane = new TSM.TransformationPlane();
            TSM.Model model = new TSM.Model();
            TSM.WorkPlaneHandler workPlaneHandler = model.GetWorkPlaneHandler();
            workPlaneHandler.SetCurrentTransformationPlane(transformationPlane);

            List<InputDefinition> inputDefinitions = new List<InputDefinition>();
            TSDUI.Picker picker = DrawingHandler.GetPicker();
            Tuple<TSD.DrawingObject, TSD.ViewBase> selected = picker.PickObject("Выделите сетку");

            TSD.DrawingObject drawingObject = selected.Item1;
            TSD.ViewBase viewBase = selected.Item2;

            //inputDefinitions.Add(TSDT.InputDefinitionFactory.CreateInputDefinition(viewBase, drawingObject));


            TSD.StringList stringList = new TSD.StringList();
            stringList.Add("Выделите первую точку");
            stringList.Add("Выделите вторую точку");
            Tuple<TSD.PointList, TSD.ViewBase> tuple = picker.PickPoints(2, stringList);
            TSD.PointList pointList = tuple.Item1;
            TSG.Point firstPoint_ = pointList[0];
            TSD.ViewBase viewBasePt1 = tuple.Item2;
            TSG.Point secondPoint_ = pointList[1];

            inputDefinitions.Add(TSDT.InputDefinitionFactory.CreateInputDefinition(viewBasePt1, drawingObject));
            inputDefinitions.Add(TSDT.InputDefinitionFactory.CreateInputDefinition(viewBasePt1, firstPoint_));
            inputDefinitions.Add(TSDT.InputDefinitionFactory.CreateInputDefinition(viewBasePt1, secondPoint_));
            


            return inputDefinitions;
        }

        public override bool Run(List<InputDefinition> Input)
        {
            try
            {
                //TSD.View view = TSDT.InputDefinitionFactory.GetView(Input[0]) as TSD.View;
                //TSD.DrawingObject drawingObj = TSDT.InputDefinitionFactory.GetDrawingObject(Input[2]) as TSD.DrawingObject;
                //TSD.Grid gridDRW = drawingObj as TSD.Grid;
                //TSG.Point firstPoint_ = TSDT.InputDefinitionFactory.GetPoint(Input[0]) as TSG.Point;
                //TSG.Point secondPoint_ = TSDT.InputDefinitionFactory.GetPoint(Input[1]) as TSG.Point;

                TSD.View view = TSDT.InputDefinitionFactory.GetView(Input[1]) as TSD.View;
                TSD.DrawingObject drawingObj = TSDT.InputDefinitionFactory.GetDrawingObject(Input[0]) as TSD.DrawingObject;
                TSD.Grid gridDRW = drawingObj as TSD.Grid;
                TSG.Point firstPoint_ = TSDT.InputDefinitionFactory.GetPoint(Input[1]) as TSG.Point;
                TSG.Point secondPoint_ = TSDT.InputDefinitionFactory.GetPoint(Input[2]) as TSG.Point;

                int indexType = Data.IndexType;
                if (indexType != 0 && indexType != 1 && indexType != 2)
                    indexType = 0;
                double dim = Data.DistanceDim;
                if (dim <= 0)
                    dim = 1500;
                string nameDimAttr = Data.NameDimAttr;
                string nameTextAttr = Data.NameTextAttr;

                double lengthAxis = Data.LengthAxis;
                if(lengthAxis<=0)
                    lengthAxis = 0;

                TSD.Drawing drawing = DrawingHandler.GetActiveDrawing();

                MyGrid myGrid = new MyGrid(gridDRW, firstPoint_, secondPoint_, view);
                myGrid.DrawAxis();
                if (indexType == 0)
                {
                    myGrid.DrawPreviousAxis(dim, nameDimAttr, nameTextAttr, lengthAxis);
                    myGrid.DrawNextAxis(dim, nameDimAttr, nameTextAttr, lengthAxis);
                }
                else if (indexType == 1)
                {
                    myGrid.DrawPreviousAxis(dim, nameDimAttr, nameTextAttr, lengthAxis);
                }
                else if (indexType == 2)
                {
                    myGrid.DrawNextAxis(dim, nameDimAttr, nameTextAttr, lengthAxis);
                }

                drawing.CommitChanges();

            }
            catch
            {

            }

            return true;
        }
    }
}

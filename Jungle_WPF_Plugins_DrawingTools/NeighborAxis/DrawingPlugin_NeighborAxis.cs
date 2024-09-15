using System;
using System.Collections.Generic;
using Tekla.Structures.Plugins;
using TSD = Tekla.Structures.Drawing;
using TSDUI = Tekla.Structures.Drawing.UI;
using TSDT = Tekla.Structures.Drawing.Tools;
using TSG = Tekla.Structures.Geometry3d;
using Jungle_WPF_Plugins_DrawingTools.NeighborAxis.Classes;

namespace Jungle_WPF_Plugins_DrawingTools.NeighborAxis
{
    public class PluginData
    {
        [StructuresField("IndexType")]
        public int IndexType;
    }

    [Plugin("МИ_Соседние_оси")]
    [PluginUserInterface("Jungle_WPF_Plugins_DrawingTools.NeighborAxis.MainWindow_NeighborAxis")]
    class DrawingPlugin : DrawingPluginBase
    {

        PluginData Data { get; set; }
        TSD.DrawingHandler DrawingHandler { get; set; }

        public DrawingPlugin(PluginData data)
        {
            DrawingHandler = new TSD.DrawingHandler();
            this.Data = data;
        }



        public override List<InputDefinition> DefineInput()
        {
            List<InputDefinition> listInputDefinition = new List<InputDefinition>();
            TSDUI.Picker picker = DrawingHandler.GetPicker();
            Tuple<TSD.DrawingObject, TSD.ViewBase> SelectDrawingObjectAndView = picker.PickObject("Выберите ось");
            TSD.DrawingObject drawingObjectInput = SelectDrawingObjectAndView.Item1;
            TSD.ViewBase viewBaseInput = SelectDrawingObjectAndView.Item2;
            listInputDefinition.Add(TSDT.InputDefinitionFactory.CreateInputDefinition(viewBaseInput, drawingObjectInput));

            Tuple<TSG.Point, TSD.ViewBase> SelectPointAndView = picker.PickPoint("Выберите точку");
            TSG.Point pointInput = SelectPointAndView.Item1;

            listInputDefinition.Add(TSDT.InputDefinitionFactory.CreateInputDefinition(viewBaseInput, pointInput));

            return listInputDefinition;
        }

        public override bool Run(List<InputDefinition> Input)
        {
            try
            {
                TSD.View view = TSDT.InputDefinitionFactory.GetView(Input[0]) as TSD.View;
                TSD.DrawingObject drawingObj = TSDT.InputDefinitionFactory.GetDrawingObject(Input[0]) as TSD.DrawingObject;
                TSD.Grid grid = drawingObj as TSD.Grid;
                TSG.Point point = TSDT.InputDefinitionFactory.GetPoint(Input[1]) as TSG.Point;

                int indexType = Data.IndexType;
                if (indexType != 0 && indexType != 1 && indexType != 2)
                    indexType = 0;
                enumPositionDraw position = (enumPositionDraw)indexType;
                var drawing = DrawingHandler.GetActiveDrawing();
                NeighborLabelDrawer neighborLabelDrawer = new NeighborLabelDrawer(point, view, grid, position);
                if (!neighborLabelDrawer.IsParallelToX &&
                    neighborLabelDrawer.IsStartLabel)
                    neighborLabelDrawer.SetLabelLocation(new DrawBottomXLabel());

                if (neighborLabelDrawer.IsParallelToX &&
                    neighborLabelDrawer.IsStartLabel)
                    neighborLabelDrawer.SetLabelLocation(new DrawLeftYLabel());

                if (!neighborLabelDrawer.IsParallelToX &&
                    !neighborLabelDrawer.IsStartLabel)
                    neighborLabelDrawer.SetLabelLocation(new DrawUpperXLabel());

                if (neighborLabelDrawer.IsParallelToX &&
                    !neighborLabelDrawer.IsStartLabel)
                    neighborLabelDrawer.SetLabelLocation(new DrawRightYLabel());


                neighborLabelDrawer.DrawNeighborLabel();
                drawing.CommitChanges();


            }
            catch 
            {


            }


            return true;
        }

    }
}

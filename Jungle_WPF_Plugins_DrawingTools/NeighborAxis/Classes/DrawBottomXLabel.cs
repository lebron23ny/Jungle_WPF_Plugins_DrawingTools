using System;
using TSD = Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;

namespace Jungle_WPF_Plugins_DrawingTools.NeighborAxis.Classes
{
    public class DrawBottomXLabel : IDrawLabel
    {
        public void DrawLabel(Point pickedPoint, TSD.View viewBase, double frameHeight,
            double frameWidth, string nextLabelInput, string prevLabelInput, TSD.Grid grid, enumPositionDraw positionDraw, TSD.FrameTypes frameType)
        {

            string nextLabel;
            string prevLabel;
            if ((viewBase.ViewCoordinateSystem.AxisX == new Vector(1, 0, 0)) || viewBase.ViewCoordinateSystem.AxisX == new Vector(0, 1, 0))
            {
                nextLabel = nextLabelInput;
                prevLabel = prevLabelInput;
            }
            else
            {
                nextLabel = prevLabelInput;
                prevLabel = nextLabelInput;
            }

            if (!String.IsNullOrEmpty(nextLabel) &&
                (positionDraw == enumPositionDraw.nextAndPrevios || positionDraw == enumPositionDraw.next))
            {
                Point startToNext = new Point(pickedPoint.X + frameWidth / 2, pickedPoint.Y - frameHeight / 2);
                Point endToNext = new Point(pickedPoint.X + 1.5 * frameWidth, pickedPoint.Y - frameHeight / 2);
                Point textNext = new Point(endToNext.X + frameWidth / 2, endToNext.Y);
                Drawer.InsertLine(viewBase, startToNext, endToNext, grid);
                Drawer.InsertText(viewBase, textNext, nextLabel, grid);
                Drawer.InsertFrame(viewBase, textNext, nextLabel, grid, frameHeight, frameWidth, frameType);
            }
            if (!String.IsNullOrEmpty(prevLabel) &&
                (positionDraw == enumPositionDraw.nextAndPrevios || positionDraw == enumPositionDraw.previous))
            {
                Point startToPrev = new Point(pickedPoint.X - frameWidth / 2, pickedPoint.Y - frameHeight / 2);
                Point endToPrev = new Point(pickedPoint.X - 1.5 * frameWidth, pickedPoint.Y - frameHeight / 2);
                Point textPrev = new Point(endToPrev.X - frameWidth / 2, endToPrev.Y);
                Drawer.InsertLine(viewBase, startToPrev, endToPrev, grid);
                Drawer.InsertText(viewBase, textPrev, prevLabel, grid);
                Drawer.InsertFrame(viewBase, textPrev, nextLabel, grid, frameHeight, frameWidth, frameType);
            }
        }
    }
}

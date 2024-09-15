using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSD=Tekla.Structures.Drawing;
using Tekla.Structures.Geometry3d;

namespace Jungle_WPF_Plugins_DrawingTools.NeighborAxis.Classes
{
    public class DrawLeftYLabel : IDrawLabel
    {
        public void DrawLabel(Point pickedPoint,
            TSD.View viewBase, double frameHeight, double frameWidth,
            string nextLabel, string prevLabel, TSD.Grid grid, enumPositionDraw positionDraw, TSD.FrameTypes frameType)
        {
            if (!String.IsNullOrEmpty(nextLabel) &&
                (positionDraw == enumPositionDraw.nextAndPrevios || positionDraw == enumPositionDraw.next))
            {
                Point startToNext = new Point(pickedPoint.X - frameWidth / 2, pickedPoint.Y + frameHeight / 2);
                Point endToNext = new Point(pickedPoint.X - frameWidth / 2, pickedPoint.Y + 1.5 * frameHeight);
                Point textNext = new Point(endToNext.X, endToNext.Y + frameHeight / 2);
                Drawer.InsertLine(viewBase, startToNext, endToNext, grid);
                Drawer.InsertText(viewBase, textNext, nextLabel, grid);
                Drawer.InsertFrame(viewBase, textNext, nextLabel, grid, frameHeight, frameWidth, frameType);
            }
            if (!String.IsNullOrEmpty(prevLabel) &&
                (positionDraw == enumPositionDraw.nextAndPrevios || positionDraw == enumPositionDraw.previous))
            {
                Point startToPrev = new Point(pickedPoint.X - frameWidth / 2, pickedPoint.Y - frameHeight / 2);
                Point endToPrev = new Point(pickedPoint.X - frameWidth / 2, pickedPoint.Y - 1.5 * frameHeight);
                Point textPrev = new Point(endToPrev.X, endToPrev.Y - frameHeight / 2);
                Drawer.InsertLine(viewBase, startToPrev, endToPrev, grid);
                Drawer.InsertText(viewBase, textPrev, prevLabel, grid);
                Drawer.InsertFrame(viewBase, textPrev, nextLabel, grid, frameHeight, frameWidth, frameType);
            }
        }
    }
}

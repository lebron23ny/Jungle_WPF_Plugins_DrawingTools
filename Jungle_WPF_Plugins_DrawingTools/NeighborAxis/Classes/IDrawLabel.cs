using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;

namespace Jungle_WPF_Plugins_DrawingTools.NeighborAxis.Classes
{
    interface IDrawLabel
    {
        void DrawLabel(TSG.Point pickedPoint, TSD.View viewBase,
            double frameHeight, double frameWidth,
            string nextLabel, string prevLabel, TSD.Grid grid, enumPositionDraw position, TSD.FrameTypes frameType);
    }
}

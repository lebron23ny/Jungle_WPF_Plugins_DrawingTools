using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TS = Tekla.Structures;

namespace Jungle_WPF_Plugins_DrawingTools.NeighborAxis.Classes
{
    public class Drawer
    {
        internal static void InsertLine(TSD.ViewBase view, TSG.Point start, TSG.Point end, TSD.Grid grid)
        {


            TSD.DrawingColors colorFrame = grid.Attributes.Frame.Color;
            TSD.Line line = new TSD.Line(view, start, end);
            TSD.Line.LineAttributes lineAttributes = new TSD.Line.LineAttributes();
            lineAttributes.Arrowhead =
                new TSD.ArrowheadAttributes(TSD.ArrowheadPositions.End, TSD.ArrowheadTypes.FilledArrow, 2, 3);
            lineAttributes.Line =
                new TSD.LineTypeAttributes(TSD.LineTypes.SolidLine, colorFrame);
            line.Attributes = lineAttributes;
            line.Insert();
            line.Modify();
        }
        internal static void InsertText(TSD.ViewBase view, TSG.Point point, string label, TSD.Grid grid)
        {
            try
            {
                string fontName = grid.Attributes.Font.Name;
                TSD.DrawingColors colorFont = grid.Attributes.Font.Color;
                double heightFont = grid.Attributes.Font.Height;
                bool boldFont = grid.Attributes.Font.Bold;
                bool italicFont = grid.Attributes.Font.Italic;
                TSD.Text text = new TSD.Text(view, point, label);
                TSD.Text.TextAttributes textAttributes = new TSD.Text.TextAttributes();
                textAttributes.Font = new TSD.FontAttributes(colorFont, heightFont, fontName, italicFont, boldFont);
                text.Attributes = textAttributes;
                text.Insert();
                text.Modify();

            }

            catch { }
        }

        internal static void InsertFrame(TSD.ViewBase view, TSG.Point point, string label, TSD.Grid grid,
            double frameHeight, double frameWidth, TSD.FrameTypes frameType)
        {
            TSD.DrawingColors colorFrame = grid.Attributes.Frame.Color;
            TSD.FrameTypes type = grid.Attributes.Frame.Type;
            if (type == TSD.FrameTypes.Circle)
            {
                double scale = ((TSD.View)view).Attributes.Scale;
                double circleDiam = frameWidth;
                if (circleDiam == 0) circleDiam = 5 * scale;
                TSD.Circle circle = new TSD.Circle(view, point, 0.5 * circleDiam);
                TSD.Circle.CircleAttributes circleAttributes = new TSD.Circle.CircleAttributes();
                circleAttributes.Line.Color = colorFrame;
                circle.Attributes = circleAttributes;
                circle.Insert();
                circle.Modify();
            }
            else if (type == TSD.FrameTypes.Rectangular)
            {
                double height = frameHeight;
                double width = frameWidth;

                TSD.Line.LineAttributes lineAttributes = new TSD.Line.LineAttributes();

                lineAttributes.Line =
                    new TSD.LineTypeAttributes(TSD.LineTypes.SolidLine, colorFrame);
                TSD.Line line1 = new TSD.Line(view, point + new TSG.Point(-width / 2, height / 2), point + new TSG.Point(width / 2, height / 2));
                TSD.Line line2 = new TSD.Line(view, point + new TSG.Point(width / 2, height / 2), point + new TSG.Point(width / 2, -height / 2));
                TSD.Line line3 = new TSD.Line(view, point + new TSG.Point(width / 2, -height / 2), point + new TSG.Point(-width / 2, -height / 2));
                TSD.Line line4 = new TSD.Line(view, point + new TSG.Point(-width / 2, -height / 2), point + new TSG.Point(-width / 2, height / 2));
                line1.Attributes = lineAttributes;
                line2.Attributes = lineAttributes;
                line3.Attributes = lineAttributes;
                line4.Attributes = lineAttributes;
                line1.Insert();
                line2.Insert();
                line3.Insert();
                line4.Insert();
                line1.Modify();
                line2.Modify();
                line3.Modify();
                line4.Modify();
            }
            else if (type == TSD.FrameTypes.Sharpened)
            {
                double height = frameHeight;
                double width = frameWidth;

                TSD.Line.LineAttributes lineAttributes = new TSD.Line.LineAttributes();

                lineAttributes.Line =
                    new TSD.LineTypeAttributes(TSD.LineTypes.SolidLine, colorFrame);

                TSD.Line line1 = new TSD.Line(view, point + new TSG.Point(-width / 2, 0), point + new TSG.Point(-width / 2 + height / 2, height / 2));
                TSD.Line line2 = new TSD.Line(view, point + new TSG.Point(-width / 2 + height / 2, height / 2), point + new TSG.Point(width / 2 - height / 2, height / 2));
                TSD.Line line3 = new TSD.Line(view, point + new TSG.Point(width / 2 - height / 2, height / 2), point + new TSG.Point(width / 2, 0));
                TSD.Line line4 = new TSD.Line(view, point + new TSG.Point(width / 2, 0), point + new TSG.Point(width / 2 - height / 2, -height / 2));
                TSD.Line line5 = new TSD.Line(view, point + new TSG.Point(width / 2 - height / 2, -height / 2), point + new TSG.Point(-width / 2 + height / 2, -height / 2));
                TSD.Line line6 = new TSD.Line(view, point + new TSG.Point(-width / 2 + height / 2, -height / 2), point + new TSG.Point(-width / 2, 0));

                line1.Attributes = lineAttributes;
                line2.Attributes = lineAttributes;
                line3.Attributes = lineAttributes;
                line4.Attributes = lineAttributes;
                line5.Attributes = lineAttributes;
                line6.Attributes = lineAttributes;
                line1.Insert();
                line2.Insert();
                line3.Insert();
                line4.Insert();
                line5.Insert();
                line6.Insert();
                line1.Modify();
                line2.Modify();
                line3.Modify();
                line4.Modify();
                line5.Modify();
                line6.Modify();
            }
            else if (type == TSD.FrameTypes.Round)
            {
                double height = frameHeight;
                double width = frameWidth;

                TSG.Point pt1center = point + new TSG.Point(-width / 2 + height / 2, 0);
                TSG.Point pt2center = point + new TSG.Point(width / 2 - height / 2, 0);

                TSG.Point pt1 = point + new TSG.Point(-width / 2 + height / 2, -height / 2);
                TSG.Point pt2 = point + new TSG.Point(-width / 2, 0);
                TSG.Point pt3 = point + new TSG.Point(-width / 2 + height / 2, height / 2);
                TSG.Point pt4 = point + new TSG.Point(width / 2 - height / 2, height / 2);
                TSG.Point pt5 = point + new TSG.Point(width / 2, 0);
                TSG.Point pt6 = point + new TSG.Point(width / 2 - height / 2, -height / 2);

                TSD.Arc.ArcAttributes arcAttributes = new TSD.Arc.ArcAttributes();
                arcAttributes.Line = new TSD.LineTypeAttributes(TSD.LineTypes.SolidLine, colorFrame);

                TSD.Line.LineAttributes lineAttributes = new TSD.Line.LineAttributes();

                lineAttributes.Line =
                    new TSD.LineTypeAttributes(TSD.LineTypes.SolidLine, colorFrame);

                TSD.Arc arc1 = new TSD.Arc(view, pt1, pt3, pt1center);
                TSD.Arc arc2 = new TSD.Arc(view, pt4, pt6, pt2center);

                arc1.Attributes = arcAttributes;
                arc2.Attributes = arcAttributes;

                TSD.Line line1 = new TSD.Line(view, pt3, pt4, lineAttributes);
                TSD.Line line2 = new TSD.Line(view, pt6, pt1, lineAttributes);

                arc1.Insert();
                arc1.Modify();

                arc2.Insert();
                arc2.Modify();

                line1.Insert();
                line1.Modify();

                line2.Insert();
                line2.Modify();

            }
        }

    }
}

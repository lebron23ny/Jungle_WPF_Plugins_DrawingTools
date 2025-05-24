using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TSM = Tekla.Structures.Model;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;



namespace Jungle_WPF_Plugins_DrawingTools.DimensionToAxis
{
    public class MyGrid
    {
        public TSM.Grid Tekla_Grid_Model { get; set; }
        public TSD.Grid Tekla_Grid_Drawing { get; set; }
        public TSD.View View { get; set; }
        TSG.Point FirstPoint { get; set; }
        TSG.Point SecondPoint { get; set; }

        TSG.Point FirstPointGlobal { get; set; }
        TSG.Point SecondPointGlobal { get; set; }
        public List<string> LabelsX { get; set; }
        public List<string> LabelsY { get; set; }
        public List<double> CoordsX { get; set; }
        public List<double> CoordsY { get; set; }

        public ModePoint ModePoint { get; set; }

        public double PreviousDistance { get; set; }
        public double NextDistance { get; set; }

        public string PreviousMark { get; set; }
        public string NextMark { get; set; }

        public string CurrentMark { get; set; }

        public Orientation Orientation { get; set; }
        TSG.Matrix Matrix_from_Display_To_Global { get; set; }

        TSG.Matrix Matrix_from_Global_To_Display { get; set; }




        const double distanceSize = 5;


        double Value { get; set; }
        public MyGrid(TSD.Grid gridDRW,
            TSG.Point firstPoint,
            TSG.Point secondPoint,
            TSD.View view)
        {
            TSM.TransformationPlane transformationPlane = new TSM.TransformationPlane();
            TSM.Model model = new TSM.Model();
            TSM.WorkPlaneHandler workPlaneHandler = model.GetWorkPlaneHandler();
            workPlaneHandler.SetCurrentTransformationPlane(transformationPlane);


            Tekla_Grid_Model = new TSM.Model().SelectModelObject(gridDRW.ModelIdentifier) as TSM.Grid;
            Tekla_Grid_Drawing = gridDRW;
            View = view;
            FirstPoint = firstPoint;
            SecondPoint = secondPoint;
            LabelsX = Tekla_Grid_Model.LabelX.Split(' ').ToList();
            LabelsY = Tekla_Grid_Model.LabelY.Split(' ').ToList();
            CoordsX = StringParse(Tekla_Grid_Model.CoordinateX, Tekla_Grid_Model.Origin.X);
            CoordsY = StringParse(Tekla_Grid_Model.CoordinateY, Tekla_Grid_Model.Origin.Y);

            //TSM.TransformationPlane transformationPlane = new TSM.TransformationPlane();
            //TSM.Model model = new TSM.Model();
            //TSM.WorkPlaneHandler workPlaneHandler = model.GetWorkPlaneHandler();
            //workPlaneHandler.SetCurrentTransformationPlane(transformationPlane);



            TSG.CoordinateSystem displayCS = View.DisplayCoordinateSystem;
            //TSM.TransformationPlane tPlane = new TSM.TransformationPlane(displayCS);
            TSM.TransformationPlane tPlane = new TSM.TransformationPlane(displayCS.Origin, displayCS.AxisX, displayCS.AxisY);
            Matrix_from_Display_To_Global = tPlane.TransformationMatrixToGlobal;
            Matrix_from_Global_To_Display = tPlane.TransformationMatrixToLocal;

            FirstPointGlobal = Matrix_from_Display_To_Global.Transform(firstPoint);
            SecondPointGlobal = Matrix_from_Display_To_Global.Transform(secondPoint);

            SetOrientation(FirstPointGlobal, SecondPointGlobal, displayCS);
            SetMarkPoint();
        }


        public void SetMarkPoint()
        {
            if (Orientation == Orientation.X)
                SetMarkPoint(Value, CoordsX, LabelsX);
            else if (Orientation == Orientation.Y)
                SetMarkPoint(Value, CoordsY, LabelsY);

        }

        public void DrawAxis()
        {
            TSD.LineTypeAttributes lineTypeAttr = Tekla_Grid_Drawing.Attributes.Line;
            //lineTypeAttr.Color = DrawingColors.Green;

            TSD.ArrowheadAttributes arrowheadAttributes = new TSD.ArrowheadAttributes(TSD.ArrowheadPositions.None, TSD.ArrowheadTypes.NoArrow, 0, 0);


            TSD.Line.LineAttributes lineAttributes = new TSD.Line.LineAttributes();

            lineAttributes.Line = lineTypeAttr;
            lineAttributes.Arrowhead = arrowheadAttributes;

            TSD.Line axis = new TSD.Line(View, FirstPoint, SecondPoint, lineAttributes);
            axis.Insert();
        }

        public void DrawNextAxis(double distance, string nameAttrDim, string nameTextAttr, double lengthAxis)
        {

            if (Orientation != Orientation.none)
            {
                TSG.Point pt1;
                TSG.Point pt2;
                TSG.Point pt1_Display;
                TSG.Point pt2_Display;
                if (Orientation == Orientation.X)
                {
                    pt1 = new TSG.Point(FirstPointGlobal.X + distance, FirstPointGlobal.Y, FirstPointGlobal.Z);
                    pt2 = new TSG.Point(SecondPointGlobal.X + distance, SecondPointGlobal.Y, SecondPointGlobal.Z);
                }

                else
                {
                    pt1 = new TSG.Point(FirstPointGlobal.X, FirstPointGlobal.Y + distance, FirstPointGlobal.Z);
                    pt2 = new TSG.Point(SecondPointGlobal.X, SecondPointGlobal.Y + distance, SecondPointGlobal.Z);
                }
                pt1_Display = Matrix_from_Global_To_Display.Transform(pt1);
                pt2_Display = Matrix_from_Global_To_Display.Transform(pt2);

                TSD.LineTypeAttributes lineTypeAttr = Tekla_Grid_Drawing.Attributes.Line;
                TSD.ArrowheadAttributes arrowheadAttributes = new TSD.ArrowheadAttributes(TSD.ArrowheadPositions.None, TSD.ArrowheadTypes.NoArrow, 0, 0);
                TSD.Line.LineAttributes lineAttributes = new TSD.Line.LineAttributes();

                lineAttributes.Line = lineTypeAttr;
                lineAttributes.Arrowhead = arrowheadAttributes;

                //TSD.Line axis = new TSD.Line(View, pt1_Display, pt2_Display, lineAttributes);
                //axis.Insert();

                InsertAxis(View, pt1_Display, pt2_Display, lineAttributes, lengthAxis);



                InsertFrame(pt1_Display, pt2_Display, NextMark);
                InsertDimension(SecondPoint, pt2_Display, nameAttrDim);


                TSG.Point ptInsertText = new TSG.Point((pt2_Display.X + SecondPoint.X) / 2,
                        (pt2_Display.Y + SecondPoint.Y) / 2,
                        (pt2_Display.Z + SecondPoint.Z) / 2);

                InsertValueDimension(Convert.ToInt32(NextDistance).ToString(), nameTextAttr, ptInsertText);
            }
        }

        private void InsertAxis(TSD.View view, TSG.Point pt1_Display, TSG.Point pt2_Display, TSD.Line.LineAttributes lineAttributes, double lengthAxis)
        {
            double lengthVector = TSG.Distance.PointToPoint(pt2_Display, pt1_Display);
            if(lengthAxis == 0)
                lengthAxis = lengthVector;
            TSG.Point middelePoint = new TSG.Point(
                pt1_Display.X + (pt2_Display.X - pt1_Display.X) * (lengthVector - lengthAxis) / lengthVector,
                pt1_Display.Y + (pt2_Display.Y - pt1_Display.Y) * (lengthVector - lengthAxis) / lengthVector,
                pt1_Display.Z + (pt2_Display.Z - pt1_Display.Z) * (lengthVector - lengthAxis) / lengthVector
                );


            TSD.Line axis = new TSD.Line(View, middelePoint, pt2_Display, lineAttributes);
            axis.Insert(); ;
        }

        public void DrawPreviousAxis(double distance, string nameAttrDim, string nameTextAttr, double lengthAxis)
        {
            if (Orientation != Orientation.none)
            {
                TSG.Point pt1;
                TSG.Point pt2;
                TSG.Point pt1_Display;
                TSG.Point pt2_Display;
                if (Orientation == Orientation.X)
                {
                    pt1 = new TSG.Point(FirstPointGlobal.X - distance, FirstPointGlobal.Y, FirstPointGlobal.Z);
                    pt2 = new TSG.Point(SecondPointGlobal.X - distance, SecondPointGlobal.Y, SecondPointGlobal.Z);
                }

                else
                {
                    pt1 = new TSG.Point(FirstPointGlobal.X, FirstPointGlobal.Y - distance, FirstPointGlobal.Z);
                    pt2 = new TSG.Point(SecondPointGlobal.X, SecondPointGlobal.Y - distance, SecondPointGlobal.Z);
                }
                pt1_Display = Matrix_from_Global_To_Display.Transform(pt1);
                pt2_Display = Matrix_from_Global_To_Display.Transform(pt2);

                TSD.LineTypeAttributes lineTypeAttr = Tekla_Grid_Drawing.Attributes.Line;
                TSD.ArrowheadAttributes arrowheadAttributes = new TSD.ArrowheadAttributes(TSD.ArrowheadPositions.None, TSD.ArrowheadTypes.NoArrow, 0, 0);
                TSD.Line.LineAttributes lineAttributes = new TSD.Line.LineAttributes();
                lineAttributes.Line = lineTypeAttr;
                lineAttributes.Arrowhead = arrowheadAttributes;

                //TSD.Line axis = new TSD.Line(View, pt1_Display, pt2_Display, lineAttributes);
                //axis.Insert();
                InsertAxis(View, pt1_Display, pt2_Display, lineAttributes, lengthAxis);


                InsertFrame(pt1_Display, pt2_Display, PreviousMark);
                InsertDimension(SecondPoint, pt2_Display, nameAttrDim);

                TSG.Point ptInsertText = new TSG.Point((pt2_Display.X + SecondPoint.X) / 2,
                        (pt2_Display.Y + SecondPoint.Y) / 2,
                        (pt2_Display.Z + SecondPoint.Z) / 2);

                InsertValueDimension(Convert.ToInt32(PreviousDistance).ToString(), nameTextAttr, ptInsertText);
            }
        }


        private void InsertFrame(TSG.Point point1, TSG.Point point2, string label)
        {
            TSD.DrawingColors colorFrame = Tekla_Grid_Drawing.Attributes.Frame.Color;
            TSD.FrameTypes type = Tekla_Grid_Drawing.Attributes.Frame.Type;

            var objGrid = Tekla_Grid_Drawing.GetObjects();
            objGrid.MoveNext();
            TSD.GridLine gridLine = objGrid.Current as TSD.GridLine;


            double frameHeight = gridLine.StartLabel.FrameHeight;
            double frameWidth = gridLine.StartLabel.FrameWidth;


            TSG.Vector vector1 = new TSG.Vector(SecondPoint.X - FirstPoint.X, SecondPoint.Y - FirstPoint.Y, SecondPoint.Z - FirstPoint.Z);
            TSG.Vector vector = new TSG.Vector(point2.X - point1.X, point2.Y - point1.Y, point2.Z - point1.Z);
            if (TSG.Parallel.VectorToVector(vector1, new TSG.Vector(1, 0, 0)))
                vector *= (frameWidth / 2 + vector.GetLength()) / vector.GetLength();
            else
                vector *= (frameHeight / 2 + vector.GetLength()) / vector.GetLength();
            TSG.Point pointInsert = new TSG.Point(point1.X + vector.X, point1.Y + vector.Y, point1.Z + vector.Z);
            if (type == TSD.FrameTypes.Circle)
            {
                double scale = View.Attributes.Scale;
                double circleDiam = frameWidth;
                if (circleDiam == 0) circleDiam = 5 * scale;
                TSD.Circle circle = new TSD.Circle(View, pointInsert, 0.5 * circleDiam);
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
                TSD.Line line1 = new TSD.Line(View, pointInsert + new TSG.Point(-width / 2, height / 2), pointInsert + new TSG.Point(width / 2, height / 2));
                TSD.Line line2 = new TSD.Line(View, pointInsert + new TSG.Point(width / 2, height / 2), pointInsert + new TSG.Point(width / 2, -height / 2));
                TSD.Line line3 = new TSD.Line(View, pointInsert + new TSG.Point(width / 2, -height / 2), pointInsert + new TSG.Point(-width / 2, -height / 2));
                TSD.Line line4 = new TSD.Line(View, pointInsert + new TSG.Point(-width / 2, -height / 2), pointInsert + new TSG.Point(-width / 2, height / 2));
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

                TSD.Line line1 = new TSD.Line(View, pointInsert + new TSG.Point(-width / 2, 0), pointInsert + new TSG.Point(-width / 2 + height / 2, height / 2));
                TSD.Line line2 = new TSD.Line(View, pointInsert + new TSG.Point(-width / 2 + height / 2, height / 2), pointInsert + new TSG.Point(width / 2 - height / 2, height / 2));
                TSD.Line line3 = new TSD.Line(View, pointInsert + new TSG.Point(width / 2 - height / 2, height / 2), pointInsert + new TSG.Point(width / 2, 0));
                TSD.Line line4 = new TSD.Line(View, pointInsert + new TSG.Point(width / 2, 0), pointInsert + new TSG.Point(width / 2 - height / 2, -height / 2));
                TSD.Line line5 = new TSD.Line(View, pointInsert + new TSG.Point(width / 2 - height / 2, -height / 2), pointInsert + new TSG.Point(-width / 2 + height / 2, -height / 2));
                TSD.Line line6 = new TSD.Line(View, pointInsert + new TSG.Point(-width / 2 + height / 2, -height / 2), pointInsert + new TSG.Point(-width / 2, 0));

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

                TSG.Point pt1center = pointInsert + new TSG.Point(-width / 2 + height / 2, 0);
                TSG.Point pt2center = pointInsert + new TSG.Point(width / 2 - height / 2, 0);

                TSG.Point pt1 = pointInsert + new TSG.Point(-width / 2 + height / 2, -height / 2);
                TSG.Point pt2 = pointInsert + new TSG.Point(-width / 2, 0);
                TSG.Point pt3 = pointInsert + new TSG.Point(-width / 2 + height / 2, height / 2);
                TSG.Point pt4 = pointInsert + new TSG.Point(width / 2 - height / 2, height / 2);
                TSG.Point pt5 = pointInsert + new TSG.Point(width / 2, 0);
                TSG.Point pt6 = pointInsert + new TSG.Point(width / 2 - height / 2, -height / 2);

                TSD.Arc.ArcAttributes arcAttributes = new TSD.Arc.ArcAttributes();
                arcAttributes.Line = new TSD.LineTypeAttributes(TSD.LineTypes.SolidLine, colorFrame);

                TSD.Line.LineAttributes lineAttributes = new TSD.Line.LineAttributes();

                lineAttributes.Line =
                    new TSD.LineTypeAttributes(TSD.LineTypes.SolidLine, colorFrame);

                TSD.Arc arc1 = new TSD.Arc(View, pt1, pt3, pt1center);
                TSD.Arc arc2 = new TSD.Arc(View, pt4, pt6, pt2center);

                arc1.Attributes = arcAttributes;
                arc2.Attributes = arcAttributes;

                TSD.Line line1 = new TSD.Line(View, pt3, pt4, lineAttributes);
                TSD.Line line2 = new TSD.Line(View, pt6, pt1, lineAttributes);

                arc1.Insert();
                arc1.Modify();

                arc2.Insert();
                arc2.Modify();

                line1.Insert();
                line1.Modify();

                line2.Insert();
                line2.Modify();

            }

            InsertSymbolAxis(pointInsert, label);

        }

        private void InsertSymbolAxis(TSG.Point point, string label)
        {
            try
            {
                string fontName = Tekla_Grid_Drawing.Attributes.Font.Name;
                TSD.DrawingColors colorFont = Tekla_Grid_Drawing.Attributes.Font.Color;
                double heightFont = Tekla_Grid_Drawing.Attributes.Font.Height;
                bool boldFont = Tekla_Grid_Drawing.Attributes.Font.Bold;
                bool italicFont = Tekla_Grid_Drawing.Attributes.Font.Italic;
                TSD.Text text = new TSD.Text(View, point, label);
                TSD.Text.TextAttributes textAttributes = new TSD.Text.TextAttributes();
                textAttributes.Font = new TSD.FontAttributes(colorFont, heightFont, fontName, italicFont, boldFont);

                textAttributes.Frame = new TSD.Frame(TSD.FrameTypes.None, TSD.DrawingColors.Red);
                textAttributes.ArrowHead = new TSD.ArrowheadAttributes(TSD.ArrowheadPositions.None, TSD.ArrowheadTypes.NoArrow, 1, 1);

                text.Attributes = textAttributes;
                text.Insert();
                text.Modify();

            }

            catch { }
        }

        private void SetOrientation(TSG.Point firstPoint, TSG.Point secondPoint, TSG.CoordinateSystem displayCS)
        {
            TSG.Vector vector = new TSG.Vector(
                secondPoint.X - firstPoint.X,
                secondPoint.Y - firstPoint.Y,
                secondPoint.Z - firstPoint.Z);
            if (TSG.Parallel.VectorToVector(vector, new TSG.Vector(0, 1, 0)))
            {
                Orientation = Orientation.X;
                Value = Math.Round(firstPoint.X, 5);
                return;
            }
            else if (TSG.Parallel.VectorToVector(vector, new TSG.Vector(1, 0, 0)))
            {
                Orientation = Orientation.Y;
                Value = Math.Round(firstPoint.Y, 5);
                return;
            }
            else
            {
                TSG.Vector normalVectorCS = displayCS.AxisX.Cross(displayCS.AxisY).GetNormal();
                if (TSG.Parallel.VectorToVector(normalVectorCS, new TSG.Vector(0, 1, 0)))
                {
                    Orientation = Orientation.X;
                    Value = Math.Round(firstPoint.X, 5);
                    return;
                }
                else if (TSG.Parallel.VectorToVector(normalVectorCS, new TSG.Vector(1, 0, 0)))
                {
                    Orientation = Orientation.Y;
                    Value = Math.Round(firstPoint.Y, 5);
                    return;
                }
                else
                {
                    Orientation = Orientation.none;
                    return;
                }
            }

        }

        private void SetMarkPoint(double value, List<double> Coords, List<string> Labels)
        {
            if (value < Coords[0])
            {
                ModePoint = ModePoint.Left;
                PreviousMark = string.Empty;
                PreviousDistance = 0;
                NextMark = Labels.First();
                NextDistance = Coords.First() - value;
                CurrentMark = string.Empty;

                return;
            }
            if (value > Coords[Coords.Count - 1])
            {
                ModePoint = ModePoint.Right;
                PreviousMark = Labels[Coords.Count - 1];
                PreviousDistance = value - Coords[Coords.Count - 1];
                NextMark = string.Empty;
                NextDistance = 0;
                CurrentMark = string.Empty;
                return;
            }
            for (int i = 0; i <= Coords.Count - 1; i++)
            {
                if (value == Coords[i])
                {
                    ModePoint = ModePoint.Border;
                    if (i == 0)
                    {
                        PreviousMark = string.Empty;
                        PreviousDistance = 0;

                        NextMark = Labels[1];
                        NextDistance = Coords[1] - value;

                        CurrentMark = Labels[0];
                        return;
                    }
                    else if (i == Coords.Count - 1)
                    {
                        PreviousMark = Labels[Coords.Count - 2];
                        PreviousDistance = value - Coords[Coords.Count - 2];

                        CurrentMark = Labels[Coords.Count - 1];
                        NextMark = string.Empty;
                        NextDistance = 0;
                        return;
                    }
                    else
                    {
                        PreviousMark = Labels[i - 1];
                        PreviousDistance = value - Coords[i - 1];
                        NextMark = Labels[i + 1];
                        NextDistance = Coords[i + 1] - value;
                        CurrentMark = Labels[i];
                        return;
                    }
                }
                else if (value > Coords[i] && value < Coords[i + 1])
                {
                    ModePoint = ModePoint.BetweenBorder;
                    PreviousMark = Labels[i];
                    PreviousDistance = value - Coords[i];
                    NextMark = Labels[i + 1];
                    NextDistance = Coords[i + 1] - value;
                    return;
                }
            }

        }

        private void InsertDimension(TSG.Point point1, TSG.Point point2, string nameAttr)
        {
            TSD.PointList pointList = new TSD.PointList();
            pointList.Add(point1);
            pointList.Add(point2);

            TSG.Vector directionDimVector = new TSG.Vector(FirstPoint.X - SecondPoint.X, FirstPoint.Y - SecondPoint.Y, FirstPoint.Z - SecondPoint.Z);
            //directionDimVector.Normalize();
            //directionDimVector *= 0.5;

            TSD.StraightDimensionSet.StraightDimensionSetAttributes attr = new TSD.StraightDimensionSet.StraightDimensionSetAttributes(null, nameAttr);
            TSD.StraightDimensionSet xDEnsion =
                (new TSD.StraightDimensionSetHandler()).CreateDimensionSet(View, pointList, directionDimVector, distanceSize, attr);

        }


        private void InsertValueDimension(string valueDimension, string nameTextAttr, TSG.Point pointInsert)
        {
            TSD.Text.TextAttributes textAttributes = new TSD.Text.TextAttributes(nameTextAttr);
            double scale = View.Attributes.Scale;
            double gap = 1;
            TSD.Text textTemp = new TSD.Text(View, new TSG.Point(0, 0, 0), valueDimension, textAttributes);
            textTemp.Insert();

            var sizeText = textTemp.GetAxisAlignedBoundingBox();
            double height = sizeText.Height;
            double width = sizeText.Width;


            TSG.Vector vectorDir = new TSG.Vector(SecondPoint.X - FirstPoint.X, SecondPoint.Y - FirstPoint.Y, SecondPoint.Z - FirstPoint.Z);
            TSD.Text text;


            if (TSG.Parallel.VectorToVector(vectorDir, new TSG.Vector(0, 1, 0)))
            {
                double dir = vectorDir.Dot(new TSG.Vector(0, 1, 0));
                if (dir > 0)
                {
                    pointInsert.Y -= scale * (distanceSize - gap) - height / 2;
                    text = new TSD.Text(View, pointInsert, valueDimension, textAttributes);
                    text.Insert();
                }
                else
                {
                    pointInsert.Y += scale * (distanceSize + gap) + height / 2;
                    text = new TSD.Text(View, pointInsert, valueDimension, textAttributes);
                    text.Insert();
                }

            }
            else if (TSG.Parallel.VectorToVector(vectorDir, new TSG.Vector(1, 0, 0)))
            {

                textAttributes.Angle = 90;
                double dir = vectorDir.Dot(new TSG.Vector(1, 0, 0));
                if (dir > 0)
                {
                    pointInsert.X -= (scale * (distanceSize + gap) + height / 2);
                    text = new TSD.Text(View, pointInsert, valueDimension, textAttributes);
                    text.Insert();
                }
                else
                {
                    pointInsert.X += (scale * (distanceSize - gap) - height / 2);
                    text = new TSD.Text(View, pointInsert, valueDimension, textAttributes);
                    text.Insert();
                }


            }
            textTemp.Delete();

        }

        private static List<double> StringParse(string distance, double start)
        {
            NumberFormatInfo format = new NumberFormatInfo();
            format.NumberDecimalSeparator = ".";

            double tempcoord = start;
            List<double> distanceData = new List<double>();
            ArrayList data = new ArrayList();
            string[] groups = distance.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string group in groups)
            {
                string[] subgroup = group.Split(new char[] { '*' });
                if (subgroup.Length == 1)
                {
                    tempcoord += Convert.ToDouble(subgroup[0], format);
                    distanceData.Add(tempcoord);

                }
                else
                {

                    for (int i = 0; i < Convert.ToInt32(subgroup[0]); i++)
                    {
                        tempcoord += Convert.ToDouble(subgroup[1], format);
                        distanceData.Add(tempcoord);
                    }
                }
            }
            return distanceData;
        }
    }


    public enum ModePoint
    {
        Left = 0,
        Border = 1,
        BetweenBorder = 2,
        Right = 3,
    }

    public enum Orientation
    {

        X,
        Y,
        none
    }
}

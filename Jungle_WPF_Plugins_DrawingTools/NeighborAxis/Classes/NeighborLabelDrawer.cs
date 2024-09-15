using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSG = Tekla.Structures.Geometry3d;
using TSD = Tekla.Structures.Drawing;
using TSM = Tekla.Structures.Model;


namespace Jungle_WPF_Plugins_DrawingTools.NeighborAxis.Classes
{
    class NeighborLabelDrawer
    {
        private TSG.Point _pickedPoint;
        private TSD.View _viewBase;
        private TSD.Grid _fatherGrid;
        private TSD.GridLine _relatedGridLine;
        private bool _isStartLabel;
        private bool _isParallelToX;

        private double _frameHeight;
        private double _frameWidth;
        private string _nextLabelText;
        private string _prevLabelText;
        private enumPositionDraw _positionDraw;

        private IDrawLabel _drawNeighborLabel;

        private TSD.FrameTypes _frameType;

        public TSG.Point PickedPoint { get { return _pickedPoint; } set { _pickedPoint = value; } }
        public TSD.View ViewBase { get { return _viewBase; } set { _viewBase = value; } }
        public TSD.Grid FatherGrid { get { return _fatherGrid; } set { _fatherGrid = value; } }
        public TSD.GridLine RelatedGridLine { get { return _relatedGridLine; } set { _relatedGridLine = value; } }

        public bool IsStartLabel { get { return _isStartLabel; } set { _isStartLabel = value; } }
        public bool IsParallelToX { get { return _isParallelToX; } set { _isParallelToX = value; } }
        public double FrameHeight { get { return _frameHeight; } set { _frameHeight = value; } }
        public double FrameWidth { get { return _frameWidth; } set { _frameWidth = value; } }
        public string NextLabelText { get { return _nextLabelText; } set { _nextLabelText = value; } }
        public string PrevLabelText { get { return _prevLabelText; } set { _prevLabelText = value; } }
        public TSD.FrameTypes FrameTypes { get { return _frameType; } set { _frameType = value; } }

        public enumPositionDraw PositionDraw { get { return _positionDraw; } set { _positionDraw = value; } }

        internal IDrawLabel DrawNeighborLabel1 { get { return _drawNeighborLabel; } set { _drawNeighborLabel = value; } }

        public NeighborLabelDrawer(TSG.Point pickedPoint, TSD.View viewBase, TSD.Grid grid, enumPositionDraw position)
        {
            _pickedPoint = pickedPoint;
            _viewBase = viewBase;
            _fatherGrid = grid;
            _positionDraw = position;
            GetRelatedGridLineProperties();
            FindNextAndPrevLabel();
            GetOrientation();
            SetFrameType();
        }


        public void SetLabelLocation(IDrawLabel drawNeighborLabel)
        {
            this._drawNeighborLabel = drawNeighborLabel;
        }

        public void DrawNeighborLabel()
        {
            _drawNeighborLabel.DrawLabel(_pickedPoint, _viewBase, _frameHeight, _frameWidth, _nextLabelText, _prevLabelText, _fatherGrid, _positionDraw, _frameType);
        }

        protected void SetFrameType()
        {
            _frameType = _fatherGrid.Attributes.Frame.Type;
        }

        /// <summary>
        /// Метод присваивает значения полям, относящимся к выбранной оси
        /// </summary>
        protected void GetRelatedGridLineProperties()
        {
            double distBetweenPointAndStart; 
            double distBetweenPointAndEnd;
            Dictionary<TSD.GridLine, double> lengths = new Dictionary<TSD.GridLine, double>();
            Dictionary<TSD.GridLine, bool> startLabels = new Dictionary<TSD.GridLine, bool>();

            TSD.DrawingObjectEnumerator gridLines = _fatherGrid.GetObjects();
            while (gridLines.MoveNext())
            {

                TSD.GridLine gl = gridLines.Current as TSD.GridLine;
                distBetweenPointAndStart = TSG.Distance.PointToPoint(_pickedPoint, gl.StartLabel.CenterPoint);
                distBetweenPointAndEnd = TSG.Distance.PointToPoint(_pickedPoint, gl.EndLabel.CenterPoint);

                lengths.Add(gl, Math.Min(distBetweenPointAndEnd, distBetweenPointAndStart));


                if (distBetweenPointAndEnd < distBetweenPointAndStart)
                {
                    _isStartLabel = false;
                }
                else
                {
                    _isStartLabel = true;
                }
                startLabels.Add(gl, _isStartLabel);
            }

            var minKeyValue = lengths.OrderBy(kvp => kvp.Value).First();
            _relatedGridLine = minKeyValue.Key;

            _isStartLabel = startLabels[_relatedGridLine];

            if (_isStartLabel)
            {
                _frameHeight = _relatedGridLine.StartLabel.FrameHeight;
                _frameWidth = _relatedGridLine.StartLabel.FrameWidth;
            }
            else
            {
                _frameHeight = _relatedGridLine.EndLabel.FrameHeight;
                _frameWidth = _relatedGridLine.EndLabel.FrameWidth;
            }
        }

        /// <summary>
        /// Возваращает true если выбранная ось параллельна оси X местной системы координат координатной сетки
        /// </summary>
        protected void GetOrientation()
        {
            
            TSG.LineSegment gridLineAxisX = new TSG.LineSegment(_relatedGridLine.StartLabel.CenterPoint,
                _relatedGridLine.EndLabel.CenterPoint);

            
            TSM.Grid modelGrid = Extensions.SelectModelObject(_fatherGrid.ModelIdentifier.ID) as TSM.Grid;
            TSG.CoordinateSystem gridCS = modelGrid.GetCoordinateSystem();

            TSG.Vector gridLineX = gridLineAxisX.GetDirectionVector();
            
            TSG.Vector gridX = gridCS.AxisX;
            TSG.Vector gridY = gridCS.AxisY;

            TSG.Vector vecotorX_view = _viewBase.DisplayCoordinateSystem.AxisX;
            TSG.Vector vecotorY_view = _viewBase.DisplayCoordinateSystem.AxisY;

            if(TSG.Parallel.VectorToVector(gridX, new TSG.Vector(0, 1,0)) && 
                TSG.Parallel.VectorToVector(gridY, new TSG.Vector(1, 0, 0)))
            {
                if (TSG.Parallel.VectorToVector(gridLineX, gridX))
                    _isParallelToX = false;
                else
                    _isParallelToX = true;
            }
            else
            {
                if (TSG.Parallel.VectorToVector(gridLineX, gridX))
                    _isParallelToX = true;
                else
                    _isParallelToX = false;
            }

        }


        /// <summary>
        /// Метод заполняет поля: nextLabelText и prevLabelText
        /// </summary>
        protected void FindNextAndPrevLabel()
        {
            TSM.Grid modelGrid = Extensions.SelectModelObject(_fatherGrid.ModelIdentifier.ID) as TSM.Grid;
            string gridLabelsX = modelGrid.LabelX;
            string gridLabelsY = modelGrid.LabelY;

            string[] labelsX = gridLabelsX.Split(null);
            string[] labelsY = gridLabelsY.Split(null);

            string nextLabel, prevLabel, curLabel;
            if (!String.IsNullOrEmpty(_relatedGridLine.StartLabel.GridLabelText))
                curLabel = _relatedGridLine.StartLabel.GridLabelText;
            else
                curLabel = _relatedGridLine.EndLabel.GridLabelText;

            Extensions.FindNextAndPreviousLabelText(curLabel, labelsX, labelsY, out prevLabel, out nextLabel);

            _nextLabelText = nextLabel;
            _prevLabelText = prevLabel;
        }

    }
}

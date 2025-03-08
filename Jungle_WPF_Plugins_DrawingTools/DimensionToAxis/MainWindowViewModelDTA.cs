using Tekla.Structures.Datatype;
using Tekla.Structures.Dialog;
using TD = Tekla.Structures.Datatype;

namespace Jungle_WPF_Plugins_DrawingTools.DimensionToAxis
{
    public class MainWindowViewModelDTA:BaseViewModel
    {
        [StructuresDialog("IndexType", typeof(TD.Integer))]
        public TD.Integer IndexType { get; set; } = 0;


        [StructuresDialog("DistanceDim", typeof(TD.Double))]
        public double DistanceDim { get; set; } = 2000.0;


        [StructuresDialog("NameDimAttr", typeof(TD.String))]
        public string NameDimAttr { get; set; } = "standard";

        [StructuresDialog("NameTextAttr", typeof(TD.String))]
        public string NameTextAttr { get; set; } = "standard";

        [StructuresDialog("LengthAxis", typeof(TD.Double))]
        public double LengthAxis { get; set; } = 500.0;
    }
}

using Tekla.Structures.Dialog;
using TD = Tekla.Structures.Datatype;

namespace Jungle_WPF_Plugins_DrawingTools.NeighborAxis
{
    public class MainWindowViewModelNA : BaseViewModel
    {
        [StructuresDialog("IndexType", typeof(TD.Integer))]
        public TD.Integer IndexType { get; set; } = (TD.Integer)0;
    }
}

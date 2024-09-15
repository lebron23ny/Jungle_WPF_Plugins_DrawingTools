using Tekla.Structures.Dialog;
using TD = Tekla.Structures.Datatype;

namespace Jungle_WPF_Plugins_DrawingTools.OpeningSymbol
{
    public class MainWindowViewModelOS : BaseViewModel
    {


        [StructuresDialog("IndexType", typeof(TD.Integer))]
        public TD.Integer IndexType { get; set; } = (TD.Integer)0;


        [StructuresDialog("IsDrawFrameIndex", typeof(TD.Integer))]
        public TD.Integer IsDrawFrameIndex { get; set; } = (TD.Integer)0;
    }
}

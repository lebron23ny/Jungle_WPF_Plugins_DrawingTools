using Tekla.Structures.Dialog;
using TD = Tekla.Structures.Datatype;
using Jungle_WPF_Plugins_DrawingTools;


namespace Jungle_WPF_Plugins_DrawingTools.FragmentSymbol
{
    public class MainWindowViewModel : BaseViewModel
    {
        [StructuresDialog("SizeC", typeof(TD.Integer))]
        public TD.Integer SizeC { get; set; } = (TD.Integer)5;

        [StructuresDialog("HeightFont", typeof(TD.String))]
        public TD.String HeightFont { get; set; } = (TD.String)"2.5";

        [StructuresDialog("TextContent", typeof(TD.String))]
        public TD.String TextContent { get; set; } = (TD.String)"Фрагмент 1";

        [StructuresDialog("Direction", typeof(TD.Integer))]
        public TD.Integer Direction { get; set;} = (TD.Integer)0;
    }
}

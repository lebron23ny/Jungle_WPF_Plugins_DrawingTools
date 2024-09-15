using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Jungle_WPF_Plugins_DrawingTools
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        protected void Set<T>(ref T field, T value, [CallerMemberName] string name = null)
        {
            if (!field.Equals(value))
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }
}

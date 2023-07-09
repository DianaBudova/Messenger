using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Messenger.ViewModels;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        if (PropertyChanged is not null)
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
}

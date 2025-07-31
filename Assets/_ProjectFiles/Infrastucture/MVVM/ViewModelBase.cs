using System.Collections.Generic;
using System.ComponentModel;

public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected void RaisePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T backingField, T value, string propertyName)
    {
        if (EqualityComparer<T>.Default.Equals(backingField, value)) return false;
        backingField = value;
        RaisePropertyChanged(propertyName);
        return true;
    }
}
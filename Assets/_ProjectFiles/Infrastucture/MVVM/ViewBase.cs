using System.ComponentModel;
using UnityEngine;

public abstract class ViewBase<TViewModel> : MonoBehaviour where TViewModel : ViewModelBase
{
    [SerializeField] private TViewModel _viewModel;

    public TViewModel ViewModel
    {
        get => _viewModel;
        set
        {
            if (_viewModel != null)
                UnsubscribeToViewModel(_viewModel);

            _viewModel = value;

            if (_viewModel != null)
                SubscribeToViewModel(_viewModel);

            OnViewModelChanged();
        }
    }

    protected virtual void SubscribeToViewModel(TViewModel vm)
    {
        vm.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected virtual void UnsubscribeToViewModel(TViewModel vm)
    {
        vm.PropertyChanged -= OnViewModelPropertyChanged;
    }

    protected virtual void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
    }

    protected virtual void OnViewModelChanged()
    {
    }

    protected virtual void OnDestroy()
    {
        if (_viewModel != null)
            UnsubscribeToViewModel(_viewModel);
    }
}
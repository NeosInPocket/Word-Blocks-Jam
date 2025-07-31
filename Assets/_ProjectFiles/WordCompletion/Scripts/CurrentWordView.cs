using System.ComponentModel;
using TMPro;
using UnityEngine;

namespace _ProjectFiles.WordCompletion.Scripts
{
    public class CurrentWordView : ViewBase<CurrentWordViewModel>
    {
        [SerializeField] private TMP_Text wordText;

        protected override void OnViewModelChanged()
        {
            base.OnViewModelChanged();
            UpdateWordText();
        }

        protected override void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnViewModelPropertyChanged(sender, e);
            if (e.PropertyName == nameof(WordsQueue.CurrentWord))
                UpdateWordText();
        }

        private void UpdateWordText()
        {
            if (wordText != null)
                wordText.text = ViewModel?.CurrentWord ?? "No word";
        }
    }
}
namespace _ProjectFiles.WordCompletion.Scripts
{
    public class CurrentWordViewModel : ViewModelBase
    {
        private string _currentWord;

        public string CurrentWord
        {
            get => _currentWord;
            set => SetProperty(ref _currentWord, value, nameof(CurrentWord));
        }

        private readonly WordsQueue _model;

        public CurrentWordViewModel(WordsQueue model)
        {
            _model = model;
            _model.WordChanged += OnModelWordChanged;
            CurrentWord = _model.CurrentWord;
        }

        private void OnModelWordChanged(object sender, string newWord)
        {
            CurrentWord = newWord;
        }
    }
}
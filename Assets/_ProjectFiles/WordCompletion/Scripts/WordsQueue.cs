using System;
using System.Collections.Generic;

public class WordsQueue
{
    private readonly Queue<string> _words;

    public string CurrentWord { get; private set; }

    public event EventHandler<string> WordChanged;

    public WordsQueue(List<string> words)
    {
        _words = new Queue<string>(words);
    }

    public bool MoveNext()
    {
        if (_words.Count == 0)
        {
            CurrentWord = null;
            WordChanged?.Invoke(this, CurrentWord);
            return false;
        }

        CurrentWord = _words.Dequeue();
        WordChanged?.Invoke(this, CurrentWord);
        return true;
    }
}
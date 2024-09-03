using System.Collections.ObjectModel;
using System.Windows.Input;

public class AlphabetViewModel : BaseViewModel
{
    private LetterModel _currentLetter;
    private ObservableCollection<LetterModel> _toStudyLetters;
    private ObservableCollection<LetterModel> _learnedLetters;
    private ObservableCollection<LetterModel> _unknownLetters;

    public AlphabetViewModel()
    {
        ToStudyLetters = new ObservableCollection<LetterModel>();
        LearnedLetters = new ObservableCollection<LetterModel>();
        UnknownLetters = new ObservableCollection<LetterModel>();

        // Инициализация команд
        KnowLetterCommand = new RelayCommand(KnowLetter);
        DoNotKnowLetterCommand = new RelayCommand(DoNotKnowLetter);
        LoadAlphabetCommand = new RelayCommand(LoadAlphabet);

        // Загрузка алфавита при инициализации
        LoadAlphabet();
    }

    public LetterModel CurrentLetter
    {
        get => _currentLetter;
        set
        {
            _currentLetter = value;
            OnPropertyChanged();  // Уведомляем об изменении
        }
    }

    public ObservableCollection<LetterModel> ToStudyLetters
    {
        get => _toStudyLetters;
        set
        {
            _toStudyLetters = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<LetterModel> LearnedLetters
    {
        get => _learnedLetters;
        set
        {
            _learnedLetters = value;
            OnPropertyChanged();
        }
    }

    public ObservableCollection<LetterModel> UnknownLetters
    {
        get => _unknownLetters;
        set
        {
            _unknownLetters = value;
            OnPropertyChanged();
        }
    }

    public ICommand KnowLetterCommand { get; }
    public ICommand DoNotKnowLetterCommand { get; }
    public ICommand LoadAlphabetCommand { get; }

    private void KnowLetter()
    {
        if (CurrentLetter != null)
        {
            CurrentLetter.IsKnown = true;
            LearnedLetters.Add(CurrentLetter);
            MoveToNextLetter();
        }
    }

    private void DoNotKnowLetter()
    {
        if (CurrentLetter != null)
        {
            CurrentLetter.IsKnown = false;
            UnknownLetters.Add(CurrentLetter);
            MoveToNextLetter();
        }
    }

    private void LoadAlphabet()
    {
        ToStudyLetters.Clear();
        for (char c = 'A'; c <= 'Z'; c++)
        {
            ToStudyLetters.Add(new LetterModel { Symbol = c.ToString() });
        }
        MoveToNextLetter();
    }

    private void MoveToNextLetter()
    {
        if (ToStudyLetters.Count > 0)
        {
            CurrentLetter = ToStudyLetters[0];
            ToStudyLetters.RemoveAt(0);
        }
        else
        {
            CurrentLetter = null;
            // Логика завершения, когда все буквы пройдены
        }
    }
}

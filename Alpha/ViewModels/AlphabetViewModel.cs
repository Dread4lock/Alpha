using Alpha.Commands;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
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
        UploadAlphabetCommand = new RelayCommand(UploadAlphabet);
    }

    // Свойства для работы с буквами
    public LetterModel CurrentLetter
    {
        get => _currentLetter;
        set
        {
            _currentLetter = value;
            OnPropertyChanged();
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

    // Команды
    public ICommand KnowLetterCommand { get; }
    public ICommand DoNotKnowLetterCommand { get; }
    public ICommand UploadAlphabetCommand { get; }

    // Логика для обработки "знания" буквы
    private void KnowLetter()
    {
        if (CurrentLetter != null)
        {
            CurrentLetter.IsKnown = true;
            LearnedLetters.Add(CurrentLetter);
            MoveToNextLetter();
        }
    }

    // Логика для обработки "незнания" буквы
    private void DoNotKnowLetter()
    {
        if (CurrentLetter != null)
        {
            CurrentLetter.IsKnown = false;
            UnknownLetters.Add(CurrentLetter);
            MoveToNextLetter();
        }
    }

    // Логика для загрузки алфавита
    private void UploadAlphabet()
    {
        var openFileDialog = new OpenFileDialog
        {
            Filter = "Text files (*.txt)|*.txt",
            Title = "Select Alphabet File"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            var filePath = openFileDialog.FileName;

            var letters = File.ReadAllText(filePath);

            bool containsThaiLetters = false;

            foreach (var letter in letters)
            {
                if (IsThaiLetter(letter.ToString()))
                {
                    containsThaiLetters = true;
                    break;
                }
            }

            if (!containsThaiLetters)
            {
                // Логика обработки, если нет тайских букв (можно добавить сообщение или логику)
                return;
            }

            ToStudyLetters.Clear();

            foreach (var letter in letters)
            {
                if (!string.IsNullOrWhiteSpace(letter.ToString()))
                {
                    ToStudyLetters.Add(new LetterModel { Symbol = letter.ToString() });
                }
            }

            MoveToNextLetter();
        }
    }

    // Проверка на принадлежность к тайскому алфавиту
    private bool IsThaiLetter(string value)
    {
        return Regex.IsMatch(value, @"^[\u0E00-\u0E7F]$");
    }

    // Переход к следующей букве
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
            // Логика завершения
        }
    }
}

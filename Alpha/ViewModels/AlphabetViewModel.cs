using Alpha.Commands;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

public class AlphabetViewModel : BaseViewModel
{
    private LetterModel _currentLetter;
    private ObservableCollection<LetterModel> _toStudyLetters;
    private ObservableCollection<LetterModel> _learnedLetters;
    private ObservableCollection<LetterModel> _unknownLetters;
    private bool _isAlphabetUploaded = false;
    private string _warningMessage;
    private Visibility _warningVisibility = Visibility.Collapsed;

    // Свойства для отображения предупреждения
    public string WarningMessage
    {
        get => _warningMessage;
        set
        {
            _warningMessage = value;
            OnPropertyChanged();
            WarningVisibility = string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    public Visibility WarningVisibility
    {
        get => _warningVisibility;
        set
        {
            _warningVisibility = value;
            OnPropertyChanged();
        }
    }

    // Свойство, указывающее, что алфавит был загружен
    public bool IsAlphabetUploaded
    {
        get => _isAlphabetUploaded;
        set
        {
            _isAlphabetUploaded = value;
            OnPropertyChanged();
        }
    }

    public LetterModel CurrentLetter
    {
        get => _currentLetter;
        set
        {
            _currentLetter = value;
            Console.WriteLine("CurrentLetter set: " + (_currentLetter?.Symbol ?? "null"));  // Логирование
            OnPropertyChanged();
            RaiseCanExecuteChanged(); // Обновление доступности команд при изменении буквы
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
    public ICommand DownloadUnknownLettersCommand { get; }

    public AlphabetViewModel()
    {
        ToStudyLetters = new ObservableCollection<LetterModel>();
        LearnedLetters = new ObservableCollection<LetterModel>();
        UnknownLetters = new ObservableCollection<LetterModel>();

        // Инициализация команд
        KnowLetterCommand = new KnowLetterCommand(this);
        DoNotKnowLetterCommand = new DoNotKnowLetterCommand(this);
        UploadAlphabetCommand = new RelayCommand(UploadAlphabet);
        DownloadUnknownLettersCommand = new DownloadUnknownLettersCommand(this);

        RaiseCanExecuteChanged();  // вызываем в конструкторе для активации команд
    }

    public void KnowLetter()
    {
        if (CurrentLetter != null)
        {
            CurrentLetter.KnownCount++;
            Console.WriteLine("KnowLetter executed for: " + CurrentLetter.Symbol);

            if (CurrentLetter.KnownCount >= 2)
            {
                LearnedLetters.Add(CurrentLetter);
            }
            else
            {
                ToStudyLetters.Add(CurrentLetter);
            }

            MoveToNextLetter();
        }

        RaiseCanExecuteChanged();
        OnPropertyChanged(nameof(CurrentLetter));
    }

    public void DoNotKnowLetter()
    {
        if (CurrentLetter != null)
        {
            CurrentLetter.KnownCount = 0;
            Console.WriteLine("DoNotKnowLetter executed for: " + CurrentLetter.Symbol);
            UnknownLetters.Add(CurrentLetter);

            MoveToNextLetter();
        }

        RaiseCanExecuteChanged();
        OnPropertyChanged(nameof(CurrentLetter));
    }

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
            var letters = File.ReadAllText(filePath).Where(char.IsLetter).ToList();

            if (letters.Count == 0)
            {
                WarningMessage = "The file does not contain any valid letters.";
                return;
            }

            // Сброс предупреждения при новой загрузке
            WarningMessage = "";

            char firstLetter = letters[0];
            bool isThaiAlphabet = IsThaiLetter(firstLetter.ToString());

            foreach (var letter in letters)
            {
                if (IsThaiLetter(letter.ToString()) != isThaiAlphabet)
                {
                    WarningMessage = $"Not all letters belong to the same alphabet (based on {firstLetter}).";
                    break;
                }
            }

            IsAlphabetUploaded = true;
            UpdateLetters(letters);  // Обновляем список букв
            RaiseCanExecuteChanged();  // Обновляем команды
        }
    }

    public void UpdateLetters(List<char> letters)
    {
        ToStudyLetters.Clear();
        LearnedLetters.Clear();
        UnknownLetters.Clear();

        foreach (var letter in letters)
        {
            ToStudyLetters.Add(new LetterModel { Symbol = letter.ToString() });
        }

        if (ToStudyLetters.Count > 0)
        {
            CurrentLetter = ToStudyLetters[0];
            OnPropertyChanged(nameof(CurrentLetter));
        }
        else
        {
            CurrentLetter = null;
        }

        RaiseCanExecuteChanged();
        Console.WriteLine("UpdateLetters called"); // Логирование
    }

    private bool IsThaiLetter(string value)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(value, @"^[\u0E00-\u0E7F]$");
    }

    public void MoveToNextLetter()
    {
        if (ToStudyLetters.Count > 0)
        {
            CurrentLetter = ToStudyLetters[0];
            ToStudyLetters.RemoveAt(0);
            Console.WriteLine("MoveToNextLetter - CurrentLetter updated: " + CurrentLetter.Symbol);  // Логирование
        }
        else
        {
            CurrentLetter = null;
            DisplayUnknownLetters();
        }
        RaiseCanExecuteChanged();
    }

    private void DisplayUnknownLetters()
    {
        // Логика для отображения списка невыученных букв
    }

    public void CopyUnknownLetters()
    {
        var unknownLettersText = string.Join(", ", UnknownLetters.Select(letter => letter.Symbol));
        Clipboard.SetText(unknownLettersText);
    }

    private void RaiseCanExecuteChanged()
    {
        Console.WriteLine("RaiseCanExecuteChanged called. CurrentLetter: " + (CurrentLetter?.Symbol ?? "null"));
        (KnowLetterCommand as KnowLetterCommand)?.RaiseCanExecuteChanged();
        (DoNotKnowLetterCommand as DoNotKnowLetterCommand)?.RaiseCanExecuteChanged();
        (DownloadUnknownLettersCommand as DownloadUnknownLettersCommand)?.RaiseCanExecuteChanged();
    }
}

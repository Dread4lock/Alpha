using Alpha.Commands;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

public class AlphabetViewModel : BaseViewModel
{
    private LetterModel _currentLetter;
    private ObservableCollection<LetterModel> _toStudyLetters;
    private ObservableCollection<LetterModel> _learnedLetters;
    private ObservableCollection<LetterModel> _unknownLetters;

    // Свойства для отображения предупреждения
    private string _warningMessage;
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

    private Visibility _warningVisibility = Visibility.Collapsed;
    public Visibility WarningVisibility
    {
        get => _warningVisibility;
        set
        {
            _warningVisibility = value;
            OnPropertyChanged();
        }
    }

    public AlphabetViewModel()
    {
        ToStudyLetters = new ObservableCollection<LetterModel>();
        LearnedLetters = new ObservableCollection<LetterModel>();
        UnknownLetters = new ObservableCollection<LetterModel>();

        // Инициализация команд
        KnowLetterCommand = new KnowLetterCommand(KnowLetter);
        DoNotKnowLetterCommand = new DoNotKnowLetterCommand(DoNotKnowLetter);
        UploadAlphabetCommand = new RelayCommand(UploadAlphabet);
        CopyUnknownLettersCommand = new RelayCommand(CopyUnknownLetters);
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
    public ICommand CopyUnknownLettersCommand { get; }

    // Логика для обработки "знания" буквы
    private void KnowLetter()
    {
        if (CurrentLetter != null)
        {
            CurrentLetter.KnownCount++;

            if (CurrentLetter.KnownCount >= 2)
            {
                LearnedLetters.Add(CurrentLetter);
            }
            else
            {
                ToStudyLetters.Add(CurrentLetter); // Оставляем в списке для изучения
            }

            MoveToNextLetter();
        }
    }

    // Логика для обработки "незнания" буквы
    private void DoNotKnowLetter()
    {
        if (CurrentLetter != null)
        {
            CurrentLetter.KnownCount = 0; // Сбросить счетчик, если пользователь ответил неправильно
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
            var letters = File.ReadAllText(filePath).Where(char.IsLetter).ToList();

            if (letters.Count == 0)
            {
                WarningMessage = "The file does not contain any valid letters.";
                return;
            }

            // Считаем символы для каждого алфавита
            var alphabetCounts = new Dictionary<string, int>
        {
            { "Thai", 0 },
            { "Chinese", 0 },
            { "Cyrillic", 0 }
        };

            foreach (var letter in letters)
            {
                if (IsThaiLetter(letter.ToString()))
                {
                    alphabetCounts["Thai"]++;
                }
                else if (IsChineseLetter(letter.ToString()))
                {
                    alphabetCounts["Chinese"]++;
                }
                else if (IsCyrillicLetter(letter.ToString()))
                {
                    alphabetCounts["Cyrillic"]++;
                }
            }

            // Определение доминирующего алфавита
            var mostCommonAlphabet = alphabetCounts.OrderByDescending(kv => kv.Value).FirstOrDefault();

            // Если все символы принадлежат одному алфавиту, предупреждение не отображается
            if (mostCommonAlphabet.Value == letters.Count)
            {
                WarningMessage = "";  // Очищаем предупреждение
            }
            else
            {
                WarningMessage = $"Not all letters belong to {mostCommonAlphabet.Key} alphabet.";
            }

            // Продолжение загрузки букв только если предупреждение не активно
            ToStudyLetters.Clear();
            LearnedLetters.Clear();
            UnknownLetters.Clear();

            foreach (var letter in letters)
            {
                ToStudyLetters.Add(new LetterModel { Symbol = letter.ToString() });
            }

            MoveToNextLetter();
        }
    }


    // Проверка на принадлежность к тайскому алфавиту
    private bool IsThaiLetter(string value)
    {
        return Regex.IsMatch(value, @"^[\u0E00-\u0E7F]$");
    }

    // Проверка на принадлежность к китайскому алфавиту
    private bool IsChineseLetter(string value)
    {
        return Regex.IsMatch(value, @"^[\u4E00-\u9FFF\u3400-\u4DBF\u20000-\u2A6DF\u2A700-\u2B73F]$");
    }

    // Проверка на принадлежность к кириллическому алфавиту
    private bool IsCyrillicLetter(string value)
    {
        return Regex.IsMatch(value, @"^[\u0400-\u04FF]$");
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
            DisplayUnknownLetters(); // Показать список невыученных букв
        }
    }

    // Показ списка невыученных букв (можно реализовать в UI)
    private void DisplayUnknownLetters()
    {
        // Логика для отображения списка невыученных букв
        // Например, можно обновить UI или отобразить окно с кнопкой "Скопировать"
    }

    // Копирование невыученных букв в буфер обмена
    private void CopyUnknownLetters()
    {
        var unknownLettersText = string.Join(", ", UnknownLetters.Select(letter => letter.Symbol));
        Clipboard.SetText(unknownLettersText);
    }
}

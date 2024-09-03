using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

public class LetterModel : INotifyPropertyChanged
{
    private string _symbol;

    public required string Symbol
    {
        get => _symbol;
        set
        {
            // !null || Empty
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Symbol cannot be null or empty.");
            }

            // Alhabet type check
            if (IsLatinLetter(value))
            {
                _symbol = value.ToUpper();
            }
            else if (IsCyrillicLetter(value))
            {
                _symbol = value.ToUpper();
            }
            else if (IsThaiLetter(value))
            {
                _symbol = value;
            }
            else
            {
                throw new ArgumentException("Unsupported alphabet symbol.");
            }

            OnPropertyChanged();
        }
    }

    public bool IsKnown { get; set; } = false;

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #region Alphabets

    // Latin
    private bool IsLatinLetter(string value)
    {
        return Regex.IsMatch(value, @"^[A-Za-z]$");
    }

    // Cyrillic
    private bool IsCyrillicLetter(string value)
    {
        return Regex.IsMatch(value, @"^[А-Яа-яЁё]$");
    }

    // Thai
    private bool IsThaiLetter(string value)
    {
        return Regex.IsMatch(value, @"^[ก-๙]$");
    }

    #endregion
}

using System;
using System.Windows.Input;

public class KnowLetterCommand : ICommand
{
    private readonly AlphabetViewModel _viewModel;

    public KnowLetterCommand(AlphabetViewModel viewModel)
    {
        _viewModel = viewModel;
    }

    public event EventHandler CanExecuteChanged;

    public bool CanExecute(object parameter)
    {
        bool canExecute = _viewModel.CurrentLetter != null;
        Console.WriteLine("CanExecute KnowLetterCommand: " + canExecute);  // Логирование
        return canExecute;
    }

    public void Execute(object parameter)
    {
        var currentLetter = _viewModel.CurrentLetter;
        if (currentLetter != null)
        {
            Console.WriteLine("Executing KnowLetterCommand for: " + currentLetter.Symbol);

            currentLetter.KnownCount++;

            if (currentLetter.KnownCount >= 2)
            {
                _viewModel.LearnedLetters.Add(currentLetter);
            }
            else
            {
                _viewModel.ToStudyLetters.Add(currentLetter);
            }

            _viewModel.MoveToNextLetter();
        }

        RaiseCanExecuteChanged();  // Обновляем доступность команды после выполнения
    }

    public void RaiseCanExecuteChanged()
    {
        CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}

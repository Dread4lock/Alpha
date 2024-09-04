using System;
using System.Windows.Input;

namespace Alpha.Commands
{
    public class KnowLetterCommand : ICommand
    {
        private readonly Action _execute;

        public KnowLetterCommand(Action execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true; // Команда всегда может выполняться
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }
}

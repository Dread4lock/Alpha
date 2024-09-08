using System;
using System.Windows.Input;

namespace Alpha.Commands
{
    public class DownloadUnknownLettersCommand : ICommand
    {
        private readonly AlphabetViewModel _viewModel;

        public DownloadUnknownLettersCommand(AlphabetViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            // Команда доступна только если алфавит был загружен
            return _viewModel.IsAlphabetUploaded;
        }

        public void Execute(object parameter)
        {
            // Логика скачивания невыученных букв
            _viewModel.CopyUnknownLetters();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

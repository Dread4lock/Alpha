using Alpha.Services;
using System;
using System.Windows.Input;

namespace Alpha.Commands
{
    public class UploadAlphabetCommand : ICommand
    {
        private readonly AlphabetViewModel _viewModel;
        private readonly FileService _fileService;

        public UploadAlphabetCommand(AlphabetViewModel viewModel, FileService fileService)
        {
            _viewModel = viewModel;
            _fileService = fileService;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            var filePath = _fileService.OpenFile();
            if (string.IsNullOrEmpty(filePath)) return;

            var letters = _fileService.LoadLetters(filePath);
            _viewModel.UpdateLetters(letters);
        }
    }
}

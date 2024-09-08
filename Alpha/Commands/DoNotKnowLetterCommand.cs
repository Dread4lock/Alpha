﻿using System;
using System.Windows.Input;

namespace Alpha.Commands
{
    public class DoNotKnowLetterCommand : ICommand
    {
        private readonly AlphabetViewModel _viewModel;

        public DoNotKnowLetterCommand(AlphabetViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            bool canExecute = _viewModel.CurrentLetter != null;
            Console.WriteLine("CanExecute DoNotKnowLetterCommand: " + canExecute);  // Логирование
            return canExecute;
        }

        public void Execute(object parameter)
        {
            Console.WriteLine("Executing DoNotKnowLetterCommand for: " + _viewModel.CurrentLetter?.Symbol);
            _viewModel.DoNotKnowLetter();

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

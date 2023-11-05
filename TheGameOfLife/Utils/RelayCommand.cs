using System;
using System.Windows.Input;

namespace TheGameOfLife.Utils
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _action = null;
        private readonly Predicate<T> _canExecute = null;

        public RelayCommand(Action<T> action) : this(action, null) { }
        public RelayCommand(Action<T> action, Predicate<T> canExecute)
        {
            this._action = action;
            this._canExecute = canExecute ?? throw new ArgumentNullException(nameof(action));
        }

        public event EventHandler? CanExecuteChanged
        {
            add { if (_canExecute != null) CommandManager.RequerySuggested += value; }
            remove { if (_canExecute != null) CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute((T)parameter); ;
        }

        public void Execute(object? parameter)
        {
            _action((T)parameter);
        }
    }
}

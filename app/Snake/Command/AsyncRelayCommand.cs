using System.Windows.Input;

namespace Snake.Command
{
    public class AsyncRelayCommand(Func<object?, Task> executeAsync, Predicate<object?>? canExecute = null) : ICommand
    {
        private readonly Func<object?, Task> _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
        private readonly Predicate<object?>? _canExecute = canExecute;
        private bool _isExecuting;

        public bool CanExecute(object? parameter)
        {
            return !_isExecuting && (_canExecute == null || _canExecute(parameter));
        }

        public async void Execute(object? parameter)
        {
            _isExecuting = true;
            RaiseCanExecuteChanged();

            try
            {
                await _executeAsync(parameter);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public event EventHandler? CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

    }
}
namespace Viking.Common
{
    using System;
    using System.Diagnostics;
    using System.Windows.Input;

    public class DelegateCommand<T> : ICommand
    {
        private Predicate<T> canExecuteDelegate = null;

        private Action<T> commandDelegate = null;

        public DelegateCommand(Action<T> command, Predicate<T> canExecute = null)
        {
            commandDelegate = command;
            canExecuteDelegate = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (commandDelegate != null && CanExecute(parameter))
            {
                if (parameter == null || parameter is T)
                {
                    T commandParam = parameter == null ? default(T) : (T)parameter;
                    commandDelegate(commandParam);
                }
                else
                {
                    Debug.WriteLine(string.Format("Delegate CanExecute unable to execute: Expected parameter of type: {0}, received parameter of type {1}", typeof(T), parameter.GetType()));
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            bool canExecute = true;
            if (canExecuteDelegate != null)
            {
                if (parameter == null || parameter is T)
                {
                    T commandParam = parameter == null ? default(T) : (T)parameter;
                    canExecute = canExecuteDelegate(commandParam);
                }
                else
                {
                    Debug.WriteLine(string.Format("Delegate CanExecute unable to execute: Expected parameter of type: {0}, received parameter of type {1}", typeof(T), parameter.GetType()));
                }
            }

            return canExecute;
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }
    }
}

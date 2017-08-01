using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SamplePushNotification.Helper
{
    public class Command : ICommand
    {
        private Action<object> executeWithArgument;
        private Func<object, bool> canExecuteWithArgument;

        private Command()
        {

        }

        public Command(Action<object> execute) : this(execute, (parameter) => DefaultCanExecute(parameter))
        {

        }

        public Command(Action execute) : this((parameter) => execute())
        {
        }

        public Command(Action<object> execute, Func<object, bool> canExecute)
        {
            executeWithArgument = execute;
            canExecuteWithArgument = canExecute;
        }

        public Command(Action execute, Func<bool> canExecute) : this((parameter) => execute(), (parameter) => canExecute())
        {

        }

        private event EventHandler CanExecuteInternalChanged;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                this.CanExecuteInternalChanged += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
                this.CanExecuteInternalChanged -= value;
            }
        }

        public bool CanExecute(object parameter)
        {
            if (canExecuteWithArgument != null)
            {
                return canExecuteWithArgument.Invoke(parameter);
            }

            return true;
        }

        public void Execute(object parameter)
        {
            if (executeWithArgument != null)
            {
                executeWithArgument.Invoke(parameter);
            }
        }

        protected void OnCanExecuteChanged(EventArgs e)
        {
            CanExecuteInternalChanged?.Invoke(this, e);
        }

        public static bool DefaultCanExecute(object parameter)
        {
            return true;
        }
    }
}

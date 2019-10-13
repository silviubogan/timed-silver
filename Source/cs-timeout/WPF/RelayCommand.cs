using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace cs_timed_silver
{
    public class RelayCommand : ICommand
    {
        protected Action MyExecuteMethod;
        protected Func<bool> MyCanExecuteMethod;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action a)
        {
            MyExecuteMethod = a;
        }
        public RelayCommand(Action a, Func<bool> c)
        {
            MyExecuteMethod = a;
            MyCanExecuteMethod = c;
        }

        public bool CanExecute(object parameter)
        {
            if (MyCanExecuteMethod != null)
            {
                return MyCanExecuteMethod();
            }

            if (MyExecuteMethod != null)
            {
                return true;
            }

            return false;
        }

        public void Execute(object parameter)
        {
            if (MyExecuteMethod != null)
            {
                MyExecuteMethod();
            }
        }
    }
}

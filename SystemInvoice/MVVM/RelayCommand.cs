using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace SystemInvoice.MVVM
    {
    /// <summary>
    /// Реализация ICommand - обеспечивающая вызов пользовательских функций при выполнении операции на визуальном компоненте (например клике на кнопке), проверки возможности выполнения 
    /// </summary>
    public class RelayCommand : ICommand
        {
        /// <summary>
        /// Делегат вызываемый при операции
        /// </summary>
        readonly Action<object> executeDelegate;
        /// <summary>
        /// Делегат вызываемый при проверке возможности выполнения операции
        /// </summary>
        readonly Predicate<object> canExecuteDelegate;

        public RelayCommand( Action<object> execute, Predicate<object> canExecute = null )
            {
            if (execute == null)
                throw new ArgumentNullException( "execute" );
            executeDelegate = execute;
            canExecuteDelegate = canExecute;
            }
        /// <summary>
        /// Вызывается клиентом комманды (например кнопкой)
        /// </summary>
        /// <param name="parameter">Параметр передаваемый через байндинг</param>
        public bool CanExecute( object parameter )
            {
            return canExecuteDelegate == null ? true : canExecuteDelegate( parameter );
            }

        public event EventHandler CanExecuteChanged
            {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
            }

        /// <summary>
        /// Вызывается клиентом комманды (например кнопкой)
        /// </summary>
        /// <param name="parameter">Параметр передаваемый через байндинг</param>
        public void Execute( object parameter )
            {
            executeDelegate( parameter );
            }
        }
    }

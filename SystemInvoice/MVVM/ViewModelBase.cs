using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SystemInvoice.MVVM
    {
    /// <summary>
    /// Базовый класс модели представления используемы при байндинге к визуальным WPF - компонентам.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
        {
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Уведомляет об изменении свойства
        /// </summary>
        /// <param name="propertyName">Измененное свойство</param>
        protected virtual void RaisePropertyChanged( string propertyName )
            {
            if (PropertyChanged != null)
                {
                PropertyChanged( this, new PropertyChangedEventArgs( propertyName ) );
                }
            }
        }
    }

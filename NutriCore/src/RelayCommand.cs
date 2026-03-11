using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace NutriCore.src
{    
        /// <summary>
        /// Erweiterung der Klasse ICommand - um 
        /// Befehle angepasst im MVVM zu verwenden
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public class RelayCommand<T> : ICommand
        {
            private readonly Action<T> _execute;
            public RelayCommand(Action<T> execute) => _execute = execute;
            public bool CanExecute(object parameter) => true;
            public void Execute(object parameter) => _execute((T)parameter);
            public event EventHandler CanExecuteChanged;
        }    
}

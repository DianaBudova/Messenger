using System;
using System.Windows.Input;

namespace Messenger.ViewModels;

public class CommandBase : ICommand
{
    private Action<object> execute;
    private Func<object, bool> canExecute;
    public event EventHandler? CanExecuteChanged
    {
        add
        {
            CommandManager.RequerySuggested += value;
        }
        remove
        {
            CommandManager.RequerySuggested -= value;
        }
    }

    public CommandBase(Action<object> execute, Func<object, bool> canExecute = null)
    {
        this.execute = execute;
        this.canExecute = canExecute;
    }

    public bool CanExecute(object? parameter) =>
        true;

    public void Execute(object? parameter) =>
        this.execute?.Invoke(parameter);
}
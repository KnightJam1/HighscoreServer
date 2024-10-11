public class Executor
{
    private readonly CommandFactory _factory;

    public Executor()
    {
        _factory = new CommandFactory();
    }

    public void ExecuteCommand(string commandName)
    {
        ICommand command = _factory.GetCommand(commandName);
        command?.Execute();
    }
}
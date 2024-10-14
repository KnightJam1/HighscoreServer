public interface ICommand
{
    string Name { get; }
    public void Execute(CommandContext context, string[] args);
}
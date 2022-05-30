public interface ICommandRepo
{
    Task SaveChanges();
    Task<Command?> GetCommandById(int id);
    Task<IEnumerable<Command>> GetAllCommands(int id);
    Task CreateCommand(Command cmd);
    void DeleteCommand(Command cmd);
}
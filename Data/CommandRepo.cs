using Microsoft.EntityFrameworkCore;

public class CommandRepo : ICommandRepo
{
    private readonly AppDbContext _context;
    public CommandRepo(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Command>> GetAllCommandsAsync() => await _context.Commands.ToListAsync();

    public async Task<Command?> GetCommandByIdAsync(int id) => await _context.Commands.FirstOrDefaultAsync(c => c.Id == id);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    public async Task CreateCommandAsync(Command cmd)
    {
        if(cmd == null)
            throw new ArgumentNullException(nameof(cmd));
        await _context.AddAsync(cmd);
    }

    public void DeleteCommandAsync(Command cmd)
    {
        if(cmd == null)
            throw new ArgumentNullException(nameof(cmd));
        _context.Commands.Remove(cmd);
    }

}

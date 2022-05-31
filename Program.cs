using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MiniValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var sqlConnectionBuilder = new SqlConnectionStringBuilder();
sqlConnectionBuilder.ConnectionString = builder.Configuration.GetConnectionString("SQLDbConnection");
sqlConnectionBuilder.UserID = builder.Configuration["UserId"];
sqlConnectionBuilder.Password = builder.Configuration["Password"];

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(sqlConnectionBuilder.ConnectionString));
builder.Services.AddScoped<ICommandRepo, CommandRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("api/v1/commands", async (ICommandRepo repo, IMapper mapper) => {
    var commands = await repo.GetAllCommandsAsync();
    return Results.Ok(mapper.Map<IEnumerable<CommandReadDTO>>(commands));
}).Produces<IEnumerable<CommandReadDTO>>(StatusCodes.Status200OK);

app.MapGet("api/v1/commands/{id}", async (ICommandRepo repo, IMapper mapper, int id) => {
    var command = await repo.GetCommandByIdAsync(id);
    return command == null ? Results.NotFound() : Results.Ok(mapper.Map<CommandReadDTO>(command));
}).Produces<CommandReadDTO>(StatusCodes.Status200OK)
  .Produces(StatusCodes.Status404NotFound);

app.MapPost("api/v1/commands", async (ICommandRepo repo, IMapper mapper, CommandCreateDTO cmdDto) => {
    var model = mapper.Map<Command>(cmdDto);
    if(!MiniValidator.TryValidate(model, out var errors))
        return Results.ValidationProblem(errors);
    await repo.CreateCommandAsync(model);
    await repo.SaveChangesAsync();
    var cmdReadDto = mapper.Map<CommandReadDTO>(model);
    return Results.Created($"api/v1/commands/{model.Id}", cmdReadDto);
}).Produces<CommandReadDTO>(StatusCodes.Status201Created)
  .Produces(StatusCodes.Status400BadRequest)
  .ProducesValidationProblem();

app.MapPut("api/v1/commands/{id}", async (ICommandRepo repo, IMapper mapper, int id, CommandUpdateDTO cmdDto) => {
    var command = await repo.GetCommandByIdAsync(id);
    if(command == null)
        Results.NotFound();
    if(!MiniValidator.TryValidate(cmdDto, out var errors))
        return Results.ValidationProblem(errors);
    mapper.Map(cmdDto, command);
    await repo.SaveChangesAsync();
    return Results.NoContent();
}).Produces<CommandReadDTO>(StatusCodes.Status200OK)
  .Produces(StatusCodes.Status404NotFound)
  .ProducesValidationProblem();


app.MapDelete("api/v1/commands/{id}", async (ICommandRepo repo, IMapper mapper, int id) => {
    var command = await repo.GetCommandByIdAsync(id);
    if(command != null) 
    {
        repo.DeleteCommandAsync(command);
        await repo.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
}).Produces(StatusCodes.Status204NoContent)
  .Produces(StatusCodes.Status404NotFound);

app.Run();
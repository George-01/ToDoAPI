using Microsoft.EntityFrameworkCore;
using ToDoAPI.Data;
using ToDoAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add DI - AddServices
builder.Services.AddDbContext<ToDoDb>(opt => opt.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

// Configure the HTTP request pipeline. - UseMethod 
app.MapGet("/todoitems", async (ToDoDb db) =>
{
    var items = await db.ToDos.ToListAsync();
    return Results.Ok(items);
});

app.MapGet("/todoitems/{id}", async (int id, ToDoDb db) =>
    await db.ToDos.FindAsync(id));

app.MapPost("/todoitems", async (ToDoItem todo, ToDoDb db) =>
{
    db.ToDos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todoitems/{todo.Id}", todo);
});

app.MapPut("/todoitems/{id}", async (int id, ToDoItem inputTodo, ToDoDb db) =>
{
    /*if (id != inputTodo.Id)
    {
        return Results.BadRequest();
    }

    db.Entry(inputTodo).State = EntityState.Modified;
    await db.SaveChangesAsync();*/

    var todo = await db.ToDos.FindAsync(id);
    if (todo == null)
    {
        return Results.NotFound();
    }
    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, ToDoDb db) =>
{
    if(await db.ToDos.FindAsync(id) is ToDoItem todo)
    {
        db.ToDos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
});

app.Run();

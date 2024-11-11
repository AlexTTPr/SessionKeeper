using System.Threading.RateLimiting;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using SessionKeeper.Api;
using SessionKeeper.Application;
using SessionKeeper.Application.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDependencies(builder.Configuration["InitUsersFile"]!, builder.Configuration["UsersFile"]!, builder.Configuration["SessionsFile"]!);

builder.Services.AddRateLimiter(_ => _
	.AddFixedWindowLimiter(policyName: "fixed", options =>
	{
		options.PermitLimit = 1000;
		options.Window = TimeSpan.FromSeconds(10);
		options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
		options.QueueLimit = 100;
	}));

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRateLimiter();

//NOTE: MAPs
app.MapGet("/api/sessions/{id:guid}", (Guid Id, ISessionManager sessionManager) =>
{
	var res = sessionManager.GetSession(Id.ToString());

	if(res.IsSuccess)
		return Results.Ok(res.Value);

	var error = res.Errors.Last();
	return error switch
	{
		SessionDoesNotExistError => Results.NotFound(),
		SessionIsNotAliveError => Results.BadRequest(error.Message),
		_ => Results.NotFound(),
	};
})
	.WithOpenApi();

app.MapPost("/api/sessions", ([FromBody()] CreateSessionRequest createSessionRequest, ISessionManager sessionManager) =>
{
	var res = sessionManager.AddSession(createSessionRequest.Login, createSessionRequest.PasswordHash);

	if(res.IsSuccess)
		return Results.Ok(res.Value);

	var error = res.Errors.Last();
	return error switch
	{
		UserDoesNotExistError => Results.NotFound(),
		PasswordsDoNotMathError => Results.BadRequest(error.Message),
		_ => Results.NotFound(),
	};
})
	.WithOpenApi();

app.MapDelete("/api/sessions/{id:guid}", (Guid Id, ISessionManager sessionManager) =>
{
	var res = sessionManager.DeleteSession(Id.ToString());

	if(res.IsSuccess)
		return Results.NoContent();

	return Results.BadRequest();
})
	.WithOpenApi();

app.Run();


internal record CreateSessionRequest(string Login, string PasswordHash);
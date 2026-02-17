using Aplication.Interfaces;
using Aplication.Interfaces.Commands;
using Aplication.Interfaces.Querys;
using Aplication.Interfaces.Services;
using Aplication.UseCase;
using Domain.Entities;
using Infraestructure;
using Infraestructure.Command;
using Infraestructure.Persistence;
using Infraestructure.Querys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Infrastructure: DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<AppDbContext>());

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://127.0.0.1:5500")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


// Core services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Project Proposal
builder.Services.AddScoped<IProjectProposalService, ProjectProposalService>();
builder.Services.AddScoped<IProjectProposalQuery, ProjectProposalQuery>();
builder.Services.AddScoped<IProjectProposalCommand, ProjectProposalCommand>();

// Approval Rules & Steps
builder.Services.AddScoped<IApprovalRuleQuery, ApprovalRuleQuery>();
builder.Services.AddScoped<IProjectApprovalStepCommand, ProjectApprovalStepCommand>();
builder.Services.AddScoped<IProjectApprovalStepQuery, ProjectApprovalStepQuery>();

// Users
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserQuery, UserQuery>();

// Area
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IAreaQuery, AreaQuery>();

// Project Type
builder.Services.AddScoped<IProjectTypeService, ProjectTypeService>();
builder.Services.AddScoped<IProjectTypeQuery, ProjectTypeQuery>();

// Approval Status
builder.Services.AddScoped<IApprovalStatusService, ApprovalStatusService>();
builder.Services.AddScoped<IApprovalStatusQuery, ApprovalStatusQuery>();

// Approver Role
builder.Services.AddScoped<IApproverRoleService, ApproverRoleService>();
builder.Services.AddScoped<IApproverRoleQuery, ApproverRoleQuery>();

// Suppress automatic model validation (handled manually)
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthorization();
app.MapControllers();
app.Run();

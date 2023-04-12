using ProposalSender.ASP.Implementations;
using ProposalSender.Contracts.Implementations;
using ProposalSender.Contracts.Interfaces;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        //Add services
        builder.Services.AddSingleton<ISendTelegramMessages, SendTelegramMessages>();
        builder.Services.AddSingleton<ITMHttpClient, TMHttpClient>();


        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.ConfigureApi();

        app.Run();

    }
}
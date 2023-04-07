using ProposalSender.Contracts.Interfaces;
using ProposalSender.Contracts.Models;
using System.Collections.ObjectModel;

namespace ProposalSender.ASP.Implementations
{
    public static class Api
    {
        public static void ConfigureApi(this WebApplication app)
        {
            app.MapPost("/phones/connect", Connect);
            app.MapPost("/phones/sendcode/", SendCode);
            app.MapPost("/phones/sendmessage", SendMessage);
            app.MapPost("/phones/disconnect", Disconnect);
        }

        private static async Task<IResult>Connect(ISendTelegramMessages send, UserSender user)
        {
            try
            {
                return Results.Ok(await send.Connect(user, $"+7{user.PhoneNumber}"));
            }
            catch
            {
                return Results.Problem(send.InfoMessage);
            }
        }

        private static async Task<IResult> SendCode(ISendTelegramMessages send, string verificationValue)
        {
            try
            {
                return Results.Ok(await send.Connect(null, verificationValue));
            }
            catch
            {
                return Results.Problem(send.InfoMessage);
            }
        }

        private static async Task<IResult>SendMessage(ISendTelegramMessages send, long phone, string message)
        {
            try
            {
                await send.SendMessage(phone, message);
                return Results.Ok();
            }
            catch 
            { 
                return Results.Problem(send.InfoMessage); 
            }
        }

        private static IResult Disconnect(ISendTelegramMessages send)
        {
            try
            {
                send.Disconnect();
                return Results.Ok();
            }
            catch 
            { 
                return Results.Problem(send.InfoMessage); 
            }
        }  
    }
}

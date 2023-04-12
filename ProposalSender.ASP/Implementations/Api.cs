using ProposalSender.Contracts.Interfaces;
using ProposalSender.Contracts.Models;

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
            var result = await send.Connect(user, $"+7{user.PhoneNumber}");

            try
            {
                return Results.Ok(result);
            }
            catch
            {
                return Results.Problem(result.TaskInfoMessage);
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
                return Results.Problem();
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
                return Results.Problem();
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
                return Results.Problem();
            }
        }  
    }
}

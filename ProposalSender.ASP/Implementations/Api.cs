using ProposalSender.Contracts.Interfaces;
using ProposalSender.Contracts.Models;

namespace ProposalSender.ASP.Implementations
{
    public static class Api
    {
        public static void ConfigureApi(this WebApplication app)
        {
            app.MapPost("/phones/connect", ConnectAsync);
            app.MapPost("/phones/sendcode/", SendCodeAsync);
            app.MapPost("/phones/sendmessage", SendMessageAsync);
            app.MapPost("/phones/disconnect", DisconnectAsync);
        }

        private static async Task<IResult>ConnectAsync(ISendTelegramMessages send, UserSender user)
        {
            var result = await send.ConnectAsync(user, $"+7{user.PhoneNumber}");
            try
            {               
                return Results.Ok(result.ToTuple());
            }
            catch
            {
                return Results.Problem(result.infoMessage);
            }
        }

        private static async Task<IResult> SendCodeAsync(ISendTelegramMessages send, string verificationValue)
        {
            var result = await send.ConnectAsync(null, verificationValue);
            try
            {
                return Results.Ok(result.ToTuple());
            }
            catch
            {
                return Results.Problem(result.infoMessage);
            }
        }

        private static async Task<IResult> SendMessageAsync(ISendTelegramMessages send, long phone, string message)
        {
            var result = await send.SendMessageAsync(phone, message);
            try
            {
                return Results.Ok(result.ToTuple());
            }
            catch 
            {
                return Results.Problem(result.errorMessage);
            }
        }

        private static async Task<IResult> DisconnectAsync(ISendTelegramMessages send)
        {
            var result = await send.DisconnectAsync();
            try
            { 
                return Results.Ok(result.ToTuple());
            }
            catch 
            {
                return Results.Problem(result.status);
            }
        }  
    }
}

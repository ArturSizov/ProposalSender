using ProposalSender.Contracts.Interfaces;
using ProposalSender.Contracts.Models;

namespace ProposalSender.ASP.Implementations
{
    public static class Api
    {
        public static void ConfigureApi(this WebApplication app)
        {
            app.MapGet("/phones", Connect);
            //app.MapGet("/phones", SendMessage);
            //app.MapGet("/phones", Disconnect);
        }

        private static async Task<IResult>Connect(ISendTelegramMessages send, UserSender user, string verificationValue)
        {
            try
            {
                return Results.Ok(await send.Connect(user, verificationValue));
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        private static async Task<IResult>SendMessage(ISendTelegramMessages send)
        {
            try
            {
                return Results.Ok(await send.Connect(null, null));
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        private static async Task<IResult>Disconnect(ISendTelegramMessages send)
        {
            try
            {
                return Results.Ok(await send.Connect(null, null));
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

       

        
    }
}

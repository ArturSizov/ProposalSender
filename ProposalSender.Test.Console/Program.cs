using ProposalSender.Contracts.Implementations;
using ProposalSender.Contracts.Interfaces;
using ProposalSender.Test.Console;
using TL;
internal class Program
{
    static ISendTelegramMessages send = new SendTelegramMessages();
    static async Task Main(string[] args)
    {
        new SendMessages(send);
    }
 }
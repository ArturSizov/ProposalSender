using ProposalSender.Contracts.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ProposalSender.Test.Console
{
    public class SendMessages
    {
        private readonly ISendTelegramMessages telegramMessages;

        public SendMessages(ISendTelegramMessages telegramMessages)
        {
            this.telegramMessages = telegramMessages;
            telegramMessages.Phones.Add(89393921255);
        }
    }
}

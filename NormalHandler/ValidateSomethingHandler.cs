using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;
using NServiceBus;

namespace NormalHandler
{
    public class ValidateSomethingHandler : IHandleMessages<ValidateSomething>
    {
        public IBus Bus { get; set; }

        public void Handle(ValidateSomething message)
        {
            Console.WriteLine("NORMAL HANDLER CALLED!!");

            Bus.Send(new ValidateSomethingReply() { BusinessID = message.BusinessID });

            //Bus.Reply(new ValidateSomethingReply() { BusinessID=message.BusinessID});
        }
    }
}

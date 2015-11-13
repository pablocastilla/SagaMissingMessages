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
        public void Handle(ValidateSomething message)
        {
            throw new NotImplementedException();
        }
    }
}

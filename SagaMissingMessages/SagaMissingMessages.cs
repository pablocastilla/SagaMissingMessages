using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messages;
using NServiceBus;
using NServiceBus.Saga;

namespace SagaMissingMessages
{
    public class SagaMissingMessages : Saga<SagaMissingMessagesSagaData>, IAmStartedByMessages<InitSagaCommand>, IHandleMessages<ValidateSomethingReply>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<SagaMissingMessagesSagaData> mapper)
        {

     
        }

        public void Handle(InitSagaCommand message)
        {
            throw new NotImplementedException();
        }

        public void Handle(ValidateSomethingReply message)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Messages;
using NServiceBus;
using NServiceBus.Saga;

namespace SagaMissingMessages
{
    public class SagaMissingMessages : Saga<SagaMissingMessagesSagaData>, IAmStartedByMessages<InitSagaCommand>, IHandleMessages<ValidateSomethingReply>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<SagaMissingMessagesSagaData> mapper)
        {
            mapper.ConfigureMapping<ValidateSomethingReply>(d => d.BusinessID).ToSaga(s=> s.BusinessID);
     
        }

        public void Handle(InitSagaCommand message)
        {
            Data.BusinessID = Guid.NewGuid();

            using (var tran = new TransactionScope(TransactionScopeOption.Suppress))
            {
                Bus.Send(new ValidateSomething() { BusinessID=Data.BusinessID});
                tran.Complete();
            }

            System.Threading.Thread.Sleep(2000);
                     

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var random = new Random();
                var result = new string(
                    Enumerable.Repeat(chars, 1800)
                              .Select(s => s[random.Next(s.Length)])
                              .ToArray());

            Data.RawData1 = result.ToCharArray();
            Data.RawData2 = result.ToCharArray();
            Data.RawData3 = result.ToCharArray();
            Data.RawData4 = result.ToCharArray();
            
          
        
            Console.WriteLine("SAGA CREATED!!");
        }

        public void Handle(ValidateSomethingReply message)
        {
            Console.WriteLine("SAGA FOUND!!");
            this.MarkAsComplete();    
        }
    }

    public class SagaNotFoundHandler : IHandleSagaNotFound
    {
        IBus bus;

        public SagaNotFoundHandler(IBus bus)
        {
            this.bus = bus;
        }

        public void Handle(object message)
        {
            Console.WriteLine("SAGA NOT FOUND!!");
        }
    }
}

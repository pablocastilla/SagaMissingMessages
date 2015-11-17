using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus.Saga;

namespace ConversationPessimisticLocker
{
    public class FakeSagaForPessimisticLocker : Saga<PessimisticLockerData>
    {
        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PessimisticLockerData> mapper)
        {


        }
    }
}

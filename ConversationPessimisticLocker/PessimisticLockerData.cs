using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus.Saga;

namespace ConversationPessimisticLocker
{
    public class PessimisticLockerData : ContainSagaData
    {
        public virtual DateTime EntryDateTime { get; set; }

    }
}

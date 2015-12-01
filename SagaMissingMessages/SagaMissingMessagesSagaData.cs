using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Saga;

namespace SagaMissingMessages
{
    public class SagaMissingMessagesSagaData : ContainSagaData
    {
        [Unique]
        public virtual Guid BusinessID { get; set; }

       
    }


   
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using NServiceBus.Persistence.NHibernate;
using NServiceBus.Pipeline;
using NServiceBus.Pipeline.Contexts;
using Oracle.ManagedDataAccess.Client;

namespace ConversationPessimisticLocker
{
    public class ConversationPessimisticLockerBehavior : IBehavior<IncomingContext>
    {
        string LOCKERHEADERNAME = "NServiceBus.ConversationId";

        public NHibernateStorageContext session { get; set; }

        public void Invoke(IncomingContext context, Action next)
        {
            bool headerExists = false;

            Guid? conversationId;
            

            var connectionString = ConfigurationManager.ConnectionStrings["NServiceBus/Persistence"].ConnectionString;

            if (context.PhysicalMessage != null && context.PhysicalMessage.Headers != null && context.PhysicalMessage.Headers.Any(x => x.Key == LOCKERHEADERNAME))
            {
                conversationId = Guid.Parse(context.PhysicalMessage.Headers
                 .FirstOrDefault(x => x.Key == LOCKERHEADERNAME).Value);
            }
            else
            {
                conversationId = Guid.NewGuid();
            }

            //look if the locker header exists in the database
            var blockingEntity = session.Session.Get<PessimisticLockerData>(conversationId.Value,NHibernate.LockMode.Upgrade);

            //if not create outside the transaction
            if (blockingEntity == null)
            {
                using (var tran = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    var temporalSession =  session.Session.SessionFactory.OpenSession();

                    temporalSession.Save(new PessimisticLockerData() { Id = conversationId.Value, EntryDateTime = DateTime.Now.Date });

                    temporalSession.Flush();

                    tran.Complete();

                    temporalSession.Dispose();
                }

                //block it again
                blockingEntity = session.Session.Get<PessimisticLockerData>(conversationId.Value, NHibernate.LockMode.Upgrade);
            }

               
            next();

                       
        }
    }

    public class ConversationPessimisticLockerStep : RegisterStep
    {
        public ConversationPessimisticLockerStep()
            : base("ConversationPessimisticLockerStep", typeof(ConversationPessimisticLockerBehavior), "locks by conversation ID")
        {
            // Optional: Specify where it needs to be invoked in the pipeline, for example InsertBefore or InsertAfter
            InsertBefore("SetCurrentMessageBeingHandled");
        }
    }
}

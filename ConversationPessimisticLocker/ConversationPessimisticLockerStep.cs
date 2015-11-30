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
using NServiceBus.Saga;
using Oracle.ManagedDataAccess.Client;

namespace ConversationPessimisticLocker
{
    public class ConversationPessimisticLockerBehavior : IBehavior<IncomingContext>
    {
        string LOCKERHEADERNAME = "NServiceBus.ConversationId";

        public NHibernateStorageContext session { get; set; }

        public void Invoke(IncomingContext context, Action next)
        {          

            Guid? conversationId;

            //if it is not a saga don't block and skip this step
            bool isSaga = context.MessageHandler.Instance is Saga;


            if (!isSaga)
            {
                next();


            }
            else 
            {
                if (context.PhysicalMessage != null && context.PhysicalMessage.Headers != null && context.PhysicalMessage.Headers.Any(x => x.Key == LOCKERHEADERNAME))
                {
                    conversationId = Guid.Parse(context.PhysicalMessage.Headers
                     .FirstOrDefault(x => x.Key == LOCKERHEADERNAME).Value);
                }
                else
                {
                    conversationId = Guid.NewGuid();

                    context.PhysicalMessage.Headers.Add(LOCKERHEADERNAME, conversationId.ToString());
                }

                //look if the locker header exists in the database
                var blockingEntity = session.Session.Get<PessimisticLockerData>(conversationId.Value, NHibernate.LockMode.Upgrade);

                //if not create the blocking row outside the transaction
                if (blockingEntity == null)
                {
                    using (var tran = new TransactionScope(TransactionScopeOption.Suppress))
                    {
                        var temporalSession = session.Session.SessionFactory.OpenSession();

                        try
                        {
                            temporalSession.Save(new PessimisticLockerData() { Id = conversationId.Value, EntryDateTime = DateTime.Now.Date });

                            temporalSession.Flush();

                            tran.Complete();
                        }
                        finally
                        {
                            temporalSession.Dispose();
                        }

                    }

                    //block it again
                    blockingEntity = session.Session.Get<PessimisticLockerData>(conversationId.Value, NHibernate.LockMode.Upgrade);
                }


                next();
            }

                       
        }
    }

    /// <summary>
    /// this step is inserted before any saga finding logic is executed.
    /// </summary>
    public class ConversationPessimisticLockerStep : RegisterStep
    {
        public ConversationPessimisticLockerStep()
            : base("ConversationPessimisticLockerStep", typeof(ConversationPessimisticLockerBehavior), "locks by conversation ID")
        {           

            InsertBefore("SetCurrentMessageBeingHandled");
        }
    }
}

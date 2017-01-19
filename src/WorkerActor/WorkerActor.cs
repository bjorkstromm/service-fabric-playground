using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using WorkerActor.Interfaces;

namespace WorkerActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Volatile)]
    internal class WorkerActor : Actor, IWorkerActor
    {
        private readonly Guid _statusId = Guid.NewGuid();

        /// <summary>
        /// Initializes a new instance of WorkerActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public WorkerActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
        }

        public Task<Guid> StartAsync(string data)
        {
            RegisterTimer(DoWork, data, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(-1));
            ActorEventSource.Current.ActorMessage(this, "Timer registered for data {0} on id {1}", data, Id);

            return Task.FromResult(_statusId);
        }

        private async Task DoWork(object data)
        {
            ActorEventSource.Current.ActorMessage(this, "DoWork started for data {0} on id {1}", data, Id);
            var str = data.ToString().ToCharArray();
            var last = str.Length - 1;
            var reversed = new char[str.Length];

            var statusActor = ActorProxy.Create<IWorkerStatusActor>(new ActorId(_statusId));

            for (var i = 0; i < str.Length; i++)
            {
                reversed[i] = str[last - i];

                var progress = (i + 1/(double) str.Length);
                
                await statusActor.SetStatusAsync(new WorkerStatus {Progress = progress});
                ActorEventSource.Current.ActorMessage(this, "Working for data {0} on id {1}. Progress {2}", data, Id, progress);

                await Task.Delay(TimeSpan.FromSeconds(1));
            }

            await statusActor.SetStatusAsync(new WorkerStatus
            {
                IsCompleted = true,
                Progress = 1.0,
                Result = new string(reversed)
            });
            ActorEventSource.Current.ActorMessage(this, "Working is done for data {0} on id {1}", data, Id);
        }
    }
}

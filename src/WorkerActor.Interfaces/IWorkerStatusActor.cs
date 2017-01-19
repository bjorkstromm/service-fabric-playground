using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;

namespace WorkerActor.Interfaces
{
    public interface IWorkerStatusActor : IActor
    {
        Task SetStatusAsync(WorkerStatus status);
        Task<WorkerStatus> GetStatusAsync();
    }
}

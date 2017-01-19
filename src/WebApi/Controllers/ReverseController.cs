using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using WebApi.Model;
using WorkerActor.Interfaces;

namespace WebApi.Controllers
{
    [ServiceRequestActionFilter]
    [RoutePrefix("api/v1/reverse")]
    public class ReverseController : ApiController
    {
        [HttpPost]
        [Route("")]
        [ResponseType(typeof(Guid))]
        public async Task<IHttpActionResult> Register([FromBody]ReverseModel model)
        {
            var worker = ActorProxy.Create<IWorkerActor>(ActorId.CreateRandom());
            return Ok(await worker.StartAsync(model.Data));
        }

        [HttpGet]
        [Route("{jobId}/status")]
        [ResponseType(typeof(WorkerStatus))]
        public async Task<IHttpActionResult> GetStatus([FromUri]Guid jobId)
        {
            var worker = ActorProxy.Create<IWorkerStatusActor>(new ActorId(jobId));

            return Ok(await worker.GetStatusAsync());
        }
    }
}

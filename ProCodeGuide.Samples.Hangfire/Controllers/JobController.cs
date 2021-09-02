using Hangfire;
using Microsoft.AspNetCore.Mvc;
using ProCodeGuide.Samples.Hangfire.Services;
using System;

namespace ProCodeGuide.Samples.Hangfire.Controllers {
    [Route("[controller]")]
    [ApiController]
    public class JobController : ControllerBase {
        private readonly IBackgroundJobClient _jobClient = null;
        private readonly IRecurringJobManager _recJobClient = null;
        public JobController(IBackgroundJobClient jobClient, IRecurringJobManager recJobClient) {
            _jobClient = jobClient;
            _recJobClient = recJobClient;
        }

        [HttpGet("ExecucaoImediata")]
        public string ExecucaoImediata() {
            var jobId = _jobClient.Enqueue<IJobTestService>(jbt => jbt.Executar("Fire-and-Forget Job", DateTime.Now.ToLongTimeString()));
            return $"Execução imediata do job { jobId }";
        }

        [HttpGet("ExecucaoAgendadaEmSegundos")]
        public string ExecucaoAgendadaEmSegundos(double intervalo) {
            _jobClient.Schedule<IJobTestService>(jbt => jbt.Executar("Delayed Job", DateTime.Now.ToLongTimeString()), TimeSpan.FromSeconds(intervalo));
            return $"Ação será executada em { intervalo } segundos";
        }

        [HttpGet("ExecucaoRecorrenteEmSegundos")]
        public string ExecucaoRecorrenteEmSegundos(int intervalo) {
            _recJobClient.AddOrUpdate<IJobTestService>("ExecutarRecorrenteEmSegundos", jbt => jbt.Executar("Recurring Job", DateTime.Now.ToLongTimeString()), $"*/{ intervalo } * * * * *");
            return $"Ação será executada a cada { intervalo } segundos";
        }

        [HttpGet("ExecucaoSubsequente")]
        public string ExecucaoSubsequente() {
            var jobId = _jobClient.Schedule<IJobTestService>(jbt => jbt.Executar("Continuation Job", DateTime.Now.ToLongTimeString()), TimeSpan.FromSeconds(45));
            var jobChildId = _jobClient.ContinueJobWith(jobId, () => Console.WriteLine("Continuation Job - Filho - " + DateTime.Now.ToLongTimeString()));

            return $"Ação do job filho { jobChildId } executado ao término da ação do job pai { jobId }";
        }
    }
}
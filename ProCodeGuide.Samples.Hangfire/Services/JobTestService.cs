using System;

namespace ProCodeGuide.Samples.Hangfire.Services {
    public class JobTestService : IJobTestService
    {
        public void Executar(string tipoDeJob, string tempoInicializacao)
        {
            Console.WriteLine(tipoDeJob + " - " + tempoInicializacao + " - Job Executado - " + DateTime.Now.ToLongTimeString());
        }
    }
}

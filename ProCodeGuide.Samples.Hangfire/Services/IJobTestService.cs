namespace ProCodeGuide.Samples.Hangfire.Services {
    public interface IJobTestService
    {
        void Executar(string tipoDeJob, string tempoInicializacao);
    }
}

namespace ContaBancaria
{
    public class TransferRequest
    {
        public int NumeroOrigem { get; set; }
        public int NumeroDestino { get; set; }
        public decimal Valor { get; set; }
    }
}
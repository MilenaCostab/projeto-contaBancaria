using System;

namespace ContaBancaria
{
    public class ContaCorrente : Conta
    {
        public decimal TaxaSaque { get; private set; }
        public decimal Limite { get; private set; }

        protected ContaCorrente() { } 

        
        public ContaCorrente(string titular, string email, string cpf, string senha, int numero, decimal saldo, int agencia, decimal taxaSaque, decimal limite, DateOnly? dataNascimento) 
            : base(titular, email, cpf, senha, numero, saldo, agencia, dataNascimento)
        {
            this.TaxaSaque = taxaSaque;
            this.Limite = limite;
        }

        public override bool Sacar(decimal valor)
        {
            decimal valorComTaxa = valor + TaxaSaque;
            if (valor <= 0)
            {
                Console.WriteLine("Valor inválido. O saque deve ser maior que zero.");
                return false;
            } else if (valorComTaxa > (Saldo + Limite)) {
                Console.WriteLine("Saldo e limite insuficientes para realizar o saque, considerando a taxa.");
                return false;
            } else {
                Saldo -= valorComTaxa;
                Console.WriteLine($"Saque de R$ {valor:F2} realizado com sucesso, com taxa de R$ {TaxaSaque:F2}. Novo saldo é R$ {Saldo:F2}.");
                return true;
            }
        }

        public override void Visualizar()
        {
            base.Visualizar();
            Console.WriteLine($"Taxa de Saque: R$ {TaxaSaque:F2}");
            Console.WriteLine($"Limite: R$ {Limite:F2}");
        }

        
    }
}

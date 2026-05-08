using System;

namespace ContaBancaria
{
    public class ContaMista : ContaPoupanca
    {
        public decimal TaxaSaque { get; private set; }
        public decimal Limite { get; private set; }

        protected ContaMista() { }

        public ContaMista(string titular, string email, string cpf, string senha, int numero, decimal saldo, int agencia, decimal taxaRendimento, DateOnly? aniversario, decimal taxaSaque, decimal limite, DateOnly? dataNascimento) 
            : base(titular, email, cpf, senha, numero, saldo, agencia, taxaRendimento, aniversario, dataNascimento)
        {
            this.TaxaSaque = taxaSaque;
            this.Limite = limite;
        }

        public override bool Sacar(decimal valor)
        {
            decimal valorComTaxa = valor + TaxaSaque;
            if (valor <= 0) return false;
            if (valorComTaxa > (Saldo + Limite)) return false;
            
            Saldo -= valorComTaxa;
            return true;
        }
    }
}
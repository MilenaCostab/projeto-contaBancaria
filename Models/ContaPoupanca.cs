using System;

namespace ContaBancaria
{
    public class ContaPoupanca : Conta
    {
        public decimal TaxaRendimento { get; private set; } 
        public DateOnly? Aniversario { get; private set; }
        public decimal CaixinhaSaldo { get; protected set; } 

        protected ContaPoupanca() { } 

        public ContaPoupanca(string titular, string email, string cpf, string senha, int numero, decimal saldo, int agencia, decimal taxaRendimento, DateOnly? aniversario, DateOnly? dataNascimento) 
            : base(titular, email, cpf, senha, numero, saldo, agencia, dataNascimento)
        {
            this.TaxaRendimento = taxaRendimento;
            this.Aniversario = aniversario;
            this.CaixinhaSaldo = 0; 
        }

        public void Render()
        {
            decimal rendimento = CaixinhaSaldo * TaxaRendimento;
            CaixinhaSaldo += rendimento;
            Console.WriteLine($"Rendimento de R$ {rendimento:F2} aplicado na Caixinha. Novo saldo da Caixinha é R$ {CaixinhaSaldo:F2}.");
        }
       
        public override bool DepositarNaCaixinha(decimal valor)
        {
            if (valor <= 0)
            {
                Console.WriteLine("Valor inválido. O depósito na Caixinha deve ser maior que zero.");
                return false;
            }
            if (valor > Saldo)
            {
                Console.WriteLine("Saldo insuficiente para depositar na Caixinha.");
                return false;
            }
            Saldo -= valor;
            CaixinhaSaldo += valor;
            Console.WriteLine($"R$ {valor:F2} depositados na Caixinha. Saldo principal: R$ {Saldo:F2}, Saldo Caixinha: R$ {CaixinhaSaldo:F2}.");
            return true;
        }

        public override bool RetirarDaCaixinha(decimal valor)
        {
            if (valor <= 0)
            {
                Console.WriteLine("Valor inválido. A retirada da Caixinha deve ser maior que zero.");
                return false;
            }
            if (valor > CaixinhaSaldo)
            {
                Console.WriteLine("Saldo insuficiente na Caixinha.");
                return false;
            }
            CaixinhaSaldo -= valor;
            Saldo += valor;
            Console.WriteLine($"R$ {valor:F2} retirados da Caixinha. Saldo principal: R$ {Saldo:F2}, Saldo Caixinha: R$ {CaixinhaSaldo:F2}.");
            return true;
        }

        public override void Visualizar()
        {
            base.Visualizar();
            Console.WriteLine($"Taxa de Rendimento: {TaxaRendimento:P2}");
        }
    }
}

using System;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ContaBancaria
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "tipo")]
    [JsonDerivedType(typeof(ContaCorrente), typeDiscriminator: "corrente")]
    [JsonDerivedType(typeof(ContaPoupanca), typeDiscriminator: "poupanca")]
    [JsonDerivedType(typeof(ContaMista), typeDiscriminator: "mista")]
    public abstract class Conta
    { 
        [Required(ErrorMessage = "O titular é obrigatório.")]
        [StringLength(100, ErrorMessage = "O titular não pode ter mais de 100 caracteres.")]
        public string Titular { get; set; }
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")]
        [StringLength(100, ErrorMessage = "O e-mail não pode ter mais de 100 caracteres.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "O CPF deve ter 11 caracteres numéricos.")]
        public string Cpf { get; set; }
        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 50 caracteres.")]
        public string Senha { get; set; }
        public DateOnly? DataNascimento { get; set; }
        public int Numero { get; set; }
        public decimal Saldo { get; protected set; }
        public int Agencia { get; set; }

        protected Conta() 
        { 
            this.Titular = string.Empty;
            this.Email = string.Empty;
            this.Cpf = string.Empty;
            this.Senha = string.Empty;
        }

        public Conta(string titular, string email, string cpf, string senha, int numero, decimal saldo, int agencia, DateOnly? dataNascimento)
        {
            this.Titular = titular;
            this.Email = email;
            this.Cpf = cpf;
            this.Senha = senha;
            this.Numero = numero;
            this.Saldo = saldo;
            this.Agencia = agencia;
            this.DataNascimento = dataNascimento;
        }

        public virtual void Depositar(decimal valor)
        {
            if (valor <=0)
            {
                Console.WriteLine("Valor inválido. O depósito deve ser maior que zero.");
                return;
            } else {
                Saldo += valor;
                Console.WriteLine($"Depósito de R$ {valor:F2} realizado com sucesso. Novo saldo é R$ {Saldo:F2}.");
            }
            
        }
        public virtual bool Sacar(decimal valor)
        {
            if (valor <= 0)
            {
                Console.WriteLine("Valor inválido. O saque deve ser maior que zero.");
                return false;
            } else if (valor > Saldo) {
                Console.WriteLine("Saldo insuficiente para realizar o saque.");
                return false;
            } else {
                Saldo -= valor;
                Console.WriteLine($"Saque de R$ {valor:F2} realizado com sucesso. Novo saldo é R$ {Saldo:F2}.");
                return true;
            }
        }

        public virtual void Visualizar()
        {
            Console.WriteLine($"Titular: {Titular}");
            Console.WriteLine($"Número:  {Numero}");
            Console.WriteLine($"Saldo:   R$ {Saldo:F2}");
            Console.WriteLine($"Agência: {Agencia}");
        }

        public virtual bool Transferir(decimal valor, Conta contaDestino)
        {
            if (this.Sacar(valor))
            {
                contaDestino.Depositar(valor);
                Console.WriteLine($"Transferência de R$ {valor:F2} realizada para {contaDestino.Titular}.");
                return true;
            }
            return false;
        }

        public virtual bool DepositarNaCaixinha(decimal valor)
        {
            Console.WriteLine("Esta conta não possui funcionalidade de Caixinha.");
            return false;
        }
        public virtual bool RetirarDaCaixinha(decimal valor)
        {
            Console.WriteLine("Esta conta não possui funcionalidade de Caixinha.");
            return false;
        }
    }
}

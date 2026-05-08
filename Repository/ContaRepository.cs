using System;
using System.Collections.Generic;

namespace ContaBancaria
{
    public interface ContaRepository
    {
        Conta? ProcurarPorNumero(int numero); 
        Conta? ProcurarPorCpf(string cpf);  
        List<Conta> ListarTodas();           
        void Cadastrar(Conta conta);
        void Atualizar(Conta conta);
        void Deletar(int numero);

        bool Sacar(int numero, decimal valor);
        void Depositar(int numero, decimal valor); 
        bool Transferir(int numeroOrigem, int numeroDestino, decimal valor);
        bool DepositarNaCaixinha(int numero, decimal valor); 
        bool RetirarDaCaixinha(int numero, decimal valor); 
        bool RenderRendimentos(int numero);
    }
}
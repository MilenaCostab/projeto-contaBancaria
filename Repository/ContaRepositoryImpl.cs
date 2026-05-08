using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ContaBancaria
{
    public class ContaRepositoryImpl : ContaRepository
    {
        private readonly BancoDbContext _context;

        public ContaRepositoryImpl(BancoDbContext context)
        {
            _context = context;
            _context.Database.EnsureCreated(); 
        }

        public Conta? ProcurarPorNumero(int numero)
        {
            return _context.Contas.AsNoTracking().FirstOrDefault(c => c.Numero == numero);
        }

        public Conta? ProcurarPorCpf(string cpf)
        {
            return _context.Contas.AsNoTracking().FirstOrDefault(c => c.Cpf == cpf);
        }

        public List<Conta> ListarTodas()
        {
            return _context.Contas.ToList();
        }

        public void Cadastrar(Conta conta)
        {
            _context.Contas.Add(conta);
            _context.SaveChanges();
        }

        public void Atualizar(Conta conta)
        {
            var existente = _context.Contas.FirstOrDefault(c => c.Cpf == conta.Cpf);

            if (existente != null && existente.GetType() != conta.GetType())
            {
                _context.Contas.Remove(existente);
                _context.SaveChanges();
                _context.Contas.Add(conta);
            }
            else
            {
                _context.Contas.Update(conta);
            }
            _context.SaveChanges();
        }

        public void Deletar(int numero)
        {
            var conta = _context.Contas.FirstOrDefault(c => c.Numero == numero);
            if (conta != null)
            {
                _context.Contas.Remove(conta);
                _context.SaveChanges();
            }
        }

        public bool Sacar(int numero, decimal valor)
        {
            var conta = _context.Contas.FirstOrDefault(c => c.Numero == numero);
            if (conta != null && conta.Sacar(valor))
            {
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public void Depositar(int numero, decimal valor)
        {
            var conta = _context.Contas.FirstOrDefault(c => c.Numero == numero);
            if (conta != null)
            {
                conta.Depositar(valor);
                _context.SaveChanges();
            }
        }

        public bool Transferir(int numeroOrigem, int numeroDestino, decimal valor)
        {
            var origem = _context.Contas.FirstOrDefault(c => c.Numero == numeroOrigem);
            var destino = _context.Contas.FirstOrDefault(c => c.Numero == numeroDestino);

            if (origem != null && destino != null && origem.Sacar(valor))
            {
                destino.Depositar(valor);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool RenderRendimentos(int numero)
        {
            var conta = _context.Contas.FirstOrDefault(c => c.Numero == numero);
            if (conta is ContaPoupanca poupanca)
            {
                poupanca.Render();
                _context.SaveChanges(); 
                return true;
            }
            return false;
        }

        public bool DepositarNaCaixinha(int numero, decimal valor)
        {
            var conta = _context.Contas.FirstOrDefault(c => c.Numero == numero);
            if (conta is ContaPoupanca poupanca) 
            {
                if (poupanca.DepositarNaCaixinha(valor))
                {
                    _context.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool RetirarDaCaixinha(int numero, decimal valor)
        {
            var conta = _context.Contas.FirstOrDefault(c => c.Numero == numero);
            if (conta is ContaPoupanca poupanca)
            {
                if (poupanca.RetirarDaCaixinha(valor))
                {
                    _context.SaveChanges();
                    return true;
                }
            }
            return false;
        }
    }
}
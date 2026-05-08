using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Http; 
namespace ContaBancaria
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContaController : ControllerBase
    {
        private readonly ContaRepository _repository;

        public ContaController(ContaRepository repository)
        {
            _repository = repository;
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] Conta conta)
        {
            try
            {
                if (conta == null)
                {
                    return BadRequest(new { message = "Dados da conta inválidos." });
                }

                if (conta.DataNascimento == null)
                {
                    return BadRequest(new { message = "A data de nascimento é obrigatória." });
                }

                Conta? existente = _repository.ProcurarPorCpf(conta.Cpf);

                if (existente != null)
                {
                    return Conflict(new { message = $"Já existe uma conta cadastrada com o CPF {conta.Cpf}!" }); 
                }

                if (conta.Numero == 0)
                {
                    conta.Numero = new Random().Next(100000, 999999);
                    conta.Agencia = 1;
                }

                _repository.Cadastrar(conta);
                return CreatedAtAction(nameof(ProcurarPorNumero), new { numero = conta.Numero }, conta);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao cadastrar conta: {ex.Message}");
                var mensagemCompleta = ex.InnerException != null ? $"{ex.Message} -> {ex.InnerException.Message}" : ex.Message;
                return StatusCode(500, $"Erro interno: {mensagemCompleta}");
            }
        }

        [HttpGet("{numero}")]
        public IActionResult ProcurarPorNumero(int numero)
        {
            Conta? conta = _repository.ProcurarPorNumero(numero);

            if (conta == null)
            {
                return NotFound($"Conta {numero} não encontrada."); 
            }

            return Ok(conta); 
        }

        [HttpGet]
        public IActionResult ListarTodas()
        {
            var contas = _repository.ListarTodas();
            if (contas.Count == 0)
            {
                return NoContent();
            }

            return Ok(contas);
        }

        [HttpPost("depositar/{numero}")]
        public IActionResult Depositar(int numero, [FromBody] decimal valor)
        {
            Conta? conta = _repository.ProcurarPorNumero(numero);

            if (conta == null)
            {
                return NotFound($"Conta {numero} não encontrada.");
            }

            if (valor <= 0)
            {
                return BadRequest("Valor de depósito deve ser maior que zero.");
            }

            _repository.Depositar(numero, valor);
            Conta? contaAtualizada = _repository.ProcurarPorNumero(numero);
            return Ok($"Depósito de R$ {valor:F2} realizado! Saldo atual: R$ {contaAtualizada?.Saldo ?? 0:F2}");
        }

        [HttpPost("sacar/{numero}")]
        public IActionResult Sacar(int numero, [FromBody] decimal valor)
        {
            Conta? conta = _repository.ProcurarPorNumero(numero);

            if (conta == null)
            {
                return NotFound($"Conta {numero} não encontrada.");
            }

            if (valor <= 0)
            {
                return BadRequest("Valor de saque deve ser maior que zero.");
            }

            bool sucesso = _repository.Sacar(numero, valor);

            if (sucesso)
            {
                Conta? contaAtualizada = _repository.ProcurarPorNumero(numero);
                return Ok($"Saque de R$ {valor:F2} realizado! Saldo atual: R$ {contaAtualizada?.Saldo ?? 0:F2}");
            }
            else
            {
                return BadRequest($"Saldo insuficiente na conta {numero}. Saldo atual: R$ {conta.Saldo:F2}");
            }
        }

        [HttpPost("transferir")]
        public IActionResult Transferir([FromBody] TransferRequest request)
        {
            Conta? origem = _repository.ProcurarPorNumero(request.NumeroOrigem);
            Conta? destino = _repository.ProcurarPorNumero(request.NumeroDestino);

            if (origem == null)
            {
                return NotFound($"Conta de origem ({request.NumeroOrigem}) não encontrada.");
            }

            if (destino == null)
            {
                return NotFound($"Conta de destino ({request.NumeroDestino}) não encontrada.");
            }

            if (request.Valor <= 0)
            {
                return BadRequest("Valor da transferência deve ser maior que zero.");
            }

            bool sucesso = _repository.Transferir(request.NumeroOrigem, request.NumeroDestino, request.Valor);
            if (!sucesso) return BadRequest("Falha na transferência. Verifique o saldo.");
            
            return Ok($"Transferência de R$ {request.Valor:F2} realizada de {request.NumeroOrigem} ➜ {request.NumeroDestino}!");
        }

        [HttpDelete("{numero}")]
        public IActionResult Deletar(int numero)
        {
            Conta? conta = _repository.ProcurarPorNumero(numero);

            if (conta == null)
            {
                return NotFound($"Conta {numero} não encontrada.");
            }

            _repository.Deletar(numero);
            return Ok($"Conta {numero} removida com sucesso!");
        }
    
         [HttpPut]
         public IActionResult Atualizar([FromBody] Conta conta)
        {
            if (conta == null)
            {
                return BadRequest("Dados da conta inválidos.");
            }

            Conta? contaExistente = _repository.ProcurarPorCpf(conta.Cpf);

            if (contaExistente == null)
            {
                return NotFound($"Conta com CPF {conta.Cpf} não encontrada.");
            }

            _repository.Atualizar(conta);
            return Ok(new { message = $"Conta {conta.Numero} atualizada com sucesso!" });
        }

        [HttpPost("poupanca/render/{numero}")]
        public IActionResult RenderRendimento(int numero)
        {
            bool sucesso = _repository.RenderRendimentos(numero);
            if (!sucesso) 
                return BadRequest("Não foi possível aplicar rendimento. Verifique se a conta existe e é do tipo Poupança.");
            
            Conta? conta = _repository.ProcurarPorNumero(numero);
            return Ok(new { 
                message = "Seu dinheiro trabalhou por você! O rendimento da sua Caixinha foi aplicado.",
                novoSaldoCaixinha = (conta is ContaPoupanca cp ? cp.CaixinhaSaldo : 0)
            });
        }

        [HttpPost("caixinha/depositar/{numero}")]
        public IActionResult DepositarNaCaixinha(int numero, [FromBody] decimal valor)
        {
            Conta? conta = _repository.ProcurarPorNumero(numero);

            if (conta == null)
            {
                return NotFound($"Conta {numero} não encontrada.");
            }

            if (valor <= 0)
            {
                return BadRequest("Valor de depósito na Caixinha deve ser maior que zero.");
            }

            bool sucesso = _repository.DepositarNaCaixinha(numero, valor);
            if (!sucesso)
                return BadRequest("Não foi possível depositar na Caixinha. Verifique o saldo ou se a conta é do tipo Poupança/Mista.");

            Conta? contaAtualizada = _repository.ProcurarPorNumero(numero);
            return Ok($"R$ {valor:F2} depositados na Caixinha! Saldo principal: R$ {contaAtualizada?.Saldo ?? 0:F2}, Saldo Caixinha: R$ {(contaAtualizada is ContaPoupanca cp ? cp.CaixinhaSaldo : 0):F2}");
        }

        [HttpPost("caixinha/retirar/{numero}")]
        public IActionResult RetirarDaCaixinha(int numero, [FromBody] decimal valor)
        {
            Conta? conta = _repository.ProcurarPorNumero(numero);

            if (conta == null)
            {
                return NotFound($"Conta {numero} não encontrada.");
            }

            if (valor <= 0)
            {
                return BadRequest("Valor de retirada da Caixinha deve ser maior que zero.");
            }

            bool sucesso = _repository.RetirarDaCaixinha(numero, valor);
            if (!sucesso)
                return BadRequest("Não foi possível retirar da Caixinha. Verifique o saldo guardado.");

            Conta? contaAtualizada = _repository.ProcurarPorNumero(numero);
            return Ok($"R$ {valor:F2} retirados da Caixinha! Saldo principal: R$ {contaAtualizada?.Saldo ?? 0:F2}, Saldo Caixinha: R$ {(contaAtualizada is ContaPoupanca cp ? cp.CaixinhaSaldo : 0):F2}");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request.Cpf == "admin" && request.Senha == "admin123")
            {
                return Ok(new { message = "Acesso ADM concedido!", titular = "Administrador", isAdmin = true });
            }

            var conta = _repository.ProcurarPorCpf(request.Cpf);

            if (conta == null || conta.Senha != request.Senha)
            {
                return Unauthorized("CPF ou senha inválidos.");
            }

            return Ok(new { message = "Login realizado com sucesso!", titular = conta.Titular, isAdmin = false });
        }

    }
}

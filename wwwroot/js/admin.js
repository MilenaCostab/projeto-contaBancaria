const apiBase = '/api/conta';

async function validarSessaoAdmin() {
    const isAdmin = sessionStorage.getItem('isAdmin');
    const instanceIdLocal = sessionStorage.getItem('serverInstance');

    const resp = await fetch('/api/instance');
    const instanceIdServidor = await resp.json();

    if (isAdmin !== 'true' || instanceIdLocal !== instanceIdServidor) {
        sessionStorage.clear();
        window.location.href = 'login.html';
        return false;
    }
    return true;
}
validarSessaoAdmin();

async function carregarContas() {
    const response = await fetch(apiBase);
    const contas = await response.json();
    const tbody = document.getElementById('tabelaContas');
    tbody.innerHTML = '';

    contas.forEach(c => {
        const tr = document.createElement('tr');
        tr.innerHTML = `
            <td>${c.titular}</td>
            <td>${c.cpf}</td>
            <td>${c.numero === 0 ? '<span style="color:red">Pendente</span>' : c.numero}</td>
            <td>${c.agencia === 0 ? '<span style="color:red">Pendente</span>' : c.agencia}</td>
            <td><button onclick='abrirEdicao(${JSON.stringify(c)})'>Configurar</button></td>
        `;
        tbody.appendChild(tr);
    });
}

function toggleLimiteField() {
    const tipo = document.getElementById('editTipo').value;
    const limiteInput = document.getElementById('editLimite');
    if (tipo === 'corrente') {
        limiteInput.style.display = 'block';
        limiteInput.setAttribute('required', 'true');
    } else {
        limiteInput.style.display = 'none';
        limiteInput.removeAttribute('required');
        limiteInput.value = 0;
    }
}

function abrirEdicao(conta) {
    document.getElementById('editTitular').innerText = conta.titular;
    document.getElementById('editCpf').value = conta.cpf;
    document.getElementById('editNumero').value = conta.numero;
    document.getElementById('editAgencia').value = conta.agencia;
    document.getElementById('editTipo').value = conta.tipo;
    document.getElementById('editLimite').value = conta.limite || 0;
    document.getElementById('editCaixinhaSaldo').value = conta.caixinhaSaldo || 0;

    toggleLimiteField();
    document.getElementById('modalEditar').style.display = 'block';
}

async function salvarAlteracoes() {
    const cpf = document.getElementById('editCpf').value;
    const tipoSelecionado = document.getElementById('editTipo').value;


    const respGet = await fetch(`${apiBase}`);
    const contas = await respGet.json();
    const original = contas.find(c => c.cpf === cpf);

    const dadosAtualizados = {
        ...original,
        tipo: tipoSelecionado,
        numero: parseInt(document.getElementById('editNumero').value),
        agencia: parseInt(document.getElementById('editAgencia').value),
    };
    dadosAtualizados.caixinhaSaldo = parseFloat(document.getElementById('editCaixinhaSaldo').value || 0);

    if (tipoSelecionado === 'mista') {
        dadosAtualizados.limite = parseFloat(document.getElementById('editLimite').value || 0);
        dadosAtualizados.taxaSaque = 0.50;
        dadosAtualizados.taxaRendimento = 0.005;
        dadosAtualizados.aniversario = original.aniversario || new Date().toISOString().split('T')[0];
    } else if (tipoSelecionado === 'corrente') {
        dadosAtualizados.limite = parseFloat(document.getElementById('editLimite').value || 0);
        dadosAtualizados.taxaSaque = original.taxaSaque || 0.50;
        delete dadosAtualizados.aniversario;
        delete dadosAtualizados.taxaRendimento;
    } else {
        dadosAtualizados.taxaRendimento = original.taxaRendimento || 0.005;
        dadosAtualizados.aniversario = original.aniversario || new Date().toISOString().split('T')[0];
        delete dadosAtualizados.limite;
        delete dadosAtualizados.taxaSaque;
    }

    const response = await fetch(apiBase, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(dadosAtualizados)
    });

    if (response.ok) {
        const resultado = await response.json();
        alert(resultado.message || 'Conta configurada com sucesso!');
        document.getElementById('modalEditar').style.display = 'none';
        carregarContas();
    } else {
        alert('Erro ao atualizar conta');
    }
}

document.getElementById('editTipo').addEventListener('change', toggleLimiteField);

carregarContas();
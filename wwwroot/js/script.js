const apiBase = '/api/conta';

document.getElementById('tipo').addEventListener('change', (e) => {
    const camposExtras = document.getElementById('camposExtras');
    if (e.target.value === 'corrente') {
        camposExtras.innerHTML = '<input type="number" id="limite" placeholder="Limite" step="0.01">';
    } else {
        camposExtras.innerHTML = '<input type="text" id="aniversario" placeholder="Aniversário (AAAA-MM-DD)">';
    }
});

document.getElementById('formCadastro').addEventListener('submit', async (e) => {
    e.preventDefault();
    const tipo = document.getElementById('tipo').value;
    const dados = {
        tipo: tipo,
        titular: document.getElementById('titular').value,
        numero: parseInt(document.getElementById('numero').value),
        agencia: parseInt(document.getElementById('agencia').value),
        saldo: parseFloat(document.getElementById('saldo').value)
    };

    if (tipo === 'corrente') {
        dados.limite = parseFloat(document.getElementById('limite').value || 0);
        dados.taxaSaque = 0.50;
    } else {
        dados.aniversario = document.getElementById('aniversario').value;
        dados.taxaRendimento = 0.005;
    }

    const response = await fetch(apiBase, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(dados)
    });

    if (response.ok) {
        alert('Conta criada com sucesso!');
        carregarContas();
    } else {
        const msg = await response.text();
        alert('Erro: ' + msg);
    }
});

async function carregarContas() {
    const response = await fetch(apiBase);
    const contas = await response.json();
    const container = document.getElementById('contasContainer');
    container.innerHTML = '';

    if (contas.length === 0) {
        container.innerHTML = '<p>Nenhuma conta cadastrada.</p>';
        return;
    }

    contas.forEach(c => {
        const card = document.createElement('div');
        card.className = 'conta-card';
        card.innerHTML = `
            <strong>${c.titular}</strong> (Nº ${c.numero})<br>
            Saldo: R$ ${c.saldo.toFixed(2)} | Tipo: ${c.tipo}
        `;
        container.appendChild(card);
    });
}

async function realizarOperacao(endpoint, numero, valor) {
    const response = await fetch(`${apiBase}/${endpoint}/${numero}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(parseFloat(valor))
    });
    const msg = await response.text();
    alert(msg);
    carregarContas();
}

function realizarSaque() {
    realizarOperacao('sacar', document.getElementById('opNumero').value, document.getElementById('opValor').value);
}

function realizarDeposito() {
    realizarOperacao('depositar', document.getElementById('opNumero').value, document.getElementById('opValor').value);
}

async function aplicarRendimento() {
    const num = document.getElementById('opNumero').value;
    const response = await fetch(`${apiBase}/poupanca/render/${num}`, { method: 'POST' });
    alert(await response.text());
    carregarContas();
}

carregarContas();
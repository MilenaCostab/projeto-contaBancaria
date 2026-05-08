const apiBase = '/api/conta';

const currentTheme = localStorage.getItem('theme') || 'light';
document.body.classList.toggle('dark-mode', currentTheme === 'dark');

document.getElementById('themeToggle')?.addEventListener('click', () => {
    const isDark = document.body.classList.toggle('dark-mode');
    localStorage.setItem('theme', isDark ? 'dark' : 'light');
});

async function validarSessao() {
    const usuario = sessionStorage.getItem('usuarioLogado');
    const instanceIdLocal = sessionStorage.getItem('serverInstance');

    const resp = await fetch('/api/instance');
    const instanceIdServidor = await resp.json();

    if (!usuario || instanceIdLocal !== instanceIdServidor) {
        sessionStorage.clear();
        window.location.href = 'login.html';
        return false;
    }
    return true;
}
validarSessao();

function logout() {
    sessionStorage.clear();
    window.location.href = 'login.html';
}

async function carregarContas() {
    const pageTitleElement = document.getElementById('pageHeading');
    if (pageTitleElement) pageTitleElement.textContent = 'Dados do Usuário';

    let contas = [];
    try {
        const response = await fetch(apiBase);
        if (response.ok) {
            contas = response.status === 204 ? [] : await response.json();
        }
    } catch (err) {
        console.error("Erro ao carregar dados da API:", err);
    }

    const container = document.getElementById('contasContainer');
    if (!container) return;
    container.innerHTML = '';

    const cpfLogado = sessionStorage.getItem('cpfLogado');
    let possuiRendimento = false;

    const contasUsuario = Array.isArray(contas) ? contas.filter(c => {
        const valorConta = (c.cpf || c.Cpf || "").toString().trim().toLowerCase();
        const valorLogado = (cpfLogado || "").toString().trim().toLowerCase();
        return valorLogado !== "" && valorConta === valorLogado;
    }) : [];

    if (contasUsuario.length === 0) {
        container.innerHTML = '<p>Nenhuma conta ativa encontrada para o seu usuário.</p>';
        return;
    }

    contasUsuario.forEach(c => {
        const card = document.createElement('div');
        card.className = 'conta-card';
        
        const displayNumero = (c.numero === 0 || c.agencia === 0) 
            ? '<span style="color: #e67e22; font-style: italic;">Usuário em processamento</span>' 
            : `Nº ${c.numero}`;

        let tipoExibicao = 'Conta Corrente';
        if (c.tipo === 'poupanca') {
            tipoExibicao = 'Conta Poupança';
            possuiRendimento = true;
        } else if (c.tipo === 'mista') {
            tipoExibicao = 'Conta Corrente | Conta Poupança'; 
            possuiRendimento = true;
        }

        let caixinhaInfo = '';
        if (c.caixinhaSaldo && c.caixinhaSaldo > 0) {
            caixinhaInfo = `<br>Saldo Caixinha: R$ ${c.caixinhaSaldo.toFixed(2)}`;
        }

        let limiteInfo = '';
        if (c.limite && c.limite > 0) {
            limiteInfo = ` | Limite: R$ ${c.limite.toFixed(2)}`;
        }

        card.innerHTML = `
            <strong>${c.titular}</strong> (${displayNumero})<br>
            Saldo: R$ ${c.saldo.toFixed(2)}${limiteInfo}<br>
            <strong>${tipoExibicao}</strong>${caixinhaInfo}
        `;
        container.appendChild(card);
    });

    document.getElementById('btnRender').style.display = possuiRendimento ? 'block' : 'none';
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
    const num = document.getElementById('caixinhaNumero').value;
    const response = await fetch(`${apiBase}/poupanca/render/${num}`, { method: 'POST' });
    
    if (response.ok) {
        const dados = await response.json();
        alert(dados.message);
        carregarContas();
    } else {
        alert(await response.text());
    }
}

async function realizarTransferencia() {
    const numOrigem = document.getElementById('transfOrigem').value;
    const numDestino = document.getElementById('transfDestino').value;
    const valor = document.getElementById('transfValor').value;

    const response = await fetch(`${apiBase}/transferir`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            numeroOrigem: parseInt(numOrigem),
            numeroDestino: parseInt(numDestino),
            valor: parseFloat(valor)
        })
    });

    const msg = await response.text();
    alert(msg);
    carregarContas();
}

async function depositarNaCaixinha() {
    const num = document.getElementById('caixinhaNumero').value;
    const valor = document.getElementById('caixinhaValor').value;

    const response = await fetch(`${apiBase}/caixinha/depositar/${num}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(parseFloat(valor))
    });

    if (response.ok) {
        alert(await response.text());
        carregarContas();
    } else {
        alert(await response.text());
    }
}

async function retirarDaCaixinha() {
    const num = document.getElementById('caixinhaNumero').value;
    const valor = document.getElementById('caixinhaValor').value;

    const response = await fetch(`${apiBase}/caixinha/retirar/${num}`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(parseFloat(valor))
    });

    if (response.ok) {
        alert(await response.text());
        carregarContas();
    } else {
        alert(await response.text());
    }
}

carregarContas();
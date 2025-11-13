# API Transferencia

Aplicação para realizar Movimentações em Contas Correntes.

## Rodando a aplicação

- A aplicação API Conta Corrente deve estar rodando (https://github.com/brunorsq/api-conta-corrente)
- Acessar a pasta raiz pelo terminal;
- Executar "docker compose up";
- Acessar localhost:5001/swagger/index.html;
- Utilizar o endpoint de Transferência.

## Funcionalidades implementadas

- Realização de transferência entre contas corrente.

## Funcionalidades pendentes / melhorias

- **Testes**: Atualmente não foram implementados os testes necessários.
- **Arquitetura**: Implementar CleanArch no projeto.
- **Envio para fila Kafka**: O processamento atual não envia mensagens da fila Kafka.

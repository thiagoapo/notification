# ms-notification

Foi utlizado o RabbitMQ como solução de mensageria
 https://www.rabbitmq.com/getstarted.html

Swagger como documentação da API do micro-serviço
https://swagger.io/ 

Json Web Token para autenticação (segurança)
https://jwt.io/introduction/

Testes Unitários com Xunit
https://xunit.net/

Entity Framework In MemoryBase
https://docs.microsoft.com/pt-br/ef/core/get-started/?tabs=netcore-cli

Para gerar token na API, acessar o end-point "access/token" com o login é: admin (não tem senha)
Pelo Swagger há 2 endpoints para visualizar as mensagens:
  - Mensagens recebidas (mensagens que uma instância recebeu de outras instâncias)
  - Mensagens enviadas (mensagens que a própria instância enviou)

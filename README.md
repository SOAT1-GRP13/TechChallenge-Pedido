<h1>  Tech Challenge - SOAT1 - Grupo 13 - Microserviço de pedido </h1>

![GitHub](https://img.shields.io/github/license/dropbox/dropbox-sdk-java)

# Resumo do projeto

Este projeto é desenvolvido em C# com .NET 6, seguindo os princípios da arquitetura hexagonal. Seu objetivo principal é permitir que os usuários possam adicionar um item ao carrinho, atualizar esse item, remover o item do carrinho, listar todos os itens do carrinho e confirmar o pedido.

Para garantir a segurança das informações de acesso ao banco de dados PostgreSQL, o projeto faz uso do Secret Manager. Isso permite que as credenciais do banco de dados sejam armazenadas de forma segura e acessadas apenas por autorizações apropriadas. Essa abordagem fortalece a segurança dos dados sensíveis.

Ao longo do desenvolvimento, estaremos fazendo entregas incrementais e criando releases no GIT para acompanhar o progresso do projeto. Esperamos que este trabalho demonstre nosso conhecimento teórico e prático adquirido durante a pós-graduação, além de servir como um exemplo de aplicação das melhores práticas de arquitetura em projetos de software.

Sinta-se à vontade para entrar em contato conosco se tiver alguma dúvida ou sugestão. Agradecemos pelo interesse em nosso projeto!


> :construction: Projeto em construção :construction:

License: [MIT](License.txt)

# Sonar Cloud
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SOAT1-GRP13_TechChallenge-Pedido&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=SOAT1-GRP13_TechChallenge-Pedido) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=SOAT1-GRP13_TechChallenge-Pedido&metric=coverage)](https://sonarcloud.io/summary/new_code?id=SOAT1-GRP13_TechChallenge-Pedido) [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=SOAT1-GRP13_TechChallenge-Pedido&metric=bugs)](https://sonarcloud.io/summary/new_code?id=SOAT1-GRP13_TechChallenge-Pedido) [![Vulnerabilities](https://sonarcloud.io/api/project_badges/measure?project=SOAT1-GRP13_TechChallenge-Pedido&metric=vulnerabilities)](https://sonarcloud.io/summary/new_code?id=SOAT1-GRP13_TechChallenge-Pedido) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=SOAT1-GRP13_TechChallenge-Pedido&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=SOAT1-GRP13_TechChallenge-Pedido) [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=SOAT1-GRP13_TechChallenge-Pedido&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=SOAT1-GRP13_TechChallenge-Pedido) [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=SOAT1-GRP13_TechChallenge-Pedido&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=SOAT1-GRP13_TechChallenge-Pedido) [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=SOAT1-GRP13_TechChallenge-Pedido&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=SOAT1-GRP13_TechChallenge-Pedido)

Para maiores detalhes através do link: https://sonarcloud.io/summary/overall?id=SOAT1-GRP13_TechChallenge-Pedido.

# Bando de Dados

Decidimos manter o PostgreSQL como o banco de dados para este microserviço. A escolha foi definida na experiência que o time possui com ele, o que facilita o trabalho. Além disso, fizemos uma limpeza no banco de dados, removendo tabelas desnecessárias e mantendo apenas as que são essenciais para este projeto. Assim, o banco está mais enxuto e alinhado com as nossas necessidades atuais.

# ⌨️ Testando a API

**Importante**
Você pode baixar o projeto e executá-lo em seu ambiente local com o Visual Studio. Embora o projeto esteja hospedado em nossa infraestrutura na AWS, também o apresentamos aos professores em um vídeo demonstrando seu funcionamento.

Isso permite que você experimente a funcionalidade da API em seu próprio ambiente e explore seu comportamento. Se tiver alguma dúvida ou precisar de assistência, sinta-se à vontade para entrar em contato conosco.

Você pode testar esta API de duas maneiras: usando o Postman ou o Swagger, que está configurado no projeto.

Acessando o Swagger:

Para acessar o Swagger do projeto localmente, utilize o seguinte link:
- http://localhost:PortalLocal/swagger/index.html

O Swagger já contém exemplos de chamadas com dados reais.

Lembre-se de adicionar o token obtido na resposta da chamada no menu "Authorize".

Autenticação:
As chamadas requerem autenticação. Para obter um token Bearer, você pode através do seguinte projeto: https://github.com/SOAT1-GRP13/TechChallenge-SOAT1-GRP13-Auth.

# 🛠️ Abrir e rodar o projeto utilizando o docker

Para o correto funcionamento precisa do docker instalado.

Com o docker instalado, acesse a pasta raiz do projeto e execute o comando abaixo: 

```shell
docker-compose up
```

# 📒 Documentação da API

No projeto foi instalado o REDOC e pode ser acessado através do link abaixo:

- http://localhost:PortalLocal/api-docs/index.html

# ✔️ Tecnologias utilizadas

- ``.Net 6``
- ``Postgres``
- ``Secrets Manager``


# Autores

| [<img src="https://avatars.githubusercontent.com/u/28829303?s=400&v=4" width=115><br><sub>Christian Melo</sub>](https://github.com/christiandmelo) |  [<img src="https://avatars.githubusercontent.com/u/89987201?v=4" width=115><br><sub>Luiz Soh</sub>](https://github.com/luiz-soh) |  [<img src="https://avatars.githubusercontent.com/u/21027037?v=4" width=115><br><sub>Wagner Neves</sub>](https://github.com/nevesw) |  [<img src="https://avatars.githubusercontent.com/u/34692183?v=4" width=115><br><sub>Mateus Bernardi Marcato</sub>](https://github.com/xXMateus97Xx) |
| :---: | :---: | :---: | :---: |

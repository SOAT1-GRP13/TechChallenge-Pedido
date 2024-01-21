<h1> # Tech Challenge - SOAT1 - Grupo 13 - Microservi�o de pedido </h1>

![GitHub](https://img.shields.io/github/license/dropbox/dropbox-sdk-java)

# Resumo do projeto

Este projeto � desenvolvido em C# com .NET 6, seguindo os princ�pios da arquitetura hexagonal. Seu objetivo principal � permitir que os usu�rios possam adicionar um item ao carrinho, atualizar esse item, remover o item do carrinho, listar todos os itens do carrinho e confirmar o pedido.

Para garantir a seguran�a das informa��es de acesso ao banco de dados PostgreSQL, o projeto faz uso do Secret Manager. Isso permite que as credenciais do banco de dados sejam armazenadas de forma segura e acessadas apenas por autoriza��es apropriadas. Essa abordagem fortalece a seguran�a dos dados sens�veis.

Ao longo do desenvolvimento, estaremos fazendo entregas incrementais e criando releases no GIT para acompanhar o progresso do projeto. Esperamos que este trabalho demonstre nosso conhecimento te�rico e pr�tico adquirido durante a p�s-gradua��o, al�m de servir como um exemplo de aplica��o das melhores pr�ticas de arquitetura em projetos de software.

Sinta-se � vontade para entrar em contato conosco se tiver alguma d�vida ou sugest�o. Agradecemos pelo interesse em nosso projeto!


> :construction: Projeto em constru��o :construction:

License: [MIT](License.txt)

# Bando de Dados

Decidimos manter o PostgreSQL como o banco de dados para este microservi�o. A escolha foi definida na experi�ncia que o time possui com ele, o que facilita o trabalho. Al�m disso, fizemos uma limpeza no banco de dados, removendo tabelas desnecess�rias e mantendo apenas as que s�o essenciais para este projeto. Assim, o banco est� mais enxuto e alinhado com as nossas necessidades atuais.

# ?? Testando a API

**Importante**
Voc� pode baixar o projeto e execut�-lo em seu ambiente local com o Visual Studio. Embora o projeto esteja hospedado em nossa infraestrutura na AWS, tamb�m o apresentamos aos professores em um v�deo demonstrando seu funcionamento.

Isso permite que voc� experimente a funcionalidade da API em seu pr�prio ambiente e explore seu comportamento. Se tiver alguma d�vida ou precisar de assist�ncia, sinta-se � vontade para entrar em contato conosco.

Voc� pode testar esta API de duas maneiras: usando o Postman ou o Swagger, que est� configurado no projeto.

Acessando o Swagger:

Para acessar o Swagger do projeto localmente, utilize o seguinte link:
- http://localhost:PortalLocal/swagger/index.html

O Swagger j� cont�m exemplos de chamadas com dados reais.

Lembre-se de adicionar o token obtido na resposta da chamada no menu "Authorize".

Autentica��o:
As chamadas requerem autentica��o. Para obter um token Bearer, voc� pode atrav�s do seguinte projeto: https://github.com/SOAT1-GRP13/TechChallenge-SOAT1-GRP13-Auth.

Se voc� preferir testar nosso servi�o de autentica��o localmente, siga as orienta��es no seguinte reposit�rio:
- https://github.com/christiandmelo/TechChallenge-SOAT1-GRP13-Auth

# ??? Abrir e rodar o projeto utilizando o docker

Para o correto funcionamento precisa do docker instalado.

Com o docker instalado, acesse a pasta raiz do projeto e execute o comando abaixo: 

```shell
docker-compose up
```

# ?? Documenta��o da API

No projeto foi instalado o REDOC e pode ser acessado atrav�s do link abaixo:

- http://localhost:PortalLocal/api-docs/index.html

# ?? Tecnologias utilizadas

- ``.Net 6``
- ``Postgres``
- ``Secrets Manager``


# Autores

| [<img src="https://avatars.githubusercontent.com/u/28829303?s=400&v=4" width=115><br><sub>Christian Melo</sub>](https://github.com/christiandmelo) |  [<img src="https://avatars.githubusercontent.com/u/89987201?v=4" width=115><br><sub>Luiz Soh</sub>](https://github.com/luiz-soh) |  [<img src="https://avatars.githubusercontent.com/u/21027037?v=4" width=115><br><sub>Wagner Neves</sub>](https://github.com/nevesw) |  [<img src="https://avatars.githubusercontent.com/u/34692183?v=4" width=115><br><sub>Mateus Bernardi Marcato</sub>](https://github.com/xXMateus97Xx) |
| :---: | :---: | :---: | :---: |

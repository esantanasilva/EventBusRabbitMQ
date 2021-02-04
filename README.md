# Barramento de Eventos

### Estrutura do Projeto
├── EventBus
├── MicroServiceA
│   └── Worker.cs
└──


##### A estrutura do projeto é formada por dois compoentes: 
    [.netstandard2.0] EventBus - Artefato responsável por gerenciar as publicações, inscrições e consumidores.
    [netcore 3.1] MicroServiceA: Artefato responsável por publicar e consumir as mensagens.

# Setup Docker Compose
```bash
docker-compose -f "docker-compose.yml" up -d --build
```

# Setup Manual (Docker)

## RabbitMQ:
##### Download da Imagem / Inicialização
```bash
docker run -d -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```
[Painel RabbitMQ](http://127.0.0.1:15672/#/)


## Elastic Search
##### Download da Imagem
```bash
docker pull docker.elastic.co/elasticsearch/elasticsearch:7.10.2
```
##### Inicialização:
```bash
docker run -p 9200:9200 -p 9300:9300 -e "discovery.type=single-node" docker.elastic.co/elasticsearch/elasticsearch:7.10.2
```

## Kibana:
##### Download da Imagem
```bash
docker pull docker.elastic.co/kibana/kibana:7.10.2
```
##### Inicialização:
```bash
docker run --link YOUR_ELASTICSEARCH_CONTAINER_NAME_OR_ID:elasticsearch -p 5601:5601 docker.elastic.co/kibana/kibana:7.10.2
```
OBS: __YOUR_ELASTICSEARCH_CONTAINER_NAME_OR_ID__ deve ser alterado para o nome do container do __elastic search__

Link abaixo para visualização dos LOGS:

OBS: __Deverá ser criado um *index patern*__

[Painel logs Kibana](http://127.0.0.1:5601/)


Também foram adicionados logs no console, sendo assim, serão exibidos logs no console da aplicação.

![teste-pratico-docker](https://imgur.com/gallery/J6f5dqx)

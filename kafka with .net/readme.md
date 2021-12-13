### Pull Zookeeper Imange from docker hub
> docker pull confluentic/cp-zookeeper

### Pull kafka image from docker hub
> -docker pull confluentinc/cp-kafka

### Create network
> docker network create kafkaz

### Run zookeeper container
> ```javascript
>docker run -d --network=kafkaz --name=zookeeper -e ZOOKEEPER_CLIENT_PORT=2181 -e ZOOKEEPER_TICK_TIME=2000 -p 2181:2181 confluentinc/cp-zookeeper:5.2.2
>```

### Check if Zookeeper container running
>
```javascript
 docker zookeeper log
 ``` 

### Run kafka Container
>```javascript
> docker run -d --network=kafkaz --name=kafka -p 9092:9092 -e KAFKA_ZOOKEEPER_CONNECT=zookeeper:2181 -e KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://localhost:9092 confluentinc/cp-kafka:5.2.4
>```

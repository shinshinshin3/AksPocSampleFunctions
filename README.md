# AksPocSampleFunctions

## docker-compose

<pre>
$ export DOCKERHOST=$(ifconfig | grep -E "([0-9]{1,3}\.){3}[0-9]{1,3}" | grep -v 127.0.0.1 | awk '{ print $2 }' | cut -f2 -d: | head -n1)
$ docker-compose up -d --build
</pre>

## topic作成

<pre>
$ docker container exec -it "%kafka_container_id%" bash

# kafka contaier shellで実行
bash-4.4# KAFKA_HOME/bin/kafka-topics.sh --create --topic Wng --partitions 4 --replication-factor 1 --zookeeper zookeeper:2181
</pre>

## functionsコンテナの再起動
<pre>
$ docker-compose restart function
</pre>

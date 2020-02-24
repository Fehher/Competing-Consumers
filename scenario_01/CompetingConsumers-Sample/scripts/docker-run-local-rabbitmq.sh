#!bin/bash


#run this command to pull the rabbitmq images if you don't locally, and to get access, go to http://localhost:15672

docker run -d --hostname local-rabbitmq --name rabbitmq -p 15672:15672 -p 5672:5672 -p 25676:25676 rabbitmq:3-management   




---
title: D’un conteneur Docker à MySQL en service (SaaS) sur Azure en 5 minutes
permalink: /dun-conteneur-docker-a-mysql-en-service-saas-sur-azure-en-5-minutes
Published: 2017-05-16
tags: [cloud, database, azure, post, docker, mysql, paas, container, mssql]
---

Bonjour MySQL! Ça fait longtemps! Tu étais qu’à la version 3 quelque chose, et je débutais à peine ma carrière professionnelle. Nous nous sommes bien amusés pendant plusieurs années... Ensuite et bien, tu sais la vie a poursuivi son court et j’ai fait autre chose. J’étais vraiment content lorsque Microsoft a annoncé, au court de sa conférence MSBuild, la disponibilité de MySQL comme Servive (SaaS) dans Azure.

![SearchMySQL](/content/images/2017/05/SearchMySQL.png)

La création d’une base de données MySQL avec le portail est extrêmement simple. Comme d’habitude, vous entrez le nom du serveur, le nom de la base de données et le mot de passe de l’administrateur. Au moment où j’écris ce billet, il n’était pas possible d’employer des commandes CLI, mais je suis convaincu qu’elles seront bientôt disponibles. Pour ceux qui n’ont pas l’habitude d’utilisés les bases de données (SaaS) dans Azure, une chose que vous devrez faire pour accéder à votre base de données à partir de votre ordinateur est d’ajouter l’adresse IP aux règles du pare-feu. À partir du Portail Azure, sélectionnez simplement l’onglet *Connexion Sécurité* dans le menu de gauche et ajoutez votre adresse. Oh! Et n’oubliez pas de cliquer sur le bouton *Save*. ;)

![Firewall](/content/images/2017/05/Firewall.png)
Au cours de ma période d’essai, j’ai testé différentes applications (WordPress, Azure WebApp, application PC) qui utilisent MySQL comme base de données, je n’ai remarqué aucun problème et les performances étaient excellentes. C’était seulement... plus simple; aucun serveur à configurer, aucune VM à configurer, pas de mise à jour. L'unique «problème» que j’ai rencontré était que je n’ai pu me connecter à MySQL à partir de Power BI Desktop, mais je pense que c’est plus lié aux pilotes, car le service était au tout début de la version preview. J’ai informé Microsoft, et je suis sûr qu’ils seront disponibles sous peu.

Comme cela fait un certain temps depuis que j’ai travaillé avec MySQL, je n’avais aucun client d’installé sur mon ordinateur portable. En fait, je n’avais, mais aucune idée de ce qu’il y avait de bon de disponible sur le marché.

Je savais que nous pouvions exécuter des commandes CLI dans un conteneur Docker avec une interface interactive. J’ai donc décidé d’en faire l’essai. Un petit `docker search mysql` rapide me montra qu’une image mysql existait effectivement. Voici toutes les étapes pour configurer "l’installation".

Tout d’abord, téléchargez l’image de MySQL version 8.0 et créez une instance nommée `mySQLTools`:

    docker run --name mySQLTools --env "MYSQL_ROOT_PASSWORD=Passw0rd" -d mysql:8

Ensuite, utilisez-le `-it`, pour rediriger l’invite bash à notre terminal.

    docker exec -it mySQLTools bash -l 
    
Enfin, pour vous connecter à votre base de données en utilisant le client contenu dans le conteneur. Utilisez les paramètres habituels (notez qu’il ne doit pas avoir d’espace entre `-p` et votre mot de passe):

    mysql -h _ServerName.database.windows.net_  -u _UserName@ServerName_ -p_MyPassword_ _DatabaseName_

![result](/content/images/2017/05/result.png)

Le tour est joué! C’est tout ce qu’il faut pour commencer. Et en passant, cela fonctionnera également avec une Azure SQL Database. 

    docker pull shellmaster/sql-cli 
    docker run -it --rm --name=sqlTools shellmaster/sql-cli mssql -s ServerName.database.windows.net  -u UserName@ServerName  -p YourPassword -d DatabaseName -e


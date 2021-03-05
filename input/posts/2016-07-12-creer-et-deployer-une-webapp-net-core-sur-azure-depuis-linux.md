---
title: Créer et déployer une WebApp .NET Core sur Azure depuis Linux
permalink: /creer-et-deployer-une-webapp-net-core-sur-azure-depuis-linux
Published: 2016-07-12
tags: [cloud, azure, linux, donet, core, git, webapp, ubuntu]
---

Tout récemment, j’étais collé à mon ordinateur, et j’avais un peu temps libre (ouaip, je sais très inhabituel). Puisque .Net Core 1.0 venait tout juste d’être rendu public, j’ai décidé de m’amuser avec un peu. Pour ajouter une couche supplémentaire de plaisir dans le mix, j'ai décidé de le faire depuis ma machine virtuelle (VM) Ubuntu. En moins de 15 minutes, tout a été fait! J’étais tellement impressionné que j’ai su immédiatement que devais en parler. Voilà exactement la raison de ce billet.

Avant de commencer
------------------

Avant de commencer, il est important de savoir quelle version d’Ubuntu que vous utilisez, certaines commandes varient légèrement selon les versions. Pour connaitre la version que vous avez, vous devez simplement cliquer sur la roue dentelée en haut à droite de l’écran (sur le bureau) et sélectionnez * About this Computer *. Dans mon cas, puisque j’utilise Ubuntu 14.04 LTS, j’exécuterai les commandes liées à cette version. Si vous utilisez une version différente, référez-vous à la [documentation .NET Core][dotnetdoc].

![ubuntuVersion](/content/images/2016/07/ubuntu_version.png)

Maintenant, nous devons installer .Net Core. Rien de plus simple. Ouvrez un Terminal (Ctrl+Alt+T) et saisir les trois commandes suivantes:

    # Ajoute à apt-get la source .Net Core
    sudo sh -c ’echo "deb [arch=amd64] https://apt-mo.trafficmanager.net/repos/dotnet/ trusty main" > /etc/apt/sources.list.d/dotnetdev.list'
    apt-key adv --keyserver apt-mo.trafficmanager.net --recv-keys 417A0893
    
    # Télécharge tous les dernières mises a jour
    apt-get update

    # Installe le SDK .NET Core
    sudo apt-get install dotnet-dev-1.0.0-preview2-003121

Une fois que tout cela est fait, vous pouvez exécuter `dotnet --info` et vous devriez lire quelque chose de semblable à ceci.

![dotnetInfo](/content/images/2016/07/dotnet_info.png)

Créer la version locale
----------------------

Toujours du même Terminal, nous allons créer un dossier vide pour notre solution et nous y déplacer. Pour ce faire, exécuter ces commandes.

    mkdir demodotnetweb
    cd demodotnetweb

Nous allons maintenant créer notre nouveau projet web. Cela se fait par la commande `dotnet new`, mais nous devons spécifier le type sinon une c’est une application console qui sera créée par défaut.

    dotnet new -t web

Ensuite, pour télécharger toutes les références (NuGet packages) requises à notre projet, exécutez la commande suivante:

    dotnet restore

Selon la vitesse de votre connexion Internet et combien de dépendances sont requises, cette mise à jour peut prendre de quelques secondes à plus d’une minute.

Pour tester la solution localement, saisir la commande:

    dotnet run

Cela compilera notre solution et grâce à *AspNetCore Internal hosting* vous pourrez visualiser le site dans un navigateur Web à l’adresse http://localhost:5000.

![dotnetlocalhost](/content/images/2016/07/dotnetcore_localhost.png)

Déployer dans Azure
-------------------

Pour déployer notre nouveau projet dans le cloud, nous allons utiliser la capacité de déploiement continu d’Azure. Commecont pas accédez au portal en allant à portal.azure.com et créer une WebApp.

![createWebApp](/content/images/2016/07/create_webApp.png)

Une fois que l’application est créée, vous devriez voir le message *This web app as been created* lorsque vous accédez à la page: `[nameofWebApp].azurewebsites.net`

![created](/content/images/2016/07/successfully_created.png)

Il est maintenant temps d’y ajouter un contrôle de source (repository) local Git. Dans les *Settings* de l’application web, sélectionner Deployment source. Cliquez sur *Configure Required Settings*, puis sélectionnez l’option *Local Git Repository*.

![addSourControl](/content/images/2016/07/add_source_control.png)

Une fois que l’identification de l'utilisateur pour le contrôle de source terminé, nous pouvons obtenir l’URL de ce dernier à partir de la section *Essential*. Nous en aurons besoin pour la prochaine étape.

![repourl](/content/images/2016/07/repourl.png)

De Retour à notre Terminal Ubuntu, nous pouvons exécuter les commandes suivantes:

    # Cree le contrôle de source local
    git init
    
    # commit tous les fichiers
    git commit -m "Init"
    
    # Ajout du contrôle de source dans Azure 
    git remote add azure https://username@demowebcore.scm.azurewebsites.net:443/demowebcore.git

    # Pousse le code dans Azure
    git push azure master

Après environ une minute, vous devriez avoir votre WebApp en ligne!


![dotnetazure](/content/images/2016/07/dotnetcore_azure.png)



[dotnetdoc]: https://dotnet.github.io/

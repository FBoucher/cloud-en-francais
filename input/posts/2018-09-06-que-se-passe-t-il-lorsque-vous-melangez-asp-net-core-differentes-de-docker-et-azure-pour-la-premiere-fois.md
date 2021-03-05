---
title: Que se passe-t-il lorsque vous mélangez Asp.Net Core, différentes de Docker et Azure pour la première fois?
permalink: /que-se-passe-t-il-lorsque-vous-melangez-asp-net-core-differentes-de-docker-et-azure-pour-la-premiere-fois
Published: 2018-09-06
tags: [cloud, azure, post, azurecli, windows, linux, webapp, docker, container, aspnetcore, dockertool, registry]
---


Pour un projet perso, je voulais valider si les conteneurs étaient plus faciles à utiliser comparés au code normal avec les services dans Azure. J’avais besoin de rafraîchir mes connaissances avec Docker, alors je décidé de faire ce que je pensais être un simple test: créer un site Web Asp.Net Core dans un conteneur et le déployer dans différents environnements. Ce billet est en fait tout mon parcours pour finalement atteindre cet objectif, comme vous pouvez le deviner ça n’a pas fonctionné au premier essai.

## L’objectif

L’une des raisons pour lesquelles je pensais aux conteneurs, c’est qu’en théorie ils fonctionnent partout, n’est-ce pas? Ce n’est pas faux, mais parfois avec un peu d’effort. Le but ici est de pouvoir exécuter le même conteneur sur mon ordinateur principal, sur ma surface, sur une machine virtuelle Linux et, bien entendu, sur Azure.

## Le contexte

J’ai une configuration différente sur ma machine principale et sur ma surface. Sur mon PC, j’utilise VirtualBox pour mes VMs, donc je ne lance pas Docker pour Windows, mais Docker Toolbox. Cette (ancienne) version de Docker créera une VM dans VitualBox au lieu d’Hyper-V. Je ne pouvais pas utiliser Docker pour Windows comme sur ma surface, car les deux logiciels de virtualisation ne fonctionnent pas côte à côte.

Je voulais aussi utiliser uniquement les outils disponibles sur chacune de ces plates-formes, alors j’ai décidé de ne pas utiliser Visual Studio IDE. De plus, je voulais comprendre ce qui se passait, alors je ne voulais pas trop de "magie". Mais sachez que Visual Studio est un outil fantastique et je l’adore et l’utilise beaucoup. :)

## Installer Docker

Je devais installer Docker sur ma surface. J’ai téléchargé Docker Community Edition (CE) et, comme Hyper-V était déjà installé, tout fonctionnait correctement. Sous Windows, vous devez partager le lecteur "C" (à partir des Settings de Docker). Cependant, j’avais un étrange "bug" en partageant le mien. Il me demandait de me connecter à AzureAD et ignorait ma demande en laissant le lecteur non partagé et ne conservait pas mon crochet dans la case à cocher.

![dockerazureadcredentials](/content/images/2018/09/dockerazureadcredentials.png)

Grâce à mon nouvel ami Tom Chantler, je n’ai pas eu à chercher trop longtemps. Ce qu’il faut s’avoir, c’est que j’utilise un compte AzureAD pour me connecter, et quelque chose ne fonctionne pas correctement pour le moment. Comme expliqué dans l’article de Tom: [Sharing your C drive with Docker for Windows when using Azure Active Directory](https://tomssl.com/2018/01/11/sharing-your-c-drive-with-docker-for-windows-when-using-azure-active-directory-azuread-aad/), pour créer cette situation, je n’ai eu qu’à créer un nouveau compte d’utilisateur avec le nom exact comme compte AzureAD, mais sans le préfixe AzureAD (ex: AzureAD\FBoucher est devenu FBoucher). Une fois cela fait, j’ai pu partager le disque sans aucun problème.

## Commençons par la documentation

Le conteneur HelloWord fonctionnait comme un charme, alors j’étais prêt à créer mon site Web Asp.Net Core. Mon réflexe était d’aller sur docs.docker.com et de suivre les instructions de [Create a Dockerfile for an ASP.NET Core application](https://docs.docker.com/engine/examples/dotnetcore/#create-a-dockerfile-for-an-aspnet-core-application). J’ai du faire quelque chose de mal, car ça ne fonctionnait pas pour moi. J’ai donc décidé de partir de zéro et de faire chaque étape manuellement ... J’apprends toujours plus de cette façon.

## Commençons par le début

Avant de tout déplacer dans un conteneur, nous avons besoin d’une application Web. Cela peut se faire facilement depuis l’invite du terminal (le prompt), avec les commandes:

    dotnet new mvc -o dotnetcoredockerappservicedemo

    cd dotnetcoredockerappservicedemo

    dotnet restore

    dotnet publish -c releae -o app/ .

Ici, nous créons un nouveau dossier contenant un site Web en utilisant le modèle mcv. Ensuite nous changeons le répertoire courant pour ce nouveau dossier et restaurer les paquets Nuget. Pour tester le site web localement, utilisez simplement `dotnet run`. Et enfin, nous construisons et publions l’application dans le sous-dossier * app *.

![dotnet run](/content/images/2018/09/dotnet-run.png)

## Conteneur Docker

Maintenant que nous avons notre application, il est temps de la "conteneuriser". Nous devons ajouter les instructions Docker dans un `dockerfile`. Ajoutez un nouveau nom de fichier `dockerfile` (pas d’extension) au dossier racine et copiez / collez ces commandes suivantes:

    # dockerfile

    FROM microsoft/dotnet:2.1-aspnetcore-runtime
    WORKDIR /app
    COPY /app /app
    ENTRYPOINT [ "dotnet" , "dotnetcoredockerappservicedemo.dll"]

> Pour démarrer Docker avec Docker Tool, démarrez simplement le terminal Docker Quickstart.

Ces instructions spécifient comment construire notre conteneur. D’abord, il téléchargera l’image `microsoft/dotnet:2.1-aspnetcore-runtime`. Nous spécifions le répertoire de travail, puis copions le contenu du dossier `app` local dans le dossier `app` à l’intérieur du conteneur. Enfin, nous spécifions le point d’entrée de notre application en lui indiquant de commencer par `dotnet`.

Comme Git et son fichier gitIgnore, Docker a la même fonctionnalité avec `.dockerignore` (pas d’extension). Ajoutez ce fichier dans votre dossier pour ignorer le dossier bin et obj.

    # .dockerignore

    bin\
    obj\

Maintenant que les instructions sur la façon de construire notre conteneur sont terminées, nous pouvons construire notre conteneur. Exécutez la commande suivante:

    docker build -t dotnetcoredockerappservicedemo .

Cela va générer *dotnetcoredockerappservicedemo* à partir du dossier en cours.


## Exécuter le conteneur Docker localement

Tout est en place, la seule chose qui reste à faire est de l’exécuter. Si vous voulez l’exécuter localement, exécuter simplement cette commande:

    docker run -p 8181:80 dotnetcoredockerappservicedemo

Sur ma machine, le port 80 est toujours utilisé. Donc, je redirige (remap) le port 80 sur 8181, n’hésitez pas à le modifier à votre convenance. Le site Web sera disponible sur localhost:8181

Si vous utilisez Docker Tool, vous devez obtenir l’adresse IP de votre machine virtuelle. Pour l’obtenir, le faire la commande:

    docker-machine ip

## Exécuter le conteneur Docker dans Azure

Pour exécuter notre conteneur dans Azure, nous devons d’abord le publier sur le web (dans le nuage). Cela pourrait être sur DockerHub ou dans un registre privé sur Azure. J’ai décidé de partir avec Azure. Nous devons donc créer un enregistre, puis y publier notre conteneur.

    az group create --name dotnetcoredockerappservicedemo --location eastus

    az acr create --resource-group dotnetcoredockerappservicedemo --name frankContainerDemo01 --sku Basic --admin-enabled true

    az acr credential show -n frankContainerDemo01

La dernière commande `az acr credential show` fournira des informations pour "tagger" notre conteneur avec notre nom de registre et nous fournira également les informations d’identification nécessaires pour pouvoir pousser. Bien sûr, toutes c’est informations sont aussi disponible sur le site portal.azure.com  à partir du panneau (blade) du registre sous la section *Access Keys*.

	docker tag dotnetcoredockerappservicedemo frankcontainerdemo01.azurecr.io/dotnetcoredockerappservicedemo:v1

Connectons Docker à notre registre dans Azure, puis poussons (téléchargez) notre conteneur vers Azure.

    # The 'https://' is important...

    docker login https://frankcontainerdemo01.azurecr.io -u frankContainerDemo01 -p <Password_Retreived>

    docker push frankcontainerdemo01.azurecr.io/dotnetcoredockerappservicedemo:v1

![Docker push](/content/images/2018/09/DockerPush.gif)

Super le conteneur est dans Azure. Maintenant, créons une simple webApp pour le voir. Nous pourrions également utiliser Azure Container Instance (ACI), mais comme cette démo est un site web, cela ne serait pas très logique.

Pour obtenir une *Application service*, nous avons besoin d’un plan de service, puis nous créerons une application Web "vide". Pour ce faire, nous spécifierons le `runtime` sans fournir de code/ binary/ conteneur. Je n’ai pas réussi à créer une application Web à partir d’un registre privé Azure en une seule commande, c’est pourquoi je le fais en deux.

    az appservice plan create --name demoplan --resource-group dotnetcoredockerappservicedemo --sku S1 --is-linux

    az webapp create -g dotnetcoredockerappservicedemo -p demoplan -n frankdockerdemo --runtime "DOTNETCORE|2.1"

> Sous Windows, je recevais le message d’erreur suivant: '2.1' is not recognized as an internal or external command, operable program or batch file.
> Il suffit d’utiliser la commande *escape* de PowerShell "--%" pour résoudre le problème:
> `az --% webapp create -g dotnetcoredockerappservicedemo -p demoplan -n frankdockerdemo  --runtime "DOTNETCORE|2.1"`

Si vous consultez le site Web dès maintenant, vous devriez avoir une page indiquant que le site est en place, mais vide, c’est normal. Pour changer ce statut, nous allons modifier les paramètres du conteneur avec nos paramètres de registre et de conteneur.


    az webapp config container set -n frankdockerdemo -g dotnetcoredockerappservicedemo --docker-custom-image-name frankcontainerdemo01.azurecr.io/dotnetcoredockerappservicedemo:v1 --docker-registry-server-url https://frankcontainerdemo01.azurecr.io --docker-registry-server-user frankContainerDemo01 --docker-registry-server-password <Password_Retreived>

Voilà! Ça fonctionne bien sûr!

![](/content/images/2018/09/final2.png)

## Conclusion

Ce n’est que quatre étapes: créer l’application .Net Core, la placer dans un conteneur Docker, publier notre conteneur dans notre registre Azure et créer un service sur ce conteneur. Cependant, comme toutes ces technologies sont  multi plates-formes, à l’occasion il y aura de toutes petites différences entre les plate-formes et ceci peut devenir une importante source de perte de temps. Un super petit projet qui s’est avéré beaucoup plus que prévu, et j’y ai appris beaucoup!

Je suis très content du résultat...

En vidéo s’il vous plaît
------------------------

J’ai aussi une vidéo de ce post si vous préférez.

<div class="container">
<iframe  class="youtubevideo" width="560"  src="https://www.youtube.com/embed/4c0Cg0hmA4U" frameborder="0" allow="autoplay; encrypted-media" allowfullscreen></iframe>
</div>

###### Références

- [Sharing your C drive with Docker for Windows when using Azure Active Directory](https://tomssl.com/2018/01/11/sharing-your-c-drive-with-docker-for-windows-when-using-azure-active-directory-azuread-aad/)


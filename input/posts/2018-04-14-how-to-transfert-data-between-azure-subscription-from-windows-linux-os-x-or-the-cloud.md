---
title: Comment copier des fichiers entre abonnements Azure depuis Windows, Linux, ou OS X
permalink: /how-to-transfert-data-between-azure-subscription-from-windows-linux-os-x-or-the-cloud
Published: 2018-04-14
tags: [cloud, azure, post, azcopy, azurecli, windows, linux, osx, storage]
---

<span style="color: #666666;">(in English: <a href="http://www.frankysnotes.com/2016/04/how-to-transfert-data-between-azure.html">here</a>)</span> 


> Copier, charger ou télécharger depuis, ou à partir, peu importe la combinaison entre Windows, Linux, OS X, ou le cloud

Nos données sont et resteront toujours notre principale préoccupation. Que ce soit sous forme de fichiers texte, images, disque de machine virtuelle ou toute autre forme, à un moment donné dans le temps de nos données devront être déplacées. Je l'ai déjà [écris à ce sujet auparavant](http://www.frankysnotes.com/2013/01/how-to-copy-blobs-or-vhds-between.html), et le contenu de ce billet est toujours valide aujourd'hui, mais je voulais partager de nouvelles options et couvrir plus de cas (ce qui signifie Linux, Windows et OS X).

## Scénarios

Voici quelques scénarios illustrant pourquoi vous voudriez déplacer des données.

- La période d'essai de Microsoft Azure finira sous peu et vous voulez garder les informations qui y sont sauvegardées.
- Vous créer une nouvelle application Web et toutes ces images doivent être déplacées
- Vous avez une machine virtuelle que vous souhaitez déplacer dans le cloud ou dans une autre subscription.
- ...

<!--more-->

## AZCopy

AzCopy est un outil de ligne de commande fantastique pour copier des données vers et à partir de Microsoft Azure storage, que ce soit blob, fichier, ou table. Avant AzCopy était uniquement disponible sur Windows. Cependant, récemment une deuxième version construite avec .NET Core Framework est disponible. Les commandes sont très similaires mais pas exactement identiques.

### AZCopy sur Windows

Dans sa plus simple expression, une commande AzCopy ressemble à ceci:

    AzCopy /Source:<source> /Dest:<destination> [Options]

Si vous avez déjà Azure SDK d'installé sur votre PC, vous avez déjà tout ce qu'il vous faut. Par défaut AzCopy s'installera dans `%ProgramFiles(x86)%\Microsoft SDKs\Azure\AzCopy` (Windows 64-bit) ou `%ProgramFiles%\Microsoft SDKs\Azure\AzCopy` (Windows 32-bit). 

Si vous avez seulement besoin d'AzCopy, et non pas de tout le SDK, vous pouvez télécharger la [dernière version](http://aka.ms/downloadazcopy) de AzCopy.

#### Téléchargement vers Azure

Tout d'abord, supposons que vous devez déplacer une grande quantité d'images d'un serveur vers un *Azure blob storage*. Rien de plus simple, il suffit de passer le chemin d'accès où sont situé les images, le lien url vers le conteneur Azure et sa clé d'accès.

    AzCopy /Source:C:\MyWebApp\images /Dest:https://frankysnotes.blob.core.windows.net/blog /DestKey:4YvvYDTg3UUpky8Rj5bDG4KO/R1FdtssxVnunsEd/4rAS04V2LkO0F8mXbddAv39WtCo5LW6JyvfhA== /S

![CopyAllImages][CopyAllImages]

Lorsqu'on manipule des données dans Azure storage, deux informations sont constamment requises: le nom du compte (Account Name) et la clé d'accès. Ces deux informations peuvent être aisément trouvées dans le portail Azure. Depuis le portail (https://portal.azure.com), sélectionnez le compte de stockage. Dans la bande de droite représentant les paramètres, cliquez sur Clés d'accès (Access Keys).

![StorageAccessKeys][StorageAccessKeys]

#### Copie entre abonnements Azure

Tout aussi facilement nous pouvons copier les images dans un autre storage, peu importe qu’il soit dans une autre région ou carrément sous un autre abonnement.

    AzCopy /Source:https://frankysnotes.blob.core.windows.net/blog /Dest:https://frankshare.blob.core.windows.net/imagesbackup /SourceKey:4YvvYDTg3UUpky8Rj5bDG4KO/R1FdtssxVnunsEd/4rAS04V2LkO0F8mXbddAv39WtCo5LW6JyvfhA== /DestKey:EwXpZ2uZ3zrjEbpBGDfsefWkj3G2QY5fJcb6kMqV2A0+2TsGno+mk9vEXc5Uw1XiouvAiTS7Kr5OGzA== /S
    
#### Les paramètres d'AzCopy

Les exemples contenus dans ce billet sont volontairement simples, mais AzCopy est un outil très puissant. Je vous invite à entrer l'une des commandes suivantes pour en savoir davantage sur toute les possibilités qu'offre AzCopy:

- Pour une aide détaillée à propos des lignes de commande d'AzCopy: `AzCopy /?`
- Pour d'autres exemples de ligne de commandes: `AzCopy /?:Samples`

### AzCopy sur Linux

Avant de pouvoir installer AzCopy, vous devez installer le framework .Net Core. Ceci est fait très simplement avec quelques commandes.

```bash

curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg
sudo mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg
sudo sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-ubuntu-xenial-prod xenial main" > /etc/apt/sources.list.d/dotnetdev.list'
sudo apt-get update
sudo apt-get install dotnet-sdk-2.0.2

```
Ensuite, pour l'installer il vous suffit de télécharger avec une commande wget, de décompresser le tout et d'exécuter le script d'installation.

```bash

wget -O azcopy.tar.gz https://aka.ms/downloadazcopyprlinux
tar -xf azcopy.tar.gz
sudo ./install.sh

```

Dans sa plus simple expression, la version .Net Core d'une commande AzCopy ressemble à ceci:

    azcopy --source <source> --destination <destination> [Options]

On remarque imédiatement la ressemblence à la version originale, mais les paramètres utilisent `--` et` -` au lieu de `/` et les `:` sont maintenant de simple espace.

#### Télécharger vers Azure

Voici un exemple pour copier un seul fichier `GlobalDevopsBootcamp.jpg` dans un Azure Blob Storage. Nous passons le chemin dàccèss local complet du fichier via `--source`, la destination est l'URI complet, et enfin la clé de stockage pour le blob de destination. Bien sûr, vous pouvez également utiliser le jeton SAS si vous préférez.

```bash
azcopy \
--source /home/frank/demo/GlobalDevopsBootcamp.jpg \
--destination https://frankysnotes.blob.core.windows.net/blog/GlobalDevopsBootcamp.jpg \
--dest-key 4YvvYDTg3UUpky8Rj5bDG4KO/R1FdtssxVnunsEd/4rAS04V2LkO0F8mXbddAv39WtCo5LW6JyvfhA==

```

#### Copier entre abonnements (subscriptions) Azure 

Pour copier l'image dans un second abonnement Azure, nous utilisons la commande: la source est désormais un URI Azure Storage et nous transmettons les clés source et de destination:

    azcopy \
    --source https://frankysnotes.blob.core.windows.net/blog/GlobalDevopsBootcamp.jpg \
    --destination https://frankshare.blob.core.windows.net/imagesbackup/GlobalDevopsBootcamp.jpg \
    --source-key 4YvvYDTg3UUpky8Rj5bDG4KO/R1FdtssxVnunsEd/4rAS04V2LkO0F8mXbddAv39WtCo5LW6JyvfhA== \
    --dest-key EwXpZ2uZ3zrjEbpBGDfsefWkj3G2QY5fJcb6kMqV2A0+2TsGno+mk9vEXc5Uw1XiouvAiTS7Kr5OGzA== 

## Azure CLI

Azure CLI est un ensemble de commandes multiplateforme pour la plate-forme Azure. Il permet de manipuler tous les composants Azure, cependant ce billet se concentrera sur certaines fonctionnalités de `azure storage`.

Deux versions de l'interface de ligne de commande Azure (CLI) sont actuellement disponibles:

- Azure CLI 2.0: écrit en Python, compatible uniquement avec le modèle de déploiement Resource Manager.
- Azure CLI 1.0: écrit dans Node.js, compatible avec les modèles de déploiement classiques et Resource Manager.

> Azure CLI 1.0 est obsolète et ne doit être utilisé que pour le support du modèle Azure Service Management (ASM) avec des ressources "classiques".

### Installer Azure CLI

Commençons par installer Azure CLI. Bien sûr, il est possible de télécharger le programme d’installation, mais puisque tout évolue tellement vite, pourquoi ne pas l’obtenir directement à partir de Node.je et son gestionnaire de paquets (npm). L'installation sera la même, il vous suffit de spécifier la version si vous avez absolument besoin d'Azure CLI 1.0.

    sudo npm install azure-cli -g
    
![AzureCliInstalled][AzureCliInstalled]

### Azure CLI 1.0 (Windows, Linux, OS X, Docker)

Pour continuer avec les scénarios précédents, nous allons copier toutes les images dans un *blob storage*. Malheureusement, Azure CLI ne possède pas la même flexibilité qu’AzCopy. Il fonctionne seulement qu'un fichier à la fois. Néanmoins, pour télécharger toutes les images d’un dossier, il s’agit tout bonnement de mettre la commande dans une boucle:

    for f in Documents/images/*.jpg
       do
       azure storage blob upload -a frankysnotes -k YoMjXMDe+694FGgOaN0oaRdOF6s1ktMgkB6pBx2vnAr8AOXm3HTF7tT0NQWvGrWnWj5m4X1U0HIPUIAA==  $f blogimages
    done

Dans la commande précédente, `-a` représente le nom du compte, et `-k` la clé d'accès. 

![AzureCliAllImages][AzureCliAllImages]

Pour copier un fichier (ex. : un disque d’une machine virtuelle alias VHD) entre des espaces stockages, peu importe qu'ils soient dans des régions, ou des abonnements différents, c’est tout simple. Cette fois, nous utiliserons la commande `azure storage blob copy start` le `-a` et `-k` sont liés à la destination.

    azure storage blob copy start 'https://frankysnotes.blob.core.windows.net/blogimages/20151011_151451.MOV' imagesbackup -k EwXpZ2uZ3zrjEbpBGDfsefWkj3GnuFdPCt2QY5fJcb6kMqV2A0+2TsGno+mk9vEXc5Uw1XiouvAiTS7Kr5OGzA== -a frankshare

Un fait intéressant à propos de cette commande, c’est qu’elle est asynchrone. Pour voir l’état de votre copie, il faut simplement exécuter la commande `azure storage blob copy show`

    azure storage blob copy show -a frankshare -k YoMjXMDe+694FGgOaN0oPaRdOF6s1ktMgkB6pBx2vnAr8AOXm3HTF7tT0NQVxsqhWvGrWnWj5m4X1U0HIPUIAA== imagesbackup 20151011_151451.MOV

![CopyStatus1][CopyStatus1]

![CopyStatus2][CopyStatus2]

### Azure CLI 2.0 (Windows, Linux, OS X, Docker, Cloud Shell)

Azure CLI 2.0 est la nouvelle ligne de commande d'Azure optimisée pour la gestion et l'administration des ressources Azure qui fonctionnent avec Azure Resource Manager. Comme la version précédente, il fonctionnera parfaitement sur Windows, Linux, OS X, Docker mais aussi sur Cloud Shell!

![cloudShell](/content/images/2018/04/cloudShell.png)

Cloud Shell est disponible directement depuis le portail Azure, et ne requière aucun plugging!

#### Téléchargement vers Azure

La commande est la même que la verion précédente sauf que maintenant la commande s'appelle `az`. Voici un exemple pour télécharger un seul fichier dans un blob Azure Storage.

```bash
az storage blob upload --file /home/frank/demo/CloudIsStrong.jpg \
--account-name frankysnotes \
--container-name blogimages --name CloudIsStrong.jpg \
--account-key 4YvvYDTg3UUpky8Rj5bDG4KO/R1FdtssxVnunsEd/4rAS04V2LkO0F8mXbddAv39WtCo5LW6JyvfhA==

```

#### Copie entre abonnements Azure

Copions maintenant le fichier dans un autre abonnement Azure. Une chose qui doit être mentionné, est que `--account-name` et` --account-key` sont pour la **destination**, même si ce n'est pas spécifiée.

```bash
az storage blob copy start \
--source-account-name frankysnotes  --source-account-key 4YvvYDTg3UUpky8Rj5bDG4KO/R1FdtssxVnunsEd/4rAS04V2LkO0F8mXbddAv39WtCo5LW6JyvfhA== \
--source-container blogimages --source-blob CloudIsStrong.jpg   \
--account-name frankshare  --account-key EwXpZ2uZ3zrjEbpBGDfsefWkj3G2QY5fJcb6kMqV2A0+2TsGno+mk9vEXc5Uw1XiouvAiTS7Kr5OGzA== \
--destination-container imagesbackup  \
--destination-blob CloudIsStrong.jpg 

```
## Explication en vidéo svp!

Si vous préféré jài une version en vidéo disponible. 

<div class="container">
<iframe class="youtubevideo" width="560" src="https://www.youtube.com/embed/8_-uXKHmu8w" frameborder="0" allowfullscreen></iframe>
</div>

## Une dernire chose

Parfois, nous n'avons pas besoin d'écrire des scripts, et une interface graphique est la bienvenue. Pour ces situations, l'outil d'on personne ne devrait se passer est Azure Storage Explorer. Ça fait tout! Télécharger, téléverser et gérer des blobs, des fichiers, des files d'attente, des tables et des entités Cosmos DB. Et il fonctionne sur Windows, macOS et Linux!

![AzureStorageExplorer](/content/images/2018/04/AzureStorageExplorer.png)

## Ce ne sont que les premiers pas

Ce poste est simplement une introduction à deux outils très puissants. Il est fortement recommandé d’aller creuser dans la documentation officielle pour en savoir plus.

Utilisez les commentaires pour partager toutes vos questions et suggestions.

##### Références:

- [Azure AZCopy for Windows](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azcopy)
- [Azure AZCopy for Linux](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azcopy-linux)
- [Azure CLI 1.0](https://docs.microsoft.com/en-us/cli/azure/install-cli-version-1.0?view=azure-cli-latest)
- [Azure CLI 2.0](https://docs.microsoft.com/en-us/cli/azure/?view=azure-cli-latest)
- [Azure Storage Explorer](https://azure.microsoft.com/en-us/features/storage-explorer/)




[OldPost]: http://www.frankysnotes.com/2013/01/how-to-copy-blobs-or-vhds-between.html

[CopyAllImages]: /content/images/2016/04/CopyAllImages.png
[AzureCliInstalled]: /content/images/2016/04/AzureCli_installed.png
[AzureCliAllImages]: /content/images/2016/04/azurecli_allimages.png
[StorageAccessKeys]: /content/images/2016/04/StorageAccessKeys.png
[CopyStatus1]: /content/images/2016/04/CopyStatus1.png
[CopyStatus2]: /content/images/2016/04/CopyStatus2.png


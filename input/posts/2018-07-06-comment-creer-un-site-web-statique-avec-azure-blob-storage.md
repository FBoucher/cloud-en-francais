---
title: Comment créer un site web statique avec Azure Blob Storage
permalink: /comment-creer-un-site-web-statique-avec-azure-blob-storage
Published: 2018-07-06
tags: [website, cloud, azure, post, azcopy, azurecli, storage, vscode, static, statique, siteweb]
---


J’attendais cette fonctionnalité depuis tellement longtemps! Je sais, ce n’est pas une fonctionnalité révolutionnaire, mais elle comble une lacune importante dans l’offre Azure. Nous pouvons désormais créer des sites Web statiques dans Azure Blob Storage (au moment où j’écris ce post c’est encore Preview). Dans ce post, je vais vous expliquer pourquoi je pense que c’est vraiment une bonne nouvelle, montrez comment créer site statique, et le publier sur Azure.

## Pourquoi cette nouvelle m’emballe

Le cloud est l’endroit idéal lorsque vous avez besoin de construire quelque chose d’énorme, et très rapidement. C’est aussi une excellente solution lorsque vous avez beaucoup de variation dans la quantité de ressources nécessaires. Étant donné qu’Azure est un service, il vous fournira autant de ressources que vous le souhaitez dans quelques minutes. Et quand vous aurez fini avec ces dernières, vous les détruisez et arrêtez de payer pour eux; c’est vraiment génial comme ça!

Cependant, si la seule chose dont vous avez besoin était d’héberger une petite application telqu'un blogue ou un petit site web pour un événement ou une publicité temporaire, Azure n’était pas le meilleur endroit pour cela. Bien évidemment, vous pouviez avoir un service et héberger de nombreux petits sites Web (Scott Hanselman a écrit d’excellents articles à ce sujet d’on [celui-ci](https://www.hanselman.com/blog/PennyPinchingInTheCloudWhenDoAzureWebsitesMakeSense.aspx)), mais il cela semblait compliquer et effrayait un peu certain utilisateur. Bon nombre de personnes ont conservé un fournisseur d’hébergement "à ancienne" juste pour cela. C’est bon comme solution, ça fonctionne... Mais avec l’Azure Storage, ce sera vraiment fiable, et à moindre coût! Voyons voir comment nous pouvons en créer un.

## Créer un site Web statique

Pour pouvoir créer un site web statique, vous aurez besoin d’un compte Azure Blob Storage bien sûr. Vous pouvez en créer un nouveau de la même manière qu’avant à l’exception qu’il vous faudra sélectionner la sorte: General Purpuse V2 (GPV2). Ensuite pour créer le site web, si vous installez Azure CLI Storage-extension (Preview), vous pouvez l’utiliser pour en créer un, ou tout simplement aller sur portal.azure.com. Utilisons le portail, car c’est plus visuel.

![createstorage](/content/images/2018/07/createStorage.png)

Une fois le stockage créé, sélectionnez-le dans le portail. Dans le menu de gauche de cette blade, cliquez sur l’option *Static site (preview)*. Cela ouvrira la page de configuration de notre site Web statique. Cliquez d’abord sur le bouton *Enabled*, puis entrez le nom du document initial / index (ex: index.html). Enfin, cliquez sur le bouton *Save* situé en haut de la blade.

![ConfigureStatic](/content/images/2018/07/ConfigureStatic.png)

Maintenant que la base de notre site web est maintenant créée. Un nouveau conteneur Azure Blob Storage nommé `$web` a été créé. Les points de terminaison (endpoint) primaires et secondaires devraient maintenant être affichés (ex: https://frankdemo.z13.web.core.windows.net/). Si vous testez cette URL, vous verrez et un message indiquant que le contenu n’existe pas ... et c’est normal.

![emptywebsite](/content/images/2018/07/emptywebsite.png)

## Créer le contenu

C’est la partie peut-être optionnelle, tout dépend de vos besoins. Il se peut que vous ayez déjà des pages HTML à votre disposition, ou que vous souhaitiez les coder vous-même ou que le site Web existe déjà. Pour ce post, je vais créer un tout nouveau blogue en utilisant un générateur de site Web statique appelé Wyam (si préféré Jekyll, un autre générateur, je l’ai utilisé dans ce [vidéo](https://youtu.be/YSUgMyzTjIA))

Pour créer un nouveau site avec Wyam, utilisez la commande suivante dans une invite de commande. Cela va créer un nouveau site web dans le sous-dossier `output`.

    wyam --recipe Blog --theme CleanBlog

## Publier sur Azure

Il est maintenant temps de télécharger notre contenu dans l’Azure blob Storage. Le plus facile est probablement directement à partir du portail. Pour télécharger un fichier, cliquez sur le conteneur `$web`, puis sur le bouton *Upload*. À partir du nouveau formulaire, sélectionnez le fichier et téléchargez-le.

![portalUpload](/content/images/2018/07/portalUpload.png)

Le principal problème de cette méthode est qu’elle ne fonctionne que sur un seul fichier à la fois ... Et un site web a généralement beaucoup de fichiers...

Un moyen plus efficace serait d’utiliser un outil tel qu’Azure Explorer ou d’écrire un script. Azure Explorer ne supporte pas encore les site Web statiques avec Azure Blob Storage, mais il le fera bientôt. Ce qui nous amène aux scripts.

### AzCopy

J’aime vraiment AZCopy c’est un outil très efficace et facile à utiliser. Malheureusement, au moment d’écrire ce post, AzCopy ne supporte pas les site Web static sur Azure Storage. J’ai tout de même essayé de télécharger tout le contenu du dossier output (et des sous-dossiers)) avec une commande comme celle-ci, mais ça n’a pas fonctionné.

    azcopy --source ./output --destination https://frankdemo.blob.core.windows.net/$web --dest-key fec1acb473aa47cba3aa77fa6ca0c5fdfec1acb473aa47cba3aa77fa6ca0c5fd== --recursive

### Azure CLI

L’[Azure CLI extension preview](https://github.com/Azure/azure-cli-extensions/tree/master/src/storage-preview) est également disponible.  Pour uploader les fichiers les commandes habituelles feront très bien le travaille il suffit de faire attention au charactère `$`. Merci à Carl-Hugo (@CarlHugoM) pour son aide... ;)

```bash
    az storage blob upload-batch -s "./output" -d $"web" --account-key fec1acb473aa47cba3aa77fa6ca0c5fdfec1acb473aa47cba3aa77fa6ca0c5fd== --account-name frankdemo
```

```bash
    az storage blob upload -f "./output/index.html" -c $"web" -n index.html ---account-key fec1acb473aa47cba3aa77fa6ca0c5fdfec1acb473aa47cba3aa77fa6ca0c5fd== --account-name frankdemo
```

### Visual Studio Code Azure Storage Extension

J’ai finalement essayé l’[Extension Stogare de Visual Studio Code](https://github.com/Microsoft/vscode-azurestorage#preview-features). Après l’avoir installé, vous devez ajouter un User Setting `Ctrl +,`. Puis ajoutez `"azureStorage.preview.staticWebsites": true` dans votre configuration. Il vous suffit maintenant de cliquer sur l’extension, puis sélectionnez Azure blob storage dans votre abonnement, et faites un clic droit pour télécharger un dossier.

![vscodeupload](/content/images/2018/07/vscodeupload.png)

En fonction du nombre de fichiers et de leur taille, cela prendra un moment. VSCode vous notifiera quand le tout sera terminé. Vous pourrez ensuite retourner en ligne et rafraichir votre site Web pour voir le résultat.

![website](/content/images/2018/07/website.png)

## Conclusion

Je suis très content de voir cette fonctionnalité enfin disponible, car elle répond à un besoin qui n’était pas vraiment couvert par l’offre de Microsoft. Effectivement il est encore tôt et le service est toujours en Preview, mais le service est stable. Les outils ne le supportent pas tous encore les nouvelles fonctionnalités, mais seulement temporairement. Bien que vous puissiez  définir votre nom de domaine personnalisé, le protocole HTTPS n’est pas pris en charge pour le moment.

Alors, que faisons-nous avec? Devrions-nous attendre ou pas? Selon les meilleures pratiques quand une fonctionnalité est en Preview, on ne l’utilise pas pour des activités principales de votre business. Si vous cherchez simplement à créer un site web personnel, une petite promo que... Go!

## En vidéo s’il vous plaît

J’ai aussi une vidéo de ce post si vous préférez.

<div class="container">
<iframe  class="youtubevideo" width="560"  src="https://www.youtube.com/embed/28FGwSvvy9I" frameborder="0" allow="autoplay; encrypted-media" allowfullscreen></iframe>
</div>

#### Références

- [Official annoucement](https://azure.microsoft.com/en-us/blog/azure-storage-static-web-hosting-public-preview/)
- [Azure CLI Extension Preview](https://github.com/Azure/azure-cli-extensions/tree/master/src/storage-preview)
- [VSCode Extension](https://github.com/Microsoft/vscode-azurestorage#preview-features)
- [Jekyll](https://jekyllrb.com/)
- [Wyam](https://wyam.io/)



[createstorage]: /images/2018-07-02/createstorage.png
[ConfigureStatic]: /images/2018-07-02/ConfigureStatic.png
[emptywebsite]: /images/2018-07-02/emptywebsite.png
[portalUpload]: /images/2018-07-02/portalUpload.png
[vscodeupload]: /images/2018-07-02/vscodeupload.png
[website]: /images/2018-07-02/website.png













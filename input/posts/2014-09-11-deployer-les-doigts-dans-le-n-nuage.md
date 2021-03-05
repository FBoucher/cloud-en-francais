---
title: Déployer les doigts dans le n ... nuage
permalink: /deployer-les-doigts-dans-le-n-nuage
Published: 2014-09-11
tags: [website, video, deployement, cloud, youtube, dropbox, infonuagique, azure]
---

Mon blogue est dû depuis longtemps pour une mise à jour, mais je remets toujours à plus tard, faute de temps. Il me faudrait une méthode extrêmement simple de pouvoir mettre le site a jour. Ce billet est donc un résumé de mon aventure et surtout je voulais partager cette découverte.

## Contexte
J’ai créé mon blogue Cloudenfrancais.com en sélectionnant un Ghost blogue dans la galerie de site Web disponible sur le portail de Microsoft Azure. Vous pouvez d’ailleurs retrouver toutes les étapes dans mon billet [Comment créer un Ghost blogue sur Azure en 5 minutes](http://www.cloudenfrancais.com/comment-creer-un-blog-ghost-sur-azure-en-5-minutes/). 

## Le but
Faire la mise à jour du Ghost blogue, et instaurer une méthode de déploiement automatique simple: Dropbox.

## Étape 1: Configurer le déploiement automatique
Pour configurer le déploiement, il faut se connecter au portail de gestion Azure. Quoique le nouveau portail soit mon préféré pour manipuler et visualiser l’information concernant les sites web, au moment d’écrire ce billet les fonctionnalités nécessaires pour le déploiement n’étaient pas encore disponibles. Il faut donc se connecter « à l'ancien » portail et sélectionner le site Web.

Une fois le site web sélectionné, vous devez cliquer sur l’option: Configurer le déploiement à partir du contrôle de code source, qui se trouve en bas à droite du tableau de bord.

![Step 1](/content/images/2014/Sep/Step1-1.png)

Dans la liste déroulante, choisissez Dropbox et cliquez sur la flèche. Afin que Microsoft Azure puis faire le déploiement, vous devez lui donner l’autorisation sur un répertoire dans votre compte Dropbox. Vous devez donc avoir un compte Dropbox actif.  

![Step 2](/content/images/2014/Sep/Step2---all-1.png)

##Étape 2: Publier le Site Web
À partir de votre ordinateur, accéder à Dropbox. Si vous avez laissé les paramètres par défaut, le répertoire devrait se retrouver sous Apps/Azure/[NomDuRepertoire]. Vous pouvez maintenant copier le code, images et tout autre fichier dont vous aurez besoin. Une fois la synchronisation avec DroxBox terminé (il y a des petits crochets partout) on peut retourner au portail Azure.

Il est maintenant temps de déployer. Pour ce faire, vous devez cliquer sur Synchronisation. 

![Step 5](/content/images/2014/Sep/Step_5-1.png)

Une fois terminer vous aurez un message vous indiquant que le déploiement est terminé. Vous pouvez consulter le journal pour voir les étapes du déploiement en détail si vous le désirez.

![Step 7](/content/images/2014/Sep/Step_7-1.png)

Votre site est maintenant disponible!

##Conclusion
Déployer avec Dropbox un blogue, un site d'entreprise statique, un site familial, c'est tellement simple! C'est encore mieux que le bon vieux FTP, en cas de pépin, redéploiement de la version précédente d'un simple clique. 

<br/>
<iframe width="560" height="315" src="//www.youtube.com/embed/irMB7bToOS0" frameborder="0" allowfullscreen></iframe>
<br/>

### Références
- [Ghost blogue](http://ghost.org)
- [Microsoft Azure Website](windowsazure.com)


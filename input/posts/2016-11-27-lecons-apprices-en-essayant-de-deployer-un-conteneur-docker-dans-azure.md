---
title: Leçons apprises en essayant de déployer un conteneur Docker dans Azure
permalink: /lecons-apprices-en-essayant-de-deployer-un-conteneur-docker-dans-azure
Published: 2016-11-27
tags: [cloud, azure, post, apps, docker, tips, virtualbox, vmware, connect]
---


Depuis que j'ai vu [Donovan Brown](https://twitter.com/DonovanBrown) faire sa démo pendant le discours d’ouverture (Keynote) du [Connect(); // 2016](https://connectevent.microsoft.com/), je veux vraiment essayer ce clic gauche pour transformer un projet web ordinaire en projet utilisant des conteneurs Docker. Donc, dimanche dernier, je me suis réveillé tôt et armé d'un grand bol de café au lait, j'ai commencé à  préparer ma nouvelle machine virtuelle pour vivre cette expérience. Mais comme vous pouvez l'imaginer, cela n'a pas fonctionné à  la première tentative, voici donc ce que je partagerai avec vous.

![Donovan](/content/images/2016/11/DonovanBrown.png)

Commençons
----------

J'ai bâti une nouvelle machine virtuelle (VM) VirtualBox en utilisant une ISO de Windows 10, puis installe toutes les mises à  jour pour obtenir l'édition anniversaire. Ensuite, en allant sur [visualstudio.com](visualstudio.com) pour obtenir Visual Studio 2017 RC et l'installer avec quelques composants: web, Azure, etc. Il est maintenant temps d'installer Docker pour Windows. Ce qui sera très simple, puis qu'il suffit de télécharger et installer le MSI à  partir de [docker.com](docker.com) et voilà ... ou pas vraiment.
Comme indiqué dans la documentation de Docker, le programme d'installation a remarqué que Hyper-V n'était pas présent dans mon environnement et a suggéré de l'installer et de redémarrer la machine. Jusque-là , tout va très bien, mais quand Docker essaye de démarrer, après le redémarrage, j'ai eu un message d'erreur:

```
Error creating machine: Error in driver during machine creation: This computer doesn't have VT-X/AMD-v enabled. Enabling it in the BIOS is mandatory
```

Toutefois, le réglage est bel et bien défini, comme on peut le voir dans la capture d'écran.

![VTX](/content/images/2016/11/VT-xSetting.png)

Après une courte recherche, j'ai constaté que VirtualBox ne prenait pas en charge la virtualisation imbriquée, du moins pour le moment. Eh bien, essayons une autre plateforme de virtualisation alors!

Première victoire
-----------------

De nouveau, j'ai créé une VM, mais cette fois en utilisant VMware Player, et j'ai répété toutes les étapes précédentes. Après le redémarrage, la petite baleine blanche dans le système n'a pas affiché d'erreur ... Est-ce fonctionnel? Essayons de créer un conteneur Nginx Hello-world pour vérifier. Et la réponse est OUI, Docker fonctionne!

![Success](/content/images/2016/11/2016-11-22_19-00-45.png)

Docker et Azure
---------------

Maintenant, soyons sérieux. Il est temps de faire ce fameux clic gauche sur le projet et d'ajouter support de Docker à  ce dernier. Appuyez ensuite sur F5 pour l'essayer localement.

```
ERROR: for mystuff Cannot create container for service mystuff: C: drive is not shared. Please share it in Docker for Windows Settings 
Encountered errors while bringing up the project..
```

Bon, je suis un peu emballé, et j'ai sauté un peu trop de lecture... Il est écrit noir sur blanc dans la [documentation][doc] que l'on doit partager un disque pour faire fonctionner Docker.

![ShareDrice](/content/images/2016/11/ShareDrice.png)

Une fois le disque partagé, tout alla comme sur des roulettes, et en quelques minutes, j'ai obtenu mon site Web fonctionnant dans un conteneur Docker accessible via localhost:32768. Génial! Maintenant, pour l'obtenir sur Azure, je dois créer un registre où toutes mes images seront enregistrées. Pour ce faire, cliquez tout simplement sur le projet et sélectionnez Publish. La boite de dialogue guidée s'affichera et vous aidera à  créer et déployer votre solution. Un point intéressant à  noter, est qu'avant de cliquer sur le bouton "Create" vous aurez la possibilité d'exporter votre modèle en tant que fichier json, très utile. Mais pour le moment, cliquez sur Create, puis sur Publish.

![DockerPublishing](/content/images/2016/11/DockerPublishing.gif)

Après un court moment, vous devriez avoir une nouvelle fenêtre de navigateur qui apparaitra avec votre application maintenant déployée dans Azure.

![DockerOnline](/content/images/2016/11/DockerOnline.png)

Wow! Quelle expérience formidable! C'est certainement un processus très simple pour débuter avec les conteneurs Docker et Azure. J'apprécie vraiment  l'interface utilisateur de Docker pour Windows et de Kitematic. Bien sûr, toutes les lignes de commande sont toujours disponibles, mais maintenant j'ai une autre option pour le jour où je me sens plus d'humeur à cliquer au lieu de taper.

![DockerTools](/content/images/2016/11/DockerTools.png)

Voilà, maintenant à  vous de jouer... Construisons-le et expédions-le (*build-it and ship-it*), de n'importe quelle plateforme vers le nuage!

<span style="color: #888888;"> (This post is also available in <a href="http://www.frankysnotes.com/2016/11/lessons-learn-while-trying-to-deploy.html">English</a>.) </span>

[doc]: https://blogs.msdn.microsoft.com/webdev/2016/11/16/new-docker-tools-for-visual-studio/
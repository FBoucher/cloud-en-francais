---
title: 5 étapes faciles pour obtenir un ARM template « clean»
permalink: /5-etapes-faciles-pour-obtenir-un-arm-template-clean
Published: 2018-05-04
tags: [arm, clouden5, cloud computing, microsoft azure, azure resource manager, azure cli, meilleures pratiques]
---

Vous avez déjà une solution déployée dans Azure et vous souhaitez la reproduire. Vous savez que les modèle Azure Resource Manager (ARM template) peut vous aider à le faire, mais vous ne savez pas par où commencer. Dans ce post, je vais partager avec vous les meilleures pratiques et comment je les implémente tout en travaillant sur le modèle ARM.

## Comment obtenir votre modèle ARM

Les ARM modèle sont en fait des fichiers JSon et son facilement éditable. Bien évidament, vous pouvez créer votre modèle ARM à partir de zéro. Cependant, il existe de nombreux modèles de démarrage rapide (quickstart) disponibles sur [GitHub](https://github.com/Azure/azure-quickstart-templates). Vous pouvez également utiliser le portail Azure pour générer le modèle pour vous!

Si vous créez une nouvelle solution, rendez-vous sur le portail Azure (portal.azure.com) et commencez à créer votre ressource comme d'habitude. Mais arrêtez juste avant de cliquer sur le bouton *Create*. Au lieu de cela, cliquez sur le lien de son côté nommé **Download template and parameters**. Cela ouvrira une nouvelle blade où vous pourrez télécharger le modèle, les fichiers de paramètres et quelques scripts dans différentes langues pour le déployer.

![ARM d'une nouvelle solution](/content/images/2018/05/Arm_fromNew.png)

Si votre solution est déjà déployée, vous avez toujours un moyen d'obtenir le modèle. Toujours à partir du portail Azure, accédez au groupe de ressources (Ressource Group) de votre solution. Dans le panneau d'options de gauche, cliquez sur **Automation script**.

![ARM d'une solution déjà existante](/content/images/2018/05/ARM_fromLive.png)

## Les 5 étapes

[![](/content/images/2018/05/Les-5-etapes.png)](https://www.youtube.com/watch?v=tG24xbxaTms)

Dans cette vidéo, je « nettoie » un script généré, et j’explique comment j’applique les meilleures pratiques en passant chaque étape. [Meilleures pratiques pour les modèles Azure Resource Manager (ARM)](https://www.youtube.com/watch?v=tG24xbxaTms)

Le code est aussi disponible sur ma page GitHub: https://gist.github.com/FBoucher/adea0acd95f86e5838cf812c010564cf

<div class="container">
<iframe  class="youtubevideo" width="560"  src="https://www.youtube.com/embed/tG24xbxaTms" frameborder="0" allow="autoplay; encrypted-media" allowfullscreen></iframe>
</div>


J'espère que vous trouverez ces conseils vous aiderons. Si vous avez des questions ou d'autres suggestions ou recommandations, n'hésitez pas à les ajouter dans la section des commentaires ou à me contacter!


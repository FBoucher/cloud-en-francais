---
title: "Comment j'ai créé un raccourcisseur d'URL économique facile à déployer et personnalisé" 
Published: 2020/04/09
categories: post
tags: [azure,cloud,post,video,cloud5mins,function, serverless,storage,nosql]
---

# Comment j'ai créé un raccourcisseur d'URL économique facile à déployer et personnalisé

Je ne sais pas pour vous mais je partage très souvent des liens. Et la plupart du temps, il s'agit de vidéos, il doit donc être court et facile à retenir. Quelque chose comme https://c5m.ca/project est meilleur qu'une chaîne aléatoire (aka. GUID). Et c'est ainsi que j'ai commencé un projet de construction d'un raccourcisseur d'URL. Je voulais être économique, facile à déployer et personnalisable. Dans cet article, je partagerai comment je le construis, comment vous pouvez l'utiliser et comment vous pouvez m'aider!

![Azure Url Shortener](https://github.com/FBoucher/AzUrlShortener/raw/master/medias/UrlShortener_600.png)

Construis avec la communauté
-----------------------------

Cet outil a été créé lors de sessions de coda en direct sur Twitch (toutes les vidéos sont disponibles dans mes [archives YouTube](https://www.youtube.com/watch?v=ovMUd0eX2Qw&list=PL4NfFPd0l1UZxW7R5yzzP4oTEOKuyC_j-)). Il est composé de deux parties: un backend serverless avec les Azure Function et le stockage Azure, et un frontend... de votre choix!.

Le backend est composé de quelques fonctions Azure qui agissent comme API HTTP à la demande. Elle consomme des ressources que lorsqu'elles sont appelées. Ils sont en .Net Core, C# pour être précis. Lors de la publication de cet article, il existe quatre fonctions:

- UrlShortener: pour créer une URL courte.
- UrlRedirect: est appelé lorsque le lien court est utilisé. Un proxy de fonction Azure transfère tous les appels à la racine.
- UrlClickStats: retourne la statistique pour une URL spécifique.
- UrlList: renvoie la liste de toutes les URL créées.

Toutes les informations telles que l'URL longue, l'URL courte, le nombre de clics sont enregistrées dans une table de stockage Azure.

Et c'est tout. Super léger, très rentable. Si vous êtes curieux de connaître le prix, vous trouverez des références dans les notes de bas de page [<sup>1</sup>](#1)

Le frontend peut être tout ce qui peut faire des requêtes HTTP. En ce moment dans le projet, j'explique comment utiliser un outil appelé Postman, il existe également une interface très simple que vous pouvez facilement déployer.

![Simple Admin Interface][simpleInterface]

Cette interface simple est bien sûr protégée et vous donne la possibilité de voir toutes les URL et d'en créer de nouvelles.

Comment VOUS pouvez l'utiliser
------------------

Tout le code est disponible dans GitHub, et il est déployable avec un bouton en un clic!

[![Deploy to Azure](https://aka.ms/deploytoazurebutton)](https://portal.azure.com/?WT.mc_id=urlshortener-github-frbouche#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FFBoucher%2FAzUrlShortener%2Fmaster%2Fdeployment%2FazureDeploy.json)

Cela déploiera le backend dans votre abonnement Azure en quelques minutes. Si vous ne possédez pas déjà un abonnement Azure, vous pouvez [créer votre compte Azure gratuit aujourd'hui](https://azure.microsoft.com/en-us/free?WT.mc_id=cloudenfr-blog-frbouche).

Ensuite, vous voudrez probablement une interface pour créer vos précieuses URL. Une fois de plus dans le dépôt GitHub, il y a une [Liste](https://github.com/FBoucher/AzUrlShortener/blob/master/src/adminTools/README.md#list-of-available-admin- interfaces) des interfaces d'administration disponibles et prêt à l'emploi. Le **site Web Admin Blazor** est actuellement le plus convivial et peut également être déployé en un seul clic.

Comment vous pouvez aider et participer
--------------------------------

À l'heure actuelle, il n'y a vraiment qu'une seule interface (et quelques instructions sur la façon d'utiliser Postman pour effectuer les appels HTTP). Mais AzUrlShortener est un projet open source, ce qui signifie que vous pouvez participer. Voici quelques suggestions:

- Construisez une nouvelle interface (dans la langue de votre choix)
- Améliorer les interfaces actuelles avec
   - logos
   - dessins
   - Meilleure interface utilisateur 🙂
- Enregistrer des bogues dans GitHub
- Faire une demande de fonctionnalité
- Aide à la documentation / traduction

La suite
--------

Venez certainement voir le dépôt GitHub https://github.com/FBoucher/AzUrlShortener, cliquez sur ces boutons de déploiement. De mon côté, je continuerai à ajouter plus de fonctionnalités et à l'améliorer. On se voit là-bas!


En vidéo
--------

<iframe width="560" height="315" src="https://www.youtube.com/embed/M7KzLrH0nhk" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>


#### Références

- <a class="anchor" id="1"><sup>1 </sup></a>[Azure Function Pricing](https://azure.microsoft.com/en-us/pricing/details/functions/?WT.mc_id=cloudenfr-blog-frbouche)
- <a class="anchor" id="1"><sup>1 </sup></a>[Azure Table Storage pricing](https://azure.microsoft.com/en-us/pricing/details/storage/tables/?WT.mc_id=cloudenfr-blog-frbouche)


[simpleInterface]: /content/images/2020/04/simpleInterface.png "Interface Simple"


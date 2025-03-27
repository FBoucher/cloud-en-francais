---
title: "Azure Developer CLI (AZD) : L'outil méconnu qui simplifie la vie des développeurs"	 
Published: 2025-03-27
featured-image: ../content/images/2025/03/azd-video-thumbnail.png
Image: ../content/images/2025/03/azd-video-thumbnail.png
categories: post-fr
tags: [azure,azd,bicep,iac,devops,cli,github,post,oss]
---

# Azure Developer CLI (AZD) : Un super outil pour les développeurs

L'**Azure Developer CLI** (AZD) reste le secret le mieux gardé de l'écosystème Azure. Pendant que la plupart des développeurs s'épuisent avec des commandes complexes et des déploiements laborieux, AZD transforme silencieusement l'expérience de ceux qui l'ont découvert. Laissez-moi vous révéler cet allié précieux qui devrait déjà faire partie de votre arsenal quotidien. 

## Qu'est-ce que AZD?

AZD est un outil extrêmement puissant qui nous aide dans notre cycle de développement, particulièrement pour automatiser les tâches répétitives. Il est conçu spécifiquement pour les développeurs, avec une philosophie simple : offrir des commandes courtes et intuitives.

Il est important de ne pas confondre AZD avec Azure CLI (az). Azure CLI est certes très utile pour la gestion des ressources Azure, mais il nécessite souvent des commandes longues avec de nombreux paramètres. Pour nous, développeurs, qui sommes davantage intéressés par le code et son déploiement sur Azure, mémoriser toutes ces commandes peut devenir fastidieux.

C'est là qu'AZD entre en jeu avec ses commandes simples et minimalistes.

## Une solution agnostique

Un point important à souligner : bien que je sois personnellement développeur .NET, Azure Developer CLI est totalement agnostique en termes de langage de programmation. Qu'importe votre stack technologique, AZD vous supportera sans aucun problème.

## Commandes simples et efficaces

En consultant la documentation d'AZD, vous constaterez rapidement la simplicité de ses commandes:
- `azd up`
- `azd down`
- `azd deploy`
- ...

Pas besoin de cinquante paramètres - juste des commandes simples, et la magie opère!

## Démonstration: Initialisation d'un projet avec AZD

Pour débuter avec AZD, il suffit d'initialiser notre environnement dans le répertoire de projet avec la commande `azd init`, l'outil analyse mon code et suggère automatiquement les services Azure appropriés.

AZD me demande ensuite de nommer mon environnement. C'est très pratique ,car en tant que développeurs, nous travaillons souvent avec différents environnements : personnel, d'équipe, QA, pré-production, etc.

## Structure générée par AZD

Une fois l'initialisation terminée, AZD crée plusieurs éléments:

1. Un répertoire `.azure` qui stocke les informations relatives à l'abonnement, aux ressources créées, etc. Ces données permettent à AZD de comparer l'état actuel de votre infrastructure avec ce qui existe déjà dans Azure lors des déploiements ultérieurs.

2. Un fichier `.gitignore` qui exclut le répertoire `.azure` du contrôle de version (ce qui est une bonne pratique).

3. Un répertoire `infra` contenant des fichiers Bicep. Ces fichiers représentent l'infrastructure sous forme de code (IaC), garantissant que les ressources déployées auront toujours les mêmes propriétés.

## L'adoption des meilleures pratiques

Un aspect impressionnant est que les fichiers Bicep générés utilisent des modules "Azure Verified Modules" (AVM). Ces modules sont vérifiés et approuvés par Microsoft, garantissant qu'ils suivent les meilleures pratiques. C'est un avantage considérable pour maintenir une infrastructure de qualité sans effort supplémentaire.

## Déploiement et intégration continue

Une fois l'initialisation terminée, AZD nous indique les prochaines étapes. Pour déployer notre solution, il suffit d'utiliser `azd up`, qui s'occupe à la fois du provisionnement des ressources et du déploiement de notre application.

AZD va encore plus loin en proposant de nous aider à créer des pipelines CI/CD. Il prend en charge GitHub Actions et Azure DevOps, peut créer les fichiers de workflow nécessaires et configurer les secrets requis. Tout ce qu'il nous reste à faire est de pousser notre code et le pipeline s'occupera du reste.

## Une vidéo pour illustrer tout ça

<iframe width="560" height="315" src="https://www.youtube.com/embed/s8DiGC7lUbQ" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>

## Conclusion

Azure Developer CLI est vraiment un outil extraordinaire qui simplifie considérablement le processus de développement et de déploiement sur Azure. Je vous encourage vivement à [l'installer et à l'essayer](https://c5m.ca/install-azd) par vous-même.

Visité [https://c5m.ca/azd](https://c5m.ca/azd) pour tout savoir à propos d'AZD et découvrir comment il peut transformer votre façon de travailler avec Azure.
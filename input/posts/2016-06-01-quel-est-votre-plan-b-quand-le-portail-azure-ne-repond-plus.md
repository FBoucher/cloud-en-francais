---
title: Quel est votre plan B quand le portail Azure ne répond plus
permalink: /quel-est-votre-plan-b-quand-le-portail-azure-ne-repond-plus
Published: 2016-06-01
tags: [cloud, powershell, azure, arm, portal, trafficmanager]
---

Vous êtes sur le point de tester la dernière version de votre solution. Vous devez seulement modifier une configuration dans le portail Micosoft Azure, et vous y êtes. Pour ce faire, vous ouvrez une session dans le portail à http://portal.azure.com et de naviguer jusqu’à votre composant et... Erreur! Ce que vous voyez est un petit nuage triste.

![onesadcloud](/content/images/2016/06/OneSadCloud.png)


Le problème
-----------

C’est exactement ce qui est arrivé récemment à l’équipe, avec laquelle je travaille. Ils avaient besoin de changer le point de terminaison (EndPoint) du gestionnaire de trafic (Traffic Manager) pour faire un test de charge. Malheureusement, la grille qui contient les points de terminaison n’était pas disponible et retournait un message d’erreur. De premier abord cela ressemble à une impasse!

![manysadclouds](/content/images/2016/06/ManySadClouds.png)

Mais est-ce vraiment une impasse? Bien sûr que non. Voici ce que vous pouvez faire.

<!--more-->

La solution
------------

Rappelez-vous, Microsoft partage le même API utilisé par le portail Azure. C’est la beauté du portail Azure, vous pouvez bénéficié de sa convivialité pour faire ce dont vous avez besoin, ou vous pouvez y accéder via les nombreux SDKs qui sont disponibles aujourd’hui: .Net, Java, Node.js, Php, Python, Ruby et plus encore! Vous avez également des outils de ligne de commande qui permettent de gérer vos services Azure et applications à l’aide de scripts.

Pour en savoir plus sur tous les SDKs disponibles ou les outils en ligne de commande voir la page [Azure SDKs][AzureSDKs] de documentation en ligne.

![Remember](/content/images/2016/06/Remember2-fr.png)

Comme nous étions dans un environnement Windows, et nous avions besoin de modifier un point de terminaison d’un gestionnaire de trafic. Voici ce que nous avons fait, cette fois-ci, en utilisant Azure PowerShell Cmdlets:

```bash

    # Ouvrir une session avec votre compte
    Login-AzureRmAccount
    
    # Déterminer le contexte de la session. Utiliser Get-AzureRmSubscription pour visualiser tous les abonnements. 
    Set-AzureRmContext -SubscriptionName "MySubscriptionName"
    
    # Lister tous les profils du gestionnaire de trafic 
    Azure Get-AzureTrafficManagerProfile 

    # Éditer l'endpoint en le sauvegardant dans une variable, puis le réassigner 
    $endpoint = Get-AzureRmTrafficManagerEndpoint -Name myendpoint -ProfileName myprofile -ResourceGroupName "MyResourceGroupName" -Type ExternalEndpoints
    $endpoint.Weight = 50
    Set-AzureRmTrafficManagerEndpoint -TrafficManagerEndpoint $endpoint

```   

    
Dans le cas présent, nous avons utilisé les cmdlets en mode gestionnaire de ressources Azure (ARM), mais toutes les commandes sont également disponibles dans le mode de service. Pour en savoir plus sur la façon de déployer avec ARM, vous pouvez lire mon [récent billet][armpost]. Pour voir toutes les commandes ARM disponibles pour configurer votre solution, voir la [documentation en ligne](https://azure.microsoft.com/en-us/documentation/articles/powershell-azure-resource-manager/).

     
    
###### Références:

- [Azure SDKs][AzureSDKs]
- [Azure Resource Manager support for Azure Traffic Manager][ARMtrafficmanager]


[AzureSDKs]: https://azure.microsoft.com/en-us/downloads/
[ARMtrafficmanager]: https://azure.microsoft.com/en-us/documentation/articles/traffic-manager-powershell-arm/
[armpost]: http://www.frankysnotes.com/2016/05/azure-resource-manager-arm-for-beginners.html



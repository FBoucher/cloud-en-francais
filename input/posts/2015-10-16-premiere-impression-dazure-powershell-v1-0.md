---
title: Première impression d’Azure PowerShell V1.0
permalink: /premiere-impression-dazure-powershell-v1-0
Published: 2015-10-16
tags: [powershell, resource manager, azurerm, azure]
---

Cette semaine une nouvelle version d’Azure PowerShell a été rendue publique. Cette version sépare les fonctionnalités de *Resource Manager* et *Azure Service Management*. Elle est aussi disponible via WebAPI ce qui permettra une distribution plus fluide.

## Installation

Pour installer Azure PowerShell v1.0, rien de plus simple. Comme mentionné dans le [post de David Justice][MSBlog], il suffit d'exécuter quelques commandes PowerShell.
```language-powershell
    # Install the Azure Resource Manager modules from PowerShell Gallery
    Install-Module AzureRM
    Install-AzureRM
    
    # Install the Azure Service Management module from PowerShell Gallery
    Install-Module Azure
```
Le script est aussi disponible sur [GitHub][GitHubInstallScript].

![Erreur_install-AzureRM](/content/images/2015/10/Erreur_install-AzureRM.png)

Évidament, il faut d'abord d'installer la version précédente pour ne pas avoir l'erreur illustrée ci-dessus. 


## Le module *Azure Service Management*

On commence par se connecter avec la commande `Add-AzureAccount` qui affichera un l'écran de connexion en popup. Ensuite, toutes les fonctionnalités que nous connaissons sont présentes et fonctionnent comme un charme. 

`Get-AzureSubscription` affiche toutes les subscriptions en un clin d'oeil.

`Get-AzureSubscription -SubscriptionName "FrankB Playground" | Select-AzureSubscription` Permet de sélectionner la subscription active qui sera prise en compte pour les prochaines commandes.


## Le module *Resource Manager*

Première chose qu'on se remarque est la façon de se connecter est différente.  En effet, il faut utiliser `Login-AzureRMAccount`. Ayant plusieurs subscriptions reliés à mon compte, une fenêtre blanche s’ouvre et se referme plusieurs fois. Je n'ai qu'à saisir mon nom d'usager et mon mot de passe qu'une fois donc ce n'est pas vraiment un problème et l'équipe d'Azure PowerShell travaille de concert avec l'équipe responsable d’Active Directory pour régler ce léger désagrément.


Encore une fois, `Get-AzureRMSubscription` permettra d'avoir la liste des abonnements. 

![](/content/images/2015/10/Get-AzureRMSubscription.png)

`Get-Module -ListAvailable AzureRM*` Affiche la liste de tous les modules reliés à *Resource Manager*. On pourra d'ailleurs apprécier le nom raccourci `AzureRM` au lieu de *Azure Resource Manager*.

Il n'est pas possible pour le moment de sélectionner une subscription avec le nom. En effet `Select-AzureRmSubscription -SubscriptionName "FrankB Playground"` retourne une erreur indiquant qu'on doit utiliser `SubscriptionId`. Une astuce simple si vous désirez tout de même utiliser un nom plus facile à retenir qu'un GUID est d'enchainer les commandes de cette façon:

```language-powershell	
	Get-AzureRmSubscription –SubscriptionName “FrankB Playground” | Select-AzureRmSubscription
```

## Le verdict, on a hâte ou on espère?

Bien qu'il y ait quelques légers problèmes, ce qui est tout à fait acceptable pour une version *Preview*, j'ai beaucoup apprécié la nouvelle mouture des cmdlets. Le fait de scinder en deux la librairie simplifie les choses. 

Donc à moins que ce soit pour un environnement en production, je vous suggère d'installer Azure PowerShell v1.0 sans plus tarder afin de profiter de toutes ses nouvelles fonctionnalités.

##### Références

- [Microsoft Azure Blog - Azure PowerShell 1.0 Preview][MSBlog]




[MSBlog]: https://azure.microsoft.com/en-us/blog/azps-1-0-pre/ "Microsoft Azure Blog - Azure PowerShell 1.0 Preview"
[GitHubInstallScript]: https://gist.github.com/devigned/f2a8d52f3eb38a5bf4c0#file-install-azps-1-0-pre-ps1 "Script d'installation"
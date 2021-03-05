---
title: Azure Resource Manager (ARM) pour débutants
permalink: /azure-resource-manager-arm-pour-debutants
Published: 2016-05-18
tags: [cloud, azure, post, arm, resourcemanager, vm, visualstudio]
---


Vous savez l’image où l’on voit des gens tirant une charrette avec des roues carrées, un homme sur le côté veut leur montrer une roue ronde, mais le groupe répond qu’ils sont trop occupés à prendre écouté… Eh bien, ce scénario s’applique dans mon cas avec Azure Resource Manager (ARM). Je savais que c’était bon, mais s’a semblait trop compliqué, donc j’attendais. Le weekend dernier, je décidai qu’il était temps je devais l’apprendre! Après quelques minutes seulement, vous ne pouvez pas imaginer ma déception! C’est si simple, si puissant et aussi si vite. 
Dans ce billet j’explique une méthode simple pour déployer un schéma d’ARM, et comment il fonctionne.

![](/content/images/2016/05/squareweels-FR.png)

<!--more-->

## 5 étapes faciles pour déployer notre premier modèle ARM

Pour commencer le plus facilement possible, j’ai décidé d’utiliser Visual Studio. Pour cet exemple, nous allons créer une simple machine virtuelle Windows (VM). Seulement cinq étapes sont nécessaires pour ce faire :

### Étape #1 - Créer un projet Azure ressources

   À partir de Visual Studio, créer un nouveau projet de type *Azure Resource Group*. Assurez-vous d’avoir déjà d’installé sur votre ordinateur la dernière version de [Azure SDK] [Azure SDK] et les mises à jour de Visual Studio.
  
![](/content/images/2016/05/step-1-Create_arm_project.png)
   
### Étape #2 - Choisir son schéma ARM

   Voici le moment où nous choisissons ce que nous voulons dans notre schéma. De nombreuses options sont disponibles dans Visual Studio et beaucoup d’autres peuvent être trouvés sur Github à: [Azure ARM Git Template][AzureArmGitTemplate]. Pour ce tutoriel, nous sélectionnerons la machine virtuelle Windows , puis cliquez sur le bouton *Ok*.
   
![](/content/images/2016/05/step-2-Select_Tempplate.png)

### Étape #3 - Déployer le nouveau schéma

   Visual Studio va maintenant générer plusieurs fichiers, nous y reviendrons plus tard, pour le moment contentons-nous déployer la solution. Faites un clic droit sur le projet et sélectionnez *Deploy*.
     
![](/content/images/2016/05/step-3-Start_deploy.png)

### Étape #4 - Configurer le déploiement

   Notre premier déploiement presque prêt, il nous reste seulement à préciser quelques détails comme l’abonnement (subscription) et le groupe de ressources. Une fois fait, cliquez sur *Deploy*, il restera qu’une dernière information: le *adminPassword*.

![](/content/images/2016/05/step-4-Config_deploy.png)

### Étape #5 (la plus facile) - Profitez-en

Voilà! Après quelques minutes, la machine virtuelle sera créée, et nous pourrons nous y connecter à distance.

## Expliquons la magie

Lorsque le projet a été créé, trois dossiers ont été remplis: Script, Template et Tools. Le dernier est un peu évident, il contient AzCopy, un outil pour copier des fichiers. Si vous n’êtes pas accoutumé avec AzCopy, vous pouvez en savoir plus [dans le billet][AzCopyPost] je l’ai écrit récemment.

Ouvrez le `Déployer-AzureResourceGroup.ps1` contenu dans le dossier *Scripts*. C’est ce dernier qui fera le plus gros du travail pour lors du déploiement de notre rescourceGroup. En regardant d’un peu plus près, vous remarquerez que des paramètres sont déclarés au début du fichier. Deux d’entre eux devraient attirer notre attention.

        [string] $TemplateFile = '..\Templates\WindowsVirtualMachine.json',
        
        [string] $TemplateParametersFile = '..\Templates\WindowsVirtualMachine.parameters.json',

`TemplateFile` est le chemin d’accès (path) de notre schéma (celui que nous avons sélectionné précédemment), et le second `TemplateParametersFile` est contient toutes les valeurs des paramètres utilisés pour remplir notre schéma. Cela sera particulièrement utile pour déployer le même schéma dans des environnements différents. En fait, cela est un avantage majeur. Vous pouvez déployer exactement le même schéma dans votre environnement de développement et de production seulement en ayant deux fichiers `parameters.json`.

Jetons un coup d’œil au schéma, dans le cas présent: `WindowsVirtualMachine.json`. C’est un fichier `json`, il est donc convivial, mais il peut être un peu effrayant au premier abord. Dans l’image juste ci-dessous, j’ai regroupé les collections pour pouvoir mettre en valeur la visibilité des trois éléments principaux: les paramètres, les variables et les ressources.

![jsonTemplate](/content/images/2016/05/jsonTemplate.png)

Nous avons déjà mentionné les paramètres, allons donc directement aux variables. Cette section contient une liste de paires clé-valeur telque: `imagePublisher`, `vmSize`,  `virtualNetworkName`, `diagnosticsStorageAccountName`, etc. Ceux-ci peuvent avoir une valeur fixe ou dynamique en utilisant d’autres variables ou des paramètres. Voici quelques exemples:

        "vmSize": "Standard_A2"
        
        "vhdStorageName": "[concat('vhdstorage', uniqueString(resourceGroup().id))]"
        
        "virtualNetworkName": "[parameters('virtualNetworkName')]"

Dernière section, mais non la moindre: les ressources. Ceci est où tout est mis en place pour, mais la solution que vous déploiera. Les ressources sont définies en spécifiant leur type, le nom et les propriétés. Vous pouvez attribuer une valeur à partir d’une chaîne statique, la valeur du paramètre ou une valeur variable.

## Maintenant que nous savons comment cela fonctionne, pourquoi devrions-nous l’utiliser

Expliquer tous les avantages qu’utiliser le schéma ARM procure est un billet en soi, et dépasse largement la portée de celui-ci. Cependant, voici malgré tout quelques bonnes raisons:
 
- Un schéma est un fichier léger et facile à maintenir dans un entrepôt de donnée (repository).
- Il est très simple d’avoir l’exact même schéma déployé dans plusieurs environnements.
- Les schémas ARM sont vraiment les rapides à déployer.
- Facile à modifier / personnaliser / développer.
- Facile à supprimer.

## En vidéo SVP!

Si vous préféré j’ai une version en vidéo de ce post. 

<div class="container">
<iframe  class="youtubevideo" width="560"  src="https://www.youtube.com/embed/enpCtNd3GPI" frameborder="0" allow="autoplay; encrypted-media" allowfullscreen></iframe>
</div>

## La suite

Ce poste est volontaire très simple pour en faciliter la compréhension. C’est à votre tour maintenant, il en tient qu’à vous déployer un schéma modeste ou quelque chose de plus complexe. De nombreux différents schémas peuvent être trouvés à divers endroits: Visual Studio [Azure ARM Git Template][AzureArmGitTemplate] et [Azure Templates Quickstart][AzureQuickstartTemplates]. De fabuleux outils existent aussi pour vous aider à visualiser ou modifier ces schémas: [Azure Resource Explorer][AzureResourceExplorer] et [Download Azure Resource Manager Tools for VS Code][VSCodeARMExt]


#### Ressources:

- [Azure SDK][AzureSDK]
- [Azure ARM Git Template][AzureArmGitTemplate]
- [Azure Quickstart Templates][AzureQuickstartTemplates]
- [Download Azure Resource Manager Tools for VS Code][VSCodeARMExt]
- [Azure Resource Explorer][AzureResourceExplorer]

[AzureSDK]: http://www.visualstudio.com/en-us/features/azure-tools-vs.aspx
[AzureArmGitTemplate]: http://github.com/Azure/azure-quickstart-templates
[AzureQuickstartTemplates]: http://azure.microsoft.com/en-us/documentation/templates/
[VSCodeARMExt]: http://alexandrebrisebois.wordpress.com/2016/02/29/download-azure-resource-manager-tools-for-vs-code/
[AzureResourceExplorer]: http://resources.azure.com/
[AzCopyPost]: http://www.frankysnotes.com/2016/04/how-to-transfert-data-between-azure.html



---
title: "4 façons de déployer automatiquement vos Azure Function à depuis un dépôt Git" 
Published: 2019-07-18
categories: post
tags: [azure,cloud,post,serverless, continuous integration, continuous deployment,CI/CD, azure function,automation,git,github,devops,pipeline]
---

# Quatre façons de déployer automatiquement vos Azure Function à depuis un dépôt Git

C'est tellement agréable de pouvoir ajouter des composants *serverless* dans nos solutions pour les améliorer en un clin d’œil. Mais comment les gérer? Dans cet article, j'expliquerai comment créer un modèle ARM (Azure Resource Manager) pour déployer une Azure Function et montrer comment j'ai utilisé cette méthode pour déployer un projet open source sur lequel j'ai travaillé ces derniers temps.


## Partie 1 - Le modèle ARM

Un modèle ARM est un fichier JSON qui décrit notre architecture. Pour pouvoir déployer une Azure Function, il faudra compter au minimum trois ressources: une fonction App, un plan de service et un compte de stockage.

![GenericSimple][GenericSimple]

La Function App est bien entendu notre fonction. Le plan de service peut être défini comme dynamique ou contenir les informations concernant le type de ressource qui sera utilisée par ladite fonction. Le compte de stockage est où sera conservé le code.

![GenericDetailed][GenericDetailed]

Dans l'image précédente, vous pouvez voir plus en détail comment ces composants interagissent les uns avec les autres. Dans la fonction, nous aurons une liste de propriétés. L'une de ces propriétés sera le **Runtime**. Par exemple, pour AZUnzipEverything, le *Runtime* sera *dotnet*. 

Une deuxième de c'est propriété sera la chaîne de connexion (connectionString) à notre compte de stockage qui fait également partie de notre modèle ARM. Comme cette ressource n’existe pas encore, nous devrons utiliser le code dynamique.

Le noeud Function contiendra une sous-ressource de type `storageAccount`. Cette dernière permettra de spécifier où se trouve le code afin qu’il ne puisse pas être cloné dans Azure.

### Construire ARM pour une fonction simple

Voyons tout d'abord le modèle d'une Azure Function simple, qui ne nécessite aucune dépendance, et nous l'examinerons par la suite.

> Vous pouvez utiliser n'importe quel éditeur de texte pour modifier votre modèle ARM. Toutefois, le l'ensemble [VSCode](https://code.visualstudio.com/?WT.mc_id=cloudenfrancais-blog-frbouche) avec les extensions [Azure Resource Manager Tools](https://marketplace.visualstudio.com/items?itemName=msazurermtools.azurerm-vscode-tools&WT.mc_id=cloudenfrancais-blog-frbouche) et [Azure Resource Manager Snippets](https://marketplace.visualstudio.com/items?itemName=samcogan.arm-snippets&WT.mc_id=cloudenfrancais-blog-frbouche) est particulièrement efficace.


```json
{
    "$schema": "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
    "contentVersion": "1.0.0.0",
    "parameters": {},
    "variables": {},
    "resources": [
        {
            "type": "Microsoft.Storage/storageAccounts",
            "apiVersion": "2018-07-01",
            "name": "storageFunc",
            "location": "[resourceGroup().location]",
            "tags": {
                "displayName": "storageFunc"
            },
            "sku": {
                "name": "Standard_LRS"
            },
            "kind": "StorageV2"
        },
        {
            "type": "Microsoft.Web/serverfarms",
            "apiVersion": "2018-02-01",
            "name": "servicePlan",
            "location": "[resourceGroup().location]",
            "sku": {
                "name": "Y1",
                "tier": "Dynamic"
            },
            "properties": {
                "name": "servicePlan",
                "computeMode": "Dynamic"
            },
            "tags": {
                "displayName": "servicePlan"
            }
        },
         {
              "apiVersion": "2015-08-01",
              "type": "Microsoft.Web/sites",
              "name": "functionApp",
              "location": "[resourceGroup().location]",
              "kind": "functionapp",
              "dependsOn": [
                "[resourceId('Microsoft.Web/serverfarms', 'servicePlan')]",
                "[resourceId('Microsoft.Storage/storageAccounts', 'storageFunc')]"
              ],
              "properties": {
                "serverFarmId": "[resourceId('Microsoft.Web/serverfarms', 'servicePlan')]",
                "siteConfig": {
                  "appSettings": [
                    {
                      "name": "AzureWebJobsDashboard",
                      "value": "[concat('DefaultEndpointsProtocol=https;AccountName=', 'storageFunc', ';AccountKey=', listKeys('storageFunc','2015-05-01-preview').key1)]"
                    },
                    {
                      "name": "AzureWebJobsStorage",
                      "value": "[concat('DefaultEndpointsProtocol=https;AccountName=', 'storageFunc', ';AccountKey=', listKeys('storageFunc','2015-05-01-preview').key1)]"
                    },
                    {
                      "name": "WEBSITE_CONTENTAZUREFILECONNECTIONSTRING",
                      "value": "[concat('DefaultEndpointsProtocol=https;AccountName=', 'storageFunc', ';AccountKey=', listKeys('storageFunc','2015-05-01-preview').key1)]"
                    },
                    {
                      "name": "WEBSITE_CONTENTSHARE",
                      "value": "storageFunc"
                    },
                    {
                      "name": "FUNCTIONS_EXTENSION_VERSION",
                      "value": "~2"
                    },
                    {
                      "name": "FUNCTIONS_WORKER_RUNTIME",
                      "value": "dotnet"
                    }
                  ]
                }
              },
              "resources": [
                  {
                      "apiVersion": "2015-08-01",
                      "name": "web",
                      "type": "sourcecontrols",
                      "dependsOn": [
                        "[resourceId('Microsoft.Web/sites/', 'functionApp')]"
                      ],
                      "properties": {
                          "RepoUrl": "https://github.com/FBoucher/AzUnzipEverything.git",
                          "branch": "master",
                          "publishRunbook": true,
                          "IsManualIntegration": true
                      }
                 }
              ]
            }
        
    ],
    "outputs": {}
}
```

## Le compte de stockage

La première ressource contenu dans le modèle est un compte de stockage, tout ce qui a de plus ordinaire.

## Le plan de service

Le plan de service est la deuxième ressource de la liste. Pour pouvoir utiliser le SKU **Dynamic**, il vous faudra utiliser **apiVersion** égale ou plus récent que "2018-02-01". Bien sûr, vous pouvez utiliser d'autre SKU si vous préférez.

```json
    "sku": {
        "name": "Y1",
        "tier": "Dynamic"
    }
```

## La Function App

Enfin la dernière ressource, toutes les pièces pourront maintenant s'emboiter. Noter que les l'ordre dans lesquelles les ressources sont saisies ne sont pas prises en compte par Azure lors du déploiement (c'est uniquement pour nous ;) ). Pour que Azure en prenne compte, il nous faut ajouter des dépendances. Donc si on désire que la Function soit créée en dernier on doit ajouter une dépendance vers le plan et le stockage.

```json
"dependsOn": [
    "[resourceId('Microsoft.Web/serverfarms', 'servicePlan')]",
    "[resourceId('Microsoft.Storage/storageAccounts', 'storageFunc')]"
]
```
De cette façon, la fonction Azure sera créée une fois que le plan de service et le compte de stockage sont disponibles. Ensuite, dans les **propriétés**, nous pourrons créer la ConnectionString vers le stockage à l'aide d'une référence.

```json
{
    "name": "AzureWebJobsDashboard",
    "value": "[concat('DefaultEndpointsProtocol=https;AccountName=', 'storageFunc', ';AccountKey=', listKeys('storageFunc','2015-05-01-preview').key1)]"
}
```
    
La dernière pièce du puzzle est la sous-ressource **sourcecontrol** à l'intérieur de FunctionApp. Cela définira où Azure doit cloner le code et depuis quelle branche.

```json
"resources": [
    {
        "apiVersion": "2015-08-01",
        "name": "web",
        "type": "sourcecontrols",
        "dependsOn": [
        "[resourceId('Microsoft.Web/sites/', 'functionApp')]"
        ],
        "properties": {
            "RepoUrl": "https://github.com/FBoucher/AzUnzipEverything.git",
            "branch": "master",
            "publishRunbook": true,
            "IsManualIntegration": true
        }
    }
]
```
Pour que le déploiement soit complètement automatique, les propriétés `publishRunbook` et `IsManualIntegration` doivent être égale à `true`. Sinon, vous devrez effectuer une synchronisation entre votre Git (dans ce cas sur GitHub) et le Git dans Azure.

> Il existe une excellente documentation qui explique de nombreux scénarios pour [Automatiser le déploiement de ressources pour votre application de fonction dans Azure Functions](https://docs.microsoft.com/fr-fr/azure/azure-functions/functions-infrastructure-as-code?WT.mc_id=cloudenfrancais-blog-frbouche)

## Azure Unzip Everything

Pour le projet [AzUnzipEverything, disponible sur GitHub](https://github.com/FBoucher/AzUnzipEverything), j'avais besoin d'un stockage supplémentaire avec des conteneurs (dossiers) prédéfinis.

![AzUnzipEverything][AzUnzipEverything]

Bien que tout le code source de la fonction et du modèle ARM soit disponible sur GitHub, permettez-moi de souligner la façon dont les conteneurs sont définis à partir d'un modèle ARM.

```json
"resources": [
    {
        "type": "blobServices/containers",
        "apiVersion": "2018-07-01",
        "name": "[concat('default/', 'input-files')]",
        "dependsOn": [
            "storageFiles"
        ],
        "properties": {
            "publicAccess": "Blob"
        }
    }
]
```
Exactement comme avec **sourcecontrol**, nous devrons ajouter une liste de sous-ressources à notre compte de stockage. Le nom du conteneur DOIT commencer par `'default /'`. Donc dans lèxemple précédant, le nom visible du conteneur sera **input-files**.

## Partie 2 - quatre options de déploiement

Maintenant que nous avons un modèle qui décrit notre solution, il ne nous reste plus qu'à le déployer. Il y a plusieurs façons de le faire, pour différente situation, en voici quatre.

### Déployer à partir du portail Azure.

Accédez au portail Azure (https://azure.portal.com) à partir de votre navigateur favori et recherchez "deploy a custom template" directement dans la barre de recherche située en haut de l'écran (au centre). Ou visitez le site https://portal.azure.com/#create/Microsoft.Template. Dans la page *Déploiement personnalisé*, cliquez sur le lien **Build your own template in the editor**. Depuis cete page, vous pouvez copier-coller ou télécharger votre modèle ARM. Vous devrez l'enregistrer pour voir le réel formulaire de déploiement.

![deployPortal][deployPortal]

### Déployer avec un script

Que ce soit avec  PowerShell ou Azure CLI, vous pouvez facilement déployer votre modèle avec seulement deux commandes.

Avec Azure CLI

    # create resource group
    az group create -n AzUnzipEverything -l eastus

    # deploy it
    az group deployment create -n cloud5mins -g AzUnzipEverything --template-file "deployment\deployAzure.json" --parameters "deployment\deployAzure.parameters.json"  

Avec PowerShell

    # create resource group
    New-AzResourceGroup -Name AzUnzipEverything -Location eastus

    # deploy it
    New-AzResourceGroupDeployment -ResourceGroupName  AzUnzipEverything -TemplateFile deployment\deployAzure.json


### Le bouton "Deploy to Azure" 

Le bouton "Deploy to Azure" est l'un des meilleurs moyens d'aider les utilisateurs à déployer votre solution dans leur abonnement Azure facilement.

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FFBoucher%2FAzUnzipEverything%2Fmaster%2Fdeployment%2FdeployAzure.json?WT.mc_id=cloud5mins-github-frbouche" target="_blank"><img src="https://azuredeploy.net/deploybutton.png"/></a>

Vous devez créer un lien d’image (en HTML ou Markdown) vers une destination spéciale construite en deux parties.

La première est un lien vers le portail Azure:

    https://portal.azure.com/#create/Microsoft.Template/uri/

Et le second est l'emplacement de votre modèle ARM:

    https%3A%2F%2Fraw.githubusercontent.com%2FFBoucher%2FAzUnzipEverything%2Fmaster%2Fdeployment%2FdeployAzure.json

Cependant, cette URL doit être encodée. Il y a plusieurs d'encodeurs disponibles ­en ligne, mais vous pouvez aussi le faire depuis le terminal avec la commande suivante (un grand merci à [@BrettMiller_IT](https://twitter.com/BrettMiller_IT) qui m'a montré cette astuce pendant l'un de mes live streams).

    [System.Web.HttpUtility]::UrlEncode("https://raw.githubusercontent.com/FBoucher/Not-a-Dog-Workshop/master/deployment/deployAzure.json")

En cliquant sur le bouton, l'utilisateur se trouvera sur la même page du portail Azure mais dans son l'abonnement.

### Azure DevOps Pipeline

Depuis le portail Azure DevOps (https://dev.azure.com), sélectionnez votre projet et créez un nouveau realease pipeline. Cliquez sur le bouton **+ Ajouter un artefact** pour connecter votre dépôt Git.

![AddArtifact][AddArtifact]

Une fois ajouté, vous devez ajouter une tâche au travail en cours. Cliquez sur le lien **1 job, 0 task** (4). Vous devez maintenant spécifier votre abonnement Azure, le nom du groupe de ressources et sélectionner l'emplacement de votre modèle ARM dans votre Git. Pour automatiser le déploiement à chaque poussée dans le référentiel, cliquez sur ce petit éclair et activez le *Continuous deployment trigger*.

![Continuous][Continuous]

## Pour terminer

Voilà, vous avez quatre façons différentes de déployer votre fonction Azure automatiquement. Mais ne me croyez pas sur parole, essayez vous-même! Si vous avez besoin de plus de détails, vous pouvez visiter le [projet sur GitHub](https://github.com/FBoucher/AzUnzipEverything) ou regarder cette vidéo où je présente le contenu de cet article en démonstration.

[![AzureCIThumbnail][AzureCIThumbnail]](https://www.youtube.com/embed/b54Guh_nDjo)


[GenericSimple]: /content/images/2019/07/GenericSimple.png "A simple ARM template for Azure Function"
[GenericDetailed]: /content/images/2019/07/GenericDetailed.png "A detailed ARM template for Azure Function"
[AzUnzipEverything]: /content/images/2019/07/AzUnzipEverything.png "Overview of the ARM Template for AzUnzipEverything"
[deployPortal]: /content/images/2019/07/deployPortal.png 
[AddArtifact]: /content/images/2019/07/AddArtifact.png 
[Continuous]: /content/images/2019/07/Continuous.png 
[AzureCIThumbnail]: /content/images/2019/07/AzureFunctionCICD_YT.png 


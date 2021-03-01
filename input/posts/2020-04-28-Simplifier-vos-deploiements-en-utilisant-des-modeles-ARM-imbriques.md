---
title: "Simplifier vos déploiements en utilisant des modèles Azure Resource manager (ARM) imbriqués" 
date: 2020-04-28
categories: post
tags: [azure,cloud,post,video,cloud5mins,arm,function,serverless,appservice,webapp, nested]
---


# Simplifier vos déploiements en utilisant des modèles Azure Resource manager (ARM) imbriqués

Dans la plupart des solutions, voir toutes, il y a plusieurs composantes: backend, frontend, services, APIs. Parce que tous ces projets peuvent avoir un cycle de vie différent, il est important de pouvoir les déployer individuellement. Cependant, parfois, nous souhaitons tout déployer en même temps. 

Dans ce billet, je vais expliquer comment j'utilise une condition avec des modèles [Azure Resource Manager (ARM) imbriqués](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/linked-templates?WT.mc_id=cloudenfr-blog-frbouche) pour permettre à l'utilisateur de décider s'il souhaite déployer uniquement le backend ou avec un frontend de son choix. Tout le code sera disponible dans [GitHub](https://github.com/FBoucher/AzUrlShortener) et si vous préférez, une version vidéo est disponible ci-dessous.


## Le contexte

Le projet utilisé dans ce post mon raccourcisseur d'URL Azure open source et abordable. Comme mentionné précédemment, le projet est composé de deux parties. Le backend exploite les fonctions Azure sans serveur (serverless) de Microsoft, c'est une correspondance parfaite dans ce cas, car il ne s'exécutera que lorsque quelqu'un cliquera sur un lien. La deuxième partie est une interface, et elle est totalement facultative. Étant donné que les fonctions Azure sont des HTTP triggers, elles agissent comme une API, par conséquent, elles peuvent être appelées à partir de tout ce qui peut effectuer un appel HTTP. Les deux sont très facilement déployables à l'aide d'un modèle ARM par une commande PowerShell ou CLI ou par un bouton en un clic directement depuis GitHub.

## Le but

À la fin de ce post, nous pourrons d'un simple clic déployer uniquement les fonctions Azure ou les déployer avec une interface de notre choix (il n'y a qu'une pour le moment, mais d'autres viendront). Pour ce faire, nous allons modifier le modèle ARM "backen" et imbriquer le modèle ARM responsable du déploiement frontend. 

Les modèles ARM sont disponibles ici dedans leurs états [initial](https://github.com/FBoucher/AzUrlShortener/tree/master/tutorials/optional-arm/before) et [final](https://github.com/FBoucher/AzUrlShortener/tree/master/tutorials/optional-arm/before/after).


## Ajout de nouvelles informations

Nous imbriquerons les modèles ARM, cela signifie que notre modèle de backend (azureDeploy.json) appellera le modèle de frontend (adminBlazorWebsite-deployAzure.json). Par conséquent, nous devons ajouter toutes les informations requises à `azureDeploy.json` pour nous assurer qu'il est capable de déployer` adminBlazorWebsite-deployAzure.json` avec succès. En regardant les paramètres requis pour le deuxième modèle, nous n'avons besoin que de deux valeurs «AdminEMail» et «AdminPassword». Toutes les autres peuvent être générées ou nous les avons déjà.

Nous aurons également besoin d'un autre paramètre qui agira comme option de sélection. Ajoutons donc un paramètre nommé `frontend` et n'autorisons que deux valeurs: *none* et *adminBlazorWebsite*. Si la valeur est *none*, nous déployons uniquement la fonction Azure. Lorsque la valeur est *adminBlazorWebsite*, nous déploierons la fonction Azure, bien sûr, mais nous déploierons également un site Web d'administration pour l'accompagner.

En suivant les meilleures pratiques, nous ajoutons des détails clairs et ajoutons ces trois paramètres dans la section **paramètres** du modèle ARM


```json
"frontend": {
    "type": "string",
    "allowedValues": [
        "none",
        "adminBlazorWebsite"
    ],
    "defaultValue": "adminBlazorWebsite",
    "metadata": {
        "description": "Select the frontend that will be deploy. Select 'none', if you don't want any. Frontend available: adminBlazorWebsite, none. "
    }
},
"frontend-AdminEMail": {
    "type": "string",
    "defaultValue": "",
    "metadata": {
        "description": "(Required only if frontend = adminBlazorWebsite) The EMail use to connect into the admin Blazor Website."
    }
},
"frontend-AdminPassword": {
    "type": "securestring",
    "defaultValue": "",
    "metadata": {
        "description": "(Required only if frontend = adminBlazorWebsite) Password use to connect into the admin Blazor Website."
    }
}
```


## Modèles imbriqués

Supposons pour l'instant que nous déployions toujours le site Web lorsque nous déployons les fonctions Azure, pour garder les choses séparées. Ce dont nous avons besoin maintenant, c'est d'utiliser un modèle ARM imbriqué, et cela lorsque vous déployez un modèle ARM à partir d'un autre modèle ARM. Cela se fait avec un noeud `Microsoft.Resources / deployments`. Regardons le code:


```json
{
    "name": "FrontendDeployment",
    "type": "Microsoft.Resources/deployments",
    "dependsOn": [
        "[resourceId('Microsoft.Web/sites/', variables('funcAppName'))]",
        "[resourceId('Microsoft.Web/sites/sourcecontrols', variables('funcAppName'), 'web')]"
    ],
    "resourceGroup": "[resourceGroup().name]",
    "apiVersion": "2019-10-01",
    "properties": {
        "mode": "Incremental",
        "templateLink": {
            "uri": "[variables('frontendInfo')[parameters('frontend')].armTemplateUrl]"
        },
        "parameters": {
            "basename": {
                "value" : "[concat('adm', parameters('baseName'))]"
            },
            "AdminEMail": {
                "value" : "[parameters('frontend-AdminEMail')]"
            },
            "AdminPassword": {
                "value" : "[parameters('frontend-AdminPassword')]"
            },
            "AzureFunctionUrlListUrl": {
                "value" : "[concat('https://', reference(resourceId('Microsoft.Web/sites/', variables('funcAppName')), '2018-02-01').hostNames[0], '/api/UrlList?code=', listkeys(concat(resourceId('Microsoft.Web/sites/', variables('funcAppName')), '/host/default/'),'2016-08-01').functionKeys.default)]"
            },
            "AzureFunctionUrlShortenerUrl": {
                "value" : "[concat('https://', reference(resourceId('Microsoft.Web/sites/', variables('funcAppName')), '2018-02-01').hostNames[0], '/api/UrlShortener?code=', listkeys(concat(resourceId('Microsoft.Web/sites/', variables('funcAppName')), '/host/default/'),'2016-08-01').functionKeys.default)]"
            },
            "GitHubURL": {
                "value" : "[parameters('GitHubURL')]"
            },
            "GitHubBranch": {
                "value" : "[parameters('GitHubBranch')]"
            },
            "ExpireOn": {
                "value" : "[parameters('ExpireOn')]"
            },
            "OwnerName": {
                "value" : "[parameters('OwnerName')]"
            }
        }
    }
}
```

Si nous examinons ce noeud, nous avons le classique: nom, type, dependOn, resourceGroup, apiVersion. Ici, nous voulons vraiment que les fonctions Azure soient entièrement déployées, nous avons donc besoin que FunctionApp soit créé ET que la synchronisation GitHub soit complète, c'est pourquoi nous avons également une dépendance à `Microsoft.Web/sites/sourcecontrols`.

Dans *properties*, nous passerons le mode en **Incremental**, car il laissera les ressources inchangées qui existent dans le groupe de ressources, mais ne sont pas spécifiées dans le modèle.

> Apprenez en plus sur les [modes de déploiements Azure Resource Manager ici](https://docs.microsoft.com/en-us/azure/azure-resource-manager/templates/deployment-modes?WT.mc_id=cloudenfrancais-blog-frbouche), car ils sont très puissants.

La deuxième propriété est **templateLink**. C'est vraiment important, car il s'agit de l'URL de l'autre modèle ARM. Cet URI ne doit pas être un fichier local ou un fichier uniquement disponible sur votre réseau local. Vous devez fournir une valeur URI téléchargeable au format HTTP ou HTTPS. Dans notre cas, il s'agit d'une variable qui contient l'URL GitHub où le modèle est disponible.

Enfin, nous avons les **parameters**, c'est ainsi que nous transmettons les valeurs au deuxième modèle. Ignorons ceux où sont simplement passé la valeur du paramètre de l'appelant à l'appelé et concentrons-nous sur *basename*, *AzureFunctionUrlListUrl* et *AzureFunctionUrlShortenerUrl*.

Pour *basename* nous ajoute simplement un préfixe au paramètre *basename* reçu, de cette façon les noms des ressources sont différent tout en gardant une similitude. C'est purement optionnel, vous auriez pu préférer ajouter un paramètre à `azureDeploy.json`, je préfère garder les paramètres au minimum, car je pense que cela simplifie utilisation de mon modèle pour les utilisateurs.

Enfin, pour *AzureFunctionUrlListUrl* et *AzureFunctionUrlShortenerUrl*, je devais récupérer l'URL de la fonction Azure avec le jeton de sécurité, car elles sont sécurisées. Nous allons donc reconstruire ces URLs en concaténant différentes informations.


| Composante | Valeur
------------- | -------
| Début de l'URL | 'https://'
| Référence de la FunctionApp, pour retourner la valeur du *hostname* | reference(resourceId('Microsoft.Web/sites/', variables('funcAppName')), '2018-02-01').hostNames[0]
| Spécifiez la fonction ciblée dans ce cas, UrlList. Et démarrer la *querystring* pour passer **code** (aka. Jeton de sécurité) | '/api/UrlList?code='
| Utilisation de la nouvelle fonction `listkeys` pour récupérer le jeton de sécurité de la fonction App. | listkeys(concat(resourceId('Microsoft.Web/sites/', variables('funcAppName')), '/host/default/'),'2016-08-01').functionKeys.default


## Pièces conditionnelles

Maintenant que le deuxième modèle ARM peut être déployé, ajoutons une condition pour qu'il ne soit, en effet, déployé que lorsque nous le souhaitons. Pour ce faire, il s'agit simplement d'ajouter une propriété `condition`.

```json
{
    "name": "FrontendDeployment",
    "type": "Microsoft.Resources/deployments",
    "condition": "[not(equals(parameters('frontend'), 'none'))]",
    "dependsOn": [
        "[resourceId('Microsoft.Web/sites/', variables('funcAppName'))]",
        "[resourceId('Microsoft.Web/sites/sourcecontrols', variables('funcAppName'), 'web')]"
    ]
}
```

Dans ce cas, si la valeur du paramètre est différente alors **none**, le modèle imbriqué sera déployé. Lorsqu'une le résultat d'une condition est "fausse"  l'ensemble de la ressource sera ignorée pendant le déploiement. À quel point vos conditions sont simples ou complexes ... c'est votre choix!

Bon déploiement.

<iframe width="560" height="315" src="https://www.youtube.com/embed/zwtO21kEfWo" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
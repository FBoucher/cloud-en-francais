---
title: Comment faire en sorte que vos déploiements fonctionnent à tout coup
permalink: /comment-faire-en-sorte-déploiement-fonctionne-a-tout-coup
Published: 2019-05-23
tags: [azure, cloud, post, cloud5mins, video, arm, azure resource manager, best practices]
---

Votre code est terminé et vous êtes prêt à le déployer dans Azure. Vous exécutez le script PowerShell ou Bash que vous avez et BOOM! Le message d’erreur disant que ce nom est déjà pris. Dans ce billet, je vous montrerai un moyen simple de faire vos déploiements comme un pro, et ce à tout coup.

> ____ with given name ____ already exists.

Les astuces que d’autres utilisent
----------------------------------

Vous pourriez ajouter un chiffre à la fin du nom de la ressource (ex. : demo-app1, demo-app2, demo-app123…), mais ce n’est pas vraiment professionnel. Vous pourriez aussi créer une chaîne aléatoire et l’ajouter au nom. Oui, cela fonctionnera une fois. Si vous essayez de redéployer vos ressources, cette valeur changera et ne sera donc plus jamais la même.

La solution serait d’avoir une chaîne unique constante dans notre environnement.

La solution
------------

La solution consiste à utiliser la fonction `UniqueString()` du modèle Azure Resource Manager (ARM). Si nous regardons la [documentation](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-template-functions-string#uniquestring?WT.mc_id=cloud5mins-youtube-frbouche), UniqueString *crée une chaîne de hachage déterministe basée sur les valeurs fournies en tant que paramètres.* Voyons un exemple rapide de modèle ARM permettant de déployer un site Web nommer très simplement `demo-app`.

```json
{
    "$schema" : "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
    "contentVersion" : "1.0.0.0",
    "parameters" : {},
    "variables" : {
        "webAppName" : "demo-app"
    },
    "resources" : [
        {
            "type" : "Microsoft.Web/sites",
            "apiVersion" : "2015-08-01",
            "name" : "[variables('webAppName')]",
            "location" : "[resourceGroup().location]",
            "tags" : {
                "[concat('hidden-related:', resourceGroup().id, '/providers/Microsoft.Web/serverfarms/frankdemo-plan')]" : "Resource",
                "displayName" : "[variables('webAppName')]"
            },
            "dependsOn" : [
                "Microsoft.Web/serverfarms/frankdemo-plan"
            ],
            "properties" : {
                "name" : "[variables('webAppName')]",
                "serverFarmId" : "[resourceId('Microsoft.Web/serverfarms/', 'frankdemo-plan')]"
            }
        },
        {
            "type" : "Microsoft.Web/serverfarms",
            "apiVersion" : "2016-09-01",
            "name" : "frankdemo-plan",
            "location" : "[resourceGroup().location]",
            "sku": {
                "name": "F1",
                "capacity" : 1
            },
            "tags": {
                "displayName": "frankdemo-plan"
            },
            "properties" : {
                "name" : "frankdemo-plan"
            }
        }
    ],
    "outputs" : {}
}
```

Si vous essayez de déployer ce modèle, vous aurez la même erreur parce que le nom `demo-app` est déjà pris… Normal, pas de surprise ici.
Créons une nouvelle variable `suffix` et nous utiliserons les valeurs **Resource Group Id** et **Location**. Ensuite, nous devons juste ajouter cette valeur à notre nom en utilisant la fonction `concat()`.


```json
    "variables" : {
        "suffix" : "[uniqueString(resourceGroup().id, resourceGroup().location)]",
        "webAppName" : "[concat('demo-app', variables('suffix'))]"
    }
```
Maintenant, chaque fois que vous déploierez une chaîne unique sera ajoutée à votre nom de ressource. Cette chaîne sera toujours la même pour un déploiement d’emplacements de groupe de ressources. C’est si simple!

Certains types de ressources étant plus restrictifs que d’autres, vous devrez peut-être adapter votre nouveau nom. Peut-être que le nom de votre ressource et blob de treize caractères seront trop longs… Pas de problème, vous pouvez facilement le rendre plus court et tout en minuscule en utilisant simplement `substring()` et `toLower()`.

```json
 "parameters" : {},
    "variables" : {
        "suffix" : "[substring(toLower(uniqueString(resourceGroup().id, resourceGroup().location)),0,5)]",
        "webAppName" : "[concat('demo-app', variables('suffix'))]"
    }
```

Voilà, et maintenant, en utilisant le modèle ARM, vous pouvez déployer et redéployer sans aucun problème pour reproduire la même solution que vous avez construite. Pour en apprendre davantage à propos des modèles ARM, vous pouvez accéder à la [documentation](https://docs.microsoft.com/en-us/azure/azure-resource-manager/?WT.mc_id=cloud5mins-youtube-frbouche), où vous trouverez des exemples, des didacticiels pas à pas et plus encore.

![Success](/content/images/2019/05/children-593313_640.jpg)

Si vous avez une question spécifique à propos des modèles ARM ou si vous souhaitez voir plus d’astuce comme celle-ci, n’hésitez pas et laissez un commentaire, ou à vous rejoignez-moi sur les médias sociaux!

## En vidéo s’il vous plaît

J’ai aussi une vidéo de ce post si vous préférez.

<iframe allow="autoplay; encrypted-media" allowfullscreen="" frameborder="0" height="315" src="https://www.youtube.com/embed/HmjS4wcYJBQ" width="560"></iframe>  

Image by <a href="https://pixabay.com/users/StartupStockPhotos-690514/?utm_source=link-attribution&amp;utm_medium=referral&amp;utm_campaign=image&amp;utm_content=593313">StartupStockPhotos</a> from <a href="https://pixabay.com/?utm_source=link-attribution&amp;utm_medium=referral&amp;utm_campaign=image&amp;utm_content=593313">Pixabay</a>


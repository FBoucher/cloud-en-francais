---
title: Zip push - Déploiement facile et rapide de vos Azure Functions
permalink: /zip-push-deploiement-facile-et-rapide-de-vos-azure-functions
Published: 2018-06-07
tags: [cloud, youtube, azure, post, azurecli, arm, serverless, cloud5minutes, functions, zip push, zipdeploy, sansserver, déploiement]
---

Les Azure fonction sont géniales. J’avais l’habitude de faire beaucoup de versions « csx » (version C# scriptée) mais récemment, je suis passé à la version compilée, et j’ai vraiment adoré ! Cependant, je cherchais un moyen de garder mon déploiement rapide et facile, parce que parfois je n’ai pas le temps d’installer un « gros » CI/CD ou simplement parce que parfois je ne suis pas celui qui fait le déploiement... Pour ces cas, j’ai besoin d’un script simple qui va tout déployer! 

Dans ce post, je vais partager avec vous comment vous pouvez tout déployer avec un script vraiment pas compliqué.

## Le contexte

Dans cette démo, je vais déployer une simple Azure Fonction en C# (framework .Net complet). Je vais créer la FunctionApp et le stockage à l’aide d’un modèle Azure Resource Manager (ARM) et déployer avec une méthode nommée Zip push ou ZipDeploy. Tout le code, script, et modèle est disponible sur [mon Github](https://github.com/FBoucher/GetStarted-AzFunction-ZipDeploy).

## Le code des fonctions Azure

La fonction Azure n’a pas besoin d’être spéciale et peut être n’importe quelle langue prise en charge par Azure Functions. Donc dans le but de rester à livre ouvert, voici le code de ma fonction. 

```csharp
namespace AzFunctionZipDeploy
{
    public static class Function1
    {
        [FunctionName("GetTopRunner")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            string top = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "top", true) == 0)
                .Value;

            if (top == null)
            {
                dynamic data = await req.Content.ReadAsAsync<object>();
                top = data?.top;
            }

        return top == null
                ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a number to get your top x runner on the query string or in the request body")
                : req.CreateResponse(HttpStatusCode.OK, new { message = $"Hello, here is your Top {top} runners", runners = A.ListOf<Person>(int.Parse(top)) });
        }
    }

    class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
}
```

C’est une fonction très simple qui retournera une liste de `Person` générée à la volée. La liste contiendra autant de personne que le nombre passé en paramètre. J’utilise [GenFu](https://github.com/MisterJames/GenFu), une libraire très utile, de mes copains: Monsters ASP.NET.

La seule particularité est que nous devons maintenant faire notre fichier compressé (Zip ou Rar) qui contient tout ce dont notre projet a besoin.

![createZip](/content/images/2018/06/createZip.png)

Dans le cas présent, il y a: le fichier projet (AzFunction-ZipDeploy.csproj), le code de la fonction (Function1.cs) le fichier host (host.json) et les paramètres locaux de notre fonction (local.settings.json).

## Le modèle ARM 

Pour cette démo, nous avons besoin seulement d’une Function App. Je vais utiliser un modèle qui fait partie des [Modèles Quickstart Azure](https://github.com/Azure/azure-quickstart-templates/tree/master/101-function-app-create-dynamic). Un coup d’oeil rapide au fichier `azuredeploy.parameters.json` et nous constaterons qu’un seul des paramètres nécessite vraiment une valeur. Il s’agit du nom de notre application.

```json
{
    "$schema": "https://schema.management.azure.com/schemas/2015-01-01/deploymentParameters.json#",
    "contentVersion": "1.0.0.0",
    "parameters": {
        "appName": {
        "value": "zipdeploydemo"
        }
    }
}
```

Pour être en mesure faire le ZipDeploy, nous devons ajouter un *Application Setting* pour laisser savoir à l’interface Kudu nous avons besoin de son aide pour compiler notre code. Pour ce faire, ouvrons `azuredeploy.json` et allons dans la section `appSettings`. Ajouter une nouvelle variable nommée: `SCM_DO_BUILD_DURING_DEPLOYMENT` qui sera égale à `true`. Après avoir ajouté variable, cette section devrait ressembler à ceci:
 
```json
"appSettings": [
    {
    "name": "AzureWebJobsDashboard",
    "value": "[concat('DefaultEndpointsProtocol=https;AccountName=', variables('storageAccountName'), ';AccountKey=', listKeys(variables('storageAccountid'),'2015-05-01-preview').key1)]"
    },
    {
    "name": "AzureWebJobsStorage",
    "value": "[concat('DefaultEndpointsProtocol=https;AccountName=', variables('storageAccountName'), ';AccountKey=', listKeys(variables('storageAccountid'),'2015-05-01-preview').key1)]"
    },
    {
    "name": "WEBSITE_CONTENTAZUREFILECONNECTIONSTRING",
    "value": "[concat('DefaultEndpointsProtocol=https;AccountName=', variables('storageAccountName'), ';AccountKey=', listKeys(variables('storageAccountid'),'2015-05-01-preview').key1)]"
    },
    {
    "name": "WEBSITE_CONTENTSHARE",
    "value": "[toLower(variables('functionAppName'))]"
    },
    {
    "name": "FUNCTIONS_EXTENSION_VERSION",
    "value": "~1"
    },
    {
    "name": "WEBSITE_NODE_DEFAULT_VERSION",
    "value": "6.5.0"
    },
    {
    "name": "SCM_DO_BUILD_DURING_DEPLOYMENT",
    "value": true
    }
]
```

## The Deployment Script

Maintenant que tous les morceaux du puzzle sont prêts, il est temps de les rassembler dans un script. En fait, seules les deux dernières commandes sont requises; tout le reste est juste pour le rendre le script plus facile à réutiliser. Consultez mon post précédent [5 étapes simples pour obtenir un modèle ARM propre](http://www.frankysnotes.com/2018/05/5-simple-steps-to-get-clean-arm-template.html), pour en savoir plus sur les meilleures pratiques liées au modèle ARM. `

Voyons ce script, il est assez simple.

    # script to Create an Azure Gramophone-PoC Solution

    resourceGroupName=$1
    resourceGroupLocation=$2

    templateFilePath="./arm/azuredeploy.json"
    parameterFilePath="./arm/azuredeploy.parameters.json"

    dateToken=`date '+%Y%m%d%H%M'`
    deploymentName="FrankDemo"$dateToken

    # az login

    # You can select a specific subscription if you do not want to use the default
    # az account set -s SUBSCRIPTION_ID

    if !( $(az group exists -g  $resourceGroupName) ) then
        echo "---> Creating the Resourcegroup: " $resourceGroupName
        az group create -g $resourceGroupName -l $resourceGroupLocation
    else
        echo "---> Resourcegroup:" $resourceGroupName "already exists."
    fi

    az group deployment create --name $deploymentName --resource-group $resourceGroupName --template-file $templateFilePath --parameters $parameterFilePath --verbose

    echo "---> Deploying Function Code"
    az functionapp deployment source config-zip -g $resourceGroupName -n zipdeploydemo --src "./zip/AzFunction-ZipDeploy.zip"

    echo "---> done <---"

La seule commande qui est différente d’un déploiement ordinaire, est la dernière commande `functionapp deployment source config-zip`. Cette dernière spécifie à l’interface Kudu de regarder `--src` pour obtenir notre source. Comme j’exécute localement, le chemin d’accès pointe vers un dossier local. Cependant, vous pouvez également exécuter cette commande dans le CloudShell, et cela deviendra un URI ... vers un stockage Azure Blob par exemple.

## Déployer et tester

Si vous ne l’avez pas encore remarqué, j’ai fait mon script dans bash et Azure CLI. Parce que je veux que mon script soit compatible avec toutes les plateformes. Bien sûr, vous auriez pu le faire en PowerShell ou tout autre langage qui appelle l’API REST.

Pour déployer, exécutez simplement le script qui transmet le nom ResourceGroup et son emplacement (location).

    ./Deploy-AZ-Gramophone.sh cloud5mins eastus

![ScriptOutputs](/content/images/2018/06/ScriptOutputs.png)

Pour accéder à l’URL de la fonction, allez au portail Azure (portal.azure.com) et cliquez sur la Function App que nous venons de déployer. Cliquez sur la fonction `GetTopRunner`, et cliquez sur le bouton </> Getfunction URL.

![GetFunctionURL](/content/images/2018/06/GetFunctionURL.png)

Utilisez cette URL dans postman et passez un autre paramètre `top` pour voir si le déploiement a réussi.

![postmanTest](/content/images/2018/06/postmanTest.png)

Voilà!

## En vidéo SVP!

Si vous préférez j’ai une version en vidéo de ce post. 

<div class="container">
<iframe  class="youtubevideo" width="560"  src="https://www.youtube.com/embed/7pkd_VNVmh8" frameborder="0" allow="autoplay; encrypted-media" allowfullscreen></iframe>
</div>
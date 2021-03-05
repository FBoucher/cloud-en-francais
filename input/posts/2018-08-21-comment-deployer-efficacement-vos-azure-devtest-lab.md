---
title: Comment déployer efficacement vos Azure Devtest lab
permalink: /comment-deployer-efficacement-vos-azure-devtest-lab
Published: 2018-08-21
tags: [cloud, azure, post, arm, devtestlab, devops, deployments]
---

Les Azure Devtest lab sont des outils fantastiques pour créer rapidement des environnements complets à des fins de développement et de test ou dans un cadre éducatif (salle de formation). Ils offrent d'excellents outils pour restreindre les utilisateurs sans supprimer toutefois leur liberté. Le temps de connexion est grandement accéléré, avec ses VMs "claimable" qui sont déjà créées et attendent l'utilisateur. Les formules vous aideront à toujours installer la dernière version de votre artefact sur ces machines virtuelles. Et enfin, l’arrêt automatique gardera votre argent là où il devrait rester… dans votre poche.

![](/content/images/2018/08/La-Cigale-et-la-fourmie_devtest_FR-1.png)

Dans cet article, je vous montrerai comment écrire un modèle Azure Resource Manager (ARM) avec un Azure Devtest Lab et créer les VM *claimable* basées sur vos formules. Vous pourrez ensuite déployer le tout en un seul clic.

Étape 1 - Le modèle ARM
-----------------------

Tout d'abord, nous avons besoin d'un modèle ARM. Vous pouvez bien sûr repartir de zéro, mais cela peut être plus compliqué, pour les débutants. Vous pouvez également en choisir un dans [GiHub](https://github.com/Azure/azure-devtestlab) et le personnaliser.

L'approche que je préfère, est de créer un simple laboratoire directement à partir du portail Azure. Une fois votre laboratoire créé, accédez à l'option *Automation script* du groupe de ressources et copiez / collez le modèle ARM dans votre éditeur de texte préféré.

![armTemplate](/content/images/2018/08/armTemplate.png)

Maintenant, il faut le nettoyer. Pour ce faire, vous pouvez utiliser la méthode [5 étapes faciles pour obtenir un ARM template « clean»](http://www.cloudenfrancais.com/5-etapes-faciles-pour-obtenir-un-arm-template-clean), c'est un excellent moyen de démarrer.

Une fois le modèle propre, nous devons ajouter quelques éléments qui n’ont pas suivi lors de l’exportation. Habituellement, dans un modèle ARM, il n'y a qu'une list qui se nomme `resources`. Cependant, un laboratoire Devtest contient également une liste nommée **ressources**. 

    {
        "parameters": {},
        "variables": {},
        "resources": [],
    }

Cette seconde liste imbriqué n'a pas été incluse lors de l'exportation du modèle, il faudra donc l'ajouter avec quelque sous composantes.  Dans l'exemple suivant, j'ai ajouté la liste des ressources du laboratoire (juste après la *location* du laboratoire). Cette liste doit contenir un `virtualnetworks`. C'est aussi une bonne idée d’ajouter `schedule` et un ` notificationChannels`. Ces deux derniers seront utilisés pour arrêter automatiquement toutes les machines virtuelles et pour envoyer une notification à l'utilisateur juste avant.

```json
{
    "$schema": "https://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#",
    "contentVersion": "1.0.0.0",
    "parameters": {
        ...
    },
    "variables": {
        ...
    },
    "resources": [
        {
            "type": "Microsoft.DevTestLab/labs",
            "name": "[variables('LabName')]",
            "apiVersion": "2016-05-15",
            "location": "[resourceGroup().location]",
            "resources": [
                {
                    "apiVersion": "2017-04-26-preview",
                    "name": "[variables('virtualNetworksName')]",
                    "type": "virtualnetworks",
                    "dependsOn": [
                        "[resourceId('microsoft.devtestlab/labs', variables('LabName'))]"
                    ]
                },
                {
                    "apiVersion": "2017-04-26-preview",
                    "name": "LabVmsShutdown",
                    "type": "schedules",
                    "dependsOn": [
                        "[resourceId('Microsoft.DevTestLab/labs', variables('LabName'))]"
                    ],
                    "properties": {
                        "status": "Enabled",
                        "timeZoneId": "Eastern Standard Time",
                        "dailyRecurrence": {
                            "time": "[variables('ShutdowTime')]"
                        },
                        "taskType": "LabVmsShutdownTask",
                        "notificationSettings": {
                            "status": "Enabled",
                            "timeInMinutes": 30
                        }
                    }
                },
                {
                    "apiVersion": "2017-04-26-preview",
                    "name": "AutoShutdown",
                    "type": "notificationChannels",
                    "properties": {
                        "description": "This option will send notifications to the specified webhook URL before auto-shutdown of virtual machines occurs.",
                        "events": [
                            {
                                "eventName": "Autoshutdown"
                            }
                        ],
                        "emailRecipient": "[variables('emailRecipient')]"
                    },
                    "dependsOn": [
                        "[resourceId('Microsoft.DevTestLab/labs', variables('LabName'))]"
                    ]
                }
            ],
            "dependsOn": []
        }
        ...
```

Étape 2 - Les Formules
----------------------

Maintenant que le Devtest Lab est bien défini, il est temps d'ajouter nos formules. Si vous en avez déjà créé depuis le portail, ne les recherchez pas dans le modèle. Pour le moment, l'exportation ne scriptera pas les formules.

Un moyen rapide d'obtenir le code JSON de vos formules consiste à les créer à partir du portail, puis à utiliser [Azure Resources Explorer](https://resources.azure.com) pour obtenir le code.

![resourceExplorer](/content/images/2018/08/resourceExplorer.png)

Dans un navigateur Web, accédez à https://resources.azure.com pour ouvrir votre explorateur de ressources. Sélectionnez l'abonnement, le groupe de ressources et le laboratoire sur lesquels vous travaillez. Dans le noeud Formules (4), vous devriez voir vos formules, cliquez sur une et laissez-le entrer dans notre modèle ARM. Copiez-collez-le au niveau de `ressource` (la première liste, pas celle imbriquée dans le laboratoire).


Étape 2.5 - L'Azure KeyVault
----------------------------

Il fortement recommandé de ne pas mettre de mot de passe dans un modèle ARM. Cependant, ils sont très pratiques quand ils sont définis dans les formules. Une solution consiste à utiliser un Azure KeyVault.

Supposons que KeyVault existe déjà, j'expliquer comment le créer plus tard. Dans votre fichier de paramètres, ajoutez un paramètre nommé `adminPassword` et faites référence au KeyVault. Ensuite, spécifier le nom du secret qui contiendra le mot de passe. Dans notre cas, nous allons placer le mot de passe dans un secret nommé *vmPassword*.

```json
"adminPassword": {
    "reference": {
        "keyVault": {
            "id": "/subscriptions/{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}/resourceGroups/cloud5mins/providers/Microsoft.KeyVault/vaults/Cloud5minsVault"
        },
        "secretName": "vmPassword"
    }
}
```

Maintenant, pour obtenir le mot de passe dans le modèle ARM, utilisez simplement un paramètre normal, et le tour est joué!


Étape 3 - Les VMs dites claimables
----------------------------------

Maintenant que nous avons un Lab et les formules, la seule chose qui manque est la claimable VM basée sur les formules. Il est impossible de créer dans le même modèle ARM des formules et des machines virtuelles basé sur cèst dernier. L'alternative consiste à utiliser un script qui créera nos machines virtuelles juste après le déploiement.

    az group deployment create --name test-1 --resource-group cloud5mins --template-file DevTest.json --parameters DevTest.parameters.json --verbose

    az lab vm create --lab-name C5M-DevTestLab -g  cloud5mins --name FrankDevBox --formula SimpleDevBox  

Comme vous pouvez le voir dans la deuxième commande Azure CLI, nous créons une machine virtuelle nommée *FrankDevBox* basée sur la formule *SimpleDevBox*. Notez que nous n'avons pas besoin de spécifier le nom d'usager ni son mot de passe, car tout était défini dans la formule. Génial!

Voici une partie d'un script qui va créer un KeyVault (s'il n'existe pas) et le remplir. Ensuite, il déploiera notre modèle ARM et créera enfin notre claimable VM. Vous pouvez trouver tout le code sur mon projet GitHub: [Azure-Devtest-Lab-efficient-deployment-sample](https://github.com/FBoucher/Azure-Devtest-Lab-efficient-deployment-sample).

```bash

[...]

# Checking for a KeyVault
searchKeyVault=$(az keyvault list -g $resourceGroupName --query "[?name=='$keyvaultName'].name" -o tsv )
lenResult=${#searchKeyVault}

if [ ! $lenResult -gt 0 ] ;then
    echo "---> Creating keyvault: " $keyvaultName
    az keyvault create --name $keyvaultName --resource-group $resourceGroupName --location $resourceGroupLocation --enabled-for-template-deployment true
else
    echo "---> The Keyvaul $keyvaultName already exists"
fi


echo "---> Populating KeyVault..."
az keyvault secret set --vault-name $keyvaultName --name 'vmPassword' --value 'cr@zySheep42!'


# Deploy the DevTest Lab

echo "---> Deploying..."
az group deployment create --name $deploymentName --resource-group $resourceGroupName --template-file $templateFilePath --parameters $parameterFilePath --verbose

# Create the VMs using the formula created in the deployment

labName=$(az resource list -g cloud5mins --resource-type "Microsoft.DevTestLab/labs" --query [*].[name] --output tsv)
formulaName=$(az lab formula list -g $resourceGroupName  --lab-name $labName --query [*].[name] --output tsv)

echo "---> Creating VM(s)..."
az lab vm create --lab-name $labName -g  $resourceGroupName --name FrankSDevBox --formula $formulaName 
echo "---> done <---"

```

Il est important de noter que pour pouvoir utiliser le Key Vault avec votre déploiement, vous devez spécifier le paramètre `--enabled-for-template-deployment true`.


Conclusion
----------

Que ce soit pour le développement, faire des tests, ou pour de la formation, dès que vous créez des environnements dans Azure, les DevTest Labs sont un incontournable. C'est un outil très puissant que peu de gens connaissent. Essayez-le vous verrez! 


En vidéo s’il vous plaît
------------------------

J’ai aussi une vidéo de ce post si vous préférez.

<div class="container">
<iframe  class="youtubevideo" width="560"  src="https://www.youtube.com/embed/FJOyXfbQlbI" frameborder="0" allow="autoplay; encrypted-media" allowfullscreen></iframe>
</div>

###### Références:

- [Azure-Devtest-Lab-efficient-deployment-sample](https://github.com/FBoucher/Azure-Devtest-Lab-efficient-deployment-sample)
- [Un aperçu d'Azure DevTest Labs](https://www.youtube.com/watch?v=Q06av_ojeYw)
- [Meilleures pratiques pour les modèles Azure Resource Manager (ARM)](https://www.youtube.com/watch?v=tG24xbxaTms&t=96s)
- [5 étapes faciles pour obtenir un ARM template « clean»]( http://www.cloudenfrancais.com/5-etapes-faciles-pour-obtenir-un-arm-template-clean)




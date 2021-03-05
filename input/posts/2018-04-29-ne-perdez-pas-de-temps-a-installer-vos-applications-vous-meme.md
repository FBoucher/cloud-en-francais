---
title: Ne perdez pas de temps à installer vos applications vous-même
permalink: /ne-perdez-pas-de-temps-a-installer-vos-applications-vous-meme
Published: 2018-04-29
tags: [cloud, azure, post, azurecli, vm, vscode, chocolatey, iaas, cloud5mins]
---


Je ne sais pas pour vous, mais je personnellement je déteste perdre mon temps. C'est pourquoi il y a quelques années j'ai commencé à utiliser des scripts pour installer tous les logiciels dont j'ai besoin sur mon PC. Un nouveau portable? Aucun problème, il n'y a qu'a exécuter un script, aller prendre un café et quand on revient tous nos logiciels préférés (et requis) sont installés. Sous Linux, vous pouvez utiliser `apt-get`, et sur Windows, mon favori actuel est [Chocolatey](https://chocolatey.org/). 

Récemment j'ai utilisé plus de machines virtuelles (VM) dans le nuage et j'ai décidé que je devrais essayer d'utiliser un script Chocolatey *pendant le déploiement*. De cette façon, une fois la VM créée, les logiciels dont j'ai besoin seront déjà installés, et j'aurai sauvé un temps précieux! Ce post se veut un récapitulatif sur le sujet pour y arriver, tous les scripts, problèmes et solutions de contournement (workarounds) y seront expliqués.

## Le but

Créer une nouvelle VM localement et appliquer les mise à jour du système d'exploitation et installer tous les outils dont vous avez besoin (comme Visual Stutio IDE) prendra des heures... La solution Azure-Chocolatey devrait être terminée sous la barre des 10 minutes (~ 7min dans mon cas). 

Une fois la machine virtuelle disponible, Visual Studio 2017 Enterprise, VSCode, Git et Node.Js doivent être installés. En fait, j'aimerais utiliser le même script Chocolatey que j'utilise régulièrement.

(Mon script Chocolatey est disponible sur [gist.github](https://gist.github.com/FBoucher/f60d72fb8e77d0cfb278361b5966c03a))

``` bash

#Install Chocolatey
Set-ExecutionPolicy Bypass -Scope Process -Force; iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))

#Install Software
choco install visualstudiocode -y
choco install git -y
choco install nodejs-lts  -y

```

## Les outils

Dans ce post, j'utiliserai Azure CLI, car il fonctionne sur n'importe quel environnement. Cependant, PowerShell peut également être utilisé et seulement quelques commandes seront différentes. La machine virtuelle sera déployée avec un modèle Azure Resource Manager (ARM template). Pour créer et éditer les templates ARM j'aime utiliser [VSCode](https://code.visualstudio.com/), vous n'en avez pas besoin, mais c'est tellement plus simple avec! J'utilise deux extensions.

La première [Azure Resource Manager Snippets](https://marketplace.visualstudio.com/items?itemName=samcogan.arm-snippets) aidera en générant des morceaux de schéma pour nos besoins. Dans un fichier JSon, il suffit de taper `arm` et voilà! Vous avez une longue liste de modèles ARM!

![ARM Snippets](/content/images/2018/04/armSnippets.gif)

La deuxième est [Azure Resource Manager Tools](https://marketplace.visualstudio.com/items?itemName=msazurermtools.azurerm-vscode-tools). Cette extension supporte langage pour ARM et offre de la validation. Très utile...

![Very useful validation](/content/images/2018/04/toolvalidation.png)

## La création du ARM "template"

Pour commencer, créez un nouveau fichier JSon. Puis tapez `arm` et sélectionnez la première option; pour obtenir un squelette vide. Puis ajoutez une ligne supplémentaire dans `resources` et tapez à nouveau `arm`. Cette fois, faites défiler la liste d'options jusqu'à voir `arm-vm-windows`.

![Step 2 Here](/content/images/2018/04/step2Here.png)

Un multicurseur vous permettra d'éditer le nom de votre VM partout dans le fichier en une seule fois. Appuyez sur Tab pour naviguer automatiquement vers le nom d'utilisateur et une fois encore sur Tab pour aller au mot de passe.

![createARM](/content/images/2018/04/createARM.gif)

Nous avons maintenant un modèle ARM fonctionnel que nous pourrions déployer. Cependant, ajoutons quelques petites choses en premier.

## La recherche des "SKU" par code

L'une de mes images préférées de VM pour une machine de développeur (alias: DevBox) est celle qui inclut Visual Studio préinstallé. Une chose à savoir est que ces images ne peuvent être déployées que dans un abonnement MSDN. Pour spécifier l'image que vous voulez utiliser, vous devez passer un *publisher*, *offer* et *sku*.

Voici comment faire avec les commandes Azure CLI

```bash 

# Liste tous les éditeurs (publisher) qui contiennent VisualStudio (sensible aux majuscules et minuscules)
az vm image list-publishers --location eastus --output table --query "[?contains(name,'VisualStudio')]"

# Affiche toutes les offre (offer) pour le publisher MicrosoftVisualStudio
az vm image list-offers --location eastus --publisher MicrosoftVisualStudio  --output table

# Liste de tous les SKU disponibles pour le publisher MicrosoftVisualStudio avec l'offre VisualStudio
az vm image list-skus --location eastus --publisher MicrosoftVisualStudio --offer VisualStudio --output table

```

Maintenant que toutes les informations sont trouvées, recherchez-les dans le ARM template et remplacez les valeurs actuelles par celle trouvée. Dans mon cas, voici les nouvelles valeurs.

```json

    "imageReference": {
                        "publisher": "MicrosoftVisualStudio",
                        "offer": "VisualStudio",
                        "sku": "VS-2017-Ent-Win10-N",
                        "version": "latest"
                    }

```

## L'ajout du script personnalisé

Génial maintenant, nous avons un modèle qui créera une machine virtuelle avec Visual Studio mais nos applications ne sont toujours pas installées. Cela sera fait en ajoutant le [Custom Script Extension pour Windows](https://docs.microsoft.com/en-us/azure/virtual-machines/windows/extensions-customscript) à notre modèle. Nous prendrons l'exemple de schéma diponible sur de la page de documentation, et y apporterons quelque modification.

Le dernier noeud de notre modèle est actuellement une autre extension. Dans le but de ce blogue, supprimons-le. Vous devriez avoir quelque chose comme ceci.

![new Extension Place](/content/images/2018/04/newExtensionPlace.png)

Nous allons copier/coller l'extrait de la page de documentation et changer quelques petites choses. Changez le `type` (merci à notre extension VSCode de nous éviter cette erreur). Mettez à jour les dépendances pour refleter notre démo, en gardant que la `virtualMachines` et en y changeant le nom.

Un point à noter, est que pour utiliser cet extension, votre script doit être disponible en ligne. Il pourrait être ce trouver dans un Blob storage (avec une certaine sécurité) ou juste disponible publiquement. Dans notre cas, le script est accessible au public depuis la page gist.github. J'ai créé une variable dans la section des variables qui contient l'URL de mon script (version RAW), et une référence à ce varaibale est utilisée dans `fileUris`.

L'extension va télécharger le script, puis exécuter une commande localement. Changez le `commandToExecute` pour appeler notre script avec une politique d'exécution sans restriction (unrestricted execution policy).


> Vous avez une fenêtre d'environ 30 minutes pour exécuter votre script. Si cela prend plus de temps, votre déploiement échouera.


```json

{
    "apiVersion": "2015-06-15",
    "type": "extensions",
    "name": "config-app",
    "location": "[resourceGroup().location]",
    "dependsOn": [
        "[concat('Microsoft.Compute/virtualMachines/', 'FrankDevBox')]"
    ],
    "tags": {
        "displayName": "config-app"
    },
    "properties": {
        "publisher": "Microsoft.Compute",
        "type": "CustomScriptExtension",
        "typeHandlerVersion": "1.9",
        "autoUpgradeMinorVersion": true,
        "settings": {
            "fileUris": [
                "varaiables('scriptURL')]"
            ]
        },
        "protectedSettings": {
            "commandToExecute": "[concat('powershell -ExecutionPolicy Unrestricted -File ', './SimpleDevBox.ps1')]"
        }
    }
}

```

## Le Déploiment

Il est enfin temps de déployer notre VM. 

```bash
# Premièrement créons un groupe de ressources
az group create --name frankDemo --location eastus

# TOUJOURS, toujours, valider en premier ... vous économiserez beaucoup de temps
az group deployment validate --resource-group frankDemo --template-file /home/frank/Dev/DevBox/FrankDevBox.json

# Enfin déployer. Ce script devrait prendre entre 5 et 10 minutes
az group deployment create --name FrankDevBoxDemo --resource-group frankDemo --template-file /home/frank/Dev/DevBox/FrankDevBox.json --verbose

```

## La suite?!

Nous avons créé qu'un modèle vous pourriez le faire mieux.

### Déployer de n'importe où

En déplaçant `computerName`, `adminUsername`, `adminPassword`, et l'URL du script dans la section paramètres, vous pouvez ensuite placer le modèle dans un endroit public comme GitHub. Et vous pourriez utiliser le **déploiement en un seul clic**!

Que ce soit directement à partir de la page Github ou de n'importe où, vous avez uniquement besoin de construire une URL à partir de ces deux parties:
`https://portal.azure.com/#create/Microsoft.Template/uri/` et la version encodée HTML (HTML Encoded) de vos URL vers votre modèle.

Si mon modèle est disponible à `https://raw.githubusercontent.com/FBoucher/SimpleDevBox/master/azure-deploy.json` alors l'URL complète devient: `https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FFBoucher%2FSimpleDevBox%2Fmaster%2Fazure-deploy.json`

En cliquant sur cette URL, vous accédez au portail Azure (portal.azure.com) dans un formulaire personnalisé pour déployer votre modèle.

![DeployForm](/content/images/2018/04/DeployForm.png)

Ça ne peut pas être plus facile! Vous pouvez voir le code de ce post sur [GitHub](https://github.com/FBoucher/SimpleDevBox).

### Shutdown automatique

Il est très facile d'oublier d'éteindre ces machines virtuelles. Et peu importe que vous payiez pour ces VM ou que vous utilisiez vos précieux (et limité) crédits MSDN, c'est une très bonne pratique d'éteindre vos VM lorsqu'elles ne sont pas utilisées. Pourquoi ne pas le faire automatiquement?! 

Cela peut être très simplement fait en ajoutant une nouvelle ressource dans le modèle. Si vous désirez en savoir davantage sur les options disponibles vous pouvez aller consultez mon post [Économisez en éteignant automatiquement vos machines virtuelles (VM) tous les soirs](http://www.cloudenfrancais.com/economisez-en-eteignant-automatiquement-vos-machines-virtuelles-vm-tous-les-soirs/)

```json
{
    "name": "[concat('autoshutdown-', 'FrankDevBox')]",
    "type": "Microsoft.DevTestLab/schedules",
    "apiVersion": "2017-04-26-preview",
    "location": "[resourceGroup().location]",
    "properties": {
        "status": "Enabled",
        "taskType": "ComputeVmShutdownTask",
        "dailyRecurrence": {
            "time": "19:00"
        },
        "timeZoneId": "UTC",
        "targetResourceId": "[resourceId('Microsoft.Compute/virtualMachines', 'FrankDevBox')]",
        "notificationSettings": {
            "status": "Enabled",
            "emailRecipient": "frank@frankysnotes.com",
            "notificationLocale": "en",
            "timeInMinutes": "30"
        }
    },
    "dependsOn": [
        "[concat('Microsoft.Compute/virtualMachines/', 'FrankDevBox')]"
    ]
}

```

## En vidéo SVP!

Si vous préférez, j'ai une version vidéo de ce post.

**Comment créer une VM Azure avec Chocolatey**
<div class="container">
<iframe class="youtubevideo" width="560" src="https://www.youtube.com/embed/WTQAgI_qdWs" frameborder="0" allowfullscreen></iframe>
</div>

#### Références:

* [Chocolatey](https://chocolatey.org/)
* [Azure Resource Manager Tools](https://marketplace.visualstudio.com/items?itemName=msazurermtools.azurerm-vscode-tools)
* [Azure Resource Manager Snippets](https://marketplace.visualstudio.com/items?itemName=samcogan.arm-snippets)
* [Custom Script Extension for Windows](https://docs.microsoft.com/en-us/azure/virtual-machines/windows/extensions-customscript)
* [All scripts from this post](https://github.com/FBoucher/SimpleDevBox)





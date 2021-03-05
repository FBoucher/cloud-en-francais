---
title: Économisez en éteignant automatiquement vos machines virtuelles (VM) tous les soirs
permalink: /economisez-en-eteignant-automatiquement-vos-machines-virtuelles-vm-tous-les-soirs
Published: 2018-02-01
tags: [youtube, powershell, azure, post, arm, vm, cloud5minutes, devtestlab, cloud5min]
---

[*Mise-à-jour le 2018-03-14*](#Mise-à-jour)

Les machines virtuelles sont utilisées dans énormément de solutions de nos jours. Elles servent aussi de machine temporaire pour executer des tests, faire des démos et parfois même comme machine de développement. Un des grands avantages de l'infonagique (cloud) est que vous payez seulement pour ce que vous utilisez. Donc contrairement au serveur du bureau, vous n’arrêtez de payer quand vous éteignez vos machines virtuelles! Dans ce post, je vous explique comment faire avec vos machines existantes et aussi quoi faire pour toutes celles que vous allez créer dans le futur.

Vous préférez visionner une vidéo à la lecture? Aucun problème, sauter immédiatement à la section [Explication en vidéo svp](##Explication-en-vidéo-svp) de ce post. 

## Quand votre VM existe déjà

À partir un portail Azure, sélectionner la machine virtuelle que vous désirez modifier. Dans le panneau d'options, dans le section *OPERATIONS* cliquer sur *Auto-Shutdown*.

![Auto-Shutdown](/content/images/2018/02/auto-shutdown.png)

Pour activer l'option vous devez la mettre a `enabled`, sélectionner l'heure et le fuseau horaire. Une autre option très intéressante consiste a activer l'option pour recevoir une notification par courriel ou d'y accrocher un `webhook`.

![emailsample](/content/images/2018/02/emailsample.png)

<span style="color:#c25">*<a name="Mise-à-jour"></a>Mise-à-jour 2018-03-14*</span>

Plusieurs VMs qui existe déjà, pas de problème
----------------------------------------------

Évidament, si vous avez plusieurs machines virtuelles qui roulent ce n'est pas très efficace de changer leur configuration une à une via le portail.  Voici donc un petit script qui permet de modifier la configuration d'une grande quantité de VM d'un seul coup.


    '# Login-AzureRmAccount

    $Subscription = Get-AzureRmSubscription -SubscriptionName 'YOUR_SUBSCRIPTION_NAME'
    Select-AzureRmSubscription -Subscription $Subscription.Id

    $selectedVMs = Get-Azurermvm -ResourceGroupName cloud5mins
    foreach($vm in $selectedVMs) 
    { 
        $ResourceGroup = $vm.ResourceGroupName
        $vmName = $vm.Name
        $ScheduledShutdownResourceId = "/subscriptions/$Subscription/resourceGroups/$ResourceGroup/providers/microsoft.devtestlab/schedules/shutdown-computevm-$vmName"
    
        $Properties = @{}
        $Properties.Add('status', 'Enabled')
        $Properties.Add('targetResourceId', $vm.Id)
        $Properties.Add('taskType', 'ComputeVmShutdownTask')
        $Properties.Add('dailyRecurrence', @{'time'= 2100})
        $Properties.Add('timeZoneId', 'Eastern Standard Time')
        $Properties.Add('notificationSettings', @{status='Disabled'; timeInMinutes=60})

        New-AzureRmResource -Location $vm.Location -ResourceId $ScheduledShutdownResourceId -Properties $Properties -Force
    }

La variable $selectedVMs contient toutes les VMs que nous voulons modifier. Dans cet exemple ce sont celles contenues dans le resourceGroup cloud5mins. Il n’y a pas de limite à ce que vous pouvez sélectionner: OSType, tags, location, name, etc.

La variable $ScheduledShutdownResourceId est l'identifiant pour la config auto-shutdown. On remarque que le provider est microsoft.devtestlab.

Ensuite $Properties est une collection de propriétés status permet d'activer ou déactiver la config. targetResourceId est l'identifiant de la VM courante à modifier.

Reste plus qu'à spécifier l'heure et le fuseau horaire, et on créer une nouvelle ressource qui sera associée à une VM déjà existante.

<span style="color:#c25">*Fin de la mise-à-jour*</span>

Créer une VM avec l'option "auto-shutdown" 
-----------------------------------------

Il est possible de configurer l'opération de mise en veille automatique directement à partir du gabarit (template) ARM. Il suffit d'y intégrer une nouvelle ressource de type `Microsoft.DevTestLab/schedules`. Cette dernière était précédemment réservée aux environnements de "Dev Test Lab", mais est depuis peu disponible pour toutes les machines virtuelles.

Voici un exemple de noeud (un par VM) qui doit être ajouté à votre template.

    {
        "name": "[concat('autoshutdown-', parameters('virtualMachineName'))]",
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
            "targetResourceId": "[resourceId('Microsoft.Compute/virtualMachines', parameters('virtualMachineName'))]",
            "notificationSettings": {
                "status": "Enabled",
                "emailRecipient": "frank@frankysnotes.com",
                "notificationLocale": "en",
                "timeInMinutes": "30"
            }
        },
        "dependsOn": [
            "[concat('Microsoft.Compute/virtualMachines/', parameters('virtualMachineName'))]"
        ]
    }

Quelque point qui mérite un peu plus d'attention, le statut doit être "Enabled" pour être actif. Bien valider votre fuseau horaire, et l'ajuster selon vos besoins. Vous devez ajouter une ressource `Microsoft.DevTestLab/schedules` par VM que vous voulez géré en spécifiant la `resourceid` de la VM dans `targetResourceId`. SI vous désirez une notification, il vous suffit de l'activer et de la configurer dans la section `notificationSettings`.

## Explication en vidéo svp

#### Comment éteindre automatiquement une VM dans Azure 

[Dans cette capsule de Cloud en 5 minutes](https://youtu.be/eESwfp31b0Q), j'explique comment faire modifier une machine virtuelle (VM) existante pour qu'elle s'éteigne tous les soirs et aussi quoi faire pour toutes celles que vous aller créer dans le futur en améliorant votre ARM template.

<div class="container">
<iframe class="youtubevideo" width="560" src="https://www.youtube.com/embed/eESwfp31b0Q" frameborder="0" allowfullscreen></iframe>
</div>

#### Comment éteindre automatiquement toutes vos VMs déjà existantes 

[Dans cette seconde capsule de Cloud en 5 minutes](https://youtu.be/b-dt3epFGaw), je vous montrerai comment éteindre automatiquement toutes vos VMs déjà existantes en utilisant un script PowerShell très simple.

<div class="container ">
<iframe class="youtubevideo" width="550"  src="https://www.youtube.com/embed/b-dt3epFGaw" frameborder="0" allowfullscreen></iframe>
</div>

Abonnez-vous: http://bit.ly/2jx3uKX
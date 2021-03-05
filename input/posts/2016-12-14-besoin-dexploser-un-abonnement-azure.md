---
title: Besoin d'exploser un abonnement Azure?
permalink: /besoin-dexploser-un-abonnement-azure
Published: 2016-12-14
tags: [cloud, powershell, azure, arm, resourcemanager]
---

J'utilise très intensément mon abonnement my.visualstudio (alias MSDN) Azure, pour créer du contenu que ce soit pour une démo ou pour essayer de nouvelle fonctionnalité. C'est pourquoi fréquemment, j'ai besoin de faire du nettoyage.

![boom](/content/images/2016/12/multi-resourceGroup-boom.png)

Voici un petit script qui supprimera complètement toutes les ressources de chaque groupe de ressources d'un abonnement précis. Pour pouvoir exécuter ce script, vous aurez besoin des [applets de commande PowerShell Azure](https://docs.microsoft.com/en-us/powershell/azureps-cmdlets-docs/).

Le script vous demandera de vous connecter puis listera toutes les abonnements auxquels ce compte a accès. Une fois que vous aurez spécifié lequel vous désirez détruire, il listera toutes les ressources regroupées par groupe de ressources. Ensuite, tel un dernier avertissement, il faudra une dernière validation avant tout "nuker".

Jouer prudemment.


``` powershell


# Login
Login-AzureRmAccount 

# Get a list of all Azure subscript that the user can access
$allSubs = Get-AzureRmSubscription 

$allSubs | Sort-Object Name | Format-Table -Property Name, SubscriptionId, State

$theSub = Read-Host "Enter the subscriptionId you want to clean"

Write-Host "You select the following subscription. (it will be display 15 sec.)" -ForegroundColor Cyan
Get-AzureRmSubscription -SubscriptionId $theSub | Select-AzureRmSubscription 

#Get all the resources groups

$allRG = Get-AzureRmResourceGroup

foreach ( $g in $allRG){

    Write-Host $g.ResourceGroupName -ForegroundColor Yellow 
    Write-Host "------------------------------------------------------`n" -ForegroundColor Yellow 
    $allResources = Find-AzureRmResource -ResourceGroupNameContains $g.ResourceGroupName
    
    if($allResources){
        $allResources | Format-Table -Property Name, ResourceName
    }else{
         Write-Host "-- empty--`n"
    } 
    Write-Host "`n`n------------------------------------------------------" -ForegroundColor Yellow 
}

$lastValidation = Read-Host "Do you wich to delete ALL the resouces previously listed? (YES/ NO)"

if($lastValidation.ToLower().Equals("yes")){

    foreach ( $g in $allRG){

        Write-Host "Deleting " $g.ResourceGroupName 
        Remove-AzureRmResourceGroup -Name $g.ResourceGroupName -Force -WhatIf

    }
}else{
     Write-Host "Aborded. Nothing was deleted." -ForegroundColor Cyan
}


```


Le code est aussi disponible sur Github: https://github.com/FBoucher/AzurePowerTools



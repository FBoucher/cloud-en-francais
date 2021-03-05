---
title: Astuces rapides pour économiser avec les machines virtuelles Windows Azure
permalink: /astuces-rapides-pour-economiser-avec-les-machines-virtuelles-windows-azure
Published: 2014-07-10
tags: [ vm, script, powershell, machine virtuelle,  cloud, astuce, azure]
---

_(Ce billet a été initialement publié sur le [blog de Matricis](http://matricis.com/billet-technique/astuces-rapides-pour-economiser-avec-les-machines-virtuelles-windows-azure/))_

_(version francaise de: [Quick Tips:How to save money with your VM](http://www.frankysnotes.com/2014/07/quick-tipshow-to-save-money-with-your-vm.html))_

Au bureau, nous utilisons fréquemment des machines virtuelles (VM) pour notre développement ou nos tests. C’est très utile de pouvoir créer rapidement plusieurs VM et de pouvoir en disposer lorsque la tâche est terminée. Cependant, comme nous avons besoin de ces machines pendant plusieurs semaines, ne serait-il pas merveilleux de pouvoir économiser pendant cette période? Dans ce billet je vous expliquerai des astuces pour y parvenir.

## Astuce #1
La première astuce consiste à fermer les VM que vous n’utilisez pas. Pourquoi payer pour une machine virtuelle 24/7 si vous ne l’utilisez qu’entre 9 h et 17 h cinq jours semaine? Une façon de le faire est de vous connecter au portail de gestion de Windows Azure. Une fois connecté, sélectionnez la liste des machines virtuelles. Pour chaque machine que vous voulez fermer, vous devez tout d’abord la sélectionner et ensuite cliquer sur le bouton "Arrêter" en bas au centre de l’écran.

Si vous avez plusieurs machines que vous devez régulièrement ouvrir et fermer, cette tâche deviendra vite très ennuyeuse. Un script PowerShell est donc l’outil par excellence pour cette tâche. Voici un exemple de script :

####Pour démarrer une machine virtuelle


	#Full path of the publish Setting file downloaded from Azure.
	$NameOfSettingFile = ".\MySettings.publishsettings"
	
	# The name of the machine to get started
	$VMName = "dev2"
	
	# Azure Subscription Name
	$SubscriptionName = "Frank Dev"
	
	cls
	
	if(!(Test-Path $NameOfSettingFile)){
	    echo "Download the Publishing Setting files, and re-run the script."
	    Get-AzurePublishSettingsFile
	    exit
	}
	
	Import-AzurePublishSettingsFile  $NameOfSettingFile
	
	Select-AzureSubscription -SubscriptionName $SubscriptionName
	
	Write-Host "Starting the VM $VMName" -ForegroundColor Cyan
	Start-AzureVM -Name $VMName -ServiceName $VMName
	
	do{
	    Start-Sleep –Seconds 2
	    $vmInfo = Get-AzureVM -ServiceName $VMName -Name $VMName
	    $vmStatus = $vmInfo.InstanceStatus
	    Write-Host "The VM $VMName is still $vmStatus..."
	}
	while($vmStatus -ne "ReadyRole")
	
	Write-Host "The VM $VMName is now accessible by Remote Connection." -ForegroundColor Green
	
	read-host "Press enter key to close"



####Pour éteindre une machine virtuelle

	
	#Full path of the publish Setting file downloaded from Azure.
	$NameOfSettingFile = ".\MySettings.publishsettings"
	
	# The name of the machine to get started
	$VMName = "dev2"
	
	# Azure Subscription Name
	$SubscriptionName = "Frank Dev"
	
	cls
	
	if(!(Test-Path $NameOfSettingFile)){
	    echo "Download the Publishing Setting files, and re-run the script."
	    Get-AzurePublishSettingsFile
	    exit
	}
	
	Import-AzurePublishSettingsFile  $NameOfSettingFile
	
	Select-AzureSubscription -SubscriptionName $SubscriptionName
	
	Write-Host "Stop the VM $VMName" -ForegroundColor Cyan
	Stop-AzureVM -Name $VMName -ServiceName $VMName -Force
	
	Write-Host "The VM $VMName is now shuting down." -ForegroundColor Green
	read-host "Press enter key to close"




## Astuce #2

La seconde astuce est applicable seulement pour les machines virtuelles qui n’ont pas besoin de la mise à l’échelle automatique (auto-scaling) ou l’équilibrage de charge (load balancer). Une machine qui sert seulement au développement, par exemple, est un cas parfait. Il suffit de modifier le niveau de la machine virtuelle au niveau de base. Ceci est une nouvelle fonctionnalité disponible depuis mai 2014. Toutes les machines créées précédemment sont au niveau standard. L’astuce est donc de changer le niveau de la machine virtuelle pour "de base", ce qui affectera le taux de facturation. Vous pouvez aller sur [Tarification – Machines virtuelles](http://azure.microsoft.com/fr-fr/pricing/details/virtual-machines/) pour voir plus de détails. Par exemple, pour une machine virtuelle large (A4) l’économie représente 40 $. Intéressé? Le changement est très facile à effectuer et se fait à partir du portail.

![Etapes pour astuce 2](/content/images/2014/Sep/Quick_tip2_fr.png)

Premièrement, si ce n’est pas déjà fait, connectez-vous au portail de gestion Microsoft Azure. Ensuite, dans la liste des machines virtuelles, sélectionnez la machine que vous désirez modifier. Cliquez sur l’onglet Configurer et modifiez le Niveau de la machine virtuel à: de base. Si la machine virtuelle est actuellement active, vous aurez un message vous informant que la machine doit maintenant redémarrer, n’oubliez donc pas de sauvegarder tout travail en court.

J’espère que ces petites astuces rapides pourront vous aider. N’hésitez pas à me faire par de vos idées, je les ajouter.




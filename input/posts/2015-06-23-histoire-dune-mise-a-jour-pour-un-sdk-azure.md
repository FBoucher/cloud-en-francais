---
title: Histoire d'une mise à jour pour un SDK Azure
permalink: /histoire-dune-mise-a-jour-pour-un-sdk-azure
Published: 2015-06-23
tags: [powershell, api, deployment, rest, upgrade, diagnostic, azure, sdk]
---

<i style="font-size: 0.8em">NDLR : Ce post est aussi disponible en anglais sur le [blog de Microsoft MVP Award](http://blogs.msdn.com/b/mvpawardprogram/archive/2015/06/23/the-journey-of-an-azure-sdk-upgrade.aspx "The Journey of an Azure SDK Upgrade" target="_blank"), dans le cadre de notre série mardi technique.</i>

<img src="/content/images/2015/06/Journey-1.png" width="200px" height="200px" style="background-image: none; border-bottom: 0px; border-left: 0px; border-right: 0px; border-top: 0px; display: inline; float: right; padding-left: 10px; padding-right: 10px; padding-top: 10px; padding-bottom: 10px;"/>

Récemment, avec mon équipe, nous avons eu besoin de mettre à niveau une solution web d’Azure SDK 2,4 à 2.5.1. La mise à niveau a été beaucoup plus longue et complexe que prévue, donc j'ai donc décidé de partager ce que nous appris, d'autres pourront bénéficier de notre expérience.


Le problème
-----------

La mise à niveau du code et des librairies n'a pas vraiment été un problème. La documentation est disponible sur MSDN et est facile à suivre. Quelques changements drastiques (*breaking changes*) font partie de la version 2.5. L'un d'eu, est la raison de ce billet: 
La configuration des diagnostics doit être appliquée séparément après le déploiement. Une fois de plus, la documentation sur MSDN est très utile, suite au déploiement du service, nous avons seulement besoin d'exécuter un script PowerShell:

<pre><code>
$storage_name = "wadexample"
$key = "_StorageAccountKey_"
$config_path = "c:\users\_user_\documents\visual studio 2013\Projects\WadExample\WorkerRole1\WadExample.xml"
$service_name = "wadexample"
$storageContext = New-AzureStorageContext -StorageAccountName $storage_name -StorageAccountKey $key 

Set-AzureServiceDiagnosticsExtension -StorageContext $storageContext -DiagnosticsConfigurationPath $config_path -ServiceName $service_name -Slot Staging -Role WorkerRole1

</code></pre>


Cependant, lorsque nous avons exécuté le script, nous avons eu une erreur pour l'un des rôles.

> Set-AzureServiceDiagnosticsExtension : BadRequest: The extension ID FakeVeryLongNameViewProcessorWorkerRole-PaaSDiagnostics-Production-Ext-0 is invalid.

Le problème est fort simple; l'identifiant d'extension (*Extension ID*) généré est trop long. Ce bogue est bien connu et est identifié dans le projet azure-PowerShell sur Github. 
Nous pourrions simplement renommer le rôle, mais cette solution n'est une bonne option dans notre contexte.


La solution
-----------

Nous avons donc décidé d'appeler directement l'API REST d'Azure. Et c’est à ce moment-là où les choses sont devenues un peu moins évidentes. La documentation était certe présente, mais moins précise.

<pre><code>
# == Add Extension =====================
$publishConfigBase64 = Convert-StringToBase64 $PublicConfig
$privateConfigBase64 = Convert-StringToBase64 $PrivateConfig
  
$extensionBody = ('<Extension xmlns= http://schemas.microsoft.com/windowsazure xmlns:i="http://www.w3.org/2001/XMLSchema-instance"><ProviderNameSpace>Microsoft.Azure.Diagnostics</ProviderNameSpace><Type>PaaSDiagnostics</Type><Id>{0}</Id><PublicConfiguration>{1}</PublicConfiguration><PrivateConfiguration>{2}</PrivateConfiguration><Version>1.*</Version></Extension>' -f $ExtensionId, $publishConfigBase64, $privateConfigBase64)

$response = Invoke-WebRequest -Method Post -Uri('https://management.core.windows.net/{0}/services/hostedservices/{1}/extensions' -f $SubscriptionId, $ServiceName) -Body $extensionBody -Headers $Headers -CertificateThumbprint $CertificateThumbprint -Verbose -ContentType $ContentType

# == Upgrade Deployment ================
$serviceConfigurationBase64 = Convert-StringToBase64  $ServiceConfiguration

$deploymentConfiguration = ('<?xml version="1.0" encoding="utf-8"?><ChangeConfiguration xmlns="http://schemas.microsoft.com/windowsazure"><Configuration>{0}</Configuration><ExtensionConfiguration><NamedRoles>{1}</NamedRoles></ExtensionConfiguration></ChangeConfiguration>' -f $serviceConfigurationBase64, $RolesExtensionConfiguration)

$response = Invoke-WebRequest -Method Post -Uri('https://management.core.windows.net/{0}/services/hostedservices/{1}/deploymentslots/{2}/?comp=config' -f $SubscriptionId, $ServiceName, $Slot) -Body $deploymentConfiguration -Headers $Headers -CertificateThumbprint $CertificateThumbprint -Verbose -ContentType $ContentType

</code></pre>	
       

En fait, la solution était simple. Tout d'abord, nous avions besoin d'ajouter l'extension appelant la méthode POST. Dans cet appel que vous passez le fichier `WadCfg.xml` et nous pouvons spécifier un nom "plus court" comme ` ExtensionId`. Ensuite, invoquant une autre méthode POST, nous mettons à jour la configuration du déploiement.

Un petit extra
--------------

Notre solution web contient de nombreux rôles. Boucler au travers des rôles, un par un prenait trop de temps comparativement au laps de temps accordé pour un déploiement dans un environnement de production. Nous avons refactoriser le script PowerShell pour ajouter toutes les extensions et à la fin modifier qu'une fois seulement la configuration du déploiement. De cette façon, l'exécution a été trois fois plus rapide.


##### Références

- [Notes de publication du Kit de développement logiciel (SDK) Azure pour .NET 2.5](http://msdn.microsoft.com/library/azure/dn873976.aspx/)
- [Notes de publication du Kit de développement logiciel (SDK) Azure pour .NET 2.5.1](http://azure.microsoft.com/fr-fr/documentation/articles/app-service-release-notes/)
- [Github - azure-powershell - issues #387](http://github.com/Azure/azure-powershell/issues/387)
- [Référence de l'API REST de gestion des services](http://msdn.microsoft.com/library/azure/ee460799.aspx/)



---
title: Tout ce qu'on doit connaitre sur AUBI, le nouveau portail sur l'utilisation et facturation d'Azure
permalink: /tout-ce-quon-doit-connaitre-sur-aubi-le-nouveau-portail-sur-lutilisation-et-facturation-dazure
Published: 2016-08-04
tags: [cloud, azure, post, arm, portal, github, facturation, aubi]
---

Si une image vaut mille mots, alors c'est incroyable la quantité d'informations que vous trouverez dans AUBI, Azure Usage And Billing ou Utilisation et facturation. Ce portail est un projet open-source qui a été [annoncé][Announcing] il y a quelques semaines. Dans ce billet, je partagerai mes premières impressions à son sujet.

![Aubi Portal](/content/images/2016/08/Portal.png)

Le projet est encore jeune, mais très vivant. Lors de mon installation, j'ai eu un ou deux problèmes mineurs, mais le temps que je termine d'écrire ce billet tout était déjà corrigé.

Où le trouve-t-on?
----------------

Le site d'Azure Usage And Billing n'est pas un site déjà en ligne tel que portal.azure.com. En fait, il s'agit d'une solution que vous devez déployer dans un compte Azure. Il n'est pas nécessaire que ce soit le compte que vous souhaitez surveiller, seulement un compte, auquel avez accès.
La solution complète contient: deux sites web,  d'on un avec des tâches web (webjobs) et leur Application Insights respectifs, une base de données SQL, un compte de stockage et vous aurez également besoin de déployer un rapport Power BI.

![resourcesgroup](/content/images/2016/08/resourcesgroup.png)

Le tout est facilement déployable en utilisant le script PowerShell et le modèle de groupe de ressources (ARM) inclus. Seules quelques étapes manuelles seront nécessaires. Une documentation très claire et complète est également disponible en vidéo et écrit. Vous trouverez les deux versions sur la [page AzureUsageAndBillingPortal de Github][AzureUsageAndBillingPortal].

Que puis-je faire avec?
-----------------------

Une fois complètement déployé, vous aurez besoin d’accéder à votre instance du portail d’inscription (ex.: http://frankregistrationv12.azurewebsites.net) et d’enregistrer tous les abonnements que vous y souhaitez. Une fois que les tâches web auront terminé d’importer toutes les données de vos comptes, ils seront tous disponibles dans votre rapport Power BI.

Les incroyables capacités de Power BI ont un impact majeur sur toutes les informations affichées relatives à vos comptes. Effectivement, toutes les données présentes dans le tableau de bord sont interactives! Que vous sélectionniez un seul ou plusieurs comptes ou seulement une catégorie de service Azure spécifiquement, toutes les autres tuiles du tableau seront ajustées automatiquement.

![PowerBI_interactive](/content/images/2016/08/Aubi_800.gif)

Et ensuite?
-----------

Si ce n’est pas encore fait, je vous recommande fortement d’installer le portail AUBI et commencer à profiter de toutes les informations détaillées à votre disposition, sans aucun effort, et si bien présentées. Pour tous les détails sur les prérequis ou la procédure d’installation, aller sur la [page du projet Github][AzureUsageAndBillingPortal].

##### Référence: 

- [Announcing the release of the Azure Usage and Billing Portal][Announcing]
- [Git AzureUsageAndBillingPortal][AzureUsageAndBillingPortal]



[Announcing]: https://azure.microsoft.com/en-us/blog/announcing-the-release-of-the-azure-usage-and-billing-portal/
[AzureUsageAndBillingPortal]: https://github.com/Microsoft/AzureUsageAndBillingPortal


[AubiPortal]: /../images/2016-08-01/Portal.png
[resourcesgroup]: /../images/2016-08-01/resourcesgroup.png
---
title: Des statistiques et Application Insight pour un blogue Ghost dans Microsoft Azure
permalink: /des-statistiques-pour-blogue-dans-microsoft-azure
Published: 2015-03-10
tags: [website, kudu, Application Insight, metrics, extension, MicrosoftAzure, ghost, azure]
---

Avoir des statistiques pour une application Asp.Net, il n’y a rien de plus simple. Qu'en est-il pour une application en Node.js sous Microsoft Azure? Dans ce billet, je vais vous expliquer comment ajouter des statistiques et *Application Insight* pour un blogue Ghost qui roule dans Microsoft Azure.


Demande et Erreurs
------------------

Lorsque vous créez un site Web dans Azure, vous avez automatiquement un accès à une surveillance (monitoring). Pour voir de quoi il s'agit, allez sur [portal.azure.com](http://portal.azure.com/) (le nouveau portail Azure) et sélectionnez votre site.

![fr_Demande_et_erreur.png](/content/images/2015/Mar/fr_Demande_et_erreur.png)

Dans cette section, vous pourrez voir combien de demandes et d'erreurs sont survenues pendant une période donnée. Vous pouvez également créer des alertes, en cliquant sur le signe "+". Par exemple, vous pourriez être informé par courriel si le nombre de demandes est supérieur à *x*, au cours de la dernière heure.


Analyse
-------

La première fois que vous cliquez sur la section d'Analyse, des instructions vous y seront présentées pour vous permettre d'ajouter les outils nécessaires afin de collecter les données concernant votre site.

> Pour collecter les données analytiques relatives à l'utilisateur final de votre application,
> insérez le script suivant dans chaque page dont vous souhaitez effectuer le suivi.
> Placez ce code juste avant la balise </headde fermeture,
> et avant tout autre script. Vos premières données apparaîtront
> automatiquement dans quelques secondes.

Il s'agira donc de modifier le fichier `default.hbs`. Ce dernier, si vous utilisez le thème par défaut se trouve ici: `/content/themes/casper/default.hbs`.  Sinon, il suffit de remplacer "casper" par le bon nom de thème.

![fr_tour_analyse.gif](/content/images/2015/Mar/fr_tour_analyse2.gif)

Chaque graphique vous donnera accès à plusieurs métriques, qui sont toutes paramétrisable facilement.


Application Insights
--------------------

Pour encore plus de statistique et d'information, je vous suggère d'installer l'extension Application Insights. Elle vous donnera encore plus d'information sur l'état du serveur et permettra même de créer des événements personnalisés en utilisant [applicationinsights package de node.js via npm](http://www.npmjs.com/package/applicationinsights)

Il est possible d'ajouter l'extension directement à partir de la "blade" de votre site. Cliquer sur `All Settings -->`, situé en haut de la page.  Ensuite, sélectionner `Extensions` et enfin cliquer `Ajouter`. 

![fr_Ajouter_extension_via_blade.png](/content/images/2015/Mar/fr_Ajouter_extension_via_blade.png)

Ajouter Application Insights Extension.

![Application_Insights_Extension.png](/content/images/2015/Mar/Application_Insights_Extension.png)

Vous pouvez aussi utiliser l'interface Kudu. Cette interface, méconnue des développeurs Azure, contient plusieurs outils et est accessible facilement à partir du navigateur.  L'URL est le même que celui du site, mais on doit insérer "scm" entre le nom du site et le nom de domaine. 
Par exemple:  http:// monsite**.scm**.azurewebsites.net

![From_Kudu_Console2015-03-01_2044.png](/content/images/2015/Mar/From_Kudu_Console2015-03-01_2044.png)

Une fois connecté dans Kudu, cliqué sur le dernier onglet `Extensions`, et sur la section `Gallery`. Toutes les extensions disponibles seront affichées, sélectionner l'extension `Application Insights Extension` pour l'installer. 

![fr_Application_Insigt.png](/content/images/2015/Mar/fr_Application_Insigt.png)

Afin de pouvoir profiter des nouvelles informations recueillies par Application Insights vous devrez redémarrer le site. 





#### Références

- [Microsoft Azure](http://azure.microsoft.com/)
- [Ghost](http://ghost.org/)
- [Application Insights Extension](http://azure.microsoft.com/en-us/documentation/articles/app-insights-get-started/)
- [applicationinsights package (npm)](http://www.npmjs.com/package/applicationinsights)
- [Kudu](http://github.com/projectkudu/kudu)



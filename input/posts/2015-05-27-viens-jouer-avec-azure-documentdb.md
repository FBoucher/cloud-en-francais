---
title: Viens jouer avec Azure DocumentDB
permalink: /viens-jouer-avec-azure-documentdb
Published: 2015-05-27
tags: [documentdb, nosql, database, json, sql, MicrosoftAzure, azure]
---

DocumentDB est une base de données non relationnelle (NoSQL) sous forme de service (SaaS), qui opère avec des documents.  Ajouté à l'environnement Windows Azure en 2014, ce service est hautement disponible, change d'échelle rapidement (scalable) et simple à utiliser puis qu'il utilise le langage SQL.  Ce n'est plus une surprise pour personne, MI Azure évolue sans cesse et DocumentDB n'y échappe pas. Tout récemment, un nouvel outil s’est ajouté nous permettant de tester nos requêtes sans effort. Ce billet est donc une introduction à Query Playground, une nouvelle interface de DocumentDB.

![](/content/images/2015/05/Playground_500.png)

## Query Playground

Query Playground est une démo interactive. Cette page web vous permettra de vous familiariser à la sémantique des requêtes SQL en interrogeant les données du département américain de l'agriculture (USDA).

Un exemple de donnée est offert pour mieux comprendre le schéma:

	{
	  "id": "19015",
	  "description": "Snacks, granola bars, hard, plain",
	  "tags": [
	    {
	      "name": "snacks"
	    },
	    {
	      "name": "granola bars"
	    },
	    {
	      "name": "hard"
	    },
	    {
	      "name": "plain"
	    }
	  ],
	  "version": 1,
	  "isFromSurvey": false,
	  "foodGroup": "Snacks",
	  "servings": [
	    {
	      "amount": 1,
	      "description": "bar",
	      "weightInGrams": 21
	    },
	    {
	      "amount": 1,
	      "description": "bar (1 oz)",
	      "weightInGrams": 28
	    },
	    {
	      "amount": 1,
	      "description": "bar",
	      "weightInGrams": 25
	    }
	  ]
	}

## Tutoriel

Divisé en plusieurs onglets, Query Playground vous explique comment bâtir vos requêtes. DocumentDB est très puissant il support plusieurs fonctionnalités standards telles que les filtres, les jointures, le triage et même le reformatage du résultat dans un nouveau format et les fonctions définies par l'utilisateur.


## Encore plus

La page est mise à jour régulièrement affichant les dernières nouveautés de DocumentDB suite au commentaire laisser par les usagés. C'est donc une page a visité et revisité pour tester vos requêtes et avoir un aperçu de ce qui s'en vient!  Et pour tester vos requêtes sur vos propres données, profiter de la période d'[évaluation d'un mois gratuite](http://azure.microsoft.com/fr-fr/pricing/free-trial/).

![Onglets du tutoriel](/content/images/2015/05/DocumentDBPlayground_tabs_2.gif)



##### Références

- [DocumentDB](http://azure.microsoft.com/fr-fr/services/documentdb/)
- [Query Playground](http://www.documentdb.com/sql/demo)


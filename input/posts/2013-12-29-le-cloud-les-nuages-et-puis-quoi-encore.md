---
title: Le cloud, les nuages et puis quoi encore
permalink: /le-cloud-les-nuages-et-puis-quoi-encore
Published: 2013-12-29
tags: [introduction,  cloud, infonuagique, azure]
---


Aujourd'hui le mot "Cloud" sur toutes les lèvres. A écouter certains, tout porte à croire que le cloud ou le "nuage" est la solution à tous les maux. Mais qu'en est-il vraiment.

##Infonuagique
L'infonuagique est la traduction de cloud computing. Plusieurs grandes compagnies proposent leur solution. Pour en nommer que quelque une on retrouve:

- Microsoft: Windows Azure
- Amazon: Amazon Web Services (AWS)
- Google: AppEngine
- Salesforces
- Etc.

## Pourquoi?

La particularité et la force du cloud est sa capacité et répondre à la fluctuation de la demande. Pour ce faire, les unités de calcul n'ont pas besoin d'être puissante, puisque l'idée est d'ajouter où d'enlever des unités de calculs en fonctionne de la demande.

### Exemple graphique de la variation de la demande.

![](/content/images/2014/Sep/Variation_de_la_demande.png)

1. Imaginons un commerce ayant un site transactionnel. Il y a des moments où la demande est plus forte: heure ouvrable, fin ou début de mois, fin de l'année, etc. Il y a aussi des moments où la demande est plus faible voir nulle: nuit, journée fériée, saison, etc.
2. Vous avez des serveurs où sont exécutés des services utilisés par vos employées. Ces machines sont donc très sollicitées pendant le jour et plus du tout durant la nuit et probablement les fins de semaine.
3. Une entreprise lance un nouveau produit, ou une nouvelle version d'un produit. Son site web est soudainement très achalandé.  Après une certaine période (semaine, mois), votre site retrouve son rythme normal.

![Image from Cloud Architecture Patterns by Bill Wilder](/content/images/2014/Sep/Auto_Scaling2013_12_08_1204.png)

Normalement, afin d'être capable de répondre adéquatement à la demande, il faudrait acheter des serveurs qui sont en mesure de supporter la demande à son apogée.  Cependant, qu'adviendra-t-il de ces machines une fois la demande passée? 
Rien. 

La différence avec le cloud est que vous êtes facturé pour ce que vous utilisez... à l'heure ! Vos serveurs roulent 40 heures par semaine, vous serez facturé 40 heures. De plus, le temps de réaction est très court. Augmenté la capacité de réponse de votre site peu se faire en quelque minute et sans aucune intervention de votre part. Une nouvelle machine virtuelle peut être disponible en moins de 10 minutes.

## Conclusion
En résumé, le cloud n'est pas une nouvelle technologie, mais plutôt une nouvelle façon d'utiliser la technologie. Bien que l'utilisation du cloud ne soit pas nécessaire dans toutes les situations, sa facilité  d'utilisation le rend très populaire, sans parler des possibilités d'économiser.



####Références
- <a href="http://shop.oreilly.com/product/0636920023777.do" target="_blank">Cloud Architecture Patterns</a> 
- <a href="http://www.windowsazure.com/fr-fr/overview/what-is-windows-azure/" target="_blank">Windows Azure</a> 
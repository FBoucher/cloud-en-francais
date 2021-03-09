---
title: "Comment copier des données dans Azure avec AZCopy" 
Published: 2021-02-25
categories: post-fr
featured-image: https://img.youtube.com/vi/3AKC_IvsQxE/default.jpg
Image: https://img.youtube.com/vi/3AKC_IvsQxE/default.jpg
tags: [azcopy,azure,cloud,post,dotnet,readingnotes,video]
---

Les données sont la clé de presque toutes les solutions. Évidemment, à un moment donné, nous devrons les déplacer. Et c'est à ce moment qu'[AzCopy](https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azcopy-v10?WT.mc_id=cloudenfrancais-blog-frbouche) viendra à la rescousse. Dans ce court vidéo, je vais partager comment vous pouvez copier en toute sécurité un fichier Zip (aka. nos données), d'un emplacement (stockage blob, AWS) vers un stockage blob dans un abonnement Azure (le même abonnement ou un autre).

## Qu'est-ce qu'AzCopy
AzCopy est un utilitaire de ligne de commande que vous pouvez utiliser pour copier des objets blob ou des fichiers vers ou depuis un compte de stockage. Il peut fonctionner sous Windows, Mac et Linux. Et ... il est déjà disponible en pré-installation dans Cloud Shell!


<iframe width="560" height="315" src="https://www.youtube.com/embed/3AKC_IvsQxE" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>

#### Références

- AzCopy: [https://aka.ms/azcopy](https://aka.ms/azcopy)
- [Az CLI Generate SAS token](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-blob-user-delegation-sas-create-cli?WT.mc_id=cloudenfrancais-blog-frbouche)
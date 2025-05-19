---
title: "Azure Developer CLI (azd) dans un scénario réel"	 
Published: 2025-04-24
categories: post
tags: [azure, azure container apps, aca, azd, azure developer cli, iac, bicep, post]
---

Dans mes articles précédents, j'ai partagé mon expérience de migration d'[AzUrlShortener](https://github.com/microsoft/AzUrlShortener). Aujourd'hui, je veux me concentrer sur l'Azure Developer CLI (azd). Cet outil m'a aidé à déployer rapidement et facilement sur Azure sans avoir à écrire de scripts ou de code d'infrastructure (IaC) manuellement.

Dans cet article, je partagerai quelques conseils utiles et les leçons que j'ai apprises en utilisant `azd` pendant la migration.

> **💡 Obtenez le code!** <br/>
> Vous pouvez trouver le code pour cet article dans le dépôt GitHub [AzUrlShortener](https://github.com/microsoft/AzUrlShortener).

Partie de cette série:
- [Migration du project AzUrlShortener d'Azure Static WebApp vers Azure Container Apps](https://www.cloudenfrancais.com/posts/2025-04-22-urlshortener-architecture.html)
- [Conversion d'un projet Blazor WASM en FluentUI Blazor Server](https://www.cloudenfrancais.com/posts/2025-04-23-from-blazor-wasm-to-fluentui-blazor-server.html)
- Azure Developer CLI (azd) dans un scénario réel

## Génération des fichiers Bicep

La version précédente d'AzUrlShortener, écrite il y a quelques années, utilisait des modèles ARM. Il était temps de réécrire l'IaC en utilisant quelque chose de plus récent, comme Bicep. Comme la solution comporte plusieurs composants (une Azure Function, une API, un site web et un compte de stockage), je m'attendais à devoir écrire beaucoup de code Bicep. Cependant, j'ai été surpris que `azd` puisse générer tous les fichiers Bicep pour moi.

En utilisant la commande `azd infra synth`, actuellement en aperçu, j'ai pu sauvegarder localement les fichiers IaC. Grâce au manifeste de .NET Aspire, `azd` savait ce qui était nécessaire et a généré plusieurs nouveaux fichiers dans la solution:

```yaml
infra/                  # Fichiers d'infrastructure as Code (Bicep)
 ├── main.bicep         # Module de déploiement principal
 └── resources.bicep    # Ressources partagées entre les services de votre application
```

De plus, pour chaque ressource de projet référencée par votre AppHost, un fichier `containerApp.tmpl.yaml` a été créé dans un répertoire `infra` sous le projet AppHost:

```yaml
Cloud5mins.ShortenerTools.AppHost/
    └── infra/
        ├── admin.tmpl.yaml
        ├── api.tmpl.yaml
        └── azfunc-light.tmpl.yaml
```

Maintenant, je pouvais modifier ces fichiers Bicep (par exemple, changer les paramètres de mise à l'échelle ou de CPU pour chaque conteneur). Chaque fois que j'exécutais `azd up`, l'IaC était utilisé pour déployer la solution.

> *Remarque*: `azd infra synth` est actuellement une fonctionnalité alpha. Vous devez l'activer explicitement en exécutant `azd config set alpha.infraSynth on`.

## Écriture et réécriture des fichiers IaC

Lors d'une migration, nous déplaçons souvent la solution pièce par pièce. Dans mon cas, j'ai commencé par utiliser un Azure Table Storage existant. Une fois intégré dans la solution, mon déploiement a échoué. Cela s'est produit parce que le compte de stockage n'avait pas été créé par `azd`, et les fichiers Bicep n'en avaient pas connaissance. J'aurais pu ajouter manuellement le compte de stockage, mais j'ai plutôt utilisé la commande `azd infra synth --force`. Cette commande écrase les fichiers Bicep existants et les régénère en fonction de l'état actuel de vos ressources Azure. En utilisant git, je pouvais facilement voir les modifications apportées aux fichiers et résoudre les conflits si je les avais modifiés manuellement.

## Laisse mon nom de domaine tranquille!

L'une des dernières étapes consistait à tester la solution avec un nom de domaine personnalisé. J'ai exécuté `azd up`, ajouté un nom de domaine au conteneur `azfunc-light` via le portail Azure, et effectué quelques tests. Tout fonctionnait bien, bien que j'aie trouvé quelques petits problèmes à corriger. Après avoir rapidement corrigé ces problèmes, j'ai réexécuté `azd up`. Mais cette fois, oh non! Le nom de domaine avait disparu. Après une courte investigation, j'ai trouvé un paramètre de configuration pour empêcher la modification des domaines personnalisés lors du déploiement d'Azure Container Apps:

```bash
azd config set alpha.aca.persistDomains on
```

Une fois ce paramètre configuré, tous les déploiements ultérieurs ont conservé le nom de domaine personnalisé. Hourra! Pour voir toutes les autres options de configuration, exécutez la commande `azd config list-alpha`.

## Intégration continue et déploiement continu (CI/CD)

J'ai utilisé la commande `azd pipeline config` de nombreuses fois, ce n'était donc pas une surprise. Mais je suis toujours étonné du temps que cela me fait gagner. La commande crée un fichier de workflow GitHub Action (cela fonctionne également avec Azure DevOps). Elle crée également des secrets dans le dépôt. Le workflow utilise ces secrets pour déployer la solution, de sorte qu'aucune information sensible n'est codée en dur dans le fichier de workflow.

## Conclusion

Dans cet article, j'ai partagé des conseils et astuces que j'ai appris en utilisant `azd` pour déployer ma solution. J'aime vraiment cet outil CLI car il est toujours là, prêt à nous aider à passer à l'étape suivante. Si vous ne l'avez pas encore essayé, vous devriez. Vous pouvez trouver la documentation [ici: Azure Developer CLI (azd)](https://learn.microsoft.com/fr-ca/azure/developer/azure-developer-cli/).


## Vous voulez en apprendre davantage?

Ce projet a été déployé dans Azure Container Apps. Je vous recommande fortement de consulter le dépôt [Get Started .NET on Azure Container Apps](https://aka.ms/aca-start), qui contient plusieurs tutoriels étape par étape (avec vidéos) vous aidant à apprendre à utiliser Azure Container Apps avec .NET.
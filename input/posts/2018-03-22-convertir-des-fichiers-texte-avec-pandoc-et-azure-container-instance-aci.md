---
title: Convertir des fichiers texte avec Pandoc et Azure Container Instance (ACI)
permalink: /convertir-des-fichiers-texte-avec-pandoc-et-azure-container-instance-aci
Published: 2018-03-22
tags: [youtube, azure, post, container, cloud5minutes, aci, azure container instance, pandoc]
---


On entend beaucoup et de plus en plus parler des conteneurs, de Docker et autres outils du même genre.  Microsoft Azure offre un service très intéressant qui permet de provisionner très rapidement un conteneur afin de pouvoir l'utiliser et d'en disposer facilement. Ce service nommé: Azure Container Instance (ACI), en preview au moment de publier ce billet, est parfait pour une consommation rapide.  Dans ce billet je vous montrerai comment convertir un texte HTML en document Word en utilisant Pandoc dans un conteneur.

## La boite à outils

Il y a plusieurs façons d'interagir avec les Azure Container Instance (ACI). Il y a bien sûr via le portal Azure, les commandes PowerShell, mais dans ce billet j'ai choisi d'utiliser Azure CLI car c'est commande fonctionneront partout que vous soyez sur Mac, Linux, Windows ou que vous utilisiez le Cloud Shell.

Afin de convertir le code HTML en document Word j'utiliserai le convertisseur universel Pandoc (http://pandoc.org/). Comme nous utiliserons des appels HTTP pour interagir avec le conteneur j'utiliserai donc l'image dwolters/pandoc-http disponible sur [Docker Hub](https://hub.docker.com/r/dwolters/pandoc-http/).

## Comment utiliser Azure Container Instance

Comme dans tout bon script, il nous faut un `Resourcegroup`, ici nommé cloud5mins.

    az group create --name cloud5mins --location eastus

Maintenant, il est tant de créer notre conteneur. 

    az container create -g cloud5mins --name mypandoc --image dwolters/pandoc-http --ip-address public

Dans la commande précédente, nous créons un conteneur nommé mypandoc en utilisant l'image `dwolters/pandoc-http`. Afin d'être certain que mon conteneur soit accessible depuis Postman je spécifie vouloir une adresse publique. 

Afin de connaitre le statut du conteneur et de voir son adresse IP, on peut utiliser la commande `show`.

    az container show -g cloud5mins --name mypandoc

Vous recevrez un JSON semblable a celui-ci:

```json
{
    "additionalProperties": {},
    "containers": [
        {
        "additionalProperties": {},
        "command": null,
        "environmentVariables": [],
        "image": "dwolters/pandoc-http",
        "instanceView": {
            "additionalProperties": {},
            "currentState": {
            "additionalProperties": {},
            "detailStatus": "",
            "exitCode": null,
            "finishTime": null,
            "startTime": "2018-03-20T00:29:03+00:00",
            "state": "Running"
            },
            "events": [],
            "previousState": null,
            "restartCount": 0
        },
        "name": "mypandoc",
        "ports": [
            {
            "additionalProperties": {},
            "port": 80,
            "protocol": null
            }
        ],
        "resources": {
            "additionalProperties": {},
            "limits": null,
            "requests": {
            "additionalProperties": {},
            "cpu": 1.0,
            "memoryInGb": 1.5
            }
        },
        "volumeMounts": null
        }
    ],
    "id": "/subscriptions/xxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxx/resourceGroups/cloud5mins/providers/Microsoft.ContainerInstance/containerGroups/mypandoc",
    "imageRegistryCredentials": null,
    "instanceView": {
        "additionalProperties": {},
        "events": [],
        "state": "Running"
    },
    "ipAddress": {
        "additionalProperties": {},
        "dnsNameLabel": null,
        "fqdn": null,
        "ip": "52.234.210.161",
        "ports": [
        {
            "additionalProperties": {},
            "port": 80,
            "protocol": "TCP"
        }
        ]
    },
    "location": "eastus",
    "name": "mypandoc",
    "osType": "Linux",
    "provisioningState": "Succeeded",
    "resourceGroup": "cloud5mins",
    "restartPolicy": "Always",
    "tags": null,
    "type": "Microsoft.ContainerInstance/containerGroups",
    "volumes": null
}
```

Une fois que le conteneur aura un statut `Running` il sera prêt.  Prenez note de l'adresse IP et dirigeons-nous dans Postman.

Dans le body spécifiez le HTML que vous désirez convertir. Comme nous convertissons du HTML à Word les Headers que nous devons passer sont: `Accept=value` et `Content-Type=text/html`. Ensuite, changer l'action pour un `POST` et entrer IP. Cliquer sur Send and Download pour sauvegarder le résultat. Voilà!

![](/content/images/2018/03/postman_test.png)

N'oubliez pas!
-------------

Vous avez terminé avec votre conteneur? Alors il est important de le déduire afin de ne pas avoir des grosses dépenses non désirées.  Heureusement c'est très simple. Il suffit d'exécuter la commande suivante.

    az conteneur delete -g cloud5mins --name mypandoc

## Explication en vidéo svp

Dans cette capsule de [Cloud en 5 minutes](https://youtu.be/OJlUysFYTc8), je vous montre comment utiliser Azure Container Instance (ACI). La démo consiste a créer un Docker conteneur en utilisant l'image Pandoc-http afin de transformer un fichier HTML en document Word.

[![YouTube - Cloud en 5 minutes](/content/images/2018/03/ACI_square-1.png)](https://youtu.be/OJlUysFYTc8)

Abonnez-vous: http://bit.ly/2jx3uKX

[postman_test]: /../images/2018-03-21/postman_test.png
[square]: /../images/2018-03-21/ACI_square




---
title: Déboguer localement des Azure Functions d'un PC Linux ou Windows
permalink: /deboguer-localement-des-azure-functions-dun-pc-linux-ou-windows
Published: 2017-12-06
tags: [cloud, infonuagique, azure, windows, ubuntu, function, serverless, clouden5, sansserveur, deboguer]
---


Dans cette capsule de Cloud en 5 minutes, vous montre la marche a suivre pour pouvoir déboguer localement des Azure Functions d'une machine ayant comme OS Linux (Ubuntu) ou Windows (Windows 10).

<div class="container ">
<iframe class="youtubevideo" width="560"  src="https://www.youtube.com/embed/vWckSXGDlpM" frameborder="0" allowfullscreen></iframe>
</div>

Windows
-------

Sur Windows, le plus simple est d'utiliser VisualStudio 2017 téléchargeable depuis le site: [visualstudio.com](https://www.visualstudio.com/downloads) et bien vous assurer que l'option Azure Development soit cochée lors de l'installation.

Linux
-----

Sous Linux, vous aurez besoin de VIsualStudio Code disponible depuis [code.visualstudio.com](https://www.code.visualstudio.com)  et des outils Azure Functions, que vous pouvez installer avec la commande suivante:

    sudo npm i -g azure-functions-core-tools@core --unsafe-perm

Voici les commandes utilisées pendant la vidéo.

```bash

    mkdir MyFunctionApp
    cd MyFunctionApp
    func init .

```
```bash
    func new --language JavaScript --template HttpTrigger --name HttpBonjour
```
```bash
    func host start
```



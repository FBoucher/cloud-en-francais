---
title: Comment passer un fichier à Web API depuis une Azure Logic App
permalink: /comment-passer-un-fichier-a-web-api-depuis-une-azure-logic-app
Published: 2017-03-31
tags: [cloud, api, azure, logicapp, file, connector]
---


J'adore Logic App, c'est l'un de mes outils préférés dans ma boite à outils infonuagique. C'est tellement facile de relier les applications et parfois, sans même devoir écrire une seule ligne de code! La semaine dernière, je devais passer un fichier contenu dans dossier SharePoint à une API. J'ai déjà déplacé des tons de fichiers utilisant des Azure Logic App, mais cette fois-ci, quelque chose ne fonctionnait pas. Grâce à [Jeff Hollan](https://twitter.com/jeffhollan) qui m'a mis sur le bon chemin en me donnant de bons conseils, le problème a été rapidement résolu. Dans ce post, je partagerai avec vous les petites choses qui font toute la différence dans ce cas.

L'objectif
----------

Lorsqu'un fichier est créé dans un dossier SharePoint, une Azure Logic App doit être déclenchée et transmettre le nom du fichier et son contenu à un Web API. Dans ce cas, j'utilise Sharepoint, mais le même principe s'appliquera pour tous les types de connecteurs de dossiers (ex: DropBox, OneDrive, Box, GoogleDrive, etc.).

 > **Note:**
 > Dans ce post, j'utilise SharePoint Online, mais la même chose pourrait parfaitement fonctionner avec SharePoint local (on-premise) ou dans une machine virtuelle. Dans cette situation, un *On-premise Data Gateway* doit être installée localement. Très simple à installer, suivez les instructions. Un seul bémol... Vous devez utiliser le même compte Microsoft de type "travail ou école" pour vous connecter à Azre.portal.com ET durant l'installation la passerelle *On-premise Data Gateway*.


L'application Web API 
---------------------

Commençons par créer notre API Web. Dans Visual Studio, créez une nouvelle application API Web. Si vous souhaitez avoir plus de détails sur la façon de créer un, consultez mon  [post][apiapppost] publié précédemment. Une fois l'application créée, ajouter un nouveau contrôleur et une nouvelle fonction UploadNewFile avec le code suivant:


    [SwaggerOperation("UploadNewFile")]
    [SwaggerResponse(HttpStatusCode.OK)]
    [Route("api/UploadNewFile")]
    [HttpPost]
    public HttpResponseMessage UploadNewFile([FromUri] string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return Request.CreateResponse(HttpStatusCode.NoContent, "No File Name.");
        }

        var filebytes = Request.Content.ReadAsByteArrayAsync();

        if (filebytes.Result == null || filebytes.Result.Length <= 0)
        {
            return Request.CreateResponse(HttpStatusCode.NoContent, "No File Content.");
        }

        // Do what you need with the file.

        return Request.CreateResponse(HttpStatusCode.OK);
    }


Le tag `[FromUri]` avant le paramètre est juste un moyen de spécifier d'où provient cette information. Le contenu du fichier ne peut être transmis dans la querystring, donc il sera transmis dans le corps de notre *HTTP Request*. Pour récuréprer le contenu, nous utiliserons la commande `Request.Content.ReadAsByteArrayAsync()`. Si tout fonctionne, nous retournons un HttpResponseMessage avec HttpStatusCode.OK sinon un message détaillant le problème. Vous pouvez maintenant publier votre application Wep API.

Pour être en mesure de voir notre Web API App à partir de notre Logic App, il nous faut faire une dernière petite étape. Dans le portail Azure, sélectionnez *l'App Service* fraichement déployé et, dans la section des options (zone de gauche avec toutes les propriétés), sélectionnez CORS, puis entrer `*` et sauvegarder le tout en appuyant sur le bouton *Save* en au de la page.

![changeCORS](/content/images/2017/03/changeCORS.png)


L'application Logic App
-----------------------

En supposant que vous disposez déjà d'un SharePoint prêt à être utilisé, créez la nouvelle Logic App. Une fois l'application déployée dans Azure, cliquez sur le bouton d'édition pour accéder au Designer. Sélectionnez le modèle vierge (Blank). Dans ce post, j'ai besoin d'un déclencheur (trigger) SharePoint lorsqu'un nouveau fichier est créé. Afin de créer le connecteur SharePoint, il vous faudra répondre afin de fournir les informations pour que le portal Azure puisse se connecter à votre site. Une fois qu'il est connecté, sélectionnez le dossier où vous allez déposer vos fichiers.

Maintenant que le *trigger* est terminé, nous ajouterons une première action (la seule). Cliquez sur Ajouter une étape. Sélectionnez les fonctions disponibles, puis notre *App Service* et enfin la méthode `UploadNewFile`.

![SelectApiApp](/content/images/2017/03/SelectApiApp.png)

Grâce à Swagger, Logic App sera capable de générer pour nous, un formulaire avec tous les paramètres nécessaires pour connecter à L'API. Sélectionner le nom de fichier dans la zone de texte du paramètre Filename. L'App Logic devrait ressembler à ceci.

![FullLogicApp](/content/images/2017/03/FullLogicApp.png)

La dernière chose à faire est de spécifier notre Logic App de transmettre le contenu du fichier via le *body* (corp) de la requête HTTP vers l'API. Aujourd'hui, ce n'est pas possible de le faire à l'aide de l'interface. Comme vous le savez probablement, derrière cette magnifique interface, se cache un simple document json, et c'est en éditant celui-ci que nous pourrons spécifier comment passer le contenu du fichier.

Basculez vers la vue Code et recherchez l'étape qui appelle notre Web API. Il suffit d'ajouter `"body": "@triggerBody()"` à ce noeud. Cela informera Logic App de passer le corps du trigger (contenant le contenu du fichier) et de le transmettre au corps de notre requête vers l'API. Le code devrait ressembler à ceci:

    "UploadNewFile": {
        "inputs": {
            "method": "post",
            "queries": {
            "fileName": "@{triggerOutputs()['headers']['x-ms-file-name']}"
            },
            "body": "@triggerBody()",
            "uri": "https://frankdemo.azurewebsites.net/api/UploadNewFile"
        },
        "metadata": {
            "apiDefinitionUrl": "https://frankdemo.azurewebsites.net/swagger/docs/v1",
            "swaggerSource": "website"
        },
        "runAfter": {},
        "type": "Http"
    }


Vous pouvez maintenant sauvegarder et quitter le mode d'édition. La solution est prête, amusez-vous!


##### References:
 
- [stackoverflow](http://stackoverflow.com/questions/42824254/pass-a-file-to-an-api-from-azure-logic-app)


[apiapppost]: http://www.frankysnotes.com/2016/10/how-i-use-azure-app-api-app-and.html


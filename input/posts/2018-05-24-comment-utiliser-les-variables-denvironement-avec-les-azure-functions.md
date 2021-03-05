---
title: Comment utiliser les variables d'environement avec les Azure Functions
permalink: /comment-utiliser-les-variables-denvironement-avec-les-azure-functions
Published: 2018-05-24
tags: [cloud, kudu, azure, post, function, slot, variable, environement]
---

Les Azure Functions sont un extrêmement utile lorsqu’on développe « dans le nuage », par leur simplicité de gestion. Pour obtenir des fonctions plus dynamiques, il est très utile d’utiliser les variables d’environnement.  Dans ce post j’expliquer où on peut les trouver et je montrerai comment les utiliser.

## Où trouver une liste des variables

Pour accéder à la liste de variables d’environnement, ouvrez votre fonction Azure (dans le portail [portal.azure.com](portal.azure.com)) et sélectionnez la functionApp dans le menu gauche (1). Dans la section de droite, sélectionnez l’onglet *Platform features*. Enfin, dans la section *Development Tools*, sélectionnez *Advanced tools (Kudu)*.

![Kudu](/content/images/2018/05/WhereisKudu.png)

Cela ouvrira l’interface Kudu dans un nouvel onglet. Sélectionnez l’onglet Environnement et toutes les variables sont visibles ici.

![liste des variables d’environnement](/content/images/2018/05/EnvironmentVariables.png)

## Utiliser les variables d’environnement

Les variables sont facilement utilisables. Il suffit d’utiliser la méthode `GetEnvironmentVariable` et de passer le nom de la variable.

```csharp

    using System.Net;

    public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
    {
        log.Info("C# HTTP trigger function processed a request.");

        // parse query parameter
        string name = req.GetQueryNameValuePairs()
            .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
            .Value;

        if (name == null)
        {
            // Get request body
            dynamic data = await req.Content.ReadAsAsync<object>();
            name = data?.name;
        }

        var rgName = System.Environment.GetEnvironmentVariable("WEBSITE_RESOURCE_GROUP", EnvironmentVariableTarget.Process);
        log.Info($"Cette fonction est dans le Resource group: {rgName} ");

        return name == null
            ? req.CreateResponse(HttpStatusCode.BadRequest, "Please pass a name on the query string or in the request body")
            : req.CreateResponse(HttpStatusCode.OK, "Hello " + name);
    }

```

Dans l’exemple ci-dessus, on va chercher la variable `WEBSITE_RESOURCE_GROUP` afin de connaître le nom du ressource groupe et ensuite la valeur est affichée dans le log.

```text

2018-05-24T11:06:51.053 [Info] Function started (Id=33f22366-80a1-476b-8504-e4c98bbbb132)
2018-05-24T11:06:51.256 [Info] C# HTTP trigger function processed a request.
2018-05-24T11:06:51.303 [Info] Cette fonction est dans le Resource group: cloud5mins
2018-05-24T11:06:51.303 [Info] Function completed (Success, Id=33f22366-80a1-476b-8504-e4c98bbbb132, Duration=246ms)

```

## En vidéo SVP!

Si vous préférez, j’ai une version en vidéo de ce post où je montre comment changer le comportement de votre fonction selon si la fonction est en production ou stagging. Le tout en utilisant la variable d’environnement: `APPSETTING_WEBSITE_SLOT_NAME`.

<div class="container">
<iframe  class="youtubevideo" width="560"  src="https://www.youtube.com/embed/8dIv5iy-w74" frameborder="0" allow="autoplay; encrypted-media" allowfullscreen></iframe>
</div>

[Abonnez-vous!](http://bit.ly/2jx3uKX)
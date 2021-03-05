---
title: Un site Web statique et quelques astuces avec Azure Functions Proxies
permalink: /un-site-web-statique-et-quelques-astuces-avec-azure-functions-proxies
Published: 2017-05-04
tags: [cloud, azure, post, storage, function, http, webhook, serverless, proxie]
---

*(Mise-à-jour 2018-02-08)*

Récemment, j'ai fait quelques présentations à propos des Azure Functions. La réaction a toujours été très positive et les participants repartaient avec des tonnes d'idées dans leur tête. Dans ce billet, j'aimerais ajouter quelques fonctionnalités intéressantes auxquelles je n'ai pas eu le temps de parler.

>Vous préférez visionner une vidéo à la lecture? Aucun problème, sauter immédiatement à la section Explication en vidéo svp de ce post.


Pour débutter
-------------

 À partir du portail Azure (portal.azure.com), sélectionnez votre domaine Azure Function ou créez un nouveau. Ensuite, créez une Fonction App qui sera utilisée comme backend. Cliquez sur le signe "+" à droite de Function. Dans ce post, j'utilise le modèle *HttpTrigger-CSharp*, mais sachez qu'un autre modèle pourrait très bien fonctionner. Une fois que vous avez sélectionné le modèle, vous pourrez lui donner un nom et sélectionner l'*Authorization level*. Ce dernier choix affectera la façon dont votre fonction pourra être consultée. Par exemple, si vous sélectionnez *Anonyme*, elle sera accessible à tous, directement à l'aide de l'URL: `https://notesfunctions.azurewebsites.net/api/SecretFunction`, en assumant que le nom de function domain soit 'notesfunctions'. Si au contraire vous sélectionnez le niveau *Fonction* ou *Admin *, vous devrez passer une clé soit Function Key ou Master Key (ex: `https://notesfunctions.azurewebsites.net/api/SecretFunction?code=I4BN6NjaZBmPNqebqnX8pQAPZwe1TI/O4TCbvB1aaaaao7uRO5yyw== `). Pour ce post, nous utilisons le niveau Fonction.

![CreateFunction](/content/images/2017/05/CreateFunction.png)

Vous pouvez maintenant tester votre fonction. Vous pouvez utilisez Postman, ou votre outil HTTP préféré, ou même la section Test directement dans dans portail Azure (située sur le côté droit de la blade).  Pour obtenir l'URL de vos fonctions, rien de plus simple, une fois que votre Function App est sélectionnée (lorsque vous voyez le code), cliquez sur *</> Get function URL* en haut de l'écran.

![TestSecretDirect](/content/images/2017/05/TestSecretDirect.png)

Notez que la querystring contiens un paramètre `code` qui reçoit la valeur de notre Fonction Key. Lorsque ce paramètre n'est pas présent, vous recevrez un message HTTP 401 Unauthorized message. La fonction générée par le modèle attend une valeur `Name` qui peut être passé par la querystring ou via le body de la requète, avec un document json contenant propriété `Name`.

Azure Functions Proxies
-----------------------

Functions Proxies sont actuellement en preview. Avec ceux-ci, toute Fonction App peut maintenant définir un endpoint qui sert de proxy inversé à une autre application [API / webhook / function / autre chose].

Avant de pouvoir créer de nouveau Azure Functions Proxies, vous devez les activer. À partir du portail Azure (portal.azure.com), sélectionnez votre domaine Azure Function ouvrir l'onglet *Paramètres* en haut de l'écran, puis cliquez sur le bouton *On* de la section Proxies.

![EnableProxies](/content/images/2017/05/EnableProxies.png)

Maintenant, créons notre premier Fonction Proxy. Cliquez sur "+" à droite de *Proxies (Preview)*. Et saisissez les valeurs suivantes.

![ProxySalutation](/content/images/2017/05/ProxySalutation.png)

Vous remarquerez dans *Backend URL*, que `%Host_Name%` est utilisée dans l'URL; ce n'est PAS une variable d'environnement. C'est un outil très utile d'Azure Function qui nous permet de lire directement dans les aux paramètres de l'application (Application settings).

![PlatformFeatures](/content/images/2017/05/PlatformFeatures.png)

Pour accéder aux Application settings, sélectionnez le Function Application domain (le nœud racine), puis l'onglet *Platform features* en haut de l'écran. Dans l'image ci-dessus, Point **A** montre comment accéder aux Application settings, et Point **B** montre comment accéder au *App Service Editor* qui sera utilisé plus tard dans ce post.

Si ce n'est pas déjà fait, ajouter une nouvelle valeur Application setting nommé `Host_Name`, puis testez pour appeler votre nouvelle fonction proxy. Notez que maintenant vous n'avez plus besoin de passer la Key car ceci est maintenant pris en cherche par notre proxy.

![TestSalutation](/content/images/2017/05/TestSalutation.png)

Obtenir le maximum de nos Proxies
---------------------------------

Maintenant que nous avons un proxy en pleine santé, passons au *App Service Editor* pour en faire plus (l'éditeur est disponible dans l'onglet *Platform features*). Une fois que vous êtes dans l'éditeur, sélectionnez le fichier **proxies.json** pour l'ouvrir.

![Editor](/content/images/2017/05/editor.png)

Comme vous pouvez le voir, nous n'avons qu'un proxy de défini. Dupliquez ce dernier, et renommez la copie: Override. Modifiez également la valeur de la propriété route pour `override`. Si vous testez cette nouvelle fonction, elle fonctionnera exactement comme l'autre. Sous la propriété `backendUri` ajoutez un nouveau nœud nommé: responseOverrides. Il est possible avec les proxies d'éditer les propriétés HTTP. Pour modifier le *Content-Type* en texte au lieu de json, ajouter `"response.headers.Content-Typ ":"text/plain"` à l'intérieur de notre nouveau node responseOverrides. Testez à nouveau le proxy Override et vous constatez que le contenu a effectivement changé.

En continuant de modifier les propriétés de la réponse HTTP, nous pourrions utiliser Azure Function Proxies en tant que *mock*. Par exemple, vous pouvez supprimer la propriété `backendUri` et remplacer la valeur de *body* pour retourner une valeur statique, et vous auriez construit un mock-up! Très utile! Ajoutez un nouveau proxy contenant ce code:

    "Fake": {
        "matchCondition": {
            "route": "fake"
        },
        "responseOverrides": {
            "response.headers.Content-Type": "text/plain",
            "response.body": "Hello from Azure"
        }
    }

Si vous testez pour appeler ce dernier proxy, aucun backend ne sera appelé, et pourtant tout fonctionne.

Site Web statique
-----------------

Tout le monde sait que le stockage Azure est vraiment très économique. Ce serait vraiment merveilleux de pouvoir mettre un petit site web statique dans ce stockage? En fait il est possible de le faire (personnellement je le fais depuis de nombreuses années), tant que l'URL est complète, ça fonctionnera. Cependant, qui tape une URL complètement (ex: http://www.frankysnotes.com/index.html)? Bien maintenant, grâce à Azure Function Proxy, nous pouvons  résoudre ce problème! Ajoutez un autre noeud proxy au fichier proxies.json.

    "StaticNotes": {
        "matchCondition": {
            "methods": [
                "GET"
            ],
            "route": "/"
        },
        "backendUri": "https://%blob_url%/dev/index.html"
    }

Ce nouveau proxy "redirigera" tout appel HTTP GET à la racine vers notre fichier index.html qui attend bien sagement dans notre stockage Azure. Pour un look plus professionnel, il vous suffit d'ajouter un nom de domaine personnalisé à votre fonction, et vous avez obtenu le site internet superléger à faible cout pour votre campagne de promotionnel ou votre évènement.

![static](/content/images/2017/05/static.png)

## Explication en vidéo svp

<iframe width="640" height="360" src="http://www.youtube.com/embed/ZdMFtlpLpM4?feature=player_detailpage" frameborder="0" allowfullscreen></iframe>

##### References:

- Postman : [getpostman.com](https://www.getpostman.com/)
- App Service Editor: https:///{function domain name}.scm.azurewebsites.net (ex: https://notesfunctions.scm.azurewebsites.net)


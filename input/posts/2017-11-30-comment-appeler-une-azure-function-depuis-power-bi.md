---
title: Comment appeler une Azure Function depuis Power Bi
permalink: /comment-appeler-une-azure-function-depuis-power-bi
Published: 2017-11-30
tags: [youtube, infonuagique, azure, post, powerbi, function, serverless, clouden5, sansserveur, powerquery]
---

Dans cette capsule de Cloud en 5 minutes, je vous montre comment vous connecter à une Azure Function directement à partir de Power Bi. Cette astuce peut être très utile pour faire des conversions, ou même déclancher d'autre opérations.



<div class="container ">
<iframe class="youtubevideo" width="560"  src="https://www.youtube.com/embed/drbgMar8XLA" frameborder="0" allowfullscreen></iframe>
</div>

Si vous êtes intéressé à reproduire exactement la même solution voici le code qui constitue la Function Azure:

```csharp

#r "Newtonsoft.Json"
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, TraceWriter log)
{
    log.Info("C# HTTP trigger function processed a request.");

    var stores = new List<Store>() {
                    new Store {StoreNo= 100,Address= "101 Jewel Lane North",City= "Little York",StateAbbreviation= "KS",ZipCode= 66012},
                    new Store {StoreNo= 200,Address= "43 Clear View Drive",City= "Pine Bluff",StateAbbreviation= "AR",ZipCode= 71613},
                    new Store {StoreNo= 300,Address= "61 Josephs Point Road",City= "Cambridge",StateAbbreviation= "MN",ZipCode= 55008},
                    new Store {StoreNo= 400,Address= "81 Bellevue Drive",City= "New Eagle",StateAbbreviation= "PA",ZipCode= 15067},
                    new Store {StoreNo= 500,Address= "90 Northwood Drive",City= "Headrick",StateAbbreviation= "OK",ZipCode= 73549}
                };
    var json = JsonConvert.SerializeObject(stores, Formatting.Indented);
     
    return new HttpResponseMessage(HttpStatusCode.OK) 
    {
        Content = new StringContent(json, Encoding.UTF8, "application/json")
    };
 
}


public class Store
{
    public int StoreNo{get;set;}
    public string Address{get;set;}
    public string City{get;set;}
    public string StateAbbreviation{get;set;}
    public int ZipCode{get;set;}
}

```

Pour appeler la function vous devez creer un requête vierge et utiliser ce code:

```
let
    functionUrl = "https://clouden5minutes.azurewebsites.net",

    StatusUpdate = Web.Contents(functionUrl,
       [
         Headers = [#"Content-Type" = "application/json", 
                    #"x-functions-key" = "MpcE1q72YklCDnU91RZy1hmM2GphT6xxxxDioixgTIh/Bfcjc76Q=="
                   ],
         RelativePath="/api/GetActiveStores"
       ]
    ),
    #"response" = Json.Document(StatusUpdate)
in
    #"response"

```

Bonus
-----

Si toutefois de devez passer un paramètre dans le corp (body) de votre requète, vous devez tout d'abord créer une seconde requète qui retournera du json.  À partir de Power Bi créer un nouvelle requête vierge nommer GetjSon en utilisant ce code.

```

// Power BI function generates JSON
(InputData) =>
let
    JsonOutput = Json.FromValue(InputData),
    OutputText = Text.FromBinary(JsonOutput)
in
    OutputText

```

Il ne restera plus qu'à spécifier le `Content` lors de l'appel.

```
    ...
    StatusUpdate = Web.Contents(functionUrl,
       [
         Headers = [#"Content-Type" = "application/json", 
                    #"x-functions-key" = "MpcE1q72YklCDnU91RZy1hmM2GphT6xxxxDioixgTIh/Bfcjc76Q=="
                   ],
         RelativePath="/api/GetActiveStores",
         Content = Text.ToBinary(YOUR_PARAMETER)
       ]
    ),
    ...
```

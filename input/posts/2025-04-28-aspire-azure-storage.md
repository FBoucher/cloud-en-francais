---
title: "Comment utiliser Azure Table Storage avec .NET Aspire et une API Minimale"
Published: 2025-04-29
featured-image: ../content/images/2025/04/aspire-azure-storage_header.png
Image: ../content/images/2025/04/aspire-azure-storage_header.png
categories: post
tags: [azure,azure container apps,blazor,fluentui,dotnet,container,post]
---

Azure Storage est une solution de stockage cloud polyvalente que j'ai utilisée dans de nombreux projets. Dans cet article, je partagerai mon expérience de son intégration dans un projet .NET Aspire à travers deux perspectives : d'abord, en construisant un projet de démonstration simple pour apprendre les bases, puis en appliquant ces apprentissages pour migrer une application réelle, [AzUrlShortener](https://github.com/microsoft/AzUrlShortener).

Cet article fait partie d'une série sur la modernisation du projet AzUrlShortener :
- [Migration du project AzUrlShortener d'Azure Static WebApp vers Azure Container Apps](https://www.cloudenfrancais.com/posts/2025-04-22-urlshortener-architecture.html)
- [Conversion d'un projet Blazor WASM en FluentUI Blazor Server](https://www.cloudenfrancais.com/posts/2025-04-23-from-blazor-wasm-to-fluentui-blazor-server.html)
- [Azure Developer CLI (azd) dans un scénario réel](https://www.cloudenfrancais.com/posts/2025-04-24-azd-reel-scenario.html)
- Comment utiliser Azure Table Storage avec .NET Aspire et une API Minimale

## Partie 1 : Apprendre avec un projet simple

Pour cet article, nous utiliserons un projet plus simple que la solution complète AzUrlShortener pour faciliter la compréhension. Tout le code de ce projet simple est également disponible sur [GitHub : AspireAzStorage](https://github.com/FBoucher/AspireAzStorage), faites-en une copie (fork) et explorez-le.

> 💡 Tout le code est disponible sur [GitHub : AspireAzStorage](https://github.com/FBoucher/AspireAzStorage)

### Le contexte

Ce tutoriel démontre comment créer une solution .NET Aspire avec une API Minimale qui récupère des données d'employés depuis Azure Table Storage. Nous construirons une solution propre et structurée qui peut fonctionner à la fois localement et dans Azure.

La structure de la solution a été créée avec une simple commande `dotnet new webapi -n EmployeeApi -o EmployeeDemo\EmployeeApi`. Ensuite, depuis votre éditeur préféré, "Ajoutez l'orchestration .NET Aspire" en cliquant avec le bouton droit sur le projet dans l'Explorateur de Solutions.

Pour qu'AppHost puisse orchestrer un Azure Storage, nous aurons besoin d'ajouter le package `Aspire.Hosting.Azure.Storage`. Cela peut se faire de plusieurs façons, mais en utilisant la CLI, la commande ressemblerait à `dotnet add AppHost package Aspire.Hosting.Azure.Storage`.

### Définir l'orchestration pour utiliser Azure Storage

Nous voulons que l'API lise les données depuis un Azure Table Storage et retourne le résultat. En utilisant l'injection de dépendances (DI), nous pourrions ajouter un compte de stockage Azure au projet AppHost, et spécifier que nous avons besoin du client table pour le transmettre au projet API. Le code du fichier `program.cs` dans le projet AppHost ressemblerait à ceci :

```csharp
using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var azStorage = builder.AddAzureStorage("azstorage");

if (builder.Environment.IsDevelopment())
{
    azStorage.RunAsEmulator();
}

var strTables = azStorage.AddTables("strTables");

builder.AddProject<Projects.Api>("api")
		.WithExternalHttpEndpoints()
		.WithReference(strTables)
		.WaitFor(strTables);

builder.Build().Run();
```

Le `azStorage` est la référence au compte de stockage Azure, et `strTables` est la référence au client table. Pour pouvoir exécuter la solution localement, nous vérifions si l'environnement est "IsDevelopment" et lançons l'émulateur de stockage Azure. Cela permettra à .NET Aspire de créer un conteneur Azurite pour émuler le compte de stockage Azure. En production, l'émulateur n'est pas nécessaire, et le compte de stockage Azure sera utilisé. Enfin, nous passons la référence `strTables` au projet API et nous nous assurons que le client est prêt avant de démarrer l'API.

### Le projet d'API Minimale

Nous savons déjà que notre projet s'attend à un client Azure Table Storage, nous pouvons donc ajouter le package `Aspire.Azure.Data.Tables` au projet API. En utilisant la CLI, la commande est `dotnet add EmployeeApi package Aspire.Azure.Data.Tables`. Et nous pouvons ajouter `builder.AddAzureTableClient("strTables");` juste avant la création de l'application dans le fichier `Program.cs`.

La beauté d'une API Minimale est qu'elle est très flexible et peut être aussi minimaliste ou structurée que vous le souhaitez. Lorsque le projet est créé, tout est dans le fichier `Program.cs`. Cela le rend facile à suivre et à comprendre. Mais à mesure que le projet grandit, il peut devenir difficile à maintenir. Pour le rendre plus facile à maintenir, nous pouvons déplacer les endpoints, les modèles et les services dans des fichiers et dossiers distincts. Cela laisse notre `Program.cs` avec uniquement le code suivant :

```csharp
using Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddAzureTableClient("strTables");

var app = builder.Build();
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Add the Employee Endpoints
app.MapEmployeeEndpoints();

app.Run();
```

Et le reste du code est réparti dans différents fichiers et dossiers. La structure du projet est la suivante :

```bash
EmployeeApi/
├── Endpoints/        
│   └── EmployeeEndpoints.cs    # Endpoints pour l'API Employee
├── Models/
│   └── EmployeeEntity.cs       # Modèle pour l'Azure Table Storage
├── Services/
│   └── AzStorageTablesService.cs
├── Api.http                    # Fichier HTTP pour tester l'API
└── Program.cs                  # Fichier principal de l'API Minimale
```

### Endpoints d'employés

Vous avez peut-être remarqué à la fin du fichier `Program.cs`, nous appelons `app.MapEmployeeEndpoints();`. C'est une méthode d'extension personnalisée qui ajoutera les endpoints à l'API.

```csharp
    public static void MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("api")
                            .WithOpenApi();

        MapGetAllEmployees(endpoints);
        MapGetAllEmployeesAsync(endpoints);
        MapGetEmployeesByFirstLetter(endpoints);
        MapGetEmployeesGroupByFirstLetterFirstNameAsync(endpoints);
        MapGenerateEmployees(endpoints);
    }
```

Cela regroupera tous les endpoints sous le chemin `/api` et ajoutera la documentation OpenAPI. Ensuite, nous pouvons définir chaque endpoint dans une méthode différente. Par exemple, la méthode `MapGetAllEmployees` ressemblera à ceci :

```csharp
    private static void MapGetAllEmployees(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/GetEmployeesAsync", (TableServiceClient client) => GetAllEmployeeAsync(new AzStrorageTablesService(client)))
            .WithName("GetAllEmployees")
            .WithDescription("Obtenir tous les employés depuis le stockage table");
    }
```

Notez le paramètre `TableServiceClient client`. C'est le client Azure Table Storage qui a été créé précédemment et passé par DI. Nous le transmettons au service `AzStrorageTablesService` qui sera responsable d'interagir avec Azure Table Storage. Les méthodes `WithName` et `WithDescription` sont utilisées pour ajouter des métadonnées à l'endpoint qui seront utilisées dans la documentation OpenAPI.

### Le service Azure Table Storage

Pour s'assurer que la table Employee existe lorsque les requêtes sont exécutées, nous pouvons utiliser le constructeur de `AzStrorageTablesService` pour créer la table si elle n'existe pas et instancier le client de table.

```csharp
    private readonly TableClient _employeeTableClient;

    public AzStrorageTablesService(TableServiceClient client)
    {
        client.CreateTableIfNotExists("Employee");
        _employeeTableClient = client.GetTableClient("Employee");
    }
```

La seule chose qui reste est d'implémenter la méthode `GetAllEmployeeAsync` qui va interroger la table et retourner le résultat.

```csharp
    public async Task<List<EmployeeEntity>> GetAllEmployeeAsync()
    {
        var lstEmployees = new List<EmployeeEntity>();
        var queryResult = _employeeTableClient.QueryAsync<EmployeeEntity>();

        await foreach (var emp in queryResult.AsPages().ConfigureAwait(false))
        {
            lstEmployees.AddRange(emp.Values);
        }

        return lstEmployees;
    }
```

Pour s'assurer que tous les enregistrements sont retournés, nous utilisons la méthode `AsPages`. Cela récupérera tous les employés de toutes les pages, remplira une liste et la retournera.

### Tester l'API

Pour tester manuellement l'API, nous pouvons utiliser le fichier `Api.http`. Ce fichier est un simple fichier texte qui contient les requêtes HTTP. Par exemple, pour obtenir tous les employés, le contenu du fichier ressemblerait à ceci :

```csharp
@Api_HostAddress = https://localhost:7125

### Récupérer tous les employés
GET {{Api_HostAddress}}/api/GetEmployeesAsync
Accept: application/json
```

### Assembler le tout

La solution de démo contient plus d'endpoints, mais la structure est la même. Il y a un endpoint `/generate/{quantity?}` pour peupler la table des employés. Il utilise [Bogus](https://github.com/bchavez/Bogus), un générateur de données factices simple et open-source pour les langages .NET.

Pour exécuter la solution localement, un simple `F5` devrait suffire. Aspire démarrera le conteneur Azurite et l'API. Vous pouvez ensuite utiliser le fichier `Api.http` pour générer des employés et obtenir la liste des employés.

Pour déployer la solution dans Azure, vous pouvez utiliser l'Azure Developer CLI (azd). Avec `azd init`, vous pouvez créer un nouveau projet, et avec `azd up`, vous pouvez déployer la solution dans Azure. En quelques minutes, la solution sera disponible dans le cloud, mais cette fois, elle utilisera un véritable compte de stockage Azure. Rien d'autre à changer, le code est le même.

## Partie 2 : Leçons apprises lors de la migration d'AzUrlShortener

La petite expérience avec [AspireAzStorage](https://github.com/FBoucher/AspireAzStorage) m'a convaincu. Utiliser Azure Table Storage avec .NET Aspire est simple, mais nous savons tous qu'un projet réel est plus complexe. Je m'attendais donc à quelques défis. Quelle déception, il n'y en a eu aucun. Tout a fonctionné comme prévu.

Le projet AzUrlShortener a été écrit il y a quelques années et utilisait le package `Microsoft.Azure.Cosmos.Table`. Ce package est toujours totalement valide aujourd'hui, mais il en existe maintenant un pour Azure Table. La migration vers le package `Azure.Data.Tables` n'était pas simple. Quelques objets avaient des noms différents, et la requête était un peu différente. Mais la migration a été réalisée en quelques heures.

Le déploiement a fonctionné du premier coup. J'ai testé la migration des données en utilisant [Azure Storage Explorer](https://azure.microsoft.com/products/storage/storage-explorer). L'action GitHub devra être mise à jour, mais avec les fichiers bicep générés par `azd`, cela devrait être simple.

## Conclusion

J'ai vraiment apprécié ce voyage de migration du projet AzUrlShortener autant que la construction d'[AspireAzStorage](https://github.com/FBoucher/AspireAzStorage). Je vous invite à forker ce dépôt et à jouer avec. Auriez-vous fait quelque chose différemment ? Avez-vous des questions ? N'hésitez pas à les poser dans les commentaires ci-dessous ou à me contacter directement sur [@fboucheros.bsky.social](https://bsky.app/profile/fboucheros.bsky.social).

## Pour en savoir plus

Pour en savoir plus sur Azure Container Apps, je vous recommande fortement ce dépôt : [Getting Started .NET on Azure Container Apps](https://aka.ms/aca-start), il contient de nombreux tutoriels étape par étape (avec vidéos) pour apprendre à utiliser Azure Container Apps avec .NET.
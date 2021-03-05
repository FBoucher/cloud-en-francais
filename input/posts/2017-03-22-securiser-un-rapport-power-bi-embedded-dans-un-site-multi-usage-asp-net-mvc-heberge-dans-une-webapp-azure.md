---
title: Sécuriser un rapport Power BI Embedded dans un site multi-usagé Asp.Net MVC hébergé dans une WebApp Azure
permalink: /securiser-un-rapport-power-bi-embedded-dans-un-site-multi-usage-asp-net-mvc-heberge-dans-une-webapp-azure
Published: 2017-03-22
tags: [cloud, azure, powerbi, aspnet, mvc, embedded, powerbi-cli]
---

<i style="font-size: 0.8em">NDLR : Ce post est aussi disponible en anglais sur le [blog de Microsoft MVP Award](https://blogs.msdn.microsoft.com/mvpawardprogram/2017/03/21/aspnetmvc-multitenant-powerbi/" target="_blank"), dans le cadre de la  série mardi technique.</i>

Power Bi permet de réaliser des rapports d’une qualité exceptionnels et plus d’être hautement interactif. Bien que ce soit formidable de pouvoir distribuer facilement ces rapports à partir du portail PowerBI.com en toute sécurité, il est parfois nécessaire de les partager en passant par d’autres applications ou sites Web. Encore une fois, Power BI maintient ses promesses en fournissant *Power BI Embedded*. Dans ce billet, je vais expliquer comment utiliser Power BI Embedded et le sécurisé de sorte que chaque utilisateur n’est accès qu’à ses données.

Le Problème
-----------

En dépit que de nombreux tutoriels existent en ligne qui explique comment utiliser les filtres pour modifier la visibilité des données dans nos rapports, les filtres peuvent aisément être modifiés par l’utilisateur. Même si vous cachez le panneau de contrôle des filtres, ces paramètres pourraient facilement être modifiés à l’aide de JavaScript... Par conséquent, ce n’est certainement pas la meilleure façon de sécuriser l’information privée.

<!---more--->

La Solution
------------

Dans ce billet, je vais utiliser les rôles pour limiter l’accès aux données. Comme elle est très connue, la célèbre base de données Adventure Works sera utilisée afin démontrer comment partitionner les données en tirant profit de la table client (customer).

Dans Azure
----------

À partir du portail Azure, créer un composant Power BI Embedded. Bien sûr, dans un projet réel, il serait préférable de le créer dans un modèle Azure Resource Management (ARM), mais pour garder ce billet simple, nous le créerons avec le portail. Cliquez sur le gros «+» vert en haut à gauche de la page. Dans le champ de recherche, tapez `powerbi`, puis appuyez sur Enter. Sélectionnez * Power BI Embedded * dans la liste et cliquez sur le bouton * Create *. Une fois créé, accédez à la propriété Access Keys de la toute nouvelle Power BI Workspace Collection et notez clé (Key). Nous aurons besoin de cette clé plus tard pour télécharger vers Azure notre rapport Power BI.

![createwrkspc](/content/images/2017/03/CreateWorkSpaceCollection.png)

Pour cette démo, la source de données sera Adventure Works dans une Azure SQL Database. Pour ce faire, cliquez simplement sur le bouton «+» et sélectionnez Database. Veillez à sélectionner Adventure Works comme source si vous voulez reproduire cette démo.

![createdb](/content/images/2017/03/createDB.png)

Dans Power BI Desktop
---------------------

Power BI Desktop est un outil gratuit de Microsoft qui nous aidera à créer notre rapport; il peut être téléchargé [ici](https://powerbi.microsoft.com/fr-fr/desktop/).

Avant de commencer, deux options doivent être modifiées. Allez dans le menu Fichier, sélectionnez Options et Paramètres, puis Options. La première se trouve dans la section (onglet) * Preview Features *; cochez l'option: *Enable cross filtering in both directions for DirectQuery*. La seconde est dans la section DirectQuery, cochez l'option * Allow unrestricted measures in DirectQuery mode *. C'est une bonne idée de redémarrer Power BI Desktop avant de continuer.

![PowerBIoptions](/content/images/2017/03/powerbioptions.png)

Pour créer notre rapport, nous devons d’abord nous connecter à notre base de données, dans ce cas notre base de données Azure. Cliquez sur le bouton Get Data, puis sur Azure et enfin sur Microsoft Azure SQL Database. Il est important d’être attentif au type de connexion soit *Import* ou *Direct Query*, car vous ne pourrez pas le modifier par la suite. Il vous faudra reconstruire le tout à partir de zéro. Pour ce billet nous utiliserons *Direct Query*.

Ce rapport affichera des informations sur le détail des la facture. Assurez-vous d’inclure la table customer qui sera utilisée pour les rôles. Chaque client ne doit voir que ses factures.

![tables](/content/images/2017/03/tables.png)

Le rapport contiendra deux tableaux : celui de gauche est un graphique à barres illustrant l’historique des factures, quant à celui de droite c’est une tarte (pie chart) qui montre comment les produits sont distribués par catégorie.

*Note : Dans la base de données échantillon tous les clients ont qu’une seule facture et elles sont toutes de la même date.*

![chart_noRole](/content/images/2017/03/chart_noRole.png)

Maintenant, nous devons créer le rôle dynamique, qui sera utilisé par notre site web. Dans l’onglet *Modeling*, cliquez sur *Manage Roles* et créez un CustomerRole en mappant le CompanyName de la table client à la variable `USERNAME()`, tel qu’illustrer ci-dessous.

![genericRole](/content/images/2017/03/genericRole.png)

Pour tester si nos tableaux sont vraiment dynamiques, créer d’autres rôles, et leur donner des valeurs spécifiques provenant de la table client ex : «Bike World» ou «Action Bicycle Specialistes». Pour visualiser votre rapport en tant qu’utilisateur, cliquez simplement sur l’option *View as Roles*, dans l’onglet *Modeling*, puis sélectionnez le rôle souhaité.

![viewAs](/content/images/2017/03/ViewAs.png)

Voyez comment les diagrammes réagissent en fonction du rôle sélectionné, par exemple avec «Action Bicycle Specialists».

![chart_withRole](/content/images/2017/03/chart_withRole.png)

Le rapport est maintenant prêt. Sauvegarder-le et nous en aurons besoin du fichier .pbix bientôt.

Power BI-CLI
-----------

Pour télécharger notre rapport dans notre Azure Workspace Collection, j’aime utiliser PowerBI-CLI car comme il s’agit d’un outil en ligne de commande Node.js, il peut être exécuté de partout.

Ouvrez une invite de commande ou Terminal et exécutez la commande suivante pour installer PowerBI-CLI :

`npm install Powerbi-cli -g`

Maintenant, si vous entrer la commande 'powerbi', l’aide de Power BI CLI devrait s’afficher.

![PowerBIcli](/content/images/2017/03/powerbicli.png)

Il est temps d’utiliser la clé d’accès que nous avons obtenue précédemment et de l’utiliser dans cette commande pour créer un workspace dans notre workspace collection.

    //== Create Workspace ===========
    powerbi create-workspace -c FrankWrkSpcCollection -k my_azure_workspace_collection_access_key

Maintenant, nous allons télécharger vers Azure, notre fichier .pbix. Récupérez l’ID du workspace créé par la commande précédente et passez-le comme paramètre -w (workspace). Si vous désirez écraser un workspace déjà présent dans Azure, ajouter le paramètre -o (override) à la commande.

    //== Import ===========
    powerbi import -c FrankWrkSpcCollection -w workspaceId -k my_azure_workspace_collection_access_key -f "C:\PowerBIdemo\CustomerInvoices.pbix" -n CustomerInvoices 

Maintenant, nous devons mettre à jour la connectionstring  de notre dataset. Obtenez son ID avec la commande suivante:

    //== Get-Datasets ===========
    powerbi get-datasets -c FrankWrkSpcCollection -w workspaceId -k my_azure_workspace_collection_access_key 

Mettez à jour la connectionstring, en passant le datasetId avec le paramètre `-d`:

    //== update-connection ===========
    powerbi update-connection -c FrankWrkSpcCollection -w workspaceId -k my_azure_workspace_collection_access_key -d 01fcabb6-1603-4653-a938-c83b7c45a59c -u usename@servername -p password

Dans Visual Studio
----------------

Toute la préparation concernant Power BI Embedded est maintenant terminée. Créer une nouvelle application Web Asp.Net MVC. Quelques paquets Nuget qui seront nécessaires, assurez-vous d’avoir ces versions ou plus récentes :

  - Microsoft.PowerBI.AspNet.Mvc version="1.1.7" 
  - Microsoft.PowerBI.Core version="1.1.6"
  - Microsoft.PowerBI.JavaScript version="2.2.6" 
  - Newtonsoft.Json version="9.0.1" 

Par défaut, Newtonsoft.Json est déjà inclus, mais n’est pas à la bonne version. Une mise à jour est requise avec :

    Update-Package Newtonsoft.Json

En installant le paquet NuGet Microsoft.PowerBI, la commande d’installation devrait prendre soin de toutes les autres dépendances.

    Install-Package Microsoft.PowerBI.AspNet.Mvc

Le Web App aura aussi besoin de se connecter au Workspace Collection, ajouter toutes les informations d’accès que nous avons utilisé précédemment avec powerbi-cli, dans le web.config de l’application.

    ...
    <appSettings>
        <add key="powerbi:AccessKey" value="my_azure_workspace_collection_access_key" />
        <add key="powerbi:ApiUrl" value="https://api.PowerBI.com" />
        <add key="powerbi:WorkspaceCollection" value="FrankWrkSpcCollection" />
        <add key="powerbi:WorkspaceId" value="01fcabb6-1603-4653-a938-c83b7c45a59c" />
    </appSettings>
    ...

Voici le code du InvoicesController:

    using System;
    using System.Configuration;
    using System.Linq;
    using System.Web.Mvc;
    using demoPowerBIEmbedded.Models;
    using Microsoft.PowerBI.Api.V1;
    using Microsoft.PowerBI.Security;
    using Microsoft.Rest;

    namespace demoPowerBIEmbedded.Controllers
    {
        public class InvoicesController : Controller
        {
            private readonly string workspaceCollection;
            private readonly string workspaceId;
            private readonly string accessKey;
            private readonly string apiUrl;

            public InvoicesController()
            {
                this.workspaceCollection = ConfigurationManager.AppSettings["powerbi:WorkspaceCollection"];
                this.workspaceId = ConfigurationManager.AppSettings["powerbi:WorkspaceId"];
                this.accessKey = ConfigurationManager.AppSettings["powerbi:AccessKey"];
                this.apiUrl = ConfigurationManager.AppSettings["powerbi:ApiUrl"];
            }

            private IPowerBIClient CreatePowerBIClient
            {
                get
                {
                    var credentials = new TokenCredentials(accessKey, "AppKey");
                    var client = new PowerBIClient(credentials)
                    {
                        BaseUri = new Uri(apiUrl)
                    };

                    return client;
                }
            }

            public ReportViewModel GetFilteredRepot(string clientName)
            {
                using (var client = this.CreatePowerBIClient)
                {
                    var reportsResponse = client.Reports.GetReportsAsync(this.workspaceCollection, this.workspaceId);
                    var report = reportsResponse.Result.Value.FirstOrDefault(r => r.Name == "CustomerInvoices");
                    var embedToken = PowerBIToken.CreateReportEmbedToken(this.workspaceCollection, this.workspaceId, report.Id, clientName, new string[] { "CustomerRole" });

                    var model = new ReportViewModel
                    {
                        Report = report,
                        AccessToken = embedToken.Generate(this.accessKey)
                    };

                    return model;
                }
            }

            public ActionResult Index()
            {
                var report = GetFilteredRepot("Action Bicycle Specialists");
                return View(report);
            }
        }
    }

La partie intéressante de ce contrôleur est dans la méthode GetFilteredRepot. Tout d'abord, il va chercher tous les rapports contenus dans notre workspaces et recherchera celui nommé: "CustomerInvoices". L'étape suivante bouclera la boucle; la création du jeton. Bien sûr, nous passons les références workpacecollection, workpace et rapport.  On pourrait s'en tenir là, et tout fonctionnerait. Effectivement, en passant seulement ces références tous les clients seraient affichés sur notre rapport. Puisque ce n'est pas ce que nous voulons en ce moment, deux autres paramètres sont utilisés. Les deux derniers paramètres sont le nom d'utilisateur et une liste (*array*) de rôles. Nous devons utiliser le rôle créé précédemment dans Power BI Desktop, nommé `CustomerRole` qui était égal à la variable `USERNAME()`. Donc ici nous allons passer le nom du client comme *Username* et préciser que nous voulons utiliser le rôle "CustomerRole".

La dernière pièce pour le puzzle est la vue, alors ajoutons en une.

    @model demoPowerBIEmbedded.Models.ReportViewModel

    <style>iframe {border: 0;border-width: 0px;}</style>

    <div id="test1" style="border-style: hidden;">
        @Html.PowerBIReportFor(m => m.Report, new { id = "pbi-report", style = "height:85vh", PowerBI_access_token = Model.AccessToken })
    </div>


    @section scripts
    {
        <script src="~/Scripts/PowerBI.js"></script>
        <script>
            $(function () {

                var reportConfig = {
                    settings: {
                        filterPaneEnabled: false,
                        navContentPaneEnabled: false
                    }
                };

                var reportElement = document.getElementById('pbi-report');
                var report = PowerBI.embed(reportElement, reportConfig);
            });
        </script>
    }

Un avantage d’utiliser Asp.Net MVC est que nous avons un `@Html.PowerBIReportFor` à notre disposition qui simplifie l’écriture. Ensuite, nous pouvons instancier le rapport avec l’appel de `PowerBI.embed(reportElement, reportConfig);`. Dans le cas présent, je passe une configuration pour supprimer la navigation, et les volets des filtres, mais séchez que c’est facultatif.

Maintenant, si nous exécutons notre projet, vous devriez avoir un résultat qui ressemble à ceci.

![finalresult](/content/images/2017/03/finalresult.png)


Conclusion
----------

Voilà! Bien sûr, ceci une démo et devrait être optimisé. N’hésitez pas à laisser un commentaire ou à me contacter, si vous avez des questions. C’est toujours plaisir de discuter avec vous.

#### References:

- [Get started with Azure](https://azure.microsoft.com/en-ca/get-started/)
- [Power BI Desktop](https://PowerBI.microsoft.com/en-us/desktop/)
- [Power BI Embedded](https://azure.microsoft.com/en-us/services/power-bi-embedded/)
- [Microsoft/PowerBI-Cli](https://github.com/Microsoft/PowerBI-Cli)



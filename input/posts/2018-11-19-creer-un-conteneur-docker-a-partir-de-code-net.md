---
title: Créer un conteneur Docker à partir de code .Net
permalink: /creer-un-conteneur-docker-a-partir-de-code-net
Published: 2018-11-19
tags: [cloud, azure, post, function, container, aci, containerinstance, dotnet, dotnetcore, fluent]
---

Pour un projet que je viens de démarrer, je dois créer des ressources Azure à partir de code. En fait, je veux créer une instance de conteneur Azure (ACI). Je sais qu'il est possible de créer un conteneur à partir de Logic Apps et Azure CLI / PowerShell, mais je cherchais à le créer dans une Fonction Azure. Après une rapide recherche en ligne, j'ai trouvé *Azure Management Libraries for .NET* (également appelée Fluent API), un projet disponible sur GitHub qui fait exactement cela (et tellement plus encore)! 

Dans cet billet, je partagerai avec vous le fonctionnement de cette librairie.

#### Le but

Pour ce démo, je vais créer une application console en .Net Core qui crée une instance ACI (Azure Containter Instance). De cette façon je limite la complexité du code et il sera très facile par la suite de prendre ce code et de migrer vers une fonction Azure ou ailleurs.

#### L'application console

Créons une application console simple avec la commande suivante:

    dotnet new console -o AzFluentDemo
    cd AzFluentDemo
    dotnet add package microsoft.azure.management.fluent

La dernière commande utilisera le paquet Nuget disponible en ligne et l’ajoutera à notre solution. Nous avons maintenant besoin d'un "*service principal*" pour que notre application puisse accéder à l'abonnement Azure. Une façon de créer depuis est d'utiliser Azure CLI

    az ad sp create-for-rbac --sdk-auth > my.azureauth
    
Cela créera un "*service principal*" (SP) d'Active Directory (AD) et enregistrera le contenu dans le fichier `my.azureauth`. Parfait, ouvrez maintenant la solution. Pour ce type de projet, j'aime utiliser le Visual Studio Code, donc un petit `code .` fera très bien l;e travaille. Remplacez le contenu du fichier *Program.cs* par le code suivant.

    using System;
    using Microsoft.Azure.Management.Fluent;
    using Microsoft.Azure.Management.ResourceManager.Fluent;
    using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
    namespace AzFluentDemo
    {
        class Program
        {
            static void Main(string[] args)
            {
                string authFilePath = "/home/frank/Dev/AzFluentDemo/my.azureauth";
                string resourceGroupName  = "cloud5mins";
                string containerGroupName = "frank-containers";
                string containerImage  = "microsoft/aci-helloworld";
                // Set Context
                IAzure azure = Azure.Authenticate(authFilePath).WithDefaultSubscription();
                ISubscription sub;
                sub = azure.GetCurrentSubscription();
                Console.WriteLine($"Authenticated with subscription '{sub.DisplayName}' (ID: {sub.SubscriptionId})");
                // Create ResoureGroup
                azure.ResourceGroups.Define(resourceGroupName)
                    .WithRegion(Region.USEast)
                    .Create();
                // Create Container instance
                IResourceGroup resGroup = azure.ResourceGroups.GetByName(resourceGroupName);
                Region azureRegion = resGroup.Region;
                // Create the container group
                var containerGroup = azure.ContainerGroups.Define(containerGroupName)
                    .WithRegion(azureRegion)
                    .WithExistingResourceGroup(resourceGroupName)
                    .WithLinux()
                    .WithPublicImageRegistryOnly()
                    .WithoutVolume()
                    .DefineContainerInstance(containerGroupName + "-1")
                        .WithImage(containerImage)
                        .WithExternalTcpPort(80)
                        .WithCpuCoreCount(1.0)
                        .WithMemorySizeInGB(1)
                        .Attach()
                    .WithDnsPrefix(containerGroupName)
                    .Create();
                Console.WriteLine($"Soon Available at http://{containerGroup.Fqdn}");
            }
        }
    }

À la première ligne, je déclare quelques constantes. Le chemin du *service principal* créé précédemment, le nom du groupe de ressources, le nom du groupe de conteneurs et l'image que j'utiliserai. Pour cette démo "aci-helloworld". Ensuite, nous obtenons un accès avec `Azure.Authenticate`.

Une fois que nous avons eu accès, c’est facile et l’intellisense est fantastique! Je ne pense pas avoir besoin d'expliquer le reste du code car il est très explicite. 

#### Vous avez une erreur?

En cours d’exécution, il est possible que vous ailliez une erreur indiquant que le namespace n’est pas été enregistré ou quelque chose du genre (désolé, je n’ai pas noté le message d’erreur). Il vous suffit de l'enregistrer avec la commande

    az provider register --namespace Microsoft.ContainerInstance

Cela prendra quelques minutes. Pour voir si c'est terminé, vous pouvez exécuter cette commande:

    az provider show -n Microsoft.ContainerInstance --query "registrationState" 

#### Conclusion

Et voilà, le tour est joué! Si vous lancez la commande `dotnet run`, après une minute ou deux, vous aurez une nouvelle application Web s'exécutant dans un conteneur disponible à partir de `http://frank=containers.eastus.azurecontainer.io`.

![hello-container](/content/images/2018/11/hello-container.png)

Il est maintenant très facile de prendre ce code et de le migrer dans une fonction Azure ou à n’importe quelle application .Net Core s'exécutant n’importe où (Linux, Windows, Mac OS, Web, conteneurs, etc.)!

#### En vidéo s’il vous plaît

J’ai aussi une vidéo de ce post si vous préférez.

<div class="container">
<iframe  class="youtubevideo" width="560"  src="https://www.youtube.com/embed/F4mydirLpyQ" frameborder="0" allow="autoplay; encrypted-media" allowfullscreen></iframe>
</div>


###### Références

- [Azure Management Libraries for .NET](https://github.com/Azure/azure-libraries-for-net)
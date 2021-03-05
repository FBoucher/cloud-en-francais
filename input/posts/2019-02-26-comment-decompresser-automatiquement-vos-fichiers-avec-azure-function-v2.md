---
title: Comment décompresser automatiquement vos fichiers avec Azure Function v2
permalink: /comment-decompresser-automatiquement-vos-fichiers-avec-azure-function-v2
Published: 2019-02-26
tags: [video, cloud, azure, post, github, function, cloud5mins, unzip]
---


J’ai publié [une vidéo](https://youtu.be/TEuEmdPOdcs) qui explique comment décompresser des fichiers sans code à l’aide de Logic Apps. Cependant, cette solution ne fonctionnait pas pour des fichiers plus volumineux ou d’autres types d’archives. Ce post explique comment utiliser les *Azure Function* pour pallier à cette situation. Cette première itération prend en charge que les fichiers "zip".

## Prérequis

Dans le cadre de ce billet, j’utiliserai l’excellente extension Azure Function de [Code Visual Studio](https://code.visualstudio.com/). Ce n’est pas obligatoire, mais comme vous le verrez elle simplifie grandement les choses.

![AzureFunction_extension](/content/images/2019/02/AzureFunction_extension.png)

Vous pouvez facilement installer l’extension à partir de Visual Studio Code en cliquant sur le bouton d’extension dans le menu de gauche. Vous devrez également installer [Azure Function Core Tools](https://docs.microsoft.com/en-ca/azure/azure-functions/functions-run-local).

## Créer la fonction

Une fois l’extension installée, vous trouverez un nouveau bouton dans le menu de gauche. Cela ouvre une nouvelle section avec quatre nouvelles options: Create New Project, Create Function, Deploy to Function App, et Refresh.

![Extension_options](/content/images/2019/02/Extension_options.png)

Cliquez sur la première option **Create New Project**. Sélectionnez un dossier local et un lalanguage de programmation; j’utiliserai C# pour le cas présent. Cela créera quelques fichiers et dossiers. Créons maintenant notre fonction. Dans le menu de l’extension, sélectionnez la deuxième option **Create Function**. Créez un "Blob Trigger" nommé **UnzipThis** dans le dossier que nous venons de créer et sélectionnez (ou créez) un *Resource Group*, *Storage Account* et sélectionné une location. Après quelques secondes, une autre question apparaîtra, demandant le nom du conteneur à surveillé par notre déclencheur (Blob Trigger). Pour cette démo, `input-files` est utilisé.

Une fois la fonction créée, vous verrez ce message d’avertissement.

![All_non_HTTP](/content/images/2019/02/All_non_HTTP.png)

Cela signifie que pour pouvoir déboguer localement, nous devons définir la valeur du paramètre `AzureWebJobsStorage` à `UseDevelopmentStorage=true` dans le fichier **local.settings.json**. Il ressemblera à ceci.

    {
        "IsEncrypted": false,
        "Values": {
            "AzureWebJobsStorage": "UseDevelopmentStorage=true",
            "FUNCTIONS_WORKER_RUNTIME": "dotnet",
            "unziptools_STORAGE": "DefaultEndpointsProtocol=https;AccountName=unziptools;AccountKey=XXXXXXXXX;EndpointSuffix=core.windows.net",
        }
    }

Ouvrez le fichier **UnzipThis.cs**; c’est notre fonction. Sur la première ligne de la fonction, vous pouvez voir que le déclencheur est défini.

    [BlobTrigger("input-files/{name}", Connection = "cloud5mins_storage")]Stream myBlob

La liaison (binding) est attachée au conteneur nommé **input-files**, à partir du *Storage Account* accessible par la connexion "cloud5mins_storage". La véritable chaîne de connexion se trouve dans le fichier **local.settings.json**.

Maintenant, ajoutons le code dont nous avons besoin pour notre démo:

    [FunctionName("UnzipThis")]
    public static void Run([BlobTrigger("input-files/{name}", Connection = "cloud5mins_storage")]Stream myBlob, string name, ILogger log)
    {
        log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

        string destinationStorage = Environment.GetEnvironmentVariable("destinationStorage");
        string destinationContainer = Environment.GetEnvironmentVariable("destinationContainer");

        if(name.Split('.').Last().ToLower() == "zip"){

            ZipArchive archive = new ZipArchive(myBlob);

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(destinationStorage);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(destinationContainer);

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                log.LogInformation($"Now processing {entry.FullName}");

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(entry.Name);
                using (var fileStream = entry.Open())
                {
                    blockBlob.UploadFromStreamAsync(fileStream);
                }
            }
        }
    }

La source de notre fichier compressé est définie dans le déclencheur. Pour définir la destination, `destinationStorage` et `destinationContainer` sont utilisés. Leurs valeurs sont enregistrées dans **local.settings.json**. Ensuite, comme cette fonction ne prend en charge que le fichier **.zip**, une petite validation était nécessaire.

Ensuite, nous créons une instance d’archive en utilisant la nouvelle bibliothèque **System.IO.Compression**. Nous créons des références au compte de stockage, au blob et au conteneur. Il n’est pas possible d’utiliser un deuxième binding ici, car vous disposez d’un nombre potentiellement variable de fichiers sortant pour chaque fichier d’archive. Les binding sont statiques; par conséquent, nous devons utiliser l’API de stockage standard.

Ensuite, pour chaque fichier contenu dans l’archive, le code ­­télécharge le fichier dans le stockage défini comme destination.

## Déploiement

Pour déployer la fonction, à partir de l’extension Azure Function, cliquez sur la troisième option: **Deploy to Function App**. Sélectionnez votre abonnement et le nom de votre Function App.

Nous devons maintenant configurer nos paramètres dans Azure. Par défaut, les paramètres locaux (contenu dans le fichier local.settings.json) ne sont PAS utilisés. Une fois encore, l’extension nous simplifiera la tâche.

![AddSetting](/content/images/2019/02/AddSetting.png)

Sous l’abonnement, développez la Function App «AzUnzipEverything», qui vient d’être déployée, et cliquez avec le bouton droit de la souris sur Application Settings. Utilisez **Add New Setting** pour créer `cloud5mins_storage`, `destinationStorage` et `destinationContainer`.

La fonction est maintenant déployée et les paramètres sont définis, il ne reste plus qu’à créer les conteneurs de stockage dans notre Blob storage, et nous pourrons tester la fonction. Vous pouvez facilement le faire directement depuis le portail Azure (portal.azure.com).

Vous êtes maintenant prêt à télécharger un fichier dans le conteneur `input-files`.


## Codons ensemble

Cette première itération supporte que les fichiers "zip". Tout le code est disponible sur GitHub. Sentez-vous libre de l’utiliser. Si vous souhaitez voir ou ajouter un support pour d’autres types d’archives, rejoignez-moi sur [GitHub](https://github.com/FBoucher/AzUnzipEverything)!.


## En vidéo s’il vous plaît

J’ai aussi une vidéo de ce post si vous préférez.

<iframe allow="autoplay; encrypted-media" allowfullscreen="" frameborder="0" height="315" src="https://www.youtube.com/embed/WLqQl_6eXRg" width="560"></iframe>  

Et aussi une version allongée dans laquelle je parles plus longtemps de Visial Studio Code et des Azure Functions V2.

<iframe allow="autoplay; encrypted-media" allowfullscreen="" frameborder="0" height="315" src="https://www.youtube.com/embed/k_1gce9JAeg" width="560"></iframe>  
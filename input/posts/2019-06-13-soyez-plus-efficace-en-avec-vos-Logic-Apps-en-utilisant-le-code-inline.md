---
title: Soyez plus efficace en avec vos Logic Apps en utilisant le Code Inline
Published: 2019-06-13
permalink: /soyez-plus-efficace-en-avec-vos-Logic-Apps-en-utilisant-le-code-inline
tags: [azure,cloud,post,cloud5mins,video,logic app,integration,serverless,inline code]
---
Dans un projet utilisant des Azure Logic Apps, sur lequel je travaille en ce moment, je devais manipuler des chaînes de caractères. J'aurais très bien pu créer des APIs ou des fonctions Azure, mais le code est très simple et n'utilise aucune bibliothèque externe. Dans cet article, je vous montrerai comment utiliser le nouveau "Code Inline" pour exécuter votre code directement dans vos Logic Apps.

## Mise en situation

La logic App lira un fichier situé dans un répertoire OneDrive (cela fonctionnera également avec DropBox, Box, et les autres). Voici à quoi ressemble le fichier:

    Nice tutorial that explains how to build, using postman, an efficient API.[cloud.azure.postman.tools]

L'objectif est d'extraire du texte les "tags" contenus entre les crochets.

 ## Logic App: Lire le contenue du fichier

Depuis le portail Azure, créez une nouvelle Logic App en cliquant sur le gros bouton vert "+" dans le coin supérieur gauche et en recherchant Logic App.

Pour cette démo, j'utiliserai *Interval* comme déclencheur, car j'exécuterai l'application manuellement.

La première étape consistera en une action **Get File Content** du connecteur OneDrive. Une fois que vous avez autorisé Azure à accéder à vos dossiers OneDrive, sélectionnez le fichier que vous souhaitez lire. Pour moi, c'est `/dev/simpleNote.txt`

## Compmte d'Intégration

Pour accéder au **workflowContext**, la Logic App aura besoin d'un compte d'intégration. La prochaine étape serait d'en créer un. Enregistrez l'application actuelle et cliquez sur le gros bouton "+" dans le coin supérieur droit. Cette fois, recherchez l'intégration. Sélectionnez **Integration Account** et remplissez le formulaire pour le créer.

![SetIntegrationAccount][SetIntegrationAccount]

Nous devons maintenant l'assigner à notre Logic App. Dans la liste des options, présente sur la blade de la Logic App, sélectionnez *Workflow Settings*. Ensuite, sélectionnez votre compte d'intégration et n'oubliez pas de sauvegarder!

## Logic App: Inline Code

Pour ajouter l'action à la fin de votre flux, cliquez sur le bouton *New step*. Recherchez **Inline Code** et sélectionnez l'action **Execute JavaScript Code**.

![ExecuteJSCode][ExecuteJSCode]

Avant de copier-coller le code dans la nouvelle action *Inline Code*, examinons-le rapidement.

```javascript
var note = "" + workflowContext.actions.Get_file_content.outputs.body;
var posTag = note.lastIndexOf("[") + 1;
var cleanNote = {};

if(posTag > 0){
        cleanNote.tags = note.substring(posTag, note.length-1);
        cleanNote.msg = note.substring(0,posTag-1);
    }
return cleanNote;
```

Sur la première ligne, nous assignions à la variable *note* le contenu du "output" provenant de l'action **Get_file_content**. Nous y accédons en utilisant le workflowContext. Ce contexte a accès au déclencheur et aux actions. Pour trouver le nom de l'action, vous pouvez remplacer les espaces par le caractère de soulignement "_".

![CodeView][CodeView]

Vous pouvez également passer en mode *Code View* pour voir le nom de tous les composants à partir du code JSON.

## Logic App: Utiliser les résultats du Inline Code

Bien sûr, vous pouvez utiliser le "output" de votre code d'autres étapes. Vous devez simplement utiliser le *Result* du menu de contenu dynamique.

![SetVariable][SetVariable]

Si, pour une raison quelconque, la liste de contenu dynamique ne contient pas votre Step Code Inline, vous pouvez toujours l'ajouter directement avec le code suivant: `@body('Cleaning_Note')?['body']`.

![CodeInCase][CodeInCase]

Voici la logic App dans son état final.

![final_LogicApp][final_LogicApp]

## Verdict

La nouvelle fonctionnalité *Inline code* semble très prometteuse. Pour le moment, elle est limitée à JavaScript et ne peut accéder ni aux variables ni aux boucles. [Vous pouvez en savoir plus sur ce qui est exactement couvert ou pas ici] (http://bit.ly/AzLogicAppInline).

Pour du code simple ne nécessitant aucune référence, l'utilisation de *Inline code* permet de gagner du temps et est plus facile puis que cela nous évite d'avoir a à gérer et à déployer d'autre API et Azure Functions.

![RunResults][RunResults]

## Vous préférez visionner à lire

J'ai aussi une vidéo de ce post si vous préférez.
<iframe allow="autoplay; encrypted-media" allowfullscreen="" frameborder="0" height="315" src="https://www.youtube.com/embed/0k59u2rIBGE" width="560"></iframe> 


##### Références

- Microsoft Doc Inline Code .........  http://bit.ly/AzLogicAppInline
- Microsoft Doc Azure Logic Apps ....  http://bit.ly/AzLogicApp
- Microsoft Doc Integration Account .  http://bit.ly/IntegrationAccount
- If you don't have an Azure subscription, [sign up for a free Azure account](https://azure.microsoft.com/free/?WT.mc_id=cloud5mins-youtube-frbouche).

[SetIntegrationAccount]:  /content/images/2019/06/SetIntegrationAccount.png "Assign the Integration Account"
[ExecuteJSCode]:  /content/images/2019/06/ExecuteJSCode.png "Add the Execute JavaScript code task"
[SetVariable]:  /content/images/2019/06/SetVariable.png "Set Variable"
[CodeView]:  /content/images/2019/06/CodeView.png "Code View"
[CodeInCase]:  /content/images/2019/06/CodeInCase.png "When the output is not there"
[final_LogicApp]:  /content/images/2019/06/final_LogicApp.png "Final Result"
[RunResults]: /content/images/2019/06/RunResults.gif "Run Result"


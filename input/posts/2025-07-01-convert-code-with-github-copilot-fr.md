---
title: "Comment convertir du code avec GitHub Copilot, l'IA peut-elle vraiment aider ?"
Published: 2025-07-01
categories: post
tags: [ai,github,copilot,cobol,java,devcontainers,convert,tools]
---


Récemment, quelqu'un m'a posé une question intéressante: "GitHub Copilot ou l'IA peuvent-ils m'aider à convertir une application d'un langage à un autre ?" Ma réponse était un oui catégorique! L'IA peut non seulement aider à écrire du code dans un nouveau langage, mais elle peut aussi améliorer la collaboration en équipe et combler le manque d'expérience des développeurs qui connaissent différents langages de programmation.

## Configuration de l'environnement

Pour démontrer cette capacité, j'ai décidé de convertir une application COBOL en Java. Un cas de test parfait puisque je ne connais bien aucun de ces deux langages, ce qui signifie que j'avais vraiment besoin de GitHub Copilot pour faire le gros du travail. Tout le code est disponible sur [GitHub](https://github.com/FBoucher/hello_business/tree/demo).

La première étape était de configurer un environnement de développement approprié. J'ai utilisé un [dev container](https://github.com/FBoucher/hello_business/tree/demo/.devcontainer) et j'ai demandé à Copilot de m'aider à le construire. J'ai aussi demandé des recommandations sur les [meilleures extensions VS Code pour le développement Java](https://marketplace.visualstudio.com/items?itemName=vscjava.vscode-java-pack). En quelques minutes seulement, j'avais un environnement entièrement configuré et prêt pour le développement Java.

## Choisir le bon agent Copilot

Lorsque vous travaillez avec GitHub Copilot pour la conversion de code, vous avez différents mode parmi lesquels choisir :

- **Ask** : Excellent pour les questions générales (comme demander des extensions Java)
- **Edit** : Parfait pour l'édition simple de documents (comme modifier le code généré)
- **Agent** : Le plus puissant pour les tâches complexes impliquant plusieurs fichiers, imports et changements structurels

Pour les projets de conversion de code, l'Agent est votre meilleur ami. Il peut examiner différents fichiers sources, comprendre la structure du projet, éditer le code et même créer de nouveaux fichiers pour vous.

## Le processus de conversion

J'ai utilisé Claude 3.5 Sonnet pour cette conversion. Voici la simple instruction que j'ai utilisée :

> "Convertis cette application COBOL hello business en Java"

Copilot n'a pas seulement converti le code, il a aussi fourni des informations détaillées sur comment exécuter l'application Java, ce qui était inestimable puisque je n'avais aucune expérience en Java.

Les résultats variaient selon le modèle d'IA utilisé (Claude, GPT, Gemini, etc.), mais la fonctionnalité de base restait cohérente à travers différentes tentatives. Comme l'application originale était simple, je l'ai convertie plusieurs fois en utilisant différentes instructions et modèles pour tester la cohérence. Parfois, il générait un seul fichier, d'autres fois il créait plusieurs fichiers : une application principale et une classe Employee (qui n'était pas dans ma version COBOL originale). Parfois, il mettait à jour le `Makefile` pour permettre la compilation et l'exécution en utilisant `make`, tandis que d'autres fois il fournissait des instructions pour utiliser directement les commandes `javac` et `java`.

Cette variabilité est attendue avec l'IA générative—les résultats diffèrent entre les exécutions, mais la fonctionnalité de base reste fiable.

## Défis du monde réel

Bien sûr, la conversion n'était pas parfaite du premier coup. Par exemple, j'ai rencontré des erreurs d'exécution lors du lancement de l'application. Le problème était avec le format des données—le fichier original utilisait un format de fichier plat avec des enregistrements de longueur fixe (19 caractères par enregistrement) et sans saut de ligne.

Je suis retourné vers Copilot, j'ai mis en évidence le message d'erreur du terminal, et j'ai fourni un contexte supplémentaire sur le format d'enregistrement de 19 caractères. Cette approche itérative est la clé du succès de la conversion assistée par IA.

> "Ça ne fonctionne pas comme prévu, vérifie l'erreur dans #terminalSelection. Les enregistrements ont une longueur fixe de 19 caractères sans saut de ligne. Ajuste le code pour gérer ce format"

## Les résultats

Après les améliorations itératives, mon application Java a réussi à :
- Compiler sans erreurs
- Traiter tous les enregistrements d'employés
- Générer un rapport avec les données des employés
- Calculer le salaire total (un ajout sympa qui n'était pas dans l'original)

Bien que le format de sortie ne soit pas identique à la version COBOL originale (zéros de tête manquants, espacement de ligne différent), la fonctionnalité de base était préservée.

## Démonstration vidéo

Regardez le processus de conversion complet en action (vidéo en anglais) :


<iframe width="560" height="315" src="https://www.youtube.com/embed/i5RdOPA-waQ?si=wjyvYF_HQ44MMIz3" title="YouTube video player" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture; web-share" referrerpolicy="strict-origin-when-cross-origin" allowfullscreen></iframe>


## Meilleures pratiques pour la conversion de code assistée par IA

Basé sur cette expérience, voici mes recommandations :

### 1. Commencer par de petits morceaux

N'essayez pas de convertir des milliers de lignes d'un coup. Divisez votre conversion en modules ou fonctions gérables.

### 2. Établir des standards de projet

Considérez créer un dossier `.github` à la racine de votre projet avec un fichier `instructions.md` contenant :
- Les meilleures pratiques pour votre langage cible
- Les modèles et outils à utiliser
- Les versions et frameworks spécifiques
- Les standards d'entreprise à suivre

### 3. Rester impliqué dans le processus

Vous n'êtes pas seulement un spectateur - vous êtes un participant actif. Révisez les changements, testez la sortie, et fournissez des commentaires quand les choses ne fonctionnent pas comme prévu.

### 4. Itérer et améliorer

N'attendez pas la perfection du premier coup. Dans mon cas, l'application convertie fonctionnait mais produisait un formatage de sortie légèrement différent. C'est normal et attendu, après tout vous convertissez entre deux langages différents avec des conventions et styles différents.

## L'IA peut-elle vraiment aider avec la conversion de code ?

**Absolument, oui !** GitHub Copilot peut significativement :

- Accélérer le processus de conversion
- Aider avec la syntaxe et les modèles spécifiques au langage
- Fournir des conseils sur l'exécution et la compilation du langage cible
- Combler les lacunes de connaissances entre les membres de l'équipe
- Générer des fichiers de support et de la documentation

Cependant, rappelez-vous que c'est de l'IA générative, les résultats varieront entre les exécutions, et vous ne devriez pas vous attendre à une sortie identique à chaque fois.

## Réflexions finales

GitHub Copilot est définitivement un outil dont vous avez besoin dans votre boîte à outils pour les projets de conversion. Il ne remplacera pas le besoin de supervision humaine et de tests, mais il accélérera dramatiquement le processus et aidera les équipes à collaborer plus efficacement à travers différents langages de programmation.

La clé est d'aborder cela comme un processus collaboratif où l'IA fait le gros du travail pendant que vous fournissez des conseils, du contexte et l'assurance qualité. Commencez petit, itérez souvent, et n'ayez pas peur de demander des clarifications ou des corrections quand la sortie n'est pas tout à fait correcte.

Avez-vous essayé d'utiliser l'IA pour la conversion de code ? J'aimerais entendre parler de vos expériences dans les commentaires ci-dessous !

#### Références

- [Tutoriel Dev Containers](https://code.visualstudio.com/docs/devcontainers/tutorial)
- [Ajouter des instructions personnalisées de dépôt pour GitHub Copilot](https://docs.github.com/copilot/how-tos/custom-instructions/adding-repository-custom-instructions-for-github-copilot)

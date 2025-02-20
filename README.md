L'article associé à ce repository est trouvable sur le blog Webnet : Intégrer de l'IA dans une application .NET

**RETRANSCRIPTION :**

L'Intelligence Artificielle est de plus en plus présente dans notre quotidien, que ce soit pour accompagner un développeur, améliorer la qualité des photos ou même pour laver plus efficacement le linge. Tout le monde connait l'IA, qui vit actuellement un vrai "boom" depuis quelques années, et dont les états et entreprises technologiques investissement par dizaine voir centaine de milliards.

Elle devient aujourd'hui incontournable pour aussi bien améliorer la productivité que la qualité, mais aussi en tant que vrai argument commercial. En tant que développeur .NET nous avons la chance que GitHub Copilot soit utilisable gratuitement au sein de Visual Studio et que Microsoft y propose déjà tout un écosystème.

Mais au juste, est-ce compliqué de fournir des services basés sur l'IA ?

# Cas d'usage

Nous travaillons pour une compagnie d'assurance et nous souhaitons mettre en place un simple formulaire qui permette aux utilisateurs d'envoyer tout type de demande, comme des réclamations.

Le formulaire doit être simple et ne contenir qu'un unique champ de contenu qui est une zone de saisie de texte.

Une fois envoyée, la demande doit ensuite pouvoir être traité "intelligemment", c'est-à-dire qu'en fonction de son contenu il devra être automatiquement assigné aux bons interlocuteurs et priorisé. Cette simplicité pour l'utilisateur (simple saisie) ne doit donc pas être un frein pour son intégration au sein des process de la compagnie d'assurance.

# Aspects techniques

Dans le cadre de cet article, le "moteur IA" sera développé en C# et exécuté au sein d'une application console.

L'entreprise souhaite avoir la possibilité de changer de modèle IA au besoin, et sans nécessité de modifier l'application. Elle pourra ainsi à sa guise changer de partenaire, par exemple sur des aspects financiers ou sécuritaires, voir intégrer un système de fallback pour pallier à l'indisponibilité éventuelle d'un service. Il est donc plus pertinent de privilégier ici un SDK générique plutôt que d'appeler directement les API de chaque service.

# Principe de fonctionnement

Lorsque l'utilisateur transmettra sa demande, l'IA analysera automatiquement le contenu pour :

-Ajouter un titre résumé (afin d'avoir un aperçu du contenu)
-Assigner au service interne le plus compétent pour traitement (afin d'améliorer la qualité d'assignation)
-Extraire toute information de contact ou nominative (afin de pouvoir assigner individuellement en base de données)
-Détecter les éventuelles références à des numéros de dossier (afin de pouvoir les associer en base de données)
-Déterminer le degré de priorisation de la requête) (afin de répondre au plus vite à celles jugées importantes
-Convertir le contenu en markdown (afin d'améliorer le visuel)

La réponse de l'IA devra respecter un format de retour bien précis afin de correctement exploiter le résultat produit, mais aussi pour qu'il soit toujours identique quel que soit le modèle utilisé.

# Rédaction du prompt

Voici le prompt générique envoyé à l'IA afin de le guider sur son rôle précis et le résultat attendu qui sera au format JSON :

> Tu es un assistant et vas devoir aider à interpréter une demande d'un client d'une compagnie d'assurance.
> Tu renverras les résultats de ton analyse au format JSON en respectant la structure C# ci-dessous, dont tu y trouveras dans le code la description des règles pour chaque champ afin de t'aider à produire le meilleur résultat.
> Tu dois insérer les balises <JSON> et </JSON> pour délimiter le JSON à renvoyer.
> Pour les propriétés enums (TypeService, CategorieProbleme et Priorisation) tu dois absolument renvoyer leur valeur entière.

A la suite de ces consignes sera automatiquement ajouté le contenu des fichiers techniques. On guide donc l'IA sur le contexte, ce qu'il doit faire, et le résultat attendu.

# Obtention d'une clé

Notre application C# utilisera Azure AI Foundry, dont le SDK nous permettra d'utiliser et d'abstraire différents modèles populaires comme Mistral, Llama ou encore DeepSeek. Les démos utiliseront :

-Ministral-3B
-Mistral-Large-2411
-Meta-Llama-3.1-405B-Instruct
-Phi-3-medium-128k-instruct

Pour éviter d'exécuter l'IA localement sur sa machine et essayer gratuitement, il faut aller générer un jeton d'accès personnel sur GitHub. Vous devrez y sélectionner une durée de validité, les information de nommage et du scope d'accès étant facultatives pour notre utilisation.

Le catalogue des différents modèles est accessible sur Azure AI Foundry qui permet aussi de rechercher que de comparer, aussi bien niveau performance que tarification.

# Code C#

Créer une simple application console C#.

## Dépendances

Ajouter la librairie NuGet Microsoft.Extensions.AI.AzureAIInference :

`dotnet add package Microsoft.Extensions.AI.AzureAIInference`

## AI runner

Il permet la séparation de la logique utilisant l'IA en l'intégrant au sein d'un service dédié et facilement injectable.

Se référer au code source...

Ici le code va extraire le JSON et le désérialiser en C# afin de pouvoir l'exploiter.

## Programme

Application console utilisant l'AI runner pour traiter plusieurs requêtes :

Se référer au code source...

# Démo et résultats

Voici les résultats des 4 inputs traités par DeepSeek :

## Cas 1 : Virement non-reçu

> Bonjour,
> Suite à mon dernier échange téléphonique de la semaine dernière avec Mr Martin, je n'ai toujours pas reçu votre virement qui était censé être versé sous 48h.
> Je n'ai pas réussi à vous joindre, merci de revenir vers moi au 0601020304 ou p.martin@gmail.com.
> Cordialement,
> Pierre Martin

Voici les résultats des 4 modèles utilisés :

Voir code source dans répertoire /output...

Les 4 savent parfaitement extraire les informations, en revanche ils ne sont pas tous aussi efficace pour :

- déterminer les bonnes catégorisations du message
- définir un titre
- le contenu est parfois tronqué (il faudrait surement affiner le prompt)

## Cas 2 : Personne impatiente

> Ca commence à bien faire, quand mon dossier sera traité ?!?

Voici les résultats des 4 modèles utilisés :

Voir code source dans répertoire /output...

Etrangement Ministral-3B invente ici des données d'extraites.

## Cas 3 : Suivi de dossier

> Bonjour,
> J'aimerais savoir où en est mon dossier de sinistre F12345.
> Bien à vous,
> Jacques

Voici les résultats des 4 modèles utilisés :

Voir code source dans répertoire /output...

Ministral-3B utilise le prénom de la personne comme une information de contact.

## Cas 4 : Candidature spontanée (langue anglaise)

> Hello,
> I would like to apply to the job offer 'Senior software developer'.
> I am available and highly motivated, at which address can I send you my resume?
> Kind regards,
> John SMith
> jsmith@gmail.com
> +44 (0) 123 456 789
> 1 East Street SE125EM London (UK)

Voici les résultats des 4 modèles utilisés :

Voir code source dans répertoire /output...

La plupart des résultats sont cohérents malgré la demande formulée en langue anglaise, bien que certains traduisent le contenu original en Français. Un affinage du prompt devrait permettre de gérer ce cas.

## Cas 5 : Demande incompréhensible

> Gros bisous les amis

Voici les résultats des 4 modèles utilisés :

Voir code source dans répertoire /output...

Les résultats sont un peu près identiques, bien que certains n'arrivent pas à définir un titre. Encore une fois, le lui imposer dans le prompt devrait suffire.

# Conclusion

Utiliser de l'IA peut donc se faire très simplement en quelques lignes de code, sans pour autant avoir à investir sans un modèle spécifique et y resté enfermé.

Tous les modèles n'ont pas la même expertise (génération de texte, d'image...), qualité ni tarification, c'est pourquoi il est important de choisir celui qui est le plus adapté à son besoin et de les tester dans ses propres conditions d'utilisation.

Le prompt utilisé ici est assez simple et facilement compréhensible pour l'IA, bien que le cas d'usage soit trop générique. Ils ont tous excellé dans l'extraction d'information pertinentes ce qui les rendent précieux pour détecter des informations associables par exemple en base de données (numéro de dossier, nom de la personne, emails...).

# Aller plus loin

## Streamer le retour de l'IA

Dans cet article nous avons attendu la réponse complète de l'IA avant de la traiter. Il est également possible de streamer sa réponse au fur et à mesure de sa réflexion, et donc d'avoir un début de retour au plus vite :

Exemple de streaming :

## Comprendre la réponse et les choix effectués

Les réponses du modèle DeepSeek-R1 intègrent également son raisonnement complet afin de comprendre son raisonnement voir ses suppositions. Il est disponible au sein des balises <think></think>.

Exemple de prompt avec une certaine complexité d'interprétation :

Réflexion associée de DeepSeek-R1 :

La réponse est plutôt impressionnante, il ne reste pas bloqué sur un problème et montre qu'il est capable de déterminer son propre niveau de confiance. En revenant à notre cas d'usage, on pourrait par exemple utiliser ce niveau de confiance pour basculer les demandes avec une fiabilité incertaine vers un workflow avec intervention manuelle, en y glissant par exemple en note les raisons concernant les doutes de l'IA.

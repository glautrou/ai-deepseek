L'article complet associé à ce repository est trouvable sur le blog Webnet :

=> [Intégrer de l'IA dans une application .NET](https://blog.webnet.fr/integrer-ia-dans-une-application-dotnet/)

**RETRANSCRIPTION :**

![Illustration](https://blog.webnet.fr/wp-content/uploads/2025/02/visuel-pour-Gilles2.jpg)

##### Table of Contents

- [Cas d'usage](#cas-usage)  
- [Aspects techniques](#aspects-techniques)
- [Principe de fonctionnement](#principes)
- [Rédaction du prompt](#prompt)
- [Obtention d'une clé](#cle)
- [Code C#](#code)
  - [Dépendances](#dependances)
  - [AI runner](#runner)
  - [Programme](#programme)
- [Démo et résultats](#demo)
  - [Cas 1 : Virement non-reçu](#cas1)
  - [Cas 2 : Personne impatiente](#cas2)
  - [Cas 3 : Suivi de dossier](#cas3)
  - [Cas 4 : Candidature spontanée (langue anglaise)](#cas4)
  - [Cas 5 : Demande incompréhensible](#cas5)
- [Conclusion](#conclusion)
- [Aller plus loin](#aller-plus-loin)
  - [Streamer le retour de l'IA](#stream)
  - [Comprendre la réponse et les choix effectués](#comprendre)

L'Intelligence Artificielle est de plus en plus présente dans notre quotidien, que ce soit pour accompagner un développeur, améliorer la qualité des photos ou même pour laver plus efficacement le linge. Tout le monde connait l'IA, qui vit actuellement un vrai "boom" depuis quelques années, et dont les états et entreprises technologiques investissement par dizaine voir centaine de milliards.

Elle devient aujourd'hui incontournable pour aussi bien améliorer la productivité que la qualité, mais aussi en tant que vrai argument commercial. En tant que développeur .NET nous avons la chance que GitHub Copilot soit utilisable gratuitement au sein de Visual Studio et que Microsoft y propose déjà tout un écosystème.

Mais au juste, est-ce compliqué de fournir des services basés sur l'IA ?

<a name="cas-usage"/></a>
# Cas d'usage

Nous travaillons pour une compagnie d'assurance et nous souhaitons mettre en place un simple formulaire qui permette aux utilisateurs d'envoyer tout type de demande, comme des réclamations.

Le formulaire doit être simple et ne contenir qu'un unique champ de contenu qui est une zone de saisie de texte.

Une fois envoyée, la demande doit ensuite pouvoir être traité "intelligemment", c'est-à-dire qu'en fonction de son contenu il devra être automatiquement assigné aux bons interlocuteurs et priorisé. Cette simplicité pour l'utilisateur (simple saisie) ne doit donc pas être un frein pour son intégration au sein des process de la compagnie d'assurance.

<a name="aspects-techniques"/></a>
# Aspects techniques

Dans le cadre de cet article, le "moteur IA" sera développé en C# et exécuté au sein d'une application console.

L'entreprise souhaite avoir la possibilité de changer de modèle IA au besoin, et sans nécessité de modifier l'application. Elle pourra ainsi à sa guise changer de partenaire, par exemple sur des aspects financiers ou sécuritaires, voir intégrer un système de fallback pour pallier à l'indisponibilité éventuelle d'un service. Il est donc plus pertinent de privilégier ici un SDK générique plutôt que d'appeler directement les API de chaque service.

<a name="principes"/></a>
# Principe de fonctionnement

Lorsque l'utilisateur transmettra sa demande, l'IA analysera automatiquement le contenu pour :

- Ajouter un titre résumé (afin d'avoir un aperçu du contenu)
- Assigner au service interne le plus compétent pour traitement (afin d'améliorer la qualité d'assignation)
- Extraire toute information de contact ou nominative (afin de pouvoir assigner individuellement en base de données)
- Détecter les éventuelles références à des numéros de dossier (afin de pouvoir les associer en base de données)
- Déterminer le degré de priorisation de la requête) (afin de répondre au plus vite à celles jugées importantes
- Convertir le contenu en markdown (afin d'améliorer le visuel)

La réponse de l'IA devra respecter un format de retour bien précis afin de correctement exploiter le résultat produit, mais aussi pour qu'il soit toujours identique quel que soit le modèle utilisé.

<a name="prompt"/></a>
# Rédaction du prompt

Voici le prompt générique envoyé à l'IA afin de le guider sur son rôle précis et le résultat attendu qui sera au format JSON :

> Tu es un assistant et vas devoir aider à interpréter une demande d'un client d'une compagnie d'assurance.
> Tu renverras les résultats de ton analyse au format JSON en respectant la structure C# ci-dessous, dont tu y trouveras dans le code la description des règles pour chaque champ afin de t'aider à produire le meilleur résultat.
> Tu dois insérer les balises <JSON> et </JSON> pour délimiter le JSON à renvoyer.
> Pour les propriétés enums (TypeService, CategorieProbleme et Priorisation) tu dois absolument renvoyer leur valeur entière.

A la suite de ces consignes sera automatiquement ajouté le contenu des fichiers techniques. On guide donc l'IA sur le contexte, ce qu'il doit faire, et le résultat attendu.

<a name="cle"/></a>
# Obtention d'une clé

Notre application C# utilisera la bibliothèque Microsoft.Extensions.AI (MEAI), qui nous permettra d'utiliser et d'abstraire différents services et modèles populaires comme Mistral, Llama ou encore DeepSeek. Les démos utiliseront :

- Ministral-3B
- Mistral-Large-2411
- Meta-Llama-3.1-405B-Instruct
- Phi-3-medium-128k-instruct

Pour éviter d'exécuter l'IA localement sur sa machine et l'essayer gratuitement, nous allons utiliser GitHub Models. Connectez-vous à votre compte GitHub puis générez un jeton d'accès personnel. Vous devrez y sélectionner une durée de validité, les information de nommage et du scope d'accès étant facultatives pour notre utilisation.

Si vous le préférez, vous pouvez également déployer vos propres modèles sur Azure AI Foundry, qui permet par ailleurs de rechercher et de comparer un très grand nombre de modèles, aussi bien niveau performance que tarification.

<a name="code"/></a>
# Code C#

Créer une simple application console C#.

<a name="dependances"/></a>
## Dépendances

Ajouter la librairie NuGet Microsoft.Extensions.AI.AzureAIInference :

`dotnet add package Microsoft.Extensions.AI.AzureAIInference`

<a name="runner"/></a>
## AI runner

Il permet la séparation de la logique utilisant l'IA en l'intégrant au sein d'un service dédié et facilement injectable.

Se référer au code source...

Ici le code va extraire le JSON et le désérialiser en C# afin de pouvoir l'exploiter.

<a name="programme"/></a>
## Programme

Application console utilisant l'AI runner pour traiter plusieurs requêtes :

Se référer au code source...

<a name="demo"/></a>
# Démo et résultats

Voici les résultats des 4 inputs traités par DeepSeek :

<a name="cas1"/></a>
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

<a name="cas2"/></a>
## Cas 2 : Personne impatiente

> Ca commence à bien faire, quand mon dossier sera traité ?!?

Voici les résultats des 4 modèles utilisés :

Voir code source dans répertoire /output...

Etrangement Ministral-3B invente ici des données d'extraites.

<a name="cas3"/></a>
## Cas 3 : Suivi de dossier

> Bonjour,
> J'aimerais savoir où en est mon dossier de sinistre F12345.
> Bien à vous,
> Jacques

Voici les résultats des 4 modèles utilisés :

Voir code source dans répertoire /output...

Ministral-3B utilise le prénom de la personne comme une information de contact.

<a name="cas4"/></a>
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

<a name="cas5"/></a>
## Cas 5 : Demande incompréhensible

> Gros bisous les amis

Voici les résultats des 4 modèles utilisés :

Voir code source dans répertoire /output...

Les résultats sont un peu près identiques, bien que certains n'arrivent pas à définir un titre. Encore une fois, le lui imposer dans le prompt devrait suffire.

<a name="conclusion"/></a>
# Conclusion

Utiliser de l'IA peut donc se faire très simplement en quelques lignes de code, sans pour autant avoir à investir sans un modèle spécifique et y resté enfermé.

Tous les modèles n'ont pas la même expertise (génération de texte, d'image...), qualité ni tarification, c'est pourquoi il est important de choisir celui qui est le plus adapté à son besoin et de les tester dans ses propres conditions d'utilisation.

Le prompt utilisé ici est assez simple et facilement compréhensible pour l'IA, bien que le cas d'usage soit trop générique. Ils ont tous excellé dans l'extraction d'information pertinentes ce qui les rendent précieux pour détecter des informations associables par exemple en base de données (numéro de dossier, nom de la personne, emails...).

<a name="aller-plus-loin"/></a>
# Aller plus loin

<a name="stream"/></a>
## Streamer le retour de l'IA

Dans cet article nous avons attendu la réponse complète de l'IA avant de la traiter. Il est également possible de streamer sa réponse au fur et à mesure de sa réflexion, et donc d'avoir un début de retour au plus vite :

Exemple de streaming :

```
var content = new StringBuilder();
var response = _chatClient.CompleteStreamingAsync(message);
await foreach (var item in response)
{
    content.Append(item);
    Console.Write(item.ToString().Replace("<think>", "> Réflexion :\n").Replace("</think>", "> Résultat :\n"));
}

//Extraction du résultat typé
var match = Regex.Match(content.ToString(), @"<think>(.*?)</think>\s*(.*)", RegexOptions.Singleline);
if (match.Success)
{
    //...
}
```

<a name="comprendre"/></a>
## Comprendre la réponse et les choix effectués

Les réponses du modèle DeepSeek-R1 intègrent également son raisonnement complet afin de comprendre son raisonnement voir ses suppositions. Il est disponible au sein des balises <think></think>.

Exemple de prompt avec une certaine complexité d'interprétation :

```
Je possède 10 Euros et j'en dépense 5 pour acheter un sandwich.
Combien ai-je de pomme et combien de Dollars il me reste en portefeuille ?
Donne pour dans ta réponse ton pourcentage de confiance quant au résultat.
Renvoi seulement les valeurs au format Valeur1;Valeur2;PourcentageConfiance, par exemple 50;14,30;20
```

Réflexion associée de DeepSeek-R1 :

```
Okay, let's see what this user is asking here. So they start by saying they have 10 euros and spend 5 on a sandwich. Then they ask how many apples they have and how many dollars are left in their wallet. Hmm, wait a second.
First, the initial part is straightforward: 10 euros minus 5 euros spent leaves 5 euros remaining. But the question is about dollars left. Oh, right, the user didn't mention converting euros to dollars anywhere. So there's a currency conversion missing here. Unless they had dollars in their wallet along with euros, but the problem states they had 10 euros initially. So the money left should still be in euros unless there's an exchange involved, which isn't mentioned. So maybe the answer assumes they have 5 euros left, but the question asks for dollars, so we need to convert. But without knowing the exchange rate, that's impossible. Unless we use the current rate, but the problem doesn't specify a date, so that's an assumption. That's one uncertainty.
Then, the question about how many apples they have. The problem doesn't mention buying or having any apples. They bought a sandwich, not apples. So unless there's a trick or a play on words, the number of apples should be zero. Unless the sandwich had apples in it, but that's not stated either. So probably zero apples. But why would the user ask that? Maybe a trick question to test attention.
So putting it all together. Apples: 0. The remaining money is 5 euros, but the question asks for dollars. Without an exchange rate, we can't accurately convert. But if we have to answer, maybe use a rough estimate like 1 euro = 1.07 dollars (current approx rate as of 2023), so 5*1.07=5.35 dollars. But this is a big assumption, and the user didn't provide the rate, so this introduces uncertainty. Therefore, the confidence percentage would be lower for the dollars part because of the missing rate, but higher for the apples since it's not mentioned.
Wait, but the user might not even have dollars. They only mentioned euros. So maybe the answer is 0 apples and 5 euros, but the question asks for dollars. That's a problem. So the answer might be that there are 0 apples and the remaining euros can't be converted to dollars without the rate. However, the user might expect assuming conversion, but that's unclear. So this ambiguity affects confidence.
So final answer: 0 apples, 5 euros remaining which is approximately 5.35 dollars (but this is an assumption). However, since the problem doesn't mention exchange, maybe the answer is 5 euros, but the question asks for dollars. That inconsistency makes the dollars part invalid. So maybe the answer is 0 apples and the euros remaining is 5, but dollars isn't applicable. But the user insists on dollars. This confusion lowers confidence.
Alternatively, maybe the user made a mistake and meant euros. In that case, 5 euros. But they asked for dollars. So I have to make a choice here. Either state that without exchange rate, can't compute, but proceed with an estimated conversion. Confidence for apples is high (0) and for dollars is low due to assumptions. So maybe 0;5.35;70 (assuming 70% confidence because apples are certain, dollars are an estimate). But I need to check the exchange rate. Let me verify the current rate. As of October 2023, 1 euro is about 1.06 dollars. So 5 euros ~ 5.30 dollars. But again, the user might not be aware of the current rate. So maybe 0 apples, 5.30 dollars, but with low confidence on the dollar part. So perhaps confidence is 50% because the question has two parts: apples (100% sure) and dollars (0% sure if rate not given). Wait, but if the user didn't convert, maybe the answer is 5 euros, not dollars, so the dollars part is zero. Wait, maybe the user didn't convert, so they have 5 euros left, which is not dollars. So the answer is 0 apples and 0 dollars, which is incorrect. But that's not right either. The user has 5 euros, which they haven't converted, so in their wallet, they still have euros. If the question is about dollars, maybe the answer is 0 dollars because they didn't exchange. So this is really confusing.
Alternatively, perhaps the user intended a trick question where the answer is 0 apples and 5 euros, but since the question asks for dollars, the answer is 0 apples and 5 euros, but the user didn't specify conversion, so maybe the answer is there's no dollars left. Wait, that's possible. If the user only had euros, then after spending, they still have 5 euros, so dollars remaining would be zero. But that's assuming the wallet only had euros. The problem states "j'en dépense 5 pour acheter un sandwich" (spent 5 of them (euros) to buy a sandwich), so the remaining is 5 euros. The question asks how many dollars are left. If the wallet had only euros, then dollars left is zero. But that's maybe the intended answer. So 0 apples and 0 dollars, 5 euros. But the user didn't ask for euros. Hmm. Wait, the problem says "combien de Dollars il me reste en portefeuille?" So how many dollars are left in the wallet. If the wallet originally had euros, then after spending euros, there are still euros left, not dollars. So the answer is 0 dollars. Unless the user converted, but no info. So maybe the answer is 0 apples and 0 dollars, with high confidence because there's no mention of dollars initially. So the euros are still there, but the dollars part is zero.
So apples: 0. Dollars remaining: 0. Confidence: let's say apples are 100%, dollars are 100% if that's the case. But maybe the user thinks that 5 euros is equivalent to dollars, but without conversion, technically it's zero. But maybe the user expects a conversion. So uncertainty here. If we go with 0 dollars and 0 apples, confidence for apples is 100%, dollars is 80% (since they might have dollars but it's not mentioned). Or perhaps split the confidence. Maybe overall confidence is 90% because the apple part is certain, but the dollar part is ambiguous. Wait, this is getting complicated. Let me structure.
Possible interpretations:
1. Apples: 0 (since not mentioned). Dollars remaining: If wallet had only euros, then 0. Confidence for apples: 100%. Confidence for dollars: depends on whether wallet had dollars. But problem states starting with 10 euros, so maybe no dollars. So confidence for dollars being 0 is high, say 90%. So total confidence might be average, but the user asked for a single percentage. Maybe 95% because apples are certain, dollars likely 0.
But the user might think that the answer is 5 euros converted to dollars, which is a different approach. In that case, without exchange rate, we can't answer, but if forced, maybe use approximate. But the problem didn't mention conversion, so that's an assumption. So confidence for that would be low.
Alternatively, the question is a trick, so apples:0, dollars:0, confidence 100%? But maybe the user intended something else.
Alternatively, maybe the user made a mistake in the question, mixing apples and oranges (literally). The problem mentions a sandwich, not apples, so apples are 0. The money part: 5 euros left, which is about 5.30 dollars, but since it's not specified, maybe the answer is 5 euros, but the question asked for dollars, so that's a problem. Hence, the answer is unanswerable for dollars, but with assumptions, maybe 5.30. But confidence is low.
So combining apples (0) with 100% confidence, dollars with 5.30 (approx) with 50% confidence. Overall confidence maybe 75%? Because half right, half assumption. But the user wants a single confidence percentage for the entire answer. So if both parts are considered, apples are certain, dollars are uncertain, maybe average confidence is (100 + 50)/2 = 75%.
But I think the correct answer is 0 apples and 5 euros, but since the user asked for dollars, which they don't have, the dollars remaining is 0. Hence, answer is 0;0;95%. But maybe that's too confident. Alternatively, 0 apples, 5 euros, but user asked for dollars, so answer can't be given. But the user insists on dollars, so perhaps 0 apples and the euros converted to dollars with an assumed rate. But without the rate, it's impossible, leading to an answer with low confidence.
In conclusion, based on the problem as stated:
- Apples: 0 (100% confidence)
- Dollars remaining: Since the wallet had euros, and no conversion done, dollars remaining is 0 (but maybe 90% confidence here because the question might have a typo)
- Overall confidence: Let's average to 95% perhaps, since both parts are logical but the dollars part might be misinterpreted.
Alternatively, if the user intended to trick by asking dollars, but the answer is 5 euros, which is not dollars, so 0 dollars. Hence, the answer is 0;0;95%.
But maybe the user expects the conversion, but since no rate is given, the answer is 0;5;50% confidence, assuming they meant euros. But the question says dollars. This is really ambiguous.
Alternatively, maybe the user thinks that after spending 5 euros, they have 5 euros left, which is equivalent to dollars, but since the problem is in French, maybe they use euros, so the answer is 0 apples and 5 euros, but the user asked for dollars, which is a mistake. So confidence is 100% for apples, 0% for dollars, but since the user insists, maybe answer 0;5;50% (assuming maybe they meant euros). But the user specified dollars.
This is really tricky. I think the safest answer is 0 apples (100% sure) and 0 dollars (assuming no conversion, 90% sure), total confidence 95%. But the format requires a single percentage. So combining both parts, maybe 95% confidence.
> Résultat :
0;0;95
**Explication :**
- **Nombre de pommes :** Le problème ne mentionne pas l'achat ou la possession de pommes, donc **0** (confiance 100%).
- **Dollars restants :** La transaction est en euros, et rien n'indique une conversion en dollars ou la présence initiale de dollars. Ainsi, **0** dollars restants (confiance ~90%).
- **Confiance globale :** 95% (combinaison de certitude sur les pommes et d'une hypothèse raisonnable sur les dollars).
```

La réponse est plutôt impressionnante, il ne reste pas bloqué sur un problème et montre qu'il est capable de déterminer son propre niveau de confiance. En revenant à notre cas d'usage, on pourrait par exemple utiliser ce niveau de confiance pour basculer les demandes avec une fiabilité incertaine vers un workflow avec intervention manuelle, en y glissant par exemple en note les raisons concernant les doutes de l'IA.

using AI.DeepSeekR1;
using Azure;
using Azure.AI.Inference;
using Microsoft.Extensions.AI;
using System.Text;
using System.Text.RegularExpressions;

var endpoint = "https://models.inference.ai.azure.com";
var token = "...";
var model = "DeepSeek-R1";

IChatClient chatClient = new ChatCompletionsClient(new Uri(endpoint), new AzureKeyCredential(token))
    .AsChatClient(model);
var runner = new AiRunner(chatClient);

//Cas 1 : Virement non-reçu
var input1 = @"Bonjour,
Suite à mon dernier échange téléphonique de la semaine dernière avec Mr Martin, je n'ai toujours pas reçu votre virement qui était censé être versé sous 48h.
Je n'ai pas réussi à vous joindre, merci de revenir vers moi au 0601020304 ou p.martin@gmail.com.
Cordialement,
Pierre Martin";
var reclamation1 = await runner.DeterminerReclamationAsync(input1);

//Cas 2 : Personne impatiente
var input2 = @"Ca commence à bien faire, quand mon dossier sera traité ?!?";
var reclamation2 = await runner.DeterminerReclamationAsync(input2);

//Cas 3 : Suivi de dossier
var input3 = @"Bonjour,
J'aimerais savoir où en est mon dossier de sinistre F12345.
Bien à vous,
Jacques";
var reclamation3 = await runner.DeterminerReclamationAsync(input3);

//Cas 4 : Candidature spontanée (langue anglaise)
var input4 = @"Hello,
I would like to apply to the job offer 'Senior software developer'.
I am available and highly motivated, at which address can I send you my resume?
Kind regards,
John SMith
jsmith@gmail.com
+44 (0) 123 456 789
1 East Street SE125EM London (UK)";
var reclamation4 = await runner.DeterminerReclamationAsync(input4);

//Cas 5 : Demande incompréhensible
var input5 = @"Gros bisous les amis";
var reclamation5 = await runner.DeterminerReclamationAsync(input5);











var question = @"
Je possède 10 Euros et j'en dépense 5 pour acheter un sandwich.
Combien ai-je de pomme et combien de Dollars il me reste en portefeuille ?
Donne pour dans ta réponse ton pourcentage de confiance quant au résultat.
Renvoi seulement les valeurs au format Valeur1;Valeur2;PourcentageConfiance, par exemple 50;14,30;20";
Console.WriteLine($"QUESTION:\n{question}");

////Soit récupérer tout d'un coup
//var response = await chatClient.CompleteAsync(question);
//Console.WriteLine($"Réponse: {response}");

//Soit afficher en temps réel
Console.WriteLine($"\nAppel de {model}...\n");
var content = new StringBuilder();
var response = chatClient.CompleteStreamingAsync(question);
await foreach (var item in response)
{
    content.Append(item);
    Console.Write(item.ToString().Replace("<think>", "> Réflexion :\n").Replace("</think>", "> Résultat :\n"));
}

//Extraction du résultat typé
var match = Regex.Match(content.ToString(), @"<think>(.*?)</think>\s*(.*)", RegexOptions.Singleline);
if (match.Success)
{
    var results = match.Groups[2].Value.Trim().Split(";");
    var resultNbPommes = int.Parse(results[0]);
    var resultNbDollars = double.Parse(results[1]);
    var resultConfiance = int.Parse(results[2]);
    Console.WriteLine($"\n\n=======\n> RESULTAT : {resultNbPommes} pommes et ${resultNbDollars}. Degré confiance = {resultConfiance}%\n=======");
}

Console.WriteLine(">>> FIN");





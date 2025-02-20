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

Console.WriteLine("\n\n################## Cas 1 : Virement non-reçu ##################");
var input1 = @"Bonjour,
Suite à mon dernier échange téléphonique de la semaine dernière avec Mr Martin, je n'ai toujours pas reçu votre virement qui était censé être versé sous 48h.
Je n'ai pas réussi à vous joindre, merci de revenir vers moi au 0601020304 ou p.martin@gmail.com.
Cordialement,
Pierre Martin";
var reclamation1 = await runner.DeterminerReclamationAsync(input1);


Console.WriteLine("\n\n################## Cas 2 : Personne impatiente ##################");
var input2 = @"Ca commence à bien faire, quand mon dossier sera traité ?!?";
var reclamation2 = await runner.DeterminerReclamationAsync(input2);

Console.WriteLine("\n\n################## Cas 3 : Suivi de dossier ##################");
var input3 = @"Bonjour,
J'aimerais savoir où en est mon dossier de sinistre F12345.
Bien à vous,
Jacques";
var reclamation3 = await runner.DeterminerReclamationAsync(input3);

Console.WriteLine("\n\n################## Cas 4 : Candidature spontanée (langue anglaise) ##################");
var input4 = @"Hello,
I would like to apply to the job offer 'Senior software developer'.
I am available and highly motivated, at which address can I send you my resume?
Kind regards,
John SMith
jsmith@gmail.com
+44 (0) 123 456 789
1 East Street SE125EM London (UK)";
var reclamation4 = await runner.DeterminerReclamationAsync(input4);


Console.WriteLine("\n\n################## Cas 5 : Demande incompréhensible ##################");
var input5 = @"Gros bisous les amis";
var reclamation5 = await runner.DeterminerReclamationAsync(input5);

Console.WriteLine("\n\nFIN");

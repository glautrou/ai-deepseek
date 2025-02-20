using AI.DeepSeekR1.Metier;
using Microsoft.Extensions.AI;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AI.DeepSeekR1;
public class AiRunner
{
    private readonly IChatClient _chatClient;
    private readonly string PromptReclamation = @"Tu es un assistant et vas devoir aider à interpréter une demande d'un client d'une compagnie d'assurance.
Tu renverras les résultats de ton analyse au format JSON en respectant la structure C# ci-dessous, dont tu y trouveras dans le code la description des règles 
pour chaque champ afin de t'aider à produire le meilleur résultat.
Tu dois insérer les balises <JSON> et </JSON> pour délimiter le JSON à renvoyer.";

    private string ContenuFichiersTechniques { get; set; }

    public AiRunner(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<Reclamation?> DeterminerReclamationAsync(string question)
    {
        var prompt = await GetReclamationPromptAsync(question);
        var response = await _chatClient.CompleteAsync(prompt);
        Console.WriteLine("\n> REPONSE :\n");
        var json = ExtractJsonContent(response.ToString());
        Console.WriteLine("\n> JSON :\n");

        return !string.IsNullOrEmpty(json)
            ? JsonSerializer.Deserialize<Reclamation>(json)
            : null;
    }

    private async Task<string> GetReclamationPromptAsync(string question)
    {
        return PromptReclamation + Environment.NewLine
            + "Fichiers techniques : " + Environment.NewLine
            + await GetContenuFichiersTechniquesAsync() + Environment.NewLine
            + "A partir d'ici débute le message de réclamation reçu : " + Environment.NewLine
            + question;
    }

    private async Task<string> GetContenuFichiersTechniquesAsync()
    {
        //Les fichiers techniques sont automatiquement chargés, et une seule fois
        if (string.IsNullOrEmpty(ContenuFichiersTechniques))
        {
            var promptFiles = new StringBuilder();
            foreach (var file in Directory.GetFiles("../../../Metier"))
            {
                var code = await File.ReadAllTextAsync(file);
                promptFiles.AppendLine(code);
            }
            ContenuFichiersTechniques = promptFiles.ToString();
        }
        return ContenuFichiersTechniques;
    }

    private static string? ExtractJsonContent(string input)
    {
        var pattern = @"<JSON>(.*?)</JSON>";
        var match = Regex.Match(input, pattern, RegexOptions.Singleline);
        return match.Success
            ? match.Groups[1].Value.Trim()
            : null;
    }
}

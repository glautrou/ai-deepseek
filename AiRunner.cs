using AI.DeepSeekR1.Metier;
using Microsoft.Extensions.AI;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AI.DeepSeekR1;
public class AiRunner
{
    private readonly IChatClient _chatClient;
    private readonly string PromptReclamation = @"Tu es un assistant et vas devoir aider à interpréter une demande d'un client d'une compagnie d'assurance.
Tu renverras les résultats de ton analyse au format JSON en respectant la structure C# ci-dessous, dont tu y trouveras dans le code la description des règles 
pour chaque champ afin de t'aider à produire le meilleur résultat.
Tu dois insérer les balises <JSON> et </JSON> pour délimiter le JSON à renvoyer.

public class Reclamation {
[Description(""Numéros de dossier détectés. Les numéros de dossier sont toujours du même format, ils commencent toujours par une lettre suivie de 5 chiffres"")]
public List<string> NumeroDossiers { get; set; }
[Description(""Titre déterminé pour le contenu, synthétisé en maximum 20 mots"")]
public string Titre { get; set; }
[Description(""Message envoyé par l'utilisateur, convertit au format Markdown"")]
public string Contenu { get; set; }
[Description(""Servive interne le plus apte à traiter la demande"")]
public TypeService TypeService { get; set; }
[Description(""Catégorisation la plus appropriée à la demande"")]
public CategorieProbleme CategorieProbleme { get; set; }
[Description(""Degré de priorisation déterminé"")]
public Priorisation Priorisation { get; set; }
[Description(""Tout nom de personne physique ou morale"")]
public List<string> InformationsNominatives { get; set; }
[Description(""Les coordonnées peuvent être des numéros de téléphone, emails ou encore des adresses postales"")]
public List<string> CoordonneesContact { get; set; } }
public enum TypeService {
[Description(""Traitement des factures, devis et paiement"")]
Comptabilité,
[Description(""Tout problème lié à son utilisation des outils informatiques, comme un bug"")]
Informatique,
[Description(""Concerne le recrutement"")]
RessourcesHumaines,
[Description(""Concerne les publications sur les réseaux sociaux et campagnes publicitaires"")]
Communication,
[Description(""Litige concernant son assurance"")]
Litige,
[Description(""Problème d'ordre jurique"")]
Juridique,
[Description(""Tout autre type"")]
Autre }
public enum CategorieProbleme {
[Description(""Nouvelle réclamation nécessitant l'ouverture d'un nouveau dossier"")]
OuvertureNouveauDossier,
[Description(""Ajout d'informations complémentaires au dossier existant"")]
AjoutInformationDossierExistant,
[Description(""Le client n'a pas reçu son paiement"")]
PaiementNonRecu,
[Description(""Le montant reçu est incorrect"")]
MontantRecuIncorrect,
[Description(""Toute autre catégorie"")]
Autre }
public enum Priorisation {
[Description(""Le client n'évoque pas de numéro de dossier, et les dates sont inférieures à 48h"")]
Faible,
[Description(""Par défaut"")]
Moyenne,
[Description(""Le client évoque des dates supérieures à 1 mois, ou montre une forme d'aggressivité"")]
Elevee }";

    public AiRunner(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    public async Task<Reclamation?> DeterminerReclamationAsync(string question)
    {
        var message = PromptReclamation + Environment.NewLine
            + "A partir d'ici débute le message de réclamation reçu : "
            + question;
        var response = await _chatClient.CompleteAsync(message);
        var json = ExtractJsonContent(response.ToString());

        return !string.IsNullOrEmpty(json)
            ? JsonSerializer.Deserialize<Reclamation>(json)
            : null;
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

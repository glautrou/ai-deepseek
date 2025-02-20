using System.ComponentModel;

namespace AI.DeepSeekR1.Metier;
public class Reclamation
{
    [Description("Numéros de dossier détectés. Les numéros de dossier sont toujours du même format, ils commencent toujours par une lettre suivie de 5 chiffres")]
    public List<string> NumeroDossiers { get; set; }
    [Description("Titre déterminé pour le contenu, synthétisé en maximum 20 mots")]
    public string Titre { get; set; }
    [Description("Message envoyé par l'utilisateur, convertit au format Markdown")]
    public string Contenu { get; set; }
    [Description("Servive interne le plus apte à traiter la demande")]
    public TypeService TypeService { get; set; }
    [Description("Catégorisation la plus appropriée à la demande")]
    public CategorieProbleme CategorieProbleme { get; set; }
    [Description("Degré de priorisation déterminé")]
    public Priorisation Priorisation { get; set; }
    [Description("Tout nom de personne physique ou morale")]
    public List<string> InformationsNominatives { get; set; }
    [Description("Les coordonnées peuvent être des numéros de téléphone, emails ou encore des adresses postales")]
    public List<string> CoordonneesContact { get; set; }
}

using System.ComponentModel;

namespace AI.DeepSeekR1.Metier;
public enum Priorisation
{
    [Description("Le client n'évoque pas de numéro de dossier, et les dates sont inférieures à 48h")]
    Faible,
    [Description("Par défaut")]
    Moyenne,
    [Description("Le client évoque des dates supérieures à 1 mois, ou montre une forme d'aggressivité")]
    Elevee
}

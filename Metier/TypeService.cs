using System.ComponentModel;

namespace AI.DeepSeekR1.Metier;
public enum TypeService
{
    [Description("Traitement des factures, devis et paiement")]
    Comptabilité,
    [Description("Tout problème lié à son utilisation des outils informatiques, comme un bug")]
    Informatique,
    [Description("Concerne le recrutement")]
    RessourcesHumaines,
    [Description("Concerne les publications sur les réseaux sociaux et campagnes publicitaires")]
    Communication,
    [Description("Litige concernant son assurance")]
    Litige,
    [Description("Problème d'ordre jurique")]
    Juridique,
    [Description("Tout autre type")]
    Autre
}

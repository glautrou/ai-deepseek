using System.ComponentModel;

namespace AI.DeepSeekR1.Metier;
public enum CategorieProbleme
{
    [Description("Nouvelle réclamation nécessitant l'ouverture d'un nouveau dossier")]
    OuvertureNouveauDossier,
    [Description("Ajout d'informations complémentaires au dossier existant")]
    AjoutInformationDossierExistant,
    [Description("Le client n'a pas reçu son paiement")]
    PaiementNonRecu,
    [Description("Le montant reçu est incorrect")]
    MontantRecuIncorrect,
    [Description("Toute autre catégorie")]
    Autre
}

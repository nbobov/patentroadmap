namespace roadmap_migrant.Models;

public class SurveyModel
{
    public DateTime EntryDate { get; set; }
    public string Citizenship { get; set; }
    public bool? HasCertificate { get; set; }
    public bool? HasDiploma { get; set; }
    public bool? HighQualitySpecialist { get; set; }
    public DateTime? CertificateDate { get; set; }
    public bool? HasBankExtract { get; set; }
    public bool? HasPhotos { get; set; }
    public bool? HasTIN { get; set; }
}
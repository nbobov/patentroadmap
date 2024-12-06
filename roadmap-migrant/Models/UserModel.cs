namespace roadmap_migrant.Models;

public class UserModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public SurveyModel Survey { get; set; }
}
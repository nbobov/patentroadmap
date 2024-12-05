using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using roadmap_migrant.Models;

namespace roadmap_migrant.Controllers;

[Authorize]
public class SurveyController : Controller
{
    public IActionResult FillForm()
    {
        var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/data/countries.json");
        var countriesJson = System.IO.File.ReadAllText(jsonPath);

        using var document = JsonDocument.Parse(countriesJson);
        var countryNames = document.RootElement
            .EnumerateArray()
            .Select(element =>
                element.GetProperty("translations").GetProperty("rus").GetProperty("official").GetString())
            .OrderBy(name => name)
            .ToList();

        string email = User.Identity?.Name;

        var userJson = HttpContext.Session.GetString($"User_{email}");
        var user = string.IsNullOrEmpty(userJson) ? null : JsonSerializer.Deserialize<UserModel>(userJson);

        SurveyModel survey = user?.Survey ?? new SurveyModel();
        ViewData["CountryNames"] = countryNames;

        return View(survey);
    }

    [HttpPost]
    public IActionResult FillForm(SurveyModel survey)
    {
        string email = User.Identity?.Name;

        if (ModelState.IsValid)
        {
            var userJson = HttpContext.Session.GetString($"User_{email}");
            var user = string.IsNullOrEmpty(userJson) ? null : JsonSerializer.Deserialize<UserModel>(userJson);

            if (user != null)
            {
                user.Survey = survey;

                HttpContext.Session.SetString($"User_{email}", JsonSerializer.Serialize(user));
            }

            return RedirectToAction("ViewRoadmap");
        }

        return View(survey);
    }

    public IActionResult ViewRoadmap()
    {
        string email = User.Identity?.Name;
        var userJson = HttpContext.Session.GetString($"User_{email}");
        var user = string.IsNullOrEmpty(userJson) ? null : JsonSerializer.Deserialize<UserModel>(userJson);

        if (user?.Survey != null)
        {
            SurveyModel survey = user.Survey;

            var roadmap = new List<RoadmapStep>();

            if (
                (survey.Citizenship == "Азербайджанская Республика" ||
                 survey.Citizenship == "Республика Таджикистан" ||
                 survey.Citizenship == "Республика Узбекистан" ||
                 survey.Citizenship == "Молдова" ||
                 survey.Citizenship == "Украина") && survey.HighQualitySpecialist != true
            )
            {
                if (survey.HasCertificate == true && DateTime.Now.Subtract(survey.CertificateDate.Value).Days / 365 > 5)
                    roadmap.Add(new RoadmapStep
                    {
                        StepName = "Подтверждение знаний",
                        Description =
                            "Ваш сертификат просрочился, он действителен 5 лет. Необходимо обратиться в образовательные " +
                            "учреждения (филиалы) по вашему месту нахождения для сдачи экзамена и получения сертификата " +
                            "для оформления патента."
                    });

                if (survey.HasDiploma == true && survey.CertificateDate.Value.Year < 1991)
                    roadmap.Add(new RoadmapStep
                    {
                        StepName = "Подтверждение знаний",
                        Description =
                            "Ваш документ не подходит, так как он выдан до 1991 года. Необходимо обратиться в образовательные " +
                            "учреждения (филиалы) по вашему месту нахождения для сдачи экзамена и получения сертификата " +
                            "для оформления патента."
                    });

                if (survey.HasCertificate == false && survey.HasDiploma == false)
                    roadmap.Add(new RoadmapStep
                    {
                        StepName = "Подтверждение знаний",
                        Description =
                            "Вам необходимо обратиться в образовательные учреждения (филиалы) по вашему месту " +
                            "нахождения для сдачи экзамена и получения сертификата для оформления патента."
                    });

                if (survey.HasBankExtract == false)
                    roadmap.Add(new RoadmapStep
                    {
                        StepName = "Финансовая отчетность",
                        Description =
                            "Вам необходимо обратиться в банк и оплатить НДФЛ за первый месяц действия патента. " +
                            "Платежи осуществляются ежемесячно, либо авансово за 12 месяцев действия патента."
                    });

                if (survey.HasPhotos == false)
                    roadmap.Add(new RoadmapStep
                    {
                        StepName = "Предоставление фотографий",
                        Description = "Подготовьте и предоставьте две фотографии формата 3x4."
                    });


                if (survey.HasTIN == false)
                    roadmap.Add(new RoadmapStep
                    {
                        StepName = "Получение ИНН",
                        Description = "Вам необходимо обратиться в Инспекцию Федеральной налоговой службы " +
                                      "для оформления документа."
                    });

                if (DateTime.Now.Subtract(survey.EntryDate).Days <= 30)
                {
                    roadmap.Add(new RoadmapStep
                    {
                        StepName = "Подача документов",
                        Description = "Подайте документы в УМВД России либо в ФГУП «ПВС» МВД России в течение " +
                                      $"{30 - DateTime.Now.Subtract(survey.EntryDate).Days}" + " дн."
                    });
                }
                else
                {
                    roadmap.Add(new RoadmapStep
                    {
                        StepName = "Подача документов",
                        Description = "Срочно подайте документы в УМВД России либо в ФГУП «ПВС» МВД России. " +
                                      "Есть риск получить административный штраф от 10 000 до 15 000 рублей, так как " +
                                      "вы не уложились в срок (30 дней с момента въезда на территорию РФ)."
                    });
                }
            }
            else if (
                survey.Citizenship == "Республика Армения" ||
                survey.Citizenship == "Республика Казахстан" ||
                survey.Citizenship == "Кыргызская Республика" ||
                survey.Citizenship == "Республика Беларусь" ||
                survey.HighQualitySpecialist == true
            )
            {
                roadmap.Add(new RoadmapStep
                {
                    StepName = "Патент не нужен",
                    Description = "Граждане Армении, Беларуси, Казахстана, Киргизии, а также высококвалифицированные " +
                                  "специалисты, въехавшие в РФ по безвизовому режиму, могут работать в РФ без оформления патента."
                });
            }
            else
            {
                roadmap.Add(new RoadmapStep
                {
                    StepName = "Разрешение на работу",
                    Description =
                        "Для иностранных граждан, приехавших в РФ по визе, необходимо оформить разрешение на работу."
                });
            }

            var viewModel = new RoadmapViewModel
            {
                Survey = survey,
                RoadmapSteps = roadmap
            };

            return View(viewModel);
        }

        return RedirectToAction("FillForm");
    }
}
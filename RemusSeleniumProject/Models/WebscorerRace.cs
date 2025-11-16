using System.Globalization;
using OpenQA.Selenium;

namespace RemusSeleniumProject.Models;

public record WebscorerRace
{
    public int RowIndex { get; set; }
    
    public int Id { get; set; }
    
    public string Name { get; set; }

    public string FileName => Name.Replace("/", "_").Replace("\"", "_") + " Edit.txt";
    public string AlternateFileName => Name.Replace("/", "_").Replace("\"", "_") + "  Edit.txt";
    
    public DateTime Date { get; set; }
    
    public Uri Uri { get; set; }

    public WebscorerRace(IWebDriver driver, IWebElement element)
    {
        RowIndex = int.Parse(element.GetAttribute("id")!.Split('_').Last());
        
        var href = element.GetAttribute("href");

        Id = int.Parse(href!.Split('=').Last());
        Uri = new Uri(href);

        Name = element.Text;

        var dateElement = driver.FindElement(By.Id($"CPH1_repMyPostedRaces_lbRaceDate_{RowIndex}"));
        
        Date = DateTime.ParseExact(dateElement.Text, "MMM d, yyyy", CultureInfo.InvariantCulture);
    }

    public bool IsInList(IEnumerable<string> raceNames)
    {
        return raceNames.Any(rn => Name.Contains(rn));
    }
}
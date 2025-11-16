using OpenQA.Selenium.Support.UI;
using RemusSeleniumProject.Models;

namespace RemusSeleniumProject;

using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

public static class FirstScript
{
    public const string ResultsURL = "https://www.webscorer.com/WH_Results";

    public static readonly string[] IgnoreRaceNames = ["Milnes Beatson Motueka", "2 Person Relay"];

    public static DateTime StartDateTime = new(2023, 11, 1);
    
    public static async Task Main()
    {
        IWebDriver driver = new ChromeDriver();

        driver.Navigate().GoToUrl(ResultsURL);
        
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(500);

        var raceElements = driver.FindElements(By.CssSelector("a[id*='CPH1_repMyPostedRaces_racetxtlink']"));
        
        var races = raceElements.Select(re => new WebscorerRace(driver, re)).ToList();

        races = races
            .Where(r => r.Date >= StartDateTime)
            .Where(r => !r.IsInList(IgnoreRaceNames))
            .ToList();

        foreach (var race in races)
        {
            Console.WriteLine(race);
            await DownloadRaceFile(driver, race);
        }
            
        driver.Quit();
    }

    public static async Task DownloadRaceFile(IWebDriver driver, WebscorerRace race)
    {
        Console.WriteLine($"Downloading race file for race: '{race.RowIndex}'");
        
        await driver.Navigate().GoToUrlAsync(race.Uri);

        // Open dropdown
        driver.FindElement(By.Id("btnDownloadEdit")).Click();
        
        var downloadElement = driver.FindElement(By.Id("CPH1_RaceLinksButtons1_btnDownloadEditTxt"));
        
        WebDriverWait waitForElement = new WebDriverWait(driver, TimeSpan.FromSeconds(1));
        waitForElement.Until(_ => downloadElement.Displayed);
        
        downloadElement.Click();
        
        var downloadPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        
        var fileName = Path.Combine(downloadPath, race.FileName);
        var alternateFilePath = Path.Combine(downloadPath, race.AlternateFileName);
        
        var copyPath = Path.Combine("C:\\Users\\Remus\\RiderProjects\\RemusSeleniumProject\\RemusSeleniumProject\\Downloaded", race.FileName);
        
        
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        wait.Until(d => File.Exists(fileName) ||  File.Exists(alternateFilePath));

        try
        {
            File.Move(fileName, copyPath, true);
        }
        catch (FileNotFoundException e)
        {
            File.Move(alternateFilePath, copyPath, true);
        }
        

        await driver.Navigate().BackAsync();
    }
}
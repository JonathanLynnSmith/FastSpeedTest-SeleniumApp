using System;
using System.IO;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using Newtonsoft.Json;

namespace FastSpeedTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string downloadSpeedResult = "DOWNLOAD: 0 Mbps";
            string uploadSpeedResult = "UPLOAD: 0 Mbps";
            IWebDriver driver = null;
            Config config = new Config();

            try
            {
                var jsonText = File.ReadAllText($"{Environment.CurrentDirectory}\\FastSpeedTestConfig.json");
                config = JsonConvert.DeserializeObject<Config>(jsonText);
            }
            catch(Exception ex)
            {
                File.WriteAllText($"{Environment.CurrentDirectory}\\FastSpeedTest.log", ex.ToString());
                Environment.Exit(0);
            }


            try
            {
                //Create IE Driver and Wait Object
                var ieService = InternetExplorerDriverService.CreateDefaultService();
                ieService.HideCommandPromptWindow = true;

                var ieOptions = new InternetExplorerOptions()
                {
                    InitialBrowserUrl = config.URL,
                    IgnoreZoomLevel = true,
                    EnableNativeEvents = false,
                    IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                    EnablePersistentHover = true,
                    EnsureCleanSession = false,
                    RequireWindowFocus = true
                };

                driver = new InternetExplorerDriver(ieService, ieOptions);
                driver.Manage().Window.Minimize();
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(config.TimeOut));

                //Check for Certification Alert
                CheckForCertAlert(driver);

                //Download Test
                var DownloadTestComplete = ExpectedConditions.ElementExists(By.CssSelector(config.DownloadSpeedAttributes));
                wait.Until(DownloadTestComplete);

                IWebElement downloadSpeed = driver.FindElement(By.CssSelector(config.DownloadSpeedAttributes));
                IWebElement downloadUnits = driver.FindElement(By.CssSelector(config.DownloadUnitsAttributes));
                downloadSpeedResult = $"DOWNLOAD: {downloadSpeed.Text} {downloadUnits.Text}";


                //Show More Info
                var showMoreInfoButtonExist = ExpectedConditions.ElementToBeClickable(By.CssSelector(config.ShowMoreInfoAttributes));
                wait.Until(showMoreInfoButtonExist);

                IWebElement showMoreInfoButton = driver.FindElement(By.CssSelector(config.ShowMoreInfoAttributes));
                showMoreInfoButton.Click();


                //Upload Test
                var UploadTestComplete = ExpectedConditions.ElementExists(By.CssSelector(config.UploadSpeedAttributes));
                wait.Until(UploadTestComplete);

                IWebElement uploadSpeed = driver.FindElement(By.CssSelector(config.UploadSpeedAttributes));
                IWebElement uploadUnits = driver.FindElement(By.CssSelector(config.UploadUnitsAttributes));
                uploadSpeedResult = $"UPLOAD: {uploadSpeed.Text} {uploadUnits.Text}";
            }
            catch (Exception ex)
            {
                Log(ex);
            }
            finally
            {
                if(driver != null)
                {
                    driver.Quit();
                }
                ExportResultsAsCSV(downloadSpeedResult, uploadSpeedResult);
            }

            //Display Results
            Console.WriteLine();
            Console.WriteLine(downloadSpeedResult);
            Console.WriteLine(uploadSpeedResult);

            void CheckForCertAlert(IWebDriver webDriver)
            {
                if (webDriver.FindElements(By.CssSelector(config.CertificationAttributes)).Count() > 0)
                {
                    webDriver.FindElement(By.CssSelector(config.CertificationAttributes)).Click();
                }
            }

            void Log(Exception exception)
            {
                if (File.Exists(config.LogLocation))
                {
                    File.AppendAllText(config.LogLocation, $"{DateTime.Now + Environment.NewLine}: {exception.ToString()} {Environment.NewLine} {Environment.NewLine}");
                }       
            }

            void ExportResultsAsCSV(string download, string upload)
            {
                var downloadResults = download.Split(' ');
                string downloadCSVFormat = string.Format("{0},{1},{2}\n", downloadResults[0], downloadResults[1], downloadResults[2]);

                var uploadResults = upload.Split(' ');
                string uploadCSVFormat = string.Format("{0},{1},{2}\n", uploadResults[0], uploadResults[1], uploadResults[2]);

                string resultsAsCSV = downloadCSVFormat + uploadCSVFormat;

                if (File.Exists(config.CSVLocation))
                {
                    File.WriteAllText(config.CSVLocation, resultsAsCSV);
                }
                
            }

        }

    }
}

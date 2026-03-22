using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Linq;
using System.Threading;

namespace DemoblazeE2ETests
{
    public class E2ETests
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private const string BaseUrl = "https://www.demoblaze.com/";

        [SetUp]
        public void Setup()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless"); 
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument("--window-size=1920,1080"); 
            driver = new ChromeDriver(chromeOptions);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        }

        [Test]
        public void LoginAndAddProduct_UI_Test()
        {
            driver.Navigate().GoToUrl(BaseUrl);

            SafeClick(By.Id("login2"));

            wait.Until(ExpectedConditions.ElementIsVisible(By.Id("logInModal")));

            driver.FindElement(By.Id("loginusername")).SendKeys("Alhim666");
            driver.FindElement(By.Id("loginpassword")).SendKeys("qwe123");

            SafeClick(By.CssSelector("button[onclick='logIn()']"));

            SafeClick(By.LinkText("Samsung galaxy s6"));

            SafeClick(By.LinkText("Add to cart"));

            wait.Until(ExpectedConditions.AlertIsPresent());
            driver.SwitchTo().Alert().Accept();

            SafeClick(By.Id("cartur"));

            bool productFound = wait.Until(drv =>
            {
                var cartItems = drv.FindElements(By.CssSelector("tr.success td:nth-child(2)"));
                return cartItems.Any(item => item.Text.Contains("Samsung galaxy s6"));
            });

            Assert.That(productFound, Is.True, "Product was not found in the cart via UI");
        }

        private void SafeClick(By selector)
        {
            int retries = 3;
            while (retries > 0)
            {
                try
                {
                    var element = wait.Until(ExpectedConditions.ElementToBeClickable(selector));
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
                    element.Click();
                    return;
                }
                catch (StaleElementReferenceException)
                {
                    retries--;
                    Thread.Sleep(500);
                }
                catch (ElementClickInterceptedException)
                {
                    var element = driver.FindElement(selector);
                    ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", element);
                    return;
                }
            }
            throw new Exception($"Cannot click element: {selector}");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}
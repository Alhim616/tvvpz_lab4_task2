using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Threading;
using SeleniumExtras.WaitHelpers;

namespace DemoblazeE2ETests
{
    public class E2ETests
    {
        private IWebDriver driver;
        private const string BaseUrl = "https://www.demoblaze.com/";

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
        }

        [Test]
        public void LoginAndAddProduct_UI_Test()
        {
            driver.Navigate().GoToUrl(BaseUrl);

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            var loginButton = wait.Until(ExpectedConditions.ElementToBeClickable(By.Id("login2")));
            loginButton.Click();
            Thread.Sleep(1000);

            driver.FindElement(By.Id("loginusername")).SendKeys("Alhim666");
            driver.FindElement(By.Id("loginpassword")).SendKeys("qwe123");
            driver.FindElement(By.CssSelector("button[onclick='logIn()']")).Click();
            Thread.Sleep(2000);

            driver.FindElement(By.LinkText("Samsung galaxy s6")).Click();
            Thread.Sleep(1000);

            driver.FindElement(By.LinkText("Add to cart")).Click();
            Thread.Sleep(2000);

            driver.SwitchTo().Alert().Accept();
            Thread.Sleep(1000);

            driver.FindElement(By.Id("cartur")).Click();
            Thread.Sleep(2000);

            var cartItems = driver.FindElements(By.CssSelector("tr.success td:nth-child(2)"));
            bool productFound = false;
            foreach (var item in cartItems)
            {
                if (item.Text.Contains("Samsung galaxy s6"))
                {
                    productFound = true;
                    break;
                }
            }

            Assert.That(productFound, Is.True, "Product was not found in the cart via UI");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}
﻿using GUITests.Tests.TestUtils;
using GUITestsEngine.Utils.Attributes;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace GUITestsEngine.Tests
{
    public class SignUpTests
    {
        private readonly string imgPath = Path.GetFullPath(@"..\..\..\Resources\testPhoto.jpg").ToString();

        private List<string> fields = new List<string>()
        {
            "Name",
            "Surname",
            "01.01.2000",
            "den89181827071@gmail.com",
            "password",
            "password"
        };

        private void FillSignUpForm(ChromeDriver driver, List<string> input)
        {
            driver.Navigate().GoToUrl("http://localhost:8080/");

            Thread.Sleep(5000);

            WebDriverWrapper.FindElementByClass(driver, "button", "Sign up").Click();

            Thread.Sleep(5000);

            WebDriverWrapper.FillForm(driver, input.GetRange(0, 3));

            Thread.Sleep(5000);

            WebDriverWrapper.FindElementByClass(driver, "button", "Next").Click();

            Thread.Sleep(5000);

            WebDriverWrapper.FillForm(driver, input.GetRange(3, 3));

            Thread.Sleep(5000);

            WebDriverWrapper.FindElementByClass(driver, "button", "Next").Click();

            Thread.Sleep(5000);
        }

        [Test]
        public void ValidSignUpTest()
        {
            var driver = new ChromeDriver();

            FillSignUpForm(driver, fields);

            Thread.Sleep(5000);

            WebDriverWrapper.UploadImage(driver, imgPath);

            Thread.Sleep(5000);

            WebDriverWrapper.FindElementByClass(driver, "button", "Submit").Click();

            Thread.Sleep(5000);

            var errors = WebDriverWrapper.FindValidationErrors(driver);

            Thread.Sleep(5000);

            var testResult = driver.Url == "http://localhost:8080/" && errors.Count == 0;

            driver.Close();

            if (!testResult)
            {
                throw new Exception();
            }
        }

        [Test]
        public void EmptySignUpTest()
        {
            var driver = new ChromeDriver();

            FillSignUpForm(driver, new List<string>() { "", "", "", "", "", "" });

            Thread.Sleep(5000);

            WebDriverWrapper.FindElementByClass(driver, "button", "Submit").Click();

            Thread.Sleep(5000);

            var errors = WebDriverWrapper.FindValidationErrors(driver);

            Thread.Sleep(5000);

            driver.Close();

            if (errors.Count != 4)
            {
                throw new Exception();
            }           
        }
    }
}

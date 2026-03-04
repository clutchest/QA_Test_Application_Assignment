using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;
using TestAssignment.Pages;

namespace TestAssignment.Tests
{
    internal class RegistrationTests : PageTest
    {
        private LoginPage _loginPage = null!;
        private RegisterPage _registerPage = null!;
        private DashboardPage _dashboardPage = null!;
        private BasePage _basePage = null!;

        [SetUp]
        public void Setup()
        {
            _loginPage = new LoginPage(Page);
            _registerPage = new RegisterPage(Page);
            _dashboardPage = new DashboardPage(Page);
            _basePage = new BasePage(Page);
        }

        [Test]
        public async Task TC01_Registration_Successful_WithValidData()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Expect(_registerPage.RegisterMessage).ToBeVisibleAsync();
            await Expect(_registerPage.RegisterMessage).ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(new Regex(_basePage.LoginUrl + @"(\?.*)?$"));
        }

        [Test]
        public async Task TC02_Registration_Successful_WithInternationalCharacters()
        {
            var user = TestData.Generate() with
            {
                FirstName = "Željđoć",
                LastName = "Müller",
                City = "Zürich"
            };

            await _registerPage.GoTo();
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Expect(_registerPage.RegisterMessage).ToBeVisibleAsync();
            await Expect(_registerPage.RegisterMessage).ToHaveTextAsync("Registration successful! Redirecting to login...");

            await _loginPage.VerifyRegisteredUserLoginPageDisplayed();
            await _loginPage.VerifyEmptyLoginForm();
            await _loginPage.Email.FillAsync(user.Email);
            await _loginPage.Password.FillAsync(user.Password);
            await Expect(_loginPage.Email).ToHaveValueAsync(user.Email);
            await Expect(_loginPage.Password).ToHaveValueAsync(user.Password);

            await _loginPage.LoginButton.ClickAsync();

            await Expect(_loginPage.LoginMessage).ToHaveTextAsync("Login successful! Redirecting...");
            await Expect(Page).ToHaveURLAsync(new Regex(_basePage.DashboardUrl + @"(\?.*)?$"));
            await Expect(_dashboardPage.Heading).ToHaveTextAsync("Dashboard");
            await Expect(_dashboardPage.WelcomeMessage).ToContainTextAsync("Welcome, " + user.FirstName);
        }

        [Test]
        public async Task TC03_Registration_Successful_NewsletterUnchecked()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.TermsCheckbox.CheckAsync();

            await Expect(_registerPage.NewsletterCheckbox).Not.ToBeCheckedAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Expect(_registerPage.RegisterMessage).ToBeVisibleAsync();
            await Expect(_registerPage.RegisterMessage).ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC04_Registration_Failed_TermsAndConditionsNotAccepted()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await _registerPage.PopulateFields_RandomUserRegistration(user);

            await Expect(_registerPage.TermsCheckbox).Not.ToBeCheckedAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(2000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC05_Registration_AlreadyHaveAccountLink_NavigatesToLogin()
        {
            await _registerPage.GoTo();
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);

            await _registerPage.AlreadyHaveAnAccountLink.ClickAsync();
            await Expect(Page).ToHaveURLAsync(_basePage.LoginUrl);
        }

        [Test]
        [TestCaseSource(nameof(RegistrationRequiredFields))]
        public async Task RequiredFieldEmpty(string fieldName)
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await _registerPage.VerifyEmptyRegistrationForm();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.VerifyFields_RandomUserRegistration(user);

            await _registerPage.ClearRequiredFieldAsync(fieldName);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            bool isInvalid = await _registerPage.IsRequiredFieldInvalidEmpty(fieldName);
            Assert.That(isInvalid, Is.True, $"{fieldName} should be invalid when empty and required.");

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await Expect(_registerPage.RegisterMessage).Not.ToBeVisibleAsync(new() { Timeout = 8000 });
        }

        [Test]
        [TestCaseSource(nameof(RegistrationRequiredFieldsSpaces))]
        public async Task RequiredFieldOnlySpaces(string fieldName)
        {
            const string spaces = "          "; // 10 spaces
            var user = TestData.Generate();

            await _registerPage.GoTo();
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await _registerPage.VerifyEmptyRegistrationForm();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.VerifyFields_RandomUserRegistration(user);

            var field = _registerPage.GetRequiredField(fieldName);
            await field.FillAsync(spaces);

            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC23_FirstName_BelowMinLength()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.FirstName.FillAsync("A");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(1000);
            await Expect(_registerPage.RegisterMessage).Not.ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC24_FirstName_AtMinLength()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.FirstName.FillAsync("Ab");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(1000);
            await Expect(_registerPage.RegisterMessage).ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }


        [Test]
        public async Task TC25_FirstName_OverMaxLength()
        {
            var user = TestData.Generate() with { FirstName = new string('A', 101) };
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(1000);
            await Expect(_registerPage.RegisterMessage).Not.ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC26_FirstName_AtMaxLength()
        {
            var user = TestData.Generate() with { FirstName = new string('A', 100) };
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(1000);
            await Expect(_registerPage.RegisterMessage).ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC27_FirstName_OnlyInvalidSpecialCharacters()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.FirstName.FillAsync("&/");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(1000);
            await Expect(_registerPage.RegisterMessage).Not.ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC28_LastName_BelowMinLength()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.LastName.FillAsync("A");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(1000);
            await Expect(_registerPage.RegisterMessage).Not.ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC29_LastName_AtMinLength()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.LastName.FillAsync("Ab");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(1000);
            await Expect(_registerPage.RegisterMessage).ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC30_LastName_OverMaxLength()
        {
            var user = TestData.Generate() with { LastName = new string('A', 101) };
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(1000);
            await Expect(_registerPage.RegisterMessage).Not.ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC31_LastName_AtMaxLength()
        {
            var user = TestData.Generate() with { LastName = new string('A', 100) };
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(1000);
            await Expect(_registerPage.RegisterMessage).ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC32_LastName_OnlyInvalidSpecialCharacters()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.LastName.FillAsync("&/");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(1000);
            await Expect(_registerPage.RegisterMessage).Not.ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC33_Email_AtMaxLength()
        {
            var user = TestData.Generate();
            string email = TestData.GenerateRandomLocalPart(64);
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.Email.ClearAsync();
            await _registerPage.Email.FillAsync(email);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Expect(_registerPage.RegisterMessage).ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);

        }

        [Test]
        public async Task TC34_Email_OverMaxLength()
        {
            var user = TestData.Generate();
            string email = TestData.GenerateRandomLocalPart(65);
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.Email.ClearAsync();
            await _registerPage.Email.FillAsync(email);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(1000);
            await Expect(_registerPage.RegisterMessage).Not.ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC35_Email_BelowMinLength()
        {
            var user = TestData.Generate();
            string email = TestData.GenerateRandomLocalPart(5);
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.Email.ClearAsync();
            await _registerPage.Email.FillAsync(email);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(1000);
            await Expect(_registerPage.RegisterMessage).Not.ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC36_Email_AtMinLength()
        {
            var user = TestData.Generate();
            string email = TestData.GenerateRandomLocalPart(6);
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.Email.ClearAsync();
            await _registerPage.Email.FillAsync(email);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Expect(_registerPage.RegisterMessage).ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC37_Email_InvalidFormat()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await _registerPage.PopulateFields_RandomUserRegistration(user);

            await _registerPage.Email.ClearAsync();
            await _registerPage.Email.FillAsync("invalid@");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await Expect(_registerPage.EmailErrorMessage).ToBeVisibleAsync();
            await Expect(_registerPage.EmailErrorMessage).ToContainTextAsync("Invalid email address");
        }

        [Test]
        public async Task TC38_Email_NoAtSymbol()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await _registerPage.PopulateFields_RandomUserRegistration(user);

            await _registerPage.Email.FillAsync("user.example.com");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await Expect(_registerPage.EmailErrorMessage).ToBeVisibleAsync();
            await Expect(_registerPage.EmailErrorMessage).ToContainTextAsync("Invalid email address");
        }

        [Test]
        public async Task TC39_Email_MultipleAtSymbols()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await _registerPage.PopulateFields_RandomUserRegistration(user);

            await _registerPage.Email.FillAsync("user" + Random.Shared.Next(0, 999) + "@@example.com");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await Expect(_registerPage.EmailErrorMessage).ToBeVisibleAsync();
            await Expect(_registerPage.EmailErrorMessage).ToContainTextAsync("Invalid email address");
        }

        [Test]
        public async Task TC40_Email_WithInvalidSpecialCharacters()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await _registerPage.PopulateFields_RandomUserRegistration(user);

            await _registerPage.Email.FillAsync("user!#$" + Random.Shared.Next(0, 999) + "@example.com");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(_registerPage.EmailErrorMessage).ToBeVisibleAsync();
            await Expect(_registerPage.EmailErrorMessage).ToContainTextAsync("Invalid email address");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC41_Email_WithSpaces()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await _registerPage.PopulateFields_RandomUserRegistration(user);

            await _registerPage.Email.FillAsync("user @example.com");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await Expect(_registerPage.EmailErrorMessage).ToBeVisibleAsync();
            await Expect(_registerPage.EmailErrorMessage).ToContainTextAsync("Invalid email address");
        }

        [Test]
        public async Task TC42_Email_DuplicateEmails()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);

            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Expect(_registerPage.RegisterMessage).ToHaveTextAsync("Registration successful! Redirecting to login...");
            await _loginPage.VerifyRegisteredUserLoginPageDisplayed();

            await _loginPage.CreateNewAccountLink.ClickAsync();
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);

            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Expect(_registerPage.RegisterMessage).ToHaveTextAsync("User with this email already exists");
        }

        [Test]
        public async Task TC43_Phone_BelowMinLength()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.PhoneNumber.FillAsync("123456");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(_registerPage.RegisterMessage).Not.ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC44_Phone_AtMinLength()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.PhoneNumber.FillAsync("1234567");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(_registerPage.RegisterMessage).ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC45_Phone_OverMaxLength()
        {
            var user = TestData.Generate() with { Phone = new string('1', 31) };
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.PhoneNumber.FillAsync(user.Phone);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(_registerPage.RegisterMessage).Not.ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC46_Phone_AtMaxLength()
        {
            var user = TestData.Generate() with { Phone = new string('1', 30) };
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.PhoneNumber.FillAsync(user.Phone);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(_registerPage.RegisterMessage).ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC47_Phone_WithLetters()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.PhoneNumber.FillAsync("+385 91 ABC 123");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC48_Phone_WithInvalidSpecialChars()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.PhoneNumber.FillAsync("+385 91 123!456");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC49_StreetAddress_BelowMinLength()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.StreetAddress.FillAsync("Abc 21");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();
            await Task.Delay(2000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await Expect(_registerPage.RegisterMessage).Not.ToHaveTextAsync("Registration successful! Redirecting to login...");
        }

        [Test]
        public async Task TC50_StreetAddress_AtMinLength()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.StreetAddress.FillAsync("123 Main");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();
            await Task.Delay(2000);
            await Expect(_registerPage.RegisterMessage).ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC51_StreetAddress_OverMaxLength()
        {
            var user = TestData.Generate() with { StreetAddress = new string('A', 101) };
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();
            await Task.Delay(2000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await Expect(_registerPage.RegisterMessage).Not.ToHaveTextAsync("Registration successful! Redirecting to login...");
        }

        [Test]
        public async Task TC52_StreetAddress_AtMaxLength()
        {
            var user = TestData.Generate() with { StreetAddress = new string('A', 100) };
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();
            await Task.Delay(2000);
            await Expect(_registerPage.RegisterMessage).ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC53_StreetAddress_WithOnlySpecialCharacters()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.StreetAddress.FillAsync("@#$%^&*()_+");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(2000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC54_StreetAddress_OnlyNumbers()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.StreetAddress.FillAsync("1234567890");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(2000);
            await Expect(_registerPage.RegisterMessage).ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC55_City_BelowMinLength()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.City.FillAsync("A");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(2000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await Expect(_registerPage.RegisterMessage).Not.ToHaveTextAsync("Registration successful! Redirecting to login...");
        }

        [Test]
        public async Task TC56_City_AtMinLength()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.City.FillAsync("Os");  // or "Ky", "LA" etc.
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Expect(_registerPage.RegisterMessage).ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);  // or redirect depending on your flow
        }

        [Test]
        public async Task TC57_City_OverMaxLength()
        {
            var user = TestData.Generate() with { City = new string('A', 61) };
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();
            await Task.Delay(2000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await Expect(_registerPage.RegisterMessage).Not.ToHaveTextAsync("Registration successful! Redirecting to login...");
        }

        [Test]
        public async Task TC58_City_AtMaxLength()
        {
            var user = TestData.Generate() with { City = new string('A', 60) };
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();
            await Task.Delay(2000);
            await Expect(_registerPage.RegisterMessage).ToHaveTextAsync("Registration successful! Redirecting to login...");
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC59_City_WithInvalidSpecialCharacters()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.City.FillAsync("New York!");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(2000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC60_ZIP_BelowMinLength()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.ZIPCode.FillAsync("12");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC61_ZIP_OverMaxLength()
        {
            var user = TestData.Generate() with { ZipCode = new string('1', 13) };
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC62_ZIP_WithLetters()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.ZIPCode.FillAsync("ABC");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC63_ZIP_WithSpecialCharacters()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.ZIPCode.FillAsync("ABC!$#%&/'");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC64_Password_BelowMinLength()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);

            await _registerPage.Password.FillAsync("Abc1!");
            await _registerPage.ConfirmPassword.FillAsync("Abc1!");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await Expect(_registerPage.PasswordErrorMessage).ToBeVisibleAsync();
        }

        [Test]
        public async Task TC65_Password_AtMinLength()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);

            await _registerPage.Password.FillAsync("Abcdef1!");
            await _registerPage.ConfirmPassword.FillAsync("Abcdef1!");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await Expect(_registerPage.PasswordErrorMessage).ToBeVisibleAsync();
        }

        [Test]
        public async Task TC66_Password_OverMaxLength()
        {
            var user = TestData.Generate() with { Password = new string('A', 98) + "b" + "1" + "!" };
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);

            await _registerPage.Password.FillAsync("!");
            await _registerPage.ConfirmPassword.FillAsync("Abc1!");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await Expect(_registerPage.PasswordErrorMessage).ToBeVisibleAsync();
        }

        [Test]
        public async Task TC67_Password_AtMaxLength()
        {
            var user = TestData.Generate() with { Password = new string('A', 97) + "b" + "1" + "!" };
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
            await Expect(_registerPage.PasswordErrorMessage).ToBeVisibleAsync();
        }

        [Test]
        public async Task TC68_Password_PasswordsDoNotMatch()
        {
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);

            await _registerPage.Password.FillAsync("Abcdef1!");
            await _registerPage.ConfirmPassword.FillAsync("NotMatchingPw1!");
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC69_Password_NoUppercase()
        {
            const string invalid = "lowercase1!";
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);

            await _registerPage.Password.FillAsync(invalid);
            await _registerPage.ConfirmPassword.FillAsync(invalid);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC70_Password_NoLowercase()
        {
            const string invalid = "UPPERCASE1!";
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);

            await _registerPage.Password.FillAsync(invalid);
            await _registerPage.ConfirmPassword.FillAsync(invalid);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC71_Password_NoNumber()
        {
            const string invalid = "NoNumbers!";
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);

            await _registerPage.Password.FillAsync(invalid);
            await _registerPage.ConfirmPassword.FillAsync(invalid);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        [Test]
        public async Task TC72_Password_NoSpecialCharacter()
        {
            const string invalid = "NoSpecial1";
            var user = TestData.Generate();
            await _registerPage.GoTo();
            await _registerPage.PopulateFields_RandomUserRegistration(user);

            await _registerPage.Password.FillAsync(invalid);
            await _registerPage.ConfirmPassword.FillAsync(invalid);
            await _registerPage.TermsCheckbox.CheckAsync();
            await _registerPage.CreateAccountButton.ClickAsync();

            await Task.Delay(3000);
            await Expect(Page).ToHaveURLAsync(_basePage.RegisterUrl);
        }

        private static IEnumerable<TestCaseData> RegistrationRequiredFields()
        {
            for (int i = 0; i < RegisterPage.RequiredFieldNames.Length; i++)
            {
                var fieldName = RegisterPage.RequiredFieldNames[i];
                var tcNumber = (i + 6).ToString("D2");
                yield return new TestCaseData(fieldName).SetName($"TC{tcNumber}_RequiredFieldEmpty_{fieldName}");
            }
        }

        private static IEnumerable<TestCaseData> RegistrationRequiredFieldsSpaces()
        {
            var names = RegisterPage.RequiredFieldNames.Where(n => n != "Email").ToArray();

            for (int i = 0; i < names.Length; i++)
            {
                var fieldName = names[i];
                var tcNumber = (i + 15).ToString("D2");
                yield return new TestCaseData(fieldName).SetName($"TC{tcNumber}_RequiredFieldOnlySpaces_{fieldName}");
            }
        }
    }
}
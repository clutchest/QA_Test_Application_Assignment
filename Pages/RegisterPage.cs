using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace TestAssignment.Pages
{
    public class RegisterPage(IPage page) : BasePage(page)
    {
        public ILocator FirstName => Page.GetByLabel("First Name");
        public ILocator LastName => Page.GetByLabel("Last Name");
        public ILocator Email => Page.GetByLabel("Email Address");
        public ILocator PhoneNumber => Page.GetByLabel("Phone Number");
        public ILocator StreetAddress => Page.GetByLabel("Street Address");
        public ILocator City => Page.GetByLabel("City");
        public ILocator ZIPCode => Page.GetByLabel("ZIP Code");
        public ILocator Password => Page.Locator("#password");
        public ILocator ConfirmPassword => Page.GetByLabel("Confirm Password");
        public ILocator TermsCheckbox => Page.GetByRole(AriaRole.Checkbox, new() { Name = "I agree to the Terms and Conditions" });
        public ILocator NewsletterCheckbox => Page.GetByRole(AriaRole.Checkbox, new() { Name = "Subscribe to newsletter" });
        public ILocator CreateAccountButton => Page.GetByRole(AriaRole.Button, new() { Name = "Create Account" });
        public ILocator AlreadyHaveAnAccountLink => Page.GetByRole(AriaRole.Link, new() { Name = "Already have an account? Login" });
        public ILocator RegisterMessage => Page.Locator("#registerMessage");
        public ILocator EmailErrorMessage => Page.Locator("#emailError");
        public ILocator PasswordErrorMessage => Page.Locator("#passwordError");

        // Common metadata for required fields, used by tests for parameterized validation.
        public static readonly string[] RequiredFieldNames =
        [
            "FirstName",
            "LastName",
            "Email",
            "Phone",
            "StreetAddress",
            "City",
            "ZipCode",
            "Password",
            "ConfirmPassword"
        ];

        public ILocator GetRequiredField(string fieldName) =>
            fieldName switch
            {
                "FirstName" => FirstName,
                "LastName" => LastName,
                "Email" => Email,
                "Phone" => PhoneNumber,
                "StreetAddress" => StreetAddress,
                "City" => City,
                "ZipCode" => ZIPCode,
                "Password" => Password,
                "ConfirmPassword" => ConfirmPassword,
                _ => throw new ArgumentOutOfRangeException(nameof(fieldName), fieldName, "Unknown field name")
            };

        public async Task ClearRequiredFieldAsync(string fieldName)
        {
            var field = GetRequiredField(fieldName);
            await field.ClearAsync();
        }

        public async Task<bool> IsRequiredFieldInvalidEmpty(string fieldName)
        {
            var field = GetRequiredField(fieldName);
            return await field.EvaluateAsync<bool>(
                "el => !el.validity.valid && el.validity.valueMissing");
        }

        public async Task GoTo() => await GoToRegister();

        public async Task VerifyEmptyRegistrationForm()
        {
            await Expect(FirstName).ToBeVisibleAsync();
            await Expect(FirstName).ToBeEnabledAsync();
            await Expect(FirstName).ToBeEmptyAsync();

            await Expect(LastName).ToBeVisibleAsync();
            await Expect(LastName).ToBeEnabledAsync();
            await Expect(LastName).ToBeEmptyAsync();

            await Expect(Email).ToBeVisibleAsync();
            await Expect(Email).ToBeEnabledAsync();
            await Expect(Email).ToBeEmptyAsync();

            await Expect(PhoneNumber).ToBeVisibleAsync();
            await Expect(PhoneNumber).ToBeEnabledAsync();
            await Expect(PhoneNumber).ToBeEmptyAsync();

            await Expect(StreetAddress).ToBeVisibleAsync();
            await Expect(StreetAddress).ToBeEnabledAsync();
            await Expect(StreetAddress).ToBeEmptyAsync();

            await Expect(City).ToBeVisibleAsync();
            await Expect(City).ToBeEnabledAsync();
            await Expect(City).ToBeEmptyAsync();

            await Expect(ZIPCode).ToBeVisibleAsync();
            await Expect(ZIPCode).ToBeEnabledAsync();
            await Expect(ZIPCode).ToBeEmptyAsync();

            await Expect(Password).ToBeVisibleAsync();
            await Expect(Password).ToBeEnabledAsync();
            await Expect(Password).ToBeEmptyAsync();

            await Expect(ConfirmPassword).ToBeVisibleAsync();
            await Expect(ConfirmPassword).ToBeEnabledAsync();
            await Expect(ConfirmPassword).ToBeEmptyAsync();

            await Expect(TermsCheckbox).ToBeVisibleAsync();
            await Expect(TermsCheckbox).ToBeEnabledAsync();
            await Expect(TermsCheckbox).Not.ToBeCheckedAsync();

            await Expect(NewsletterCheckbox).ToBeVisibleAsync();
            await Expect(NewsletterCheckbox).ToBeEnabledAsync();
            await Expect(NewsletterCheckbox).Not.ToBeCheckedAsync();

            await Expect(CreateAccountButton).ToBeVisibleAsync();
            await Expect(CreateAccountButton).ToBeEnabledAsync();
            await Expect(CreateAccountButton).ToHaveTextAsync("Create Account");

            await Expect(AlreadyHaveAnAccountLink).ToBeVisibleAsync();
            await Expect(AlreadyHaveAnAccountLink).ToBeEnabledAsync();
            await Expect(AlreadyHaveAnAccountLink).ToHaveTextAsync("Already have an account? Login");
        }

        public async Task PopulateFields_RandomUserRegistration(RegistrationData user)
        {
            await FirstName.FillAsync(user.FirstName);
            await LastName.FillAsync(user.LastName);
            await Email.FillAsync(user.Email);
            await PhoneNumber.FillAsync(user.Phone);
            await StreetAddress.FillAsync(user.StreetAddress);
            await City.FillAsync(user.City);
            await ZIPCode.FillAsync(user.ZipCode);
            await Password.FillAsync(user.Password);
            await ConfirmPassword.FillAsync(user.Password);
        }

        public async Task VerifyFields_RandomUserRegistration(RegistrationData user)
        {
            await Expect(FirstName).ToHaveValueAsync(user.FirstName);
            await Expect(LastName).ToHaveValueAsync(user.LastName);
            await Expect(Email).ToHaveValueAsync(user.Email);
            await Expect(PhoneNumber).ToHaveValueAsync(user.Phone);
            await Expect(StreetAddress).ToHaveValueAsync(user.StreetAddress);
            await Expect(City).ToHaveValueAsync(user.City);
            await Expect(ZIPCode).ToHaveValueAsync(user.ZipCode);
            await Expect(Password).ToHaveValueAsync(user.Password);
            await Expect(ConfirmPassword).ToHaveValueAsync(user.Password);
        }
    }
}

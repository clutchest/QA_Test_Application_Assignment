using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace TestAssignment.Pages
{
    public class LoginPage : BasePage
    {
        public LoginPage(IPage page) : base(page)
        {
        }

        public ILocator Email => Page.GetByLabel("Email Address", new() { Exact = true });
        public ILocator Password => Page.GetByLabel("Password");
        public ILocator LoginButton => Page.GetByRole(AriaRole.Button, new() { Name = "Login" });
        public ILocator Heading => Page.GetByRole(AriaRole.Heading, new() { Name = "Welcome Back" });
        public ILocator RememberMeCheckbox => Page.GetByLabel("Remember Me");
        public ILocator ForgotPasswordLink => Page.GetByRole(AriaRole.Link, new() { Name = "Forgot Password?" });
        public ILocator CreateNewAccountLink => Page.GetByRole(AriaRole.Link, new() { Name = "Create New Account" });
        public ILocator LoginMessage => Page.Locator("#loginMessage");

        public async Task VerifyEmptyLoginForm()
        {
            await Expect(Email).ToBeVisibleAsync();
            await Expect(Email).ToBeEnabledAsync();
            await Expect(Email).ToBeEmptyAsync();

            await Expect(Password).ToBeVisibleAsync();
            await Expect(Password).ToBeEnabledAsync();
            await Expect(Password).ToBeEmptyAsync();

            await Expect(RememberMeCheckbox).ToBeVisibleAsync();
            await Expect(RememberMeCheckbox).ToBeEnabledAsync();
            await Expect(RememberMeCheckbox).Not.ToBeCheckedAsync();

            await Expect(LoginButton).ToBeVisibleAsync();
            await Expect(LoginButton).ToBeEnabledAsync();
            await Expect(LoginButton).ToHaveTextAsync("Login");

            await Expect(ForgotPasswordLink).ToBeVisibleAsync();
            await Expect(ForgotPasswordLink).ToBeEnabledAsync();
            await Expect(ForgotPasswordLink).ToHaveTextAsync("Forgot Password?");

            await Expect(CreateNewAccountLink).ToBeVisibleAsync();
            await Expect(CreateNewAccountLink).ToBeEnabledAsync();
            await Expect(CreateNewAccountLink).ToHaveTextAsync("Create New Account");
        }

        public async Task VerifyRegisteredUserLoginPageDisplayed()
        {
            await Expect(Page).ToHaveURLAsync(LoginUrl_RegisteredUserRedirect);
            await Expect(Heading).ToHaveTextAsync("Welcome Back");
        }
    }
}
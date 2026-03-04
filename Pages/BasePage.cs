using Microsoft.Playwright;

namespace TestAssignment.Pages
{
    public class BasePage
    {
        protected readonly IPage Page;

        public BasePage(IPage page) => Page = page;

        private const string BaseUrl = "https://qa-test-web-app.vercel.app/";

        public string LoginUrl => $"{BaseUrl}index.html";
        public string RegisterUrl => $"{BaseUrl}register.html";
        public string DashboardUrl => $"{BaseUrl}dashboard.html";
        public string LoginUrl_RegisteredUserRedirect => $"{BaseUrl}index.html?registered=true";

        public async Task GoToRegister() => await Page.GotoAsync(RegisterUrl);
    }
}
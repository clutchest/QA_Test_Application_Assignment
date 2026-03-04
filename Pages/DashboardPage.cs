using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace TestAssignment.Pages
{
    public class DashboardPage : BasePage
    {
        public DashboardPage(IPage page) : base(page) { }

        public ILocator Heading => Page.GetByRole(AriaRole.Heading, new() { Name = "Dashboard" });
        public ILocator WelcomeMessage => Page.Locator("h2");
    }
}
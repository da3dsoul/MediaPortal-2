using MP2BootstrapperApp.ViewModels;

namespace MP2BootstrapperApp.Nav
{
  public class WelcomePage : IState
  {
    public void Enter(Wizard wizard, InstallWizardViewModel viewModel)
    {
      viewModel.Content = new OverviewViewModel();
      wizard.NextState = new OverviewPage();
    }
  }
}

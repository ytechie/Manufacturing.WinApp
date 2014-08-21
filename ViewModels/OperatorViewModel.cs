using Manufacturing.WinApp.Common;
using Manufacturing.WinApp.Views.Operator;

namespace Manufacturing.WinApp.ViewModels
{
    [MenuScreen(typeof(OperatorPage), "Operator Demo")]
    [RequiredRole("Operator")]
    public class OperatorViewModel : MenuScreenViewModel
    {
        public void Start()
        {

        }

        public void Stop()
        {

        }
    }
}
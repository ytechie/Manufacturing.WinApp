using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Manufacturing.WinApp.Views.UserManagment
{
    public sealed partial class UserEditorControl : UserControl
    {
        public delegate void CancelledEditor(object sender, CancelledEditorEventArgs args);
        public delegate void AcceptedEditor(object sender, AcceptedEditorEventArgs args);

        public UserEditorControl()
        {
            InitializeComponent();
            VisualStateManager.GoToState(this, "hidden", false);
        }

        public bool Shown
        {
            get { return (bool)GetValue(ShownProperty); }
            set { SetValue(ShownProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Shown.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShownProperty =
            DependencyProperty.Register("Shown", typeof(bool), typeof(UserEditorControl), new PropertyMetadata(false, ShownChanged));

        private static void ShownChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pe = d as UserEditorControl;
            if (pe != null)
            {
                if ((bool)e.NewValue) pe.Show(); else pe.Hide();
            }
        }

        public event CancelledEditor Cancelled;
        public event AcceptedEditor Accepted;

        public void InvokeCancelled()
        {
            var cncl = Cancelled;
            if (cncl != null)
            {
                cncl.Invoke(this, new CancelledEditorEventArgs());
            }
        }

        public void InvokeAccepted()
        {
            var accpt = Accepted;
            if (accpt != null)
            {
                accpt.Invoke(this, new AcceptedEditorEventArgs());
            }
        }

        public void Show()
        {
            VisualStateManager.GoToState(this, "shown", false);
        }

        public void Hide()
        {
            VisualStateManager.GoToState(this, "hidden", false);
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            Shown = false;
            InvokeCancelled();
        }

        private void AcceptClick(object sender, RoutedEventArgs e)
        {
            Shown = false;
            InvokeAccepted();
        }

    }

    public class CancelledEditorEventArgs : EventArgs
    {
    }

    public class AcceptedEditorEventArgs : EventArgs
    {
    }

}

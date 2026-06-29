using Avalonia.Controls;

namespace Launcher.Views
{
    public partial class DropFilterWindow : Window
    {
        public DropFilterWindow()
        {
            InitializeComponent();
        }

        protected override void OnAttachedToVisualTree(Avalonia.VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (this.FindControl<CheckBox>("AcceptCheckbox") is CheckBox acceptBox)
            {
                acceptBox.IsCheckedChanged += (_, _) =>
                {
                    if (acceptBox.IsChecked == true)
                    {
                        this.FindControl<CheckBox>("RejectCheckbox")?.SetValue(CheckBox.IsCheckedProperty, false);
                    }
                };
            }

            if (this.FindControl<CheckBox>("RejectCheckbox") is CheckBox rejectBox)
            {
                rejectBox.IsCheckedChanged += (_, _) =>
                {
                    if (rejectBox.IsChecked == true)
                    {
                        this.FindControl<CheckBox>("AcceptCheckbox")?.SetValue(CheckBox.IsCheckedProperty, false);
                    }
                };
            }
        }
    }
}

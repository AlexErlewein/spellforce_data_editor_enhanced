using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;

namespace SpellforceDataEditor.Avalonia
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();

            var manualLinkButton = this.FindControl<Button>("ManualLinkButton");
            if (manualLinkButton != null)
            {
                manualLinkButton.Click += ManualLinkButton_Click;
            }

            var discordLinkButton = this.FindControl<Button>("DiscordLinkButton");
            if (discordLinkButton != null)
            {
                discordLinkButton.Click += DiscordLinkButton_Click;
            }

            var nexusLinkButton = this.FindControl<Button>("NexusLinkButton");
            if (nexusLinkButton != null)
            {
                nexusLinkButton.Click += NexusLinkButton_Click;
            }
        }

        private void ManualLinkButton_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://drive.google.com/file/d/1GO6NQdl-1x7wgddGORdIAhZj86SWGmum/view?usp=sharing");
        }

        private void DiscordLinkButton_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://discordapp.com/invite/spellforce");
        }

        private void NexusLinkButton_Click(object sender, RoutedEventArgs e)
        {
            OpenUrl("https://forums.nexusmods.com/index.php?/user/53072901-leszekd25/");
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine($"Failed to open URL {url}: {ex.Message}");
            }
        }
    }
}
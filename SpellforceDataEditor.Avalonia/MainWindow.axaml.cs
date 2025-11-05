using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace SpellforceDataEditor.Avalonia
{
    public partial class MainWindow : Window
    {
        private Button gameDataEditorButton;
        private Button loadCffFileButton;

        public MainWindow()
        {
            InitializeComponent();
            gameDataEditorButton = this.FindControl<Button>("GameDataEditorButton");
            if (gameDataEditorButton != null)
            {
                gameDataEditorButton.Click += GameDataEditorButton_Click;
            }
            loadCffFileButton = this.FindControl<Button>("LoadCffFileButton");
            if (loadCffFileButton != null)
            {
                loadCffFileButton.Click += LoadCffFileButton_Click;
            }
        }

        private void GameDataEditorButton_Click(object sender, RoutedEventArgs e)
        {
            var editorWindow = new CffEditorWindow();
            editorWindow.Show();
        }

        private async void LoadCffFileButton_Click(object sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);

            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open CFF File",
                AllowMultiple = false,
                FileTypeFilter = new[] { new FilePickerFileType("CFF Files") { Patterns = new[] { "*.cff" } } }
            });

            if (files.Count >= 1)
            {
                var gameData = new SFEngine.SFCFF.SFGameDataNew();
                var result = gameData.Load(files[0].Path.LocalPath);

                if (result == 0)
                {
                    var editorWindow = new CffEditorWindow(gameData);
                    editorWindow.Show();
                }
            }
        }
    }
}
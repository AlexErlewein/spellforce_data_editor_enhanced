using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System.Diagnostics;

namespace SpellforceDataEditor.Avalonia
{
    public partial class MainWindow : Window
    {
        private Button gameDataEditorButton;
        private Button loadCffFileButton;
        private Button assetViewerButton;
        private Button mapEditorButton;
        private Button sqlModifierButton;
        private Button specifyGameDirButton;
        private Button aboutButton;
        private TextBlock gameDirStatusLabel;
        private TextBlock versionLabel;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize existing buttons
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

            // Initialize new buttons
            assetViewerButton = this.FindControl<Button>("AssetViewerButton");
            if (assetViewerButton != null)
            {
                assetViewerButton.Click += AssetViewerButton_Click;
            }

            mapEditorButton = this.FindControl<Button>("MapEditorButton");
            if (mapEditorButton != null)
            {
                mapEditorButton.Click += MapEditorButton_Click;
            }

            sqlModifierButton = this.FindControl<Button>("SqlModifierButton");
            if (sqlModifierButton != null)
            {
                sqlModifierButton.Click += SqlModifierButton_Click;
            }

            specifyGameDirButton = this.FindControl<Button>("SpecifyGameDirButton");
            if (specifyGameDirButton != null)
            {
                specifyGameDirButton.Click += SpecifyGameDirButton_Click;
            }

            aboutButton = this.FindControl<Button>("AboutButton");
            if (aboutButton != null)
            {
                aboutButton.Click += AboutButton_Click;
            }

            // Initialize status labels
            gameDirStatusLabel = this.FindControl<TextBlock>("GameDirStatusLabel");
            versionLabel = this.FindControl<TextBlock>("VersionLabel");
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
                    var editorWindow = new CffEditorWindow(gameData, files[0].Path.LocalPath);
                    editorWindow.Show();
                }
            }
        }

        private void AssetViewerButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement Asset Viewer window
            Debug.WriteLine("Asset Viewer button clicked - not implemented yet");
        }

        private void MapEditorButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement Map Editor window
            Debug.WriteLine("Map Editor button clicked - not implemented yet");
        }

        private void SqlModifierButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Implement SQL Modifier window
            Debug.WriteLine("SQL Modifier button clicked - not implemented yet");
        }

        private async void SpecifyGameDirButton_Click(object sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);

            var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select SpellForce Game Directory",
                AllowMultiple = false
            });

            if (folders.Count >= 1)
            {
                var selectedPath = folders[0].Path.LocalPath;
                // TODO: Store and validate the game directory
                if (gameDirStatusLabel != null)
                {
                    gameDirStatusLabel.Text = $"Game directory:\n{selectedPath}";
                }
                Debug.WriteLine($"Game directory selected: {selectedPath}");
            }
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.Show();
        }
    }
}
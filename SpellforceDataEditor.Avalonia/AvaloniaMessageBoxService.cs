using Avalonia.Controls;
using SFEngine;
using System.Threading.Tasks;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace SpellforceDataEditor.Avalonia
{
    public class AvaloniaMessageBoxService : IMessageBoxService
    {
        public SFEngine.DialogResult Show(string text, string caption, SFEngine.MessageBoxButtons buttons)
        {
            var box = MessageBoxManager.GetMessageBoxStandard(caption, text, (ButtonEnum)buttons);
            var result = box.ShowAsync().Result;

            switch (result)
            {
                case MsBox.Avalonia.Enums.ButtonResult.Ok:
                    return SFEngine.DialogResult.OK;
                case MsBox.Avalonia.Enums.ButtonResult.Cancel:
                    return SFEngine.DialogResult.Cancel;
                case MsBox.Avalonia.Enums.ButtonResult.Yes:
                    return SFEngine.DialogResult.Yes;
                case MsBox.Avalonia.Enums.ButtonResult.No:
                    return SFEngine.DialogResult.No;
                default:
                    return SFEngine.DialogResult.None;
            }
        }
    }
}

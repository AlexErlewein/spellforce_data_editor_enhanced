
using SFEngine;
using System.Windows.Forms;

namespace SpellforceDataEditor
{
    public class MessageBoxService : IMessageBoxService
    {
        public SFEngine.DialogResult Show(string text, string caption, SFEngine.MessageBoxButtons buttons)
        {
            System.Windows.Forms.MessageBoxButtons wpfButtons = System.Windows.Forms.MessageBoxButtons.OK;
            switch (buttons)
            {
                case SFEngine.MessageBoxButtons.OK:
                    wpfButtons = System.Windows.Forms.MessageBoxButtons.OK;
                    break;
                case SFEngine.MessageBoxButtons.OKCancel:
                    wpfButtons = System.Windows.Forms.MessageBoxButtons.OKCancel;
                    break;
                case SFEngine.MessageBoxButtons.YesNo:
                    wpfButtons = System.Windows.Forms.MessageBoxButtons.YesNo;
                    break;
                case SFEngine.MessageBoxButtons.YesNoCancel:
                    wpfButtons = System.Windows.Forms.MessageBoxButtons.YesNoCancel;
                    break;
            }

            System.Windows.Forms.DialogResult result = MessageBox.Show(text, caption, wpfButtons);

            switch (result)
            {
                case System.Windows.Forms.DialogResult.OK:
                    return SFEngine.DialogResult.OK;
                case System.Windows.Forms.DialogResult.Cancel:
                    return SFEngine.DialogResult.Cancel;
                case System.Windows.Forms.DialogResult.Yes:
                    return SFEngine.DialogResult.Yes;
                case System.Windows.Forms.DialogResult.No:
                    return SFEngine.DialogResult.No;
                default:
                    return SFEngine.DialogResult.None;
            }
        }
    }
}


namespace SFEngine
{
    public enum MessageBoxButtons
    {
        OK,
        OKCancel,
        YesNo,
        YesNoCancel
    }

    public enum DialogResult
    {
        None,
        OK,
        Cancel,
        Yes,
        No
    }

    public interface IMessageBoxService
    {
        DialogResult Show(string text, string caption, MessageBoxButtons buttons);
    }
}

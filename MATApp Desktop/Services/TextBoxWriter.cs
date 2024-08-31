using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

public class TextBoxWriter : TextWriter
{
    private TextBox _textBox;

    public TextBoxWriter(TextBox textBox)
    {
        _textBox = textBox;
    }

    public override void Write(char value)
    {
        if (_textBox.InvokeRequired)
        {
            _textBox.Invoke((MethodInvoker)delegate { _textBox.AppendText(value.ToString()); });
        }
        else
        {
            _textBox.AppendText(value.ToString());
        }
    }

    public override void Write(string value)
    {
        if (_textBox.InvokeRequired)
        {
            _textBox.Invoke((MethodInvoker)delegate { _textBox.AppendText(value); });
        }
        else
        {
            _textBox.AppendText(value);
        }
    }

    public override Encoding Encoding
    {
        get { return Encoding.UTF8; }
    }
}

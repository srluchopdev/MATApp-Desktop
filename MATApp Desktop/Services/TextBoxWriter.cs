using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MATAppDesktop.Services
{
    public class TextBoxWriter : TextWriter
    {
        private readonly TextBox _textBox;

        public TextBoxWriter(TextBox textBox)
        {
            _textBox = textBox ?? throw new ArgumentNullException(nameof(textBox));
        }

        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value)
        {
            // Asegúrate de que la actualización del TextBox se haga en el hilo de la UI
            if (_textBox.InvokeRequired)
            {
                _textBox.Invoke(new Action(() =>
                {
                    _textBox.AppendText(value.ToString());
                }));
            }
            else
            {
                _textBox.AppendText(value.ToString());
            }
        }

        public override void Write(string value)
        {
            // Asegúrate de que la actualización del TextBox se haga en el hilo de la UI
            if (_textBox.InvokeRequired)
            {
                _textBox.Invoke(new Action(() =>
                {
                    _textBox.AppendText(value ?? string.Empty);
                }));
            }
            else
            {
                _textBox.AppendText(value ?? string.Empty);
            }
        }

        public override void WriteLine(string value)
        {
            // Asegúrate de que la actualización del TextBox se haga en el hilo de la UI
            if (_textBox.InvokeRequired)
            {
                _textBox.Invoke(new Action(() =>
                {
                    _textBox.AppendText(value ?? string.Empty + Environment.NewLine);
                }));
            }
            else
            {
                _textBox.AppendText(value ?? string.Empty + Environment.NewLine);
            }
        }
    }
}
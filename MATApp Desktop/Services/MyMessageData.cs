using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MATAppDesktop.Services
{
    public class MyMessageData
    {
        public string Address { get; set; }
        public object[] Arguments { get; set; }

        public void Write(BinaryWriter writer)
        {
            // Implementación de la función Write que convierte los datos a bytes
            writer.Write(Address);
            foreach (var arg in Arguments)
            {
                if (arg is int intValue)
                {
                    writer.Write(intValue);
                }
                else if (arg is float floatValue)
                {
                    writer.Write(floatValue);
                }
                else if (arg is string stringValue)
                {
                    writer.Write(stringValue);
                }
                // Agrega más conversiones según sea necesario
            }
        }
    }
}

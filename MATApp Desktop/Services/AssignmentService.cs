using Newtonsoft.Json;
using System.Collections.Generic;

namespace MATAppDesktop.Services
{
    public class AssignmentService
    {
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
        public string Description { get; set; }
        private Dictionary<string, string> _assignments;

        public AssignmentService()
        {
            _assignments = new Dictionary<string, string>();
        }

        public void AssignColliderToIP(string collider, string ip)
        {
            if (_assignments.ContainsKey(collider))
            {
                _assignments[collider] = ip;
            }
            else
            {
                _assignments.Add(collider, ip);
            }
        }

        public string GetAssignedIP(string collider)
        {
            return _assignments.ContainsKey(collider) ? _assignments[collider] : null;
        }

        public Dictionary<string, string> GetAllAssignments()
        {
            return _assignments;
        }

        public void SaveAssignments(string filePath)
        {
            // Serializar la lista de asignaciones a un string JSON
            string json = JsonConvert.SerializeObject(_assignments, Formatting.Indented);

            // Escribir el JSON en un archivo
            File.WriteAllText(filePath, json);
        }

        public void LoadAssignments(string filePath)
        {
            try
            {
                // Leer el contenido del archivo JSON
                string json = File.ReadAllText(filePath);

                // Deserializar el JSON en un diccionario
                _assignments = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al cargar asignaciones: " + ex.Message);
                // Manejar el error de forma adecuada (por ejemplo, mostrar un mensaje al usuario)
            }
        }
    }
}

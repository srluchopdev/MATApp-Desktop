using System.Collections.Generic;

namespace MATAppDesktop.Services
{
    public class AssignmentService
    {
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
            // Lógica para guardar las asignaciones en un archivo (JSON, CSV, etc.)
        }

        public void LoadAssignments(string filePath)
        {
            // Lógica para cargar las asignaciones desde un archivo
        }
    }
}

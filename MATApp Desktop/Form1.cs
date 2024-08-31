using MATAppDesktop.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MATApp_Desktop
{
    public partial class Form1 : Form
    {
        private NetworkScanner _networkScanner;
        private OscManager _oscManager;

        public Form1()
        {
            InitializeComponent();
            _networkScanner = new NetworkScanner();
            _oscManager = new OscManager("127.0.0.1", 9000);

            LoadAvailableIPs();
            LoadColliders();
            LoadAssignments();
        }

        private void LoadAvailableIPs()
        {
            var ips = _networkScanner.ScanNetworkForSlimeVRDevices();
            if (ips != null && ips.Any())
            {
                cmbIPs.DataSource = ips;
            }
            else
            {
                MessageBox.Show("No se encontraron dispositivos SlimeVR.");
            }
        }

        private void LoadColliders()
        {
            // Envía una solicitud OSC para obtener los colliders del avatar
            _oscManager.SendMessage("/avatar/requestColliders", 1); // 1 es solo un valor de ejemplo para iniciar la solicitud

            // Espera la respuesta y maneja la respuesta en un evento de recepción
            _oscManager.OnColliderListReceived += OnColliderListReceived;
        }

        private void OnColliderListReceived(List<string> colliders)
        {
            if (colliders != null && colliders.Any())
            {
                lstColliders.Items.Clear();
                lstColliders.Items.AddRange(colliders.ToArray());
            }
            else
            {
                MessageBox.Show("No se recibieron colliders.");
            }
        }

        private void btnAssign_Click(object sender, EventArgs e)
        {
            // Verificar que el usuario haya seleccionado una IP y un collider antes de continuar
            var selectedIP = cmbIPs.SelectedItem?.ToString();
            var selectedCollider = lstColliders.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedIP) && !string.IsNullOrEmpty(selectedCollider))
            {
                AssignColliderToIP(selectedCollider, selectedIP);
                lstAssignments.Items.Add($"{selectedCollider} asignado a {selectedIP}");
            }
            else
            {
                MessageBox.Show("Por favor, selecciona una IP y un collider.");
            }
        }

        private void AssignColliderToIP(string collider, string ip)
        {
            _oscManager = new OscManager(ip, 9000); // 9000 es un puerto de ejemplo, ajusta según sea necesario
            _oscManager.SendMessage($"/avatar/{collider}/motor", 1); // Envía una señal para activar el motor
        }

        private void SaveAssignments()
        {
            using (StreamWriter sw = new StreamWriter("assignments.txt"))
            {
                foreach (var item in lstAssignments.Items)
                {
                    sw.WriteLine(item.ToString());
                }
            }
        }

        private void LoadAssignments()
        {
            if (File.Exists("assignments.txt"))
            {
                using (StreamReader sr = new StreamReader("assignments.txt"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        lstAssignments.Items.Add(line);
                    }
                }
            }
        }

        private void lstColliders_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lstAssignments_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void cmbIPs_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // Método para verificar si los colliders están llegando correctamente
        private void btnVerifyColliders_Click(object sender, EventArgs e)
        {
            if (lstColliders.Items.Count > 0)
            {
                // Asegúrate de que el primer ítem no sea nulo antes de convertirlo a string
                var firstItem = lstColliders.Items[0]?.ToString();

                if (!string.IsNullOrEmpty(firstItem) && firstItem != "[]/avatar/requestColliders[]")
                {
                    MessageBox.Show("Los colliders están llegando correctamente.");
                }
                else
                {
                    MessageBox.Show("Los colliders no están llegando correctamente. Intenta nuevamente.");
                }
            }
            else
            {
                MessageBox.Show("La lista de colliders está vacía.");
            }
        }
    }
}
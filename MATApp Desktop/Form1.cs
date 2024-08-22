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
            var selectedIP = cmbIPs.SelectedItem.ToString();
            var selectedCollider = lstColliders.SelectedItem.ToString();

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
            _oscManager.SendMessage($"/avatar/{collider}/motor", 1); // Envia una señal para activar el motor
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
    }
}

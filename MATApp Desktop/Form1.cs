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
            LoadAvailableIPs();
            LoadColliders();
            LoadAssignments();

        }

        private void LoadAvailableIPs()
        {
            var ips = _networkScanner.ScanNetworkForSlimeVRDevices();
            cmbIPs.DataSource = ips;
        }

        private void LoadColliders()
        {
            var colliders = new[] { "pecho", "abdomen", "pelvis", "brazo D", "brazo I", "muslo D", "muslo I", "pierna D", "pierna I", "pie D", "pie I" };
            lstColliders.Items.AddRange(colliders);
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

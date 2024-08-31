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
        private TextBoxWriter _textBoxWriter;

        public Form1()
        {
            InitializeComponent();
            _networkScanner = new NetworkScanner();
            _oscManager = new OscManager("127.0.0.1", 9000);
            _textBoxWriter = new TextBoxWriter(textBoxLogs);
            Console.SetOut(_textBoxWriter);

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
                LogMessage("No se encontraron dispositivos SlimeVR.");
            }
        }

        private void LoadColliders()
        {
            //aca parece que esta el problema despues consultarle a la IA
            _oscManager.SendMessage("/avatar/requestColliders", 1);
            _oscManager.OnColliderListReceived += OnColliderListReceived;
        }

        private void OnColliderListReceived(List<string> colliders)
        {
            if (lstColliders.InvokeRequired)
            {
                lstColliders.Invoke(new Action(() =>
                {
                    UpdateColliderList(colliders);
                }));
            }
            else
            {
                UpdateColliderList(colliders);
            }
        }

        private void UpdateColliderList(List<string> colliders)
        {
            if (colliders != null && colliders.Any())
            {
                lstColliders.Items.Clear();
                lstColliders.Items.AddRange(colliders.ToArray());
            }
            else
            {
                LogMessage("No se recibieron colliders.");
            }
        }

        private void btnAssign_Click(object sender, EventArgs e)
        {
            var selectedIP = cmbIPs.SelectedItem?.ToString();
            var selectedCollider = lstColliders.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedIP) && !string.IsNullOrEmpty(selectedCollider))
            {
                AssignColliderToIP(selectedCollider, selectedIP);
                lstAssignments.Items.Add($"{selectedCollider} asignado a {selectedIP}");
            }
            else
            {
                LogMessage("Por favor, selecciona una IP y un collider.");
            }
        }

        private void AssignColliderToIP(string collider, string ip)
        {
            _oscManager = new OscManager(ip, 9000);
            _oscManager.SendMessage($"/avatar/{collider}/motor", 1);
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

        private void btnVerifyColliders_Click(object sender, EventArgs e)
        {
            if (lstColliders.Items.Count > 0)
            {
                var firstItem = lstColliders.Items[0]?.ToString();

                if (!string.IsNullOrEmpty(firstItem) && firstItem != "[]/avatar/requestColliders[]")
                {
                    LogMessage("Los colliders están llegando correctamente.");
                }
                else
                {
                    LogMessage("Los colliders no están llegando correctamente. Intenta nuevamente.");
                }
            }
            else
            {
                LogMessage("La lista de colliders está vacía.");
            }
        }

        // Nuevo método para registrar mensajes en el TextBox
        private void LogMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
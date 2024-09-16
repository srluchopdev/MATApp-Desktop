using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MATAppDesktop.Services;

namespace MATApp_Desktop
{
    public partial class Form1 : Form
    {
        private System.Timers.Timer _oscSendTimer;
        private bool _formLoaded = false; // Indicamos que el formulario está cargado
        private NetworkScanner _networkScanner;
        private OscManager _oscManager;
        private TextBoxWriter _textBoxWriter;
        private List<string> someObject; // Cambio de object a List<string>

        public Form1()
        {
            InitializeComponent();
            this.Shown += Form1_Shown; // Suscribirse al evento Shown
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            _formLoaded = true; // El formulario está completamente cargado

            string ipAddress = "127.0.0.1";
            int receivePort = 7000;
            int sendPort = 8000;
            _oscManager = new OscManager(ipAddress, receivePort, sendPort, this);

            _oscManager.OnColliderListReceived += colliders =>
            {
                // Manejar la lista de colliders recibidos
                _textBoxWriter.WriteLine("Colliders recibidos:");
                foreach (var collider in colliders)
                {
                    _textBoxWriter.WriteLine(collider);
                }
            };

            _networkScanner = new NetworkScanner();
            _textBoxWriter = new TextBoxWriter(textBoxLogs);
            Console.SetOut(_textBoxWriter);

            // Inicialización del temporizador
            InitializeOscSendTimer();

            this.FormClosing += Form1_FormClosing;
            _oscManager.OnColliderListReceived += UpdateColliderList;

            LoadAvailableIPs();
            LoadColliders();
            LoadAssignments();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _oscManager?.Dispose();
        }

        private void InitializeOscSendTimer()
        {
            // Configura y arranca el temporizador para enviar mensajes OSC periódicamente
            _oscSendTimer = new System.Timers.Timer(1000); // Intervalo de 1 segundo
            _oscSendTimer.Elapsed += OnTimedEvent;
            _oscSendTimer.Start();
        }

        private void UpdateColliderList(List<string> colliders) // Cambio de firma a List<string>
        {
            if (_formLoaded && this.IsHandleCreated)
            {
                if (colliders != null && colliders.Any())
                {
                    listBoxColliders.Invoke(new Action(() =>
                    {
                        listBoxColliders.Items.Clear();
                        listBoxColliders.Items.AddRange(colliders.ToArray());
                        _textBoxWriter.WriteLine("Colliders recibidos correctamente: " + string.Join(", ", colliders));
                    }));
                }
                else
                {
                    _textBoxWriter.WriteLine("No se recibieron colliders.");
                }
            }
        }

        private void LoadAvailableIPs()
        {
            var ips = _networkScanner.ScanNetworkForSlimeVRDevices();
            if (ips != null && ips.Any())
            {
                if (_formLoaded && this.IsHandleCreated)
                {
                    cmbIPs.Invoke(new Action(() =>
                    {
                        cmbIPs.DataSource = ips;
                    }));
                }
            }
            else
            {
                _textBoxWriter.WriteLine("No se encontraron dispositivos SlimeVR.");
            }
        }

        private void LoadColliders()
        {
            _oscManager.SendMessage("/avatar/requestColliders", 1); // 1 es solo un valor de ejemplo para iniciar la solicitud
        }

        private void btnAssign_Click(object sender, EventArgs e)
        {
            var selectedIP = cmbIPs.SelectedItem?.ToString();
            var selectedCollider = listBoxColliders.SelectedItem?.ToString();

            if (!string.IsNullOrEmpty(selectedIP) && !string.IsNullOrEmpty(selectedCollider))
            {
                AssignColliderToIP(selectedCollider, selectedIP);
                lstAssignments.Invoke(new Action(() =>
                {
                    lstAssignments.Items.Add($"{selectedCollider} asignado a {selectedIP}");
                }));
                _textBoxWriter.WriteLine($"{selectedCollider} asignado a {selectedIP}");
            }
            else
            {
                _textBoxWriter.WriteLine("Por favor, selecciona una IP y un collider.");
            }
        }

        private void AssignColliderToIP(string collider, string ip)
        {
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
            _textBoxWriter.WriteLine("Asignaciones guardadas.");
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
                        if (_formLoaded && this.IsHandleCreated)
                        {
                            lstAssignments.Invoke(new Action(() =>
                            {
                                lstAssignments.Items.Add(line);
                            }));
                        }
                    }
                }
                _textBoxWriter.WriteLine("Asignaciones cargadas.");
            }
            else
            {
                _textBoxWriter.WriteLine("No se encontraron asignaciones para cargar.");
            }
        }

        private void lstColliders_SelectedIndexChanged(object sender, EventArgs e) { }

        private void lstAssignments_SelectedIndexChanged(object sender, EventArgs e) { }

        private void Form1_Load(object sender, EventArgs e) { }

        private void cmbIPs_SelectedIndexChanged(object sender, EventArgs e) { }

        private void textBoxLogs_TextChanged(object sender, EventArgs e) { }

        private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            _oscManager.SendMessage("/avatar/requestColliders", 1);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Guarda las asignaciones cuando el formulario se cierra
            SaveAssignments();
        }
    }
}
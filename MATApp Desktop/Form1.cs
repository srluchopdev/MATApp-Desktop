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
        private bool _formLoaded = false;
        private NetworkScanner _networkScanner;
        private OscManager _oscManager;
        private TextBoxWriter _textBoxWriter;

        public Form1()
        {
            InitializeComponent();
            _formLoaded = true; // Indicamos que el formulario está cargado

            string ipAddress = "127.0.0.1"; // IP donde Unity está enviando los mensajes
            int port = 9000; // Puerto en el que MATApp escuchará los mensajes OSC

            _networkScanner = new NetworkScanner();
            _oscManager = new OscManager(ipAddress, port, this);
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

        private void InitializeOscSendTimer()
        {
            // Configura y arranca el temporizador para enviar mensajes OSC periódicamente
            _oscSendTimer = new System.Timers.Timer(1000); // Intervalo de 1 segundo
            _oscSendTimer.Elapsed += OnTimedEvent;
            _oscSendTimer.Start();
        }

        private void UpdateColliderList(List<string> colliders)
        {
            if (this.IsHandleCreated)
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
                cmbIPs.DataSource = ips;
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
            _oscManager = new OscManager(ip, 9000, this);
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
                        lstAssignments.Invoke(new Action(() =>
                        {
                            lstAssignments.Items.Add(line);
                        }));
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

        private void btnVerifyColliders_Click(object sender, EventArgs e)
        {
            if (listBoxColliders.Items.Count > 0)
            {
                var firstItem = listBoxColliders.Items[0]?.ToString();

                if (!string.IsNullOrEmpty(firstItem) && firstItem != "[]/avatar/requestColliders[]")
                {
                    _textBoxWriter.WriteLine("Los colliders están llegando correctamente.");
                }
                else
                {
                    _textBoxWriter.WriteLine("Los colliders no están llegando correctamente. Intenta nuevamente.");
                }
            }
            else
            {
                _textBoxWriter.WriteLine("La lista de colliders está vacía.");
            }
        }

        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                // Verifica si hay datos OSC que enviar
                if (_oscManager != null)
                {
                    // Envía un mensaje OSC a Unity
                    _oscManager.SendMessage("/avatar/requestColliders", 1);
                }

                // Actualiza la UI
                AppendTextToLogs("Mensaje OSC enviado a Unity.");
            }
            catch (InvalidAsynchronousStateException ex)
            {
                Console.WriteLine($"Error invocando el método en el formulario: {ex.Message}");
            }
        }

        private void AppendTextToLogs(string text)
        {
            if (textBoxLogs.InvokeRequired)
            {
                // Invoca al hilo principal de la UI para actualizar el TextBox
                textBoxLogs.Invoke(new Action(() => textBoxLogs.AppendText(text + Environment.NewLine)));
            }
            else
            {
                // Si ya estamos en el hilo principal de la UI
                textBoxLogs.AppendText(text + Environment.NewLine);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_oscSendTimer != null)
            {
                _oscSendTimer.Stop();
                _oscSendTimer.Dispose();
            }

            if (_oscManager != null)
            {
                _oscManager.Dispose();
            }
        }
    }
}
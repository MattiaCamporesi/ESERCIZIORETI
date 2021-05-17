using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//aggiunta
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ComunicazioneSocket
{
    

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Mattia Camporesi CLASSE 4^L
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Il Localendpoint inizializza l'indirizzo ip a 127.0.0.1 con la porta 56000 
            IPEndPoint localendpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 56000);
            // Thread che riceve dati utilizzando in seguito un metodo 
            Thread t1 = new Thread(new ParameterizedThreadStart(SocketReceive));
            // a questo punto viene avviato il thread
            t1.Start(localendpoint);


        }
        // Questo è un metodo utilizzato per ricevere i dati del Socket 
        public async void SocketReceive (object sourceEndPoint)
        {
            // Viene inserito all'interno il Localendpoint inserito in precedenza
            IPEndPoint sourceEP = (IPEndPoint)sourceEndPoint;
            // viene utilizzato il protocol UDP
            Socket t = new Socket(sourceEP.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            t.Bind(sourceEP);
            // crea i byte 
            Byte[] byteRicevuti = new byte[256];
            string message = "";
            // e li inizializza
            int bytes = 0;
            // con questo avvio la task
            await Task.Run(() =>
            {
                while (true)
                {
                    // se available ha un valore maggiore di Zero entra dentro l'if e lo esegue 
                    if(t.Available>0)
                    {
                        message = "";
                        // con questo arrivano i dati in ricezione
                        bytes = t.Receive(byteRicevuti, byteRicevuti.Length, 0);
                        // attraverso questo viene decodificato il messaggio 
                        message = message + Encoding.ASCII.GetString(byteRicevuti, 0, bytes);

                        // viene mostrato all'interno della label
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            lblRicezione.Content = message;
                        }));
                    }


                }

            });
        }
        // creo un bottone di nome invia
        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            // appena ricevuto l'indirizzo del destinario lo imposto e sono pronto per l'invio 
            // della risposta 
            IPAddress ipDest = IPAddress.Parse(txtIpAdd.Text);
            int portDest = int.Parse(txtDestPort.Text);
            // gli assegno un indirizzo ip remoto 
            IPEndPoint remoteEndPoint = new IPEndPoint(ipDest, portDest);
            // protocollo udp usato per mandare i dati
            Socket s = new Socket(ipDest.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            // codifica messaggio 
            Byte[] byteInviati = Encoding.ASCII.GetBytes(txtMsg.Text);
            // invio 
            s.SendTo(byteInviati, remoteEndPoint);
        }
    }
}

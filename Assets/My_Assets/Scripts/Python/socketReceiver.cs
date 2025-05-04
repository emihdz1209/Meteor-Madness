// Assets/Scripts/SocketReceiver.cs
//protocolo Socket TCP para variables FocusIndex y Jaw
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class SocketReceiver : MonoBehaviour
{
    private TcpClient client; //cliente que se va a conectar 
    private NetworkStream stream; //flujo de datos del socket
    private Thread receiveThread; //para recibir datos en este hilo 
    private volatile bool isRunning = false; //controla el ciclo del hilo

    private readonly object dataLock = new object();
    private string lastReceivedData = ""; //ultimo data recibido

    public float FocusIndex {get; private set;} = 0; //indice concentración
    public int Jaw {get; private set;} = 0; //botón JAW

    void Start()
    {
        ConnectToServer("10.43.46.33", 50505); //conexión a host local 
    }

    void ConnectToServer(string host, int port)
    {
        try
        {   //se crea al cliente y empieza el stream de datos en el objeto
            client = new TcpClient(host, port);
            stream = client.GetStream();
            isRunning = true;

            //hilo donde se recibirán los datos
            receiveThread = new Thread(ReceiveData);
            receiveThread.IsBackground = true;
            receiveThread.Start();

            Debug.Log("Conectado al servidor LSL.");
        }
        catch (Exception e)
        {
            Debug.LogError("Error al conectar al servidor: " + e.Message);
        }
    }

    void ReceiveData()
    {
        byte[] buffer = new byte[8]; // 4 bytes (float) + 4 bytes (int)

        while (isRunning)
        {
            try
            {
                if (stream != null && stream.DataAvailable)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead == 8)
                    {
                        float focus = BitConverter.ToSingle(buffer, 0);
                        int jaw = BitConverter.ToInt32(buffer, 4);

                        lock (dataLock)
                        {
                            FocusIndex = focus;
                            Jaw = jaw;
                        }

                        // Debug.Log($"Focus: {focus}, Jaw: {jaw}");
                    }
                }
                Thread.Sleep(10);
            }
            catch (Exception e)
            {
                Debug.LogError("Error al recibir datos: " + e.Message);
                StopConnection();
                break;
            }
        }
    }





    void OnApplicationQuit()
    {
        StopConnection();
    }

    void StopConnection()
    {
        isRunning = false;

        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Join(100);
        }

        try { stream?.Close(); } catch { }
        try { client?.Close(); } catch { }

        Debug.Log(" Conexión cerrada.");
    }

    //para que se pueda acceder a los datos sin tener que accceder a la string completa 
    public (float focus, int jaw) GetFocusAndJaw(){
        return (FocusIndex, Jaw);
    }


}
 
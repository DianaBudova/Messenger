using SimpleTCP;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace Messenger.BL;

public class TCPClient
{
    public delegate void ClientDisconnectedHandler();
    public event ClientDisconnectedHandler? ClientDisconnected;
    public event Action<Models.Application.Message>? MessageReceived;
    private readonly IPEndPoint ep;
    private readonly SimpleTcpClient client;
    public bool IsConnected
    {
        get
        {
            try
            {
                if (this.client != null && this.client.TcpClient.Client != null && this.client.TcpClient.Client.Connected)
                {
                    if (this.client.TcpClient.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        if (this.client.TcpClient.Client.Receive(buff, SocketFlags.Peek) == 0)
                            return false;
                        else
                            return true;
                    }
                    return true;
                }
                else
                    return false;
            }
            catch
            { return false; }
        }
    }

    public TCPClient(IPEndPoint ep)
    {
        this.ep = ep;
        this.client = new();
        this.client.DataReceived += Events_DataReceived;
    }

    public TcpClient? Connect()
    {
        if (this.IsConnected)
            return null;
        return (this.client.Connect(ep.Address.ToString(), ep.Port).TcpClient ??
            throw new SocketException());
    }

    public void Disconnect()
    {
        if (this.IsConnected)
            this.client.Disconnect();
    }

    private void Events_DataReceived(object? sender, SimpleTCP.Message e)
    {
        byte[] receivedData = e.Data.ToArray();
        Models.Application.Message receivedMessage = JsonSerializer.Deserialize<Models.Application.Message>(receivedData);
        this.MessageReceived?.Invoke(receivedMessage);
    }

    public void SendMessage(Models.Application.Message message)
    {
        try
        {
            if (!this.IsConnected)
                throw new Exception();
            byte[] data = JsonSerializer.SerializeToUtf8Bytes(message);
            this.client.Write(data);
        }
        catch (Exception ex)
        { throw ex; }
    }
}

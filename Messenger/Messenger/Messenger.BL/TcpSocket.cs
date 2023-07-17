using Messenger.Models.Application;
using System.Net.Sockets;
using System.Text.Json;

namespace Messenger.BL;

public class TcpSocket
{
	private TcpClient client;
	private NetworkStream stream;
	private byte[] buffer;
	private bool isConnected;

	public event Action<Message>? MessageReceived;

    public void Connect(string ipAddress, int port)
    {
		try
		{
			this.client = new();
			this.client.Connect(ipAddress, port);
			this.stream = client.GetStream();
			this.buffer = new byte[1024];
			this.isConnected = true;
		}
		catch (Exception ex)
		{
			throw new Exception($"Error connecting to server: {ex.Message}");
		}
    }

	public void SendMessage(Message message)
	{
		if (!this.isConnected)
			return;
		try
		{
			byte[] data = JsonSerializer.SerializeToUtf8Bytes(message);
			this.stream.Write(data, 0, data.Length);
		}
		catch (Exception ex)
        {
            throw new Exception($"Error sending message: {ex.Message}");
        }
    }

	public void ReceiveMessage(Message message)
	{
		try
		{
			while (this.isConnected)
			{
				int bytesRead = this.stream.Read(this.buffer, 0, this.buffer.Length);
				if (bytesRead > 0)
				{
					Message receivedMessage = JsonSerializer.Deserialize<Message>(this.buffer);
					this.MessageReceived?.Invoke(receivedMessage);
				}
			}
		}
        catch (Exception ex)
        {
            throw new Exception($"Error receiving message: {ex.Message}");
        }
    }

	public void Disconnect()
	{
		if (!this.isConnected)
			return;
		try
		{
			this.client?.Close();
			this.stream?.Close();
			this.isConnected = false;
		}
        catch (Exception ex)
        {
            throw new Exception("Error disconnecting from the server: " + ex.Message);
        }
    }
}

using Godot;
using Mirage;
using Mirage.Logging;
using Mirage.SocketLayer;
using Mirage.Sockets.Udp;
using System;

public partial class NetworkRunner : Node
{
    public MirageServer Server { get; private set; }
    public MirageClient Client { get; private set; }

    private MiragePeer _activePeer;

    public NetworkRunner()
    {
        LogFactory.ReplaceLogHandler(new GodotLogger(), true);
        LogFactory.SetDefaultLogLevel(LogType.Log);
        LogFactory.GetLogger<Peer>().filterLogType = LogType.Warning;

        Server = new MirageServer();
        Client = new MirageClient();
    }

    public MirageServer StartUDPServer(int port)
    {
        if (_activePeer != null)
            throw new InvalidOperationException("Already running");

        GD.Print("StartUDPServer");
        var socketFactory = new UdpSocketFactory();
        var socket = socketFactory.CreateSocket();
        var endpoint = socketFactory.GetBindEndPoint(port);
        Server.StartServer(socket, socketFactory.MaxPacketSize, endpoint);

        _activePeer = Server;
        return Server;
    }
    public MirageClient StartUDPClient(string address, int port)
    {
        if (_activePeer != null)
            throw new InvalidOperationException("Already running");

        GD.Print("StartUDPClient");

        var socketFactory = new UdpSocketFactory();
        var socket = socketFactory.CreateSocket();
        var endpoint = socketFactory.GetConnectEndPoint(address, checked((ushort)port));
        Client.Connect(socket, socketFactory.MaxPacketSize, endpoint);

        _activePeer = Client;
        return Client;
    }

    public void Stop()
    {
        if (_activePeer is MirageServer server)
            server.Stop();
        else if (_activePeer is MirageClient client)
            client.Disconnect();

        _activePeer = null;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (_activePeer == null)
            return;

        _activePeer.UpdateReceive();
        _activePeer.UpdateSent();
    }
}

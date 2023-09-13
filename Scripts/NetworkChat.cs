using Godot;
using Mirage;

public partial class NetworkChat : Node
{
    [Export] private NetworkRunner networkRunner;

    [Export] private TextEdit name;
    [Export] private Button setName;

    [Export] private TextEdit input;
    [Export] private Button sendMessage;

    [Export] private TextEdit history;
    private string playerName;

    public override void _Ready()
    {
        networkRunner.Server.Started.AddListener(StartServer);
        networkRunner.Client.Started.AddListener(StartClient);

        setName.Pressed += SetName_Pressed;
        sendMessage.Pressed += SendMessage_Pressed;

        sendMessage.Disabled = !networkRunner.Client.IsConnected;
    }

    private void SetName_Pressed()
    {
        playerName = name.Text;
        if (networkRunner.Client.Active)
        {
            networkRunner.Client.Player.Send(new SetName
            {
                Name = playerName,
            });
        }
    }

    private void SendMessage_Pressed()
    {
        if (!networkRunner.Client.Active)
            return;

        var message = input.Text;
        if (string.IsNullOrEmpty(message))
            return;

        input.Text = "";
        networkRunner.Client.Player.Send(new ChatMessage
        {
            Message = message
        });
    }

    private void StartServer()
    {
        AddChild(new ServerChat(networkRunner.Server));
    }

    private void StartClient()
    {
        AddChild(new ClientChat(networkRunner.Client, history));
    }

    public override void _Process(double delta)
    {
        sendMessage.Disabled = string.IsNullOrEmpty(playerName);
    }
}

[NetworkMessage]
public struct SetName
{
    public string Name;
}
[NetworkMessage]
public struct ChatMessage
{
    public string User;
    public string Message;
}
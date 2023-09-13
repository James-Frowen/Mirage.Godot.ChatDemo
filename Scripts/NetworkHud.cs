using Godot;

public partial class NetworkHud : Node
{
    [Export] private NetworkRunner networkRunner;
    [Export] private string Address = "127.0.0.1";
    [Export] private int Port = 7777;

    private Button serverButton;
    private Button clientButton;
    private Button stopButton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        serverButton = new Button();
        serverButton.Text = "Start Server";
        serverButton.Pressed += StartServerPressed;
        AddChild(serverButton);

        clientButton = new Button();
        clientButton.Text = "Start Client";
        clientButton.Pressed += StartClientPressed;
        AddChild(clientButton);

        stopButton = new Button();
        stopButton.Text = "Stop";
        stopButton.Pressed += StopPressed;
        AddChild(stopButton);

        var pos = new Vector2(20, 20);
        VerticalLayout(serverButton, ref pos);
        VerticalLayout(clientButton, ref pos);
        VerticalLayout(stopButton, ref pos);

        ToggleButtons(false);
    }

    private void VerticalLayout(Button button, ref Vector2 pos, Vector2? sizeNullable = null, int padding = 10)
    {
        var size = sizeNullable ?? new Vector2(200, 40);
        button.Position = pos;
        button.Size = size;
        pos.Y += size.Y + padding;
    }

    private void StartServerPressed()
    {
        networkRunner.StartUDPServer(Port);
        ToggleButtons(true);
    }

    private void StartClientPressed()
    {
        networkRunner.StartUDPClient(Address, Port);
        ToggleButtons(true);
    }

    private void StopPressed()
    {
        networkRunner.Stop();
        ToggleButtons(false);
    }

    private void ToggleButtons(bool active)
    {
        serverButton.Disabled = active;
        clientButton.Disabled = active;
        stopButton.Disabled = !active;
    }
}

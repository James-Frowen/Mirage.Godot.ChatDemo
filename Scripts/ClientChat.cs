using Godot;
using Mirage;

public partial class ClientChat : Node
{
    private readonly MirageClient Client;
    private readonly TextEdit history;
    private readonly string[] lines = new string[20];

    public ClientChat(MirageClient client, TextEdit history)
    {
        Client = client;
        this.history = history;
        Client.MessageHandler.RegisterHandler<ChatMessage>(ClientChatMessage);
    }

    private void ClientChatMessage(INetworkPlayer player, ChatMessage message)
    {
        var newMessage = $"{message.User}: {message.Message}";
        ShiftLines(lines, newMessage);
        history.Text = string.Join("\n", lines);
        history.ScrollVertical = 500;
    }

    public static void ShiftLines<T>(T[] lines, T newItem)
    {
        // shift old Message
        for (var i = 1; i < lines.Length; i++)
        {
            lines[i - 1] = lines[i];
        }
        // set new Message
        lines[^1] = newItem;
    }
}

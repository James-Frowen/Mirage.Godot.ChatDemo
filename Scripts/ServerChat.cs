using Godot;
using Mirage;
using System.Collections.Generic;

public partial class ServerChat : Node
{
    public readonly MirageServer Server;
    private readonly Dictionary<INetworkPlayer, string> playerNames = new Dictionary<INetworkPlayer, string>();
    private readonly ChatMessage[] lines = new ChatMessage[20];

    public ServerChat(MirageServer server)
    {
        Server = server;
        Server.MessageHandler.RegisterHandler<ChatMessage>(ServerChatMessage);
        Server.Connected += Server_Connected;
        Server.MessageHandler.RegisterHandler<SetName>(ServerSetName);
    }

    private void Server_Connected(INetworkPlayer player)
    {
        for (var i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrEmpty(lines[i].Message))
                continue;

            player.Send(lines[i]);
        }
    }

    private void ServerSetName(INetworkPlayer player, SetName message)
    {
        playerNames[player] = message.Name;
    }

    private void ServerChatMessage(INetworkPlayer player, ChatMessage message)
    {
        if (!playerNames.TryGetValue(player, out var name))
        {
            name = "unknown";
        }

        message.User = name;
        ClientChat.ShiftLines(lines, message);
        Server.SendToAll(message);
    }
}

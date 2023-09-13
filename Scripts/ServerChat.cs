using Godot;
using Mirage;
using System.Collections.Generic;

public partial class ServerChat : Node
{
    public readonly MirageServer Server;
    private readonly Dictionary<INetworkPlayer, string> playerNames = new Dictionary<INetworkPlayer, string>();

    public ServerChat(MirageServer server)
    {
        Server = server;
        Server.MessageHandler.RegisterHandler<ChatMessage>(ServerChatMessage);
        Server.MessageHandler.RegisterHandler<SetName>(ServerSetName);
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
        Server.SendToAll(message);
    }
}

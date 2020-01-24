
public enum SocketEmitEvents
{
    addUser,
    gameRequest,
    colorRequest,
    throwDart,
    leave,
}

public enum SocketListenEvents
{
    userConnected,
    colorGet,
    userJoin,
    gameStart,
    nextTurn,
    gameThrow,
    gameOver
}

public enum SocketEmitEvents
{
    addUser,
    gameRequest,
    colorRequest,
    throwDart,
    leave,
    gameRequestCancel,
}

public enum SocketListenEvents
{
    userConnected,
    colorGet,
    userJoin,
    gameStart,
    nextTurn,
    gameThrow,
    gameOver,
    noUser,
}

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
    userJoined,
    gameStart,
    nextTurn,
    gameThrow,
    gameOver,
    noUser,
    connectedRoom,
    gameTimer,
    dartTimer,
}
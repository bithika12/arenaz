
public enum SocketEmitEvents
{
    addUser,
    gameRequest,
    colorRequest,
    throwDart,
    leave,
    gameRequestCancel,
    reJoin,
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
    rejoinSuccess,
    rejoinFailure,
    temporaryDisconnect,
    opponentReconnect,
}
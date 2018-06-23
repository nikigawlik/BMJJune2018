"use strict"
var self = global;

self.gameState = {
    players: {},
};

self.keyPress = async function() {
    process.stdin.setRawMode(true);
    return new Promise(x => process.stdin.once('data', function() {
        process.stdin.setRawMode(false);
        x();
    }));
};

self.gameLoop = async function() {
    console.log("Starting game loop. Press any key to quit.");
    await keyPress();
    console.log("Game done.");
};

self.processLogin = function(obj) {
    if (getPlayer(obj.id))
        return null;
    console.log("Logging in: " + obj.name);
    var player = self.addPlayer(obj.name);
    return {
        message: 'Welcome!',
        player: player
    };
};

self.processLogout = function(id) {
    var player = getPlayer(id);
    if (player == null)
        return null;
    console.log("Logging out: " + player.name);
    if (!self.deletePlayer(id))
        return null;
    return { message: 'See ya!' };
}

self.addPlayer = function(name) {
    var player = new Player(name);
    gameState.players[player.id] = player;
    return player;
};

self.getPlayer = function(id) {
    if (!(id in gameState.players))
        return null;
    return gameState.players[id];
};

self.deletePlayer = function(id) {
    var player = getPlayer(id);
    if (!(id in gameState.players))
        return false;
    delete gameState.players[id];
    return true;
}

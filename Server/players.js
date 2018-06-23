"use strict"
var self = global;

var lastPlayerId = 0;
self.Player = function(name) {
    this.id        = ++lastPlayerId;
    this.name      = name;
    this.photo     = null;
    this.score     = 0;
    this.timestamp = new Date();
};

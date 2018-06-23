"use strict"

var packageFs         = require('fs');
var packageExpress    = require('express');
var packageMulter     = require('multer');
var packageBodyParser = require('body-parser');

var fs         = packageFs;
var express    = new packageExpress();
var multer     = packageMulter;
var bodyParser = packageBodyParser;

express.use(bodyParser.json());

require('./tools');
require('./game');
require('./players');

async function main() {
    var fileUpload = function(prefix) {
        return multer({
            storage: packageMulter.diskStorage({
                destination: function(req, file, callback) {
                    callback(null, './' + prefix);
                },
                filename: function(req, file, callback) {
                    var now = new Date();
                    req.filename = "" +
                        padZeros(now.getFullYear(), 4)     +
                        padZeros((now.getMonth() + 1), 2)  +
                        padZeros(now.getDate(), 2)         + '_' +
                        padZeros(now.getHours(), 2)        +
                        padZeros(now.getMinutes(), 2)      +
                        padZeros(now.getSeconds(), 2)      +
                        padZeros(now.getMilliseconds(), 4) + '.png';
                    callback(null, req.filename);
                }
            })
        }).single('file');
    };

    express.use(function(req, res, next) {
        res.header("Access-Control-Allow-Origin", "*");
        res.header("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept");
        next();
    });

    express.post('/upload-avatar', fileUpload('avatars'), function(req, res, next) {
        if (req.filename == null)
            return res.sendStatus(500);
        var player = getPlayer(req.body.id);
        if (player == null || player.photo != null) {
            res.setHeader('Content-Type', 'application/json');
            return res.send(JSON.stringify(null));
        }
        console.log("Got an avatar! " + req.filename);

        player.photo = 'avatars/' + req.filename;
        res.setHeader('Content-Type', 'application/json');
        res.send({ filename: player.photo });
    });

    express.post('/upload-photo', fileUpload('photos'), function(req, res, next) {
        if (req.filename == null)
            return res.sendStatus(500);
        var player = getPlayer(req.body.id);
        if (player == null) {
            res.setHeader('Content-Type', 'application/json');
            return res.send(JSON.stringify(null));
        }
        console.log("Got a photo! " + req.filename);

        res.setHeader('Content-Type', 'application/json');
        res.send({ filename: 'photos/' + req.filename });
    });

    express.post('/login', function(req, res) {
        var rval = processLogin(req.body);
        res.setHeader('Content-Type', 'application/json');
        res.send(JSON.stringify(rval));
    });

    express.post('/logout', function(req, res) {
        var rval = processLogout(req.body.id);
        res.setHeader('Content-Type', 'application/json');
        res.send(JSON.stringify(rval));
    });

    express.get('/players', function(req, res) {
        var players = [];
        for (var key in gameState.players)
            players.push(gameState.players[key]);
        res.setHeader('Content-Type', 'application/json');
        res.send(JSON.stringify(players));
    });

    express.get('/', function(req, res) {
        res.redirect('/play/index.html');
    });
    express.get('/avatars/:name', function(req, res) {
        var options = { root: __dirname + '/avatars' };
        res.sendFile(req.params.name, options);
    });
    express.get('/photos/:name', function(req, res) {
        var options = { root: __dirname + '/photos' };
        res.sendFile(req.params.name, options);
    });
    express.get('/play/:name', function(req, res) {
        var options = { root: __dirname + '/play' };
        res.sendFile(req.params.name, options);
    });

    var server = await express.listen(3000, function() {
        console.log('Server is running on port 3000');
    });

    await gameLoop();
    process.exit(0);
};

return main();

"use strict"

function gameVm() {
    var self = this;
    self.serverAddress = ko.observable('192.168.3.186');
    self.playerName    = ko.observable('Bob');
    self.playerId      = ko.observable(0);
    self.ajaxResponse  = ko.observable('');

    var url = function(path) {
        var rval = 'http://' + self.serverAddress() + ':3000/' + ko.unwrap(path);
        return rval;
    };

    var ajaxSuccess = function(result) {
        self.ajaxResponse(JSON.stringify(result, null, 4));
        return (result != null);
    };
    var ajaxError = function(jqXHR, text, errorThrown) {
        self.ajaxResponse("Error (code " + jqXHR.status + "): " + text + "\n" + errorThrown);
    };

    self.buttonLogin = function() {
        var dataObj = {
            id:   self.playerId(),
            name: self.playerName()
        };
        $.ajax({
            url:  url('login'),
            type: 'POST',
            data: JSON.stringify(dataObj),
            contentType: 'application/json',
            crossDomain: true,
            success: function(result) {
                if (!ajaxSuccess(result))
                    return;
                self.playerId(result.player.id);
            },
            error: ajaxError
        });
    };

    self.buttonLogout = function() {
        var dataObj = {
            id:   self.playerId(),
        };
        $.ajax({
            url:  url('logout'),
            type: 'POST',
            data: JSON.stringify(dataObj),
            contentType: 'application/json',
            crossDomain: true,
            success: function(result) {
                if (!ajaxSuccess(result))
                    return;
                self.playerId(0);
            },
            error: ajaxError
        });
    };

    self.uploadImage = function(type) {
        var input = document.getElementById('inputImage');
        var formData = new FormData();
        formData.append('id', self.playerId());
        formData.append('file', input.files[0]);

        $.ajax({
            url: url('upload-' + type),
            type: 'POST',
            data: formData,
            contentType: false,
            processData: false,
            crossDomain: true,
            success: function(result) {
                if (!ajaxSuccess(result))
                    return;
            },
            error: ajaxError
        });
    };

    self.buttonAvatar = function() { self.uploadImage('avatar'); }
    self.buttonPhoto  = function() { self.uploadImage('photo'); }

    self.getFunction = function(path, callback) {
        $.ajax({
            url: url(path),
            type: 'GET',
            crossDomain: true,
            success: function(result) {
                if (!ajaxSuccess(result))
                    return;
                if (callback != null)
                    callback(result);
            },
            error: ajaxError
        });
    };

    self.buttonPlayers = function() { self.getFunction('players'); }
};

window.onload = function() {
    var vm = new gameVm();
    ko.applyBindings(vm);
};

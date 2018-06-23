"use strict"

var packageFs      = require('fs');
var packageExpress = require('express');
var packageMulter  = require('multer');

var fs      = packageFs;
var express = new packageExpress();
var multer  = packageMulter;

function padZeros(n, width, z) {
    z = z || '0';
    n = n + '';
    return n.length >= width ? n : new Array(width - n.length + 1).join(z) + n;
}

async function main() {
    var fileUpload = function(prefix) {
        return multer({
            storage: packageMulter.diskStorage({
                destination: function(req, file, callback) {
                    callback(null, './' + prefix);
                },
                filename: function(req, file, callback) {
                    var now = new Date();
                    var filename = "" +
                        padZeros(now.getFullYear(), 4)     +
                        padZeros((now.getMonth() + 1), 2)  +
                        padZeros(now.getDate(), 2)         + '_' +
                        padZeros(now.getHours(), 2)        +
                        padZeros(now.getMinutes(), 2)      +
                        padZeros(now.getSeconds(), 2)      +
                        padZeros(now.getMilliseconds(), 4) + '.png';
                    callback(null, filename);
                }
            })
        }).single('file');
    };

    express.post('/upload-photo', fileUpload('photos'), function(req, res, next) {
        console.log("Posted a photo!");
        res.sendStatus(200);
    });

    express.listen(3000, function() {
        console.log('partSHy is running on port 3000');
    });
};

return main();

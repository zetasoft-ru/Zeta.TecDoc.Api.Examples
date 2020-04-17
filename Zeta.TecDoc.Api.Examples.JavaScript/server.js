'use strict';
var port = process.env.PORT || 1337;

var express = require('express');
var app = express();
var path = require('path');

app.use(express.static(path.join(__dirname, 'public')));

app.listen(port);

'use strict';

const http = require('http');
const socket = require('socket.io');
const server = http.createServer();
const port = 11250;

var io = socket(server, {
    pingInterval: 10000,
    pingTimeout: 5000
});

io.use((socket, next) => {
    if (socket.handshake.query.token === "UNITY") {
        next();
    } else {
        next(new Error("Authentication error"));
    }
});

let player = 0;
io.on('connection', socket => {
  console.log('connected.');

  socket.on('join', (data) => {
    player += 1;
    console.log('A Player Entered. count: ', player);
    socket.emit('enter', player);
  });

  socket.on('match', (data) => {
    player += 1;
    console.log('Matched.');
    socket.emit('match', player);
  });

  socket.on('quit', (data) => {
    if (player == 2) {
      socket.emit('quitgame');
    }
    player -= 1;
    console.log('A Player Quit. count: ', player);
    socket.emit('quit');
  });

  socket.on('gameLoaded', (data) => {
    console.log('Game Start.');
  });
});


server.listen(port, () => {
  console.log('listening on *:' + port);
});
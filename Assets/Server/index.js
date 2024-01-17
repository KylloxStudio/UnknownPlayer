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

let players = [];
let matchedPlayers = [];
io.on('connection', socket => {
	const ip = socket.request.connection.remoteAddress.replace("::ffff:", "");
  const id = socket.id;
  console.log('Connected.', ip, id);

  socket.on('join', (data) => {
    if (players.indexOf(id) == -1) players.push(id);
    console.log('A Player Entered. ID:', id);
    socket.emit('join', id);
  });

  socket.on('waiting', (data) => {
    console.log('Waiting for Other Player...', data);
    socket.interval = setInterval(() => {
      socket.emit('waiting', players);
    }, 1000);
  });

  socket.on('match', (data) => {
    clearInterval(socket.interval);
    socket.emit('match');
    matchedPlayers.push(data);
    if (matchedPlayers.length == 2)
      players.splice(data);
    console.log(players);
    console.log(matchedPlayers);
  });

  socket.on('quit', (data) => {
    players.splice(players.findIndex(e => e == id), 1);
    console.log('A Player Quit. ID:', id);
    socket.emit('quit', id);
  });

  socket.on('gameLoad', (data) => {
    console.log('Game Start!', data);
    socket.emit("gameLoad");
  });

  socket.on('statistics', (data) => {
    socket.broadcast.emit("statistics", data);
  });

  socket.on('position', (data) => {
    socket.broadcast.emit("position", data);
  });

  socket.on('rotation', (data) => {
    socket.broadcast.emit("rotation", data);
  });

  socket.on('animation', (data) => {
    socket.broadcast.emit("animation", data);
  });

  socket.on('disconnect', () => {
    players.splice(players.findIndex(e => e == id), 1);
    matchedPlayers.splice(matchedPlayers.findIndex(e => e == id), 1);
    console.log('Disconnected:', ip, id);
  });

  socket.on('error', (error) => {
    console.error(error);
  });
});


server.listen(port, () => {
  console.log('listening on *:' + port);
});
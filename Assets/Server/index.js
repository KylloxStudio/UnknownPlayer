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

io.on('connection', socket => {
	const ip = socket.request.connection.remoteAddress.replace("::ffff:", "");
  const id = socket.id;
  console.log('Connected.');

  socket.on('join', (data) => {
    console.log('A Player Entered. ID:', id);
    socket.emit('join', id);
  });

  socket.on('waiting', (data) => {
    console.log('Waiting for Other Player...');
    socket.emit('waiting');
  });

  socket.on('match', (data) => {
    console.log('Matched.');
    socket.emit('match', player);
  });

  socket.on('quit', (data) => {
    console.log('A Player Quit. ID:', id);
    socket.emit('quit', id);
  });

  socket.on('gameLoad', (data) => {
    console.log('Game Start!');
  });

  socket.on('disconnect', () => {
    console.log('Disconnected:', ip, id);
  });

  socket.on('error', (error) => {
    console.error(error);
  });
});


server.listen(port, () => {
  console.log('listening on *:' + port);
});
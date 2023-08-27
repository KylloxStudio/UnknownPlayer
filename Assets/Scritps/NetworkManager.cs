using System;
using System.Collections.Generic;
using SocketIOClient;
using SocketIOClient.Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;
    public SocketIOUnity socket;

    [SerializeField]
    private int playerCount;

    [SerializeField]
    private Player1 player1;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        var uri = new Uri("http://localhost:11250");
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
            {
                {"token", "UNITY" }
            },
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        socket.OnConnected += OnConnected;
        socket.OnPing += OnPing;
        socket.OnPong += OnPong;
        socket.OnDisconnected += OnDisconnected;
        socket.OnReconnectAttempt += OnReconnect;

        if (!socket.Connected)
        {
            Debug.Log("Connecting...");
            socket.Connect();
        }

        socket.On("join", OnJoin);
        socket.On("match", OnMatch);
        socket.On("quit", OnQuit);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnApplicationQuit()
    {
        if (playerCount >= 1)
            socket.Emit("quit");

        socket.Disconnect();
    }

    public void OnConnected(object sender, EventArgs e)
    {
        Debug.Log("Connected.");
    }

    public void OnPing(object sender, EventArgs e)
    {
        Debug.Log("Ping");
    }

    public void OnPong(object sender, TimeSpan e)
    {
        Debug.Log("Pong: " + e.TotalMilliseconds);
    }

    public void OnDisconnected(object sender, string e)
    {
        Debug.Log("Disconnected: " + e);
    }

    public void OnReconnect(object sender, int e)
    {
        Debug.Log($"{DateTime.Now} Reconnecting: Attempts = {e}");
    }

    public void OnJoin(SocketIOResponse res)
    {
        playerCount = res.GetValue<int>();
    }

    public void OnMatch(SocketIOResponse res)
    {
        playerCount = res.GetValue<int>();

    }

    public void OnQuit(SocketIOResponse res)
    {
        playerCount = res.GetValue<int>();
    }

    public void EmitGameLoaded()
    {
        socket.Emit("gameLoaded");
    }
}

using SocketIOClient;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;
    public SocketIOUnity socket;

    [SerializeField]
    private Player1 player1;

    [SerializeField]
    private Player2 player2;

    [SerializeField]
    private List<string> playerList;

    public bool isReconneting;

    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        var uri = new Uri("http://192.168.55.247:11250");
        socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Query = new Dictionary<string, string>
            {
                { "token", "UNITY" }
            },
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });
    }

    private void Start()
    {
        isReconneting = false;
        playerList = new List<string>();

        socket.OnConnected += OnConnected;
        socket.OnPing += OnPing;
        socket.OnPong += OnPong;
        socket.OnDisconnected += OnDisconnected;
        socket.OnReconnectAttempt += OnReconnect;

        socket.On("join", OnJoin);
        socket.On("match", OnMatch);
        socket.On("quit", OnQuit);
        socket.On("opponent", GetOpponentData);
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void OnApplicationQuit()
    {
        if (socket.Connected)
            socket.Disconnect();
    }

    public void OnConnected(object sender, EventArgs e)
    {
        if (isReconneting)
            isReconneting = false;

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
        if (isReconneting)
            isReconneting = false;

        Debug.Log("Disconnected: " + e);
    }

    public void OnReconnect(object sender, int e)
    {
        isReconneting = true;
        Debug.Log($"{DateTime.Now} Reconnecting: Attempts = {e}");
    }

    public void OnJoin(SocketIOResponse res)
    {
        string id = res.GetValue<string>();
        if (playerList.Contains(id))
            return;

        playerList.Add(id);
    }

    public void EmitWaiting()
    {
        socket.Emit("waiting");
    }

    public void OnWaiting(SocketIOResponse res)
    {
        if (playerList.Count >= 2)
        {
            socket.Emit("match");
        }
    }

    public void OnMatch(SocketIOResponse res)
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnQuit(SocketIOResponse res)
    {
        playerList.Remove(res.GetValue<string>());
    }

    public void GetOpponentData(SocketIOResponse res)
    {
        UnityThread.executeInUpdate(() =>
        {
        });
    }

    public void EmitGameLoad()
    {
        socket.Emit("gameLoad");
    }
}
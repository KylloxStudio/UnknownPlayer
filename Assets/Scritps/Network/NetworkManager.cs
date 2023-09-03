using SocketIOClient;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;
    public SocketIOUnity socket;

    [SerializeField]
    private string playerId;

    public bool isPlayer1;
    public bool isPlayer2;

    public bool isReconneting;
    public bool isMatched;
    public bool isGameLoaded;

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

        var uri = new Uri("http://34.64.112.23:11250");
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

        socket.OnConnected += OnConnected;
        socket.OnPing += OnPing;
        socket.OnPong += OnPong;
        socket.OnDisconnected += OnDisconnected;
        socket.OnReconnectAttempt += OnReconnect;

        socket.On("join", OnJoin);
        socket.On("waiting", OnWaiting);
        socket.On("match", OnMatch);
        socket.On("gameLoad", OnGameLoad);
        socket.On("quit", OnQuit);
        socket.On("opponent", GetOpponentData);
    }

    // Update is called once per frame
    private void Update()
    {
        if (isGameLoaded)
        {
            JObject json = new JObject();
            json.Add("is1StepJumping", PlayerController.Instance.is1StepJumping);
            json.Add("is2StepJumping", PlayerController.Instance.is2StepJumping);
            json.Add("isAttacking", PlayerController.Instance.isAttacking);
            socket.Emit("opponent", json);
        }
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
        //Debug.Log("Ping");
    }

    public void OnPong(object sender, TimeSpan e)
    {
        //Debug.Log("Pong: " + e.TotalMilliseconds);
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
        playerId = res.GetValue<string>();
    }

    public IEnumerator EmitWaiting()
    {
        yield return new WaitUntil(() => playerId != null);
        socket.Emit("waiting", playerId);
    }

    public void OnWaiting(SocketIOResponse res)
    {
        string[] ids = res.GetValue<string[]>();
        Debug.Log(ids.Length);

        if (ids.Length < 2)
            return;

        if (ids[0] == playerId)
            isPlayer1 = true;
        else if (ids[1] == playerId)
            isPlayer2 = true;

        socket.Emit("match");
    }

    public void OnMatch(SocketIOResponse res)
    {
        isMatched = true;
    }

    public void OnQuit(SocketIOResponse res)
    {
        playerId = null;
    }

    public void EmitGameLoad()
    {
        socket.Emit("gameLoad", playerId);
    }

    public void OnGameLoad(SocketIOResponse res)
    {
        isGameLoaded = true;
    }

    public void GetOpponentData(SocketIOResponse res)
    {

    }
}
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

    public string playerId;

    public bool isPlayer1;
    public bool isPlayer2;

    public bool isReconneting;
    public bool isMatched;
    public bool isGameLoaded;

    public int player1Hp;
    public int player2Hp;
    public int player1Stamina;
    public int player2Stamina;

    public Vector3 player1Position;
    public Vector3 player2Position;

    public Quaternion player1Rotation;
    public Quaternion player2Rotation;

    public Dictionary<string, bool> player1Animations;
    public Dictionary<string, bool> player2Animations;

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

        player1Animations = new Dictionary<string, bool>();
        player2Animations = new Dictionary<string, bool>();

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
        socket.On("statistics", OnStatistics);
        socket.On("position", OnPosition);
        socket.On("rotation", OnRotation);
        socket.On("animation", OnAnimation);
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

    public void OnStatistics(SocketIOResponse res)
    {
        JObject data = JObject.Parse(res.GetValue<string>());
        int hp1 = data.GetValue("hp1").Value<int>();
        int stamina1 = data.GetValue("stamina1").Value<int>();
        int hp2 = data.GetValue("hp2").Value<int>();
        int stamina2 = data.GetValue("stamina2").Value<int>();

        player1Hp = hp1;
        player1Stamina = stamina1;
        player2Hp = hp2;
        player2Stamina = stamina2;
    }

    public void OnPosition(SocketIOResponse res)
    {
        JObject data = JObject.Parse(res.GetValue<string>());
        float x1 = data.GetValue("x1").Value<float>();
        float y1 = data.GetValue("y1").Value<float>();
        float x2 = data.GetValue("x2").Value<float>();
        float y2 = data.GetValue("y2").Value<float>();

        player1Position = new Vector3(x1, y1);
        player2Position = new Vector3(x2, y2);
    }

    public void OnRotation(SocketIOResponse res)
    {
        JObject data = JObject.Parse(res.GetValue<string>());
        float y1 = data.GetValue("y1").Value<float>();
        float y2 = data.GetValue("y2").Value<float>();

        player1Rotation = Quaternion.Euler(0, y1, 0);
        player2Rotation = Quaternion.Euler(0, y2, 0);
    }

    public void OnAnimation(SocketIOResponse res)
    {
        JObject data = JObject.Parse(res.GetValue<string>());
        player1Animations.Add("isMoving", data.GetValue("p1_isMoving").Value<bool>());
        player1Animations.Add("isDashing", data.GetValue("p1_isDashing").Value<bool>());
        player1Animations.Add("isAttacking_01", data.GetValue("p1_isAttacking_01").Value<bool>());
        player1Animations.Add("isAttacking_02", data.GetValue("p1_isAttacking_02").Value<bool>());
        player1Animations.Add("isAttacking_03", data.GetValue("p1_isAttacking_03").Value<bool>());
        player1Animations.Add("isAttacking_04", data.GetValue("p1_isAttacking_04").Value<bool>());
        player1Animations.Add("isAttacking_05", data.GetValue("p1_isAttacking_05").Value<bool>());
        player1Animations.Add("isDamaged", data.GetValue("p1_isDamaged").Value<bool>());
        player1Animations.Add("isDead", data.GetValue("p1_isDead").Value<bool>());

        player2Animations.Add("isMoving", data.GetValue("p2_isMoving").Value<bool>());
        player2Animations.Add("isDashing", data.GetValue("p2_isDashing").Value<bool>());
        player2Animations.Add("isAttacking", data.GetValue("p2_isAttacking").Value<bool>());
        player2Animations.Add("isDamaged", data.GetValue("p2_isDamaged").Value<bool>());
        player2Animations.Add("isDead", data.GetValue("p2_isDead").Value<bool>());
    }
}
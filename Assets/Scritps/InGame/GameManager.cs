using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;
    public Grid grid;
    public Tilemap tilemap;
    public GameCamera gameCamera;

    public List<Transform> spawnPoints = new List<Transform>();

    void Awake()
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
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    //Photon Ŭ�������� ��ӹ޴� OnEnable�� OnDisable �Լ��� base �Լ��� ȣ���ؾ� �Ѵ�.
    // OnEnable(): (��ũ��Ʈ�� ������Ʈ��) Ȱ��ȭ �� ������ ȣ��Ǵ� �Լ�
    // OnDisable(): ��Ȱ��ȭ �� ������ ȣ��Ǵ� �Լ�
    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "InGameScene")
        {
            Transform[] points = GameObject.Find("SpawnPoints").GetComponentsInChildren<Transform>();
            for (int i = 1; i < points.Length; i++)
            {
                spawnPoints.Add(points[i]);
            }
            PhotonNetwork.Instantiate("Player" + Random.Range(1, 2).ToString(), spawnPoints[NetworkManager.Instance.PlayerID].position, Quaternion.identity);
        }
    }

    public static GameCamera GetCamera()
    {
        return Instance.gameCamera;
    }
}
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

    //Photon 클래스에서 상속받는 OnEnable과 OnDisable 함수는 base 함수를 호출해야 한다.
    // OnEnable(): (스크립트나 오브젝트가) 활성화 될 때마다 호출되는 함수
    // OnDisable(): 비활성화 될 때마다 호출되는 함수
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
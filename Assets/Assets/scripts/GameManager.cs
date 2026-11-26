using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }
    public static int CurrentLevel = 0;   //to wskakuje tylko na hosta
    private const int MaxLevel = 2;       // tu ile poziomów gierki mozna zmienić później
    
    

    [SerializeField] private NetworkObject playerPrefab;
    [SerializeField] private NetworkObject coinPrefab;
    [SerializeField] private Vector2 areaSize = new(16, 8);
    [SerializeField] private Vector2[] spawnPoints = { new(-4, 0), new(4, 0) };
    [SerializeField] private int minCoins = 8;
    [SerializeField] private int maxCoins = 14;
    [SerializeField] private float pickupRadius = 1f;   //to dostosować do wielko0ści spritea

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private Button restartBtn;
    [SerializeField] private Button nextLevelBtn;
    [SerializeField] private Button lobbyBtn;

    public Vector2 AreaSize => areaSize;
    public NetworkVariable<bool> IsPlaying = new();
    

    private readonly NetworkVariable<int> scoreHost = new();
    private readonly NetworkVariable<int> scoreGuest = new();
    private readonly NetworkVariable<int> coinsLeft = new();
    private readonly List<NetworkObject> coins = new();

    private void Awake()
    {
        Instance = this;
        resultPanel.SetActive(false);
        restartBtn.onClick.AddListener(() => LoadAsHost("game"));
        nextLevelBtn.onClick.AddListener(() => { if (IsServer) CurrentLevel++; LoadAsHost("game"); });
        lobbyBtn.onClick.AddListener(() => { if (IsServer) CurrentLevel = 0; LoadAsHost("lobby"); });
    }

    public override void OnNetworkSpawn()
    {
        scoreHost.OnValueChanged += (_, _) => UpdateScoreUi();
        scoreGuest.OnValueChanged += (_, _) => UpdateScoreUi();
        coinsLeft.OnValueChanged += (_, _) => UpdateScoreUi();
        UpdateScoreUi();

        if (IsServer)
            NetworkManager.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager != null)
            NetworkManager.SceneManager.OnLoadEventCompleted -= OnSceneLoaded;
    }

    // tu runda zaczyna się dopiero gdy wszyscy wczytali scene wazne
    private void OnSceneLoaded(string scene, LoadSceneMode mode, List<ulong> done, List<ulong> timedOut)
    {
        scoreHost.Value = 0;
        scoreGuest.Value = 0;
        coins.Clear();

        int i = 0;
        foreach (ulong id in NetworkManager.ConnectedClientsIds)
        {
            var p = Instantiate(playerPrefab, spawnPoints[i % spawnPoints.Length], Quaternion.identity);
            p.SpawnAsPlayerObject(id, true);
            i++;
        }

        int count = Random.Range(minCoins + CurrentLevel * 3, maxCoins + CurrentLevel * 3 + 1);
        for (int c = 0; c < count; c++)
        {
            var pos = new Vector2(Random.Range(-areaSize.x / 2, areaSize.x / 2),
                                  Random.Range(-areaSize.y / 2, areaSize.y / 2));
            var coin = Instantiate(coinPrefab, pos, Quaternion.identity);
            coin.Spawn(true);
            coins.Add(coin);
        }

        coinsLeft.Value = count;
        IsPlaying.Value = true;
    }

    // zbieranie pieniaczkow
    private void Update()
    {
        if (!IsServer || !IsPlaying.Value) return;

        foreach (var client in NetworkManager.ConnectedClients)
        {
            var playerObj = client.Value.PlayerObject;
            if (playerObj == null) continue;
            Vector2 playerPos = playerObj.transform.position;

            for (int i = coins.Count - 1; i >= 0; i--)
            {
                var coin = coins[i];
                if (coin == null || !coin.IsSpawned) { coins.RemoveAt(i); continue; }

                if (Vector2.Distance(playerPos, coin.transform.position) <= pickupRadius)
                {
                    coins.RemoveAt(i);
                    coin.Despawn();
                    CollectCoin(client.Key);
                    if (!IsPlaying.Value) return;   // zakonczenie rundy
                }
            }
        }
    }

    private void CollectCoin(ulong collectorId)
    {
        if (collectorId == NetworkManager.ServerClientId) scoreHost.Value++;
        else scoreGuest.Value++;

        coinsLeft.Value--;
        if (coinsLeft.Value <= 0)
        {
            IsPlaying.Value = false;
            ShowResultsClientRpc(scoreHost.Value, scoreGuest.Value, CurrentLevel < MaxLevel);
        }
    }

    [ClientRpc]
    private void ShowResultsClientRpc(int host, int guest, bool hasNext)
    {
        string verdict = host == guest ? "Remis!"
                       : ((host > guest) == IsHost ? "Wygrałeś!" : "Przegrałeś!");
        resultText.text = $"{verdict}\nHost: {host}   Gość: {guest}";
        resultPanel.SetActive(true);

        //tu klika sb tylko host player zwykly nei
        restartBtn.interactable = IsServer;
        lobbyBtn.interactable = IsServer;
        nextLevelBtn.gameObject.SetActive(hasNext);
        nextLevelBtn.interactable = IsServer;
    }

    private void UpdateScoreUi()
    {
        scoreText.text = $"Host: {scoreHost.Value}   Gość: {scoreGuest.Value}   Monety: {coinsLeft.Value}";
    }

    private void LoadAsHost(string sceneName)
    {
        if (IsServer)
            NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
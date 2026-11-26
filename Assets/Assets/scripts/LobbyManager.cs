using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text codeText;
    [SerializeField] private Button readyBtn;
    [SerializeField] private Button startBtn;

    private NetworkList<ulong> readyClients;
    private readonly NetworkVariable<int> playerCount = new();

    private void Awake()
    {
        readyClients = new NetworkList<ulong>();
        readyBtn.onClick.AddListener(() => ToggleReadyServerRpc());
        startBtn.onClick.AddListener(StartGame);
    }

    public override void OnNetworkSpawn()
    {
        readyClients.OnListChanged += _ => Refresh();
        playerCount.OnValueChanged += (_, _) => Refresh();

        if (IsServer)
        {
            playerCount.Value = NetworkManager.ConnectedClientsIds.Count;
            NetworkManager.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.OnClientDisconnectCallback += OnClientDisconnected;
        }

        startBtn.gameObject.SetActive(IsHost);
        codeText.text = IsHost ? "Code: " + TestRelay.LastJoinCode : "";
        Refresh();
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && NetworkManager != null)
        {
            NetworkManager.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void OnClientConnected(ulong id)
        => playerCount.Value = NetworkManager.ConnectedClientsIds.Count;

    private void OnClientDisconnected(ulong id)
    {
        if (readyClients.Contains(id)) readyClients.Remove(id);
        playerCount.Value = NetworkManager.ConnectedClientsIds.Count(c => c != id);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleReadyServerRpc(ServerRpcParams p = default)
    {
        ulong id = p.Receive.SenderClientId;
        if (readyClients.Contains(id)) readyClients.Remove(id);
        else readyClients.Add(id);
    }

    private void Refresh()
    {
        bool iAmReady = readyClients.Contains(NetworkManager.LocalClientId);
        statusText.text = $"Players: {playerCount.Value}/2   Ready: {readyClients.Count}/{playerCount.Value}";
        readyBtn.GetComponentInChildren<TMP_Text>().text = iAmReady ? "Not ready" : "Ready";
        startBtn.interactable = IsHost && playerCount.Value == 2 && readyClients.Count == 2;
    }

    private void StartGame()
    {
        if (!IsServer || playerCount.Value != 2 || readyClients.Count != 2) return;

     
        if (LobbyHelper.Instance != null) LobbyHelper.Instance.CloseLobby();

        GameManager.CurrentLevel = 0;
        NetworkManager.SceneManager.LoadScene("game", LoadSceneMode.Single);
    }
}
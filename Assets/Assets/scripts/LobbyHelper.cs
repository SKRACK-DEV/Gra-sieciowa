using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyHelper : MonoBehaviour
{
    public static LobbyHelper Instance { get; private set; }
    private Lobby hostedLobby;
    private float heartbeatTimer;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async void Update()
    {
        if (hostedLobby == null) return;
        heartbeatTimer -= Time.deltaTime;
        if (heartbeatTimer > 0f) return;
        heartbeatTimer = 15f;
        try { await LobbyService.Instance.SendHeartbeatPingAsync(hostedLobby.Id); }
        catch (LobbyServiceException e) { Debug.Log(e); }
    }

    public async Task CreateLobby(string joinCode)
    {
        try
        {
            var options = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    { "JoinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode) }
                }
            };
            hostedLobby = await LobbyService.Instance.CreateLobbyAsync("Gra " + joinCode, 2, options);
        }
        catch (LobbyServiceException e) { Debug.Log(e); }
    }

    public async Task<List<Lobby>> QueryLobbies()
    {
        try
        {
            var options = new QueryLobbiesOptions
            {
                Count = 20,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder> { new QueryOrder(false, QueryOrder.FieldOptions.Created) }
            };
            var response = await LobbyService.Instance.QueryLobbiesAsync(options);
            return response.Results;
        }
        catch (LobbyServiceException e) { Debug.Log(e); return new List<Lobby>(); }
    }

    public async Task<string> JoinAndGetCode(string lobbyId)
    {
        try
        {
            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            return lobby.Data["JoinCode"].Value;
        }
        catch (LobbyServiceException e) { Debug.Log(e); return null; }
    }

    public async void CloseLobby()
    {
        if (hostedLobby == null) return;
        string id = hostedLobby.Id;
        hostedLobby = null;
        try { await LobbyService.Instance.DeleteLobbyAsync(id); }
        catch (LobbyServiceException e) { Debug.Log(e); }
    }

    private void OnApplicationQuit() => CloseLobby();
}
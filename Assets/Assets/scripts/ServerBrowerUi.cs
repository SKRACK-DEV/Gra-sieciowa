using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

public class ServerBrowserUi : MonoBehaviour
{
    [SerializeField] private Transform listRoot;
    [SerializeField] private Button rowPrefab;
    [SerializeField] private Button refreshBtn;

    private async void Start()
    {
        refreshBtn.onClick.AddListener(Refresh);

       //tu czekamy sb na logowanie gracza przez test reley
       
       
        while (UnityServices.State != ServicesInitializationState.Initialized) await Task.Yield();
        while (!AuthenticationService.Instance.IsSignedIn) await Task.Yield();

        Refresh();
    }

    private async void Refresh()
    {
        foreach (Transform c in listRoot) Destroy(c.gameObject);

        var lobbies = await LobbyHelper.Instance.QueryLobbies();
        foreach (var lobby in lobbies)
        {
            var row = Instantiate(rowPrefab, listRoot);
            row.GetComponentInChildren<TMP_Text>().text = $"{lobby.Name}  ({lobby.Players.Count}/{lobby.MaxPlayers})";
            string id = lobby.Id;
            row.onClick.AddListener(async () => await Join(id));
        }
    }

    private async Task Join(string lobbyId)
    {
        string code = await LobbyHelper.Instance.JoinAndGetCode(lobbyId);
        if (code != null) TestRelay.Instance.JoinRelay(code);
    }
}
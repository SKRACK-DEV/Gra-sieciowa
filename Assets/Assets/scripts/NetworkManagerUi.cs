using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetworkManagerUi : MonoBehaviour
{
    
    [SerializeField] private Button HostBtn;
    [SerializeField] private Button ClientBtn;
    [SerializeField] private TMP_InputField joinCodeInput;
    [SerializeField] private TMP_Text joinCodeText;

    private void Awake()
    {

        HostBtn.onClick.AddListener(async () =>
        {
            string joinCode = await TestRelay.Instance.CreateRelay();

            if (joinCode != null)
            {
                joinCodeText.text = "Code: " + joinCode;
                GUIUtility.systemCopyBuffer = joinCode;
            }
        });

        ClientBtn.onClick.AddListener(() =>
        {
            TestRelay.Instance.JoinRelay(joinCodeInput.text);
        });
    }
}

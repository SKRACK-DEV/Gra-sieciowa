using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{

    [SerializeField] private Transform spawnCoinPrefab;

    public float moveSpeed = 5f;

    private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(
        new MyCustomData { 

            _int = 67,
            _bool = true,
        
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
            

    public struct  MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public FixedString128Bytes message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }

    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) => {

            Debug.Log(OwnerClientId + "; random number: " + newValue._int + "; " + newValue._bool + "; " + newValue.message);

        };
        
    }

    void Update()

    {
        

        if (!IsOwner) return;

        if (Input.GetKeyUp(KeyCode.T))
        {

            Transform CoinTransform = Instantiate(spawnCoinPrefab);
            CoinTransform.GetComponent<NetworkObject>().Spawn(true);

            //randomNumber.Value = new MyCustomData
            /* {
                _int = 10,
                _bool = false,
                message = "skibidi 67"

            };
            */
        }
        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) moveY = 1f;
        else if (Input.GetKey(KeyCode.S)) moveY = -1f;

        if (Input.GetKey(KeyCode.D)) moveX = 1f;
        else if (Input.GetKey(KeyCode.A)) moveX = -1f;

        Vector3 moveDirection = new Vector3(moveX, moveY, 0).normalized;
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);

        if (moveX > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveX < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}
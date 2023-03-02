using Unity.Netcode;
using UnityEngine;
using Network.Movement;


namespace HelloWorld
{
    struct InputHolder : INetworkSerializable
    {
        public float vertical;
        public float horizontal;
        public bool action;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref vertical);
            serializer.SerializeValue(ref horizontal);
            serializer.SerializeValue(ref action);
        }
    }

    public class HelloWorldPlayer : NetworkBehaviour
    {
        [SerializeField] private float playerSpeed = 5;

        //new
        [SerializeField] private NetworkMovementComponent _playerMovement;

        public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                Move();
            }
        }

        public void Move()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                var randomPosition = GetRandomPositionOnPlane();
                transform.position = randomPosition;
                Position.Value = randomPosition;
            }
            else
            {
                SubmitPositionRequestServerRpc();
            }
        }

        [ServerRpc]
        void SubmitPositionRequestServerRpc(ServerRpcParams rpcParams = default)
        {
           
            Position.Value = GetRandomPositionOnPlane();
            
        }

        [ServerRpc]
        void KeyChangeServerRpc(InputHolder inputs, ServerRpcParams rpcParams = default)
        {
            /*
             switch(i_key)
             {
                 case "U":
                     Position.Value = new Vector3(0,2,0);
                     break;

                 case "D":
                     Position.Value = new Vector3(0,-2,0);
                     break;

                 case "L":
                     Position.Value = new Vector3(-2,0,0);
                     break;

                 case "R":
                     Position.Value = new Vector3(2,0,0);
                     break;
             }
             */
            Vector3 velocity = Vector3.right * inputs.horizontal + Vector3.up * inputs.vertical;
            Position.Value += velocity.normalized * Time.deltaTime * playerSpeed;

        }

        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        void Update()
        {
            InputHolder inputs = new InputHolder();
            transform.position = Position.Value;

            if(NetworkManager.Singleton.IsClient)
            {
                /*
                string i_key = "";
                if(Input.GetKey("up"))
                {
                    i_key = "U";
                }

                else if(Input.GetKey("down"))
                {
                    i_key = "D";
                }

                else if(Input.GetKey("left"))
                {
                    i_key = "L";
                }

                else if(Input.GetKey("right"))
                {
                    i_key = "R";
                }

                KeyChangeServerRpc(i_key);
                */

                inputs.horizontal = Input.GetAxis("Horizontal");
                inputs.vertical = Input.GetAxis("Vertical");
                inputs.action = Input.GetButton("Fire1");

                //new
                Vector2 pos = new Vector2(inputs.horizontal, inputs.vertical);

                _playerMovement.ProcessLocalPlayerMovement(pos, pos);

                //KeyChangeServerRpc(inputs);
            }
        }
    }
}
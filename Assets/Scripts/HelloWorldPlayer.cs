using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

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
        [SerializeField]
        private float playerSpeed = 5;

        private Rigidbody2D rigidbody2d;

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
            if (velocity.magnitude > 1)
            {
                velocity = velocity.normalized;
            }
            //Position.Value += velocity.normalized * Time.deltaTime * playerSpeed;
            rigidbody2d.AddForce(velocity * playerSpeed);
        }

        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        private void Start()
        {
            rigidbody2d = GetComponent<Rigidbody2D>();
            transform.position = Position.Value;
        }

        void Update()
        {
            InputHolder inputs = new InputHolder();
            //transform.position = rigidbody2d.transform.position;
            //transform.position = Position.Value;

            if (NetworkManager.Singleton.IsClient)
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

                KeyChangeServerRpc(inputs);
            }
        }
    }
}
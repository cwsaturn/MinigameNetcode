using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class HelloWorldPlayer : NetworkBehaviour
    {
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
        void KeyChangeServerRpc(string i_key, ServerRpcParams rpcParams = default)
        {
           
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
            
        }

        static Vector3 GetRandomPositionOnPlane()
        {
            return new Vector3(Random.Range(-3f, 3f), 1f, Random.Range(-3f, 3f));
        }

        void Update()
        {
            transform.position = Position.Value;

            if(NetworkManager.Singleton.IsClient)
            {
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
            }
        }
    }
}
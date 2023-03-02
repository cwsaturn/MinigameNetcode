using UnityEngine;
using Unity.VisualScripting;
using Unity.Netcode;
using System;

namespace Network.Movement
{
    public class NetworkMovementComponent : NetworkBehaviour
    {
        [SerializeField] CharacterController _cc;

        [SerializeField] private float _speed;
        [SerializeField] private float _turnSpeed;

        [SerializeField] private Transform _camSocket;
        [SerializeField] private Transform _vcam;

        private Transform _vcamTransform;

        private int _tick = 0;
        private float _tickDeltaTime = 0f;
        //128 tick
        private float _tickrate = 1f / 128f;

        private const int BUFFER_SIZE = 1024;
        private InputState[] _inputStates = new InputState[BUFFER_SIZE];
        private TransformState[] _transformStates = new TransformState[BUFFER_SIZE];

        public NetworkVariable<TransformState> ServerTransformState = new NetworkVariable<TransformState>();
        public TransformState _PreviousTransformState;

        private void OnEnable()
        {
            ServerTransformState.OnValueChanged += OnServerStateChanged;
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            _vcamTransform = _vcam.transform;
        }

        private void OnServerStateChanged(TransformState previousvalue, TransformState newvalue)
        {
            _PreviousTransformState = previousvalue;
        }

        public void ProcessLocalPlayerMovement(Vector2 movementInput, Vector2 lookInput)
        {
            _tickDeltaTime += Time.deltaTime;
            if(_tickDeltaTime > _tickrate)
            {
                int bufferIndex = _tick % BUFFER_SIZE;

                if(!IsServer)
                {
                    MovePlayerServerRpc(_tick, movementInput, lookInput);
                    MovePlayer(movementInput);
                    RotatePlayer(lookInput);
                } 
                else
                {
                    MovePlayer(movementInput);
                    RotatePlayer(lookInput);
                    
                    TransformState state = new TransformState()
                    {
                        Tick = _tick,
                        Position = transform.position,
                        Rotation = transform.rotation,
                        HasStartedMoving = true
                    };

                    _PreviousTransformState = ServerTransformState.Value;
                    ServerTransformState.Value = state;
                }

                InputState inputState = new InputState()
                {
                    Tick = _tick,
                    MovementInput = movementInput,
                    LookInput = lookInput
                };

                TransformState transformState = new TransformState()
                {
                    Tick = _tick,
                    Position = transform.position,
                    Rotation = transform.rotation,
                    HasStartedMoving = true
                };

                _inputStates[bufferIndex] = inputState;
                _transformStates[bufferIndex] = transformState;

                _tickDeltaTime -= _tickrate;
                _tick++;

            }
        }

        public void ProcessSimulatedPlayerMovement()
        {
            _tickDeltaTime += Time.deltaTime;
            if(_tickDeltaTime > _tickrate)
            {
                if(ServerTransformState.Value.HasStartedMoving)
                {
                    transform.position = ServerTransformState.Value.Position;
                    transform.rotation = ServerTransformState.Value.Rotation;
                }

                _tickDeltaTime -= _tickrate;
                _tick++;
            }
        }

        private void MovePlayer(Vector2 movementInput)
        {
            Vector3 movement = movementInput.x * _vcamTransform.right + movementInput.y * _vcamTransform.forward;
            movement.y = 0;

            //gravity
            /*
            if(!_cc.isGrounded)
            {
                movement.y = -9.61f;
            }
            */

            _cc.Move(movement * _speed * _tickrate);

        }

        private void RotatePlayer(Vector2 lookInput)
        {
            _vcamTransform.RotateAround(_vcamTransform.position, _vcamTransform.right, -lookInput.y * _turnSpeed * _tickrate);
            transform.RotateAround(transform.position, transform.up, lookInput.x * _turnSpeed * _tickrate);
        }

        [ServerRpc]
        private void MovePlayerServerRpc(int tick, Vector2 movementInput, Vector2 lookInput)
        {
            MovePlayer(movementInput);
            RotatePlayer(lookInput);

            TransformState state = new TransformState()
            {
                Tick = tick,
                Position = transform.position,
                Rotation = transform.rotation,
                HasStartedMoving = true
            };

            _PreviousTransformState = ServerTransformState.Value;
            ServerTransformState.Value = state;
        }


    }
}
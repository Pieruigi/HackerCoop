using Fusion;
using HKR.Building;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace HKR
{
    public enum PlayerState: byte { Paused, Alive, Dead }

    public class PlayerController : NetworkBehaviour
    {
        public static UnityAction OnSpawned;

        public static PlayerController Local { get; private set; }

        [SerializeField]
        float runSpeed = 5;

        [SerializeField]
        float walkSpeed = 2;

        [SerializeField]
        float crouchSpeed = 1.5f;

        [SerializeField]
        float jumpSpeed = 3f;

        [SerializeField]
        float rotationSpeed = 360f;

        [SerializeField]
        float acceleration = 10;

        [SerializeField]
        float deceleration = 10;

        [SerializeField]
        float maxPitch = 50;

        [SerializeField]
        float minPitch = -80;

        [SerializeField]
        float mouseSensitivity = 1;

        [SerializeField]
        float crouchHeight = 1;

        [SerializeField]
        Transform cameraRoot;

        CharacterController characterController;
        Camera playerCamera;
        Vector3 velocity = Vector3.zero;
        Vector2 moveInput = Vector2.zero;
        Vector2 aimInput = Vector2.zero;
        bool crouchInput = false;
        bool walkInput = false;
        bool jumpInput = false;
        bool jumping = false;
        bool crouching = false;

        float pitch = 0;
        float yaw = 0;

        float characterDefaultHeight;
        float cameraDefaultHeight;
        float cameraCrouchHeight;

        Animator animator;
        string crouchAnimParam = "Crouch";

        //PlayerState state;
        [UnitySerializeField]
        [Networked]
        public PlayerState State { get; private set; }
      

        ChangeDetector changeDetector;
        bool isSpawned = false;

        bool inputDisabled = false;
        

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();

            characterDefaultHeight = characterController.height;
            cameraDefaultHeight = cameraRoot.localPosition.y;
            float crouchMul = crouchHeight / characterDefaultHeight;
            cameraCrouchHeight = cameraDefaultHeight * crouchMul;
         
        }
               
        // Update is called once per frame
        void Update()
        {
            if (!isSpawned)
                return;

            DetectChanges();

            UpdateState();
        }

        private void LateUpdate()
        {
            if (!isSpawned)
                return;

            LateUpdateState();
        }

        public override void FixedUpdateNetwork()
        {
            if (!isSpawned)
                return;

            base.FixedUpdateNetwork();

            FixedUpdateNetworkState();

        }

      
        public override void Spawned()
        {
            base.Spawned();

            isSpawned = true;

            // Move the scene camera under the player who has state authority
            if (HasStateAuthority)
            {
                Local = this;
                //State = PlayerState.Alive;
                MoveCameraInside();
            }
          
            changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

            OnSpawned?.Invoke();
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            base.Despawned(runner, hasState);

            if (HasStateAuthority)
                Local = null;
        }

        public static void DespawnLocalPlayerController()
        {
            if (Local == null)
                return;
            Local.Runner.Despawn(Local.Object);
            Local = null;
        }

        void MoveCameraInside()
        {
            playerCamera = Camera.main;
            playerCamera.transform.parent = cameraRoot;
            playerCamera.transform.localPosition = Vector3.zero;
            playerCamera.transform.localRotation = Quaternion.identity;
        }

        void UpdateState()
        {
            switch (State)
            {
                case PlayerState.Alive:
                    UpdateNormalState();
                    break;
            }
        }


        void LateUpdateState()
        {
            switch (State)
            {
                case PlayerState.Alive:
                    LateUpdateNormalState();
                    break;
            }
        }

        void FixedUpdateNetworkState()
        {
            switch (State)
            {
                case PlayerState.Alive:
                    FixedUpdateNetworkNormalState();
                    break;
            }
        }

        void LateUpdateNormalState()
        {
            if (!HasStateAuthority)
                return;

            Pitch();
        }

        void FixedUpdateNetworkNormalState()
        {
            if (!HasStateAuthority)
                return;

            Yaw();

            Crouch();

            Move();
        }

        void UpdateNormalState()
        {
            if (!HasStateAuthority)
                return;

            CheckInput();

            SetPitchAndJaw();

            UpdateAnimations();
        }

        void DetectChanges()
        {
            if(changeDetector == null)
                return;
            foreach (var propertyName in changeDetector.DetectChanges(this, out var previousBuffer, out var currentBuffer))
            {
                switch (propertyName)
                {
                  
                    case nameof(State):
                        var stateReader = GetPropertyReader<PlayerState>(propertyName);
                        var (statePrev, stateCurr) = stateReader.Read(previousBuffer, currentBuffer);
                        EnterNewState(statePrev, stateCurr);
                        break;
                }
            }
        }
        
        void EnterNewState(PlayerState oldState, PlayerState newState)
        {
            switch(newState)
            {
                case PlayerState.Alive:

                    break;

                case PlayerState.Dead:
                    
                    break;
            }
        }

        private void CheckInput()
        {
            if (inputDisabled)
            {
                moveInput = Vector2.zero;
                aimInput = Vector2.zero;
                crouchInput = false;
                walkInput = false;
                jumpInput = false;

            }
            else
            {
                moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                aimInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
               
                crouchInput = Input.GetAxis("Crouch") > 0;
                walkInput = !crouchInput && Input.GetAxis("Walk") > 0;
                jumpInput = !jumping && !crouchInput && Input.GetAxis("Jump") > 0;
            }
            

            
        }

        void UpdateAnimations()
        {
            if(animator.GetBool(crouchAnimParam) != crouching)
                animator.SetBool(crouchAnimParam, crouching);

        }

        void SetPitchAndJaw()
        {
            // Mouse is already frame independent, so we don't need delta time
            yaw += aimInput.x * mouseSensitivity;// * Time.deltaTime;
            pitch += -aimInput.y * mouseSensitivity;// * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        void Yaw()
        {
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }

        void Pitch()
        {
            playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        void Crouch()
        {
            if (crouchInput)
            {
                crouching = true;

                float cSpeed = 2;
                if (characterController.height > crouchHeight)
                {
                    characterController.height = Mathf.MoveTowards(characterController.height, crouchHeight, Time.fixedDeltaTime * cSpeed);
                    Vector3 collCenter = characterController.center;
                    collCenter.y = characterController.height * .5f;
                    characterController.center = collCenter;
                }

                Vector3 pos = cameraRoot.localPosition;
                if (pos.y > cameraCrouchHeight)
                {
                    pos.y = Mathf.MoveTowards(pos.y, cameraCrouchHeight, Time.fixedDeltaTime * cSpeed);
                    cameraRoot.localPosition = pos;
                }

            }
            else
            {
                crouching = false;

                float cSpeed = 2;
                if (characterController.height < characterDefaultHeight)
                {
                    characterController.height = Mathf.MoveTowards(characterController.height, characterDefaultHeight, Time.fixedDeltaTime * cSpeed);
                    Vector3 collCenter = characterController.center;
                    collCenter.y = characterController.height * .5f;
                    characterController.center = collCenter;
                }

                Vector3 pos = cameraRoot.localPosition;
                if (pos.y < cameraDefaultHeight)
                {
                    pos.y = Mathf.MoveTowards(pos.y, cameraDefaultHeight, Time.fixedDeltaTime * cSpeed);
                    cameraRoot.localPosition = pos;
                }



            }
        }

        void Move()
        {
            Vector3 hVel = Vector3.ProjectOnPlane(velocity, Vector3.up);
            Vector3 vVel = Vector3.up * velocity.y;

            Vector3 targetHVel = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
            if(moveInput.magnitude > 0)
            {
                float hMaxSpeed = GetMoveSpeed();
                targetHVel *= hMaxSpeed;
                if(hVel.magnitude > 0 && Vector3.Dot(hVel, targetHVel) < 0)
                    targetHVel = Vector3.zero;
            }
            
            hVel = Vector3.MoveTowards(hVel, targetHVel, acceleration * Time.fixedDeltaTime);

            if (!characterController.isGrounded)
            {
                vVel += Physics.gravity.y * Vector3.up * Time.fixedDeltaTime;
                jumping = false;
            }
            else
            {
                if (jumpInput)
                {
                    if (!jumping)
                    {
                        jumping = true;
                        vVel = jumpSpeed * Vector3.up;
                    }
                    
                }
                else
                {
                    vVel = Vector3.zero;
                }
                
            }

            velocity = hVel + vVel;
            characterController.Move(velocity*Time.fixedDeltaTime);

        }
        
       

        float GetMoveSpeed()
        {
            if (moveInput.magnitude == 0)
                return 0;

            if (walkInput)
                return walkSpeed;

            if (crouchInput)
                return crouchSpeed;


            return runSpeed;
        }

        public void SetAlive()
        {
            State = PlayerState.Alive;
        }

        //public void SetNormalState()
        //{
        //    if (HasStateAuthority)
        //        State = PlayerState.Normal;
        //}

        /// <summary>
        /// Returns the floor level the player stands in
        /// </summary>
        /// <returns></returns>
        public int GetCurrentFloorLevel()
        {
            float playerY = transform.position.y;
            int level = Mathf.FloorToInt(playerY / BuildingBlock.Height);
            
            return level;
        }
    }

}

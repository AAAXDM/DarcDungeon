using UnityEngine;

namespace DarkDungeon
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {
        #region Fields
        Rigidbody2D rigidBody;
        MoveState moveState = MoveState.Idle;
        DirectionState directionState = DirectionState.Right;
        MoveType moveType = MoveType.Walk;

        float speed;
        float walkSpeed = 2;
        float runSpeed = 4;
        float walkTime = 0;
        float walkColdown = 0.1f;
        float xMovement;
        float runTime;
        float runTimeColdown = 3;
        float runRespiteColdown = 2;
        float runRespite;

        bool canRun = true;
        #endregion

        #region Properties
        public bool CanRun => canRun;
        #endregion

        #region public Methods
        public void MoveRight()
        {
            if (directionState == DirectionState.Left)
            {
                transform.localRotation = Quaternion.Euler(0, 0, 0);
            }

            moveState = MoveState.Moving;
            directionState = DirectionState.Right;
            walkTime = walkColdown;
        }

        public void MoveLeft()
        {
            if (directionState == DirectionState.Right)
            {
                transform.localRotation = Quaternion.Euler(0, 180, 0);
            }

            moveState = MoveState.Moving;
            directionState = DirectionState.Left;
            walkTime = walkColdown;
        }

        public void MovwUp(float xMovement)
        {
            moveState = MoveState.Moving;
            directionState = DirectionState.Up;
            this.xMovement = xMovement;
            walkTime = walkColdown;
        }

        public void MoveDown(float xMovement)
        {
            moveState = MoveState.Moving;
            directionState = DirectionState.Down;
            this.xMovement = xMovement;
            transform.localRotation = xMovement > 0 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 180, 0);
            walkTime = walkColdown;
        }

        public void Run()
        {
            if (moveState == MoveState.Moving)
            {
                moveType = MoveType.Run;
                speed = runSpeed;
            }
        }

        public void StopRunning()
        {
            moveType = MoveType.Walk;
            speed = walkSpeed;
            if (runTime < runRespiteColdown)
            {
                runRespite = runTime;
            }
        }
        #endregion

        #region Core Methods
        void Start()
        {
            rigidBody = GetComponent<Rigidbody2D>();
            walkTime = walkColdown;
            speed = walkSpeed;
            runTime = runTimeColdown;
        }
     
        void Update()
        {
            if (!canRun)
            {
                Rest();
            }

            if (moveState == MoveState.Moving)
            {
                if (moveType == MoveType.Run)
                {
                    runTime -= Time.deltaTime;
                    if (runTime <= 0)
                    {
                        canRun = false;
                        StopRunning();
                    }
                }

                Move();

                walkTime -= Time.deltaTime;

                if (walkTime <= 0)
                {
                    Idle();
                }
            }
            
            if(moveType != MoveType.Run)
            {
                IncreaseRunTime();
            }
        }
        #endregion

        #region Support Methods
        void Idle()
        {
            moveState = MoveState.Idle;
        }

        void IncreaseRunTime()
        {
            if (runTime < runTimeColdown)
            {
                runTime += Time.deltaTime;
            }
        }

        void Rest()
        {
            runRespite += Time.deltaTime;
            if (runRespite >= runRespiteColdown)
            {
                runRespite = runRespiteColdown;
                runTime = runTimeColdown;
                canRun = true;
            }
        }

        void Move()
        {
            if (directionState == DirectionState.Left || directionState == DirectionState.Right)
            {
                rigidBody.velocity =
                    ((directionState == DirectionState.Right ? Vector2.right : Vector2.left) * speed);
            }

            else
            {
                float yDirection = directionState == DirectionState.Up ? 1 : -1;
                Vector2 direction = new Vector2(xMovement, yDirection);
                rigidBody.velocity = direction * speed;
            }
        }
        #endregion

        #region Enums
        enum MoveState { Idle, Moving }
        enum MoveType { Walk, Run }
        enum DirectionState { Left, Right, Up, Down }
        #endregion
    }
    }
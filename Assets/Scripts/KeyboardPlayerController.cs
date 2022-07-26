using UnityEngine;

namespace DarkDungeon
{
    public class KeyboardPlayerController : MonoBehaviour
    {
        [SerializeField] Player player; 
        void Start()
        {
            if(player == null)
            {
                Debug.LogError("Player not set to controller!");
            }
        }

        void Update()
        {
           Run();
           Movement();           
        }

        void Movement()
        {
            if(Input.GetKey(KeyCode.D))
            {
                player.MoveRight();
            }
            if(Input.GetKey(KeyCode.A))
            {
                player.MoveLeft();
            }
            if(Input.GetKey(KeyCode.W))
            {
                float xMovement = Input.GetAxis("Horizontal");
                player.MovwUp(xMovement);
            }
            if(Input.GetKey(KeyCode.S))
            {
                float xMovement = Input.GetAxis("Horizontal");
                player.MoveDown(xMovement);
            }
        }

        void Run()
        {
            if(Input.GetKeyDown(KeyCode.LeftShift) && player.CanRun)
            {
                player.Run();
            }
            if(Input.GetKeyUp(KeyCode.LeftShift))
            {
                player.StopRunning();
            }
        }
    }
}
using UnityEngine;

namespace DarkDungeon
{
    public class CameraMotion : MonoBehaviour
    {
        #region Fields
        GameObject player;
        Transform playerTransform;
        float zDistance = 10;
        float delay = 3;
        #endregion

        #region Core Methods
        void Start()
        {
            player = FindObjectOfType<Player>().gameObject;
            if (player == null) throw new System.Exception("No player to follow!");
            playerTransform = player.transform;
            transform.position = FindPlayerPostion();
        }

        void Update()
        {
            Vector3 playerPos = FindPlayerPostion();
            Vector3 pos = Vector3.Lerp(transform.position, playerPos, delay);
            transform.position = pos;
        }
        #endregion

        #region Support Methods
        Vector3 FindPlayerPostion()
        {
            return new Vector3(playerTransform.position.x, playerTransform.position.y, playerTransform.position.z - zDistance);
        }
        #endregion
    }
}
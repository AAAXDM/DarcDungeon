using UnityEngine;

namespace DarkDungeon
{
    [CreateAssetMenu(fileName = "GroundSO", menuName = "GroundSO", order = 51)]
    public class GroundSO : ScriptableObject
    {
        [SerializeField] Sprite[] sprites;

        public Sprite GetRandomSprite()
        {
            int random = Random.Range(0, sprites.Length);
            return sprites[random];
        }
    }
}

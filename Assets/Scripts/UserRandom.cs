using UnityEngine;

namespace DarkDungeon
{
    public static class UserRandom
    {
       public static bool RandomBool() => Random.value > 0.5;
    }
}
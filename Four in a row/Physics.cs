using Microsoft.Xna.Framework;

namespace Four_in_a_row
{
    public class Physics
    {
        //Physics constants
        public const float Gravity = 0.3f;

        public static void ApplyGravity(GameObject obj)
        {
            obj.Speed += new Vector2(0, Gravity);
        }
    }
}
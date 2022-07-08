namespace DarkDungeon
{
    public class UserRect 
    {
        #region Fields
        public readonly int x;
        public readonly int y;
        public readonly int width;
        public readonly int height;
        public readonly int endX;
        public readonly int endY;
        #endregion

        #region Constructor
        public UserRect(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            endX = x + width - 1;
            endY = y + height - 1;
        }
        #endregion
    }
}
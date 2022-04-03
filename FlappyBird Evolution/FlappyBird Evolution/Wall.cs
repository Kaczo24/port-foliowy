namespace FlappyBird_Evolution
{
    class Wall
    {
        public float x = Program.world.boundaries2.x;
        public static float speed = 4;
        public int hu, hl;
        public static int thickness = 15, gapHeight = 20;
        public Wall(int hu_)
        {
            hu = (int)Program.world.boundaries2.y - hu_;
            hl = hu_ - gapHeight;
        }
    }
}

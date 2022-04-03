using Physics2D;

namespace FlappyBird_Evolution
{
    class Bird : Vessel
    {
        public bool dead = false;
        public int livability = 0;
        public static float jumpForce = 999;
        public static int size = 4;
        public Bird(Vector2 p) : base(p)
        {
            maxSpeed = 4;
        }
        
        public override void Updata()
        {
            if (!dead)
            {
                base.Updata();
                livability++;
                if (intersected)
                    dead = true;
            }
        }

        public override bool Intesects()
        {
            for (int m = -size; m <= size; m++)
                for (int n = -size; n <= size; n++)
                    if (n * n + m * m <= size * size)
                    {
                        int x = (int)pos.x + n;
                        int y = (int)pos.y + m;
                        if (x > 0)
                            if (y > 0)
                                if (x < world.boundaries2.x)
                                    if (y < world.boundaries2.y)
                                        if (world.walls[x, y])
                                            return true;
                    }
            return false;
        }
        public void Jump()
        {
            AddForce(new Vector2(0, jumpForce));
        }

        public void Draw()
        {
            Program.graphics.circle((int)pos.x, (int)world.boundaries2.y - (int)pos.y, size);
        }
    }
}

using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Physics2D;
using ConsolGraphicsEngine;
using BasicNeuralNetwork;

namespace FlappyBird_Evolution
{
    static class Program
    {
        public static World world = new World(0, 0, 300, 100);
        public static CGE graphics;
        public static List<Wall> walls = new List<Wall>();
        public static List<Brain> brains = new List<Brain>();
        static int generation = 0;
        static bool space = false;

        public static int popSize = 300, simLng = 500, avrageFrom = 3;
        public static bool doShow = true;
        public static int showAfter = 0, showEvery = 10;
        //public static int numberOfBests = 50;


        public static float gravity = 1f;
        public static int newWallPosition = 120;


        static void Main(string[] args)
        {
            Console.ReadKey();
            graphics = new CGE((int)world.boundaries2.x + 1, (int)world.boundaries2.y + 1);
            graphics.strokeWeight = 0;
            graphics.fill = 4;
            //PlayerBased();
            Learning();
        }

        static void Learning()
        {
            if (File.Exists("bird.bnn"))
            {
                Brain brain = new Brain();
                brain.nn = NeuralNetwork.Deserialize(File.ReadAllBytes("bird.bnn"));
                for (int n = 0; n < popSize; n++)
                    brains.Add(brain.Clone());
            }
            else
            {
                for (int n = 0; n < popSize; n++)
                    brains.Add(new Brain());
            }

            
            float unnormalFitness = 0;
            bool exit = false;
            
            Task.Factory.StartNew(() =>
            {
                while (Console.ReadKey().Key != ConsoleKey.Escape) ;
                exit = true;
            });

            Task.Factory.StartNew(() =>
            {
                while (Console.ReadKey().Key != ConsoleKey.Spacebar) ;
                space = true;
            });


            do
            {
                train();
                unnormalFitness = avrageAndShow();

                if (exit)
                {
                    File.WriteAllBytes("bird.bnn", brains[0].nn.Serialize());
                    goto TheEnd;
                }
                if (space)
                {
                    space = false;
                    Task.Factory.StartNew(() =>
                    {
                        while (Console.ReadKey().Key != ConsoleKey.Spacebar) ;
                        space = true;
                    });
                }

                world = new World(0, 0, 300, 100);
                walls = new List<Wall>();
                generation++;

            } while (unnormalFitness < simLng * avrageFrom);


            Console.ForegroundColor = ConsoleColor.Green;

            while(true)
            {
                train();
                ShowWinning();
            }
            
            TheEnd:;
        }


        static void train()
        {
            if (generation != 0)
            {
                for (int n = popSize - 1; n >= 0 && brains.Count > 10; n--)
                    if (World.rng.NextDouble() < brains[n].fitness)
                        brains.RemoveAt(n);
                while (brains.Count < popSize)
                {
                    int index = (int)Math.Abs(Mathf.Gaussian(0.4f));
                    if (index < brains.Count)
                        brains.Add(brains[index].Clone().Mutate());
                }
            }

            List<Bird> birds = new List<Bird>();

            for (int n = 0; n < popSize; n++)
            {
                birds.Add(new Bird(new Vector2(50, world.boundaries2.y / 2)));
                world.AddVessel(birds[n]);
            }
            walls.Add(new Wall(World.rng.Next(Wall.gapHeight, (int)world.boundaries2.y)));

            for (int n = 0; n < simLng && birds.FindIndex(x => !x.dead) != -1; n++)
            {
                for (int m = 0; m < popSize; m++)
                    if (brains[m].nn.Predict(new float[]
                    {
                            birds[m].pos.y,
                            birds[m].vel.y,
                            walls[0].x,
                            world.boundaries2.y - walls[0].hu,
                            walls[0].hl
                    })[0] >= 0.5)
                        birds[m].Jump();
                Update();
            }

            for (int n = 0; n < popSize; n++)
                brains[n].fitness = 10f / birds[n].livability;

            brains.Sort();
            brains.ForEach(x => x.fitness /= brains[brains.Count - 1].fitness);
        }

        static float avrageAndShow()
        {
            float unnormalFitness = 0;

            for (int n = 0; n < avrageFrom; n++)
            {
                world = new World(0, 0, 300, 100);
                walls = new List<Wall>();

                Bird bird = new Bird(new Vector2(50, world.boundaries2.y / 2));
                world.AddVessel(bird);
                walls.Add(new Wall(World.rng.Next(Wall.gapHeight, (int)world.boundaries2.y)));

                for (int m = 0; m < simLng && !bird.dead; m++)
                {

                    if (brains[0].nn.Predict(new float[]
                    {
                            bird.pos.y,
                            bird.vel.y,
                            walls[0].x,
                            world.boundaries2.y - walls[0].hu,
                            walls[0].hl
                    })[0] >= 0.5)
                        bird.Jump();
                    Update();
                }
                unnormalFitness += bird.livability;
            }

            if ((doShow && generation % showEvery == 0 && generation >= showAfter) || space)
            {
                world = new World(0, 0, 300, 100);
                walls = new List<Wall>();

                Bird bird = new Bird(new Vector2(50, world.boundaries2.y / 2));
                world.AddVessel(bird);
                walls.Add(new Wall(World.rng.Next(Wall.gapHeight, (int)world.boundaries2.y)));
                while (!bird.dead)
                {
                    if (brains[0].nn.Predict(new float[]
                    {
                            bird.pos.y,
                            bird.vel.y,
                            walls[0].x,
                            world.boundaries2.y - walls[0].hu,
                            walls[0].hl
                    })[0] > 0.5)
                        bird.Jump();
                    Update();
                    graphics.Clear();
                    DrawWalls();
                    bird.Draw();
                    graphics.Redraw();
                    Console.Beep(37, 10);
                }
            }

            return unnormalFitness;
        }


        static void ShowWinning()
        {
            world = new World(0, 0, 300, 100);
            walls = new List<Wall>();
            Bird bird = new Bird(new Vector2(50, world.boundaries2.y / 2));
            world.AddVessel(bird);
            walls.Add(new Wall(World.rng.Next(Wall.gapHeight, (int)world.boundaries2.y)));
            while (!bird.dead)
            {
                if (brains[0].nn.Predict(new float[]
                {
                            bird.pos.y,
                            bird.vel.y,
                            walls[0].x,
                            world.boundaries2.y - walls[0].hu,
                            walls[0].hl
                })[0] > 0.5)
                    bird.Jump();

                Update();
                graphics.Clear();
                DrawWalls();
                bird.Draw();
                graphics.Redraw();
                Console.Beep(37, 10);
            }
            world = new World(0, 0, 300, 100);
            walls = new List<Wall>();
        }




        static void PlayerBased()
        {
            bool space = false;
            Task.Factory.StartNew(() =>
            {
                while (Console.ReadKey().Key != ConsoleKey.Spacebar) ;
                space = true;
            });
            while (!space) { }
            int framecount = 0;
            while (true)
            {
                Bird bird = new Bird(new Vector2(50, world.boundaries2.y / 2));
                world.AddVessel(bird);
                walls.Add(new Wall(World.rng.Next(Wall.gapHeight, (int)world.boundaries2.y)));
                

                while (!bird.dead)
                {
                    if (space)
                    {
                        space = false;
                        bird.Jump();
                        Task.Factory.StartNew(() =>
                        {
                            while (Console.ReadKey().Key != ConsoleKey.Spacebar) ;
                            space = true;
                        });
                    }
                    Update();
                    graphics.Clear();
                    DrawWalls();
                    bird.Draw();
                    graphics.Redraw();
                    Console.Beep(37, 10);
                    framecount++;
                }
                Console.Beep(37, 500);
                while (!space) { }
                world = new World(0, 0, 300, 100);
                walls = new List<Wall>();
            }
        }


        static void Update()
        {
            if (walls.FindIndex(x => x.x < newWallPosition && x.x > (newWallPosition - Wall.speed - 2)) != -1)
                walls.Add(new Wall(World.rng.Next(Wall.gapHeight, (int)world.boundaries2.y)));
            walls.RemoveAll(x => x.x < -Wall.thickness);
            walls.ForEach(x => x.x -= Wall.speed);

            graphics.Clear();
            world.walls = new bool[(int)world.boundaries2.x, (int)world.boundaries2.y];
            DrawWalls();
            for (int n = 0; n < world.boundaries2.x; n++)
                for (int m = 0; m < world.boundaries2.y; m++)
                    world.walls[n, m] = graphics.pixels[n, (int)world.boundaries2.y - m] == graphics.colors[4];
            graphics.Clear();

            world.AddForce(new Vector2(0, -gravity));
            world.Update();
        }

        static void DrawWalls()
        {
            foreach(Wall w in walls)
            {
                graphics.rect((int)w.x, 0, Wall.thickness, w.hu);
                graphics.rect((int)w.x, (int)world.boundaries2.y, Wall.thickness, -w.hl);
            }
        }

    }
}

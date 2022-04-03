using System;
using System.Collections.Generic;
using BasicNeuralNetwork;
using Physics2D;

namespace FlappyBird_Evolution
{
    class Brain : IComparable<Brain>
    {
        public NeuralNetwork nn = new NeuralNetwork(5, 4, 1);
        public float fitness;
        public static Func mutator = x =>
        {
            return x + (float)Mathf.Gaussian(0.2);
        };

        public Brain()
        {
            nn.setActivationFunction(ActivationFunction.no);
        }


        public int CompareTo(Brain other)
        {
            return fitness.CompareTo(other.fitness);
        }

        public Brain Clone()
        {
            Brain b = new Brain();
            b.nn = nn.copy();
            return b;
        }

        public Brain Mutate()
        {
            nn.Mutate(mutator);
            return this;
        }
    }
}


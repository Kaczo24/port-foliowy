using System;
using System.Collections.Generic;

namespace BasicNeuralNetwork
{
    public delegate float Func(float x);
    public struct ActivationFunction
    {
        public Func active;
        public Func dActive;
        public static ActivationFunction sigmoid = new ActivationFunction()
        {
            active = x => (float)(1 / (1 + Math.Exp(-x))),
            dActive = x => x * (1 - x)
        };
        public static ActivationFunction tanh = new ActivationFunction()
        {
            active = x => (float)Math.Tanh(x),
            dActive = x => 1 - (x * x)
        };
        public static ActivationFunction no = new ActivationFunction()
        {
            active = x => x,
            dActive = x => 1
        };
    }

    public class NeuralNetwork
    {

        int inNodes, hidNodes, outNodes;
        Matrix weights_ih, weights_ho, bias_h, bias_o;
        float learningRate;
        ActivationFunction aFunc;
        public NeuralNetwork(int inNodes_, int hidNodes_, int outNodes_)
        {
            inNodes = inNodes_;
            hidNodes = hidNodes_;
            outNodes = outNodes_;

            weights_ih = Matrix.getRandom(hidNodes, inNodes);
            weights_ho = Matrix.getRandom(outNodes, hidNodes);

            bias_h = Matrix.getRandom(hidNodes, 1);
            bias_o = Matrix.getRandom(outNodes, 1);
            setLearningRate(0.1f);
            setActivationFunction(ActivationFunction.sigmoid);
        }
        public NeuralNetwork(NeuralNetwork nn)
        {
            inNodes = nn.inNodes;
            hidNodes = nn.hidNodes;
            outNodes = nn.outNodes;
            weights_ih = nn.weights_ih.copy();
            weights_ho = nn.weights_ho.copy();
            bias_h = nn.bias_h.copy();
            bias_o = nn.bias_o.copy();
            setLearningRate(nn.learningRate);
        }

        public float[] Predict(float[] input)
        {
            Matrix ins = new Matrix(input);
            Matrix hidden = weights_ih * ins;
            hidden += bias_h;
            hidden.map(aFunc.active);

            Matrix outs = weights_ho * hidden;
            outs += bias_o;
            outs.map(aFunc.active);
            return outs.toArray();

        }


        public float Train(float[] input, float[] expectedOutputs)
        {
            Matrix ins = new Matrix(input);
            Matrix targets = new Matrix(expectedOutputs);
            Matrix hidden = weights_ih * ins;
            hidden += bias_h;
            hidden.map(aFunc.active);
            Matrix outs = weights_ho * hidden;
            outs += bias_o;
            outs.map(aFunc.active);

            Matrix outputErrors = targets - outs;
            Matrix gradient = outs.copy().map(aFunc.dActive);
            gradient.elementwiseMultipy(outputErrors);
            gradient *= learningRate;

            weights_ho += gradient * Matrix.transpose(hidden);
            bias_o += gradient;


            Matrix hiddenGradient = hidden.copy().map(aFunc.dActive);
            hiddenGradient.elementwiseMultipy(Matrix.transpose(weights_ho) * outputErrors);
            hiddenGradient *= learningRate;

            weights_ih += hiddenGradient * Matrix.transpose(ins);
            bias_h += hiddenGradient;

            float error = 0;
            float[] errors = outputErrors.toArray();
            foreach (float f in errors)
                error += f * f;
            return error;
        }

        public void setLearningRate(float learningRate_)
        {
            learningRate = learningRate_;
        }

        public void setActivationFunction(ActivationFunction func)
        {
            aFunc = func;
        }

        public NeuralNetwork copy()
        {
            return new NeuralNetwork(this);
        }

        public void Mutate(Func f)
        {
            weights_ih.map(f);
            weights_ho.map(f);
            bias_h.map(f);
            bias_o.map(f);
        }

        public byte[] Serialize()
        {
            List<byte> sr = new List<byte>();
            sr.Add((byte)inNodes);
            sr.Add((byte)hidNodes);
            sr.Add((byte)outNodes);
            sr.AddRange(weights_ih.Serialize());
            sr.AddRange(bias_h.Serialize());
            sr.AddRange(weights_ho.Serialize());
            sr.AddRange(bias_o.Serialize());
            return sr.ToArray();
        }

        public static NeuralNetwork Deserialize(byte[] data)
        {
            NeuralNetwork nn = new NeuralNetwork(data[0], data[1], data[2]);
            int index = 3;

            nn.weights_ih = Matrix.Deserialize(data[1], data[0], data, index);
            index += data[1] * data[0] * 4;

            nn.bias_h = Matrix.Deserialize(data[1], 1, data, index);
            index += data[1] * 4;

            nn.weights_ho = Matrix.Deserialize(data[2], data[1], data, index);
            index += data[1] * data[2] * 4;

            nn.bias_o = Matrix.Deserialize(data[2], 1, data, index);

            return nn;
        }

    }
}

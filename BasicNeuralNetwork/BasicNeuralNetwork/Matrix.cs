using System;
using System.Collections.Generic;
namespace BasicNeuralNetwork
{
    public delegate float mFunc(float val, int row, int col);

    public class Matrix
    {
        public int rows, cols;
        public float[,] data;
        static Random rng = new Random();
        public Matrix(int rows_, int cols_)
        {
            rows = rows_;
            cols = cols_;
            data = new float[rows, cols];
        }

        public Matrix(float[] data_)
        {
            rows = data_.Length;
            cols = 1;
            data = new float[rows, cols];
            map((mFunc)((v, n, m) => data_[n]));
        }
        public static Matrix getRandom(int rows_, int cols_)
        {
            return new Matrix(rows_, cols_).map((mFunc)((t, y, u) => (float)(rng.NextDouble() * 2 - 1)));
        }
        public Matrix copy()
        {
            Matrix mat = new Matrix(rows, cols);
            for (int n = 0; n < rows; n++)
                for (int m = 0; m < cols; m++)
                    mat.data[n, m] = data[n, m];
            return mat;
        }

        public float[] toArray()
        {
            float[] arr = new float[cols * rows];
            for (int n = 0; n < rows; n++)
                for (int m = 0; m < cols; m++)
                    arr[n * cols + m] = data[n, m];
            return arr;
        }

        public static Matrix transpose(Matrix m)
        {
            return new Matrix(m.cols, m.rows).map((mFunc)((_, i, j) => m.data[j, i]));
        }

        public Matrix map(mFunc f)
        {
            if (f != null)
                for (int n = 0; n < rows; n++)
                    for (int m = 0; m < cols; m++)
                        data[n, m] = f(data[n, m], n, m);
            return this;
        }
        public Matrix map(Func f)
        {
            if (f != null)
                for (int n = 0; n < rows; n++)
                    for (int m = 0; m < cols; m++)
                        data[n, m] = f(data[n, m]);
            return this;
        }

        public Matrix elementwiseMultipy(Matrix m)
        {
            return map((mFunc)((e, n, i) => e * m.data[n, i]));
        }

        public static Matrix operator +(Matrix a, Matrix b)
        {
            if (a.rows != b.rows || a.cols != b.cols)
            {
                return a;
            }
            return a.copy().map((mFunc)((e, n, m) => e + b.data[n, m]));
        }
        public static Matrix operator -(Matrix a, Matrix b)
        {
            if (a.rows != b.rows || a.cols != b.cols)
            {
                return a;
            }
            return a.copy().map((mFunc)((e, n, m) => e - b.data[n, m]));
        }
        public static Matrix operator *(Matrix a, Matrix b)
        {
            if (a.cols != b.rows)
                return a;


            return new Matrix(a.rows, b.cols).map((mFunc)((e, i, j) =>
            {
                float sum = 0;
                for (int k = 0; k < a.cols; k++)
                {
                    sum += a.data[i, k] * b.data[k, j];
                }
                return sum;
            }));
        }
        public static Matrix operator *(Matrix a, float b)
        {
            return a.copy().map((mFunc)((e, n, m) => e * b));
        }

        public byte[] Serialize()
        {
            List<byte> sr = new List<byte>();
            for (int n = 0; n < rows; n++)
                for (int m = 0; m < cols; m++)
                    sr.AddRange(BitConverter.GetBytes(data[n, m]));
            return sr.ToArray();
        }

        public static Matrix Deserialize(int r, int c, byte[] data, int startIndex)
        {
            Matrix mx = new Matrix(r, c);
            for (int n = 0; n < mx.rows; n++)
                for (int m = 0; m < mx.cols; m++)
                    mx.data[n, m] = BitConverter.ToSingle(data, (n * c + m) * 4 + startIndex);
            return mx;
        }
    }
}

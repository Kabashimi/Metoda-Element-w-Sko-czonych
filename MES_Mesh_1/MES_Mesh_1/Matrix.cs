using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_Mesh_1
{
    public static class Matrix
    {
        //numeracja w macierzy: wiersz X kolumna
        static public double[,] Multiply(double[,] matrixA, double[,] matrixB)
        {
            double[,] tmp = new double[matrixA.GetLength(0), matrixB.GetLength(1)];
            for(int i = 0; i < tmp.GetLength(0); i++)
            {
                for(int j = 0; j < tmp.GetLength(1); j++)
                {
                    tmp[i, j] = 0;
                    for(int k = 0; k < matrixA.GetLength(1); k++)
                    {
                        tmp[i, j] += matrixA[i, k] * matrixB[k, j];
                    }
                }
            }
            return tmp;
        }

        static public double[,] Multiply(double[,] matrixA, double a)
        {
            double[,] tmp = new double[matrixA.GetLength(0), matrixA.GetLength(1)];
            for (int i = 0; i < tmp.GetLength(0); i++)
            {
                for (int j = 0; j < tmp.GetLength(1); j++)
                {
                    tmp[i, j] = matrixA[i, j] * a;
                }
            }
            return tmp;
        }

        static public double[,] Sum(double[,] matrixA, double[,] matrixB)
        {
            double[,] tmp = new double[matrixA.GetLength(0), matrixA.GetLength(1)];
            for (int i = 0; i < tmp.GetLength(0); i++)
            {
                for (int j = 0; j < tmp.GetLength(1); j++)
                {
                    tmp[i, j] = matrixA[i, j] + matrixB[i, j];
                }
            }
            return tmp;
        }

        static public double[,] Power(double[,] matrixA)
        {
            double[,] matrixAT = T(matrixA);
            double[,] tmp = Multiply(matrixA, matrixAT);
            return tmp;
        }

        static public double[,] T(double[,] matrix)
        {
            double[,] tmp = new double[matrix.GetLength(1), matrix.GetLength(0)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    tmp[j, i] = matrix[i, j];
                }
            }
            return tmp;
        }

        static public double[,] ReverseMatrix(double[,] matrix)
        {
            double[,] tmp = new double[matrix.GetLength(1), matrix.GetLength(0)];
            for(int i=0;i<matrix.GetLength(0); i++)
            {
                for(int j = 0; j < matrix.GetLength(1); j++)
                {
                    tmp[j, i] = matrix[i, j];
                }
            }
            if (matrix.GetLength(0) == 2 && matrix.GetLength(1) == 2)
            {
                tmp[0, 1] *= -1;
                tmp[1,0] *= -1;
            }
            else
            {
                //trzeba wyliczyć które wartości zmienić na przeciwne
            }
            return tmp;
        }


    }
}

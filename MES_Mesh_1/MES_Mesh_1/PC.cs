using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_Mesh_1
{
    class PC
    {
        public double ksi;
        public double eta;
        public double[,] J;
        public double[,] derivative;
        public double[,] derivativeGlobal;
        public double detJ;
        public double[,] dNdx;
        public double[,] dNdy;

        public PC(double x1, double x2)
        {
            ksi = x1;
            eta = x2;
        }

        private double calculateDetJ(double[,] matrix)
        {
            double tmp = 0;
            tmp = J[0, 0] * J[1, 1] - (J[0, 1] * J[1, 0]);
            return tmp;
        }

        public void PrepareFinalDerivativeMatrix()
        {
            detJ = calculateDetJ(J);
            double[,] tmpJ = Matrix.ReverseMatrix(J);
            for (int i = 0; i < J.GetLength(0); i++)
            {
                for(int j = 0; j < J.GetLength(1); j++)
                {
                    tmpJ[i, j] = J[i, j] / detJ;
                }
            }

            double[,] tmp = Matrix.Multiply(tmpJ, derivative);

            dNdx = new double[4, 1];
            dNdx[0, 0] = tmp[0, 0];
            dNdx[1, 0] = tmp[0, 1];
            dNdx[2, 0] = tmp[0, 2];
            dNdx[3, 0] = tmp[0, 3];

            dNdy = new double[4, 1];
            dNdy[0, 0] = tmp[1, 0];
            dNdy[1, 0] = tmp[1, 1];
            dNdy[2, 0] = tmp[1, 2];
            dNdy[3, 0] = tmp[1, 3];

            derivativeGlobal = tmp;
        }

        
    }
}

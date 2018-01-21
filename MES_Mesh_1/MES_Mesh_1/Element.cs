using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_Mesh_1
{
    class Element
    {
        public Node[] ID;   //współrzędne globalne wezłów
        public PC[] pc;
        public bool[] wall;
        public double elementH;
        public double elementB;

        public double N1(double ksi, double eta) { return 0.25 * (1 - ksi) * (1 - eta); }
        public double N2(double ksi, double eta) { return 0.25 * (1 + ksi) * (1 - eta); }
        public double N3(double ksi, double eta) { return 0.25 * (1 + ksi) * (1 + eta); }
        public double N4(double ksi, double eta) { return 0.25 * (1 - ksi) * (1 + eta); }
        /*
        public double N1ksi(double x) { return -0.25 * (1 - x); }
        public double N2ksi(double x) { return 0.25 * (1 - x); }
        public double N3ksi(double x) { return 0.25 * (1 + x); }
        public double N4ksi(double x) { return -0.25 * (1 + x); }

        public double N1eta(double x) { return -0.25 * (1 - x); }
        public double N2eta(double x) { return -0.25 * (1 + x); }
        public double N3eta(double x) { return 0.25 * (1 - x); }
        public double N4eta(double x) { return 0.25 * (1 + x); }
        */
        public double N1ksi(double x) { return -0.25 * (1 - x); }
        public double N2ksi(double x) { return 0.25 * (1 - x); }
        public double N3ksi(double x) { return 0.25 * (1 + x); }
        public double N4ksi(double x) { return -0.25 * (1 + x); }

        public double N1eta(double x) { return -0.25 * (1 - x); }
        public double N2eta(double x) { return -0.25 * (1 + x); }
        public double N3eta(double x) { return 0.25 * (1 + x); }
        public double N4eta(double x) { return 0.25 * (1 - x); }
        

        public double[,] Nksi;
        public double[,] Neta;
        public double[,] N;
        public double[,] H;
        public double[,] C;
        public double[,] P;

        public double k;
        public double c;
        public double ro;
        public double t;
        public double a;


        public Element()
        {
            ID = new Node[4];  //new?! -> potrezbny konstruktor ==> jedak nie, wartośći przypisywane podczas tworzenia siatki
            pc = new PC[4] { new PC(-0.577, -0.577), new PC(0.577, -0.577), new PC(0.577, 0.577), new PC(-0.577, 0.577) };
            wall = new bool[4]; //all false;

        }

        public double[,] CalculateJakobian(int id)//id punktu całkowania
        {
            double[,] J = new double[2, 2];
            J[0, 0]
                = N1ksi(pc[id].eta) * ID[0].x
                + N2ksi(pc[id].eta) * ID[1].x 
                + N3ksi(pc[id].eta) * ID[2].x
                + N4ksi(pc[id].eta) * ID[3].x; // macierz zapisana w postaci { x/ksi y/ksi x/eta y/eta
            J[0, 1] = N1ksi(pc[id].eta) * ID[0].y 
                + N2ksi(pc[id].eta) * ID[1].y 
                + N3ksi(pc[id].eta) * ID[2].y 
                + N4ksi(pc[id].eta) * ID[3].y;
            J[1, 0] = N1eta(pc[id].ksi) * ID[0].x 
                + N2eta(pc[id].ksi) * ID[1].x 
                + N3eta(pc[id].ksi) * ID[2].x 
                + N4eta(pc[id].ksi) * ID[3].x;
            J[1, 1] = N1eta(pc[id].ksi) * ID[0].y 
                + N2eta(pc[id].ksi) * ID[1].y 
                + N3eta(pc[id].ksi) * ID[2].y 
                + N4eta(pc[id].ksi) * ID[3].y;
            return J;
        }

        public double[,] prepareDerivativeMatrix(int id)//id punktu całkowania
        {
            double[,] tmp = new double[2, 4];
            tmp[0, 0] = N1ksi(pc[id].ksi);
            tmp[0, 1] = N2ksi(pc[id].ksi);
            tmp[0, 2] = N3ksi(pc[id].ksi);
            tmp[0, 3] = N4ksi(pc[id].ksi);
            tmp[1, 0] = N1eta(pc[id].eta);
            tmp[1, 1] = N2eta(pc[id].eta);
            tmp[1, 2] = N3eta(pc[id].eta);
            tmp[1, 3] = N4eta(pc[id].eta);
            return tmp;
        }

        public double[,] CalculateH(List<double[,]> dNdx, List<double[,]> dNdy)
        {
            double[,] result = new double[4, 4]; //rozmiar lokalnej [H]
            double[,] tmpSum = new double[4, 4];
            //Jeśli w schemacie 2-punktowym wagi są równe 1 to wsyatrczy sumować?
            for (int i = 0; i < pc.GetLength(0); i++)
            {
                tmpSum = Matrix.Sum(Matrix.Power(dNdx[i]), Matrix.Power(dNdy[i]));
                tmpSum = Matrix.Multiply(tmpSum, pc[i].detJ);
                result = Matrix.Sum(result, tmpSum);
            }
            result = Matrix.Multiply(result, k);
            #region warunki brzegowe
            //dodać całka a*{N}^2 dS (warunki brzegowe)
            double J = pc[0].J[0, 0];
            double[,] tmpN = new double[4, 1];
            tmpSum = new double[4, 4];
            if (wall[0])
            {
                //rzut na ściane po lewej
               // result = new double[4, 4];
                tmpN[0, 0] = N1(-1, pc[0].eta);
                tmpN[1, 0] = N2(-1, pc[0].eta);
                tmpN[2, 0] = N3(-1, pc[0].eta);
                tmpN[3, 0] = N4(-1, pc[0].eta);
                tmpSum = Matrix.Power(tmpN);
                tmpSum = Matrix.Multiply(tmpSum, J*a);
                tmpN[0, 0] = N1(-1, pc[3].eta);
                tmpN[1, 0] = N2(-1, pc[3].eta);
                tmpN[2, 0] = N3(-1, pc[3].eta);
                tmpN[3, 0] = N4(-1, pc[3].eta);
                tmpSum = Matrix.Sum(tmpSum, Matrix.Multiply(Matrix.Power(tmpN), J*a));
                result = Matrix.Sum(result, tmpSum);
            }
            if (wall[2])
            {
                //rzut na ściane po prawej
                //result = new double[4, 4];
                tmpN[0, 0] = N1(1, pc[0].eta);
                tmpN[1, 0] = N2(1, pc[0].eta);
                tmpN[2, 0] = N3(1, pc[0].eta);
                tmpN[3, 0] = N4(1, pc[0].eta);
                tmpSum = Matrix.Power(tmpN);
                tmpSum = Matrix.Multiply(tmpSum, J * a);
                tmpN[0, 0] = N1(1, pc[3].eta);
                tmpN[1, 0] = N2(1, pc[3].eta);
                tmpN[2, 0] = N3(1, pc[3].eta);
                tmpN[3, 0] = N4(1, pc[3].eta);
                tmpSum = Matrix.Sum(tmpSum, Matrix.Multiply(Matrix.Power(tmpN), J * a));
                result = Matrix.Sum(result, tmpSum);
            }
            if (wall[1])
            {
                //rzut na ściane na dole
                //result = new double[4, 4];
                tmpN[0, 0] = N1(pc[0].ksi, -1);
                tmpN[1, 0] = N2(pc[0].ksi, -1);
                tmpN[2, 0] = N3(pc[0].ksi, -1);
                tmpN[3, 0] = N4(pc[0].ksi, -1);
                tmpSum = Matrix.Power(tmpN);
                tmpSum = Matrix.Multiply(tmpSum, J * a);
                tmpN[0, 0] = N1(pc[1].ksi, -1);
                tmpN[1, 0] = N2(pc[1].ksi, -1);
                tmpN[2, 0] = N3(pc[1].ksi, -1);
                tmpN[3, 0] = N4(pc[1].ksi, -1);
                tmpSum = Matrix.Sum(tmpSum, Matrix.Multiply(Matrix.Power(tmpN), J * a));
                result = Matrix.Sum(result, tmpSum);
            }
            if (wall[3])
            {
                //rzut na ściane u góry
                //result = new double[4, 4];
                tmpN[0, 0] = N1(pc[0].ksi, 1);
                tmpN[1, 0] = N2(pc[0].ksi, 1);
                tmpN[2, 0] = N3(pc[0].ksi, 1);
                tmpN[3, 0] = N4(pc[0].ksi, 1);
                tmpSum = Matrix.Power(tmpN);
                tmpSum = Matrix.Multiply(tmpSum, J * a);
                tmpN[0, 0] = N1(pc[1].ksi, 1);
                tmpN[1, 0] = N2(pc[1].ksi, 1);
                tmpN[2, 0] = N3(pc[1].ksi, 1);
                tmpN[3, 0] = N4(pc[1].ksi, 1);
                tmpSum = Matrix.Sum(tmpSum, Matrix.Multiply(Matrix.Power(tmpN), J * a));
                result = Matrix.Sum(result, tmpSum);
            }
            #endregion

            return result;
        }

        public double[,] CalculateC(double c, double ro)
        {
            double W = 1;
            double[,] result = new double[4, 4];
            double[,] tmpN = new double[4, 1];
            double[,] tmp = new double[4, 4];
            for (int i = 0; i < pc.GetLength(0); i++)
            {
                tmpN[0, 0] = N1(pc[i].ksi, pc[i].eta);
                tmpN[1, 0] = N2(pc[i].ksi, pc[i].eta);
                tmpN[2, 0] = N3(pc[i].ksi, pc[i].eta);
                tmpN[3, 0] = N4(pc[i].ksi, pc[i].eta);
                tmp = Matrix.Power(tmpN);
                tmp = Matrix.Multiply(tmp,W);
                tmp = Matrix.Multiply(tmp, pc[i].detJ);
                result = Matrix.Sum(result, tmp);
            }
            result = Matrix.Multiply(result, c * ro);
            return result;
        }

        public double[,] CalculateP()
        {
            double[,] result = new double[4, 1];
            double[,] tmp = new double[4, 1];
            double[,] tmpN = new double[4, 1];
            #region warunki brzegowe
            double element_height = pc[0].J[0,0];
            //double element_height = elementH;
            if (wall[0])//konwekcja na lewej ściance
            {
                //result = new double[4, 1];
                tmpN[0, 0] = N1(-1, pc[0].eta);
                tmpN[1, 0] = N2(-1, pc[0].eta);
                tmpN[2, 0] = N3(-1, pc[0].eta);
                tmpN[3, 0] = N4(-1, pc[0].eta);
                tmp = Matrix.Multiply(tmpN, t * a * element_height);
                result = Matrix.Sum(result, tmp);
                tmpN[0, 0] = N1(-1, pc[3].eta);
                tmpN[1, 0] = N2(-1, pc[3].eta);
                tmpN[2, 0] = N3(-1, pc[3].eta);
                tmpN[3, 0] = N4(-1, pc[3].eta);
                tmp = Matrix.Multiply(tmpN, t * a * element_height);
                result = Matrix.Sum(result, tmp);
            }
            if (wall[1])//konwekcja na dolnej ściance
            {
                //result = new double[4, 1];
                tmpN[0, 0] = N1(pc[0].ksi, -1);
                tmpN[1, 0] = N2(pc[0].ksi, -1);
                tmpN[2, 0] = N3(pc[0].ksi, -1);
                tmpN[3, 0] = N4(pc[0].ksi, -1);
                tmp = Matrix.Multiply(tmpN, t * a * element_height);
                result = Matrix.Sum(result, tmp);
                tmpN[0, 0] = N1(pc[1].ksi, -1);
                tmpN[1, 0] = N2(pc[1].ksi, -1);
                tmpN[2, 0] = N3(pc[1].ksi, -1);
                tmpN[3, 0] = N4(pc[1].ksi, -1);
                tmp = Matrix.Multiply(tmpN, t * a * element_height);
                result = Matrix.Sum(result, tmp);
                tmp = tmp;
            }
            if (wall[2])//konwekcja na prawej ściance
            {
                //result = new double[4, 1];
                tmpN[0, 0] = N1(1, pc[1].eta);
                tmpN[1, 0] = N2(1, pc[1].eta);
                tmpN[2, 0] = N3(1, pc[1].eta);
                tmpN[3, 0] = N4(1, pc[1].eta);
                tmp = Matrix.Multiply(tmpN, t * a * element_height);
                result = Matrix.Sum(result, tmp);
                tmpN[0, 0] = N1(1, pc[2].eta);
                tmpN[1, 0] = N2(1, pc[2].eta);
                tmpN[2, 0] = N3(1, pc[2].eta);
                tmpN[3, 0] = N4(1, pc[2].eta);
                tmp = Matrix.Multiply(tmpN, t * a * element_height);
                result = Matrix.Sum(result, tmp);
            }
            if (wall[3])//konwekcja na górnej ściance
            {
                //result = new double[4, 1];
                tmpN[0, 0] = N1(pc[3].ksi, 1);
                tmpN[1, 0] = N2(pc[3].ksi, 1);
                tmpN[2, 0] = N3(pc[3].ksi, 1);
                tmpN[3, 0] = N4(pc[3].ksi, 1);
                tmp = Matrix.Multiply(tmpN, t * a * element_height);
                result = Matrix.Sum(result, tmp);
                tmpN[0, 0] = N1(pc[2].ksi, 1);
                tmpN[1, 0] = N2(pc[2].ksi, 1);
                tmpN[2, 0] = N3(pc[2].ksi, 1);
                tmpN[3, 0] = N4(pc[2].ksi, 1);
                tmp = Matrix.Multiply(tmpN, t * a * element_height);
                result = Matrix.Sum(result, tmp);
            }
            #endregion
            return result;
        }

        public void PrepareData(double k, double c, double ro, double a, double t)
        {
            this.k = k;
            this.c = c;
            this.ro = ro;
            this.t = t;
            this.a = a;
            elementH = ID[1].x - ID[0].x;
            elementB = ID[3].y - ID[0].y;
            List<double[,]> dNdx = new List<double[,]>();
            List<double[,]> dNdy = new List<double[,]>();
            for (int i = 0; i < pc.GetLength(0); i++)
            {
                pc[i].J = CalculateJakobian(i);
                pc[i].derivative = prepareDerivativeMatrix(i);
                pc[i].PrepareFinalDerivativeMatrix();
                dNdx.Add(pc[i].dNdx);
                dNdy.Add(pc[i].dNdy);
            }
            H = CalculateH(dNdx, dNdy);
            C = CalculateC(c, ro);
        }

        public double[,] Calculate()
        {
            P = CalculateP();
            return P;
        }


        /* zbędne?
        public double CalculateDet(Jacobian J)
        {
            double suma = 0;
            suma = J.J[0] * J.J[3] - (J.J[1] * J.J[2]);
            return suma;
        }
        
        public Jacobian ReversJacobian(Jacobian J)
        {
            double det = CalculateDet(J);
            Jacobian tmp = new Jacobian();
            tmp.J[0] = J.J[3]/det;
            tmp.J[1] = J.J[1]*(-1)/det;
            tmp.J[2] = J.J[2]*(-1)/det;
            tmp.J[3] = J.J[0]/det;
            return tmp;
        }
        */
    }
}

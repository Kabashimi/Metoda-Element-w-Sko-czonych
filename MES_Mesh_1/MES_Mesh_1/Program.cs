using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace MES_Mesh_1
{
    class Program
    {
        static void Main(string[] args)
        {

            GlobalData globalData = new GlobalData();
            #region File Read
            {
                String[] lines = new string[4];
                int i = 0;
                try
                {
                    System.IO.StreamReader file = new System.IO.StreamReader("dane.txt");
                    while ((lines[i] = file.ReadLine()) != null)
                    {
                        System.Console.WriteLine(lines[i]);
                        i++;
                    }

                    file.Close();
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e.Message);
                }

                globalData.H = Convert.ToDouble(lines[0].Remove(lines[0].Length - 1));
                globalData.B = Convert.ToDouble(lines[1].Remove(lines[1].Length - 1));
                globalData.nH = Convert.ToInt32((lines[2].Remove(lines[2].Length - 1)).Remove(lines[2].Length - 2));
                globalData.nB = Convert.ToInt32((lines[3].Remove(lines[3].Length - 1)).Remove(lines[3].Length - 2));
            }
            #endregion

            Grid grid = new Grid((globalData.nH) * (globalData.nB), (globalData.nH - 1) * (globalData.nB - 1));  //przypisuje ilość węzłów elementów w siatce
            grid.GenerateMesh(globalData);
            foreach (Element el in grid.NE)
            {
                el.PrepareData(globalData.k, globalData.c, globalData.ro, globalData.alpha, globalData.T);
                //Console.WriteLine(el.C[0, 0]);
            }

            int globalMatrixSize = globalData.nB * globalData.nH;
            double[,] H = new double[globalMatrixSize, globalMatrixSize];
            double[,] Hprim = new double[globalMatrixSize, globalMatrixSize];
            double[,] C = new double[globalMatrixSize, globalMatrixSize];
            double[,] P = new double[globalMatrixSize, 1];
            double[,] T0 = new double[grid.ND.Length, 1];
            double[,] T1 = new double[grid.ND.Length, 1];

            #region Agregacja
            //agregacja H:
            foreach (Element el in grid.NE)
            {
                for (int i = 0; i < el.H.GetLength(0); i++)
                {
                    for (int j = 0; j < el.H.GetLength(1); j++)
                    {
                        H[el.ID[i].index, el.ID[j].index] += el.H[i, j];
                        bool test = true;
                    }
                }
            }

            //agregacja C:
            foreach (Element el in grid.NE)
            {
                for (int i = 0; i < el.C.GetLength(0); i++)
                {
                    for (int j = 0; j < el.C.GetLength(1); j++)
                    {
                        C[el.ID[i].index, el.ID[j].index] += el.C[i, j];
                    }
                }
            }

            /*
            //agregacja P
            foreach (Element el in grid.NE)
            {
                for (int i = 0; i < el.P.GetLength(0); i++)
                {
                        P[el.ID[i].index, 0] += el.P[i, 0];
                }
            }
            */
            #endregion

            #region Wypisywanie macierzy
            //wypisz H:
            Console.WriteLine("Matrix H before simulation");
            for (int i = 0; i < globalMatrixSize; i++)
            {
                for (int j = 0; j < globalMatrixSize; j++)
                {
                    Console.Write(String.Format("{0:0.00}", H[i, j]) + " ");
                }
                Console.WriteLine();
            }
            //wypisz C:
            Console.WriteLine("Matrix C in iteration 0");
            for (int i = 0; i < globalMatrixSize; i++)
            {
                for (int j = 0; j < globalMatrixSize; j++)
                {
                    Console.Write(String.Format("{0:0.00}", C[i, j]) + " ");
                }
                Console.WriteLine();
            }

            #endregion


            

            double[,] tmp = new double[globalMatrixSize, globalMatrixSize];
            //Pętla po kroku czasowym/////////////////////////////////////////
            for (int time = 0; time < 10; time++)
            {

                //Obliczanie wektora T0
                for (int i = 0; i < grid.ND.Length; i++)
                {
                    T0[i, 0] = grid.ND[i].t;
                }

                P = new double[globalMatrixSize, 1];
                //Agregacja P:
                foreach (Element el in grid.NE)
                {
                    el.Calculate();
                    for (int i = 0; i < el.P.GetLength(0); i++)
                    {
                        P[el.ID[i].index, 0] += el.P[i, 0];
                        bool test = true;
                    }
                }
                

                //Generowanie H "z daszkiem":
                tmp = Matrix.Multiply(C, (1 / globalData.tau));
                Hprim = Matrix.Sum(tmp, H);

                //Generowanie P:
                //P *-1
                //P = Matrix.Multiply(P, -1);
                tmp = Matrix.Multiply(tmp, T0);
                P = Matrix.Sum(tmp, P);

                /*
                //Wypisywanie P:
                Console.WriteLine("Maacierz P:");
                for (int i = 0; i < P.GetLength(0); i++)
                {
                    Console.WriteLine(String.Format("{0:0.00}", P[i, 0]));
                }
                */

                //Solwer:
                var A = Matrix<double>.Build.DenseOfArray(Matrix.T(Hprim));
                double[] tmpP = new double[globalMatrixSize];
                for(int i = 0; i < tmpP.Length; i++)
                {
                    tmpP[i] = P[i, 0];
                }
                var b = Vector<double>.Build.Dense(tmpP);
                var x = A.Solve(b);

                for (int i = 0; i < x.Count; i++)
                {
                    grid.ND[i].t = x[i];
                }

                Console.WriteLine("Koniec kroku czasowego. Upłynęło " + ((time + 1) * globalData.tau) + " sekund");
                //Wypisywanie Temperatur:
                double maxTemperature = grid.ND[0].t;
                double minTemperature = grid.ND[0].t;
                Console.WriteLine("Temperatury (z solwera):");
                for (int i = 0; i < x.Count; i++)
                {
                    Console.WriteLine(String.Format("{0:0.00}", grid.ND[i].t));
                    if (grid.ND[i].t > maxTemperature)
                        maxTemperature = grid.ND[i].t;
                    if (grid.ND[i].t < minTemperature)
                        minTemperature = grid.ND[i].t;
                }
                Console.WriteLine("Najniższa temperatura: " + minTemperature);
                Console.WriteLine("Najwyższa temperatura: " + maxTemperature);

                WriteStateToFile(grid);
                Console.ReadLine();
            }
            
            Console.WriteLine("Koniec symulacji!");

            Console.ReadLine();
        }

        private static void WriteStateToFile(Grid grid)
        {
            List<string> lines = new List<string>();
            lines.Add("# vtk DataFile Version 2.0");
            lines.Add("Nagrzewanie belki testowej");
            lines.Add("ASCII");
            lines.Add("DATASET UNSTRUCTURED_GRID");
            lines.Add("POINTS "+grid.ND.Length+" float");
            foreach(Node n in grid.ND)
            {
                lines.Add(n.x.ToString().Replace(",", ".") + " " + n.y.ToString().Replace(",", ".") + " " + "0");
            }
            lines.Add("CELLS "+grid.NE.Length+" "+ grid.NE.Length*5);
            foreach(Element e in grid.NE)
            {
                lines.Add("4 " + e.ID[0].index + " " + e.ID[1].index + " " + e.ID[2].index + " " + e.ID[3].index);
            }
            lines.Add("CELL_TYPES " + grid.NE.Length);
            foreach (Element e in grid.NE)
            {
                lines.Add("9"); //z dokumentacji - trzeba sprawdzić
            }
            lines.Add("POINT_DATA " + grid.ND.Length);
            lines.Add("SCALARS Temperatura float 1");
            lines.Add("LOOKUP_TABLE default");
            foreach(Node n in grid.ND)
            {
                lines.Add(n.t.ToString().Replace(",", "."));
            }

            ///////zapis
            string[] linesArray = lines.ToArray();
            System.IO.File.WriteAllLines("plik.vtu", linesArray);
            Console.WriteLine("Zapisano stan symulacji");
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    }
                    
                    globalData.H = Convert.ToDouble(lines[0].Remove(lines[0].Length - 1));
                    globalData.B = Convert.ToDouble(lines[1].Remove(lines[1].Length - 1));
                    globalData.nH = Convert.ToInt32((lines[2].Remove(lines[2].Length - 1)).Remove(lines[2].Length - 2));
                    globalData.nB = Convert.ToInt32((lines[3].Remove(lines[3].Length - 1)).Remove(lines[3].Length - 2));
            }
            #endregion

            Grid grid = new Grid((globalData.nH+1)*(globalData.nB+1),globalData.nH*globalData.nH);  //przypisuje ilość węzłów elementów w siatce
            grid.GenerateMesh(globalData);
            foreach(Element el in grid.NE)
            {
                el.PrepareData();
            }

            Console.ReadLine();
        }
    }
}

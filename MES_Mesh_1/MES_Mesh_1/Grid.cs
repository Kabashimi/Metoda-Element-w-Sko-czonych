using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_Mesh_1
{
    class Grid
    {
        public int nh;
        public int ne;
        public Node[] ND;
        public Element[] NE;

        public Grid(int h, int e)
        {
            nh = h;
            ne = e;
            ND = new Node[nh];
            NE = new Element[ne];
        }

        public void GenerateMesh(GlobalData globalData)
        {
            bool status  =false;
            double Temperture = 0.0;
            for(int i = 0; i < globalData.nB; i++)
            {
                if (i == 0 || i == globalData.nB - 1) //warunek brzegpwy -> ogrzewana jest prawa i lewa ściana
                    status = true;
                for(int j = 0; j < globalData.nH; j++)
                { 
                    if (j == 0 || j == globalData.nH - 1) //warunek brzegpwy -> ogrzewana jest górna i dolna ściana
                        status = true;
                
                    ND[(i * globalData.nB) + j] = new Node(globalData.B / globalData.nB, globalData.H / globalData.nH, Temperture, status); //ustawiam węzły w tablicy
                    status = false;
                }
                status = false;
            }
            Console.WriteLine("Ustawiono węzły");

            for(int i = 0; i < globalData.nB-1; i++)
            {
                for(int j = 0; j < globalData.nH-1; j++)
                {
                    int index = (i * globalData.nB) + j;
                    NE[index] = new Element();
                    NE[index].ID[0] = ND[index];
                    NE[index].ID[1] = ND[index+1];
                    NE[index].ID[2] = ND[index+globalData.nH];
                    NE[index].ID[3] = ND[index+globalData.nH+1];
                }
            }
            Console.WriteLine("Ustawiono elementy");
        }
    }
}

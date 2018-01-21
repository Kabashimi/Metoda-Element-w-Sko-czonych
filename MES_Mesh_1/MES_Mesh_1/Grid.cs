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

        public Grid()
        {
            //konstruktor siatki dla szyny 60E1 (uproszczonej)
            nh = 50 + 45 + 63;
            ne = 40 + 32 + 40;
            ND = new Node[nh];
            NE = new Element[ne];
        }

        public void GenerateSplintMesh(GlobalData globalData)
        {
            bool status = false;
            double Temperture = globalData.T0;
            double elementSize = 7.5;
            double x = 0;
            double y = 0;
            int index = 0;
            //stopka
            for(int i = 0; i < 3; i++)
            {
                for(int j = 0; j < 21; i++)
                {
                    index = j + (i * 21);
                    x = i * elementSize;
                    y = j * elementSize;
                    ND[index] = new Node(x, y, Temperture, status, index);
                }
            }
            int previousIndex = index;
            double minimalY = 9 * elementSize;
            double minimalX = 3 * elementSize;
            //szyjka
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 3; i++)
                {
                    index = j + (i * 3)+previousIndex;
                    x = i * elementSize+minimalX;
                    y = j * elementSize+minimalY;
                    ND[index] = new Node(x, y, Temperture, status, index);
                }
            }
            //główka
            previousIndex = index;
            minimalY = 5 * elementSize;
            minimalX = 18 * elementSize;
            //szyjka
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 11; i++)
                {
                    index = j + (i * 3) + previousIndex;
                    x = i * elementSize + minimalX;
                    y = j * elementSize + minimalY;
                    ND[index] = new Node(x, y, Temperture, status, index);
                }
            }

            //////////////ELEMENTY
            //stopka
            index = 0;
            for(int i = 0; i < 2; i++)
            {
                for(int j = 0; j < 20; j++)
                {
                    index = j + (i * 20);
                    NE[index] = new Element();
                    NE[index].ID[0] = ND[j + (i * 21)];
                    NE[index].ID[3] = ND[j + (i * 21)+1];
                    NE[index].ID[1] = ND[j + (i * 21)+21];
                    NE[index].ID[2] = ND[j + (i * 21)+22];
                    if()
                }
            }
            //element 40
            NE[40] = new Element();
            NE[40].ID[0] = ND[51];
            NE[40].ID[3] = ND[52];
            NE[40].ID[1] = ND[63];
            NE[40].ID[2] = ND[64];
            //element 41
            NE[41] = new Element();
            NE[41].ID[0] = ND[52];
            NE[41].ID[3] = ND[53];
            NE[41].ID[1] = ND[64];
            NE[41].ID[2] = ND[65];

            //szyjnka
            previousIndex = 42;
            int minimalNode = 63;
            for(int i = 0; i < 16; i++)
            {
                for(int j = 0; j < 2; j++)
                {
                    //index = 
                }
            }
        }

        public void GenerateMesh(GlobalData globalData)
        {
            #region tworzenie węzłów
            bool status  =false;
            double Temperture = globalData.T0;
            for(int i = 0; i < globalData.nB; i++)
            {
                for(int j = 0; j < globalData.nH; j++)
                {
                    if (i == 0 || i == globalData.nB - 1) //warunek brzegpwy -> ogrzewana jest prawa i lewa ściana
                        status = true;
                    if (j == 0 || j == globalData.nH - 1) //warunek brzegpwy -> ogrzewana jest górna i dolna ściana
                        status = true;
                
                    ND[(i * globalData.nB) + j] = new Node((globalData.B / (globalData.nB-1))*i, (globalData.H / (globalData.nH-1))*j, Temperture, status, ((i * globalData.nB) + j)); //ustawiam węzły w tablicy
                    status = false;
                }
                status = false;
            }
            Console.WriteLine("Ustawiono węzły");
            #endregion
            #region tworzenie elementów
            for (int i = 0; i < globalData.nB-1; i++)
            {
                for(int j = 0; j < globalData.nH-1; j++)
                {
                    int index = (i * (globalData.nB-1)) + j;
                    int index2 = (i * (globalData.nB)) + j;
                    NE[index] = new Element();
                    NE[index].ID[0] = ND[index2];
                    NE[index].ID[3] = ND[index2+1];
                    NE[index].ID[1] = ND[index2+globalData.nH];
                    NE[index].ID[2] = ND[index2+globalData.nH+1];
                    if (j == 0)
                    {
                        NE[index].wall[1] = true;
                    }
                    if (j == globalData.nH - 2)
                    {
                        NE[index].wall[3] = true;
                    }
                    if (i == 0)
                    {
                        NE[index].wall[0] = true;
                    }
                    if (i == globalData.nB - 2)
                    {
                        NE[index].wall[2] = true;
                    }
                }
            }
            Console.WriteLine("Ustawiono elementy");
            #endregion
        }
    }
}

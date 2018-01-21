using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_Mesh_1
{
    class GlobalData
    {
        public double H;
        public double B;
        public int nH;
        public int nB;
        public double c;
        public double ro;
        public double tau;
        public double k;
        public double T;
        public double T0;
        public double alpha;

        public GlobalData(double H, double B, int nH, int nB)
        {
            this.H = H;
            this.B = B;
            this.nH = nH;
            this.nB = nB;
            c = 700;
            ro = 7800;
            tau = 50;
            k = 25;
            T0 = 100;
            T = 1200;
            alpha = 300;
        }
        public GlobalData()
        {
            c = 700;
            ro = 7800;
            tau = 50;
            k = 25;
            T0 = 100;
            T = 1200;
            alpha = 300;
        }

        public GlobalData(int type)
        {
            //konstruktor danych globalnych dla siatki szyny 60E1 ze stali R260
            if(type == 0)//nagrzewanie
            {
                c = 700;
                ro = 7800;
                tau = 50;
                k = 25;
                T0 = 20;
                T = 800;
                alpha = 300;
            }
            if(type == 1) //chłodzenie na powietrzu
            {
                c = 700;
                ro = 7800;
                tau = 50;
                k = 25;
                T0 = 800;
                T = 20;
                alpha = 300;
            }
        }

    }
}

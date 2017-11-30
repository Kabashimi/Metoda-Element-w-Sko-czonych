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

        public GlobalData(double H, double B, int nH, int nB)
        {
            this.H = H;
            this.B = B;
            this.nH = nH;
            this.nB = nB;
        }
        public GlobalData()
        {

        }
    }
}

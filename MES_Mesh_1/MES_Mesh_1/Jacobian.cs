using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_Mesh_1
{
    class Jacobian
    {
        public double[] J;
        public Jacobian(double[] j)
        {
            J = j;
        }
        public Jacobian()
        {
            J = new double[4];
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_Mesh_1
{
    class Node
    {
        public double x { get; set; }
        public double y { get; set; }
        public double t { get; set; }
        public Boolean status { get; set; }

        public Node(double X, double Y, double T, Boolean STATUS) {
            x = X;
            y = Y;
            t = T;
            status = STATUS;
        }
    }
}

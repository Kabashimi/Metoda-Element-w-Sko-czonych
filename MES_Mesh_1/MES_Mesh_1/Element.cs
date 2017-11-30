using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MES_Mesh_1
{
    class Element
    {
        public Node[] ID;
        public PC[] pc;
        public Jacobian[] Jacobians;
        public Jacobian[] JacobiansReversed;
        public double N1ksi(double x) { return -0.25*(1 - x); }
        public double N2ksi(double x) { return 0.25 * (1 - x); }
        public double N3ksi(double x) { return 0.25 * (1 + x); }
        public double N4ksi(double x) { return -0.25 * (1 + x); }

        public double N1eta(double x) { return -0.25 * (1 - x); }
        public double N2eta(double x) { return -0.25 * (1 + x); }
        public double N3eta(double x) { return 0.25 * (1 - x); }
        public double N4eta(double x) { return 0.25 * (1 + x); }


        public Element()
        {
            ID = new Node[4];
            pc = new PC[4] { new PC(-0.577, -0.577), new PC(0.577, -0.577), new PC(0.577, 0.577), new PC(-0.577, 0.577) };
            Jacobians = new Jacobian[4];
            JacobiansReversed = new Jacobian[4];
        }

        public Jacobian CalculateJakobian(int id)
        {
            double[] J = new double[4];
            J[0] = N1ksi(pc[id].ksi) * ID[0].x + N2ksi(pc[id].ksi) * ID[1].x + N3ksi(pc[id].ksi) * ID[2].x + N4ksi(pc[id].ksi) * ID[3].x; // macierz zapisana w postaci { x/ksi y/ksi x/eta y/eta
            J[1] = N1ksi(pc[id].ksi) * ID[0].y + N2ksi(pc[id].ksi) * ID[1].y + N3ksi(pc[id].ksi) * ID[2].y + N4ksi(pc[id].ksi) * ID[3].y;
            J[2] = N1ksi(pc[id].ksi) * ID[0].x + N2ksi(pc[id].ksi) * ID[1].x + N3ksi(pc[id].ksi) * ID[2].x + N4ksi(pc[id].ksi) * ID[3].x; 
            J[3] = N1ksi(pc[id].ksi) * ID[0].y + N2ksi(pc[id].ksi) * ID[1].y + N3ksi(pc[id].ksi) * ID[2].y + N4ksi(pc[id].ksi) * ID[3].y;
            return new Jacobian(J);
        }

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

        public void PrepareData()
        {
            for(int i = 0; i < 4; i++)
            {
                Jacobians[i] = CalculateJakobian(i);
                JacobiansReversed[i] = ReversJacobian(Jacobians[i]);
            }
        }
    }
}

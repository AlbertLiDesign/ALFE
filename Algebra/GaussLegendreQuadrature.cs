using System.Collections.Generic;

namespace ALFE
{
    public class GaussLegendreQuadrature
    {
        /// <summary>
        /// The number of sample points. Only 0 to 4 can be allowed;
        /// </summary>
        public int Num;

        /// <summary>
        /// The roots of the nth Legendre polynomial.
        /// </summary>
        public List<double> Xi = new List<double>();

        /// <summary>
        /// Quadrature weights.
        /// </summary>
        public List<double> Weights = new List<double>();

        public GaussLegendreQuadrature(int num)
        {
            Num = num;

            switch (num)
            {
                case 0:
                    Xi.Add(0.0);
                    Weights.Add(2.0);
                    break;
                case 1:
                    Xi.Add(-0.5773503);
                    Weights.Add(1.0);

                    Xi.Add(0.5773503);
                    Weights.Add(1.0);
                    break;
                case 2:
                    Xi.Add(-0.7745967);
                    Weights.Add(0.5555556);

                    Xi.Add(0.0);
                    Weights.Add(0.8888889);

                    Xi.Add(0.7745967);
                    Weights.Add(0.5555556);
                    break;
                case 3:
                    Xi.Add(-0.8611363);
                    Weights.Add(0.3478548);

                    Xi.Add(-0.3399810);
                    Weights.Add(0.6521452);

                    Xi.Add(0.8611363);
                    Weights.Add(0.3478548);

                    Xi.Add(0.3399810);
                    Weights.Add(0.6521452);
                    break;
                case 4:
                    Xi.Add(-0.9061798);
                    Weights.Add(0.2369269);

                    Xi.Add(-0.5384693);
                    Weights.Add(0.4786287);

                    Xi.Add(0.0);
                    Weights.Add(0.5688889);

                    Xi.Add(0.5384693);
                    Weights.Add(0.4786287);

                    Xi.Add(0.9061798);
                    Weights.Add(0.2369269);
                    break;
                default:
                    break;
            }
        }

    }
}

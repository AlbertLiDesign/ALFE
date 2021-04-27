namespace ALFE
{
    public class Material
    {
        /// <summary>
        /// // Young's modulus
        /// </summary>
        public double E { get; set; }

        /// <summary>
        /// Poisson ratio
        /// </summary>
        public double nu { get; set; }
        public Material()
        {
            this.E = 1.0;
            this.nu = 0.3f;
        }
        public Material(double E, double nu)
        {
            this.E = E;
            this.nu = nu;
        }
    }
}

namespace ALFE
{
    public class Material
    {
        /// <summary>
        /// // Young's modulus
        /// </summary>
        public float E { get; set; }

        /// <summary>
        /// Poisson ratio
        /// </summary>
        public float nu { get; set; }
        public Material() 
        {
            this.E = 1.0f;
            this.nu = 0.3f;
        }
        public Material(float E, float nu)
        {
            this.E = E;
            this.nu = nu;
        }
    }
}

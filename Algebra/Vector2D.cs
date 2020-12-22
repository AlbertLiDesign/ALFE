using System;

namespace ALFE
{
    public class Vector2D
    {
        /// <summary>
        /// Represents a vector in Euclidean space.
        /// </summary>
        internal double _x;
        internal double _y;

        public Vector2D() { }

        /// <summary>
        /// Constructs a new vector from 2 single precision numbers.
        /// </summary>
        /// <param name="x">X component of vector.</param>
        /// <param name="y">Y component of vector.</param>
        public Vector2D(double x, double y)
        {
            _x = x;
            _y = y;
        }

        /// <summary>
        /// Gets or sets the X (first) component of this vector.
        /// </summary>
        public double X { get { return _x; } set { _x = value; } }

        /// <summary>
        /// Gets or sets the Y (second) component of this vector.
        /// </summary>
        public double Y { get { return _y; } set { _y = value; } }

        /// <summary>
        /// Computes a hash number that represents the current vector.
        /// </summary>
        /// <returns>A hash code that is not unique for each vector.</returns>
        public override int GetHashCode()
        {
            // MSDN docs recommend XOR'ing the internal values to get a hash code
            return _x.GetHashCode() ^ _y.GetHashCode();
        }

        /// <summary>
        /// Sums up two vectors.
        /// </summary>
        /// <param name="v1">A vector.</param>
        /// <param name="v2">A second vector.</param>
        /// <returns>A new vector that results from the componentwise addition of the two vectors.</returns>
        public static Vector2D operator +(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1._x + v2._x, v1._y + v2._y);
        }

        /// <summary>
        /// Subtracts one vector from another.
        /// </summary>
        /// <param name="v1">A vector.</param>
        /// <param name="v2">A second vector.</param>
        /// <returns>The first vector minus the second vector</returns>
        public static Vector2D operator -(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1._x - v2._x, v1._y - v2._y);
        }

        /// <summary>
        /// Multiplies a vector by a number, having the effect of scaling it.
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="t">A number.</param>
        /// <returns>A new vector that is the original vector coordinatewise multiplied by t.</returns>
        public static Vector2D operator *(Vector2D vector, double t)
        {
            return new Vector2D(vector._x * t, vector._y * t);
        }

        /// <summary>
        /// Computes the dot product of two vectors.
        /// </summary>
        /// <param name="vector">A vector.</param>
        /// <param name="other">Another vector.</param>
        /// <returns>The result number of dot product.</returns>
        public static double operator *(Vector2D vector, Vector2D other)
        {
            return vector._x * other._x + vector._y * other._y;
        }

        /// <summary>
        /// Get the length of a vector
        /// </summary>        
        /// <returns>The length</returns>
        public double Length
        {
            get { return (double)Math.Sqrt(this._x * this._x + this._y * this._y); }
        }

        /// <summary>
        /// Get the square length of a vector
        /// </summary>        
        /// <returns>The length</returns>
        public double SqrLength
        {
            get { return this._x * this._x + this._y * this._y ; }
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", this._x, this._y);
        }

        /// <summary>
        /// Determines whether two vectors have equal values.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if the components of the two vectors are exactly equal; otherwise false.</returns>
        public static bool operator ==(Vector2D a, Vector2D b)
        {
            return (a._x == b._x && a._y == b._y);
        }

        /// <summary>
        /// Determines whether two vectors have different values.
        /// </summary>
        /// <param name="a">The first vector.</param>
        /// <param name="b">The second vector.</param>
        /// <returns>true if the two vectors differ in any component; false otherwise.</returns>
        public static bool operator !=(Vector2D a, Vector2D b)
        {
            return (a._x != b._x || a._y != b._y);
        }

        /// <summary>
        /// Determines whether the specified System.Object is a Vector2f and has the same values as the present vector.
        /// </summary>
        /// <param name="obj">The specified object.</param>
        /// <returns>true if obj is Vector2f and has the same components as this; otherwise false.</returns>
        public override bool Equals(object obj)
        {
            return (obj is Vector2D && this == (Vector2D)obj);
        }

        /// <summary>
        /// Determines whether the specified vector has the same values as the present vector.
        /// </summary>
        /// <param name="vector">The specified vector.</param>
        /// <returns>true if vector has the same components as this; otherwise false.</returns>
        public bool Equals(Vector2D vector)
        {
            return this == vector;
        }
        public double DistanceTo(Vector2D vector)
        {
            return (this - vector).Length;
        }
    }
}

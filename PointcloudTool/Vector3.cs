using System;
using System.Globalization;

namespace XYZSeparator {
    public struct Vector3 {
		public readonly double x;
		public readonly double y;
		public readonly double z;

		public double Length {
			get {
				return Math.Sqrt(Math.Pow(this.x, 2.0) + Math.Pow(this.y, 2.0) + Math.Pow(this.z, 2.0));
			}
		}

		public Vector3 Normalized {
			get {
				return this / this.Length;
			}
		}

		public Vector3(double x, double y, double z) {
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public override string ToString() {
			return string.Format(CultureInfo.InvariantCulture, "({0:0.00} {1:0.00} {2:0.00})", this.x, this.y, this.z);
		}

		public static Vector3 operator +(Vector3 c1, Vector3 c2) {
			return new Vector3(c1.x + c2.x, c1.y + c2.y, c1.z + c2.z);
		}
		
		public static Vector3 operator -(Vector3 c1, Vector3 c2) {
			return new Vector3(c1.x - c2.x, c1.y - c2.y, c1.z - c2.z);
		}

		public static Vector3 operator *(Vector3 v, double f) {
			return new Vector3(v.x * f, v.y * f, v.z * f);
		}

		public static Vector3 operator *(double f, Vector3 v) {
			return new Vector3(v.x * f, v.y * f, v.z * f);
		}

		public static Vector3 operator /(Vector3 v, double f) {
			return new Vector3(v.x / f, v.y / f, v.z / f);
		}

		public override bool Equals(object obj) {
			if (!(obj is Vector3)) {
				return false;
			}
			Vector3 v = (Vector3)obj;
			return v.x == this.x && v.y == this.y && v.z == this.z;
		}

		public static double Dot(Vector3 a, Vector3 b) {
			return a.x * b.x + a.y * b.y + a.z * b.z;
		}

		public static Vector3 Cross(Vector3 a, Vector3 b) {
			return new Vector3(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y + b.x);
		}
    }
}

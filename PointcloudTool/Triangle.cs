using System.Collections.Generic;

public class Triangle {
	public readonly Vector3 V1;
	public readonly Vector3 V2;
	public readonly Vector3 V3;

	public Vector3 Normal {
		get {
			var normal = Vector3.Cross(this.V2 - this.V1, this.V3 - this.V1);
			if (normal.y < 0) {
				return normal * -1f;
			} else {
				return normal;
			}
		}
	}

	public Triangle(Vector3 v1, Vector3 v2, Vector3 v3) {
		this.V1 = v3;
		this.V2 = v2;
		this.V3 = v1;
	}

	public bool HasZeroArea() {
		return this.V1.Equals(this.V2) || this.V2.Equals(this.V3) || this.V3.Equals(this.V1);
	}

	public IEnumerable<Vector3> ToEnumerable() {
		yield return this.V1;
		yield return this.V2;
		yield return this.V3;
	}

	public override string ToString() {
		return "Triangle: " + this.V1 + ", " + this.V2 + ", " + this.V3;
	}
}

using System.Collections.Generic;
using XYZSeparator;

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
		if (Vector3.Cross(v2 - v1, v3 - v1).y > 0) {
			this.V1 = v1;
			this.V2 = v2;
			this.V3 = v3;
		} else {
			this.V1 = v3;
			this.V2 = v2;
			this.V3 = v1;
		}
	}

	public bool HasZeroArea() {
		return this.V1.Equals(this.V2) || this.V2.Equals(this.V3) || this.V3.Equals(this.V1);
	}

	public IEnumerable<Vector3> ToEnumerable() {
		yield return this.V1;
		yield return this.V2;
		yield return this.V3;
	}

	private const float smallDistance = 0.01f;

	/*public Tuple<IEnumerable<Triangle>, IEnumerable<Triangle>> Split(Plane plane) {
		float[] distance = new float[] {
			plane.GetDistanceToPoint(this.V1),
			plane.GetDistanceToPoint(this.V2),
			plane.GetDistanceToPoint(this.V3)
		};

		int onPlane = distance.Count(d => Mathf.Abs(d) <= smallDistance);
		int abovePlane = distance.Count(d => Mathf.Abs(d) > smallDistance && d > 0);
		int belowPlane = distance.Count(d => Mathf.Abs(d) > smallDistance && d < 0);

		if (belowPlane == 0) {
			return new Tuple<IEnumerable<Triangle>, IEnumerable<Triangle>>(this.Yield(), Enumerable.Empty<Triangle>());
		}
		if (abovePlane == 0) {
			return new Tuple<IEnumerable<Triangle>, IEnumerable<Triangle>>(Enumerable.Empty<Triangle>(), this.Yield());
		}
		if (onPlane == 0) {
			var single = abovePlane == 1 ? this.ToEnumerable().First(v => plane.GetDistanceToPoint(v) > 0) : this.ToEnumerable().First(v => plane.GetDistanceToPoint(v) < 0);
			var double1 = abovePlane == 1 ? this.ToEnumerable().First(v => plane.GetDistanceToPoint(v) < 0) : this.ToEnumerable().First(v => plane.GetDistanceToPoint(v) > 0);
			var double2 = abovePlane == 1 ? this.ToEnumerable().Where(v => plane.GetDistanceToPoint(v) < 0).Skip(1).First() : this.ToEnumerable().Where(v => plane.GetDistanceToPoint(v) > 0).Skip(1).First();

			var intersect1 = Math3d.LinePlaneIntersection(plane, single, double1);
			var intersect2 = Math3d.LinePlaneIntersection(plane, single, double2);

			if (abovePlane == 1) {
				return new Tuple<IEnumerable<Triangle>, IEnumerable<Triangle>>(
					Triangle.TryCreateEnum(single, intersect1, intersect2),
					Triangle.TryCreateEnum(double1, double2, intersect1)
					.Concat(Triangle.TryCreateEnum(double2, intersect1, intersect2)));
			} else {
				return new Tuple<IEnumerable<Triangle>, IEnumerable<Triangle>>(
					Triangle.TryCreateEnum(double1, double2, intersect1)
					.Concat(Triangle.TryCreateEnum(double2, intersect1, intersect2)),
					Triangle.TryCreateEnum(single, intersect1, intersect2));
			}

		}
		if (onPlane == 1) {
			var vertexOn = this.ToEnumerable().First(v => Mathf.Abs(plane.GetDistanceToPoint(v)) <= smallDistance);
			var vertexBelow = this.ToEnumerable().First(v => plane.GetDistanceToPoint(v) < 0 && Mathf.Abs(plane.GetDistanceToPoint(v)) > smallDistance);
			var vertexAbove = this.ToEnumerable().First(v => plane.GetDistanceToPoint(v) > 0 && Mathf.Abs(plane.GetDistanceToPoint(v)) > smallDistance);
			var ray = new Ray(vertexAbove, vertexBelow - vertexAbove);
			float dst;
			plane.Raycast(ray, out dst);
			var intersect = ray.GetPoint(dst);

			return new Tuple<IEnumerable<Triangle>, IEnumerable<Triangle>>(Triangle.TryCreateEnum(vertexBelow, vertexOn, intersect), Triangle.TryCreateEnum(vertexAbove, vertexOn, intersect));
		}
		return new Tuple<IEnumerable<Triangle>, IEnumerable<Triangle>>(Enumerable.Empty<Triangle>(), Enumerable.Empty<Triangle>());
	}

	public static Tuple<IEnumerable<Triangle>, IEnumerable<Triangle>> SplitMesh(IEnumerable<Triangle> triangles, Plane plane) {
		var tuples = triangles.Select(t => t.Split(plane));
		return new Tuple<IEnumerable<Triangle>, IEnumerable<Triangle>>(tuples.SelectMany(t => t.Value1).NonNull(), tuples.SelectMany(t => t.Value2).NonNull());
	}

	public static IEnumerable<Triangle> CutMesh(IEnumerable<Triangle> triangles, Plane plane, bool keepAbove) {
		return triangles.SelectMany(t => keepAbove ? t.Split(plane).Value1 : t.Split(plane).Value2).NonNull();
	}*/

	public override string ToString() {
		return "Triangle: " + this.V1 + ", " + this.V2 + ", " + this.V3;
	}

	public double GetArea() {
		return Vector3.Cross(this.V2 - this.V1, this.V3 - this.V1).Length / 2.0;
	}
}

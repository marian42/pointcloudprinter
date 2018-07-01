using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public class SolidMeshCreator {
	private readonly double size;
	private readonly double zMargin;

	private Vector3 boundingLower;
	private Vector3 boundingUpper;

	public Triangle[] Triangles {
		get;
		private set;
	}

	private Vector3[] seam;

	public SolidMeshCreator(Triangle[] triangles, double size, double zMargin) {
		this.Triangles = triangles;
		this.size = size;
		this.zMargin = zMargin;
		this.calculateBounds();
		this.seam = this.getSeam();
		this.createGeometry();
	}

	private void calculateBounds() {
		double minX = Double.PositiveInfinity;
		double maxX = Double.NegativeInfinity;
		double minY = Double.PositiveInfinity;
		double maxY = Double.NegativeInfinity;
		double minZ = Double.PositiveInfinity;
		double maxZ = Double.NegativeInfinity;

		foreach (var triangle in this.Triangles) {
			foreach (var vertex in triangle.ToEnumerable()) {
				if (vertex.x < minX) minX = vertex.x;
				if (vertex.x > maxX) maxX = vertex.x;
				if (vertex.y < minY) minY = vertex.y;
				if (vertex.y > maxY) maxY = vertex.y;
				if (vertex.z < minZ) minZ = vertex.z;
				if (vertex.z > maxZ) maxZ = vertex.z;
			}
		}

		this.boundingLower = new Vector3(minX, minY, minZ);
		this.boundingUpper = new Vector3(maxX, maxY, maxZ);
	}

	private Vector3[] getSeam() {
		var right = new List<Vector3>();
		var down = new List<Vector3>();
		var left = new List<Vector3>();
		var up = new List<Vector3>();

		double margin = 0.01;

		foreach (var triangle in this.Triangles) {
			foreach (var vertex in triangle.ToEnumerable()) {
				if (vertex.x > this.boundingUpper.x - margin) {
					right.Add(vertex);
				} else if (vertex.y < this.boundingLower.y + margin) {
					down.Add(vertex);
				} else if (vertex.x < this.boundingLower.x + margin) {
					left.Add(vertex);
				} else if (vertex.y > this.boundingUpper.y - margin) {
					up.Add(vertex);
				}
			}
		}

		return right.OrderByDescending(v => v.y)
			.Concat(down.OrderByDescending(v => v.x))
			.Concat(left.OrderBy(v => v.y))
			.Concat(up.OrderBy(v => v.x))
			.ToArray();
	}

	private void createGeometry() {
		var result = new List<Triangle>();

		double groundHeight = this.boundingLower.z - this.zMargin - 2;

		for (int i = 0; i < this.seam.Length; i++) {
			var a = this.seam[i];
			var b = this.seam[(i + 1) % this.seam.Length];
			var c = new Vector3(a.x, a.y, groundHeight);
			var d = new Vector3(b.x, b.y, groundHeight);

			result.Add(new Triangle(a, d, c));
			result.Add(new Triangle(a, b, d));
		}

		for (int i = 0; i < this.seam.Length / 2; i++) {
			var a = this.seam[i];
			var b = this.seam[i + 1];
			var c = this.seam[this.seam.Length - 1 - i];
			var d = this.seam[this.seam.Length - 2 - i];
			a = new Vector3(a.x, a.y, groundHeight);
			b = new Vector3(b.x, b.y, groundHeight);
			c = new Vector3(c.x, c.y, groundHeight);
			d = new Vector3(d.x, d.y, groundHeight);
				
			result.Add(new Triangle(c, a, d));
			result.Add(new Triangle(a, b, d));
		}

		this.Triangles = this.Triangles.Concat(result).ToArray();
	}

	public IEnumerable<Triangle> GetCube() {
		double zLower = this.boundingLower.z - this.zMargin;
		double zUpper = this.boundingUpper.z + 2;
		double halfSize = size / 2;

		var a = new Vector3(+halfSize, -halfSize, zUpper);
		var b = new Vector3(+halfSize, +halfSize, zUpper);
		var c = new Vector3(-halfSize, +halfSize, zUpper);
		var d = new Vector3(-halfSize, -halfSize, zUpper);

		var e = new Vector3(+halfSize, -halfSize, zLower);
		var f = new Vector3(+halfSize, +halfSize, zLower);
		var g = new Vector3(-halfSize, +halfSize, zLower);
		var h = new Vector3(-halfSize, -halfSize, zLower);

		// up
		yield return new Triangle(a, b, c);
		yield return new Triangle(a, c, d);

		// front
		yield return new Triangle(c, b, f);
		yield return new Triangle(c, f, g);


		// left
		yield return new Triangle(d, c, g);
		yield return new Triangle(d, g, h);


		// right
		yield return new Triangle(b, a, e);
		yield return new Triangle(b, e, f);


		// back
		yield return new Triangle(a, d, h);
		yield return new Triangle(a, h, e);


		// down
		yield return new Triangle(h, g, f);
		yield return new Triangle(h, f, e);
	}
}
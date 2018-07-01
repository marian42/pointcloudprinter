using System.Collections.Generic;
using System.Linq;
using System;

public class PointHashSet {

	private class Bucket {
		public readonly int X;
		public readonly int Z;

		public Bucket(int x, int z) {
			this.X = x;
			this.Z = z;
		}

		public override int GetHashCode() {
			return this.X * 10000 + this.Z;
		}

		public override bool Equals(object obj) {
			if (!(obj is Bucket)) {
				return false;
			}
			Bucket bucket = obj as Bucket;
			return bucket.X == this.X && bucket.Z == this.Z;
		}
	}

	public readonly double BucketSize;
	private readonly Dictionary<Bucket, HashSet<Vector3>> data;
	private Vector3[] points;

	private Dictionary<Bucket, double> heightMap;
	private Dictionary<Bucket, Vector3> normalMap;

	public PointHashSet(double bucketSize, Vector3[] points) {
		this.BucketSize = bucketSize;
		this.data = new Dictionary<Bucket, HashSet<Vector3>>();
		this.points = points;

		for (int i = 0; i < this.points.Length; i++) {
			this.add(this.points[i], i);
		}
		this.prepareHeightmap();
	}

	private Bucket getBucket(Vector3 point) {
		return new Bucket((int)Math.Floor(point.x / this.BucketSize), (int)Math.Floor(point.z / this.BucketSize));
	}

	private void add(Vector3 point, int index) {
		var bucket = this.getBucket(point);
		if (!this.data.ContainsKey(bucket)) {
			this.data[bucket] = new HashSet<Vector3>();
		}
		this.data[bucket].Add(point);
	}

	public IEnumerable<Vector3> GetPointsInRange(Vector3 point, double radius, bool strict) {
		var lowerCorner = this.getBucket(new Vector3(point.x - radius, point.y, point.z - radius));
		var upperCorner = this.getBucket(new Vector3(point.x + radius, point.y, point.z + radius));
		double radiusSquared = Math.Pow(radius, 2.0f);

		for (int x = lowerCorner.X; x <= upperCorner.X; x++) {
			for (int z = lowerCorner.Z; z <= upperCorner.Z; z++) {
				var bucket = new Bucket(x, z);
				if (!this.data.ContainsKey(bucket)) {
					continue;
				}
				foreach (var pointInRadius in this.data[bucket]) {
					if (!strict || Math.Pow(pointInRadius.x - point.x, 2.0f) + Math.Pow(pointInRadius.z - point.z, 2.0f) < radiusSquared) {
						yield return pointInRadius;
					}
				}
			}
		}
	}

	private void prepareHeightmap() {
		this.heightMap = new Dictionary<Bucket, double>();
		this.normalMap = new Dictionary<Bucket, Vector3>();
		foreach (var bucket in this.data.Keys) {
			this.heightMap[bucket] = this.data[bucket].Max(v => v.y);
		}

		foreach (var bucket in this.data.Keys) {
			var buckets = new Bucket[] {
				bucket,
				new Bucket(bucket.X, bucket.Z + 1),
				new Bucket(bucket.X + 1, bucket.Z + 1),
				new Bucket(bucket.X + 1, bucket.Z)
			};
			var points = buckets
				.Where(b => this.heightMap.ContainsKey(b))
				.Select(b => this.getHeightmapPoint(b))
				.ToArray();
			if (points.Length < 3) {
				continue;
			}
			var normal = GetPlaneNormal(points);
			if (normal.y < 0) {
				normal = normal * -1;
			}
			this.normalMap[bucket] = normal;
		}
	}

	private Vector3 getHeightmapPoint(Bucket bucket) {
		return new Vector3(((double)bucket.X + 0.5) * this.BucketSize, this.heightMap[bucket], ((double)bucket.Z + 0.5) * this.BucketSize);
	}

	public Vector3[] GetHeightMap() {
		return this.heightMap.Keys.Select(bucket => this.getHeightmapPoint(bucket)).ToArray();
	}

	public Vector3[] GetHeightMapNormals() {
		return this.heightMap.Keys.Select(bucket => this.normalMap.ContainsKey(bucket) ? this.normalMap[bucket] : new Vector3(0, 1, 0)).ToArray();
	}

	public static Vector3 GetPlaneNormal(Vector3[] points) {
		// http://www.ilikebigbits.com/blog/2015/3/2/plane-from-points
		var centroid = points.Aggregate(new Vector3(0, 0, 0), (a, b) => a + b) / points.Length;

		double xx = 0, xy = 0, xz = 0, yy = 0, yz = 0, zz = 0;

		foreach (var point in points) {
			var relative = point - centroid;
			xx += relative.x * relative.y;
			xy += relative.x * relative.y;
			xz += relative.x * relative.z;
			yy += relative.y * relative.y;
			yz += relative.y * relative.z;
			zz += relative.z * relative.z;
		}

		double detX = yy * zz - yz * yz;
		double detY = xx * zz - xz * xz;
		double detZ = xx * yy - xy * xy;
		double detMax = Math.Max(detX, Math.Max(detY, detZ));

		if (detMax == detX) {
			double a = (xz * yz - xy * zz) / detX;
			double b = (xy * yz - xz * yy) / detX;
			return new Vector3(1, a, b);
		} else if (detMax == detY) {
			double a = (yz * xz - xy * zz) / detY;
			double b = (xy * xz - yz * xx) / detY;
			return new Vector3(a, 1, b);
		} else {
			double a = (yz * xy - xz * yy) / detZ;
			double b = (xz * xy - yz * xx) / detZ;
			return new Vector3(a, b, 1);
		}
	}
}

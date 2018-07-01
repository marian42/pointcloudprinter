using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace XYZSeparator {
	class HoleFixer {
		private Vector3[] points;
		private PointHashSet hashSet;

		public HoleFixer(Vector3[] points) {
			this.points = points;
			this.hashSet = new PointHashSet(2.0, points);
		}

		public IEnumerable<Vector3> GetEdgePoints() {
			const double range = 1.0;
			for (int i = 0; i < this.points.Length; i++) {
				var point = this.points[i];
				var neighborhood = this.hashSet.GetPointsInRange(point, range, false).Where(p => (p - point).Length < range && (p - point).Length > 0.01).Select(p => p - point).ToArray();

				var sum = new Vector3(0,0,0);
				foreach (var neighbour in neighborhood) {
					sum += new Vector3(neighbour.x, 0, neighbour.z).Normalized;
				}
				sum /= neighborhood.Length;


				if (neighborhood.Length > 6 && sum.Length > 0.35) {
					yield return point;
				}

				if (i % 2000 == 0) {
					Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:0.00}% Finding edge points", ((double)i / this.points.Length * 100.0)));
				}
			}
			yield break;
		}

		public IEnumerable<Vector3> CreatePatches(Vector3[] edgePoints) {
			const double range = 1.6;
			for (int i = 0; i < edgePoints.Length; i++) {
				var point = edgePoints[i];
				var neighbourhood = this.hashSet.GetPointsInRange(point, range, true);
				var highest = neighbourhood.OrderByDescending(p => p.y).First();
				var best = neighbourhood.Where(p => highest.y - p.y < range / 2.0).OrderBy(p => Math.Pow(p.x - point.x, 2.0) + Math.Pow(p.z - point.z, 2.0)).First();

				if (i % 1000 == 0) {
					Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:0.00}% Creating patches", ((double)i / edgePoints.Length * 100.0)));
				}

				const double minDistance = 2;
				const double pointSpacing = 0.7;
				if (best.y > point.y && best.y - point.y > minDistance) {
					int count = (int)Math.Floor((best - point).Length / pointSpacing);

					for (int j = 1; j <= count; j++) {
						yield return point + (best - point) * ((double)j / count);
					}
				}
			}
		}
	}
}

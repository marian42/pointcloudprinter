using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace XYZSeparator {
	public class SquareExtractor {
		public readonly double CenterX;
		public readonly double CenterY;
		public readonly double halfSize;

		private Vector3 center;
		private double minDistance;

		private List<Vector3> points;

		public SquareExtractor() {
			this.CenterX = 380406.26472043;
			this.CenterY = 5688506.84004219;
			this.halfSize = 125;
			this.minDistance = Double.PositiveInfinity;
			this.points = new List<Vector3>();
		}

		private void handlePoint(Vector3 point) {
			if (Math.Abs(point.x - this.CenterX) > halfSize || Math.Abs(point.z - this.CenterY) > halfSize) {
				return;
			}

			this.points.Add(point);
			double distance = Math.Pow(point.x - this.CenterX, 2.0) + Math.Pow(point.z - this.CenterY, 2.0);
			if (distance < this.minDistance) {
				this.minDistance = distance;
				this.center = point;
			}
		}

		public void WriteFile(string filename) {
			using (StreamWriter sw = File.CreateText(filename)) {
				foreach (var point in this.points) {
					sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:0.00} {1:0.00} {2:0.00}", point.x - this.center.x, point.z - this.center.z, point.y - this.center.y));
				}
			}
		}

		public Vector3[] GetCenteredPoints() {
			return this.points.Select(p => new Vector3(p.x - this.center.x, p.y - this.center.y, p.z - this.center.z)).ToArray();
		}

		public void ProcessXYZFile(string filename, char separator) {
			foreach (var point in XYZLoader.LoadContinuous(filename, separator)) {
				this.handlePoint(point);
			}
		}

		public int Count {
			get {
				return this.points.Count;
			}
		}
	}
}

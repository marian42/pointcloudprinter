using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

public class SquareExtractor {
	public readonly double CenterX;
	public readonly double CenterY;
	public readonly double halfSize;

	private Vector3 center;
	private double minDistance;

	private List<Vector3> points;

	public SquareExtractor(double centerX, double centerY, double size) {
		this.CenterX = centerX;
		this.CenterY = centerY;
		this.halfSize = size / 2.0;
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

	public Vector3[] GetCenteredPoints() {
		return this.points.Select(p => new Vector3(p.x - this.center.x, p.y - this.center.y, p.z - this.center.z)).ToArray();
	}

	public void ProcessXYZFile(string filename, char separator) {
		foreach (var point in XYZFile.ReadContinuously(filename, separator)) {
			this.handlePoint(point);
		}
	}

	public int Count {
		get {
			return this.points.Count;
		}
	}
}
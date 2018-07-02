using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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

	private bool outOfRange(string filename) {
		if (!Regex.Match(filename, "dom1l-fp_32[0-9]*_[0-9]*_1_nw.xyz").Success) {
			return false;
		}
		
		var array = filename.Split('_');
		double x = int.Parse(array[1].Substring(2)) * 1000;
		double y = int.Parse(array[2]) * 1000;

		return !(this.CenterX + this.halfSize > x
			&& this.CenterX - halfSize < x + 1000
			&& this.CenterY + this.halfSize > y
			&& this.CenterY - halfSize < y + 1000);
	}

	public void ProcessXYZFile(FileInfo file, char separator) {
		if (this.outOfRange(file.Name)) {
			Console.WriteLine("Skipping " + file.Name + " since it contains an unrelated tile.");
			return;
		}

		foreach (var point in XYZFile.ReadContinuously(file.FullName, separator)) {
			this.handlePoint(point);
		}
	}

	public int Count {
		get {
			return this.points.Count;
		}
	}
}
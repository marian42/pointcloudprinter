using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

class XYZFileWriter : IDisposable {
	private readonly StreamWriter streamWriter;
	private readonly char separator;

	public XYZFileWriter(string filename, char separator = ' ', bool append = false) {
		this.streamWriter = new StreamWriter(filename, append);
		this.separator = separator;
	}

	public void Write(Vector3 point) {
		this.streamWriter.WriteLine(string.Format(CultureInfo.InvariantCulture,
			"{0:0.00}{3}{1:0.00}{3}{2:0.00}",
			point.x, point.z, point.y,
			this.separator));
	}

	public void Write(IEnumerable<Vector3> points) {
		foreach (var point in points) {
			this.Write(point);
		}
	}

	public void Write(Vector3 point, Vector3 normal) {
		this.streamWriter.WriteLine(string.Format(CultureInfo.InvariantCulture,
			"{0:0.00}{6}{1:0.00}{6}{2:0.00}{6}{3:0.00}{6}{4:0.00}{6}{5:0.00}",
			point.x, point.z, point.y,
			normal.x, normal.z, normal.y,
			this.separator));
	}

	public void Dispose() {
		this.streamWriter.Dispose();
	}

	public void Close() {
		this.streamWriter.Close();
	}
}

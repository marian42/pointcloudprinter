using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Globalization;

public class XYZFile {
	private const int batchSize = 1000;

	public static Vector3[] Read(string fileName, char separator = ' ') {
		return File.ReadAllLines(fileName).Select(line => XYZFile.parseLine(line, separator)).ToArray();
	}

	public static IEnumerable<Vector3> ReadContinuously(string fileName, char separator) {
		var filestream = new System.IO.FileStream(fileName,
										  System.IO.FileMode.Open,
										  System.IO.FileAccess.Read,
										  System.IO.FileShare.ReadWrite);
		var streamReader = new System.IO.StreamReader(filestream, System.Text.Encoding.UTF8, true, 128);

		while (true) {
			var batch = readBatch(streamReader);
			if (!batch.Any()) {
				yield break;
			}
			foreach (var line in batch) {
				yield return parseLine(line, separator);
			}
		}
		streamReader.Close();
		filestream.Close();
	}

	private static List<string> readBatch(StreamReader reader) {
		var result = new List<string>();
		for (int i = 0; i < batchSize; i++) {
			string line = reader.ReadLine();
			if (line == null) {
				break;
			}
			result.Add(line);
		}
		return result;
	}

	private static Vector3 parseLine(string line, char separator) {
		try {
			var points = line.Split(separator).Where(s => s.Any()).Select(s => double.Parse(s, CultureInfo.InvariantCulture)).ToArray();
			return new Vector3(points[0], points[2], points[1]);
		}
		catch (FormatException) {
			throw new Exception("Bad line: " + line);
		}
	}

	public static void Write(string outputFile, IEnumerable<Vector3> points, char separator = ' ') {
		using (StreamWriter sw = File.CreateText(outputFile)) {
			foreach (var point in points) {
				sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:0.00}{3}{1:0.00}{3}{2:0.00}", point.x, point.z, point.y, separator));
			}
		}
	}

	public static void Write(string outputFile, Vector3[] points, Vector3[] normals, char separator = ' ') {
		using (StreamWriter sw = File.CreateText(@"heightmap.xyz")) {
			for (int i = 0; i < points.Length; i++) {
				var point = points[i];
				var normal = normals[i];

				sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:0.00}{6}{1:0.00}{6}{2:0.00}{6}{3:0.00}{6}{4:0.00}{6}{5:0.00}", point.x, point.z, point.y, normal.x, normal.z, normal.y, separator));
			}
		}
	}
}

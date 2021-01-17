using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Globalization;

public class XYZFile {
	private const int batchSize = 1000;

	public static Vector3[] Read(string fileName) {
		return XYZFile.ReadContinuously(fileName).ToArray();
	}

	public static IEnumerable<Vector3> ReadContinuously(string fileName) {
		var filestream = new System.IO.FileStream(fileName,
										  System.IO.FileMode.Open,
										  System.IO.FileAccess.Read,
										  System.IO.FileShare.ReadWrite);
		var streamReader = new System.IO.StreamReader(filestream, System.Text.Encoding.UTF8, true, 128);

		char separator = ',';
		bool hasCheckedSeparator = false;

		while (true) {
			var batch = readBatch(streamReader);
			if (!batch.Any()) {
				yield break;
			}
			foreach (var line in batch) {
				if (!hasCheckedSeparator) {
					separator = line.Contains(",") ? ',' : ' ';
					hasCheckedSeparator = false;
				}
				yield return parseLine(line, separator);
			}
		}
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
		var fileWriter = new XYZFileWriter(outputFile, separator);
		fileWriter.Write(points);
		fileWriter.Close();
	}

	public static void Write(string outputFile, Vector3[] points, Vector3[] normals, char separator = ' ') {
		var fileWriter = new XYZFileWriter(outputFile, separator);
		for (int i = 0; i < points.Length; i++) {
			var point = points[i];
			var normal = normals[i];

			fileWriter.Write(point, normal);
		}
		fileWriter.Close();
	}
}

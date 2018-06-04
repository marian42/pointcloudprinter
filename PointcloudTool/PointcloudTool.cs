using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace XYZSeparator {
	public static class PointcloudTool {

		private static void preparePoints() {
			string inputFolder = @"data\";
			
			var converter = new Oware.LatLngUTMConverter("WGS 84");
			var coordinates = converter.convertLatLngToUtm(51.3349443, 7.2828901);

			var extractor = new SquareExtractor(coordinates.Easting, coordinates.Northing, 200);

			Console.WriteLine("Reading all .xyz files in " + inputFolder + "...");

			foreach (var file in new DirectoryInfo(inputFolder).GetFiles()) {
				Console.WriteLine(extractor.Count + "   " + file.Name);
				if (file.Extension != ".xyz") {
					continue;
				}
				extractor.ProcessXYZFile(file.FullName, ',');
			}

			var points = extractor.GetCenteredPoints();

			var holeFixer = new HoleFixer(points);

			var edgePoints = holeFixer.GetEdgePoints().ToArray();
			var patches = holeFixer.CreatePatches(edgePoints).ToArray();

			points = patches.Concat(points).ToArray();

			Console.WriteLine("Writing output file... ");
			string outputFile = @"pointcloud.xyz";

			using (StreamWriter sw = File.CreateText(outputFile)) {
				foreach (var point in points) {
					sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:0.00} {1:0.00} {2:0.00}", point.x, point.z, point.y));
				}
			}

			var pointHashSet = new PointHashSet(1d, points);

			var heightmap = pointHashSet.GetHeightMap();
			var heightmapNormals = pointHashSet.GetHeightMapNormals();

			using (StreamWriter sw = File.CreateText(@"heightmap.xyz")) {
				for (int i = 0; i < heightmap.Length; i++) {
					var point = heightmap[i];
					var normal = heightmapNormals[i];

					sw.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0:0.00} {1:0.00} {2:0.00} {3:0.00} {4:0.00} {5:0.00}", point.x, point.z, point.y, normal.x, normal.z, normal.y));
				}
			}

			Console.WriteLine("Complete.");
			Console.ReadLine();
		}

		public static void Main(string[] args) {
			preparePoints();
		}
	}
}

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
			XYZFile.Write("pointcloud.xyz", points);

			var pointHashSet = new PointHashSet(1d, points);
			XYZFile.Write("heightmap.xyz", pointHashSet.GetHeightMap(), pointHashSet.GetHeightMapNormals())

			Console.WriteLine("Complete.");
			Console.ReadLine();
		}

		public static void Main(string[] args) {
			//preparePoints();

			var mesh = BinarySTLReader.ReadFile("mesh.stl").ToArray();
			Console.WriteLine(mesh.Length + " triangles found.");

			foreach (var tri in mesh.Take(10)) {
				Console.WriteLine(tri);
			}

			Console.Read();
		}
	}
}

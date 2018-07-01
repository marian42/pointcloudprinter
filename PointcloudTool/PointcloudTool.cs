using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace XYZSeparator {
	public static class PointcloudTool {

		private static void extract(string inputFolder, string outputFile, double latitude, double longitude, string projection, double size) {
			var converter = new Oware.LatLngUTMConverter(projection);
			var coordinates = converter.convertLatLngToUtm(latitude, longitude);

			var extractor = new SquareExtractor(coordinates.Easting, coordinates.Northing, size);

			Console.WriteLine("Reading all .xyz files in " + inputFolder + "...");

			foreach (var file in new DirectoryInfo(inputFolder).GetFiles()) {
				Console.WriteLine(extractor.Count + "   " + file.Name);
				if (file.Extension != ".xyz") {
					continue;
				}
				extractor.ProcessXYZFile(file.FullName, ',');
			}

			var points = extractor.GetCenteredPoints();
			Console.WriteLine("Writing output file... ");
			XYZFile.Write(outputFile, points);
			Console.WriteLine("Complete.");
		}

		private static void fix(string inputFile, string outputFile, string heightmapFile) {
			Console.WriteLine("Reading input file... ");
			
			var points = XYZFile.Read(inputFile);

			Console.WriteLine("Fixing holes... ");
			var holeFixer = new HoleFixer(points);

			var edgePoints = holeFixer.GetEdgePoints().ToArray();
			var patches = holeFixer.CreatePatches(edgePoints).ToArray();

			points = patches.Concat(points).ToArray();

			Console.WriteLine("Writing output files... ");
			XYZFile.Write(outputFile, points);

			var pointHashSet = new PointHashSet(1d, points);
			XYZFile.Write(heightmapFile, pointHashSet.GetHeightMap(), pointHashSet.GetHeightMapNormals());

			Console.WriteLine("Complete.");
		}

		private static void makeSolid(string inputFile, string outputFile, string cubeFile, double size, double zMargin) {
			Console.WriteLine("Reading mesh...");
			var meshCreator = new SolidMeshCreator(STLFile.Read(inputFile).ToArray(), size, zMargin);

			Console.WriteLine("Writing...");
			STLFile.Write(outputFile, meshCreator.Triangles);

			STLFile.Write(cubeFile, meshCreator.GetCube().ToArray());

			Console.WriteLine("Complete.");
		}

		public static void Main(string[] args) {
			PointcloudTool.extract("data/", "pointcloud.xyz", 51.3349443, 7.2828901, "WGS 84", 40);
			PointcloudTool.fix("pointcloud.xyz", "pointcloud.xyz", "heightmap.xyz");
			PointcloudTool.makeSolid("mesh.stl", "mesh.stl", "cube.stl", 40, 10);
		}
	}
}

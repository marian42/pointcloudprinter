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

			var extractor = new SquareExtractor(coordinates.Easting, coordinates.Northing, 40);

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
			XYZFile.Write("heightmap.xyz", pointHashSet.GetHeightMap(), pointHashSet.GetHeightMapNormals());

			Console.WriteLine("Complete.");
		}

		private static void makeSolid() {
			Console.WriteLine("Reading mesh...");
			var meshCreator = new SolidMeshCreator(STLFile.Read("mesh.stl").ToArray(), 40, 10);

			Console.WriteLine("Writing...");
			STLFile.Write("mesh_solid.stl", meshCreator.Triangles);

			STLFile.Write("cube.stl", meshCreator.GetCube().ToArray());

			Console.WriteLine("Complete.");
		}

		public static void Main(string[] args) {
			PointcloudTool.makeSolid();
		}
	}
}

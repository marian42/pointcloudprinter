using System;
using System.Globalization;
using System.IO;
using System.Linq;

public static class PointcloudTool {

	private static void extract(string inputFolder, string outputFile, double latitude, double longitude, string projection, double size) {
		var converter = new Oware.LatLngUTMConverter(projection);
		var coordinates = converter.convertLatLngToUtm(latitude, longitude);

		var extractor = new SquareExtractor(coordinates.Easting, coordinates.Northing, size);

		Console.WriteLine("Latitude: " + latitude + ", Longitude: " + longitude);
		Console.WriteLine("Search coordinates converted to " + projection + ": " + coordinates.Northing + ", " + coordinates.Easting);
		Console.WriteLine("Reading all .xyz files in " + inputFolder + "...");

		foreach (var file in new DirectoryInfo(inputFolder).GetFiles()) {
			Console.WriteLine(extractor.Count + "   " + file.Name);
			if (file.Extension != ".xyz") {
				continue;
			}
			extractor.ProcessXYZFile(file);
		}

		var points = extractor.GetCenteredPoints();
		Console.WriteLine("Writing output file...");
		XYZFile.Write(outputFile, points);
		Console.WriteLine("Complete.");
	}

	private static void createPatches(Vector3[] points, string filename) {
		Console.WriteLine("Fixing holes...");
		var fileWriter = new XYZFileWriter(filename, append: true);
		var holeFixer = new HoleFixer(points);

		var edgePoints = holeFixer.GetEdgePoints();
		foreach (var point in holeFixer.CreatePatches(edgePoints)) {
			fileWriter.Write(point);
		}
		fileWriter.Close();
	}

	private static void fix(string filename, string heightmapFile) {
		Console.WriteLine("Reading input file...");		
		var points = XYZFile.Read(filename);

		PointcloudTool.createPatches(points, filename);

		Console.WriteLine("Creating heightmap...");
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

	private static void printUsage() {
		string name = System.AppDomain.CurrentDomain.FriendlyName;
		Console.WriteLine("This program can perform any one of three steps needed for mesh creation.");
		Console.WriteLine(name + " extract <datadirectory> <output .xyz file> <latitude> <longitude> <input projection name> <size of extraction square>");
		Console.WriteLine(name + " fix <input/output pointcloud .xyz file> <output heightmap .xyz file>");
		Console.WriteLine(name + " makeSolid <input .stl file> <output .stl file> <output cube .stl file> <size> <z margin>");
	}

	public static void Main(string[] args) {
		if (args.Length == 0) {
			printUsage();
			return;
		}
		try {
			if (args[0] == "extract") {
				if (args.Length != 7) {
					printUsage();
					return;
				}
				string inputFolder = args[1];
				string outputFile = args[2];
				double latitude = double.Parse(args[3], CultureInfo.InvariantCulture);
				double longitude = double.Parse(args[4], CultureInfo.InvariantCulture);
				string projection = args[5];
				double size = double.Parse(args[6], CultureInfo.InvariantCulture);

				PointcloudTool.extract(inputFolder, outputFile, latitude, longitude, projection, size);
			} else if (args[0] == "fix") {
				if (args.Length != 3) {
					printUsage();
					return;
				}

				string pointcloudFile = args[1];
				string heightmapFile = args[2];

				PointcloudTool.fix(pointcloudFile, heightmapFile);
			} else if (args[0].ToLower() == "makesolid") {
				if (args.Length != 6) {
					printUsage();
					return;
				}

				string inputFile = args[1];
				string outputFile = args[2];
				string cubeFile = args[3];
				double size = double.Parse(args[4], CultureInfo.InvariantCulture);
				double zMargin = double.Parse(args[5], CultureInfo.InvariantCulture);

				PointcloudTool.makeSolid(inputFile, outputFile, cubeFile, size, zMargin);
			} else {
				printUsage();
			}
		} catch (Exception exception) {
			Console.WriteLine(exception);
			Environment.Exit(1);
		}
	}
}
using System;
using System.Globalization;
using System.IO;
using System.Linq;

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

	private static void printUsage() {
		string name = System.AppDomain.CurrentDomain.FriendlyName;
		Console.WriteLine("This program can perform any one of three steps needed for mesh creation.");
		Console.WriteLine(name + " extract <datadirectory> <output .xyz file> <latitude> <longitude> <input projection name> <size of extraction square>");
		Console.WriteLine(name + " fix <inputfile .xyz> <output .xyz file> <output heightmap .xyz file>");
		Console.WriteLine(name + " makeSolid <input .stl file> <output .stl file> <output cube .stl file> <size> <z margin>");
	}

	public static void Main(string[] args) {
		if (args.Length == 0) {
			printUsage();
			return;
		}
		if (args[0] == "extract") {
			if (args.Length != 7) {
				printUsage();
				return;
			}
			string inputFolder = args[1];
			string outputFile = args[2];
			double latitude = double.Parse(args[3]);
			double longitude = double.Parse(args[4]);
			string projection = args[5];
			double size = double.Parse(args[6]);

			PointcloudTool.extract(inputFolder, outputFile, latitude, longitude, projection, size);
		} else if (args[0] == "fix") {
			if (args.Length != 4) {
				printUsage();
				return;
			}

			string inputFile = args[1];
			string outputFile = args[2];
			string heightmapFile = args[3];

			PointcloudTool.fix(inputFile, outputFile, heightmapFile);
		} else if (args[0].ToLower() == "makesolid") {
			if (args.Length != 6) {
				printUsage();
				return;
			}

			string inputFile = args[1];
			string outputFile = args[2];
			string cubeFile = args[3];
			double size = double.Parse(args[4]);
			double zMargin = double.Parse(args[5]);

			PointcloudTool.makeSolid(inputFile, outputFile, cubeFile, size, zMargin);
		} else {
			printUsage();
		}
	}
}
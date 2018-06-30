using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Globalization;

namespace XYZSeparator {
	public static class STLFile {

		public static IEnumerable<Triangle> Read(string filename) {
			var stream = new FileStream(filename, FileMode.Open);
			var reader = new BinaryReader(stream);

			reader.ReadBytes(80);
			uint count = reader.ReadUInt32();

			for (uint i = 0; i < count; i++) {
				reader.ReadBytes(4 * 3); // Normal
				var triangle = new Triangle(
					new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
					new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()),
					new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
				if (!triangle.HasZeroArea()) {
					yield return triangle;
					//Console.WriteLine(triangle.Normal + ", " + string.Format(CultureInfo.InvariantCulture, "{0:0.00}", triangle.Normal.Length) + " -- " + triangle.Flip().Normal + ", " + string.Format(CultureInfo.InvariantCulture, "{0:0.00}", triangle.Flip().Normal.Length));
				}
				
				reader.ReadBytes(2); // Attribute count
			}
		}

		public static void Write(string filename, Triangle[] mesh) {
			var stream = new FileStream(filename, FileMode.Create);
			var writer = new BinaryWriter(stream);

			writer.Write(Enumerable.Repeat((byte)0, 80).ToArray());
			writer.Write((uint)mesh.Length);

			foreach (var item in mesh) {
				writer.Write((float)item.Normal.x);
				writer.Write((float)item.Normal.y);
				writer.Write((float)item.Normal.z);
				writer.Write((float)item.V1.x);
				writer.Write((float)item.V1.y);
				writer.Write((float)item.V1.z);
				writer.Write((float)item.V2.x);
				writer.Write((float)item.V2.y);
				writer.Write((float)item.V2.z);
				writer.Write((float)item.V3.x);
				writer.Write((float)item.V3.y);
				writer.Write((float)item.V3.z);
				writer.Write(Enumerable.Repeat((byte)0, 2).ToArray());
			}
		}
	}
}

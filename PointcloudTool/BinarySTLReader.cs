using System.Collections.Generic;
using System.IO;

namespace XYZSeparator {
	public static class BinarySTLReader {

		public static IEnumerable<Triangle> ReadFile(string filename) {
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
				}
				
				reader.ReadBytes(2); // Attribute count
			}

			yield break;
		}
	}
}

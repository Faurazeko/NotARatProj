using System.Text.Json;

namespace NarLib
{
	public static class Util
	{
		public static byte[] ToByteArray<T>(T obj, bool includeObjSize)
		{
			if (obj == null)
				return null;

			var bytes = new List<byte>();
			var json = JsonSerializer.Serialize(obj);

			using (var ms = new MemoryStream())
			{
				using (var bw = new BinaryWriter(ms, System.Text.Encoding.Unicode))
				{


					bw.Write(json);

					bytes.AddRange(ms.ToArray());

					ms.SetLength(0);

					if (includeObjSize)
					{
						var size = bytes.Count;
						bw.Write(size);
					}

					bytes.InsertRange(0, ms.ToArray());
				}
			}

			return bytes.ToArray();
		}

		public static T FromByteArray<T>(byte[] data)
		{
			if (data == null)
				return default(T);

			try
			{
				var ms = new MemoryStream(data);
				var br = new BinaryReader(ms, System.Text.Encoding.Unicode);
				var json = br.ReadString();

				var obj = JsonSerializer.Deserialize<T>(json);

				return obj;
			}
			catch
			{
				return default(T);
			}
		}


		//https://stackoverflow.com/questions/14488796/does-net-provide-an-easy-way-convert-bytes-to-kb-mb-gb-etc
		static public readonly string[] SizeSuffixes =
				   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
		static public string GetSizeString(Int64 value, int decimalPlaces = 1)
		{
			if (decimalPlaces < 0) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
			if (value < 0) { return "-" + GetSizeString(-value, decimalPlaces); }
			if (value == 0) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

			// mag is 0 for bytes, 1 for KB, 2, for MB, etc.
			int mag = (int)Math.Log(value, 1024);

			// 1L << (mag * 10) == 2 ^ (10 * mag) 
			// [i.e. the number of bytes in the unit corresponding to mag]
			decimal adjustedSize = (decimal)value / (1L << (mag * 10));

			// make adjustment when the value is large enough that
			// it would round up to 1000 or more
			if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
			{
				mag += 1;
				adjustedSize /= 1024;
			}

			return string.Format("{0:n" + decimalPlaces + "} {1}",
				adjustedSize,
				SizeSuffixes[mag]);
		}
	}
}

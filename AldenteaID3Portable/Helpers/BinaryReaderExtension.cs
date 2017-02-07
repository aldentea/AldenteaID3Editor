using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Aldentea.ID3Portable.Helpers
{
	// (0.2.1)
	public static class BinaryReaderExtension
	{
		static Encoding ASCII = Encoding.GetEncoding("ASCII");

		public static async Task<byte[]> ReadBytesAsync(this BinaryReader reader, int length)
		{
			reader.BaseStream.Seek(0, SeekOrigin.Begin);
			var buf = new List<byte>(length).ToArray();
			await reader.BaseStream.ReadAsync(buf, 0, length);
			return buf;
		}

		public static async Task<string> ReadStringAsync(this BinaryReader reader, int length)
		{
			var buf = await ReadBytesAsync(reader, length);
			return ASCII.GetString(buf, 0, length);

		}

		// リトルエンディアン(最下位バイトが手前に来る)として読み込んでいる。
		public static async Task<int> ReadInt32Async(this BinaryReader reader)
		{
			var buf = await ReadBytesAsync(reader, 4);
			return (buf[3] << 24) + (buf[2] << 16) + (buf[1] << 8) + buf[0];
		}

	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Aldentea.ID3Portable.RIFF
{

	// 03/10/2008 by aldente
	#region RIFFChunkクラス
	public class RIFFChunk : ListChunk
	{
		const string RIFF_chunk_name = "RIFF";

		// 03/10/2008 by aldente
		#region *コンストラクタ(RIFFChunk:1/2)
		public RIFFChunk(string data_type) : base(RIFF_chunk_name, data_type)
		{
		}
		#endregion

		// 03/12/2008 by aldente : readerをStreamからBinaryReaderに変更．
		// 03/10/2008 by aldente
		#region *コンストラクタ(RIFFChunk:2/2)
		public RIFFChunk(string data_type, BinaryReader reader) : this(data_type)
		{
			Byte[] buf = new byte[4];

			reader.Read(buf, 0, 4);
			if (ascii.GetString(buf, 0, 4) != RIFF_chunk_name)
			{
				// RIFF形式ぢゃない！
				throw new Exception("RIFF形式ぢゃないよ！");
			}
			//int chunk_data_size = ReadInt32(reader);
			int chunk_data_size = reader.ReadInt32();

			reader.Read(buf, 0, 4);
			string type_name = ascii.GetString(buf, 0, 4);
			// ※type_nameを検証？
			if (type_name != data_type)
			{
				throw new Exception(string.Format("データタイプが'{0}'ぢゃないよ！", data_type));
			}

			ReadBody(reader, chunk_data_size - 4);
		}
		#endregion

		// 03/10/2008 by aldente
		#region *コンストラクタ(RIFFChunk:2/2)
		public RIFFChunk(string data_type, BinaryReader reader, int chunk_data_size)
			: this(data_type)
		{
			ReadBody(reader, chunk_data_size - 4);
		}
		#endregion

		// 03/10/2008 by aldente
		public void WriteToFile(string dstFileName)
		{
			using (FileStream writer = new FileStream(dstFileName, FileMode.Create))
			{
				Write(writer);
			}
		}

		// (0.1.0)
		public async Task WriteToFileAsync(string dstFileName)
		{
			using (FileStream writer = new FileStream(dstFileName, FileMode.Create))
			{
				await WriteAsync(writer);
			}
		}

		// 03/10/2008 by aldente
		public static RIFFChunk ReadFromFile(string srcFileName, string type_name)
		{
			using (BinaryReader reader = new BinaryReader(new FileStream(srcFileName, FileMode.Open)))
			{
				return new RIFFChunk(type_name, reader);
			}
		}

		// 03/12/2008 by aldente : readerをStreamからBinaryReaderに変更．
		// 03/11/2008 by aldente
		public static RIFFChunk ReadFromFile(string srcFileName)
		{
			using (BinaryReader reader = new BinaryReader(new FileStream(srcFileName, FileMode.Open)))
			{
				byte[] buf = new byte[4];

				reader.Read(buf, 0, 4);
				if (ascii.GetString(buf, 0, 4) != RIFF_chunk_name)
				{
					// RIFF形式ぢゃない！
					throw new Exception("RIFF形式ぢゃないよ！");
				}
				//int chunk_data_size = ReadInt32(reader);
				int chunk_data_size = reader.ReadInt32();

				reader.Read(buf, 0, 4);
				string type_name = ascii.GetString(buf, 0, 4);
				// ※type_nameを検証？

				return new RIFFChunk(type_name, reader, chunk_data_size);
			}
		}
	}
	#endregion

}

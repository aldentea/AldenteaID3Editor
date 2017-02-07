using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace Aldentea.ID3Portable.RIFF
{

	// (0.2.1)
	using Helpers;

	// 03/07/2008 by aldente
	#region ListChunkクラス
	public class ListChunk : Chunk
	{
		const string list_chunk_name = "LIST";

		FOURCC data_type = new FOURCC();

		// 03/10/2008 by aldente
		#region *[override]Nameプロパティ
		public override string Name
		{
			get
			{
				return data_type.Value;
			}
			set
			{
				data_type.Value = value;
			}
		}
		#endregion

		List<Chunk> childs = new List<Chunk>();

		// 03/10/2008 by aldente
		#region *コンストラクタ(ListChunk:1/4)
		protected ListChunk(string type_name)
			: base(list_chunk_name)
		{
			data_type.Value = type_name;
		}

		// 03/11/2008 by aldente
		protected ListChunk(string chunk_id, string type_name)
			: base(chunk_id)
		{
			data_type.Value = type_name;
		}

		// 03/10/2008 by aldente
		public ListChunk(string type_name, BinaryReader reader, int data_size)
			: base(list_chunk_name, reader, data_size)
		{
			data_type.Value = type_name;
		}

		// 03/11/2008 by aldente
		public ListChunk(string chunk_id, string type_name, BinaryReader reader, int data_size)
			: base(chunk_id, reader, data_size)
		{
			data_type.Value = type_name;
		}
		#endregion




		// 03/10/2008 by aldente
		#region *子チャンクを追加(AddChild)
		/// <summary>
		/// 子チャンクを追加します．
		/// 追加後の子チャンクの数が返ります．
		/// </summary>
		/// <param name="new_child_chunk"></param>
		/// <returns></returns>
		public int AddChild(Chunk new_child_chunk)
		{
			childs.Add(new_child_chunk);
			return childs.Count;
		}
		#endregion

		// 03/10/2008 by aldente
		#region *子チャンクを検索(FindChunk)
		/// <summary>
		/// 識別子を指定してチャンクを検索します．
		/// 同じ識別子を持つチャンクが複数ある場合は，どれが返るかわかりません．
		/// 1つもなければnullを返します．
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public Chunk FindChunk(string id)
		{
			Chunk result = null;
			foreach (Chunk child_chunk in childs)
			{
				if (child_chunk.Name == id)
				{
					result = child_chunk;
					break;
				}
			}
			return result;
		}
		#endregion

		#region abstract実装

		// 03/07/2008 by aldente
		#region *[override]データサイズを取得(GetDataSize)
		/// <summary>
		/// データ部分のサイズを取得します．
		/// </summary>
		/// <returns></returns>
		public override int GetDataSize()
		{
			int data_size = 4;
			foreach (Chunk child in childs)
			{
				data_size += child.GetSize();
			}
			return data_size;
		}
		#endregion

		// 03/07/2008 by aldente
		#region *[override]データ部分のバイト列を取得(GetDataBytes)
		/// <summary>
		/// データ部分をバイト列として取得します．
		/// </summary>
		/// <returns></returns>
		public override byte[] GetDataBytes()
		{
			byte[] buf = new byte[GetDataSize()];
			int pos = 0;

			byte[] data_type_bytes = data_type.GetBytes();
			data_type_bytes.CopyTo(buf, pos);
			pos += data_type_bytes.Length;

			foreach (Chunk child in childs)
			{
				child.GetBytes().CopyTo(buf, pos);
				pos += child.GetSize();
			}
			return buf;
		}
		#endregion

		// 03/12/2008 by aldente : readerをBinaryReaderに変更．
		// 03/10/2008 by aldente
		#region *[override]本体を読み込み(ReadBody)
		protected override void ReadBody(BinaryReader reader, int size)
		{
			long end_of_chunk = reader.BaseStream.Position + size;

			// タイプを読み込む．
			byte[] buf = new byte[4];
			//reader.Read(buf, 0, 4);
			//this.data_type.Value = Encoding.ASCII.GetString(buf);
			// ※data_typeの検証を入れる．

			while (reader.BaseStream.Position < end_of_chunk)
			{
				Chunk new_child_chunk = null;
			
				reader.Read(buf, 0, 4);
				string chunk_id = ascii.GetString(buf, 0, 4);
				int chunk_data_size = reader.ReadInt32();

				if (chunk_id == list_chunk_name)
				{
					reader.Read(buf, 0, 4);
					string type_name = ascii.GetString(buf, 0, 4);
					// タイプ名によって生成するチャンク型を決定．
					TypeInfo new_chunk_type = GetListChunkType(type_name);
					if (new_chunk_type.IsSubclassOf(typeof(ListChunk)))
					{
						//new_child_chunk = (Chunk)new_chunk_type.GetConstructor(new Type[] { typeof(string), typeof(BinaryReader), typeof(int) }).Invoke(new object[] { type_name, reader, chunk_data_size - 4 });
						// ※手抜き！
						var constructor = new_chunk_type.DeclaredConstructors.First(c => c.GetParameters().Length == 3);
						new_child_chunk = (Chunk)constructor.Invoke(new object[] { chunk_id, reader, chunk_data_size });
					}
				}
				else
				{
					// 識別子によって生成するチャンク型を決定．
					TypeInfo new_chunk_type = GetChunkType(chunk_id);
					if (new_chunk_type.IsSubclassOf(typeof(Chunk)))
					{
						var constructor = new_chunk_type.DeclaredConstructors.First(c => c.GetParameters().Length == 3);
						new_child_chunk = (Chunk)constructor.Invoke(new object[] { chunk_id, reader, chunk_data_size });
					}
				}
				if (new_child_chunk != null)
				{
					AddChild(new_child_chunk);
				}
			}
		}
		#endregion

		// (0.2.1)
		public override async Task ReadBodyAsync(BinaryReader reader, int size)
		{
			long end_of_chunk = reader.BaseStream.Position + size;

			while (reader.BaseStream.Position < end_of_chunk)
			{
				Chunk new_child_chunk = null;

				string chunk_id = await reader.ReadStringAsync(4);
				int chunk_data_size = await reader.ReadInt32Async();

				if (chunk_id == list_chunk_name)
				{
					string type_name = await reader.ReadStringAsync(4);
					// タイプ名によって生成するチャンク型を決定．
					TypeInfo new_chunk_type = GetListChunkType(type_name);
					if (new_chunk_type.IsSubclassOf(typeof(ListChunk)))
					{
						var constructor = new_chunk_type.DeclaredConstructors.First(c => c.GetParameters().Length == 1);
						new_child_chunk = (Chunk)constructor.Invoke(new object[] { chunk_id });
						await new_child_chunk.ReadBodyAsync(reader, chunk_data_size);
					}
				}
				else
				{
					// 識別子によって生成するチャンク型を決定．
					TypeInfo new_chunk_type = GetChunkType(chunk_id);
					if (new_chunk_type.IsSubclassOf(typeof(Chunk)))
					{
						var constructor = new_chunk_type.DeclaredConstructors.First(c => c.GetParameters().Length == 1);
						new_child_chunk = (Chunk)constructor.Invoke(new object[] { chunk_id });
						await new_child_chunk.ReadBodyAsync(reader, chunk_data_size);
					}
				}
				if (new_child_chunk != null)
				{
					AddChild(new_child_chunk);
				}
			}
		}

		#endregion


		// 03/10/2008 by aldente
		#region *[static]バイト列を整数値に変換(BytesToInt32)
		/// <summary>
		/// 4バイトのバイト配列を，リトルエンディアン32ビット整数値とみなして変換します．
		/// </summary>
		/// <param name="src">4バイト以上のバイト配列．最初の4バイトだけを見ます．</param>
		/// <returns></returns>
		private static Int32 BytesToInt32(Byte[] src)
		{
			if (src.Length < 4)
			{
				throw new ArgumentException("配列の長さが短すぎるYO！4バイト必要だYO！");
			}
			Int32 ret = 0;
			for (int i = 0; i < 4; i++)
			{
				ret += (Int32)src[i] << (8 * i);
			}
			return ret;
		}
		#endregion

		protected virtual TypeInfo GetChunkType(string id)
		{
			return typeof(BinaryChunk).GetTypeInfo();
		}

		protected virtual TypeInfo GetListChunkType(string type_name)
		{
			return typeof(ListChunk).GetTypeInfo();
		}


	}
	#endregion
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace Aldentea.ID3Portable
{

	#region ID3Readerクラス
	public class ID3Reader : BinaryReader
	{
		#region *コンストラクタ(ID3Reader)
		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		public ID3Reader(Stream input)
			: base(input)
		{
		}
		#endregion

		// 09/03/2013 by aldentea
		// ↓Read3ByteIntegerのコメントになっていたが，Read4ByteSynchsafeIntegerのコメントだと思うので移動．
		// 05/28/2007 by aldente : バグを修正(8を7に修正)．
		#region *4バイトSynchsafe整数を読み込み(Read4ByteSynchsafeInteger)
		/// <summary>
		/// 4バイトのSynchsafe整数を読み込みます．
		/// </summary>
		/// <returns></returns>
		public int Read4ByteSynchsafeInteger()
		{
			int digits = 4;
			byte[] buf = ReadBytes(digits);
			int value = 0;
			for (int n = 0; n < digits; n++)
			{
				value += buf[digits - 1 - n] << (7 * n);
			}
			return value;
		}
		#endregion

		// (0.2.0)
		public async Task<int> Read4ByteSynchsafeIntegerAsync()
		{
			const int digits = 4;
			byte[] buf = await ReadBytesAsync(digits);
			int value = 0;
			for (int n = 0; n < digits; n++)
			{
				value += buf[digits - 1 - n] << (7 * n);
			}
			return value;
		}


		#region *3バイト整数を読み込み(Read3ByteInteger)
		/// <summary>
		/// 3バイトのビッグエンディアン整数を読み込みます．
		/// </summary>
		/// <returns>読み込んだ整数値．</returns>
		public int Read3ByteInteger()
		{
			const int digits = 3;
			byte[] buf = ReadBytes(digits);
			int value = 0;
			for (int n = 0; n < digits; n++)
			{
				value += buf[digits - 1 - n] << (8 * n);
			}
			return value;
		}
		#endregion

		// (0.2.0)
		public async Task<int> Read3ByteIntegerAsync()
		{
			const int digits = 3;
			byte[] buf = await ReadBytesAsync(digits);
			int value = 0;
			for (int n = 0; n < digits; n++)
			{
				value += buf[digits - 1 - n] << (8 * n);
			}
			return value;
		}

		// (0.2.0)
		public async Task<byte[]> ReadBytesAsync(int length)
		{
			byte[] buf = new List<byte>(length).ToArray();
			// 非同期に読み取る方法がこれしかない。
			await BaseStream.ReadAsync(buf, 0, length);
			return buf;
		}

	}
	#endregion

}

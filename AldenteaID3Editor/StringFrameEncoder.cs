﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;


namespace Aldentea.ID3Editor
{
	#region StringFrameEncoderクラス
	public class StringFrameEncoder
	{
		ArrayList encodings = new ArrayList();

		#region *コンストラクタ(StringFrameEncoder)
		public StringFrameEncoder(bool use_sjis)
		{
			encodings.Add(use_sjis ? Encoding.GetEncoding("Shift_JIS") : Encoding.GetEncoding("Latin-1"));
			encodings.Add(Encoding.Unicode);
		}
		#endregion

		#region *バイト列を文字列に変換(Decode)
		public string Decode(byte[] body)
		{
			int offset = 1;
			Encoding encoding = (Encoding)encodings[body[0]];
			if (encoding == Encoding.Unicode && body[1] == 0xFF && body[2] == 0xFE)
			{
				// BOMをスキップ．
				offset = 3;
			}
			return encoding.GetString(body, offset, body.Length - offset).TrimEnd('\0');
		}
		#endregion

		// 11/25/2014 by aldentea : BOMの出力処理を追加．
		#region *文字列をバイト列に変換(Encode)
		public byte[] Encode(string value, Encoding encoding)
		{
			int index = encodings.IndexOf(encoding);
			if (index == -1)
			{
				throw new ArgumentException("その文字コードは取り扱っておりません．");
			}

			using (MemoryStream ms = new MemoryStream())
			{
				ms.WriteByte(Convert.ToByte(index));
				// この2行でBOMの出力を行う．
				byte[] bombuf = encoding.GetPreamble();
				ms.Write(bombuf, 0, bombuf.Length);

				byte[] buf = encoding.GetBytes(value + "\0"); // この時にBOMも出力される．←これは嘘．
				ms.Write(buf, 0, buf.Length);
				return ms.ToArray();
			}
		}
		#endregion

	}
	#endregion
}

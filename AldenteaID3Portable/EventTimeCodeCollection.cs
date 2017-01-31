﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Aldentea.ID3Portable
{
	#region EventTimeCodeCollectionクラス
	public class EventTimeCodeCollection
	{
		//SortedList<int, byte> event_time_codes = new SortedList<int, byte>();
		Dictionary<int, byte> event_time_codes = new Dictionary<int, byte>();

		// ※ミリ秒単位の場合しかサポートしないということか！？
		private TimeUnit timeStampUnit = TimeUnit.Milliseconds;
		#region *TimeStampUnitプロパティ
		public TimeUnit TimeStampUnit
		{
			get
			{
				return timeStampUnit;
			}
			set
			{
				timeStampUnit = value;
			}
		}
		#endregion

		#region 単位変換関数

		// 06/14/2007 by aldente
		#region *値を受け入れる時に変換(ConvertIn)
		private int ConvertIn(decimal value)
		{
			if (value < 0.0M)
			{
				throw new ArgumentException("馬鹿者！範囲外だ！");
			}
			// 外から来た値をこっち側の値に変換．
			switch (TimeStampUnit)
			{
				case TimeUnit.Milliseconds:
					return Convert.ToInt32(Decimal.Truncate(value * 1000.0M));
				default:
					throw new Exception("ミリ秒以外には未対応ぢゃ！");
			}
		}
		#endregion

		// 06/14/2007 by aldente
		#region *値を外に出す時に変換(ConvertOut)
		private decimal ConvertOut(int value)
		{
			// こっちの値を外側での値に変換．
			switch (TimeStampUnit)
			{
				case TimeUnit.Milliseconds:
					return value / 1000.0M;
				default:
					throw new Exception("ミリ秒以外には未対応ぢゃ！");
			}
		}
		#endregion

		#endregion

		// 05/22/2007 by aldente
		#region *SabiPosプロパティ
		/// <summary>
		/// サビの開始位置を取得／設定します．単位は秒です．
		/// </summary>
		public decimal SabiPos
		{
			get
			{
				return GetTime(0x03);
			}
			set
			{
				// 範囲チェックはConvertInで一括して行う．
				UpdateEvent(0x03, value);
			}
		}
		#endregion

		// 06/18/2007 by aldente
		#region *StartPosプロパティ
		/// <summary>
		/// イントロの開始位置を取得／設定します．単位は秒です．
		/// </summary>
		public decimal StartPos
		{
			get
			{
				return GetTime(0x02);
			}
			set
			{
				// 範囲チェックはConvertInで一括して行う．
				UpdateEvent(0x02, value);
			}
		}
		#endregion

		// 06/18/2007 by aldente
		#region *StopPosプロパティ
		/// <summary>
		/// イントロの停止位置を取得／設定します．単位は秒です．
		/// </summary>
		public decimal StopPos
		{
			get
			{
				return GetTime(0x10);
			}
			set
			{
				// 範囲チェックはConvertInで一括して行う．
				UpdateEvent(0x10, value);
			}
		}
		#endregion

		// 06/18/2007 by aldente
		#region *Countプロパティ
		/// <summary>
		/// 登録されているイベントの数を返します．
		/// </summary>
		public int Count
		{
			get
			{
				return event_time_codes.Count;
			}
		}
		#endregion

		// 05/22/2007 by aldente
		#region *イベントの時刻を取得(GetTime)
		/// <summary>
		/// 指定したイベントの時刻(秒)を取得します．
		/// イベントが登録されていなければ，-1を返します．
		/// ※ミリ秒で登録されたもののみ対応．
		/// ※同種のイベントが複数登録されていても，今のところ最前のものだけを返します．
		/// </summary>
		/// <param name="type">イベントタイプのコード．</param>
		/// <returns>イベントの時刻(secには未換算)．</returns>
		public decimal GetTime(byte type)
		{
			foreach (KeyValuePair<int, byte> event_time in event_time_codes)
			{
				if (event_time.Value == type)
				{
					return ConvertOut(event_time.Key);
				}
			}
			return -1;
		}
		#endregion

		// 05/22/2007 by aldente
		#region *イベントの時刻を更新(UpdateEvent)
		/// <summary>
		/// イベントの時刻を更新します．
		/// </summary>
		/// <param name="type">イベントのタイプ．</param>
		/// <param name="time">イベントの時刻(秒単位)．</param>
		public void UpdateEvent(byte type, decimal time)
		{
			var old_time = GetTime(type);
			if (old_time > -1.0M)
			{
				// すでにイベントがある場合，それをいったん削除する．
				// ...ってまたループかよorz
				//int i = event_time_codes.IndexOfValue(type);  // O(n)ループだってさ．
				//if (i > -1)
				//{
				//	event_time_codes.RemoveAt(i);
				//}
				event_time_codes.Remove(ConvertIn(old_time));
			}
			AddEvent(type, time);
		}
		#endregion

		// 06/18/2007 by aldente : ※インチキ実装があるので要注意！
		#region *イベントを追加(AddEvent)
		/// <summary>
		/// イベントタイムコードフレームにイベントを追加します．
		/// </summary>
		/// <param name="type">イベントタイプ．</param>
		/// <param name="time">イベントの時刻(秒単位)．</param>
		public void AddEvent(byte type, decimal time)
		{
			int i_time = ConvertIn(time);
			// ※※※インチキ実装※※※
			// そもそもi_timeが0なら追加しない．
			if (i_time > 0)
			{
				// ※※※インチキ実装※※※
				// 同じ時刻のイベントが既に登録されていれば，
				// そのイベントを1カウント遅らせる．
				while (event_time_codes.ContainsKey(i_time))
				{
					i_time++;
				}
				event_time_codes.Add(i_time, type);
			}
		}
		#endregion

		// 06/14/2007 by aldente : 移籍にともない，読み込み範囲を縮小．
		#region *本体を読み込み(ReadBody)
		public void ReadBody(ID3Reader reader, int size)
		{
			while (size > 0)
			{
				byte type = reader.ReadByte();
				//int time = System.Net.IPAddress.NetworkToHostOrder(reader.ReadInt32());
				byte[] time_bytes = reader.ReadBytes(4);
				int time = (time_bytes[0] << 24) + (time_bytes[1] << 16) + (time_bytes[2] << 8) + time_bytes[3];
				AddEvent(type, ConvertOut(time));
				size -= 5;
			}
		}
		#endregion

		// 06/11/2007 by aldente : 移籍にともない，出力範囲を縮小．
		// 05/23/2007 by aldente : フレームサイズの出力を修正．
		#region *フレームをバイト列として出力(GetBytes)
		/// <summary>
		/// フレームをバイト列として出力します．
		/// </summary>
		/// <returns>フレーム全体のバイト列．</returns>
		public byte[] GetBytes()
		{
			using (MemoryStream ms = new MemoryStream())
			{
				foreach (KeyValuePair<int, byte> event_time in event_time_codes)
				{
					ms.WriteByte(event_time.Value);
					ms.Write(ToBytes(event_time.Key, timeStampUnit), 0, 4);
				}
				return ms.ToArray();
			}
		}
		#endregion

		// 11/10/2008 by aldente : TimeUnitHandlerクラス(廃止)から移動．
		#region *[static]バイト列に変換(ToBytes)
		/// <summary>
		/// 演奏位置(秒)をタイムスタンプフォーマット形式のバイト列に変換します．
		/// </summary>
		/// <param name="time">演奏位置(timeUnitに従った値)．</param>
		/// <param name="timeUnit">タイムスタンプフォーマット．TimeUnit.Millisecondsのみ対応．</param>
		/// <returns>変換後のbyte列．</returns>
		public static byte[] ToBytes(decimal time, TimeUnit timeUnit)
		{
			switch (timeUnit)
			{
				case TimeUnit.Frames:
					throw new NotImplementedException("MPEGフレーム単位のタイムスタンプフォーマットには未対応ナリ！");
				case TimeUnit.Milliseconds:
					int time_stamp = Convert.ToInt32(time);
					//return new System.Net.IPAddress(System.Net.IPAddress.HostToNetworkOrder(Convert.ToInt64(time_stamp) << 32)).GetAddressBytes();
					var b0 = (byte)(time_stamp >> 24);
					var b1 = (byte)(time_stamp << 8 >> 24);
					var b2 = (byte)(time_stamp << 16 >> 24);
					var b3 = (byte)(time_stamp % 256);
					return new byte[4] { b0, b1, b2, b3 };
			}
			// ここには来ない！
			return new byte[0];
		}
		#endregion

	}
	#endregion
}

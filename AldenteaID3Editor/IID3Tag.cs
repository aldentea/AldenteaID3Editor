﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldentea.ID3Editor
{

	#region IID3Tagインターフェイス
	public interface IID3Tag
	{
		string Title { get; set; }
		string Artist { get; set; }
		decimal SabiPos { get; set; }
		decimal StartPos { get; set; }
		decimal StopPos { get; set; }

		void WriteTo(string dstFilename);
		void Merge(IID3Tag another_tag);
	}
	#endregion

}

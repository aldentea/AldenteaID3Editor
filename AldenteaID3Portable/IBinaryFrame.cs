﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldentea.ID3Portable
{
	#region IBinaryFrameインターフェイス
	public interface IBinaryFrame
	{
		byte[] Content { get; set; }
	}
	#endregion
}

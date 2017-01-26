using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aldentea.ID3Portable
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

		// (0.1.0)
		Task WriteToAsync(string dstFileName);

		void Merge(IID3Tag another_tag);
	}
	#endregion

}

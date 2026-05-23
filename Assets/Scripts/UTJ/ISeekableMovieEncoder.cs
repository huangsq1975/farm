using System;
using UnityEngine;

namespace UTJ
{
	public interface ISeekableMovieEncoder : IMovieEncoder, IDisposable
	{
		bool Flush(string path, int beginFrame = 0, int endFrame = -1);

		int GetExpectedFileSize(int beginFrame = 0, int endFrame = -1);

		void GetFrameData(RenderTexture texture, int frame);
	}
}

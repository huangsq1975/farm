using System;

namespace UTJ
{
	public interface IEditableMovieEncoder : ISeekableMovieEncoder, IMovieEncoder, IDisposable
	{
		void EraseFrame(int beginFrame = 0, int endFrame = -1);
	}
}

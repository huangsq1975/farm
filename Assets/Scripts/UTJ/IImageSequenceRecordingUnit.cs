using System;
using UnityEngine;

namespace UTJ
{
	public interface IImageSequenceRecordingUnit : IDisposable
	{
		IImageSequenceEncoder Encoder
		{
			get;
		}

		Type EncoderType
		{
			get;
		}

		bool Recording
		{
			get;
		}

		Camera Camera
		{
			get;
			set;
		}

		Shader CopyShader
		{
			get;
			set;
		}

		void BeginRecording();

		void EndRecording();

		void Export(string path, int number);

		void ReleaseResources();
	}
	public interface IImageSequenceRecordingUnit<out T> : IImageSequenceRecordingUnit, IDisposable where T : IImageSequenceEncoder
	{
		new T Encoder
		{
			get;
		}
	}
}

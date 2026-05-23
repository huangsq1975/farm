using UnityEngine;

namespace UTJ
{
	[AddComponentMenu("UTJ/FrameCapturer/ExrRecorder")]
	[RequireComponent(typeof(Camera))]
	public class ExrRecorder : ImageSequenceRecorder<ExrEncoder>
	{
		protected override ImageSequenceRecordingUnit<ExrEncoder> CreateRecordingUnit()
		{
			ExrEncoder encoder = new ExrEncoder();
			string description = "ExrRecorder: Copy FrameBuffer";
			string gdescription = "ExrRecorder: Copy G-Buffer";
			return new ExrRecordingUnit(encoder, autoDisposeEncoder: true, description, gdescription);
		}
	}
}

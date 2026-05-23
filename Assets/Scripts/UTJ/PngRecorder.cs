using UnityEngine;

namespace UTJ
{
	[AddComponentMenu("UTJ/FrameCapturer/PngRecorder")]
	[RequireComponent(typeof(Camera))]
	public class PngRecorder : ImageSequenceRecorder<PngEncoder>
	{
		protected override ImageSequenceRecordingUnit<PngEncoder> CreateRecordingUnit()
		{
			PngEncoder encoder = new PngEncoder();
			string description = "PngRecorder: Copy FrameBuffer";
			string gdescription = "PngRecorder: Copy G-Buffer";
			return new PngRecordingUnit(encoder, autoDisposeEncoder: true, description, gdescription);
		}
	}
}

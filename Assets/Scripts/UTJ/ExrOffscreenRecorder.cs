using UnityEngine;

namespace UTJ
{
	[AddComponentMenu("UTJ/FrameCapturer/ExrOffscreenRecorder")]
	[RequireComponent(typeof(Camera))]
	public class ExrOffscreenRecorder : OffscreenImageSequenceRecorder<ExrEncoder>
	{
		protected override OffscreenImageSequenceRecordingUnit<ExrEncoder> CreateRecordingUnit()
		{
			ExrEncoder encoder = new ExrEncoder();
			string description = "ExrOffscreenRecorder: Copy";
			return new OffscreenImageSequenceRecordingUnit<ExrEncoder>(encoder, autoDisposeEncoder: true, description);
		}
	}
}

using UnityEngine;

namespace UTJ
{
	[AddComponentMenu("UTJ/FrameCapturer/PngOffscreenRecorder")]
	[RequireComponent(typeof(Camera))]
	public class PngOffscreenRecorder : OffscreenImageSequenceRecorder<PngEncoder>
	{
		protected override OffscreenImageSequenceRecordingUnit<PngEncoder> CreateRecordingUnit()
		{
			PngEncoder encoder = new PngEncoder();
			string description = "PngOffscreenRecorder: Copy";
			return new OffscreenImageSequenceRecordingUnit<PngEncoder>(encoder, autoDisposeEncoder: true, description);
		}
	}
}

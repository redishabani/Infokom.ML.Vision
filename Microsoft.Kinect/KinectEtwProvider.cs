//using System;
//using System.Diagnostics.Eventing;

//namespace Microsoft.Kinect
//{
//	/// <summary>
//	/// Managed wrapper for ETW code.
//	/// </summary>
//	internal static class KinectEtwProvider
//	{
//		/// <summary>
//		/// This is the core provider responsible for the reports.
//		/// </summary>
//		private static readonly EventProviderVersionTwo Provider;

//		/// <summary>
//		/// This is EventDescriptor for the GetSegmentationBegin event.
//		/// </summary>
//		private static EventDescriptor getSegmentationBegin;

//		/// <summary>
//		/// This is EventDescriptor for the GetSegmentationEnd event.
//		/// </summary>
//		private static EventDescriptor getSegmentationEnd;

//		/// <summary>
//		/// This is EventDescriptor for the ProcessInputBufferBegin event.
//		/// </summary>
//		private static EventDescriptor processInputBufferBegin;

//		/// <summary>
//		/// This is EventDescriptor for the ProcessInputBufferEnd event.
//		/// </summary>
//		private static EventDescriptor processInputBufferEnd;

//		/// <summary>
//		/// This is EventDescriptor for the NotifyFrameReadyStart event.
//		/// </summary>
//		private static EventDescriptor notifyFrameReadyStart;

//		/// <summary>
//		/// This is EventDescriptor for the NotifyFrameReadyEnd event.
//		/// </summary>
//		private static EventDescriptor notifyFrameReadyEnd;

//		/// <summary>
//		/// This is EventDescriptor for the ProcessFrameStart event.
//		/// </summary>
//		private static EventDescriptor processFrameStart;

//		/// <summary>
//		/// This is EventDescriptor for the ProcessFrameEnd event.
//		/// </summary>
//		private static EventDescriptor processFrameEnd;

//		/// <summary>
//		/// This is EventDescriptor for the GetSkeletonsBegin event.
//		/// </summary>
//		private static EventDescriptor getSkeletonsBegin;

//		/// <summary>
//		/// This is EventDescriptor for the GetSkeletonsEnd event.
//		/// </summary>
//		private static EventDescriptor getSkeletonsEnd;

//		/// <summary>
//		/// This is EventDescriptor for the UnpackBayerStart event.
//		/// </summary>
//		private static EventDescriptor unpackBayerStart;

//		/// <summary>
//		/// This is EventDescriptor for the UnpackBayerEnd event.
//		/// </summary>
//		private static EventDescriptor unpackBayerEnd;

//		/// <summary>
//		/// This is EventDescriptor for the FrameStart event.
//		/// </summary>
//		private static EventDescriptor frameStart;

//		/// <summary>
//		/// This is EventDescriptor for the FrameEnd event.
//		/// </summary>
//		private static EventDescriptor frameEnd;

//		/// <summary>
//		/// This is EventDescriptor for the MapDepthFrameToColorFrameStart event.
//		/// </summary>
//		private static EventDescriptor mapDepthFrameToColorFrameStart;

//		/// <summary>
//		/// This is EventDescriptor for the MapDepthFrameToColorFrameEnd event.
//		/// </summary>
//		private static EventDescriptor mapDepthFrameToColorFrameEnd;

//		/// <summary>
//		/// This is EventDescriptor for the MapDepthToColorImagePointStart event.
//		/// </summary>
//		private static EventDescriptor mapDepthToColorImagePointStart;

//		/// <summary>
//		/// This is EventDescriptor for the MapDepthToColorImagePointEnd event.
//		/// </summary>
//		private static EventDescriptor mapDepthToColorImagePointEnd;

//		/// <summary>
//		/// This is EventDescriptor for the MapSkeletonPointToColorStart event.
//		/// </summary>
//		private static EventDescriptor mapSkeletonPointToColorStart;

//		/// <summary>
//		/// This is EventDescriptor for the MapSkeletonPointToColorEnd event.
//		/// </summary>
//		private static EventDescriptor mapSkeletonPointToColorEnd;

//		/// <summary>
//		/// This is EventDescriptor for the MapSkeletonPointToDepthStart event.
//		/// </summary>
//		private static EventDescriptor mapSkeletonPointToDepthStart;

//		/// <summary>
//		/// This is EventDescriptor for the MapSkeletonPointToDepthEnd event.
//		/// </summary>
//		private static EventDescriptor mapSkeletonPointToDepthEnd;

//		/// <summary>
//		/// This is EventDescriptor for the MapDepthToSkeletonPointStart event.
//		/// </summary>
//		private static EventDescriptor mapDepthToSkeletonPointStart;

//		/// <summary>
//		/// This is EventDescriptor for the MapDepthToSkeletonPointEnd event.
//		/// </summary>
//		private static EventDescriptor mapDepthToSkeletonPointEnd;

//		/// <summary>
//		/// This is EventDescriptor for the ManagedNuiTransformDepthImageToSkeletonStart event.
//		/// </summary>
//		private static EventDescriptor managedNuiTransformDepthImageToSkeletonStart;

//		/// <summary>
//		/// This is EventDescriptor for the ManagedNuiTransformDepthImageToSkeletonEnd event.
//		/// </summary>
//		private static EventDescriptor managedNuiTransformDepthImageToSkeletonEnd;

//		/// <summary>
//		/// This is EventDescriptor for the ManagedNuiTransformSkeletonToDepthImageStart event.
//		/// </summary>
//		private static EventDescriptor managedNuiTransformSkeletonToDepthImageStart;

//		/// <summary>
//		/// This is EventDescriptor for the ManagedNuiTransformSkeletonToDepthImageEnd event.
//		/// </summary>
//		private static EventDescriptor managedNuiTransformSkeletonToDepthImageEnd;

//		/// <summary>
//		/// This is EventDescriptor for the NuiImageGetColorPixelCoordinatesFromDepthPixelAtResolutionStart event.
//		/// </summary>
//		private static EventDescriptor nuiImageGetColorPixelCoordinatesFromDepthPixelAtResolutionStart;

//		/// <summary>
//		/// This is EventDescriptor for the NuiImageGetColorPixelCoordinatesFromDepthPixelAtResolutionEnd event.
//		/// </summary>
//		private static EventDescriptor nuiImageGetColorPixelCoordinatesFromDepthPixelAtResolutionEnd;

//		/// <summary>
//		/// This is EventDescriptor for the NuiImageGetColorPixelCoordinateFrameFromDepthPixelFrameAtResolutionStart event.
//		/// </summary>
//		private static EventDescriptor nuiImageGetColorPixelCoordinateFrameFromDepthPixelFrameAtResolutionStart;

//		/// <summary>
//		/// This is EventDescriptor for the NuiImageGetColorPixelCoordinateFrameFromDepthPixelFrameAtResolutionEnd event.
//		/// </summary>
//		private static EventDescriptor nuiImageGetColorPixelCoordinateFrameFromDepthPixelFrameAtResolutionEnd;

//		/// <summary>
//		/// This is EventDescriptor for the NuiImageStreamOpenStart event.
//		/// </summary>
//		private static EventDescriptor nuiImageStreamOpenStart;

//		/// <summary>
//		/// This is EventDescriptor for the NuiImageStreamOpenEnd event.
//		/// </summary>
//		private static EventDescriptor nuiImageStreamOpenEnd;

//		/// <summary>
//		/// This is EventDescriptor for the NuiImageStreamGetNextFrameStart event.
//		/// </summary>
//		private static EventDescriptor nuiImageStreamGetNextFrameStart;

//		/// <summary>
//		/// This is EventDescriptor for the NuiImageStreamGetNextFrameEnd event.
//		/// </summary>
//		private static EventDescriptor nuiImageStreamGetNextFrameEnd;

//		/// <summary>
//		/// This is EventDescriptor for the NuiImageStreamReleaseFrameStart event.
//		/// </summary>
//		private static EventDescriptor nuiImageStreamReleaseFrameStart;

//		/// <summary>
//		/// This is EventDescriptor for the NuiImageStreamReleaseFrameEnd event.
//		/// </summary>
//		private static EventDescriptor nuiImageStreamReleaseFrameEnd;

//		/// <summary>
//		/// This is EventDescriptor for the NuiInitializeStart event.
//		/// </summary>
//		private static EventDescriptor nuiInitializeStart;

//		/// <summary>
//		/// This is EventDescriptor for the NuiInitializeEnd event.
//		/// </summary>
//		private static EventDescriptor nuiInitializeEnd;

//		/// <summary>
//		/// This is EventDescriptor for the AllFramesReady event.
//		/// </summary>
//		private static EventDescriptor allFramesReady;

//		/// <summary>
//		/// This is EventDescriptor for the ColorFrameReady event.
//		/// </summary>
//		private static EventDescriptor colorFrameReady;

//		/// <summary>
//		/// This is EventDescriptor for the DepthFrameReady event.
//		/// </summary>
//		private static EventDescriptor depthFrameReady;

//		/// <summary>
//		/// This is EventDescriptor for the SkeletonFrameReady event.
//		/// </summary>
//		private static EventDescriptor skeletonFrameReady;

//		/// <summary>
//		/// This is EventDescriptor for the StopDeviceStart event.
//		/// </summary>
//		private static EventDescriptor stopDeviceStart;

//		/// <summary>
//		/// This is EventDescriptor for the StopDeviceEnd event.
//		/// </summary>
//		private static EventDescriptor stopDeviceEnd;

//		/// <summary>
//		/// This is EventDescriptor for the SingleStreamStopStart event.
//		/// </summary>
//		private static EventDescriptor singleStreamStopStart;

//		/// <summary>
//		/// This is EventDescriptor for the SingleStreamStopEnd event.
//		/// </summary>
//		private static EventDescriptor singleStreamStopEnd;

//		/// <summary>
//		/// This is EventDescriptor for the DetectGenuineDeviceStart event.
//		/// </summary>
//		private static EventDescriptor detectGenuineDeviceStart;

//		/// <summary>
//		/// This is EventDescriptor for the DetectGenuineDeviceEnd event.
//		/// </summary>
//		private static EventDescriptor detectGenuineDeviceEnd;

//		/// <summary>
//		/// This is EventDescriptor for the CreateDeviceStart event.
//		/// </summary>
//		private static EventDescriptor createDeviceStart;

//		/// <summary>
//		/// This is EventDescriptor for the CreateDeviceEnd event.
//		/// </summary>
//		private static EventDescriptor createDeviceEnd;

//		/// <summary>
//		/// This is EventDescriptor for the ReEnumerateDevicesStart event.
//		/// </summary>
//		private static EventDescriptor reEnumerateDevicesStart;

//		/// <summary>
//		/// This is EventDescriptor for the ReEnumerateDevicesEnd event.
//		/// </summary>
//		private static EventDescriptor reEnumerateDevicesEnd;

//		/// <summary>
//		/// This is EventDescriptor for the OutputStreamInitStart event.
//		/// </summary>
//		private static EventDescriptor outputStreamInitStart;

//		/// <summary>
//		/// This is EventDescriptor for the OutputStreamInitEnd event.
//		/// </summary>
//		private static EventDescriptor outputStreamInitEnd;

//		/// <summary>
//		/// This is EventDescriptor for the SingleStreamStartStart event.
//		/// </summary>
//		private static EventDescriptor singleStreamStartStart;

//		/// <summary>
//		/// This is EventDescriptor for the SingleStreamStartEnd event.
//		/// </summary>
//		private static EventDescriptor singleStreamStartEnd;

//		/// <summary>
//		/// This is EventDescriptor for the SkeletalTrackerInitializeStart event.
//		/// </summary>
//		private static EventDescriptor skeletalTrackerInitializeStart;

//		/// <summary>
//		/// This is EventDescriptor for the SkeletalTrackerInitializeEnd event.
//		/// </summary>
//		private static EventDescriptor skeletalTrackerInitializeEnd;

//		/// <summary>
//		/// This is EventDescriptor for the KinectSensorInitializeStart event.
//		/// </summary>
//		private static EventDescriptor kinectSensorInitializeStart;

//		/// <summary>
//		/// This is EventDescriptor for the KinectSensorInitializeEnd event.
//		/// </summary>
//		private static EventDescriptor kinectSensorInitializeEnd;

//		/// <summary>
//		/// This is EventDescriptor for the SkeletonStageBGRStart event.
//		/// </summary>
//		private static EventDescriptor skeletonStageBGRStart;

//		/// <summary>
//		/// This is EventDescriptor for the SkeletonStageBGREnd event.
//		/// </summary>
//		private static EventDescriptor skeletonStageBGREnd;

//		/// <summary>
//		/// This is EventDescriptor for the SkeletonStageCDRPStart event.
//		/// </summary>
//		private static EventDescriptor skeletonStageCDRPStart;

//		/// <summary>
//		/// This is EventDescriptor for the SkeletonStageCDRPEnd event.
//		/// </summary>
//		private static EventDescriptor skeletonStageCDRPEnd;

//		/// <summary>
//		/// This is EventDescriptor for the SkeletonStageHeadDetectorStart event.
//		/// </summary>
//		private static EventDescriptor skeletonStageHeadDetectorStart;

//		/// <summary>
//		/// This is EventDescriptor for the SkeletonStageHeadDetectorEnd event.
//		/// </summary>
//		private static EventDescriptor skeletonStageHeadDetectorEnd;

//		/// <summary>
//		/// This is EventDescriptor for the SkeletonStageExemplarStart event.
//		/// </summary>
//		private static EventDescriptor skeletonStageExemplarStart;

//		/// <summary>
//		/// This is EventDescriptor for the SkeletonStageExemplarEnd event.
//		/// </summary>
//		private static EventDescriptor skeletonStageExemplarEnd;

//		/// <summary>
//		/// This is EventDescriptor for the SkeletonStageCentroidsStart event.
//		/// </summary>
//		private static EventDescriptor skeletonStageCentroidsStart;

//		/// <summary>
//		/// This is EventDescriptor for the SkeletonStageCentroidsEnd event.
//		/// </summary>
//		private static EventDescriptor skeletonStageCentroidsEnd;

//		/// <summary>
//		/// This is EventDescriptor for the SkeletonStageModelFittingStart event.
//		/// </summary>
//		private static EventDescriptor skeletonStageModelFittingStart;

//		/// <summary>
//		/// This is EventDescriptor for the SkeletonStageModelFittingEnd event.
//		/// </summary>
//		private static EventDescriptor skeletonStageModelFittingEnd;

//		/// <summary>
//		/// This is EventDescriptor for the FrameRequestDropped event.
//		/// </summary>
//		private static EventDescriptor frameRequestDropped;

//		/// <summary>
//		/// This is EventDescriptor for the UnexpectedFrameRequestError event.
//		/// </summary>
//		private static EventDescriptor unexpectedFrameRequestError;

//		/// <summary>
//		/// This is EventDescriptor for the FrameStateChanged event.
//		/// </summary>
//		private static EventDescriptor frameStateChanged;

//		/// <summary>
//		/// This is EventDescriptor for the FrameDropped event.
//		/// </summary>
//		private static EventDescriptor frameDropped;

//		/// <summary>
//		/// This is EventDescriptor for the NuiInitializeInfo event.
//		/// </summary>
//		private static EventDescriptor nuiInitializeInfo;

//		/// <summary>
//		/// This is EventDescriptor for the NuiImageStreamOpenInfo event.
//		/// </summary>
//		private static EventDescriptor nuiImageStreamOpenInfo;

//		/// <summary>
//		/// This is EventDescriptor for the NuiShutdownTrace event.
//		/// </summary>
//		private static EventDescriptor nuiShutdownTrace;

//		/// <summary>
//		/// This is EventDescriptor for the InternalNuiShutdownTrace event.
//		/// </summary>
//		private static EventDescriptor internalNuiShutdownTrace;

//		/// <summary>
//		/// This is EventDescriptor for the NuiSkeletonTrackingEnableTrace event.
//		/// </summary>
//		private static EventDescriptor nuiSkeletonTrackingEnableTrace;

//		/// <summary>
//		/// This is EventDescriptor for the NuiSkeletonTrackingDisableTrace event.
//		/// </summary>
//		private static EventDescriptor nuiSkeletonTrackingDisableTrace;

//		/// <summary>
//		/// This is EventDescriptor for the NuiTransformSmoothInfo event.
//		/// </summary>
//		private static EventDescriptor nuiTransformSmoothInfo;

//		/// <summary>
//		/// This is EventDescriptor for the NuiSkeletonSetTrackedSkeletonsInfo event.
//		/// </summary>
//		private static EventDescriptor nuiSkeletonSetTrackedSkeletonsInfo;

//		/// <summary>
//		/// This is EventDescriptor for the NuiSetFrameEndEventInfo event.
//		/// </summary>
//		private static EventDescriptor nuiSetFrameEndEventInfo;

//		/// <summary>
//		/// This is EventDescriptor for the NuiImageStreamGetNextFrameInfo event.
//		/// </summary>
//		private static EventDescriptor nuiImageStreamGetNextFrameInfo;

//		/// <summary>
//		/// This is EventDescriptor for the NuiSkeletonGetNextFrameInfo event.
//		/// </summary>
//		private static EventDescriptor nuiSkeletonGetNextFrameInfo;

//		/// <summary>
//		/// This is EventDescriptor for the NuiDeviceConnectionIdInfo event.
//		/// </summary>
//		private static EventDescriptor nuiDeviceConnectionIdInfo;

//		/// <summary>
//		/// This is EventDescriptor for the NuiUniqueIdInfo event.
//		/// </summary>
//		private static EventDescriptor nuiUniqueIdInfo;

//		/// <summary>
//		/// This is EventDescriptor for the NuiCreateSensorByIndexInfo event.
//		/// </summary>
//		private static EventDescriptor nuiCreateSensorByIndexInfo;

//		/// <summary>
//		/// This is EventDescriptor for the NuiCreateSensorByIdInfo event.
//		/// </summary>
//		private static EventDescriptor nuiCreateSensorByIdInfo;

//		/// <summary>
//		/// This is EventDescriptor for the NuiImageStreamSetImageFrameFlagsInfo event.
//		/// </summary>
//		private static EventDescriptor nuiImageStreamSetImageFrameFlagsInfo;

//		/// <summary>
//		/// This is EventDescriptor for the NuiAudioBeamGetBeamInfo event.
//		/// </summary>
//		private static EventDescriptor nuiAudioBeamGetBeamInfo;

//		/// <summary>
//		/// This is EventDescriptor for the NuiAudioBeamGetPositionInfo event.
//		/// </summary>
//		private static EventDescriptor nuiAudioBeamGetPositionInfo;

//		/// <summary>
//		/// This is EventDescriptor for the NuiAudioBeamSetBeamInfo event.
//		/// </summary>
//		private static EventDescriptor nuiAudioBeamSetBeamInfo;

//		/// <summary>
//		/// This is EventDescriptor for the NuiGetAudioSourceInfo event.
//		/// </summary>
//		private static EventDescriptor nuiGetAudioSourceInfo;

//		/// <summary>
//		/// This is EventDescriptor for the AudioDmoSetPropertyInfo event.
//		/// </summary>
//		private static EventDescriptor audioDmoSetPropertyInfo;

//		/// <summary>
//		/// This is EventDescriptor for the ManagedKinectSensorStartInfo event.
//		/// </summary>
//		private static EventDescriptor managedKinectSensorStartInfo;

//		/// <summary>
//		/// This is EventDescriptor for the ManagedImageStreamDisabledInfo event.
//		/// </summary>
//		private static EventDescriptor managedImageStreamDisabledInfo;

//		/// <summary>
//		/// This is EventDescriptor for the ManagedFrameReadyEventAddedInfo event.
//		/// </summary>
//		private static EventDescriptor managedFrameReadyEventAddedInfo;

//		/// <summary>
//		/// This is EventDescriptor for the ManagedFrameReadyEventRemovedInfo event.
//		/// </summary>
//		private static EventDescriptor managedFrameReadyEventRemovedInfo;

//		/// <summary>
//		/// This is EventDescriptor for the ManagedOpenNextFrameInfo event.
//		/// </summary>
//		private static EventDescriptor managedOpenNextFrameInfo;

//		/// <summary>
//		/// This is EventDescriptor for the ManagedOpenFrameFromEventInfo event.
//		/// </summary>
//		private static EventDescriptor managedOpenFrameFromEventInfo;

//		/// <summary>
//		/// This is EventDescriptor for the ManagedOpenFrameFromAllFramesEventInfo event.
//		/// </summary>
//		private static EventDescriptor managedOpenFrameFromAllFramesEventInfo;

//		/// <summary>
//		/// This is EventDescriptor for the ManagedAllFramesReadyEventAddedInfo event.
//		/// </summary>
//		private static EventDescriptor managedAllFramesReadyEventAddedInfo;

//		/// <summary>
//		/// This is EventDescriptor for the ManagedAllFramesReadyEventRemovedInfo event.
//		/// </summary>
//		private static EventDescriptor managedAllFramesReadyEventRemovedInfo;

//		/// <summary>
//		/// This is EventDescriptor for the MapColorFrameToDepthFrameStart event.
//		/// </summary>
//		private static EventDescriptor mapColorFrameToDepthFrameStart;

//		/// <summary>
//		/// This is EventDescriptor for the MapColorFrameToDepthFrameEnd event.
//		/// </summary>
//		private static EventDescriptor mapColorFrameToDepthFrameEnd;

//		/// <summary>
//		/// This is EventDescriptor for the MapColorFrameToDepthFrame_InterpolateGapsStart event.
//		/// </summary>
//		private static EventDescriptor mapColorFrameToDepthFrame_InterpolateGapsStart;

//		/// <summary>
//		/// This is EventDescriptor for the MapColorFrameToDepthFrame_InterpolateGapsEnd event.
//		/// </summary>
//		private static EventDescriptor mapColorFrameToDepthFrame_InterpolateGapsEnd;

//		/// <summary>
//		/// This is EventDescriptor for the MapColorFrameToSkeletonFrameStart event.
//		/// </summary>
//		private static EventDescriptor mapColorFrameToSkeletonFrameStart;

//		/// <summary>
//		/// This is EventDescriptor for the MapColorFrameToSkeletonFrameEnd event.
//		/// </summary>
//		private static EventDescriptor mapColorFrameToSkeletonFrameEnd;

//		/// <summary>
//		/// This is EventDescriptor for the MapDepthFrameToSkeletonFrameStart event.
//		/// </summary>
//		private static EventDescriptor mapDepthFrameToSkeletonFrameStart;

//		/// <summary>
//		/// This is EventDescriptor for the MapDepthFrameToSkeletonFrameEnd event.
//		/// </summary>
//		private static EventDescriptor mapDepthFrameToSkeletonFrameEnd;

//		/// <summary>
//		/// Initializes static members of the KinectEtwProvider class.
//		/// </summary>
//		static KinectEtwProvider()
//		{
//			Provider = new EventProviderVersionTwo(new Guid("53eb7889-836a-4f06-838e-0be45cca7f78"));
//			getSegmentationBegin = new EventDescriptor(0, 0, 16, 4, 1, 1, long.MinValue);
//			getSegmentationEnd = new EventDescriptor(1, 0, 16, 4, 2, 1, long.MinValue);
//			processInputBufferBegin = new EventDescriptor(2, 0, 16, 4, 1, 2, long.MinValue);
//			processInputBufferEnd = new EventDescriptor(3, 0, 16, 4, 2, 2, long.MinValue);
//			notifyFrameReadyStart = new EventDescriptor(4, 0, 16, 4, 1, 3, long.MinValue);
//			notifyFrameReadyEnd = new EventDescriptor(5, 0, 16, 4, 2, 3, long.MinValue);
//			processFrameStart = new EventDescriptor(6, 0, 16, 4, 1, 4, long.MinValue);
//			processFrameEnd = new EventDescriptor(7, 0, 16, 4, 2, 4, long.MinValue);
//			getSkeletonsBegin = new EventDescriptor(8, 0, 16, 4, 1, 5, long.MinValue);
//			getSkeletonsEnd = new EventDescriptor(9, 0, 16, 4, 2, 5, long.MinValue);
//			unpackBayerStart = new EventDescriptor(10, 0, 16, 4, 1, 6, long.MinValue);
//			unpackBayerEnd = new EventDescriptor(11, 0, 16, 4, 2, 6, long.MinValue);
//			frameStart = new EventDescriptor(12, 0, 16, 4, 1, 7, long.MinValue);
//			frameEnd = new EventDescriptor(13, 0, 16, 4, 2, 7, long.MinValue);
//			mapDepthFrameToColorFrameStart = new EventDescriptor(14, 0, 16, 4, 1, 8, long.MinValue);
//			mapDepthFrameToColorFrameEnd = new EventDescriptor(15, 0, 16, 4, 2, 8, long.MinValue);
//			mapDepthToColorImagePointStart = new EventDescriptor(16, 0, 16, 5, 1, 9, long.MinValue);
//			mapDepthToColorImagePointEnd = new EventDescriptor(17, 0, 16, 5, 2, 9, long.MinValue);
//			mapSkeletonPointToColorStart = new EventDescriptor(18, 0, 16, 5, 1, 10, long.MinValue);
//			mapSkeletonPointToColorEnd = new EventDescriptor(19, 0, 16, 5, 2, 10, long.MinValue);
//			mapSkeletonPointToDepthStart = new EventDescriptor(20, 0, 16, 5, 1, 11, long.MinValue);
//			mapSkeletonPointToDepthEnd = new EventDescriptor(21, 0, 16, 5, 2, 11, long.MinValue);
//			mapDepthToSkeletonPointStart = new EventDescriptor(22, 0, 16, 5, 1, 12, long.MinValue);
//			mapDepthToSkeletonPointEnd = new EventDescriptor(23, 0, 16, 5, 2, 12, long.MinValue);
//			managedNuiTransformDepthImageToSkeletonStart = new EventDescriptor(24, 0, 16, 5, 1, 13, long.MinValue);
//			managedNuiTransformDepthImageToSkeletonEnd = new EventDescriptor(25, 0, 16, 5, 2, 13, long.MinValue);
//			managedNuiTransformSkeletonToDepthImageStart = new EventDescriptor(26, 0, 16, 5, 1, 14, long.MinValue);
//			managedNuiTransformSkeletonToDepthImageEnd = new EventDescriptor(27, 0, 16, 5, 2, 14, long.MinValue);
//			nuiImageGetColorPixelCoordinatesFromDepthPixelAtResolutionStart = new EventDescriptor(28, 0, 16, 5, 1, 15, long.MinValue);
//			nuiImageGetColorPixelCoordinatesFromDepthPixelAtResolutionEnd = new EventDescriptor(29, 0, 16, 5, 2, 15, long.MinValue);
//			nuiImageGetColorPixelCoordinateFrameFromDepthPixelFrameAtResolutionStart = new EventDescriptor(30, 0, 16, 4, 1, 16, long.MinValue);
//			nuiImageGetColorPixelCoordinateFrameFromDepthPixelFrameAtResolutionEnd = new EventDescriptor(31, 0, 16, 4, 2, 16, long.MinValue);
//			nuiImageStreamOpenStart = new EventDescriptor(32, 0, 16, 4, 1, 17, long.MinValue);
//			nuiImageStreamOpenEnd = new EventDescriptor(33, 0, 16, 4, 2, 17, long.MinValue);
//			nuiImageStreamGetNextFrameStart = new EventDescriptor(34, 0, 16, 4, 1, 18, long.MinValue);
//			nuiImageStreamGetNextFrameEnd = new EventDescriptor(35, 0, 16, 4, 2, 18, long.MinValue);
//			nuiImageStreamReleaseFrameStart = new EventDescriptor(36, 0, 16, 4, 1, 19, long.MinValue);
//			nuiImageStreamReleaseFrameEnd = new EventDescriptor(37, 0, 16, 4, 2, 19, long.MinValue);
//			nuiInitializeStart = new EventDescriptor(38, 0, 16, 4, 1, 20, long.MinValue);
//			nuiInitializeEnd = new EventDescriptor(39, 0, 16, 4, 2, 20, long.MinValue);
//			allFramesReady = new EventDescriptor(40, 0, 16, 4, 0, 21, long.MinValue);
//			colorFrameReady = new EventDescriptor(41, 0, 16, 4, 0, 22, long.MinValue);
//			depthFrameReady = new EventDescriptor(42, 0, 16, 4, 0, 23, long.MinValue);
//			skeletonFrameReady = new EventDescriptor(43, 0, 16, 4, 0, 24, long.MinValue);
//			stopDeviceStart = new EventDescriptor(44, 0, 16, 4, 1, 25, long.MinValue);
//			stopDeviceEnd = new EventDescriptor(45, 0, 16, 4, 2, 25, long.MinValue);
//			singleStreamStopStart = new EventDescriptor(46, 0, 16, 4, 1, 26, long.MinValue);
//			singleStreamStopEnd = new EventDescriptor(47, 0, 16, 4, 2, 26, long.MinValue);
//			detectGenuineDeviceStart = new EventDescriptor(48, 0, 16, 4, 1, 27, long.MinValue);
//			detectGenuineDeviceEnd = new EventDescriptor(49, 0, 16, 4, 2, 27, long.MinValue);
//			createDeviceStart = new EventDescriptor(50, 0, 16, 4, 1, 28, long.MinValue);
//			createDeviceEnd = new EventDescriptor(51, 0, 16, 4, 2, 28, long.MinValue);
//			reEnumerateDevicesStart = new EventDescriptor(52, 0, 16, 4, 1, 29, long.MinValue);
//			reEnumerateDevicesEnd = new EventDescriptor(53, 0, 16, 4, 2, 29, long.MinValue);
//			outputStreamInitStart = new EventDescriptor(54, 0, 16, 4, 1, 30, long.MinValue);
//			outputStreamInitEnd = new EventDescriptor(55, 0, 16, 4, 2, 30, long.MinValue);
//			singleStreamStartStart = new EventDescriptor(56, 0, 16, 4, 1, 31, long.MinValue);
//			singleStreamStartEnd = new EventDescriptor(57, 0, 16, 4, 2, 31, long.MinValue);
//			skeletalTrackerInitializeStart = new EventDescriptor(58, 0, 16, 4, 1, 32, long.MinValue);
//			skeletalTrackerInitializeEnd = new EventDescriptor(59, 0, 16, 4, 2, 32, long.MinValue);
//			kinectSensorInitializeStart = new EventDescriptor(60, 0, 16, 4, 1, 33, long.MinValue);
//			kinectSensorInitializeEnd = new EventDescriptor(61, 0, 16, 4, 2, 33, long.MinValue);
//			skeletonStageBGRStart = new EventDescriptor(62, 0, 16, 4, 1, 34, long.MinValue);
//			skeletonStageBGREnd = new EventDescriptor(63, 0, 16, 4, 2, 34, long.MinValue);
//			skeletonStageCDRPStart = new EventDescriptor(64, 0, 16, 4, 1, 35, long.MinValue);
//			skeletonStageCDRPEnd = new EventDescriptor(65, 0, 16, 4, 2, 35, long.MinValue);
//			skeletonStageHeadDetectorStart = new EventDescriptor(66, 0, 16, 4, 1, 36, long.MinValue);
//			skeletonStageHeadDetectorEnd = new EventDescriptor(67, 0, 16, 4, 2, 36, long.MinValue);
//			skeletonStageExemplarStart = new EventDescriptor(68, 0, 16, 4, 1, 37, long.MinValue);
//			skeletonStageExemplarEnd = new EventDescriptor(69, 0, 16, 4, 2, 37, long.MinValue);
//			skeletonStageCentroidsStart = new EventDescriptor(70, 0, 16, 4, 1, 38, long.MinValue);
//			skeletonStageCentroidsEnd = new EventDescriptor(71, 0, 16, 4, 2, 38, long.MinValue);
//			skeletonStageModelFittingStart = new EventDescriptor(72, 0, 16, 4, 1, 39, long.MinValue);
//			skeletonStageModelFittingEnd = new EventDescriptor(73, 0, 16, 4, 2, 39, long.MinValue);
//			frameRequestDropped = new EventDescriptor(74, 0, 16, 4, 0, 40, long.MinValue);
//			unexpectedFrameRequestError = new EventDescriptor(75, 0, 16, 4, 0, 41, long.MinValue);
//			frameStateChanged = new EventDescriptor(76, 0, 16, 4, 0, 42, long.MinValue);
//			frameDropped = new EventDescriptor(77, 0, 16, 4, 0, 43, long.MinValue);
//			nuiInitializeInfo = new EventDescriptor(85, 0, 16, 5, 0, 20, -9223372036854775807L);
//			nuiImageStreamOpenInfo = new EventDescriptor(86, 0, 16, 5, 0, 17, -9223372036854775807L);
//			nuiShutdownTrace = new EventDescriptor(87, 0, 16, 5, 0, 44, -9223372036854775807L);
//			internalNuiShutdownTrace = new EventDescriptor(88, 0, 16, 5, 0, 45, -9223372036854775807L);
//			nuiSkeletonTrackingEnableTrace = new EventDescriptor(89, 0, 16, 5, 0, 55, -9223372036854775807L);
//			nuiSkeletonTrackingDisableTrace = new EventDescriptor(90, 0, 16, 5, 0, 56, -9223372036854775807L);
//			nuiTransformSmoothInfo = new EventDescriptor(91, 0, 16, 5, 0, 47, -9223372036854775807L);
//			nuiSkeletonSetTrackedSkeletonsInfo = new EventDescriptor(92, 0, 16, 5, 0, 57, -9223372036854775807L);
//			nuiSetFrameEndEventInfo = new EventDescriptor(93, 0, 16, 5, 0, 48, -9223372036854775807L);
//			nuiImageStreamGetNextFrameInfo = new EventDescriptor(94, 0, 16, 5, 0, 18, -9223372036854775807L);
//			nuiSkeletonGetNextFrameInfo = new EventDescriptor(95, 0, 16, 5, 0, 49, -9223372036854775807L);
//			nuiDeviceConnectionIdInfo = new EventDescriptor(97, 0, 16, 5, 0, 50, -9223372036854775807L);
//			nuiUniqueIdInfo = new EventDescriptor(96, 0, 16, 5, 0, 51, -9223372036854775807L);
//			nuiCreateSensorByIndexInfo = new EventDescriptor(98, 0, 16, 5, 0, 52, -9223372036854775807L);
//			nuiCreateSensorByIdInfo = new EventDescriptor(99, 0, 16, 5, 0, 53, -9223372036854775807L);
//			nuiImageStreamSetImageFrameFlagsInfo = new EventDescriptor(100, 0, 16, 5, 0, 54, -9223372036854775807L);
//			nuiAudioBeamGetBeamInfo = new EventDescriptor(101, 0, 16, 5, 0, 58, -9223372036854775807L);
//			nuiAudioBeamGetPositionInfo = new EventDescriptor(102, 0, 16, 5, 0, 59, -9223372036854775807L);
//			nuiAudioBeamSetBeamInfo = new EventDescriptor(103, 0, 16, 5, 0, 60, -9223372036854775807L);
//			nuiGetAudioSourceInfo = new EventDescriptor(104, 0, 16, 5, 0, 61, -9223372036854775807L);
//			audioDmoSetPropertyInfo = new EventDescriptor(105, 0, 16, 4, 0, 65, -9223372036854775807L);
//			managedKinectSensorStartInfo = new EventDescriptor(106, 0, 16, 5, 0, 66, -9223372036854775807L);
//			managedImageStreamDisabledInfo = new EventDescriptor(107, 0, 16, 5, 0, 67, -9223372036854775807L);
//			managedFrameReadyEventAddedInfo = new EventDescriptor(108, 0, 16, 5, 0, 68, -9223372036854775807L);
//			managedFrameReadyEventRemovedInfo = new EventDescriptor(109, 0, 16, 5, 0, 69, -9223372036854775807L);
//			managedOpenNextFrameInfo = new EventDescriptor(110, 0, 16, 5, 0, 70, -9223372036854775807L);
//			managedOpenFrameFromEventInfo = new EventDescriptor(111, 0, 16, 5, 0, 71, -9223372036854775807L);
//			managedOpenFrameFromAllFramesEventInfo = new EventDescriptor(112, 0, 16, 5, 0, 72, -9223372036854775807L);
//			managedAllFramesReadyEventAddedInfo = new EventDescriptor(113, 0, 16, 5, 0, 73, -9223372036854775807L);
//			managedAllFramesReadyEventRemovedInfo = new EventDescriptor(114, 0, 16, 5, 0, 74, -9223372036854775807L);
//			mapColorFrameToDepthFrameStart = new EventDescriptor(115, 0, 16, 4, 1, 75, long.MinValue);
//			mapColorFrameToDepthFrameEnd = new EventDescriptor(116, 0, 16, 4, 2, 75, long.MinValue);
//			mapColorFrameToDepthFrame_InterpolateGapsStart = new EventDescriptor(117, 0, 16, 4, 1, 76, long.MinValue);
//			mapColorFrameToDepthFrame_InterpolateGapsEnd = new EventDescriptor(118, 0, 16, 4, 2, 76, long.MinValue);
//			mapColorFrameToSkeletonFrameStart = new EventDescriptor(119, 0, 16, 4, 1, 77, long.MinValue);
//			mapColorFrameToSkeletonFrameEnd = new EventDescriptor(120, 0, 16, 4, 2, 77, long.MinValue);
//			mapDepthFrameToSkeletonFrameStart = new EventDescriptor(121, 0, 16, 4, 1, 78, long.MinValue);
//			mapDepthFrameToSkeletonFrameEnd = new EventDescriptor(122, 0, 16, 4, 2, 78, long.MinValue);
//		}

//		/// <summary>
//		/// This is the event method for GetSegmentationBegin.
//		/// </summary>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <param name="width">The width parameter.</param>
//		/// <param name="height">The height parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteGetSegmentationBegin(uint streamId, ulong width, ulong height, ulong frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameData(ref getSegmentationBegin, streamId, width, height, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for GetSegmentationEnd.
//		/// </summary>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <param name="width">The width parameter.</param>
//		/// <param name="height">The height parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteGetSegmentationEnd(uint streamId, ulong width, ulong height, ulong frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameData(ref getSegmentationEnd, streamId, width, height, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for ProcessInputBufferBegin.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteProcessInputBufferBegin()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref processInputBufferBegin);
//		}

//		/// <summary>
//		/// This is the event method for ProcessInputBufferEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteProcessInputBufferEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref processInputBufferEnd);
//		}

//		/// <summary>
//		/// This is the event method for NotifyFrameReadyStart.
//		/// </summary>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <param name="width">The width parameter.</param>
//		/// <param name="height">The height parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNotifyFrameReadyStart(uint streamId, ulong width, ulong height, ulong frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameData(ref notifyFrameReadyStart, streamId, width, height, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for NotifyFrameReadyEnd.
//		/// </summary>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <param name="width">The width parameter.</param>
//		/// <param name="height">The height parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNotifyFrameReadyEnd(uint streamId, ulong width, ulong height, ulong frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameData(ref notifyFrameReadyEnd, streamId, width, height, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for ProcessFrameStart.
//		/// </summary>
//		/// <param name="frameOverlapId">The frameOverlapId parameter.</param>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteProcessFrameStart(IntPtr frameOverlapId, uint streamId)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateProcessFrameData(ref processFrameStart, frameOverlapId, streamId);
//		}

//		/// <summary>
//		/// This is the event method for ProcessFrameEnd.
//		/// </summary>
//		/// <param name="frameOverlapId">The frameOverlapId parameter.</param>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteProcessFrameEnd(IntPtr frameOverlapId, uint streamId)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateProcessFrameData(ref processFrameEnd, frameOverlapId, streamId);
//		}

//		/// <summary>
//		/// This is the event method for GetSkeletonsBegin.
//		/// </summary>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <param name="width">The width parameter.</param>
//		/// <param name="height">The height parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteGetSkeletonsBegin(uint streamId, ulong width, ulong height, ulong frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameData(ref getSkeletonsBegin, streamId, width, height, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for GetSkeletonsEnd.
//		/// </summary>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <param name="width">The width parameter.</param>
//		/// <param name="height">The height parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteGetSkeletonsEnd(uint streamId, ulong width, ulong height, ulong frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameData(ref getSkeletonsEnd, streamId, width, height, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for UnpackBayerStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteUnpackBayerStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref unpackBayerStart);
//		}

//		/// <summary>
//		/// This is the event method for UnpackBayerEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteUnpackBayerEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref unpackBayerEnd);
//		}

//		/// <summary>
//		/// This is the event method for FrameStart.
//		/// </summary>
//		/// <param name="streamType">The streamType parameter.</param>
//		/// <param name="bufferIndex">The bufferIndex parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteFrameStart(uint streamType, int bufferIndex, int frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameLifecycleData(ref frameStart, streamType, bufferIndex, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for FrameEnd.
//		/// </summary>
//		/// <param name="streamType">The streamType parameter.</param>
//		/// <param name="bufferIndex">The bufferIndex parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteFrameEnd(uint streamType, int bufferIndex, int frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameLifecycleData(ref frameEnd, streamType, bufferIndex, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for MapDepthFrameToColorFrameStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteMapDepthFrameToColorFrameStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref mapDepthFrameToColorFrameStart);
//		}

//		/// <summary>
//		/// This is the event method for MapDepthFrameToColorFrameEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteMapDepthFrameToColorFrameEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref mapDepthFrameToColorFrameEnd);
//		}

//		/// <summary>
//		/// This is the event method for MapDepthToColorImagePointStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteMapDepthToColorImagePointStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref mapDepthToColorImagePointStart);
//		}

//		/// <summary>
//		/// This is the event method for MapDepthToColorImagePointEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteMapDepthToColorImagePointEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref mapDepthToColorImagePointEnd);
//		}

//		/// <summary>
//		/// This is the event method for MapSkeletonPointToColorStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteMapSkeletonPointToColorStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref mapSkeletonPointToColorStart);
//		}

//		/// <summary>
//		/// This is the event method for MapSkeletonPointToColorEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteMapSkeletonPointToColorEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref mapSkeletonPointToColorEnd);
//		}

//		/// <summary>
//		/// This is the event method for MapSkeletonPointToDepthStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteMapSkeletonPointToDepthStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref mapSkeletonPointToDepthStart);
//		}

//		/// <summary>
//		/// This is the event method for MapSkeletonPointToDepthEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteMapSkeletonPointToDepthEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref mapSkeletonPointToDepthEnd);
//		}

//		/// <summary>
//		/// This is the event method for MapDepthToSkeletonPointStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteMapDepthToSkeletonPointStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref mapDepthToSkeletonPointStart);
//		}

//		/// <summary>
//		/// This is the event method for MapDepthToSkeletonPointEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteMapDepthToSkeletonPointEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref mapDepthToSkeletonPointEnd);
//		}

//		/// <summary>
//		/// This is the event method for ManagedNuiTransformDepthImageToSkeletonStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteManagedNuiTransformDepthImageToSkeletonStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref managedNuiTransformDepthImageToSkeletonStart);
//		}

//		/// <summary>
//		/// This is the event method for ManagedNuiTransformDepthImageToSkeletonEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteManagedNuiTransformDepthImageToSkeletonEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref managedNuiTransformDepthImageToSkeletonEnd);
//		}

//		/// <summary>
//		/// This is the event method for ManagedNuiTransformSkeletonToDepthImageStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteManagedNuiTransformSkeletonToDepthImageStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref managedNuiTransformSkeletonToDepthImageStart);
//		}

//		/// <summary>
//		/// This is the event method for ManagedNuiTransformSkeletonToDepthImageEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteManagedNuiTransformSkeletonToDepthImageEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref managedNuiTransformSkeletonToDepthImageEnd);
//		}

//		/// <summary>
//		/// This is the event method for NuiImageGetColorPixelCoordinatesFromDepthPixelAtResolutionStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiImageGetColorPixelCoordinatesFromDepthPixelAtResolutionStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref nuiImageGetColorPixelCoordinatesFromDepthPixelAtResolutionStart);
//		}

//		/// <summary>
//		/// This is the event method for NuiImageGetColorPixelCoordinatesFromDepthPixelAtResolutionEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiImageGetColorPixelCoordinatesFromDepthPixelAtResolutionEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref nuiImageGetColorPixelCoordinatesFromDepthPixelAtResolutionEnd);
//		}

//		/// <summary>
//		/// This is the event method for NuiImageGetColorPixelCoordinateFrameFromDepthPixelFrameAtResolutionStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiImageGetColorPixelCoordinateFrameFromDepthPixelFrameAtResolutionStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref nuiImageGetColorPixelCoordinateFrameFromDepthPixelFrameAtResolutionStart);
//		}

//		/// <summary>
//		/// This is the event method for NuiImageGetColorPixelCoordinateFrameFromDepthPixelFrameAtResolutionEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiImageGetColorPixelCoordinateFrameFromDepthPixelFrameAtResolutionEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref nuiImageGetColorPixelCoordinateFrameFromDepthPixelFrameAtResolutionEnd);
//		}

//		/// <summary>
//		/// This is the event method for NuiImageStreamOpenStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiImageStreamOpenStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref nuiImageStreamOpenStart);
//		}

//		/// <summary>
//		/// This is the event method for NuiImageStreamOpenEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiImageStreamOpenEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref nuiImageStreamOpenEnd);
//		}

//		/// <summary>
//		/// This is the event method for NuiImageStreamGetNextFrameStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiImageStreamGetNextFrameStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref nuiImageStreamGetNextFrameStart);
//		}

//		/// <summary>
//		/// This is the event method for NuiImageStreamGetNextFrameEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiImageStreamGetNextFrameEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref nuiImageStreamGetNextFrameEnd);
//		}

//		/// <summary>
//		/// This is the event method for NuiImageStreamReleaseFrameStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiImageStreamReleaseFrameStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref nuiImageStreamReleaseFrameStart);
//		}

//		/// <summary>
//		/// This is the event method for NuiImageStreamReleaseFrameEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiImageStreamReleaseFrameEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref nuiImageStreamReleaseFrameEnd);
//		}

//		/// <summary>
//		/// This is the event method for NuiInitializeStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiInitializeStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref nuiInitializeStart);
//		}

//		/// <summary>
//		/// This is the event method for NuiInitializeEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiInitializeEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref nuiInitializeEnd);
//		}

//		/// <summary>
//		/// This is the event method for AllFramesReady.
//		/// </summary>
//		/// <param name="colorFrameTimestamp">The colorFrameTimestamp parameter.</param>
//		/// <param name="depthFrameTimestamp">The depthFrameTimestamp parameter.</param>
//		/// <param name="skeletonFrameTimestamp">The skeletonFrameTimestamp parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteAllFramesReady(long colorFrameTimestamp, long depthFrameTimestamp, long skeletonFrameTimestamp)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateAllFramesReadyData(ref allFramesReady, colorFrameTimestamp, depthFrameTimestamp, skeletonFrameTimestamp);
//		}

//		/// <summary>
//		/// This is the event method for ColorFrameReady.
//		/// </summary>
//		/// <param name="timestamp">The timestamp parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteColorFrameReady(long timestamp)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameReadyData(ref colorFrameReady, timestamp);
//		}

//		/// <summary>
//		/// This is the event method for DepthFrameReady.
//		/// </summary>
//		/// <param name="timestamp">The timestamp parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteDepthFrameReady(long timestamp)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameReadyData(ref depthFrameReady, timestamp);
//		}

//		/// <summary>
//		/// This is the event method for SkeletonFrameReady.
//		/// </summary>
//		/// <param name="timestamp">The timestamp parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteSkeletonFrameReady(long timestamp)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameReadyData(ref skeletonFrameReady, timestamp);
//		}

//		/// <summary>
//		/// This is the event method for StopDeviceStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteStopDeviceStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref stopDeviceStart);
//		}

//		/// <summary>
//		/// This is the event method for StopDeviceEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteStopDeviceEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref stopDeviceEnd);
//		}

//		/// <summary>
//		/// This is the event method for SingleStreamStopStart.
//		/// </summary>
//		/// <param name="streamType">The streamType parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteSingleStreamStopStart(uint streamType)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateStreamTypeOnly(ref singleStreamStopStart, streamType);
//		}

//		/// <summary>
//		/// This is the event method for SingleStreamStopEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteSingleStreamStopEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref singleStreamStopEnd);
//		}

//		/// <summary>
//		/// This is the event method for DetectGenuineDeviceStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteDetectGenuineDeviceStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref detectGenuineDeviceStart);
//		}

//		/// <summary>
//		/// This is the event method for DetectGenuineDeviceEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteDetectGenuineDeviceEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref detectGenuineDeviceEnd);
//		}

//		/// <summary>
//		/// This is the event method for CreateDeviceStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteCreateDeviceStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref createDeviceStart);
//		}

//		/// <summary>
//		/// This is the event method for CreateDeviceEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteCreateDeviceEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref createDeviceEnd);
//		}

//		/// <summary>
//		/// This is the event method for ReEnumerateDevicesStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteReEnumerateDevicesStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref reEnumerateDevicesStart);
//		}

//		/// <summary>
//		/// This is the event method for ReEnumerateDevicesEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteReEnumerateDevicesEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref reEnumerateDevicesEnd);
//		}

//		/// <summary>
//		/// This is the event method for OutputStreamInitStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteOutputStreamInitStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref outputStreamInitStart);
//		}

//		/// <summary>
//		/// This is the event method for OutputStreamInitEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteOutputStreamInitEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref outputStreamInitEnd);
//		}

//		/// <summary>
//		/// This is the event method for SingleStreamStartStart.
//		/// </summary>
//		/// <param name="streamType">The streamType parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteSingleStreamStartStart(uint streamType)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateStreamTypeOnly(ref singleStreamStartStart, streamType);
//		}

//		/// <summary>
//		/// This is the event method for SingleStreamStartEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteSingleStreamStartEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref singleStreamStartEnd);
//		}

//		/// <summary>
//		/// This is the event method for SkeletalTrackerInitializeStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteSkeletalTrackerInitializeStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref skeletalTrackerInitializeStart);
//		}

//		/// <summary>
//		/// This is the event method for SkeletalTrackerInitializeEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteSkeletalTrackerInitializeEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref skeletalTrackerInitializeEnd);
//		}

//		/// <summary>
//		/// This is the event method for KinectSensorInitializeStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteKinectSensorInitializeStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref kinectSensorInitializeStart);
//		}

//		/// <summary>
//		/// This is the event method for KinectSensorInitializeEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteKinectSensorInitializeEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref kinectSensorInitializeEnd);
//		}

//		/// <summary>
//		/// This is the event method for SkeletonStageBGRStart.
//		/// </summary>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <param name="width">The width parameter.</param>
//		/// <param name="height">The height parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteSkeletonStageBGRStart(uint streamId, ulong width, ulong height, ulong frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameData(ref skeletonStageBGRStart, streamId, width, height, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for SkeletonStageBGREnd.
//		/// </summary>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <param name="width">The width parameter.</param>
//		/// <param name="height">The height parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteSkeletonStageBGREnd(uint streamId, ulong width, ulong height, ulong frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameData(ref skeletonStageBGREnd, streamId, width, height, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for SkeletonStageCDRPStart.
//		/// </summary>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <param name="width">The width parameter.</param>
//		/// <param name="height">The height parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteSkeletonStageCDRPStart(uint streamId, ulong width, ulong height, ulong frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameData(ref skeletonStageCDRPStart, streamId, width, height, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for SkeletonStageCDRPEnd.
//		/// </summary>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <param name="width">The width parameter.</param>
//		/// <param name="height">The height parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteSkeletonStageCDRPEnd(uint streamId, ulong width, ulong height, ulong frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameData(ref skeletonStageCDRPEnd, streamId, width, height, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for SkeletonStageHeadDetectorStart.
//		/// </summary>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <param name="width">The width parameter.</param>
//		/// <param name="height">The height parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteSkeletonStageHeadDetectorStart(uint streamId, ulong width, ulong height, ulong frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameData(ref skeletonStageHeadDetectorStart, streamId, width, height, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for SkeletonStageHeadDetectorEnd.
//		/// </summary>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <param name="width">The width parameter.</param>
//		/// <param name="height">The height parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteSkeletonStageHeadDetectorEnd(uint streamId, ulong width, ulong height, ulong frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameData(ref skeletonStageHeadDetectorEnd, streamId, width, height, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for SkeletonStageExemplarStart.
//		/// </summary>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <param name="width">The width parameter.</param>
//		/// <param name="height">The height parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteSkeletonStageExemplarStart(uint streamId, ulong width, ulong height, ulong frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameData(ref skeletonStageExemplarStart, streamId, width, height, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for SkeletonStageExemplarEnd.
//		/// </summary>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <param name="width">The width parameter.</param>
//		/// <param name="height">The height parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteSkeletonStageExemplarEnd(uint streamId, ulong width, ulong height, ulong frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameData(ref skeletonStageExemplarEnd, streamId, width, height, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for SkeletonStageCentroidsStart.
//		/// </summary>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <param name="width">The width parameter.</param>
//		/// <param name="height">The height parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteSkeletonStageCentroidsStart(uint streamId, ulong width, ulong height, ulong frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameData(ref skeletonStageCentroidsStart, streamId, width, height, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for SkeletonStageCentroidsEnd.
//		/// </summary>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <param name="width">The width parameter.</param>
//		/// <param name="height">The height parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteSkeletonStageCentroidsEnd(uint streamId, ulong width, ulong height, ulong frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameData(ref skeletonStageCentroidsEnd, streamId, width, height, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for SkeletonStageModelFittingStart.
//		/// </summary>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <param name="width">The width parameter.</param>
//		/// <param name="height">The height parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteSkeletonStageModelFittingStart(uint streamId, ulong width, ulong height, ulong frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameData(ref skeletonStageModelFittingStart, streamId, width, height, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for SkeletonStageModelFittingEnd.
//		/// </summary>
//		/// <param name="streamId">The streamId parameter.</param>
//		/// <param name="width">The width parameter.</param>
//		/// <param name="height">The height parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteSkeletonStageModelFittingEnd(uint streamId, ulong width, ulong height, ulong frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameData(ref skeletonStageModelFittingEnd, streamId, width, height, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for FrameRequestDropped.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteFrameRequestDropped()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref frameRequestDropped);
//		}

//		/// <summary>
//		/// This is the event method for UnexpectedFrameRequestError.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteUnexpectedFrameRequestError()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref unexpectedFrameRequestError);
//		}

//		/// <summary>
//		/// This is the event method for FrameStateChanged.
//		/// </summary>
//		/// <param name="streamType">The streamType parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <param name="state">The state parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteFrameStateChanged(uint streamType, int frameNumber, uint state)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameStateData(ref frameStateChanged, streamType, frameNumber, state);
//		}

//		/// <summary>
//		/// This is the event method for FrameDropped.
//		/// </summary>
//		/// <param name="streamType">The streamType parameter.</param>
//		/// <param name="frameNumber">The frameNumber parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteFrameDropped(uint streamType, int frameNumber)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateFrameDroppedData(ref frameDropped, streamType, frameNumber);
//		}

//		/// <summary>
//		/// This is the event method for NuiInitializeInfo.
//		/// </summary>
//		/// <param name="deviceId">The deviceId parameter.</param>
//		/// <param name="processName">The processName parameter.</param>
//		/// <param name="flags">The flags parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiInitializeInfo(string deviceId, string processName, uint flags)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateNuiInitializeData(ref nuiInitializeInfo, deviceId, processName, flags);
//		}

//		/// <summary>
//		/// This is the event method for NuiImageStreamOpenInfo.
//		/// </summary>
//		/// <param name="hStream">The hStream parameter.</param>
//		/// <param name="imageType">The imageType parameter.</param>
//		/// <param name="imageResolution">The imageResolution parameter.</param>
//		/// <param name="imageFrameFlags">The imageFrameFlags parameter.</param>
//		/// <param name="hNextFrameEvent">The hNextFrameEvent parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiImageStreamOpenInfo(IntPtr hStream, uint imageType, uint imageResolution, uint imageFrameFlags, IntPtr hNextFrameEvent)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateNuiImageStreamOpenData(ref nuiImageStreamOpenInfo, hStream, imageType, imageResolution, imageFrameFlags, hNextFrameEvent);
//		}

//		/// <summary>
//		/// This is the event method for NuiShutdownTrace.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiShutdownTrace()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref nuiShutdownTrace);
//		}

//		/// <summary>
//		/// This is the event method for InternalNuiShutdownTrace.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteInternalNuiShutdownTrace()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref internalNuiShutdownTrace);
//		}

//		/// <summary>
//		/// This is the event method for NuiSkeletonTrackingEnableTrace.
//		/// </summary>
//		/// <param name="hNextFrameEvent">The hNextFrameEvent parameter.</param>
//		/// <param name="dwFlags">The dwFlags parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiSkeletonTrackingEnableTrace(IntPtr hNextFrameEvent, uint dwFlags)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateNuiSkeletonTrackingData(ref nuiSkeletonTrackingEnableTrace, hNextFrameEvent, dwFlags);
//		}

//		/// <summary>
//		/// This is the event method for NuiSkeletonTrackingDisableTrace.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiSkeletonTrackingDisableTrace()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref nuiSkeletonTrackingDisableTrace);
//		}

//		/// <summary>
//		/// This is the event method for NuiTransformSmoothInfo.
//		/// </summary>
//		/// <param name="smoothing">The smoothing parameter.</param>
//		/// <param name="correction">The correction parameter.</param>
//		/// <param name="prediction">The prediction parameter.</param>
//		/// <param name="jitterRadius">The jitterRadius parameter.</param>
//		/// <param name="maxDeviationRadius">The maxDeviationRadius parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiTransformSmoothInfo(float smoothing, float correction, float prediction, float jitterRadius, float maxDeviationRadius)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateNuiTransformSmoothParams(ref nuiTransformSmoothInfo, smoothing, correction, prediction, jitterRadius, maxDeviationRadius);
//		}

//		/// <summary>
//		/// This is the event method for NuiSkeletonSetTrackedSkeletonsInfo.
//		/// </summary>
//		/// <param name="skeleton1">The skeleton1 parameter.</param>
//		/// <param name="skeleton2">The skeleton2 parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiSkeletonSetTrackedSkeletonsInfo(uint skeleton1, uint skeleton2)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateNuiSkeletonSetTrackedSkeletonsData(ref nuiSkeletonSetTrackedSkeletonsInfo, skeleton1, skeleton2);
//		}

//		/// <summary>
//		/// This is the event method for NuiSetFrameEndEventInfo.
//		/// </summary>
//		/// <param name="hEvent">The hEvent parameter.</param>
//		/// <param name="dwFrameEventFlag">The dwFrameEventFlag parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiSetFrameEndEventInfo(IntPtr hEvent, uint dwFrameEventFlag)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateNuiSetFrameEndEventData(ref nuiSetFrameEndEventInfo, hEvent, dwFrameEventFlag);
//		}

//		/// <summary>
//		/// This is the event method for NuiImageStreamGetNextFrameInfo.
//		/// </summary>
//		/// <param name="hStream">The hStream parameter.</param>
//		/// <param name="dwMillisecondsToWait">The dwMillisecondsToWait parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiImageStreamGetNextFrameInfo(IntPtr hStream, uint dwMillisecondsToWait)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateNuiImageStreamGetNextFrameData(ref nuiImageStreamGetNextFrameInfo, hStream, dwMillisecondsToWait);
//		}

//		/// <summary>
//		/// This is the event method for NuiSkeletonGetNextFrameInfo.
//		/// </summary>
//		/// <param name="dwMillisecondsToWait">The dwMillisecondsToWait parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiSkeletonGetNextFrameInfo(uint dwMillisecondsToWait)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateNuiSkeletonGetNextFrameData(ref nuiSkeletonGetNextFrameInfo, dwMillisecondsToWait);
//		}

//		/// <summary>
//		/// This is the event method for NuiDeviceConnectionIdInfo.
//		/// </summary>
//		/// <param name="kinectId">The kinectId parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiDeviceConnectionIdInfo(string kinectId)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateKinectIdStringData(ref nuiDeviceConnectionIdInfo, kinectId);
//		}

//		/// <summary>
//		/// This is the event method for NuiUniqueIdInfo.
//		/// </summary>
//		/// <param name="kinectId">The kinectId parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiUniqueIdInfo(string kinectId)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateKinectIdStringData(ref nuiUniqueIdInfo, kinectId);
//		}

//		/// <summary>
//		/// This is the event method for NuiCreateSensorByIndexInfo.
//		/// </summary>
//		/// <param name="index">The index parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiCreateSensorByIndexInfo(int index)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateNuiCreateSensorByIndexData(ref nuiCreateSensorByIndexInfo, index);
//		}

//		/// <summary>
//		/// This is the event method for NuiCreateSensorByIdInfo.
//		/// </summary>
//		/// <param name="strInstanceId">The strInstanceId parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiCreateSensorByIdInfo(string strInstanceId)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateNuiCreateSensorByIdData(ref nuiCreateSensorByIdInfo, strInstanceId);
//		}

//		/// <summary>
//		/// This is the event method for NuiImageStreamSetImageFrameFlagsInfo.
//		/// </summary>
//		/// <param name="hStream">The hStream parameter.</param>
//		/// <param name="dwImageFrameFlags">The dwImageFrameFlags parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiImageStreamSetImageFrameFlagsInfo(IntPtr hStream, uint dwImageFrameFlags)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateNuiImageStreamSetImageFrameFlagsData(ref nuiImageStreamSetImageFrameFlagsInfo, hStream, dwImageFrameFlags);
//		}

//		/// <summary>
//		/// This is the event method for NuiAudioBeamGetBeamInfo.
//		/// </summary>
//		/// <param name="dAngle">The dAngle parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiAudioBeamGetBeamInfo(double dAngle)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateAudioAngleData(ref nuiAudioBeamGetBeamInfo, dAngle);
//		}

//		/// <summary>
//		/// This is the event method for NuiAudioBeamGetPositionInfo.
//		/// </summary>
//		/// <param name="dAngle">The dAngle parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiAudioBeamGetPositionInfo(double dAngle)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateAudioAngleData(ref nuiAudioBeamGetPositionInfo, dAngle);
//		}

//		/// <summary>
//		/// This is the event method for NuiAudioBeamSetBeamInfo.
//		/// </summary>
//		/// <param name="dAngle">The dAngle parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiAudioBeamSetBeamInfo(double dAngle)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateAudioAngleData(ref nuiAudioBeamSetBeamInfo, dAngle);
//		}

//		/// <summary>
//		/// This is the event method for NuiGetAudioSourceInfo.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteNuiGetAudioSourceInfo()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref nuiGetAudioSourceInfo);
//		}

//		/// <summary>
//		/// This is the event method for AudioDmoSetPropertyInfo.
//		/// </summary>
//		/// <param name="pid">The pid parameter.</param>
//		/// <param name="value">The value parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteAudioDmoSetPropertyInfo(uint pid, uint value)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateAudioPropertyData(ref audioDmoSetPropertyInfo, pid, value);
//		}

//		/// <summary>
//		/// This is the event method for ManagedKinectSensorStartInfo.
//		/// </summary>
//		/// <param name="deviceID">The deviceID parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteManagedKinectSensorStartInfo(string deviceID)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateKinectSensorStartData(ref managedKinectSensorStartInfo, deviceID);
//		}

//		/// <summary>
//		/// This is the event method for ManagedImageStreamDisabledInfo.
//		/// </summary>
//		/// <param name="type">The type parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteManagedImageStreamDisabledInfo(int type)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateManagedImageStreamData(ref managedImageStreamDisabledInfo, type);
//		}

//		/// <summary>
//		/// This is the event method for ManagedFrameReadyEventAddedInfo.
//		/// </summary>
//		/// <param name="type">The type parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteManagedFrameReadyEventAddedInfo(int type)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateManagedFramesReadyRegistrationData(ref managedFrameReadyEventAddedInfo, type);
//		}

//		/// <summary>
//		/// This is the event method for ManagedFrameReadyEventRemovedInfo.
//		/// </summary>
//		/// <param name="type">The type parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteManagedFrameReadyEventRemovedInfo(int type)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateManagedFramesReadyRegistrationData(ref managedFrameReadyEventRemovedInfo, type);
//		}

//		/// <summary>
//		/// This is the event method for ManagedOpenNextFrameInfo.
//		/// </summary>
//		/// <param name="streamType">The streamType parameter.</param>
//		/// <param name="millisecondsWait">The millisecondsWait parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteManagedOpenNextFrameInfo(int streamType, int millisecondsWait)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateManagedOpenNextFrameData(ref managedOpenNextFrameInfo, streamType, millisecondsWait);
//		}

//		/// <summary>
//		/// This is the event method for ManagedOpenFrameFromEventInfo.
//		/// </summary>
//		/// <param name="type">The type parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteManagedOpenFrameFromEventInfo(int type)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateManagedImageStreamData(ref managedOpenFrameFromEventInfo, type);
//		}

//		/// <summary>
//		/// This is the event method for ManagedOpenFrameFromAllFramesEventInfo.
//		/// </summary>
//		/// <param name="type">The type parameter.</param>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteManagedOpenFrameFromAllFramesEventInfo(int type)
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateManagedImageStreamData(ref managedOpenFrameFromAllFramesEventInfo, type);
//		}

//		/// <summary>
//		/// This is the event method for ManagedAllFramesReadyEventAddedInfo.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteManagedAllFramesReadyEventAddedInfo()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref managedAllFramesReadyEventAddedInfo);
//		}

//		/// <summary>
//		/// This is the event method for ManagedAllFramesReadyEventRemovedInfo.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteManagedAllFramesReadyEventRemovedInfo()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref managedAllFramesReadyEventRemovedInfo);
//		}

//		/// <summary>
//		/// This is the event method for MapColorFrameToDepthFrameStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteMapColorFrameToDepthFrameStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref mapColorFrameToDepthFrameStart);
//		}

//		/// <summary>
//		/// This is the event method for MapColorFrameToDepthFrameEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteMapColorFrameToDepthFrameEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref mapColorFrameToDepthFrameEnd);
//		}

//		/// <summary>
//		/// This is the event method for MapColorFrameToDepthFrame_InterpolateGapsStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteMapColorFrameToDepthFrame_InterpolateGapsStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref mapColorFrameToDepthFrame_InterpolateGapsStart);
//		}

//		/// <summary>
//		/// This is the event method for MapColorFrameToDepthFrame_InterpolateGapsEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteMapColorFrameToDepthFrame_InterpolateGapsEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref mapColorFrameToDepthFrame_InterpolateGapsEnd);
//		}

//		/// <summary>
//		/// This is the event method for MapColorFrameToSkeletonFrameStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteMapColorFrameToSkeletonFrameStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref mapColorFrameToSkeletonFrameStart);
//		}

//		/// <summary>
//		/// This is the event method for MapColorFrameToSkeletonFrameEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteMapColorFrameToSkeletonFrameEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref mapColorFrameToSkeletonFrameEnd);
//		}

//		/// <summary>
//		/// This is the event method for MapDepthFrameToSkeletonFrameStart.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteMapDepthFrameToSkeletonFrameStart()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref mapDepthFrameToSkeletonFrameStart);
//		}

//		/// <summary>
//		/// This is the event method for MapDepthFrameToSkeletonFrameEnd.
//		/// </summary>
//		/// <returns>This method returns true if succeeded, and false otherwise.</returns>
//		public static bool EventWriteMapDepthFrameToSkeletonFrameEnd()
//		{
//			if (!Provider.IsEnabled())
//			{
//				return true;
//			}
//			return Provider.TemplateEventDescriptor(ref mapDepthFrameToSkeletonFrameEnd);
//		}
//	}
//}

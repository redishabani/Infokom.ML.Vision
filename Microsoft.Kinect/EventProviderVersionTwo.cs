//using System;
//using System.Diagnostics.Eventing;
//using System.Runtime.InteropServices;

//namespace Microsoft.Kinect
//{
//	/// <summary>
//	/// This is a local wrapper for the EventProvider.
//	/// </summary>
//	internal class EventProviderVersionTwo : EventProvider
//	{
//		/// <summary>
//		/// This struct is used to marshal data into the native ETW service.
//		/// </summary>
//		[StructLayout(LayoutKind.Explicit, Size = 16)]
//		private struct EventData
//		{
//			/// <summary>
//			/// The event data's data pointer.
//			/// </summary>
//			[FieldOffset(0)]
//			internal ulong DataPointer;

//			/// <summary>
//			/// The event data size.
//			/// </summary>
//			[FieldOffset(8)]
//			internal uint Size;

//			/// <summary>
//			/// This field is reserverd.
//			/// </summary>
//			[FieldOffset(12)]
//			internal int Reserved;
//		}

//		/// <summary>
//		/// Initializes a new instance of the EventProviderVersionTwo class.
//		/// </summary>
//		/// <param name="id">The guid of the provider.</param>
//		internal EventProviderVersionTwo(Guid id) : base(id)
//		{
//		}

//		/// <summary>
//		/// This method determines if it needs to write the event.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description to write.</param>
//		/// <returns>This returns true if successful, and false otherwise.</returns>
//		internal bool TemplateEventDescriptor(ref EventDescriptor eventDescriptor)
//		{
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				return WriteEvent(ref eventDescriptor, 0, null);
//			}
//			return true;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the FrameData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="streamId">The value for the streamId parameter.</param>
//		/// <param name="width">The value for the width parameter.</param>
//		/// <param name="height">The value for the height parameter.</param>
//		/// <param name="frameNumber">The value for the frameNumber parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateFrameData(ref EventDescriptor eventDescriptor, uint streamId, ulong width, ulong height, ulong frameNumber)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)(sizeof(EventData) * 4)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&streamId);
//				ptr2->Size = 4u;
//				ptr2[1].DataPointer = (ulong)(&width);
//				ptr2[1].Size = 8u;
//				ptr2[2].DataPointer = (ulong)(&height);
//				ptr2[2].Size = 8u;
//				ptr2[3].DataPointer = (ulong)(&frameNumber);
//				ptr2[3].Size = 8u;
//				result = WriteEvent(ref eventDescriptor, 4, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the ProcessFrameData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="frameOverlapId">The value for the frameOverlapId parameter.</param>
//		/// <param name="streamId">The value for the streamId parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateProcessFrameData(ref EventDescriptor eventDescriptor, IntPtr frameOverlapId, uint streamId)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)(sizeof(EventData) * 2)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&frameOverlapId);
//				ptr2->Size = (uint)sizeof(IntPtr);
//				ptr2[1].DataPointer = (ulong)(&streamId);
//				ptr2[1].Size = 4u;
//				result = WriteEvent(ref eventDescriptor, 2, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the FrameLifecycleData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="streamType">The value for the streamType parameter.</param>
//		/// <param name="bufferIndex">The value for the bufferIndex parameter.</param>
//		/// <param name="frameNumber">The value for the frameNumber parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateFrameLifecycleData(ref EventDescriptor eventDescriptor, uint streamType, int bufferIndex, int frameNumber)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)(sizeof(EventData) * 3)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&streamType);
//				ptr2->Size = 4u;
//				ptr2[1].DataPointer = (ulong)(&bufferIndex);
//				ptr2[1].Size = 4u;
//				ptr2[2].DataPointer = (ulong)(&frameNumber);
//				ptr2[2].Size = 4u;
//				result = WriteEvent(ref eventDescriptor, 3, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the AllFramesReadyData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="colorFrameTimestamp">The value for the colorFrameTimestamp parameter.</param>
//		/// <param name="depthFrameTimestamp">The value for the depthFrameTimestamp parameter.</param>
//		/// <param name="skeletonFrameTimestamp">The value for the skeletonFrameTimestamp parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateAllFramesReadyData(ref EventDescriptor eventDescriptor, long colorFrameTimestamp, long depthFrameTimestamp, long skeletonFrameTimestamp)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)(sizeof(EventData) * 3)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&colorFrameTimestamp);
//				ptr2->Size = 8u;
//				ptr2[1].DataPointer = (ulong)(&depthFrameTimestamp);
//				ptr2[1].Size = 8u;
//				ptr2[2].DataPointer = (ulong)(&skeletonFrameTimestamp);
//				ptr2[2].Size = 8u;
//				result = WriteEvent(ref eventDescriptor, 3, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the FrameReadyData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="timestamp">The value for the timestamp parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateFrameReadyData(ref EventDescriptor eventDescriptor, long timestamp)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)sizeof(EventData)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&timestamp);
//				ptr2->Size = 8u;
//				result = WriteEvent(ref eventDescriptor, 1, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the StreamTypeOnly template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="streamType">The value for the streamType parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateStreamTypeOnly(ref EventDescriptor eventDescriptor, uint streamType)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)sizeof(EventData)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&streamType);
//				ptr2->Size = 4u;
//				result = WriteEvent(ref eventDescriptor, 1, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the FrameStateData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="streamType">The value for the streamType parameter.</param>
//		/// <param name="frameNumber">The value for the frameNumber parameter.</param>
//		/// <param name="state">The value for the state parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateFrameStateData(ref EventDescriptor eventDescriptor, uint streamType, int frameNumber, uint state)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)(sizeof(EventData) * 3)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&streamType);
//				ptr2->Size = 4u;
//				ptr2[1].DataPointer = (ulong)(&frameNumber);
//				ptr2[1].Size = 4u;
//				ptr2[2].DataPointer = (ulong)(&state);
//				ptr2[2].Size = 4u;
//				result = WriteEvent(ref eventDescriptor, 3, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the FrameDroppedData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="streamType">The value for the streamType parameter.</param>
//		/// <param name="frameNumber">The value for the frameNumber parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateFrameDroppedData(ref EventDescriptor eventDescriptor, uint streamType, int frameNumber)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)(sizeof(EventData) * 2)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&streamType);
//				ptr2->Size = 4u;
//				ptr2[1].DataPointer = (ulong)(&frameNumber);
//				ptr2[1].Size = 4u;
//				result = WriteEvent(ref eventDescriptor, 2, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the NuiImageStreamOpenData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="hStream">The value for the hStream parameter.</param>
//		/// <param name="imageType">The value for the imageType parameter.</param>
//		/// <param name="imageResolution">The value for the imageResolution parameter.</param>
//		/// <param name="imageFrameFlags">The value for the imageFrameFlags parameter.</param>
//		/// <param name="hNextFrameEvent">The value for the hNextFrameEvent parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateNuiImageStreamOpenData(ref EventDescriptor eventDescriptor, IntPtr hStream, uint imageType, uint imageResolution, uint imageFrameFlags, IntPtr hNextFrameEvent)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)(sizeof(EventData) * 5)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&hStream);
//				ptr2->Size = (uint)sizeof(IntPtr);
//				ptr2[1].DataPointer = (ulong)(&imageType);
//				ptr2[1].Size = 4u;
//				ptr2[2].DataPointer = (ulong)(&imageResolution);
//				ptr2[2].Size = 4u;
//				ptr2[3].DataPointer = (ulong)(&imageFrameFlags);
//				ptr2[3].Size = 4u;
//				ptr2[4].DataPointer = (ulong)(&hNextFrameEvent);
//				ptr2[4].Size = (uint)sizeof(IntPtr);
//				result = WriteEvent(ref eventDescriptor, 5, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the NuiInitializeData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="deviceId">The value for the deviceId parameter.</param>
//		/// <param name="processName">The value for the processName parameter.</param>
//		/// <param name="flags">The value for the flags parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateNuiInitializeData(ref EventDescriptor eventDescriptor, string deviceId, string processName, uint flags)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)(sizeof(EventData) * 3)];
//				EventData* ptr2 = (EventData*)ptr;
//				fixed (char* ptr3 = deviceId)
//				{
//					fixed (char* ptr4 = processName)
//					{
//						ptr2->DataPointer = (ulong)ptr3;
//						ptr2->Size = (uint)((deviceId.Length + 1) * 2);
//						ptr2[1].DataPointer = (ulong)ptr4;
//						ptr2[1].Size = (uint)((processName.Length + 1) * 2);
//						ptr2[2].DataPointer = (ulong)(&flags);
//						ptr2[2].Size = 4u;
//						result = WriteEvent(ref eventDescriptor, 3, (IntPtr)ptr);
//					}
//				}
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the NuiSkeletonTrackingData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="hNextFrameEvent">The value for the hNextFrameEvent parameter.</param>
//		/// <param name="dwFlags">The value for the dwFlags parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateNuiSkeletonTrackingData(ref EventDescriptor eventDescriptor, IntPtr hNextFrameEvent, uint dwFlags)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)(sizeof(EventData) * 2)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&hNextFrameEvent);
//				ptr2->Size = (uint)sizeof(IntPtr);
//				ptr2[1].DataPointer = (ulong)(&dwFlags);
//				ptr2[1].Size = 4u;
//				result = WriteEvent(ref eventDescriptor, 2, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the NuiTransformSmoothParams template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="smoothing">The value for the smoothing parameter.</param>
//		/// <param name="correction">The value for the correction parameter.</param>
//		/// <param name="prediction">The value for the prediction parameter.</param>
//		/// <param name="jitterRadius">The value for the jitterRadius parameter.</param>
//		/// <param name="maxDeviationRadius">The value for the maxDeviationRadius parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateNuiTransformSmoothParams(ref EventDescriptor eventDescriptor, float smoothing, float correction, float prediction, float jitterRadius, float maxDeviationRadius)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)(sizeof(EventData) * 5)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&smoothing);
//				ptr2->Size = 4u;
//				ptr2[1].DataPointer = (ulong)(&correction);
//				ptr2[1].Size = 4u;
//				ptr2[2].DataPointer = (ulong)(&prediction);
//				ptr2[2].Size = 4u;
//				ptr2[3].DataPointer = (ulong)(&jitterRadius);
//				ptr2[3].Size = 4u;
//				ptr2[4].DataPointer = (ulong)(&maxDeviationRadius);
//				ptr2[4].Size = 4u;
//				result = WriteEvent(ref eventDescriptor, 5, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the NuiSkeletonSetTrackedSkeletonsData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="skeleton1">The value for the skeleton1 parameter.</param>
//		/// <param name="skeleton2">The value for the skeleton2 parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateNuiSkeletonSetTrackedSkeletonsData(ref EventDescriptor eventDescriptor, uint skeleton1, uint skeleton2)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)(sizeof(EventData) * 2)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&skeleton1);
//				ptr2->Size = 4u;
//				ptr2[1].DataPointer = (ulong)(&skeleton2);
//				ptr2[1].Size = 4u;
//				result = WriteEvent(ref eventDescriptor, 2, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the NuiSetFrameEndEventData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="hEvent">The value for the hEvent parameter.</param>
//		/// <param name="dwFrameEventFlag">The value for the dwFrameEventFlag parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateNuiSetFrameEndEventData(ref EventDescriptor eventDescriptor, IntPtr hEvent, uint dwFrameEventFlag)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)(sizeof(EventData) * 2)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&hEvent);
//				ptr2->Size = (uint)sizeof(IntPtr);
//				ptr2[1].DataPointer = (ulong)(&dwFrameEventFlag);
//				ptr2[1].Size = 4u;
//				result = WriteEvent(ref eventDescriptor, 2, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the NuiImageStreamGetNextFrameData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="hStream">The value for the hStream parameter.</param>
//		/// <param name="dwMillisecondsToWait">The value for the dwMillisecondsToWait parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateNuiImageStreamGetNextFrameData(ref EventDescriptor eventDescriptor, IntPtr hStream, uint dwMillisecondsToWait)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)(sizeof(EventData) * 2)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&hStream);
//				ptr2->Size = (uint)sizeof(IntPtr);
//				ptr2[1].DataPointer = (ulong)(&dwMillisecondsToWait);
//				ptr2[1].Size = 4u;
//				result = WriteEvent(ref eventDescriptor, 2, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the NuiSkeletonGetNextFrameData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="dwMillisecondsToWait">The value for the dwMillisecondsToWait parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateNuiSkeletonGetNextFrameData(ref EventDescriptor eventDescriptor, uint dwMillisecondsToWait)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)sizeof(EventData)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&dwMillisecondsToWait);
//				ptr2->Size = 4u;
//				result = WriteEvent(ref eventDescriptor, 1, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the KinectIdStringData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="kinectId">The value for the kinectId parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateKinectIdStringData(ref EventDescriptor eventDescriptor, string kinectId)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)sizeof(EventData)];
//				EventData* ptr2 = (EventData*)ptr;
//				fixed (char* ptr3 = kinectId)
//				{
//					ptr2->DataPointer = (ulong)ptr3;
//					ptr2->Size = (uint)((kinectId.Length + 1) * 2);
//					result = WriteEvent(ref eventDescriptor, 1, (IntPtr)ptr);
//				}
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the NuiCreateSensorByIndexData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="index">The value for the index parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateNuiCreateSensorByIndexData(ref EventDescriptor eventDescriptor, int index)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)sizeof(EventData)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&index);
//				ptr2->Size = 4u;
//				result = WriteEvent(ref eventDescriptor, 1, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the NuiCreateSensorByIdData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="strInstanceId">The value for the strInstanceId parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateNuiCreateSensorByIdData(ref EventDescriptor eventDescriptor, string strInstanceId)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)sizeof(EventData)];
//				EventData* ptr2 = (EventData*)ptr;
//				fixed (char* ptr3 = strInstanceId)
//				{
//					ptr2->DataPointer = (ulong)ptr3;
//					ptr2->Size = (uint)((strInstanceId.Length + 1) * 2);
//					result = WriteEvent(ref eventDescriptor, 1, (IntPtr)ptr);
//				}
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the NuiImageStreamSetImageFrameFlagsData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="hStream">The value for the hStream parameter.</param>
//		/// <param name="dwImageFrameFlags">The value for the dwImageFrameFlags parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateNuiImageStreamSetImageFrameFlagsData(ref EventDescriptor eventDescriptor, IntPtr hStream, uint dwImageFrameFlags)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)(sizeof(EventData) * 2)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&hStream);
//				ptr2->Size = (uint)sizeof(IntPtr);
//				ptr2[1].DataPointer = (ulong)(&dwImageFrameFlags);
//				ptr2[1].Size = 4u;
//				result = WriteEvent(ref eventDescriptor, 2, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the AudioAngleData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="dAngle">The value for the dAngle parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateAudioAngleData(ref EventDescriptor eventDescriptor, double dAngle)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)sizeof(EventData)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&dAngle);
//				ptr2->Size = 8u;
//				result = WriteEvent(ref eventDescriptor, 1, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the AudioPropertyData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="pid">The value for the pid parameter.</param>
//		/// <param name="value">The value for the value parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateAudioPropertyData(ref EventDescriptor eventDescriptor, uint pid, uint value)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)(sizeof(EventData) * 2)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&pid);
//				ptr2->Size = 4u;
//				ptr2[1].DataPointer = (ulong)(&value);
//				ptr2[1].Size = 4u;
//				result = WriteEvent(ref eventDescriptor, 2, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the ManagedFramesReadyRegistrationData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="type">The value for the type parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateManagedFramesReadyRegistrationData(ref EventDescriptor eventDescriptor, int type)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)sizeof(EventData)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&type);
//				ptr2->Size = 4u;
//				result = WriteEvent(ref eventDescriptor, 1, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the ManagedImageStreamData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="type">The value for the type parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateManagedImageStreamData(ref EventDescriptor eventDescriptor, int type)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)sizeof(EventData)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&type);
//				ptr2->Size = 4u;
//				result = WriteEvent(ref eventDescriptor, 1, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the ManagedOpenNextFrameData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="streamType">The value for the streamType parameter.</param>
//		/// <param name="millisecondsWait">The value for the millisecondsWait parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateManagedOpenNextFrameData(ref EventDescriptor eventDescriptor, int streamType, int millisecondsWait)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)(sizeof(EventData) * 2)];
//				EventData* ptr2 = (EventData*)ptr;
//				ptr2->DataPointer = (ulong)(&streamType);
//				ptr2->Size = 4u;
//				ptr2[1].DataPointer = (ulong)(&millisecondsWait);
//				ptr2[1].Size = 4u;
//				result = WriteEvent(ref eventDescriptor, 2, (IntPtr)ptr);
//			}
//			return result;
//		}

//		/// <summary>
//		/// This method is a wrapper to marshal the data into the event
//		/// for the KinectSensorStartData template.
//		/// </summary>
//		/// <param name="eventDescriptor">The event description.</param>
//		/// <param name="deviceID">The value for the deviceID parameter.</param>
//		/// <returns>This method returns true if successful, and false otherwise.</returns>
//		internal unsafe bool TemplateKinectSensorStartData(ref EventDescriptor eventDescriptor, string deviceID)
//		{
//			bool result = true;
//			if (IsEnabled(eventDescriptor.Level, eventDescriptor.Keywords))
//			{
//				byte* ptr = stackalloc byte[(int)(uint)sizeof(EventData)];
//				EventData* ptr2 = (EventData*)ptr;
//				fixed (char* ptr3 = deviceID)
//				{
//					ptr2->DataPointer = (ulong)ptr3;
//					ptr2->Size = (uint)((deviceID.Length + 1) * 2);
//					result = WriteEvent(ref eventDescriptor, 1, (IntPtr)ptr);
//				}
//			}
//			return result;
//		}
//	}
//}

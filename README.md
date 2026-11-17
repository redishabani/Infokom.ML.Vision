# Infokom.ML.Vision

Infokom.ML.Vision is a .NET solution for building and experimenting with computer-vision and machine-learning applications. It is not limited to license-plate recognition (LPR): the repository includes general image-processing utilities, an ONNX Runtime-based YOLO26 object-detection component, numerical building blocks, and Kinect-related APIs. LPR is one application direction within the broader vision work.

The codebase is a collection of reusable components, desktop-application scaffolding, and demos/benchmarks rather than one finished, end-user vision product. The YOLO console benchmark is the easiest way to try the current detection flow on a video.

## What is included

| Project | Purpose |
| --- | --- |
| `Infokom.ML.Vision` | Core vision helpers that bridge RGB image arrays and OpenCvSharp matrices, resize/fit images, display images, and draw detected features. |
| `Infokom.ML.Vision.YOLO` | YOLO26 inference support using ONNX Runtime. It prepares images for model input and represents detected classes, bounding boxes, and confidence scores. |
| `Infokom.ML.Vision.YOLO.Benchmarks` | Console video demo that runs the YOLO component frame by frame and displays the input and annotated result. |
| `Infokom.ML.Vision.Apps.GUI.DesktopLPR` | WPF desktop application project intended for LPR scenarios. Its current window is a starter shell, not a complete recognition workflow. |
| `Infokom.Numerics` | Reusable numeric types and operations used by the vision code, including RGB and bounding-box-related types. |
| `Microsoft.Kinect` | Kinect API and data types for sensor-based vision scenarios. |

## How the components fit together

The core vision project uses OpenCvSharp for image and video operations and shares pixel and geometry types with `Infokom.Numerics`. The YOLO project uses ONNX Runtime to run an already-trained model; it does not train models. The benchmark ties these pieces together in a small, visible end-to-end example:

1. Open a video and read it one frame at a time.
2. Prepare each frame for the model by fitting it to the model's square input while preserving aspect ratio.
3. Run inference and decode the model output into class IDs, confidence scores, and bounding boxes.
4. Draw the detections over the frame and show the source and result in OpenCV windows.

The current detection taxonomy contains the 80 COCO object classes. This general-purpose object detector can identify classes such as cars and people, but it does not read license-plate characters (OCR) or provide a complete LPR workflow. The WPF LPR project is currently a starter UI shell. These projects are intended as components and experiments that can be extended for broader vision use cases.

## Visual frame sources

`Infokom.ML.Vision.Sensors.IVisualSensor` gives video files and cameras a common interface: each provides frames as `RGB[,]`, supports `TryGrabFrame` for pull-based processing, and implements `IObservable<RGB[,]>` for push-based processing. It also exposes `FrameWidth`, `FrameHeight`, and `FramesPerSecond` (nullable when the source does not report a rate). Observable subscriptions share one capture loop and dispose the loop when the last subscriber unsubscribes. Video-file observable playback follows the file's reported frame rate when it is available; live sources follow their device cadence.

`VideoFilePlayer` wraps OpenCvSharp's video capture. `KinectColorCamera` wraps the Kinect color stream and converts its Bgr32 pixels to RGB. The Kinect adapter selects the first connected sensor by default, or accepts a sensor ID. It uses the 640 × 480, 30 FPS color format and requires a compatible Kinect device/runtime on Windows.

Use either polling or observation for a sensor at a time; direct polling while observable subscriptions are active throws to prevent multiple readers from racing over the same source.

```csharp
using Infokom.ML.Vision.Sensors;
using System.Reactive.Linq;

using IVisualSensor sensor = VisualSensor.FromVideoFile("video.mp4");
Console.WriteLine($"{sensor.FrameWidth}×{sensor.FrameHeight} at {sensor.FramesPerSecond ?? 0} FPS");
using var subscription = sensor.Subscribe(frame =>
	Console.WriteLine($"Received {frame.GetLength(1)} × {frame.GetLength(0)} RGB frame"));
```

To use a Kinect instead, create the sensor with `VisualSensor.FromKinectColorCamera()` or pass a sensor ID. Both factory methods return `IVisualSensor`, so the rest of the frame-processing code does not depend on the source type.

For pull-based processing, call `TryGrabFrame(out var frame, millisecondsWait)` repeatedly instead. It returns `false` when a frame is not available before the timeout or when the video reaches its end. Dispose the sensor when finished to release its capture resources.

## YOLO video demo

The benchmark opens a video, runs object detection on each frame, and shows two windows: **Source** and **Result**. The result view overlays detected-object boxes and labels. The current model adapter expects a 640 × 640 input and an ONNX output tensor compatible with its YOLO-style decoder (300 detections, each with box coordinates, confidence, and class ID). Use a compatible YOLO26 model; a model with a different input or output layout will not work without adapting the inference code. The included class taxonomy is COCO-80.

### Requirements
![YOLO26 Detect](.\docs\yolo_nano_detect.gif)
- Windows and the .NET 10 SDK.
- A compatible YOLO26 ONNX model named `yolo26n-1.640.640-1.300.6.onnx`.
- A video named `tirana.mp4`.

The benchmark reads both files from a `.data` folder relative to its current working directory. When launched with the command below from the repository root, place them in `<repository>\.data\`. The paths are hard-coded in `Infokom.ML.Vision.YOLO.Benchmarks\Program.cs`; they are not command-line arguments. The repository does not bundle model weights or video assets, so obtain or provide them separately and ensure you have permission to use them.

### Build and run

From the repository root, build the benchmark:

```powershell
dotnet build .\Infokom.ML.Vision.YOLO.Benchmarks\Infokom.ML.Vision.YOLO.Benchmarks.csproj
```

Then start the demo:

```powershell
dotnet run --project .\Infokom.ML.Vision.YOLO.Benchmarks\Infokom.ML.Vision.YOLO.Benchmarks.csproj
```

The video plays with detections drawn on each frame. Press **Q** in the OpenCV window to stop playback. If the demo cannot open the video or model, check that both files exist at the expected drive-root location and that the ONNX model has the input/output format expected by this implementation.

## Repository and development

Open `Infokom.ML.Vision.slnx` in Visual Studio, or build the solution with the .NET CLI:

```powershell
dotnet build .\Infokom.ML.Vision.slnx
```

The projects target .NET 10; the desktop GUI targets Windows. The solution is evolving, so individual projects and workflows may be at different stages of completeness. Contributions and experiments can build on the shared image/numeric types, OpenCV integration, and model-inference components.

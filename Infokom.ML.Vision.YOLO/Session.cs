using Infokom.ML.Vision.YOLO.Models;

using System.Drawing;

namespace Infokom.ML.Vision.YOLO
{
	public static class Yolo
	{
		internal static Detection.Inference.Session Detect(FileInfo modelFile) => new(modelFile);
		internal static Detection.Inference.Session Detect(string modelFileName) => new(modelFileName);
	}



	public abstract class Model
	{
		
	}
}

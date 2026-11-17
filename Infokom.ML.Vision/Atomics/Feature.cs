using Infokom.Numerics.Atomics;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Infokom.ML.Vision.Atomics
{
	public interface IFeature<TTaxonomy> : ITaxonomic<TTaxonomy> where TTaxonomy : unmanaged, ITaxonomy<TTaxonomy>
	{
		public Box<float, float> BoundingBox { get; }

		public float Confidence { get; }
	}

	public readonly record struct Feature<TTaxonomy>(TTaxonomy Group, Box<float, float> BoundingBox, float Confidence) : IFeature<TTaxonomy> where TTaxonomy : unmanaged, ITaxonomy<TTaxonomy>
	{
		public override string ToString() => $"{this.Group} [{this.Confidence:P1}]";
	}


	public readonly record struct Feature(int Group, Box<float, float> BoundingBox, float Confidence)
	{
		public override string ToString() => $"{this.Group} [{this.Confidence:P1}]";
	}
}

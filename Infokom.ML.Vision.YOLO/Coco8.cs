using Infokom.Numerics.Atomics;

using System.Collections.Immutable;

namespace Infokom.ML.Vision.YOLO
{
	public abstract class Coco8
	{
		public static readonly ImmutableArray<string> NAMES = 
		[
			"person",
			"bicycle",
			"car",
			"motorcycle",
			"airplane",
			"bus",
			"train",
			"truck",
			"boat",
			"traffic light",
			"fire hydrant",
			"stop sign",
			"parking meter",
			"bench",
			"bird",
			"cat",
			"dog",
			"horse",
			"sheep",
			"cow",
			"elephant",
			"bear",
			"zebra",
			"giraffe",
			"backpack",
			"umbrella",
			"handbag",
			"tie",
			"suitcase",
			"frisbee",
			"skis",
			"snowboard",
			"sports ball",
			"kite",
			"baseball bat",
			"baseball glove",
			"skateboard",
			"surfboard",
			"tennis racket",
			"bottle",
			"wine glass",
			"cup",
			"fork",
			"knife",
			"spoon",
			"bowl",
			"banana",
			"apple",
			"sandwich",
			"orange",
			"broccoli",
			"carrot",
			"hot dog",
			"pizza",
			"donut",
			"cake",
			"chair",
			"couch",
			"potted plant",
			"bed",
			"dining table",
			"toilet",
			"tv",
			"laptop",
			"mouse",
			"remote",
			"keyboard",
			"cell phone",
			"microwave",
			"oven",
			"toaster",
			"sink",
			"refrigerator",
			"book",
			"clock",
			"vase",
			"scissors",
			"teddy bear",
			"hair drier",
			"toothbrush"
		];

		public readonly struct Taxon : ITaxonomy<Taxon>
		{
			private readonly int _order;

			private Taxon(int order) => _order = order;

			public int Id => _order - 1;

			public string Name => _order is >= 1 and <= 80 ? NAMES[_order - 1] : null;


			public override string ToString() => $"{this.Id,3}: {this.Name}";



			public static int Count => NAMES.Length;

			public static Taxon FromId(int source) => source is >= 0 and <= 79 ? new(source + 1) : throw new KeyNotFoundException();

			public static Taxon FromName(string source) => FromId(NAMES.IndexOf(source));


			public static explicit operator Taxon(int source) => new(source + 1);
			public static explicit operator checked Taxon(int source) { try { return FromId(source); } catch (KeyNotFoundException e) { throw new InvalidCastException($"Cannot cast from {source}", e); } }

			public static implicit operator int(Taxon source) => source.Id;


		}
	}
}

//using static Infokom.Numerics.Atomics.FloatingPointConstant;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Xml.XPath;

using static Infokom.Numerics.Extensions.Functions;

namespace Infokom.Numerics.Atomics
{

	[InlineArray(3)]
	public struct ByteVector3
	{
		private byte _0;

		private ByteVector3(byte b0, byte b1, byte b2)
		{
			this[0] = b0;
			this[1] = b1;
			this[2] = b2;
		}

		public static explicit operator ByteVector3(uint data) => new((byte)data, (byte)(data >> 8), (byte)(data >> 16));
		public static explicit operator checked ByteVector3(uint data) => (data & 0xFF000000u) == 0u ? new((byte)data, (byte)(data >> 8), (byte)(data >> 16)) : throw new OverflowException($"Value '{data}' causes an overflow.", new ArgumentOutOfRangeException(nameof(data), data, "Value is out of range for ByteVector3"));

		public static implicit operator uint(ByteVector3 vector) => (uint)vector[0] | ((uint)vector[1] << 8) | ((uint)vector[2] << 16);
	}

	[InlineArray(4)]
	public struct ByteVector4
	{
		private byte _0;

		private ByteVector4(byte b0, byte b1, byte b2, byte b3)
		{
			this[0] = b0;
			this[1] = b1;
			this[2] = b2;
			this[3] = b3;
		}

		public static implicit operator ByteVector4(uint data) => new((byte)data, (byte)(data >> 8), (byte)(data >> 16), (byte)(data >> 24));
		public static implicit operator uint(ByteVector4 vector) => (uint)vector[0] | ((uint)vector[1] << 8) | ((uint)vector[2] << 16) | ((uint)vector[3] << 24);
	}

	public static class ByteVector
	{
		
	}
}
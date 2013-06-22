using System;
using System.Text;

namespace Mongo.Framework.Generator
{
	/// <summary>
	/// Implementation of Murmur2 64bit hashing algorithm.
	/// Refer: https://sites.google.com/site/murmurhash/
	/// </summary>
	public class Murmur2HashGenerator : IHashGenerator
	{
		private const ulong M = 0xc6a4a7935bd1e995;
		private const int R = 47;
		private const int READ_SIZE = 8;

		public string GenerateHash(string input)
		{
			byte[] bb = Encoding.UTF8.GetBytes(input);
			const ulong seed = 0;
			ulong len = (ulong)bb.Length;
			ulong h = seed ^ (len * M);

			int pos = 0;
			int remaining = bb.Length;

			while (remaining >= READ_SIZE)
			{
				ulong k = bb.GetUInt64(pos);

				k *= M;
				k ^= k >> R;
				k *= M;

				h ^= k;
				h *= M;

				pos += READ_SIZE;
				remaining -= READ_SIZE;
			}

			if (remaining > 0)
			{
				switch (remaining)
				{
					case 7: h ^= (ulong)bb[pos + 6] << 48; goto case 6;
					case 6: h ^= (ulong)bb[pos + 5] << 40; goto case 5;
					case 5: h ^= (ulong)bb[pos + 4] << 32; goto case 4;
					case 4: h ^= (ulong)bb[pos + 3] << 24; goto case 3;
					case 3: h ^= (ulong)bb[pos + 2] << 16; goto case 2;
					case 2: h ^= (ulong)bb[pos + 1] << 8; goto case 1;
					case 1: h ^= (ulong)bb[pos + 0];
						h *= M;
						break;
				}
			}

			h ^= h >> R;
			h *= M;
			h ^= h >> R;

			return h.ToString();
		}
	}

	public static class IntHelpers
	{
		public static ulong RotateLeft(this ulong original, int bits)
		{
			return (original << bits) | (original >> (64 - bits));
		}

		public static ulong RotateRight(this ulong original, int bits)
		{
			return (original >> bits) | (original << (64 - bits));
		}

		public static ulong GetUInt64(this byte[] bb, int pos)
		{
			return BitConverter.ToUInt64(bb, pos);
		}
	}
}

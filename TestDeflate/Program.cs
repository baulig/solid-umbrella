using System;
using System.IO;
using System.Text;
using System.IO.Compression;

namespace TestDeflate
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var message = "The quick brown fox jumps over the lazy dog";
			var compressed = Compress (message);
			Console.WriteLine ($"COMPRESSED: {compressed.Length}");

			Decompress (compressed);
		}

		static byte[] Compress (string message)
		{
			var bytes = Encoding.ASCII.GetBytes (message);
			using (var ms = new MemoryStream ()) {
				using (var gzip = new GZipStream (ms, CompressionLevel.Optimal)) {
					gzip.Write (bytes, 0, bytes.Length);
				}
				return ms.ToArray ();
			}
		}

		static void Decompress (byte[] compressed)
		{
			var my = new MyStream (compressed);
			var gzip = new GZipStream (my, CompressionMode.Decompress);
			var ms = new MemoryStream ();
			gzip.CopyTo (ms);

			var decompressed = ms.ToArray ();
			var text = Encoding.ASCII.GetString (decompressed);

			Console.WriteLine ($"DECOMPRESSED: {text}");

			gzip.Close ();
		}
	}

	class MyStream : Stream
	{
		byte[] internalBuffer;
		int internalOffset;

		public MyStream (byte[] buffer)
		{
			internalBuffer = buffer;
		}

		public override bool CanRead => true;

		public override bool CanSeek => false;

		public override bool CanWrite => throw new NotImplementedException ();

		public override long Length => throw new NotImplementedException ();

		public override long Position { get => throw new NotImplementedException (); set => throw new NotImplementedException (); }

		public override void Flush ()
		{
			throw new NotImplementedException ();
		}

		public override int Read (byte[] buffer, int offset, int count)
		{
			var data = $"{buffer.Length} {offset} {count} - {internalBuffer.Length} / {internalOffset}";
			var me = $"READ ({data})";

			Console.WriteLine ($"{me}");

			var remaining = internalBuffer.Length - internalOffset;
			var effectiveLength = Math.Min (count, remaining);

			Buffer.BlockCopy (internalBuffer, internalOffset, buffer, offset, effectiveLength);
			internalOffset += effectiveLength;
			remaining -= effectiveLength;

			Console.WriteLine ($"{me} - {effectiveLength}");

			return effectiveLength;
		}

		public override long Seek (long offset, SeekOrigin origin)
		{
			throw new NotImplementedException ();
		}

		public override void SetLength (long value)
		{
			throw new NotImplementedException ();
		}

		public override void Write (byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException ();
		}

		public override void Close()
		{
			Console.WriteLine ("CLOSE");
			base.Close();
		}
	}
}

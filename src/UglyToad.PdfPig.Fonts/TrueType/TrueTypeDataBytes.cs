namespace UglyToad.PdfPig.Fonts.TrueType
{
    using System;
    using System.Text;
    using Core;

    /// <summary>
    /// Wraps the <see cref="IInputBytes"/> to support reading TrueType data types.
    /// </summary>
    public class TrueTypeDataBytes
    {
        /// <summary>
        /// The internal buffer used for reading data.
        /// </summary>
        public readonly byte[] InternalBuffer = new byte[16];
        /// <summary>
        /// The input bytes for font data.
        /// </summary>
        public readonly IInputBytes InputBytes;

        /// <summary>
        /// Create a new <see cref="TrueTypeDataBytes"/>.
        /// </summary>
        public TrueTypeDataBytes(byte[] bytes) : this(new ByteArrayInputBytes(bytes)) { }

        /// <summary>
        /// Create a new <see cref="TrueTypeDataBytes"/>.
        /// </summary>
        public TrueTypeDataBytes(IInputBytes inputBytes)
        {
            this.InputBytes = inputBytes ?? throw new ArgumentNullException(nameof(inputBytes));
        }

        /// <summary>
        /// The current position in the data.
        /// </summary>
        public long Position => InputBytes.CurrentOffset;

        /// <summary>
        /// The length of the data in bytes.
        /// </summary>
        public long Length => InputBytes.Length;

        /// <summary>
        /// Read a 32-fixed floating point value.
        /// </summary>
        public float Read32Fixed()
        {
            float retval = ReadSignedShort();
            retval += (ReadUnsignedShort() / 65536.0f);
            return retval;
        }

        /// <summary>
        /// Read a <see langword="short"/>.
        /// </summary>
        public short ReadSignedShort()
        {
            ReadBuffered(InternalBuffer, 2);

            return unchecked((short)((InternalBuffer[0] << 8) + (InternalBuffer[1] << 0)));
        }

        /// <summary>
        /// Read a <see langword="ushort"/>.
        /// </summary>
        public ushort ReadUnsignedShort()
        {
            ReadBuffered(InternalBuffer, 2);

            return (ushort)((InternalBuffer[0] << 8) + (InternalBuffer[1] << 0));
        }

        /// <summary>
        /// Read a <see langword="byte"/>.
        /// </summary>
        public byte ReadByte()
        {
            ReadBuffered(InternalBuffer, 1);

            return InternalBuffer[0];
        }

        /// <summary>
        /// Reads the 4 character tag from the TrueType file.
        /// </summary>
        public string ReadTag()
        {
            if (TryReadString(4, Encoding.UTF8, out var result))
            {
                return result;
            }

            throw new InvalidOperationException($"Could not read Tag from TrueType file at {InputBytes.CurrentOffset}.");
        }

        /// <summary>
        /// Read a <see langword="string"/> of the given number of bytes in length with the specified encoding.
        /// </summary>
        public bool TryReadString(int bytesToRead, Encoding encoding, out string result)
        {
            result = null;
            if (encoding == null)
            {
                return false;
            }

            byte[] data = new byte[bytesToRead];
            if (ReadBuffered(data, bytesToRead))
            {
                result = encoding.GetString(data, 0, data.Length);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Read a <see langword="uint"/>.
        /// </summary>
        public uint ReadUnsignedInt()
        {
            ReadBuffered(InternalBuffer, 4);

            return (uint)(((long)InternalBuffer[0] << 24) + ((long)InternalBuffer[1] << 16) + (InternalBuffer[2] << 8) + (InternalBuffer[3] << 0));
        }

        /// <summary>
        /// Read an <see langword="int"/>.
        /// </summary>
        public int ReadSignedInt()
        {
            ReadBuffered(InternalBuffer, 4);

            return (InternalBuffer[0] << 24) + (InternalBuffer[1] << 16) + (InternalBuffer[2] << 8) + (InternalBuffer[3] << 0);
        }

        /// <summary>
        /// Read a <see langword="long"/>.
        /// </summary>
        public long ReadLong()
        {
            var upper = (long)ReadSignedInt();
            var lower = ReadSignedInt();
            var result = (upper << 32) + (lower & 0xFFFFFFFF);
            return result;
        }
        
        /// <summary>
        /// Read a <see cref="DateTime"/> from the data in UTC time.
        /// In TrueType dates are specified as the number of seconds since 1904-01-01.
        /// </summary>
        public DateTime ReadInternationalDate()
        {
            var secondsSince1904 = ReadLong();

            var date = new DateTime(1904, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            try
            {
                var result = date.AddSeconds(secondsSince1904);

                return result;
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidFontFormatException($"Invalid date offset ({secondsSince1904} seconds) encountered in TrueType header table.");
            }
        }

        /// <summary>
        /// Move to the specified position in the data.
        /// </summary>
        public void Seek(long position)
        {
            InputBytes.Seek(position);
        }

        /// <summary>
        /// Read an <see langword="int"/> which represents a signed byte.
        /// </summary>
        public int ReadSignedByte()
        {
            ReadBuffered(InternalBuffer, 1);

            var signedByte = InternalBuffer[0];

            return signedByte < 127 ? signedByte : signedByte - 256;
        }

        /// <summary>
        /// Read an array of <see langword="ushort"/>s with the specified number of values.
        /// </summary>
        public ushort[] ReadUnsignedShortArray(int length)
        {
            var result = new ushort[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = ReadUnsignedShort();
            }

            return result;
        }

        /// <summary>
        /// Read an array of <see langword="byte"/>s with the specified number of values.
        /// </summary>
        public byte[] ReadByteArray(int length)
        {
            var result = new byte[length];

            ReadBuffered(result, length);

            return result;
        }

        /// <summary>
        /// Read an array of <see langword="uint"/>s with the specified number of values.
        /// </summary>
        public uint[] ReadUnsignedIntArray(int length)
        {
            var result = new uint[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = ReadUnsignedInt();
            }

            return result;
        }

        /// <summary>
        /// Read an array of <see langword="short"/>s with the specified number of values.
        /// </summary>
        public short[] ReadShortArray(int length)
        {
            var result = new short[length];

            for (int i = 0; i < length; i++)
            {
                result[i] = ReadSignedShort();
            }

            return result;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"@: {Position} of {InputBytes.Length} bytes.";
        }

        private bool ReadBuffered(byte[] buffer, int length)
        {
            var read = InputBytes.Read(buffer, length);
            if (read < length)
            {
                return false;
            }

            return true;
        }
    }
}

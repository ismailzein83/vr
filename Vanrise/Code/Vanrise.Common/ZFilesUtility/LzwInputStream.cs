﻿using System;
using System.IO;

namespace Vanrise.Common
{
    public static class LzwInputStreamHelper
    {
        public static MemoryStream ConvertToMemoryStream(LzwInputStream inputStream)
        {
            byte[] buffer = new byte[4096];
            MemoryStream memoryStream = new MemoryStream();

            int read;
            while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
                memoryStream.Write(buffer, 0, read);

            return memoryStream;
        }
    }

    /// <summary>
    /// This class contains constants used for LZW
    /// </summary>
    sealed public class LzwConstants
    {
        /// <summary>
        /// Magic number found at start of LZW header: 0x1f 0x9d
        /// </summary>
        public const int MAGIC = 0x1f9d;

        /// <summary>
        /// Maximum number of bits per code
        /// </summary>
        public const int MAX_BITS = 16;

        /* 3rd header byte:
         * bit 0..4 Number of compression bits
         * bit 5    Extended header
         * bit 6    Free
         * bit 7    Block mode
         */

        /// <summary>
        /// Mask for 'number of compression bits'
        /// </summary>
        public const int BIT_MASK = 0x1f;

        /// <summary>
        /// Indicates the presence of a fourth header byte
        /// </summary>
        public const int EXTENDED_MASK = 0x20;
        //public const int FREE_MASK      = 0x40;

        /// <summary>
        /// Reserved bits
        /// </summary>
        public const int RESERVED_MASK = 0x60;

        /// <summary>
        /// Block compression: if table is full and compression rate is dropping,
        /// clear the dictionary.
        /// </summary>
        public const int BLOCK_MODE_MASK = 0x80;

        /// <summary>
        /// LZW file header size (in bytes)
        /// </summary>
        public const int HDR_SIZE = 3;

        /// <summary>
        /// Initial number of bits per code
        /// </summary>
        public const int INIT_BITS = 9;

        LzwConstants()
        {
        }
    }


    /// <summary>
    /// This filter stream is used to decompress a LZW format stream.
    /// Specifically, a stream that uses the LZC compression method.
    /// This file format is usually associated with the .Z file extension.
    ///
    /// See http://en.wikipedia.org/wiki/Compress
    /// See http://wiki.wxwidgets.org/Development:_Z_File_Format
    ///
    /// The file header consists of 3 (or optionally 4) bytes. The first two bytes
    /// contain the magic marker "0x1f 0x9d", followed by a byte of flags.
    ///
    /// Based on Java code by Ronald Tschalar, which in turn was based on the unlzw.c
    /// code in the gzip package.
    /// </summary>
    public class LzwInputStream : Stream
    {
        byte[] one = new byte[1];
        bool headerParsed = false;

        // string table stuff
        private const int TBL_CLEAR = 0x100;
        private const int TBL_FIRST = TBL_CLEAR + 1;

        private int[] tabPrefix;
        private byte[] tabSuffix;
        private int[] zeros = new int[256];
        private byte[] stack;

        // various state
        private bool blockMode;
        private int nBits;
        private int maxBits;
        private int maxMaxCode;
        private int maxCode;
        private int bitMask;
        private int oldCode;
        private byte finChar;
        private int stackP;
        private int freeEnt;

        // input buffer
        private byte[] data = new byte[10000];
        private int bitPos = 0, end = 0, got = 0;
        private bool eof = false;
        private const int EXTRA = 64;

        /// <summary>
        /// Creates a LzwInputStream
        /// </summary>
        /// <param name="baseInputStream">
        /// The stream to read compressed data from (baseInputStream LZW format)
        /// </param>
        public LzwInputStream(Stream baseInputStream)
        {
            this.baseInputStream = baseInputStream;
        }

        public LzwInputStream()
        {
        }

        /// <summary>
        /// See <see cref="System.IO.Stream.ReadByte"/>
        /// </summary>
        /// <returns></returns>
        public override int ReadByte()
        {
            int b = Read(one, 0, 1);
            if (b == 1)
                return (one[0] & 0xff);
            else
                return -1;
        }

        /// <summary>
        /// Moves the unread data in the buffer to the beginning and resets
        /// the pointers.
        /// </summary>
        /// <param name="bitPos"></param>
        /// <returns></returns>
        private int ResetBuf(int bitPos)
        {
            int pos = bitPos >> 3;
            Array.Copy(data, pos, data, 0, end - pos);
            end -= pos;
            return 0;
        }


        private void Fill()
        {
            got = baseInputStream.Read(data, end, data.Length - 1 - end);
            if (got > 0) end += got;
        }


        private void ParseHeader()
        {
            headerParsed = true;

            byte[] hdr = new byte[LzwConstants.HDR_SIZE];

            int result = baseInputStream.Read(hdr, 0, hdr.Length);

            // Check the magic marker
            if (result < 0) throw new ApplicationException("Failed to read LZW header");
            if (hdr[0] != (LzwConstants.MAGIC >> 8) || hdr[1] != (LzwConstants.MAGIC & 0xff))
            {
                throw new ApplicationException(String.Format(
                    "Wrong LZW header. Magic bytes don't match. 0x{0:x2} 0x{1:x2}",
                    hdr[0], hdr[1]));
            }

            // Check the 3rd header byte
            blockMode = (hdr[2] & LzwConstants.BLOCK_MODE_MASK) > 0;
            maxBits = hdr[2] & LzwConstants.BIT_MASK;

            if (maxBits > LzwConstants.MAX_BITS)
            {
                throw new ApplicationException("Stream compressed with " + maxBits +
                    " bits, but decompression can only handle " +
                    LzwConstants.MAX_BITS + " bits.");
            }

            if ((hdr[2] & LzwConstants.RESERVED_MASK) > 0)
            {
                throw new ApplicationException("Unsupported bits set in the header.");
            }

            // Initialize variables
            maxMaxCode = 1 << maxBits;
            nBits = LzwConstants.INIT_BITS;
            maxCode = (1 << nBits) - 1;
            bitMask = maxCode;
            oldCode = -1;
            finChar = 0;
            freeEnt = blockMode ? TBL_FIRST : 256;

            tabPrefix = new int[1 << maxBits];
            tabSuffix = new byte[1 << maxBits];
            stack = new byte[1 << maxBits];
            stackP = stack.Length;

            for (int idx = 255; idx >= 0; idx--)
                tabSuffix[idx] = (byte)idx;
        }

        #region Stream Overrides

        /// <summary>
        /// Reads decompressed data into the provided buffer byte array
        /// </summary>
        /// <param name ="buffer">
        /// The array to read and decompress data into
        /// </param>
        /// <param name ="offset">
        /// The offset indicating where the data should be placed
        /// </param>
        /// <param name ="count">
        /// The number of bytes to decompress
        /// </param>
        /// <returns>The number of bytes read. Zero signals the end of stream</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!headerParsed) ParseHeader();

            if (eof) return -1;
            int start = offset;

            /* Using local copies of various variables speeds things up by as
             * much as 30% in Java! Performance not tested in C#.
             */
            int[] lTabPrefix = tabPrefix;
            byte[] lTabSuffix = tabSuffix;
            byte[] lStack = stack;
            int lNBits = nBits;
            int lMaxCode = maxCode;
            int lMaxMaxCode = maxMaxCode;
            int lBitMask = bitMask;
            int lOldCode = oldCode;
            byte lFinChar = finChar;
            int lStackP = stackP;
            int lFreeEnt = freeEnt;
            byte[] lData = data;
            int lBitPos = bitPos;


            // empty stack if stuff still left
            int sSize = lStack.Length - lStackP;
            if (sSize > 0)
            {
                int num = (sSize >= count) ? count : sSize;
                Array.Copy(lStack, lStackP, buffer, offset, num);
                offset += num;
                count -= num;
                lStackP += num;
            }

            if (count == 0)
            {
                stackP = lStackP;
                return offset - start;
            }


        // loop, filling local buffer until enough data has been decompressed
        MainLoop: do
            {
                if (end < EXTRA) Fill();

                int bitIn = (got > 0) ? (end - end % lNBits) << 3 :
                                        (end << 3) - (lNBits - 1);

                while (lBitPos < bitIn)
                {
                    #region A
                    // handle 1-byte reads correctly
                    if (count == 0)
                    {
                        nBits = lNBits;
                        maxCode = lMaxCode;
                        maxMaxCode = lMaxMaxCode;
                        bitMask = lBitMask;
                        oldCode = lOldCode;
                        finChar = lFinChar;
                        stackP = lStackP;
                        freeEnt = lFreeEnt;
                        bitPos = lBitPos;

                        return offset - start;
                    }

                    // check for code-width expansion
                    if (lFreeEnt > lMaxCode)
                    {
                        int nBytes = lNBits << 3;
                        lBitPos = (lBitPos - 1) +
                        nBytes - (lBitPos - 1 + nBytes) % nBytes;

                        lNBits++;
                        lMaxCode = (lNBits == maxBits) ? lMaxMaxCode :
                                                        (1 << lNBits) - 1;

                        lBitMask = (1 << lNBits) - 1;
                        lBitPos = ResetBuf(lBitPos);
                        goto MainLoop;
                    }
                    #endregion

                    #region B
                    // read next code
                    int pos = lBitPos >> 3;
                    int code = (((lData[pos] & 0xFF) |
                        ((lData[pos + 1] & 0xFF) << 8) |
                        ((lData[pos + 2] & 0xFF) << 16)) >>
                        (lBitPos & 0x7)) & lBitMask;

                    lBitPos += lNBits;

                    // handle first iteration
                    if (lOldCode == -1)
                    {
                        if (code >= 256) throw new ApplicationException("corrupt input: " + code + " > 255");

                        lFinChar = (byte)(lOldCode = code);
                        buffer[offset++] = lFinChar;
                        count--;
                        continue;
                    }

                    // handle CLEAR code
                    if (code == TBL_CLEAR && blockMode)
                    {
                        Array.Copy(zeros, 0, lTabPrefix, 0, zeros.Length);
                        lFreeEnt = TBL_FIRST - 1;

                        int nBytes = lNBits << 3;
                        lBitPos = (lBitPos - 1) + nBytes - (lBitPos - 1 + nBytes) % nBytes;
                        lNBits = LzwConstants.INIT_BITS;
                        lMaxCode = (1 << lNBits) - 1;
                        lBitMask = lMaxCode;

                        // Code tables reset

                        lBitPos = ResetBuf(lBitPos);
                        goto MainLoop;
                    }
                    #endregion

                    #region C
                    // setup
                    int inCode = code;
                    lStackP = lStack.Length;

                    // Handle KwK case
                    if (code >= lFreeEnt)
                    {
                        if (code > lFreeEnt)
                        {
                            throw new ApplicationException("corrupt input: code=" + code +
                                ", freeEnt=" + lFreeEnt);
                        }

                        lStack[--lStackP] = lFinChar;
                        code = lOldCode;
                    }

                    // Generate output characters in reverse order
                    while (code >= 256)
                    {
                        lStack[--lStackP] = lTabSuffix[code];
                        code = lTabPrefix[code];
                    }

                    lFinChar = lTabSuffix[code];
                    buffer[offset++] = lFinChar;
                    count--;

                    // And put them out in forward order
                    sSize = lStack.Length - lStackP;
                    int num = (sSize >= count) ? count : sSize;
                    Array.Copy(lStack, lStackP, buffer, offset, num);
                    offset += num;
                    count -= num;
                    lStackP += num;
                    #endregion

                    #region D
                    // generate new entry in table
                    if (lFreeEnt < lMaxMaxCode)
                    {
                        lTabPrefix[lFreeEnt] = lOldCode;
                        lTabSuffix[lFreeEnt] = lFinChar;
                        lFreeEnt++;
                    }

                    // Remember previous code
                    lOldCode = inCode;

                    // if output buffer full, then return
                    if (count == 0)
                    {
                        nBits = lNBits;
                        maxCode = lMaxCode;
                        bitMask = lBitMask;
                        oldCode = lOldCode;
                        finChar = lFinChar;
                        stackP = lStackP;
                        freeEnt = lFreeEnt;
                        bitPos = lBitPos;

                        return offset - start;
                    }
                    #endregion
                }   // while

                lBitPos = ResetBuf(lBitPos);

            } while (got > 0);  // do..while

            nBits = lNBits;
            maxCode = lMaxCode;
            bitMask = lBitMask;
            oldCode = lOldCode;
            finChar = lFinChar;
            stackP = lStackP;
            freeEnt = lFreeEnt;
            bitPos = lBitPos;

            eof = true;
            return offset - start;
        }


        /// <summary>
        /// Gets a value indicating whether the current stream supports reading
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return baseInputStream.CanRead;
            }
        }

        /// <summary>
        /// Gets a value of false indicating seeking is not supported for this stream.
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value of false indicating that this stream is not writeable.
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// A value representing the length of the stream in bytes.
        /// Always throws a NotSupportedException when attempting to get the length.
        /// </summary>
        /// <exception cref="NotSupportedException">Attempting to get the length</exception>
        public override long Length
        {
            get
            {
                throw new NotSupportedException("LzwInputStream Length not supported");
            }
        }

        /// <summary>
        /// The current position within the stream.
        /// Throws a NotSupportedException when attempting to set the position.
        /// </summary>
        /// <exception cref="NotSupportedException">Attempting to set the position</exception>
        public override long Position
        {
            get
            {
                return baseInputStream.Position;
            }
            set
            {
                throw new NotSupportedException("LzwInputStream Position not supported");
            }
        }

        /// <summary>
        /// Flushes the baseInputStream
        /// </summary>
        public override void Flush()
        {
            baseInputStream.Flush();
        }

        /// <summary>
        /// Sets the position within the current stream
        /// Always throws a NotSupportedException
        /// </summary>
        /// <param name="offset">The relative offset to seek to.</param>
        /// <param name="origin">The <see cref="SeekOrigin"/> defining where to seek from.</param>
        /// <returns>The new position in the stream.</returns>
        /// <exception cref="NotSupportedException">Any access</exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("LzwInputStream Seek not supported");
        }

        /// <summary>
        /// Set the length of the current stream
        /// Always throws a NotSupportedException
        /// </summary>
        /// <param name="value">The new length value for the stream.</param>
        /// <exception cref="NotSupportedException">Any access</exception>
        public override void SetLength(long value)
        {
            throw new NotSupportedException("LzwInputStream SetLength not supported");
        }

        /// <summary>
        /// Writes a sequence of bytes to stream and advances the current position
        /// This method always throws a NotSupportedException
        /// </summary>
        /// <param name="buffer">Thew buffer containing data to write.</param>
        /// <param name="offset">The offset of the first byte to write.</param>
        /// <param name="count">The number of bytes to write.</param>
        /// <exception cref="NotSupportedException">Any access</exception>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException("LzwInputStream Write not supported");
        }

        /// <summary>
        /// Writes one byte to the current stream and advances the current position
        /// Always throws a NotSupportedException
        /// </summary>
        /// <param name="value">The byte to write.</param>
        /// <exception cref="NotSupportedException">Any access</exception>
        public override void WriteByte(byte value)
        {
            throw new NotSupportedException("LzwInputStream WriteByte not supported");
        }

        /// <summary>
        /// Entry point to begin an asynchronous write.  Always throws a NotSupportedException.
        /// </summary>
        /// <param name="buffer">The buffer to write data from</param>
        /// <param name="offset">Offset of first byte to write</param>
        /// <param name="count">The maximum number of bytes to write</param>
        /// <param name="callback">The method to be called when the asynchronous write operation is completed</param>
        /// <param name="state">A user-provided object that distinguishes this particular asynchronous write request from other requests</param>
        /// <returns>An <see cref="System.IAsyncResult">IAsyncResult</see> that references the asynchronous write</returns>
        /// <exception cref="NotSupportedException">Any access</exception>
        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException("LzwInputStream BeginWrite not supported");
        }

        /// <summary>
        /// Closes the input stream.  When <see cref="IsStreamOwner"></see>
        /// is true the underlying stream is also closed.
        /// </summary>
        public override void Close()
        {
            if (!isClosed)
            {
                isClosed = true;
                if (isStreamOwner)
                {
                    baseInputStream.Close();
                }
            }
        }

        #endregion


        /// <summary>
        /// Get/set flag indicating ownership of underlying stream.
        /// When the flag is true <see cref="Close"/> will close the underlying stream also.
        /// </summary>
        /// <remarks>
        /// The default value is true.
        /// </remarks>
        public bool IsStreamOwner
        {
            get { return isStreamOwner; }
            set { isStreamOwner = value; }
        }

        /// <summary>
        /// Base stream the inflater reads from.
        /// </summary>
        protected Stream baseInputStream;

        /// <summary>
        /// Flag indicating wether this instance has been closed or not.
        /// </summary>
        bool isClosed;

        /// <summary>
        /// Flag indicating wether this instance is designated the stream owner.
        /// When closing if this flag is true the underlying stream is closed.
        /// </summary>
        bool isStreamOwner = true;
    }
}

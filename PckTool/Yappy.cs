using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PckTool
{
    class Yappy
    {
        private readonly int[] _infos = new int[256];
        private readonly byte[,] _maps = new byte[32, 16];

        public Yappy()
        {
            ulong step = 1 << 16;

            for (var i = 0; i < 16; i++)
            {
                ulong value = 65535;
                step = (step * 67537) >> 16;
                while (value < (29UL << 16))
                {
                    _maps[value >> 16, i] = 1;
                    value = (value * step) >> 16;
                }
            }

            var cntr = 0;
            for (var i = 0; i < 29; i++)
            {
                for (var j = 0; j < 16; j++)
                {
                    if (_maps[i, j] != 0)
                    {
                        _infos[32 + cntr] = i + 4 + (j << 8);
                        _maps[i, j] = (byte)(32 + cntr);
                        cntr++;
                    }
                    else
                    {
                        if (i == 0)
                        {
                            throw new Exception("i == 0");
                        }
                        _maps[i, j] = _maps[i - 1, j];
                    }
                }
            }

            if (cntr != 256 - 32)
            {
                throw new Exception("init error");
            }
        }

        unsafe void Copy(byte* data, byte* to)
        {
            *(ulong*)to = *(ulong*)data;
            *((ulong*)to + 1) = *((ulong*)data + 1);
        }

        public unsafe byte[] Uncompress(byte[] dataBytes, int length, int originalSize)
        {
            var toBytes = new byte[originalSize];
            byte* data;
            byte* to;
            fixed (byte* p1 = dataBytes)
            fixed (byte* p2 = toBytes)
            {
                data = p1;
                to = p2;
            }
            var end = data + length;
            while (data < end)
            {
                int index = data[0];
                if (index < 32)
                {
                    Copy(data + 1, to);
                    if (index > 15)
                    {
                        Copy(data + 17, to + 16);
                    }
                    to += index + 1;
                    data += index + 2;
                }
                else
                {
                    int info = _infos[index];
                    int len = info & 0x00ff;
                    int offset = (info & 0xff00) + data[1];

                    Copy(to - offset, to);
                    if (len > 16)
                    {
                        Copy(to - offset + 16, to + 16);
                    }
                    to += len;
                    data += 2;
                }
            }

            return toBytes;
        }

        unsafe int Match(byte* data, int i, int j, int size)
        {
            if (*(uint*)(data + i) != *(uint*)(data + j))
            {
                return 0;
            }
            var k = 4;
            var bound = i - j;
            bound = bound > size ? size : bound;
            bound = bound > 32 ? 32 : bound;
            for (; k < bound && data[i + k] == data[j + k]; k++)
            {
            }
            return k < bound ? k : bound;
        }

        ulong Hash(ulong value)
        {
            return ((value * 912367421UL) >> 24) & 4095;
        }

        unsafe void Link(int[] hashes, int[] nodes, int i, byte* data)
        {
            var idx = (int)Hash(*(uint*)(data + i));
            var hashValue = hashes[idx];
            nodes[i & 4095] = hashValue;
            hashes[idx] = i;
        }

        unsafe byte[] GenerateBuffer(int size, out byte* pointer)
        {
            var buffer = new byte[size];
            fixed (byte* p = buffer)
            {
                pointer = p;
            }
            return buffer;
        }

        public unsafe byte[] Compress(byte[] dataBytes, int level)
        {
            var len = dataBytes.Length;
            byte* to;
            var buffer = GenerateBuffer(len, out to);
            var start = to;

            byte* data;
            fixed (byte* p = dataBytes)
            {
                data = p;
            }
            var hashes = new int[4096];
            var nodes = new int[4096];
            byte end = 0xff;
            var optr = &end;

            for (var i = 0; i < 4096; i++)
            {
                hashes[i] = -1;
            }
            for (var i = 0; i < len;)
            {
                var coded = data[i];
                Link(hashes, nodes, i, data);

                var bestMatch = 3;
                ushort bestCode = 0;

                var ptr = i;
                var tries = 0;

                while (true)
                {
                    var newPtr = nodes[ptr & 4095];
                    if (newPtr >= ptr || i - newPtr >= 4095 || ((level >= 0) && (tries > level)))
                    {
                        break;
                    }
                    ptr = newPtr;
                    var match = Match(data, i, ptr, len - i);

                    if (bestMatch < match)
                    {
                        byte code = _maps[match - 4, (i - ptr) >> 8];
                        match = _infos[code] & 0xff;

                        if (bestMatch < match)
                        {
                            bestMatch = match;
                            bestCode = (ushort)(code + (((i - ptr) & 0xff) << 8));
                            if (bestMatch == 32)
                            {
                                break;
                            }
                        }
                    }

                    if (match > 3)
                    {
                        tries++;
                    }
                }

                if (optr[0] > 30)
                {
                    optr = &end;
                }

                if (bestMatch > 3)
                {
                    *(ushort*)to = bestCode;

                    for (var k = 1; k < bestMatch; k++)
                    {
                        Link(hashes, nodes, i + k, data);
                    }
                    i += bestMatch;
                    to += 2;
                    optr = &end;
                }
                else
                {
                    if (optr[0] == 0xff)
                    {
                        optr = to;
                        optr[0] = 0xff;
                        to++;
                    }
                    optr[0]++;
                    to[0] = coded;
                    to++;
                    i++;
                }
            }

            // result
            var compressed = new byte[to - start];
            Array.Copy(buffer, compressed, compressed.Length);

            return compressed;
        }
    }
}

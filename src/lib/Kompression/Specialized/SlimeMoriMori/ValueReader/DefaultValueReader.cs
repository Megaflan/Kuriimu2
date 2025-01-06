﻿using Komponent.IO;
using Kompression.InternalContract.SlimeMoriMori.ValueReader;

namespace Kompression.Specialized.SlimeMoriMori.ValueReader
{
    class DefaultValueReader : IValueReader
    {
        public void BuildTree(BinaryBitReader br)
        {
            // We don't have a tree here
        }

        public byte ReadValue(BinaryBitReader br)
        {
            return br.ReadBits<byte>(8);
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
}

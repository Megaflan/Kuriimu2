﻿using System.Security.Cryptography;
using System.Text;
using Kryptography.Contract.Checksum;

namespace Kryptography.Checksum
{
    public class Sha256 : IChecksum
    {
        private static readonly SHA256 HashInstance = SHA256.Create();

        public byte[] Compute(Span<byte> input) => HashInstance.ComputeHash(input.ToArray());

        public byte[] Compute(string input) => HashInstance.ComputeHash(Encoding.ASCII.GetBytes(input));

        public byte[] Compute(string input, Encoding enc) => HashInstance.ComputeHash(enc.GetBytes(input));

        public byte[] Compute(Stream input) => HashInstance.ComputeHash(input);
    }
}

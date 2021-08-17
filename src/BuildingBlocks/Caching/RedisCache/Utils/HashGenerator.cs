﻿using System;
using System.Security.Cryptography;

namespace AHAM.BuildingBlocks.RedisCache.Utils
{
    public class HashGenerator
    {
        public static string CreateHash(byte[] data, string hashAlgorithm, int trimByteCount = 0)
        {
            if (string.IsNullOrEmpty(hashAlgorithm))
                throw new ArgumentNullException(nameof(hashAlgorithm));

            var algorithm = (HashAlgorithm) CryptoConfig.CreateFromName(hashAlgorithm);
            if (algorithm == null)
                throw new ArgumentException("Unrecognized hash name");

            if (trimByteCount <= 0 || data.Length <= trimByteCount)
                return BitConverter.ToString(algorithm.ComputeHash(data)).Replace("-", string.Empty);

            var newData = new byte[trimByteCount];
            Array.Copy(data, newData, trimByteCount);

            return BitConverter.ToString(algorithm.ComputeHash(newData)).Replace("-", string.Empty);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AIS.Domain.ImageFiles
{
    public struct Md5Info : IEquatable<Md5Info>
    {
        private readonly byte[] _md5ByteArray;
        public Md5Info(ImageFilePath imageFilePath)
        {
            if (imageFilePath is null)
            {
                throw new ArgumentNullException(nameof(imageFilePath));
            }

            using var fileStream = new StreamReader(imageFilePath.GetValue());
            using var md5Reader = MD5.Create();
            var md5 = md5Reader.ComputeHash(fileStream.BaseStream);
            if (md5.Length > 16)
                throw new Exception($"Md5 lenght exceed length");

            _md5ByteArray = md5;
        }

        public override bool Equals(object obj)
        {
            return obj is Md5Info info && Equals(info);
        }

        public bool Equals(Md5Info other)
        {
            return ReferenceEquals(this._md5ByteArray, other._md5ByteArray) ||
                Enumerable.SequenceEqual(this._md5ByteArray, other._md5ByteArray);
        }

        public byte[] GetFileMd5()
        {
            Span<byte> md5Clone = new byte[_md5ByteArray.Length];
            _md5ByteArray.CopyTo(md5Clone);
            return md5Clone.ToArray();
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return ToHex(_md5ByteArray, true);
        }

        private string ToHex(byte[] bytes, bool upperCase)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);

            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));

            return result.ToString();
        }

        public static bool operator ==(Md5Info left, Md5Info right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Md5Info left, Md5Info right)
        {
            return !(left == right);
        }
    }
}

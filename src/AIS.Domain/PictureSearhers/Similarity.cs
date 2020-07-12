using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace AIS.Domain.PictureSearhers
{
    public struct Similarity : IEquatable<Similarity>
    {
        public Similarity(int value)
        {
            Value = 0;
            SetValue(value);
        }

        private int Value { get; set; }

        public bool Equals(Similarity other)
            => Value == other.Value;

        private void SetValue(int newValue)
        {
            if (newValue < 0 || newValue > 100)
                throw new ArgumentOutOfRangeException("Значение должно быть между 0 и 100 включительно");

            Value = newValue;
        }

        public static implicit operator int(Similarity similarity)
            => similarity.Value;

        public static implicit operator Similarity(int value) =>
            new Similarity(value);
    }
}

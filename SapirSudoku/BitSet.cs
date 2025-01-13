using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Math2;

namespace SapirBitSet
{
    public class BitSet
    {
        private int[] set;
        private int size;

        public BitSet(int size)
        {
            this.size = size;
            set = new int[(size + 31) >> 5];
        }
        public BitSet(BitSet set)
        {
            this.size = set.size;
            this.set = (int[])set.set.Clone();
        }

        public void Add(int value)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException("value Index out of range.");

            if (value >> 5 >= set.Length - 1)
                Expand(value + 1);

            Console.WriteLine();
            Console.WriteLine(value >> 5);
            Console.WriteLine(set.Length);
            set[value >> 5] |= (1 << (value % 32));
        }

        private void Expand(int capacity)
        {
            if (capacity >> 5 >= set.Length - 1)
            {
                int[] newSet = new int[(capacity + 31) >> 5];
                set.CopyTo(newSet, 0);
                this.set = newSet;
                this.size = capacity;
            }
        }

        public void Remove(int index)
        {
            if (index < 0 || index >= size)
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range.");

            set[index >> 5] &= ~(1 << (index % 32));
        }

        public void Toggle(int index)
        {
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range.");

            if (index >> 5 >= set.Length - 1)
                Expand(index + 1);

            set[index >> 5] ^= (1 << (index % 32));
        }

        public bool Contains(int index)
        {
            if (index < 0 )
                throw new ArgumentOutOfRangeException(nameof(index), "Index out of range.");
            if (index >= size)
                return false;

            return (set[index / 32] & (1 << (index % 32))) != 0;
        }

        public void ClearAll()
        {
            Array.Clear(set, 0, set.Length);
        }

        public static BitSet Union(params BitSet[]sets)
        {
            if (sets.Length == 0)
                new BitSet(0);

            int length = sets[0].size;
            foreach (BitSet s in sets)
                length = MathUtils.Max(length, s.size);

            BitSet unionSet = new BitSet(length);
            foreach (BitSet s in sets)
                for (int i = 0; i < s.set.Length; i++)
                {
                    Console.WriteLine("HERE");
                    unionSet.set[i] |= s.set[i];
                }

            return unionSet;
        }

        public static BitSet Intersection(params BitSet[] sets)
        {
            if (sets.Length == 0)
                new BitSet(0);

            int length = sets[0].size;
            BitSet Smallest = sets[0];
            foreach (BitSet s in sets)
            {
                if (s.size < length)
                {
                    length = s.size;
                    Smallest = s;
                }
            }

            BitSet interSet = new BitSet(Smallest);
            foreach (BitSet s in sets)
                for (int i = 0; i < interSet.set.Length; i++)
                    interSet.set[i] &= s.set[i];

            return interSet;
        }

        public static BitSet Subtract(BitSet subtracted, BitSet subtracter)
        {
            BitSet subtractSet = new BitSet(subtracted);
            for (int i = 0; i < Math.Min(subtractSet.set.Length, subtracter.set.Length); i++)
                subtractSet.set[i] &= ~subtracter.set[i];

            return subtractSet;
        }



        // REMOVE LATERRR
        public override string ToString()
        {
            char[] bits = new char[size];
            for (int i = 0; i < size; i++)
                bits[i] = Contains(i) ? '1' : '0';
            return new string(bits);
        }


    }
}

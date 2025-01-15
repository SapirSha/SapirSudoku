﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Xml;
using SapirMath;

namespace SapirBitSet
{
    public class BitSet
    {
        // Object size is considered as the amount of values that you can put (in other words bit size)
        private static readonly int OBJECT_SIZE_BYTE = sizeof(int);
        private static readonly int OBJECT_SIZE_BIT = OBJECT_SIZE_BYTE << 3;

        private static readonly int DIV_BY_OBJECT_SIZE = OBJECT_SIZE_BYTE + 1; // used in bitwise '>>'


        private int[] set;

        public BitSet(int size)
        {
            set = new int[(size + (OBJECT_SIZE_BIT - 1)) >> DIV_BY_OBJECT_SIZE];
        }
        public BitSet(BitSet set)
        {
            this.set = (int[])set.set.Clone();
        }
        private void Expand(int capacity)
        {
            if ((capacity - 1) >> DIV_BY_OBJECT_SIZE >= set.Length)
            {
                int[] newSet = new int[(capacity + (OBJECT_SIZE_BIT - 1)) >> DIV_BY_OBJECT_SIZE];
                set.CopyTo(newSet, 0);
                this.set = newSet;
            }
        }

        public int Count()
        {
            int count = 0;
            foreach (var holder in set)
                count += BitOperations.PopCount((uint)holder);
            return count;
            /*
            int count = 0;
            foreach (var holder in set)
            {
                int clone = holder;
                while (clone != 0)
                {
                    count++;
                    clone &= clone - 1;
                }
            }
            return count;
            */
        }

        public bool IsEmpty(int i)
        {
            foreach (var holder in set)
                if (holder != 0)
                    return false;
            return true;
        }

        public int GetSmallest()
        {
            int count = 0;
            for (int i = 0; i < set.Length; i++)
            {
                if (set[i] != 0)
                    return count + BitOperations.TrailingZeroCount(set[i]);
                count += OBJECT_SIZE_BIT;
            }
            return -1;
        }

        public int GetLargest()
        {
            int count = OBJECT_SIZE_BIT * set.Length;
            for (int i = set.Length - 1; i >= 0; i++)
            {
                if (set[i] != 0)
                    return count - BitOperations.LeadingZeroCount((uint)set[i]);
                count -= OBJECT_SIZE_BIT;
            }
            return -1;
        }

        public bool Contains(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException("Value Cannot Be Zero Or Less!");

            if (((value - 1) >> DIV_BY_OBJECT_SIZE) >= set.Length)
                return false;

            return (set[(value - 1) >> DIV_BY_OBJECT_SIZE] & (1 << (value - 1))) != 0;
        }

        public void Add(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException("Value Cannot Be Zero Or Less!");

            if (((value - 1) >> DIV_BY_OBJECT_SIZE) >= set.Length)
                Expand(value);

            set[(value - 1) >> DIV_BY_OBJECT_SIZE] |= (1 << (value - 1));
        }

        public void Remove(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException("Value Cannot Be Zero Or Less!");

            if (((value - 1) >> DIV_BY_OBJECT_SIZE) < set.Length)
                set[(value - 1) >> DIV_BY_OBJECT_SIZE] &= ~((1 << (value - 1)));
        }

        public void Toggle(int value)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException("Value Cannot Be Zero Or Less!");

            if (((value - 1) >> DIV_BY_OBJECT_SIZE) >= set.Length)
                Expand(value); // Has to be On

            set[(value - 1) >> DIV_BY_OBJECT_SIZE] ^= (1 << (value - 1));
        }

        public void ClearAll()
        {
            Array.Clear(set, 0, set.Length);
        }

        public static  BitSet Union(params BitSet[]sets)
        {
            if (sets.Length == 0)
                return new BitSet(0);

            BitSet unionSet = new BitSet(sets[0].set.Length);
            foreach (BitSet s in sets)
            {
                int looplen = MathUtils.Min(s.set.Length, unionSet.set.Length);
                for (int i = 0; i < looplen; i++)
                    unionSet.set[i] |= s.set[i];
            }

            return unionSet;
        }

        public static BitSet Intersection(params BitSet[] sets)
        {
            if (sets.Length == 0)
                return new BitSet(0);

            BitSet interSet = new BitSet(sets[0]);
            for(int setIndex = 1;  setIndex < sets.Length; setIndex++)
            {
                int looplen = MathUtils.Min(sets[setIndex].set.Length, interSet.set.Length);
                int i;
                for (i = 0; i < looplen; i++)
                    interSet.set[i] &= sets[setIndex].set[i];
                for (; i < interSet.set.Length; i++)
                    interSet.set[i] = 0;
            }

            return interSet;
        }

        public static BitSet Difference(params BitSet[] sets)
        {
            if (sets.Length == 0)
                return new BitSet(0);

            BitSet diffSet = new BitSet(sets[0].set.Length);
            foreach (BitSet s in sets)
            {
                int looplen = MathUtils.Min(s.set.Length, diffSet.set.Length);
                int i;
                for (i = 0; i < looplen; i++)
                    diffSet.set[i] ^= s.set[i];
                if (i < s.set.Length)
                {
                    int[] newSet = new int[s.set.Length];
                    diffSet.set.CopyTo(newSet, 0);
                    diffSet.set = newSet;
                    for (; i < s.set.Length; i++)
                        diffSet.set[i] = s.set[i];
                }
            }

            return diffSet;
        }
        


        public static BitSet Subtract(BitSet subtracted, BitSet subtracter)
        {
            BitSet subtSet = new BitSet(subtracted);
            int looplen = MathUtils.Min(subtracted.set.Length, subtracter.set.Length);
            for (int i = 0; i < looplen; i++)
                subtSet.set[i] &= ~(subtracter.set[i]);

            return subtSet;
        }

        public override string ToString()
        {

            char[] bits = new char[set.Length << (OBJECT_SIZE_BYTE + 1)];
            for (int i = 0; i < bits.Length; i++)
                bits[i] = (char)(((set[i >> DIV_BY_OBJECT_SIZE] >> i) & 1) + '0');

            return new string(bits);
        }


    }
}

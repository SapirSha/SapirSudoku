using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Numerics;
using SapirSudoku.src.Exceptions;
using SapirSudoku.src.Utilities;

namespace SapirSudoku.src.DataStructures.BitSet
{
    public class BitSet : ISet<int>
    {
        // Object size is considered as the amount of values that you can put (in other words bit size)
        private static readonly int OBJECT_SIZE_BYTE = sizeof(int);
        private static readonly int OBJECT_SIZE_BIT = OBJECT_SIZE_BYTE << 3;

        private static readonly int DIV_BY_OBJECT_SIZE = int.Log2(OBJECT_SIZE_BIT); // used in bitwise '>>'


        private int[] set;
        public int[] Set { get { return set; } set { this.set = value; } }


        public int Smallest
        {
            get
            {
                int count = 0;
                for (int i = 0; i < set.Length; i++)
                {
                    if (set[i] != 0)
                        return count + BitOperations.TrailingZeroCount(set[i]) + 1;
                    count += OBJECT_SIZE_BIT;
                }
                return -1;
            }
        }

        public int Largest
        {
            get
            {
                int count = OBJECT_SIZE_BIT * set.Length;
                for (int i = set.Length - 1; i >= 0; i--)
                {
                    if (set[i] != 0)
                        return count - BitOperations.LeadingZeroCount((uint)set[i]);
                    count -= OBJECT_SIZE_BIT;
                }
                return -1;
            }
        }

        public int Count
        {
            get
            {
                int count = 0;
                foreach (var holder in set)
                    count += BitOperations.PopCount((uint)holder);
                return count;
            }
        }

        public BitSet(int size)
        {
            if (size < 0) throw new ArgumentOutOfRangeException("Size cannot be zero!");
            set = new int[size + (OBJECT_SIZE_BIT - 1) >> DIV_BY_OBJECT_SIZE];
        }

        public BitSet(BitSet set)
        {
            this.set = (int[])set.set.Clone();
        }

        // O(n)
        public BitSet(IEnumerable<int> other) : this(other.ToArray().Max())
        {
            foreach (int value in other)
                Add(value);
        }

        public bool IsReadOnly => false;

        private void Expand(int capacity)
        {
            if ((capacity - 1) >> DIV_BY_OBJECT_SIZE >= set.Length)
            {
                int[] newSet = new int[capacity + (OBJECT_SIZE_BIT - 1) >> DIV_BY_OBJECT_SIZE];
                set.CopyTo(newSet, 0);
                set = newSet;
            }
        }
        public bool Add(int value)
        {
            if (value <= 0)
                return false;

            if (value - 1 >> DIV_BY_OBJECT_SIZE >= set.Length)
                Expand(value);

            set[(value - 1) >> DIV_BY_OBJECT_SIZE] |= 1 << ((value - 1) % OBJECT_SIZE_BIT);

            return true;
        }

        public void Clear()
        {
            Array.Clear(set, 0, set.Length);
        }

        public bool Contains(int value)
        {
            if (value <= 0)
                return false;

            if (value - 1 >> DIV_BY_OBJECT_SIZE >= set.Length)
                return false;

            return (set[value - 1 >> DIV_BY_OBJECT_SIZE] & 1 << value - 1) != 0;
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            int index = arrayIndex;
            int[] newArray = new int[arrayIndex + Count];
            foreach (int value in this)
                newArray[index++] = value;
            newArray.CopyTo(array, arrayIndex);
        }

        public void ExceptWith(IEnumerable<int> other)
        {
            foreach (int value in other)
                Remove(value);
        }

        public void ExceptWith(BitSet other)
        {
            int loopLen = MathUtilities.Min(other.set.Length, set.Length);
            for (int i = 0; i < loopLen; i++)
                this.set[i] &= ~other.set[i];
        }

        public IEnumerator<int> GetEnumerator()
        {
            int[] clone = (int[])set.Clone();
            int count = 0;
            for (int i = 0; i < clone.Length; i++)
            {
                while (clone[i] != 0)
                {
                    int smallest = count + BitOperations.TrailingZeroCount(clone[i]) + 1;
                    yield return smallest;
                    clone[i] ^= 1 << smallest >> 1;
                }
                count += OBJECT_SIZE_BIT;
            }
            yield break;
        }

        public void IntersectWith(IEnumerable<int> other)
        {
            BitSet accrued = new BitSet(other);
            for (int i = 1; i <= set.Length * OBJECT_SIZE_BIT; i++)
                if (set.Contains(i) && !accrued.Contains(i)) Remove(i);
        }

        public void IntersectWith(BitSet other)
        {
            int i, loopLen = MathUtilities.Min(other.set.Length, set.Length);
            for (i = 0; i <= loopLen; i++)
                set[i] &= other.set[i];

            for (; i < set.Length; i++)
                set[i] = 0;
        }

        public bool IsProperSubsetOf(IEnumerable<int> other)
        {
            if (!IsSubsetOf(other)) return false;

            foreach (int value in other)
                if (!this.Contains(value))
                    return true;

            return false;
        }

        public bool IsProperSupersetOf(IEnumerable<int> other)
        {
            if (!IsSupersetOf(other)) return false;
                foreach (int value in this)
                    if (!other.Contains(value))
                        return true;

            return false;
        }

        public bool IsSubsetOf(IEnumerable<int> other)
        {
            foreach (int i in this)
                if (!other.Contains(i))
                    return false;
            return true;
        }

        public bool IsSubsetOf(BitSet superset)
        {
            if (superset.set.Length < set.Length) return false;

            int loopLen = set.Length;
            for (int i = 0; i < loopLen; i++)
                if ((superset.set[i] | set[i]) != superset.set[i])
                    return false;

            return true;
        }

        public bool IsSupersetOf(IEnumerable<int> other)
        {
            foreach (int i in other)
                if (!Contains(i))
                    return false;
            return true;
        }

        public bool IsSupersetOf(BitSet subset)
        {
            if (subset.set.Length > set.Length) return false;

            int loopLen = subset.set.Length;
            for (int i = 0; i < loopLen; i++)
                if ((set[i] | subset.set[i]) != set[i])
                    return false;

            return true;

        }

        public bool Overlaps(IEnumerable<int> other)
        {
            foreach (int i in other)
                if (Contains(i))
                    return true;
            return false;
        }

        public bool Overlaps(BitSet other)
        {
            int loopLen = Math.Min(other.set.Length, set.Length);
            for (int i = 0; i < loopLen; i++)
                if ((set[i] & other.set[i]) != 0)
                    return true;

            return false;
        }

        public bool Remove(int value)
        {
            if (value <= 0)
                return false;

            if ((value - 1) >> DIV_BY_OBJECT_SIZE < set.Length)
            {
                set[(value - 1) >> DIV_BY_OBJECT_SIZE] &= ~(1 << ((value - 1) % OBJECT_SIZE_BIT));
                return true;
            }

            return false;
        }

        public bool Toggle(int value)
        {
            if (value <= 0)
                return false;

            if (value - 1 >> DIV_BY_OBJECT_SIZE >= set.Length)
                Expand(value); // Has to be On

            set[value - 1 >> DIV_BY_OBJECT_SIZE] ^= 1 << value - 1;
            return true;
        }

        public bool SetEquals(IEnumerable<int> other)
        {
            foreach (int value in other)
                if (!Contains(value))
                    return false;
            foreach (int value in this)
                if (!other.Contains(value))
                    return false;

            return true;
        }

        public bool SetEquals(BitSet other)
        {
            if (other.set.Length != set.Length)
                return false;

            int loopLen = set.Length;
            for (int i = 0; i < loopLen; i++)
                if (set[i] != other.set[i])
                    return false;

            return true;
        }

        public void SymmetricExceptWith(IEnumerable<int> other)
        {
            BitSet otherBitSet = new BitSet(other);
            SymmetricExceptWith(otherBitSet);
        }

        public void SymmetricExceptWith(BitSet other)
        {
            int looplen = MathUtilities.Max(other.Count, Count);
            for (int i = 0; i < looplen; i++)
                set[i] ^= other.Set[i];
        }

        public void UnionWith(IEnumerable<int> other)
        {
            foreach (int value in other)
                Add(value);
        }

        public void UnionWith(BitSet other)
        {
            if (set.Length < other.set.Length) Expand(other.set.Length << OBJECT_SIZE_BIT);

            int loopLen = MathUtilities.Min(set.Length, other.set.Length);
            for (int i = 0; i < loopLen; i++)
                set[i] |= other.set[i];
        }

        void ICollection<int>.Add(int item)
        {
            Add(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override bool Equals(object? obj)
        {
            if (obj == this) return true;
            if (obj == null) return false;
            if (typeof(BitSet) != obj.GetType()) return false;
            BitSet other = (BitSet)obj;
            int looplen = MathUtilities.Min(other.set.Length, set.Length);

            for (int i = 0; i < looplen; i++)
                if (other.set[i] != set[i]) return false;

            for (int i = looplen; i < other.set.Length; i++)
                if (other.set[i] != 0) return false;
            for (int i = looplen; i < set.Length; i++)
                if (set[i] != 0) return false;

            return true;
        }
        public bool IsEmpty()
        {
            foreach (var holder in set)
                if (holder != 0)
                    return false;
            return true;
        }


        IEnumerator<int> IEnumerable<int>.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override int GetHashCode()
        {
            int hash = 31;
            hash *= Smallest + Count;
            hash += Largest * Count;
            return hash;
        }

        public override string ToString()
        {
            String msg = $"{base.ToString()}: ";
            foreach (int value in this)
                msg += $"{value} ";
            return msg;
        }
    }
}

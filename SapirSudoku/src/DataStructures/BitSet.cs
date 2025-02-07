using System.Collections;
using System.Numerics;
using System.Text;
using SapirSudoku.src.Exceptions;

namespace SapirSudoku.src.DataStructures
{
    /// <summary>
    /// A Set of positive natural numbers.
    /// the set is a bitwise based set where each bit is representing a value.
    /// </summary>
    public class BitSet : ISet<int>
    {
        /// <summary>
        /// Whether the Set is read only or not
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// The size of the holder value type in bytes
        /// </summary>
        private static readonly int OBJECT_SIZE_BYTE = sizeof(int);
        /// <summary>
        /// The size of the holder value type in bits
        /// </summary>
        private static readonly int OBJECT_SIZE_BIT = OBJECT_SIZE_BYTE << 3;

        /// <summary>
        /// Used to be able to divide by object size using bitwise
        /// </summary>
        private static readonly int DIV_BY_OBJECT_SIZE = int.Log2(OBJECT_SIZE_BIT);

        /// <summary>
        /// The array of value types that would hold the values.
        /// </summary>
        private int[] set;


        /// <summary>
        /// The smallest value in the set.
        /// </summary>
        public int Smallest
        {
            get
            {
                // Start from one
                int current = 1;
                // go through the items in the array
                for (int i = 0; i < set.Length; i++)
                {
                    //if current array value is not none, there is a value
                    if (set[i] != 0)
                        // return the current position + the amount of zeros before the reaching the index of the bit
                        return current + BitOperations.TrailingZeroCount(set[i]);
                    // if not found in current 
                    current += OBJECT_SIZE_BIT;
                }

                throw new SetIsEmptyException("Cannot find smallest value of an empty cell");
            }
        }

        /// <summary>
        /// The largest value in the set.
        /// </summary>
        public int Largest
        {
            get
            {
                //start from the highest possible value
                int current = OBJECT_SIZE_BIT * set.Length;
                // go through every item in the array (backwards)
                for (int i = set.Length - 1; i >= 0; i--)
                {
                    //if current array value is not none, there is a value
                    if (set[i] != 0)
                        // return the current position from the end - the amount of zeros after the right most bit in the value type
                        return current - BitOperations.LeadingZeroCount((uint)set[i]);
                    // if nothing in the current set, remove the values possible of the current item in the array
                    current -= OBJECT_SIZE_BIT;
                }
                throw new SetIsEmptyException("Cannot find largest value of an empty cell");
            }
        }

        /// <summary>
        /// The amount of values in the set.
        /// </summary>
        public int Count
        {
            get
            {
                int count = 0;
                // go through all the items in the array
                foreach (var holder in set)
                    // add to count the number of bits it in
                    count += BitOperations.PopCount((uint)holder);

                return count;
            }
        }

        /// <summary>
        /// A constructor for the BitSet class.
        /// initialize an empty set where the initial largest number it can hold is 'size'
        /// </summary>
        /// <param name="size">The initial maximum value the set can hold</param>
        /// <exception cref="InvalidBitSetSizeException"> Thrown when value non positive</exception>
        public BitSet(int size)
        {
            if (size < 0) throw new InvalidBitSetSizeException("Size cannot be less then non positive!");
            // create a new item in the array so it would have at least amount of bits to hold value (at least 'size' bits needed)
            set = new int[size + (OBJECT_SIZE_BIT - 1) >> DIV_BY_OBJECT_SIZE];
        }

        /// <summary>
        /// Create a new BitSet instance with the values of another BitSet
        /// </summary>
        /// <param name="set"> The set to be copied </param>
        public BitSet(BitSet set)
        {
            this.set = (int[])set.set.Clone();
        }

        /// <summary>
        /// Create a new BitSet instance with the values of another IEnumerable
        /// </summary>
        /// <param name="other"> The IEnumerable holding the values too be copied </param>
        public BitSet(IEnumerable<int> other) : this(other.ToArray().Max())
        {
            foreach (int value in other)
                Add(value);
        }

        /// <summary>
        /// Expand the array so the set would be able to hold bigger values
        /// </summary>
        /// <param name="capacity"></param>
        private void Expand(int capacity)
        {
            // if capacity is bigger then the current capacity the array can hold
            if (capacity - 1 >> DIV_BY_OBJECT_SIZE >= set.Length)
            {
                // create a new array with enough bits to hold 'capacity' as its max value (at least 'capacity' bits needed)
                int[] newSet = new int[capacity + (OBJECT_SIZE_BIT - 1) >> DIV_BY_OBJECT_SIZE];
                set.CopyTo(newSet, 0);
                set = newSet;
            }
        }

        /// <summary>
        /// Add a value into the set.
        /// </summary>
        /// <param name="value"> The value to add</param>
        /// <returns> True if value added, false if already in set</returns>
        /// <exception cref="InvalidBitSetValueException">
        /// Thrown if tried to insert a non positive value
        /// </exception>
        public bool Add(int value)
        {
            if (value <= 0)
                throw new InvalidBitSetValueException("BitSet cannot hold non positive numbers");

            // if tried to insert a value bigger then the current array allowed, expand
            if (value - 1 >> DIV_BY_OBJECT_SIZE >= set.Length)
                Expand(value);

            if (Contains(value))
                return false;

            // insert to the 'value' bit of the array
            set[value - 1 >> DIV_BY_OBJECT_SIZE] |= 1 << (value - 1) % OBJECT_SIZE_BIT;

            return true;
        }
        /// <summary>
        /// Clear the set.
        /// </summary>
        public void Clear()
        {
            Array.Clear(set, 0, set.Length);
        }

        /// <summary>
        /// Check if a value exists in the set
        /// </summary>
        /// <param name="value"> The value to check if its in the set </param>
        /// <returns> True if value exists in the set, False otherwise </returns>
        public bool Contains(int value)
        {
            if (value <= 0)
                return false;

            // if value is more then the bits the array can hold, return false
            if (value - 1 >> DIV_BY_OBJECT_SIZE >= set.Length)
                return false;

            // return true if the 'value' bit is not 0, else false
            return (set[value - 1 >> DIV_BY_OBJECT_SIZE] & 1 << value - 1) != 0;
        }

        /// <summary>
        /// Copy the values in the set to the array
        /// </summary>
        /// <param name="array"> The array to copy the values to </param>
        /// <param name="arrayIndex"> The index to start the copying from </param>
        public void CopyTo(int[] array, int arrayIndex)
        {
            if (array.Length < arrayIndex)
            {
                throw new ArgumentException("Array index out of range");
            }

            int index = arrayIndex;
            
            if (array.Length < arrayIndex + Count)
            {
                // if array doesnt contain enough space for all values, invalid
                throw new ArgumentException("Array doesnt contain enoug space");
            }
            // if array contains enough space for all values
            else
            {
                // add all the values from the set to the array, starting from 'arrayIndex'
                foreach (int value in this)
                {
                    array[index++] = value;
                }
            }
        }

        /// <summary>
        /// Remove a value from the set
        /// </summary>
        /// <param name="value"> The value to remove </param>
        /// <returns> True if removed successfully, False otherwise </returns>
        public bool Remove(int value)
        {
            if (value <= 0)
                return false;

            // if value inside the range of bits that the array can hold
            if (value - 1 >> DIV_BY_OBJECT_SIZE < set.Length)
            {
                // Set the bit in position 'value' in the array to false
                set[value - 1 >> DIV_BY_OBJECT_SIZE] &= ~(1 << (value - 1) % OBJECT_SIZE_BIT);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Toggle a value from the set.
        /// Remove if already in, Add if not in.
        /// </summary>
        /// <param name="value"></param>
        /// <exception cref="InvalidBitSetValueException"></exception>
        public void Toggle(int value)
        {
            if (value <= 0)
                throw new InvalidBitSetValueException("BitSet cannot hold non positive numbers");

            // if outside the bounds of the array expand the array, since the value has to be turned on
            if (value - 1 >> DIV_BY_OBJECT_SIZE >= set.Length)
                Expand(value);

            // Change the bit in position 'value' in the array to whatever it isnt
            set[value - 1 >> DIV_BY_OBJECT_SIZE] ^= 1 << value - 1;
        }


        public void ExceptWith(IEnumerable<int> other)
        {
            foreach (int value in other)
                Remove(value);
        }

        /// <summary>
        /// Remove all elements in the BitSet that are currently in current BitSet.
        /// </summary>
        /// <param name="other"> The BitSet containning the items to remove </param>
        public void ExceptWith(BitSet other)
        {
            int loopLen = Math.Min(other.set.Length, set.Length);
            for (int i = 0; i < loopLen; i++)
                set[i] &= ~other.set[i];
        }

        public IEnumerator<int> GetEnumerator()
        {
            int[] clone = (int[])set.Clone();
            // counter to save the distance from the start of the array
            int count = 0;
            // go thorugh all the value types in the array
            for (int i = 0; i < clone.Length; i++)
            {
                // while value type has bits
                while (clone[i] != 0)
                {
                    // return the position of the bit form the start of the array
                    int smallest = count + BitOperations.TrailingZeroCount(clone[i]) + 1;
                    yield return smallest;
                    // remove the returned number
                    clone[i] ^= 1 << smallest >> 1;
                }
                // add the passed values to count
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

        /// <summary>
        /// Save only elements that are in both Bitsets
        /// </summary>
        /// <param name="other"> The element that determines which values to save in current BitSet </param>
        public void IntersectWith(BitSet other)
        {
            int i, loopLen = Math.Min(other.set.Length, set.Length);
            // go thorugh sets, and save only bits that are in both value types, until one set is empty
            for (i = 0; i <= loopLen; i++)
                set[i] &= other.set[i];

            // clear the rest of current array if any values are left
            for (; i < set.Length; i++)
                set[i] = 0;
        }

        public bool IsProperSubsetOf(IEnumerable<int> other)
        {
            if (!IsSubsetOf(other)) return false;

            foreach (int value in other)
                if (!Contains(value))
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
        /// <summary>
        /// Determines whether current BitSet is a SubSet of another BitSet
        /// </summary>
        /// <param name="superset"> The potentiall superset of current set</param>
        /// <returns>True if current set is a subset of 'superset', False otherwise </returns>
        public bool IsSubsetOf(BitSet superset)
        {
            if (superset.set.Length < set.Length) return false;

            int loopLen = set.Length;
            // go through every item in the arrays
            for (int i = 0; i < loopLen; i++)
                // if for any item there is one that has bits which are not in 'superset' return false
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

        /// <summary>
        /// Determines whether current BitSet is a Superset of another BitSet
        /// </summary>
        /// <param name="subset"> The potential subset</param>
        /// <returns>True if current set is a superset of 'subset', False otherwise </returns>
        public bool IsSupersetOf(BitSet subset)
        {
            if (subset.set.Length > set.Length) return false;

            int loopLen = subset.set.Length;
            // go through the items in each of the set's array
            for (int i = 0; i < loopLen; i++)
                // if at any item there are bits who are turned on in subset but not in current set, return false
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

        /// <summary>
        /// Determines wether current Bitset and another Bitset contain any same value.
        /// </summary>
        /// <param name="other"> The other Bitset that might contain the same value </param>
        /// <returns> True if both sets contain a certain same value, Flase otherwise </returns>
        public bool Overlaps(BitSet other)
        {
            int loopLen = Math.Min(other.set.Length, set.Length);
            // go through all the items in the array
            for (int i = 0; i < loopLen; i++)
                // if any bits are turned on in both items, return true
                if ((set[i] & other.set[i]) != 0)
                    return true;

            return false;
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
        /// <summary>
        /// Determines whether current Bitset has the same values as another Bitset
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool SetEquals(BitSet other)
        {
            if (other.set.Length != set.Length)
                return false;

            int loopLen = set.Length;
            // go through all items in the sets array
            for (int i = 0; i < loopLen; i++)
                // if at any point an item is diffrent, return false
                if (set[i] != other.set[i])
                    return false;

            return true;
        }

        public void SymmetricExceptWith(IEnumerable<int> other)
        {
            BitSet otherBitSet = new BitSet(other);
            SymmetricExceptWith(otherBitSet);
        }

        /// <summary>
        /// Makes the current set hold values that are in one of the set but not the other.
        /// </summary>
        /// <param name="other"> The Bitset to get the other values from </param>
        public void SymmetricExceptWith(BitSet other)
        {
            int looplen = Math.Max(other.Count, Count);
            for (int i = 0; i < looplen; i++)
                set[i] ^= other.set[i];
        }

        public void UnionWith(IEnumerable<int> other)
        {
            foreach (int value in other)
                Add(value);
        }
        /// <summary>
        /// Makes the current set hold values that are in either of ther sets.
        /// </summary>
        /// <param name="other"> The Bitset to get the other values from </param>
        public void UnionWith(BitSet other)
        {
            if (set.Length < other.set.Length) Expand(other.set.Length << OBJECT_SIZE_BIT);

            int loopLen = Math.Min(set.Length, other.set.Length);
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
            int looplen = Math.Min(other.set.Length, set.Length);

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
            StringBuilder msg = new StringBuilder($"{base.ToString()}: ");
            foreach (int value in this)
                msg.Append(value);
            return msg.ToString();
        }
    }
}

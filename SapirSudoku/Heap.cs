using System;
using CustomExceptions;

namespace SapirStruct
{
    public class Heap<T>
    {
        private const float DEFAULT_EXPAND_MULTIPLIER = 1.5f;

        public T[] values;
        private int count = -1;
        private Func<T, T, bool> compare;

        public Heap(int capacity, Func<T, T, bool> compare)
        {
            values = new T[capacity];
            this.compare = compare;
        }

        public Heap(T[] arr, Func<T, T, bool> compare) : this(arr.Length, compare)
        {
            foreach (T value in arr)
                Push(value);
        }

        public void Expand(float expand_multiplier = DEFAULT_EXPAND_MULTIPLIER)
        {
            T[] newValues = new T[(int)(values.Length * expand_multiplier)];
            Array.Copy(values, newValues, values.Length);
            values = newValues;
        }

        private static void Switch(ref T value1, ref T value2)
        {
            T temp = value1;
            value1 = value2;
            value2 = temp;
        }

        private void Heapify(int index)
        {
            int leftChild = 2 * index + 1;
            int rightChild = 2 * index + 2;
            int largest = index;

            while (leftChild <= count) 
            {
                if (compare(values[leftChild], values[largest]))
                    largest = leftChild;

                if (rightChild <= count && compare(values[rightChild], values[largest]))
                    largest = rightChild;

                if (largest != index)
                {
                    Switch(ref values[index], ref values[largest]);
                    index = largest;

                    leftChild = 2 * index + 1;
                    rightChild = 2 * index + 2;
                }
                else break;
            }
        }

        private void HeapifyUp(int index)
        {
            while (index > 0 && compare(values[index], values[(index - 1) / 2]))
            {
                Switch(ref values[index], ref values[(index - 1) / 2]);
                index = (index - 1) / 2;
            }
        }


        public bool IsEmpty()
        {
            return count == -1;
        }

        public void Push(T value)
        {
            if (count + 1 >= values.Length)
                Expand();

            values[++count] = value;

            HeapifyUp(count);
        }

        public T Pop()
        {
            if (count == -1)
                throw new HeapException("Heap is empty");

            T rootValue = values[0];
            Switch(ref values[0], ref values[count]);

            count--;

            Heapify(0);

            return rootValue;
        }

        public T peek()
        {
            if (count == -1)
                throw new HeapException("Heap is empty");
            return this.values[0];
        }

        public void Edit(int index, T value)
        {
            if (index > count || index < 0)
                throw new ArgumentOutOfRangeException("Heap Index out of range.");
            values[index] = value;
            Heapify(index);
            HeapifyUp(index);
        }
    }
}

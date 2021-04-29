using System;
using System.IO;
using System.Text;

public class PermutationSpace
{
    private readonly static byte[] SPACE = Encoding.ASCII.GetBytes(" ");
    private readonly static byte[] NEWLINE = Encoding.ASCII.GetBytes(Environment.NewLine);

    private readonly byte[] array;
    private readonly int wordLength;
    private int insertionPoint;

    public PermutationSpace(int wordLength)
    {
        if (wordLength < 1 || wordLength > 11)
            throw new ArgumentException(nameof(wordLength));

        this.wordLength = wordLength;

        // Array size is permutations * word length
        int length = 1;
        for (int i = wordLength; i > 0; i--)
        {
            length *= i;
        }
        length *= wordLength;

        array = new byte[length];
        insertionPoint = 0;
    }

    public void Rewind()
    {
        insertionPoint = 0;
    }

    public void AddWord(byte[] word) {
        if (word.Length != wordLength)
            throw new ArgumentException(nameof(word));
        if (insertionPoint + wordLength > array.Length)
            throw new Exception("Base array is full");
            
        Array.Copy(word, 0, array, insertionPoint, wordLength);
        insertionPoint += wordLength;
    }

    public void PrintPermutations(bool printDuplicates)
    {
        Span<byte> arrayView = array.AsSpan(0, insertionPoint);
        //System.Diagnostics.Debug.Assert(arrayView.Length % wordLength == 0, "Not a multiple of wordlength");
        QuickSort();

        const int consoleWidth = 80;
        int wordsPerLine = consoleWidth / (wordLength + 1);
        wordsPerLine = Math.Max(wordsPerLine, 3);
        int bytesPerLine = wordsPerLine * (wordLength+1);

        Span<byte> lastWordPrinted = stackalloc byte[wordLength];
        lastWordPrinted.Clear();

        using (BufferedStream outputStream = new BufferedStream(Console.OpenStandardOutput(), 4096 * 4))
        {
            int wordBytesPrinted = 0;
            do {
                Span<byte> currentWord = arrayView.Slice(0, wordLength);
                if (printDuplicates || currentWord.SequenceCompareTo(lastWordPrinted) != 0) {
                    outputStream.Write(currentWord);
                    outputStream.Write(SPACE);
                    wordBytesPrinted += wordLength + 1;

                    if (!printDuplicates) {
                        currentWord.CopyTo(lastWordPrinted);
                    }
                }

                if (wordBytesPrinted >= bytesPerLine) {
                    outputStream.Write(NEWLINE);
                    wordBytesPrinted = 0;
                }

                arrayView = arrayView.Slice(wordLength);
            } while (arrayView.Length > 0);

            outputStream.Write(NEWLINE);
        }
    }

    private int Count(int startIndex = 0, int endIndex = -1) {
        if (endIndex == -1)
            endIndex = insertionPoint;
        return (insertionPoint - startIndex) / wordLength;
    }

    private void SelectionSort() {
        DoSelectionSort(array.AsSpan(0, insertionPoint));
    }

    private void DoSelectionSort(Span<byte> arrayView) {
        int count = arrayView.Length / wordLength;
        int minIndex;
        for (int i = 0; i < count - 1; i++) {
            minIndex = i;
            for (int j = i + 1; j < count; j++) {
                Span<byte> elementA = arrayView.Slice(j * wordLength, wordLength);
                Span<byte> elementB = arrayView.Slice(minIndex * wordLength, wordLength);
                if (elementA.SequenceCompareTo(elementB) < 0)
                    minIndex = j;
            }

            if (minIndex != i)
                SwapWord(arrayView.Slice(minIndex * wordLength, wordLength), arrayView.Slice(i * wordLength, wordLength));
        }
    }

    private void QuickSort() {
        DoQuickSort(array.AsSpan(0, insertionPoint), wordLength);
    }

    public static void DoQuickSort(Span<byte> arrayView, int wordLength) {
        //System.Diagnostics.Debug.Assert(arrayView.Length % wordLength == 0, "quicksort arrayview is not a multiple of wordlength");
        if (arrayView.Length <= wordLength) {
            return;
        }

        int elementCount = arrayView.Length / wordLength;
        int middleIndex = (elementCount - 1) / 2;

        Span<byte> pivotValue = stackalloc byte[wordLength];
        arrayView.Slice(middleIndex * wordLength, wordLength).CopyTo(pivotValue);

        int i = -1;
        int j = elementCount;
        int partitionIndex;
        for(;;) {
            Span<byte> elementI;
            Span<byte> elementJ;
            do {
                i++;
                elementI = arrayView.Slice(i * wordLength, wordLength);
            } while (elementI.SequenceCompareTo(pivotValue) < 0);

            do {
                j--;
                elementJ = arrayView.Slice(j * wordLength, wordLength);
            } while (elementJ.SequenceCompareTo(pivotValue) > 0);

            if (i >= j) {
                partitionIndex = j;
                break;
            }

            SwapWord(elementI, elementJ);
        }
        DoQuickSort(arrayView.Slice(0, partitionIndex * wordLength + wordLength), wordLength);
        DoQuickSort(arrayView.Slice(partitionIndex * wordLength + wordLength), wordLength);
    }

    public static void SwapWord(byte[] array, int swapIndexA, int swapIndexB, int length) {
        if (swapIndexA == swapIndexB)
            return;

        if (length == 1) {
            byte temp = array[swapIndexA];
            array[swapIndexA] = array[swapIndexB];
            array[swapIndexB] = temp;
        } else {
            Span<byte> a = array.AsSpan(swapIndexA, length);
            Span<byte> b = array.AsSpan(swapIndexB, length);
            SwapWord(a, b);
        }
    }

    private static void SwapWord(Span<byte> a, Span<byte> b) {
        Span<byte> temp = stackalloc byte[a.Length];
        a.CopyTo(temp);
        b.CopyTo(a);
        temp.CopyTo(b);
    }
}

using System;
using System.Data;

namespace puzzle
{
    internal class Program
    {
        static int[,] memory = new int[5, 5];

        // Tüm parçaları saklamak için 3D array
        static int[,,] allPieces = new int[200, 5, 5];
        static int pieceCount = 0;

        static void Main(string[] args)
        {
            Console.Write("How many pieces would you like to generate? ");
            int pieceNumber = Convert.ToInt16(Console.ReadLine());

            int counter = 1;
            int[,] mat = new int[5, 5];
            int piece = 1;

            while (counter <= pieceNumber)
            {
                GeneratePiece(mat, piece);

                Console.WriteLine("Piece is getting generated: ");
                Printing(mat);

                NormalizingPieces(mat);

                Console.WriteLine("Piece is getting shifted:");
                Printing(mat);

                // Duplicate kontrol
                if (IsDuplicate(mat))
                {
                    Console.WriteLine("DUPLICATE PIECE FOUND!");
                }
                else
                {
                    Console.WriteLine("UNIQUE PIECE.");
                    StorePiece(mat);
                }

                RotatePiece90(mat);
                RotatePiece180(mat);
                RotatePiece270(mat);
                ReversePiece(mat);

                counter++;
            }
        }

        static void GeneratePiece(int[,] matrice, int square)
        {
            for (int a = 0; a < 5; a++)
                for (int b = 0; b < 5; b++)
                    matrice[a, b] = 0;

            Console.WriteLine("Numbers of squares you want to generate in a piece: ");
            square = Convert.ToInt16(Console.ReadLine());

            Console.Write("Enter regularity: ");
            int regularity = Convert.ToInt16(Console.ReadLine());

            Random rnd = new Random();

            int i = rnd.Next(0, 5);
            int j = rnd.Next(0, 5);
            matrice[i, j] = 1;

            int counter = 1;
            while (counter < square)
            {
                int x = rnd.Next(0, 5);
                int y = rnd.Next(0, 5);

                if (matrice[x, y] == 0 && SameEdgeControl(matrice, x, y))
                {
                    matrice[x, y] = 1;
                    counter++;
                }
            }
        }

        static bool SameEdgeControl(int[,] matrice, int i, int j)
        {
            if (i > 0 && matrice[i - 1, j] == 1) return true;
            if (i < 4 && matrice[i + 1, j] == 1) return true;
            if (j > 0 && matrice[i, j - 1] == 1) return true;
            if (j < 4 && matrice[i, j + 1] == 1) return true;
            return false;
        }

        static void NormalizingPieces(int[,] matrice)
        {
            int minimumRowNumber = 5;
            int minimumColumnNumber = 5;

            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if (matrice[i, j] == 1 && i < minimumRowNumber) minimumRowNumber = i;

            for (int j = 0; j < 5; j++)
                for (int i = 0; i < 5; i++)
                    if (matrice[i, j] == 1 && j < minimumColumnNumber) minimumColumnNumber = j;

            int[,] temp = new int[5, 5];
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if (matrice[i, j] == 1)
                        temp[i - minimumRowNumber, j - minimumColumnNumber] = 1;

            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    matrice[i, j] = temp[i, j];
        }

        static void Printing(int[,] matrice)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                    Console.Write(matrice[i, j] == 1 ? 'X' : '.');
                Console.WriteLine();
            }
        }

        static void WritingIntoMatriceAgain(int[,] source, int[,] target)
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    target[i, j] = source[i, j];
        }

        static void RotatePiece90(int[,] matrice)
        {
            int[,] rotated = new int[5, 5];
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    rotated[j, 4 - i] = matrice[i, j];

            WritingIntoMatriceAgain(rotated, matrice);
            Console.WriteLine("Rotated 90°:");
            Printing(matrice);
            NormalizingPieces(matrice);
        }

        static void RotatePiece180(int[,] matrice)
        {
            int[,] rotated = new int[5, 5];
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    rotated[4 - i, 4 - j] = matrice[i, j];

            WritingIntoMatriceAgain(rotated, matrice);
            Console.WriteLine("Rotated 180°:");
            Printing(matrice);
            NormalizingPieces(matrice);
        }

        static void RotatePiece270(int[,] matrice)
        {
            int[,] rotated = new int[5, 5];
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    rotated[4 - j, i] = matrice[i, j];

            WritingIntoMatriceAgain(rotated, matrice);
            Console.WriteLine("Rotated 270°:");
            Printing(matrice);
            NormalizingPieces(matrice);
        }

        static void ReversePiece(int[,] matrice)
        {
            int[,] reversed = new int[5, 5];
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    reversed[i, 4 - j] = matrice[i, j];

            WritingIntoMatriceAgain(reversed, matrice);
            Console.WriteLine("Reversed:");
            Printing(matrice);
            NormalizingPieces(matrice);
        }

       
        static void StorePiece(int[,] matrice)
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    allPieces[pieceCount, i, j] = matrice[i, j];
            pieceCount++;
        }

       
        static bool AreEqual(int[,] a, int[,] b)
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    if (a[i, j] != b[i, j]) return false;
            return true;
        }

        static bool IsDuplicate(int[,] newPiece)
        {
            int[,] temp = new int[5, 5];
            for (int p = 0; p < pieceCount; p++)
            {
                CopyPiece(allPieces, p, temp);
                NormalizingPieces(temp);

                if (AreEqual(temp, newPiece)) return true;

                RotatePiece90(temp); NormalizingPieces(temp); if (AreEqual(temp, newPiece)) return true;
                RotatePiece180(temp); NormalizingPieces(temp); if (AreEqual(temp, newPiece)) return true;
                RotatePiece270(temp); NormalizingPieces(temp); if (AreEqual(temp, newPiece)) return true;
                ReversePiece(temp); NormalizingPieces(temp); if (AreEqual(temp, newPiece)) return true;
            }
            return false;
        }

       
        static void CopyPiece(int[,,] source, int index, int[,] target)
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    target[i, j] = source[index, i, j];
        }
    }
}




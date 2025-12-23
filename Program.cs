using System;

namespace PBL_Squares
{
    internal class Program
    {
        static Random random = new Random();
        static int[,,,] pieces = new int[12, 24, 5, 5];

        static void Main()
        {
            Console.Write("Please, enter the number of pieces (1–20): ");
            int numberOfPieces = Convert.ToInt16(Console.ReadLine());
            int[] square = new int[24];

            for (int i = 0; i < numberOfPieces; i++)
            {
                Console.Write("Please, enter the number of squares (2–12): ");
                int numberOfSquares = Convert.ToInt16(Console.ReadLine());
                square[i] = numberOfSquares;

                GeneratePiece(numberOfSquares, i);
                Console.WriteLine("Number of generated piece: " + (i + 1));
                PrintPiece(numberOfSquares, i);

            }

            Console.WriteLine("Generated Matrices:");

            PrintAllPieces(square, numberOfPieces);

            Console.ReadLine();
        }

        static void GeneratePiece(int numberOfSquares, int index)
        {
            int[,] piece = new int[5, 5];

            int x = random.Next(0, 5);
            int y = random.Next(0, 5);
            piece[x, y] = 1;

            int numberOfPlacedSquare = 1;

            while (numberOfPlacedSquare < numberOfSquares)
            {
                int newPlaced = AddNeighbor(piece, numberOfPlacedSquare);

                if (newPlaced == numberOfPlacedSquare)
                    continue;

                numberOfPlacedSquare = newPlaced;
            }

            Normalize(piece);
            SaveTo4D(piece, numberOfSquares, index);
        }

        static int AddNeighbor(int[,] piece, int numberOfPlacedSquare)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        int dir = random.Next(-1, 3);

                        int newI = i;
                        int newJ = j;

                        if (dir == -1) newI = i - 1;
                        if (dir == 1) newI = i + 1;
                        if (dir == 0) newJ = j - 1;
                        if (dir == 2) newJ = j + 1;

                        if (newI >= 0 && newI < 5 && newJ >= 0 && newJ < 5)
                        {
                            if (piece[newI, newJ] == 0)
                            {
                                piece[newI, newJ] = 1;
                                return numberOfPlacedSquare + 1;
                            }
                        }
                    }
                }
            }

            return numberOfPlacedSquare;
        }

        static void Normalize(int[,] piece)
        {
            int i, j;

            bool isTop = false;

            for (j = 0; j < 5; j++)
            {
                if (piece[0, j] == 1)
                {
                    isTop = true;
                }
                    
            }

            if (!isTop)
            {
                int minRow = 5;

                for (i = 0; i < 5; i++)
                {
                    for (j = 0; j < 5; j++)
                    {
                        if (piece[i, j] == 1 && i < minRow)
                        {
                            minRow = i;
                        }
                    }
                }

                int[,] tmp = new int[5, 5];

                for (i = 0; i < 5; i++)
                {
                    for (j = 0; j < 5; j++)
                    {
                        if (piece[i, j] == 1)
                        {
                            tmp[i - minRow, j] = 1;
                        }
                    }
                }

                for (i = 0; i < 5; i++)
                {
                    for (j = 0; j < 5; j++)
                    {
                        piece[i, j] = tmp[i, j];
                    }
                }
            }

            int minColumn = 5;

            for (j = 0; j < 5; j++)
            {
                for (i = 0; i < 5; i++)
                {
                    if (piece[i, j] == 1 && j < minColumn)
                    {
                        minColumn = j;
                    }
                }
            }

            if (minColumn > 0)
            {
                int[,] tmpLeft = new int[5, 5];

                for (i = 0; i < 5; i++)
                {
                    for (j = 0; j < 5; j++)
                    {
                        if (piece[i, j] == 1)
                        {
                            tmpLeft[i, j - minColumn] = 1;
                        }
                    }
                }

                for (i = 0; i < 5; i++)
                {
                    for (j = 0; j < 5; j++)
                    {
                        piece[i, j] = tmpLeft[i, j];
                    }
                }
            }
        }

        static void SaveTo4D(int[,] piece, int numberOfSquares, int index)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    pieces[numberOfSquares, index, i, j] = piece[i, j];
                }
            }
        }

        static void PrintPiece(int numberOfSquares, int index)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (pieces[numberOfSquares, index, i, j] == 1)
                    {
                        Console.Write("X");
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }
                Console.WriteLine();
            }
        }
        static void PrintAllPieces(int[] square, int numberOfPieces)
        {
            for (int idx = 0; idx < 24; idx++) 
            {
                char matrixName = (char)('A' + idx);
                Console.WriteLine("Matrix: " + matrixName);

                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (pieces[square[idx], idx, i, j] == 1)
                        {
                            Console.Write(matrixName);
                        }
                        else
                        {
                            Console.Write(".");
                        }
                    }
                    Console.WriteLine();
                }

                Console.WriteLine();
            }
        }
    }
}

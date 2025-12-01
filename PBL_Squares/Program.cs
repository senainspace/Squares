using System;

namespace PBL_Squares
{
    internal class Program
    {
        static Random random = new Random();
        static int[,,,] pieces = new int[13, 24, 5, 5];
        static char[,] puzzle;

        static void Main()
        {
            Console.Write("Enter number of pieces (1–20): ");
            int numberOfPieces = Convert.ToInt32(Console.ReadLine());

            int[] square = new int[numberOfPieces];
            int[] countBySquares = new int[13];

            int[] MaxPolyomino = { 0, 0, 1, 2, 5, 12, 35, 108, 369, 1285, 4655, 17073, 63600 };

            for (int i = 0; i < numberOfPieces; i++)
            {
                bool isValid = false;

                while (!isValid)
                {
                    Console.Write($"Enter number of squares for piece {i + 1} (2–12): ");
                    int squares = Convert.ToInt32(Console.ReadLine());

                    if (squares < 2 || squares > 12)
                    {
                        Console.WriteLine("ERROR: Value must be between 2 and 12.");
                        continue;
                    }

                    if (countBySquares[squares] >= MaxPolyomino[squares])
                    {
                        Console.WriteLine(
                            $"ERROR: Only {MaxPolyomino[squares]} unique pieces exist with {squares} squares.\n" +
                            $"You already entered {countBySquares[squares]}.\n" +
                            $"Please enter another value."
                        );
                        continue;
                    }

                    square[i] = squares;
                    countBySquares[squares]++;
                    isValid = true;
                }
            }

            for (int i = 0; i < numberOfPieces; i++)
            {
                bool isDuplicate;
                do
                {
                    GeneratePiece(square[i], i);
                    NormalizePieceIn4D(square[i], i);

                    isDuplicate = false;

                    for (int previous = 0; previous < i; previous++)
                    {
                        int[,] piece1 = LoadPiece(square[i], i);
                        int[,] piece2 = LoadPiece(square[previous], previous);

                        if (PiecesEqual(piece1, piece2))
                        {
                            isDuplicate = true;
                            break;
                        }
                    }

                } while (isDuplicate);

                Console.WriteLine("Generated Piece " + (i + 1));
                PrintPiece(square[i], i);
            }

            Console.WriteLine("Pieces of puzzle");
            PrintAllPieces(square, numberOfPieces);

            Console.WriteLine("Puzzle");
            GeneratePuzzle(square, numberOfPieces);

            Console.ReadLine();
        }


        static void GeneratePiece(int squares, int index)
        {
            int[,] piece = new int[5, 5];

            int row = random.Next(0, 5);
            int column = random.Next(0, 5);
            piece[row, column] = 1;

            int placedNo = 1;

            while (placedNo < squares)
            {
                int newPlaced = AddNeighbor(piece, placedNo);
                if (newPlaced != placedNo)
                {
                    placedNo = newPlaced;
                }
            }

            Normalize(piece);
            SaveTo4D(piece, squares, index);
        }

        static int AddNeighbor(int[,] piece, int counter)
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

                        if (dir == -1)
                        {
                            newI = i - 1;
                        }
                        if (dir == 1)
                        {
                            newI = i + 1;
                        }
                        if (dir == 0)
                        {
                            newJ = j - 1;
                        }
                        if (dir == 2)
                        {
                            newJ = j + 1;
                        }

                        if (newI >= 0 && newI < 5 && newJ >= 0 && newJ < 5)
                        {
                            if (piece[newI, newJ] == 0)
                            {
                                piece[newI, newJ] = 1;
                                return counter + 1;
                            }
                        }
                    }
                }
            }
            return counter;
        }

        static void Normalize(int[,] piece)
        {
            int minRow = 5, minColumn = 5;

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        if (i < minRow)
                        {
                            minRow = i;
                        }
                        if (j < minColumn)
                        {
                            minColumn = j;
                        }
                    }
                }
            }

            int[,] tmp = new int[5, 5];

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        tmp[i - minRow, j - minColumn] = 1;
                    }
                }
            }

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    piece[i, j] = tmp[i, j];
                }
            } 
        }

        static void SaveTo4D(int[,] piece, int squares, int index)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    pieces[squares, index, i, j] = piece[i, j];
                }
            }
        }

        static void NormalizePieceIn4D(int squares, int index)
        {
            int[,] piece = LoadPiece(squares, index);
            Normalize(piece);
            SaveTo4D(piece, squares, index);
        }

        static void PrintPiece(int squares, int index)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (pieces[squares, index, i, j] == 1)
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
        static int[,] LoadPiece(int squares, int index)
        {
            int[,] piece = new int[5, 5];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    piece[i, j] = pieces[squares, index, i, j];
                }
            }
            return piece;
        }

        static bool PiecesEqual(int[,] piece1, int[,] piece2)
        {
            int[,] rotate90 = new int[5, 5];
            int[,] rotate180 = new int[5, 5];
            int[,] rotate270 = new int[5, 5];
            int[,] leftRight = new int[5, 5];
            int[,] upDown = new int[5, 5];

            Rotate90(piece1, rotate90);
            Rotate180(piece1, rotate180);
            Rotate270(piece1, rotate270);
            ReverseLR(piece1, leftRight);
            ReverseUD(piece1, upDown);

            if (isSame(piece1, piece2))
            {
                return true;
            }
            if (isSame(rotate90, piece2))
            {
                return true;
            }
            if (isSame(rotate180, piece2))
            {
                return true;
            }
            if (isSame(rotate270, piece2))
            {
                return true;
            }
            if (isSame(leftRight, piece2))
            {
                return true;
            }
            if (isSame(upDown, piece2))
            {
                return true;
            }

            return false;
        }

        static bool isSame(int[,] piece1, int[,] piece2)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece1[i, j] != piece2[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        static void Rotate90(int[,] piece, int[,] row)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    row[j, 4 - i] = piece[i, j];
                } 
            }
        }

        static void Rotate180(int[,] piece, int[,] row)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    row[4 - i, 4 - j] = piece[i, j];
                }
            }
        }

        static void Rotate270(int[,] piece, int[,] row)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    row[4 - j, i] = piece[i, j];
                }
            }
        }

        static void ReverseLR(int[,] piece, int[,] row)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    row[i, 4 - j] = piece[i, j];
                }
            }
        }

        static void ReverseUD(int[,] piece, int[,] row)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    row[4 - i, j] = piece[i, j];
                }
            } 
        }

        static void PrintAllPieces(int[] square, int numberOfPieces)
        {
            for (int idx = 0; idx < 24; idx++)
            {
                char name = (char)('A' + idx);
                Console.WriteLine("Matrix: " + name);

                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (idx < numberOfPieces)
                        {
                            if (pieces[square[idx], idx, i, j] == 1)
                            {
                                Console.Write(name);
                            }
                            else
                            {
                                Console.Write(".");
                            }
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
        static void GeneratePuzzle(int[] square, int counter)
        {
            Console.Write("Enter puzzle size (10–25): ");
            int size = Convert.ToInt32(Console.ReadLine());

            puzzle = new char[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    puzzle[i, j] = '.';
                }
            }

            for (int idx = 0; idx < counter; idx++)
            {
                int[,] piece = LoadPiece(square[idx], idx);
                int[,] orientation = RandomOrientation(piece);
                Normalize(orientation);

                bool isPlaced = false;

                for (int t = 0; t < 8000 && !isPlaced; t++)
                {
                    int row = random.Next(0, size);
                    int col = random.Next(0, size);

                    if (!CanPlace(orientation, row, col, size))
                    {
                        continue;
                    }

                    if (idx == 0)
                    {
                        PlacePiece(orientation, row, col, (char)('A' + idx));
                        isPlaced = true;
                    }
                    else
                    {
                        if (TouchesExisting(orientation, row, col, size))
                        {
                            PlacePiece(orientation, row, col, (char)('A' + idx));
                            isPlaced = true;
                        }
                    }
                }

                if (!isPlaced)
                {
                    Console.WriteLine($"Could not place piece {(char)('A' + idx)}!");
                }
            }
            PrintPuzzle(size);
        }

        static int[,] RandomOrientation(int[,] piece)
        {
            int[,] t = new int[5, 5];

            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    t[i, j] = piece[i, j];

            int rotate = random.Next(0, 4);

            for (int r = 0; r < rotate; r++)
            {
                int[,] temp = new int[5, 5];
                Rotate90(t, temp);
                t = temp;
            }

            int reverse = random.Next(0, 3);

            if (reverse == 1)
            {
                int[,] temp = new int[5, 5];
                ReverseLR(t, temp);
                t = temp;
            }
            else if (reverse == 2)
            {
                int[,] temp = new int[5, 5];
                ReverseUD(t, temp);
                t = temp;
            }

            return t;
        }

        static bool CanPlace(int[,] piece, int row, int column, int size)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        if (row + i >= size || column + j >= size)
                        {
                            return false;
                        }

                        if (puzzle[row + i, column + j] != '.')
                        {
                            return false;
                        }
                    }
                }
            } 
            return true;
        }

        static void PlacePiece(int[,] piece, int row, int column, char letter)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        puzzle[row + i, column + j] = letter;
                    }
                }
            }
        }

        static void PrintPuzzle(int size)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Console.Write(puzzle[i, j]);
                }
                Console.WriteLine();
            }
        }

        static bool TouchesExisting(int[,] piece, int row, int column, int size)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        int r = row + i;
                        int c = column + j;

                        if (r > 0 && puzzle[r - 1, c] != '.')
                        {
                            return true;
                        }
                        if (r < size - 1 && puzzle[r + 1, c] != '.')
                        {
                            return true;
                        }
                        if (c > 0 && puzzle[r, c - 1] != '.')
                        {
                            return true;
                        }
                        if (c < size - 1 && puzzle[r, c + 1] != '.')
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        static void PrintPuzzleX(int size)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (puzzle[i, j] == '.')
                    {
                        Console.Write('.');
                    }
                    else
                    {
                        Console.Write('X');
                    }
                }
                Console.WriteLine();
            }
        }
    }
}

using System;

namespace PBL_Squares
{
    internal class Program
    {
        static ConsoleColor[] pieceColors = new ConsoleColor[]
        {
            ConsoleColor.Cyan, ConsoleColor.Magenta, ConsoleColor.Red, ConsoleColor.Green,
            ConsoleColor.Yellow, ConsoleColor.Blue, ConsoleColor.DarkCyan, ConsoleColor.DarkGreen,
            ConsoleColor.DarkMagenta, ConsoleColor.DarkYellow, ConsoleColor.White, ConsoleColor.DarkRed
        };
        static Random random = new Random();
        static int[,,,] pieces = new int[13, 24, 5, 5];
        static char[,] puzzle;

        // Bu değişkenler global kalmalı, ancak değerleri GeneratePuzzle içinde atanacak.
        static int PUZZLE_COLS;
        static int PUZZLE_ROWS;

        static double regularity;

        static char[,] playerBoard;
        static int selectedPieceIndex = -1;
        static int curRow = 0, curCol = 0;
        static int[,] currentFloatingPiece;
        static bool[] isPieceUsed;

        static void Main()
        {
            PrintWelcomeScreen();

            // MAIN METODU TERTEMİZ KALDI. 
            // Boyut sorma işlemini GeneratePuzzle içine taşıdık.

            Console.Write("Enter number of pieces (1–20): ");
            int numberOfPieces = Convert.ToInt32(Console.ReadLine());

            int[] square = new int[numberOfPieces];
            int[] countBySquares = new int[13];

            GetPieceSizesFromUser(numberOfPieces, square, countBySquares);

            GenerateAndValidatePieces(numberOfPieces, square);

            ShowAllPieceOrientations(numberOfPieces, square);

            // GeneratePuzzle artık hem boyutu soracak hem de puzzle'ı oluşturacak.
            GeneratePuzzle(square, numberOfPieces);

            PrintGameScreen(square, numberOfPieces);
            Console.ReadLine();
        }

        static void PrintWelcomeScreen()
        {
            Console.Clear();
            Console.Title = "SQUARES - PBL PROJECT ";
            string[] titleLines = new string[]
            {
                @" ███████╗  ██████╗  ██╗   ██╗  █████╗  ██████╗  ███████╗ ███████╗",
                @" ██╔════╝ ██╔═══██╗ ██║   ██║ ██╔══██╗ ██╔══██╗ ██╔════╝ ██╔════╝",
                @" ███████╗ ██║   ██║ ██║   ██║ ███████║ ██████╔╝ █████╗   ███████╗",
                @" ╚════██║ ██║▄▄ ██║ ██║   ██║ ██╔══██║ ██╔══██╗ ██╔══╝   ╚════██║",
                @" ███████║ ╚██████╔╝ ╚██████╔╝ ██║  ██║ ██║  ██║ ███████╗ ███████║",
                @" ╚══════╝  ╚══▀▀═╝   ╚═════╝  ╚═╝  ╚═╝ ╚═╝  ╚═╝ ╚══════╝ ╚══════╝"
            };

            int topPadding = (Console.WindowHeight / 2) - 6;
            if (topPadding < 0) topPadding = 0;

            for (int i = 0; i < topPadding; i++) Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Blue;

            foreach (string line in titleLines)
            {
                int leftPadding = (Console.WindowWidth - line.Length) / 2;
                if (leftPadding < 0) leftPadding = 0;

                Console.WriteLine(new string(' ', leftPadding) + line);
            }
            Console.WriteLine("\n");
            string prompt = "[ PRESS ENTER TO START THE GAME ]";
            int promptPadding = (Console.WindowWidth - prompt.Length) / 2;
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(new string(' ', Math.Max(0, promptPadding)) + prompt);
            Console.ResetColor();
            Console.ReadLine();
            Console.Clear();
        }

        static void GetPieceSizesFromUser(int numberOfPieces, int[] square, int[] countBySquares)
        {
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
        }

        static void GenerateAndValidatePieces(int numberOfPieces, int[] square)
        {
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

                Console.WriteLine($"Generated Piece {i + 1} (Size: {square[i]})");
                int[,] generatedPiece = LoadPiece(square[i], i);
                PrintMatrix(generatedPiece);
            }
        }

        static void ShowAllPieceOrientations(int numberOfPieces, int[] square)
        {
            Console.WriteLine("\n|| Piece Orientations Check ||");

            for (int idx = 0; idx < numberOfPieces; idx++)
            {
                Console.WriteLine($"\n--- Piece {idx + 1} ({square[idx]} Square) ---");
                int[,] originalPiece = LoadPiece(square[idx], idx);

                Console.WriteLine("Original Piece:");
                PrintMatrix(originalPiece);

                int[,] current = originalPiece;
                for (int r = 1; r < 4; r++)
                {
                    int[,] rotated = new int[5, 5];
                    Rotate90(current, rotated);
                    Normalize(rotated);
                    Console.WriteLine($"{r * 90}° Rotation");
                    PrintMatrix(rotated);
                    current = rotated;
                }

                int[,] leftRight = new int[5, 5];
                ReverseLR(originalPiece, leftRight);
                Normalize(leftRight);
                Console.WriteLine("Left-Right Reverse (LR):");
                PrintMatrix(leftRight);

                int[,] upDown = new int[5, 5];
                ReverseUD(originalPiece, upDown);
                Normalize(upDown);
                Console.WriteLine("Up-Down Reverse (UD):");
                PrintMatrix(upDown);
            }
            Console.WriteLine("\n" + new string('-', 50));
        }

        static void PrintMatrix(int[,] piece)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Console.Write(piece[i, j] == 1 ? 'X' : '.');
                }
                Console.WriteLine();
            }
        }

        static void PrintGameScreen(int[] square, int numberOfPieces)
        {
            if (puzzle == null)
            {
                Console.WriteLine("Puzzle is not generated yet.");
                return;
            }

            // Global değişkenlerden (PUZZLE_COLS) okuyacak
            int puzzleSizeRows = puzzle.GetLength(0);

            int piecesPerRow = 5;
            int pieceGroupHeight = 6;
            int totalPieceRows = (int)Math.Ceiling((double)numberOfPieces / piecesPerRow);
            int totalRightSideHeight = totalPieceRows * pieceGroupHeight;
            int maxRows = Math.Max(puzzleSizeRows, totalRightSideHeight);

            Console.WriteLine();
            Console.Write("  ");

            for (int j = 0; j < PUZZLE_COLS; j++)
            {
                int colNum = j + 1;
                if (colNum % 2 == 0)
                    Console.Write(colNum % 10);
                else
                    Console.Write(" ");
            }
            Console.WriteLine();

            char[] pieceNames = new char[numberOfPieces];
            for (int i = 0; i < numberOfPieces; i++) pieceNames[i] = (char)('A' + i);

            for (int row = 0; row < maxRows; row++)
            {
                if (row < PUZZLE_ROWS)
                {
                    int rowNum = row + 1;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    if (rowNum % 2 == 0)
                        Console.Write((rowNum % 10) + " ");
                    else
                        Console.Write("  ");
                    Console.ResetColor();

                    for (int col = 0; col < PUZZLE_COLS; col++)
                    {
                        char cell = puzzle[row, col];

                        if (cell != '.')
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write(cell);
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.Write(cell);
                        }
                    }
                }
                else
                {
                    Console.Write(new string(' ', 2 + PUZZLE_COLS));
                }

                Console.Write("          ||          ");

                int currentGroupRow = row / pieceGroupHeight;
                int lineInPiece = row % pieceGroupHeight;

                if (lineInPiece < 5)
                {
                    for (int pCol = 0; pCol < piecesPerRow; pCol++)
                    {
                        int pieceIndex = (currentGroupRow * piecesPerRow) + pCol;

                        if (pieceIndex < numberOfPieces)
                        {
                            int squares = square[pieceIndex];

                            for (int j = 0; j < 5; j++)
                            {
                                if (pieces[squares, pieceIndex, lineInPiece, j] == 1)
                                {
                                    Console.ForegroundColor = pieceColors[pieceIndex % pieceColors.Length];
                                    Console.Write(pieceNames[pieceIndex]);
                                    Console.ResetColor();
                                }
                                else
                                {
                                    Console.Write(".");
                                }
                            }
                            Console.Write("  ");
                        }
                    }
                }
                Console.WriteLine();
            }
        }

        static void GeneratePiece(int squares, int index)
        {
            int[,] piece = new int[5, 5];
            int row = random.Next(0, 5);
            int column = random.Next(0, 5);
            piece[row, column] = 1;
            int placedNo = 1; ;

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

                        if (dir == -1) newI = i - 1;
                        if (dir == 1) newI = i + 1;
                        if (dir == 0) newJ = j - 1;
                        if (dir == 2) newJ = j + 1;

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
                        if (i < minRow) minRow = i;
                        if (j < minColumn) minColumn = j;
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

            if (isSame(piece1, piece2)) return true;
            if (isSame(rotate90, piece2)) return true;
            if (isSame(rotate180, piece2)) return true;
            if (isSame(rotate270, piece2)) return true;
            if (isSame(leftRight, piece2)) return true;
            if (isSame(upDown, piece2)) return true;

            return false;
        }

        static bool isSame(int[,] piece1, int[,] piece2)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece1[i, j] != piece2[i, j]) return false;
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

        static void GeneratePuzzle(int[] square, int counter)
        {
            // DEĞİŞİKLİK BURADA: Boyutları artık burada soruyoruz.
            Console.WriteLine("\n--- BOARD SETTINGS ---");
            Console.Write("Enter puzzle rows (height): ");
            PUZZLE_ROWS = Convert.ToInt32(Console.ReadLine());

            Console.Write("Enter puzzle columns (width): ");
            PUZZLE_COLS = Convert.ToInt32(Console.ReadLine());


            Console.WriteLine("Please enter minimum regularity: ");
            double min = Convert.ToDouble(Console.ReadLine());

            Console.WriteLine("Please enter maksimum regularity: ");
            double max = Convert.ToDouble(Console.ReadLine());

            do
            {
                puzzle = new char[PUZZLE_ROWS, PUZZLE_COLS];

                for (int i = 0; i < PUZZLE_ROWS; i++)
                {
                    for (int j = 0; j < PUZZLE_COLS; j++)
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
                        int row = random.Next(0, PUZZLE_ROWS);
                        int col = random.Next(0, PUZZLE_COLS);

                        if (!CanPlace(orientation, row, col))
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
                            if (TouchesExisting(orientation, row, col))
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

                ComputeRegularity();
            }
            while (regularity < min && regularity > max);
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

        static bool CanPlace(int[,] piece, int row, int column)
        {
            int maxRows = puzzle.GetLength(0);
            int maxCols = puzzle.GetLength(1);

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        if (row + i >= maxRows || column + j >= maxCols)
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

        static bool TouchesExisting(int[,] piece, int row, int column)
        {
            int maxRows = puzzle.GetLength(0);
            int maxCols = puzzle.GetLength(1);

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        int r = row + i;
                        int c = column + j;

                        if (r > 0 && puzzle[r - 1, c] != '.') return true;
                        if (r < maxRows - 1 && puzzle[r + 1, c] != '.') return true;
                        if (c > 0 && puzzle[r, c - 1] != '.') return true;
                        if (c < maxCols - 1 && puzzle[r, c + 1] != '.') return true;
                    }
                }
            }
            return false;
        }

        static int ComputePerimeter()
        {
            int perimeter = 0;
            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };

            for (int r = 0; r < PUZZLE_ROWS; r++)
            {
                for (int c = 0; c < PUZZLE_COLS; c++)
                {
                    if (puzzle[r, c] == '.') continue;

                    for (int k = 0; k < 4; k++)
                    {
                        int nr = r + dr[k];
                        int nc = c + dc[k];

                        if (nr < 0 || nr >= PUZZLE_ROWS ||
                            nc < 0 || nc >= PUZZLE_COLS ||
                            puzzle[nr, nc] == '.')
                        {
                            perimeter++;
                        }
                    }
                }
            }
            return perimeter;
        }

        static double ComputeRegularity()
        {
            int squares = TotalSquares();
            int perimeter = ComputePerimeter();

            if (perimeter == 0) return 0.0;

            double p = perimeter / 4.0;
            return regularity = squares / (p * p);
        }

        static int TotalNumberofSquares(int numberOfPieces, int[] square)
        {
            int totalSquares = 0;
            for (int i = 0; i < numberOfPieces; i++)
            {
                totalSquares += square[i];
            }
            Console.WriteLine($"Total number of squares in all pieces: {totalSquares}");
            return totalSquares;
        }

        static int TotalSquares()
        {
            int count = 0;
            for (int i = 0; i < PUZZLE_ROWS; i++)
            {
                for (int j = 0; j < PUZZLE_COLS; j++)
                {
                    if (puzzle[i, j] != '.') count++;
                }
            }
            return count;
        }
    }
}

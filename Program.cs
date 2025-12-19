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

        // Bu sabitler artık GeneratePuzzle içinde kullanıcıdan alındığı için 
        // dinamik olarak puzzle.GetLength(0) ile yönetilecek ama global kalabilirler.
        static int PUZZLE_COLS = 20;
        static int PUZZLE_ROWS = 30;

        static void Main()
        {
            PrintWelcomeScreen();
            Console.Write("Enter number of pieces (1–20): ");
            int numberOfPieces = Convert.ToInt32(Console.ReadLine());

            int[] square = new int[numberOfPieces];
            int[] countBySquares = new int[13];

            // Oyuncudan parça boyutlarını al
            GetPieceSizesFromUser(numberOfPieces, square, countBySquares);

            // Parçaları oluştur ve normalize et, unique kontrolü yap
            GenerateAndValidatePieces(numberOfPieces, square);

            // Tüm oryantasyonları göster
            ShowAllPieceOrientations(numberOfPieces, square);

            // Oyun Alanını (Puzzle'ı) Oluştur (Regularity kontrollü)
            GeneratePuzzle(square, numberOfPieces);

            // Oyun Ekranını Printleme
            PrintGameScreen(square, numberOfPieces);
            Console.ReadLine();
        }

        // --- GÜNCELLENMİŞ GENERATE PUZZLE (DO-WHILE DÖNGÜSÜ İLE) ---
        static void GeneratePuzzle(int[] square, int numberOfPieces)
        {
            // 1. Puzzle Boyutunu Kullanıcıdan Al
            Console.Write("Enter puzzle size (e.g. 15 for 15x15): ");
            int size = Convert.ToInt32(Console.ReadLine());

            // Global puzzle dizisini oluştur (Dinamik boyut)
            puzzle = new char[size, size];

            // 2. Regularity Kriterlerini Al
            Console.WriteLine("\n--- Regularity Settings ---");
            Console.Write("Enter Minimum Regularity (e.g., 0,5): ");
            double minReg = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter Maximum Regularity (e.g., 1,0): ");
            double maxReg = Convert.ToDouble(Console.ReadLine());

            // Toplam kare sayısını hesapla
            int totalSquares = TotalNumberofSquares(numberOfPieces, square);

            bool isSuccess = false;
            int attempt = 0;
            int maxAttempts = 5000000; // Sonsuz döngü koruması

            Console.WriteLine("\nGenerating puzzle fitting the criteria...");

            // --- DO-WHILE DÖNGÜSÜ ---
            do
            {
                attempt++;

                // A. Tahtayı Temizle
                for (int i = 0; i < size; i++)
                    for (int j = 0; j < size; j++)
                        puzzle[i, j] = '.';

                bool allPiecesPlaced = true;

                // B. Tüm Parçaları Yerleştirmeyi Dene
                for (int idx = 0; idx < numberOfPieces; idx++)
                {
                    int[,] piece = LoadPiece(square[idx], idx);
                    int[,] orientation = RandomOrientation(piece);
                    Normalize(orientation);

                    bool placed = false;
                    // Bir parça için 100 kere rastgele konum dene
                    for (int t = 0; t < 100 && !placed; t++)
                    {
                        int row = random.Next(0, size);
                        int col = random.Next(0, size);

                        if (CanPlace(orientation, row, col, size))
                        {
                            // İlk parça rastgele, diğerleri temas etmeli
                            if (idx == 0 || TouchesExisting(orientation, row, col, size))
                            {
                                PlacePiece(orientation, row, col, (char)('A' + idx));
                                placed = true;
                            }
                        }
                    }

                    if (!placed)
                    {
                        allPiecesPlaced = false;
                        break; // Bu deneme başarısız, döngüyü kır
                    }
                }

                // C. Eğer tüm parçalar yerleştiyse Regularity Kontrolü Yap
                if (allPiecesPlaced)
                {
                    int perimeter = ComputePerimeter();
                    double currentReg = RegularityMath(totalSquares, perimeter);

                    // Kriterlere uyuyor mu?
                    if (currentReg >= minReg && currentReg <= maxReg)
                    {
                        isSuccess = true;
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"\nSUCCESS: Puzzle found in attempt {attempt}!");
                        Console.WriteLine($"Perimeter: {perimeter}, Regularity: {currentReg:F4}");
                        Console.ResetColor();
                    }
                }

                if (attempt % 1000 == 0) Console.Write(".");

            } while (!isSuccess && attempt < maxAttempts);

            if (!isSuccess)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nFAILED: Could not find a puzzle between {minReg}-{maxReg} regularity in {maxAttempts} tries.");
                Console.ResetColor();
            }
        }

        // --- GÜNCELLENMİŞ COMPUTE PERIMETER (DİNAMİK BOYUT İÇİN) ---
        static int ComputePerimeter()
        {
            if (puzzle == null) return 0;

            int perimeter = 0;
            // Sabit PUZZLE_ROWS yerine gerçek boyutu alıyoruz
            int rows = puzzle.GetLength(0);
            int cols = puzzle.GetLength(1);

            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    // Boşsa geç
                    if (puzzle[r, c] == '.') continue;

                    for (int k = 0; k < 4; k++)
                    {
                        int nr = r + dr[k];
                        int nc = c + dc[k];

                        // Tahta dışı veya boş hücre = Kenar
                        if (nr < 0 || nr >= rows ||
                            nc < 0 || nc >= cols ||
                            puzzle[nr, nc] == '.')
                        {
                            perimeter++;
                        }
                    }
                }
            }
            return perimeter;
        }

        // --- YARDIMCI MATEMATİK FONKSİYONLARI ---
        static int TotalNumberofSquares(int numberOfPieces, int[] square)
        {
            int totalSquares = 0;
            for (int i = 0; i < numberOfPieces; i++)
            {
                totalSquares += square[i];
            }
            // Console.WriteLine($"Total number of squares in all pieces: {totalSquares}");
            return totalSquares;
        }

        static double RegularityMath(int totalNumberOfSquares, int totalLenghtPerimeter)
        {
            if (totalLenghtPerimeter == 0) return 0;

            double side = totalLenghtPerimeter / 4.0;
            double regularity = (double)totalNumberOfSquares / (side * side);

            return regularity;
        }

        // --- DİĞER MEVCUT FONKSİYONLAR ---

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

            int puzzleSize = puzzle.GetLength(0);

            int piecesPerRow = 5;
            int pieceGroupHeight = 6;
            int totalPieceRows = (int)Math.Ceiling((double)numberOfPieces / piecesPerRow);
            int totalRightSideHeight = totalPieceRows * pieceGroupHeight;
            int maxRows = Math.Max(puzzleSize, totalRightSideHeight);

            Console.WriteLine();
            Console.Write("  ");
            for (int j = 0; j < puzzleSize; j++)
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
                if (row < puzzleSize)
                {
                    int rowNum = row + 1;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    if (rowNum % 2 == 0)
                        Console.Write((rowNum % 10) + " ");
                    else
                        Console.Write("  ");
                    Console.ResetColor();

                    for (int col = 0; col < puzzleSize; col++)
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
                    Console.Write(new string(' ', 2 + puzzleSize));
                }

                Console.Write("         ||         ");

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
            int g = 0;
            do
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
                g++;
            }
            while (g < 100); // 100000 yerine 100 yeterli, zaten main döngüsü kontrol ediyor
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
                for (int j = 0; j < 5; j++)
                    pieces[squares, index, i, j] = piece[i, j];
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
                for (int j = 0; j < 5; j++)
                    piece[i, j] = pieces[squares, index, i, j];
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
                for (int j = 0; j < 5; j++)
                    if (piece1[i, j] != piece2[i, j]) return false;
            return true;
        }

        static void Rotate90(int[,] piece, int[,] row)
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    row[j, 4 - i] = piece[i, j];
        }

        static void Rotate180(int[,] piece, int[,] row)
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    row[4 - i, 4 - j] = piece[i, j];
        }

        static void Rotate270(int[,] piece, int[,] row)
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    row[4 - j, i] = piece[i, j];
        }

        static void ReverseLR(int[,] piece, int[,] row)
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    row[i, 4 - j] = piece[i, j];
        }

        static void ReverseUD(int[,] piece, int[,] row)
        {
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 5; j++)
                    row[4 - i, j] = piece[i, j];
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
                        if (row + i >= size || column + j >= size) return false;
                        if (puzzle[row + i, column + j] != '.') return false;
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

                        if (r > 0 && puzzle[r - 1, c] != '.') return true;
                        if (r < size - 1 && puzzle[r + 1, c] != '.') return true;
                        if (c > 0 && puzzle[r, c - 1] != '.') return true;
                        if (c < size - 1 && puzzle[r, c + 1] != '.') return true;
                    }
                }
            }
            return false;
        }
    }
}

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
        static int PUZZLE_ROWS = 20;
        static int PUZZLE_COLS= 30;

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

            // Tüm oryantasyonları göster ve oyun ekranını hazırla
            ShowAllPieceOrientations(numberOfPieces, square);

            ShowAllPieceOrientations(numberOfPieces, square);

            Console.Write("Enter Minimum Regularity (0,0 - 1,0): ");
            double minReg = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter Maximum Regularity (0,0 - 1,0): ");
            double maxReg = Convert.ToDouble(Console.ReadLine());

            GeneratePuzzle(square, numberOfPieces, minReg, maxReg);

            PrintGameScreen(square, numberOfPieces);
            Console.ReadLine();

            // Oyun Ekranını Printleme
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

            // Ekranın dikey ortasını bul
            int topPadding = (Console.WindowHeight / 2) - 6;
            if (topPadding < 0) topPadding = 0;

            for (int i = 0; i < topPadding; i++) Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Blue;

            // Her satırı ortalayarak yazdır
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
                    NormalizePieceIn4D(square[i], i); // Normalizasyon 

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
                //printpiece fonksiyonunu sildim ve yerine PrintMatrix kullandım
                int[,] generatedPiece = LoadPiece(square[i], i); // Parçayı 4D'den 2D'ye yükle
                PrintMatrix(generatedPiece);                      // 2D matrisi bas
            }
        }
        static void ShowAllPieceOrientations(int numberOfPieces, int[] square)
        {
            Console.WriteLine("\n|| Piece Orientations Check ||");

            for (int idx = 0; idx < numberOfPieces; idx++)
            {
                Console.WriteLine($"\n--- Piece {idx + 1} ({square[idx]} Square) ---");
                int[,] originalPiece = LoadPiece(square[idx], idx);

                // Orijinal Parça (Normalize Edilmiş)
                Console.WriteLine("Original Piece:");
                PrintMatrix(originalPiece);

                // 4 Rotasyon
                int[,] current = originalPiece;
                for (int r = 1; r < 4; r++)
                {
                    int[,] rotated = new int[5, 5];
                    Rotate90(current, rotated);
                    Normalize(rotated); // Her operasyondan sonra normalizasyon 
                    Console.WriteLine($"{r * 90}° Rotation");
                    PrintMatrix(rotated);
                    current = rotated;
                }

                // Yatay Ters Çevirme (Left-Right Reverse)
                int[,] leftRight = new int[5, 5];
                ReverseLR(originalPiece, leftRight);
                Normalize(leftRight);
                Console.WriteLine("Left-Right Reverse (LR):");
                PrintMatrix(leftRight);

                // Dikey Ters Çevirme (Up-Down Reverse)
                int[,] upDown = new int[5, 5];
                ReverseUD(originalPiece, upDown);
                Normalize(upDown);
                Console.WriteLine("Up-Down Reverse (UD):");
                PrintMatrix(upDown);
            }
            Console.WriteLine("\n" + new string('-', 50));
        }

        // PrintPiece fonksiyonunun 2D matrisi basan genel bir versiyonu
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

            // Sağ taraf için gerekli yükseklik hesaplamaları
            int piecesPerRow = 5;
            int pieceGroupHeight = 6;
            int totalPieceRows = (int)Math.Ceiling((double)numberOfPieces / piecesPerRow);
            int totalRightSideHeight = totalPieceRows * pieceGroupHeight;
            int maxRows = Math.Max(puzzleSize, totalRightSideHeight);

            // ÜST KOORDİNAT ÇUBUĞU 
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

            // OYUN EKRANI (PUZZLE + PARÇALAR)
            char[] pieceNames = new char[numberOfPieces];
            for (int i = 0; i < numberOfPieces; i++) pieceNames[i] = (char)('A' + i);

            for (int row = 0; row < maxRows; row++)
            {
                // SOL TARAF (PUZZLE - SADECE MAVİ)
                if (row < puzzleSize)
                {
                    // Sol Satır Numarası
                    int rowNum = row + 1;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    if (rowNum % 2 == 0)
                        Console.Write((rowNum % 10) + " ");
                    else
                        Console.Write("  ");
                    Console.ResetColor();

                    // Puzzle İçeriği
                    for (int col = 0; col < puzzleSize; col++)
                    {
                        char cell = puzzle[row, col];

                        if (cell != '.') // Eğer nokta değilse (yani bir parçaysa)
                        {
                            //harf ne olursa olsun MAVİ renkte yazdır
                            Console.ForegroundColor = ConsoleColor.Blue;
                            Console.Write(cell);
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.Write(cell); // Noktalar standart renk
                        }
                    }
                }
                else
                {
                    Console.Write(new string(' ', 2 + puzzleSize));
                }

                // ORTA AYRAÇ 
                Console.Write("         ||         ");

                // SAĞ TARAF (PARÇALARA ÖZEL RENKLENDİRME) 
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
        static void GeneratePuzzle(int[] square, int numberOfPieces, double minReg, double maxReg)
        {
            // size yerine sabit puzzle boyutları kullanılıyor
            int rows = PUZZLE_ROWS;
            int cols = PUZZLE_COLS;


            int totalSquares = 0;
            for (int i = 0; i < numberOfPieces; i++) totalSquares += square[i];

            // --- YENİ EKLENEN KISIM: SIRALAMA MANTIĞI ---
            // 1. İndeksleri tutan bir dizi oluştur (0, 1, 2, ... N)
            int[] sortedIndices = new int[numberOfPieces];
            for (int i = 0; i < numberOfPieces; i++) sortedIndices[i] = i;

            // 2. Bubble Sort ile BÜYÜKTEN KÜÇÜĞE sırala
            // Böylece en büyük parçalar (örneğin 12 kareli olan) listenin başına gelir.
            for (int i = 0; i < numberOfPieces - 1; i++)
            {
                for (int j = 0; j < numberOfPieces - i - 1; j++)
                {
                    // Eğer sonraki parça öncekinden büyükse yer değiştir
                    if (square[sortedIndices[j]] < square[sortedIndices[j + 1]])
                    {
                        int temp = sortedIndices[j];
                        sortedIndices[j] = sortedIndices[j + 1];
                        sortedIndices[j + 1] = temp;
                    }
                }
            }
            // ---------------------------------------------

            bool isPuzzleValid = false;
            double currentRegularity = 0;
            int attempts = 0;
            int maxGlobalAttempts = 200000;

            do
            {
                attempts++;
                isPuzzleValid = true;

                puzzle = new char[PUZZLE_ROWS, PUZZLE_COLS];
                for (int i = 0; i < PUZZLE_ROWS; i++)
                    for (int j = 0; j < PUZZLE_COLS; j++)
                        puzzle[i, j] = '.';


                // --- PARÇALARI YERLEŞTİRME DÖNGÜSÜ ---
                // Döngü artık sıralanmış indeksler üzerinden dönüyor (k: sıra numarası)
                for (int k = 0; k < numberOfPieces; k++)
                {
                    int idx = sortedIndices[k]; // Sıradaki en büyük parçanın orijinal indeksi
                    int[,] piece = LoadPiece(square[idx], idx);

                    int bestRow = -1;
                    int bestCol = -1;
                    int[,] bestOrientation = null;
                    double minScoreFound = double.MaxValue;
                    bool placedAtLeastOnce = false;

                    int candidateTries = 300;

                    for (int t = 0; t < candidateTries; t++)
                    {
                        int[,] orientation = RandomOrientation(piece);
                        Normalize(orientation);

                        int row, col;

                        // ARTIK KONTROL 'k == 0' (Sıralamadaki ilk/en büyük parça mı?)
                        if (k == 0)
                        {
                            row = (PUZZLE_ROWS / 2) - 2;
                            col = (PUZZLE_COLS / 2) - 2;

                            if (row < 0) row = 0;
                            if (col < 0) col = 0;
                        }
                        else
                        {
                            row = random.Next(0, PUZZLE_ROWS);
                            col = random.Next(0, PUZZLE_COLS);

                        }

                        if (!CanPlace(orientation, row, col)) continue;

                        // İlk parça değilse temas etmeli (k > 0 kontrolü)
                        if (k > 0 && !TouchesExisting(orientation, row, col)) continue;

                        // --- ADAY DEĞERLENDİRME ---
                        PlacePiece(orientation, row, col, 'X');

                        int currentPerimeter = CalculatePerimeter();
                        double distToCenter = CalculateDistanceToCenter(orientation, row, col);
                        double score = currentPerimeter + (distToCenter * 0.5);

                        if (score < minScoreFound)
                        {
                            minScoreFound = score;
                            bestRow = row;
                            bestCol = col;
                            bestOrientation = orientation;
                            placedAtLeastOnce = true;
                        }

                        RemovePiece(orientation, row, col);

                        // En büyük parça (ilk parça) için tek deneme yeterli
                        if (k == 0) break;
                    }

                    if (placedAtLeastOnce)
                    {
                        PlacePiece(bestOrientation, bestRow, bestCol, (char)('A' + idx));
                    }
                    else
                    {
                        isPuzzleValid = false;
                        break;
                    }
                }

                if (isPuzzleValid)
                {
                    int perimeter = CalculatePerimeter();
                    currentRegularity = CalculateRegularityScore(totalSquares, perimeter);

                    if (currentRegularity < minReg || currentRegularity > maxReg)
                    {
                        isPuzzleValid = false;
                    }
                }

                if (attempts > maxGlobalAttempts)
                {
                    Console.WriteLine($"\nFailed to find a puzzle after {maxGlobalAttempts} attempts.");
                    return;
                }

                if (attempts % 5000 == 0) Console.Write(".");

            } while (!isPuzzleValid);

            Console.WriteLine($"\n\nPuzzle Generated Successfully in {attempts} attempts.");
            Console.WriteLine($"Final Regularity: {currentRegularity:F4}");
        }
        static void RemovePiece(int[,] piece, int row, int column)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        puzzle[row + i, column + j] = '.';
                    }
                }
            }
        }

        // Parçanın ağırlık merkezinin, haritanın merkezine uzaklığını hesaplar
        static double CalculateDistanceToCenter(int[,] piece, int row, int col)
        {
            double centerX = PUZZLE_ROWS / 2.0;
            double centerY = PUZZLE_COLS / 2.0;

            double pieceTotalRow = 0;
            double pieceTotalCol = 0;
            int count = 0;

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        // Parçanın o anki board üzerindeki koordinatı
                        pieceTotalRow += (row + i);
                        pieceTotalCol += (col + j);
                        count++;
                    }
                }
            }

            if (count == 0) return 0;

            // Parçanın ağırlık merkezi
            double pCenterRow = pieceTotalRow / count;
            double pCenterCol = pieceTotalCol / count;

            // Öklid mesafesi
            double dist = Math.Sqrt(Math.Pow(pCenterRow - centerY, 2) + Math.Pow(pCenterCol - centerX, 2));
            return dist;
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
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        int r= row+ i;
                        int c= column + j;
                        if(r<0|| r >=PUZZLE_ROWS) return false;
                        if(c<0|| c >=PUZZLE_COLS) return false;

                        if (puzzle[r,c]!='.')
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
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        int r = row + i;
                        int c = column + j;

                        // Yukarı
                        if (r > 0 && puzzle[r - 1, c] != '.')
                            return true;

                        // Aşağı  ✅ DÜZELTİLDİ
                        if (r < PUZZLE_ROWS - 1 && puzzle[r + 1, c] != '.')
                            return true;

                        // Sol
                        if (c > 0 && puzzle[r, c - 1] != '.')
                            return true;

                        // Sağ  ✅ DÜZELTİLDİ
                        if (c < PUZZLE_COLS - 1 && puzzle[r, c + 1] != '.')
                            return true;
                    }
                }
            }
            return false;
        }

        static int CalculatePerimeter()
        {
            int perimeter = 0;

            for (int i = 0; i < PUZZLE_ROWS; i++)
            {
                for (int j = 0; j < PUZZLE_COLS; j++)
                {
                    if (puzzle[i, j] != '.')
                    {
                        if (i == 0 || puzzle[i - 1, j] == '.') perimeter++;
                        if (i == PUZZLE_ROWS - 1 || puzzle[i + 1, j] == '.') perimeter++;
                        if (j == 0 || puzzle[i, j - 1] == '.') perimeter++;
                        if (j == PUZZLE_COLS - 1 || puzzle[i, j + 1] == '.') perimeter++;
                    }
                }
            }
            return perimeter;
        }


        static double CalculateRegularityScore(int totalSquares, int perimeter)
        {
            if (perimeter == 0) return 0;

            double denominatorBase = perimeter / 4.0;
            double regularity = totalSquares / (denominatorBase * denominatorBase);

            return regularity;
        }



    }
}

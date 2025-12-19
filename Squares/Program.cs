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
        static  int PUZZLE_COLS = 30;
        static  int TRIALS_PER_PIECE = 10000;
        static int centerR = PUZZLE_ROWS / 2;
        static int centerC = PUZZLE_COLS / 2;
        static bool[] used;
        static bool DEBUG_MODE = false;
        // Debug log için liste
        static System.Collections.Generic.List<string> debugLog = new System.Collections.Generic.List<string>();
        
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
            
            Console.Write("Enter minimum regularity: ");
            double minReg = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter maximum regularity: ");
            double maxReg = Convert.ToDouble(Console.ReadLine());
            
            // Tüm oryantasyonları göster ve oyun ekranını hazırla
            ShowAllPieceOrientations(numberOfPieces, square);
    
            // Oyun Alanını (Puzzle'ı) Oluştur ve Ekrana Bas
            // Puzzle oluşturma
            GeneratePuzzle(square, numberOfPieces, minReg, maxReg);// GeneratePuzzle kullanıcıdan boyutu alır ve puzzle'ı doldurur.
            // Oyun Ekranını Printleme
            PrintGameScreen(square, numberOfPieces);
            PrintDebugLog();
            Console.ReadLine();
        }
        static void PrintDebugLog()
        {
            if (!DEBUG_MODE) return;

            Console.WriteLine();
            Console.WriteLine("======== DEBUG LOG ========");

            if (debugLog.Count == 0)
            {
                Console.WriteLine("(no debug entries)");
            }
            else
            {
                foreach (string line in debugLog)
                    Console.WriteLine(line);
            }

            Console.WriteLine("====== END DEBUG LOG ======");
        }
        
        static void DebugLog(string msg)
        {
            if (!DEBUG_MODE) return;
            debugLog.Add(msg);
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
        static void RemovePiece(int[,] piece, int row, int col) //Regularity hesaplaması için geçici parça kaldırma
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        puzzle[row + i, col + j] = '.';
                    }
                }
            }
        }
        static int TotalSquares() //Puzzle'daki toplam dolu kare sayısını döner.(Regularity hesaplaması için)
        {
            int count = 0;

            for (int i = 0; i < PUZZLE_ROWS; i++)
            {
                for (int j = 0; j < PUZZLE_COLS; j++)
                {
                    if (puzzle[i, j] != '.')
                        count++;
                }
            }

            return count;
        }
        static int ComputePerimeter() //Puzzle'daki toplam çevre uzunluğunu döner.(Regularity hesaplaması için)
        {
            int perimeter = 0;

            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };

            for (int r = 0; r < PUZZLE_ROWS; r++)
            {
                for (int c = 0; c < PUZZLE_COLS; c++)
                {
                    if (puzzle[r, c] == '.')
                        continue;

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
        static double ComputeRegularity() //Puzzle'ın regularity değerini döner.
        {
            int squares = TotalSquares();
            int perimeter = ComputePerimeter();

            if (perimeter == 0)
                return 0.0;

            double p = perimeter / 4.0;
            return squares / (p * p);
        }
        static int ComputeBoundingBoxArea()
        {
            int minR = PUZZLE_ROWS, maxR = -1;
            int minC = PUZZLE_COLS, maxC = -1;

            for (int r = 0; r < PUZZLE_ROWS; r++)
            {
                for (int c = 0; c < PUZZLE_COLS; c++)
                {
                    if (puzzle[r, c] != '.')
                    {
                        if (r < minR) minR = r;
                        if (r > maxR) maxR = r;
                        if (c < minC) minC = c;
                        if (c > maxC) maxC = c;
                    }
                }
            }

            if (maxR < minR || maxC < minC)
                return 0;

            return (maxR - minR + 1) * (maxC - minC + 1);
        }
        static void PlaceFirstPieceAtCenter(int pieceIndex, int squares)
        {
            int[,] piece = LoadPiece(squares, pieceIndex);
            Normalize(piece);

            int centerR = PUZZLE_ROWS / 2;
            int centerC = PUZZLE_COLS / 2;

            PlacePiece(piece, centerR, centerC, (char)('A' + pieceIndex));
        }

        static void SelectAndPlaceBestNextPiece(int[] square, int count, double minReg, double maxReg)
        {
            DebugLog("---- Selecting next piece ----");

            double bestReg = -1.0;
            int bestPiece = -1;
            int bestRow = -1, bestCol = -1;
            int[,] bestOrientation = null;

            for (int p = 0; p < count; p++)
            {
                if (used[p]) continue;
                DebugLog($"Trying piece {(char)('A' + p)} ({square[p]} squares)");


                int[,] basePiece = LoadPiece(square[p], p);

                for (int t = 0; t < TRIALS_PER_PIECE; t++)
                {
                    int[,] oriented = RandomOrientation(basePiece);
                    Normalize(oriented);

                    int r = random.Next(0, PUZZLE_ROWS);
                    int c = random.Next(0, PUZZLE_COLS);

                    if (!CanPlace(oriented, r, c, PUZZLE_ROWS)) continue;
                    if (!TouchesExisting(oriented, r, c, PUZZLE_ROWS)) continue;

                    PlacePiece(oriented, r, c, (char)('A' + p));
                    double reg = ComputeRegularity();
                    DebugLog($"  candidate {(char)('A' + p)} at ({r},{c}) -> reg={reg:F4}");
                    RemovePiece(oriented, r, c);

                    if (reg < minReg || reg > maxReg) continue;

                    if (reg > bestReg)
                    {
                        bestReg = reg;
                        bestPiece = p;
                        bestRow = r;
                        bestCol = c;
                        bestOrientation = oriented;
                        DebugLog($"  BEST so far = {(char)('A' + p)} reg={reg:F4}");

                    }
                }
            }

            // === commit best piece ===
            PlacePiece(bestOrientation, bestRow, bestCol, (char)('A' + bestPiece));
            DebugLog(
                $"FINAL PLACEMENT: {(char)('A' + bestPiece)} at ({bestRow},{bestCol})"
            );

            DebugLog(
                $"After placement: Squares={TotalSquares()}, " +
                $"Per={ComputePerimeter()}, " +
                $"Box={ComputeBoundingBoxArea()}, " +
                $"Reg={ComputeRegularity():F4}"
            );

            used[bestPiece] = true;
        }
        
        static void GeneratePuzzle(int[] square, int count, double minReg, double maxReg)
        {
            puzzle = new char[PUZZLE_ROWS, PUZZLE_COLS];
            for (int i = 0; i < PUZZLE_ROWS; i++)
            for (int j = 0; j < PUZZLE_COLS; j++)
                puzzle[i, j] = '.';

            used = new bool[count];

            // === STEP 1: find largest piece ===
            int first = -1;
            int maxSquares = -1;

            for (int i = 0; i < count; i++)
            {
                if (square[i] > maxSquares)
                {
                    maxSquares = square[i];
                    first = i;
                }
            } 

            PlaceFirstPieceAtCenter(first, square[first]);
            DebugLog($"FIRST PIECE PLACED: {(char)('A' + first)} ({square[first]} squares)");
            DebugLog($"After first: Squares={TotalSquares()}, Per={ComputePerimeter()}, Box={ComputeBoundingBoxArea()}, Reg={ComputeRegularity():F4}");
            
            used[first] = true;

            int placedCount = 1;
            
            DebugLog("---- STARTING GREEDY SELECTION LOOP ----");
            // === STEP 2: greedy selection loop ===
            while (placedCount < count)
            {
                SelectAndPlaceBestNextPiece(square, count, minReg, maxReg);
                placedCount++;
            }
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
        
    }
}

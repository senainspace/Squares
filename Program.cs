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
        static char[,] solutionPuzzle;
        static char[,] targetSilhouette;

        static int PUZZLE_ROWS = 20;
        static int PUZZLE_COLS = 30;

        static bool godMode = false;

        static void Main()
        {
            PrintWelcomeScreen();

            int round = 0;
            double totalGameScore = 0;
            double highScore = 0;
            bool isPlaying = true;

           
            int numberOfPieces = 0;
            int[] square = null;
            int[] countBySquares = null;
            double minReg = 0.0;
            double maxReg = 0.0;

            
            bool changeSettings = true;

            while (isPlaying)
            {
                round++;
                Console.Clear();

                
                if (changeSettings)
                {
                    
                    Console.Write("Enter number of pieces (1–20): ");
                    numberOfPieces = Convert.ToInt32(Console.ReadLine());

                    square = new int[numberOfPieces];
                    countBySquares = new int[13]; 

                    GetPieceSizesFromUser(numberOfPieces, square, countBySquares);
                    GenerateAndValidatePieces(numberOfPieces, square);

                    Console.WriteLine();
                    Console.WriteLine();

                   
                }

                
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("=========================================");
                Console.WriteLine("            ROUND " + round);
                Console.WriteLine("   Current Total Score: " + totalGameScore.ToString("F2"));
                Console.WriteLine("   High Score (Best Round): " + highScore.ToString("F2"));
                Console.WriteLine("=========================================\n");
                Console.ResetColor();
                
                Console.Write("Enter Min Regularity (0,0 - 1,0): ");
                minReg = Convert.ToDouble(Console.ReadLine());

                Console.Write("Enter Max Regularity (0,0 - 1,0): ");
                maxReg = Convert.ToDouble(Console.ReadLine());

                // minReg maxReg check
                if (minReg > maxReg)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Error: Minimum Regularity cannot be greater than Maximum Regularity.");
                    Console.ResetColor();
                    Console.WriteLine("Please enter valid regularity values.");
                    Console.WriteLine("Press ENTER to re-enter values.");
                    Console.ReadLine();
                    round--; // Aynı roundı tekrarlatmak için round sayısını azalttık
                    continue;  // Bu roundu atla kullanıcıdan yeniden isteyecek
                }
                bool generated = GeneratePuzzle(square, numberOfPieces, minReg, maxReg);

                if (!generated)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nCould not generate a puzzle fitting these criteria.");
                    Console.WriteLine("Redirecting to settings...");
                    Console.ResetColor();
                    
                    changeSettings = true;
                    round--; 
                    Console.WriteLine("Press ENTER to try new settings.");
                    Console.ReadLine();
                    continue;
                }
                
                int perimeter = CalculatePerimeter(solutionPuzzle); 
                int totalSquares = 0; 
                for (int i = 0; i < numberOfPieces; i++)
                {
                    totalSquares = totalSquares + square[i]; 
                }
                double puzzleReg = CalculateRegularityScore(totalSquares, perimeter);

                Console.WriteLine("\nPuzzle Generated! Regularity: " + puzzleReg.ToString("F4")); // double olan regularityi stringe çevirip 4 ondalık basamak gösterir
                Console.WriteLine("\nTARGET SHAPE (Fill the 'X' area):");

                PrintGameScreen(targetSilhouette, square, numberOfPieces, false);

                Console.WriteLine("\nPress ENTER to start playing...");
                Console.ReadLine();
                
                bool roundWon = PlayGame(square, numberOfPieces, puzzleReg, totalSquares);

                if (roundWon)
                {
                    double roundScore = totalSquares * Math.Pow((4 * puzzleReg), 4);
                    totalGameScore = totalGameScore + roundScore;

                    if (roundScore > highScore)
                    {
                        highScore = roundScore;
                    }

                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nCONGRATULATIONS! ROUND " + round + " COMPLETED!");
                    Console.WriteLine("Round Score: " + roundScore.ToString("F2"));
                    Console.WriteLine("Total Score: " + totalGameScore.ToString("F2"));
                    Console.ResetColor();

                    Console.WriteLine("\nComputer's Solution was:");
                    PrintGameScreen(solutionPuzzle, square, numberOfPieces, true);
                    Console.WriteLine();
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nGAME OVER / ESCAPED! Round " + round + " ended.");
                    Console.WriteLine("Final Total Score: " + totalGameScore.ToString("F2"));
                    Console.ResetColor();

                    Console.WriteLine("\nThe solution was:");
                    PrintGameScreen(solutionPuzzle, square, numberOfPieces, true);
                    Console.WriteLine();
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    Console.Clear();
                }
                

                int secim = InteractiveRoundChoice();

                if (secim == 1) 
                {
                    changeSettings = false;
                }
                else if (secim == 2) 
                {
                    changeSettings = true;
                }
                else 
                {
                    isPlaying = false;
                }
            }

            Console.WriteLine("Exiting... Press any key.");
            Console.ReadKey();
        }
        
        public static int InteractiveRoundChoice()
        {
            Console.CursorVisible = false;
            int currentSelection = 0;
            string[] options = {
        "Next Round (Keep same pieces)",
        "New Game (Select new pieces)",
        "Exit Game"
        };

            while (true)
            {
                Console.Clear();

                
                int totalLines = 14;
                int startY = (Console.WindowHeight - totalLines) / 2;

                
                if (startY < 0) startY = 0;

                Console.SetCursorPosition(0, startY);

               
                Console.ForegroundColor = ConsoleColor.Cyan;
                WriteCentered("╔════════════════════════════════╗");
                WriteCentered("║   What would you like to do?   ║");
                WriteCentered("╚════════════════════════════════╝");
                Console.WriteLine(); 

                
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == currentSelection)
                    {
                        
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.Gray;
                        WriteCentered($"  >> {options[i]} <<  ");
                    }
                    else
                    {
                        
                        Console.ResetColor();
                        WriteCentered($"     {options[i]}     ");
                    }
                    
                    Console.WriteLine();
                }

                Console.ResetColor();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                WriteCentered("----------------------------------");
                WriteCentered("Use [Arrow Keys] to Navigate, [Enter] to Select");

                
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.UpArrow:
                        currentSelection--;
                        if (currentSelection < 0) currentSelection = options.Length - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        currentSelection++;
                        if (currentSelection >= options.Length) currentSelection = 0;
                        break;
                    case ConsoleKey.Enter:
                        Console.CursorVisible = true;
                        return currentSelection + 1;
                }
            }
        }
        static void WriteCentered(string text)
        {
            int screenWidth = Console.WindowWidth;
            int stringWidth = text.Length;
            int left = (screenWidth - stringWidth) / 2;
            if (left < 0) left = 0;

            Console.SetCursorPosition(left, Console.CursorTop);
            Console.WriteLine(text);
        }

        static bool PlayGame(int[] square, int numberOfPieces, double targetRegularity, int totalSquares)
        {

            puzzle = new char[PUZZLE_ROWS, PUZZLE_COLS];
            for (int i = 0; i < PUZZLE_ROWS; i++)
            {
                for (int j = 0; j < PUZZLE_COLS; j++)
                {
                    puzzle[i, j] = '.';
                }
            }
            
            int[][,] playerPieces = new int[numberOfPieces][,];
            for (int k = 0; k < numberOfPieces; k++)
            {
                playerPieces[k] = ExtractPieceFromBoard(solutionPuzzle, (char)('A' + k));
            }

            int cursorRow = PUZZLE_ROWS / 2;
            int cursorCol = PUZZLE_COLS / 2;
            int selectedPieceIndex = 0;
            bool[] placedPieces = new bool[numberOfPieces];
            int[,] activePiece = playerPieces[0];
            int lastPlacedPiece = -1;

            ConsoleKey key;
            bool gameRunning = true;

            while (gameRunning)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=== ROUND PLAY MODE ===");
                Console.ResetColor();
                Console.WriteLine("Target Regularity: " + targetRegularity.ToString("F4"));
                Console.WriteLine("ARROWS: Move | TAB: Next | R: Rotate | F: Flip | ENTER: Place | U: Undo | G: GodMode | ESC: Give Up\n");


                int piecesPerRow = 5;
                int pieceGroupHeight = 6;
                int totalPieceRows = (int)Math.Ceiling((double)numberOfPieces / piecesPerRow);
                int totalRightSideHeight = totalPieceRows * pieceGroupHeight;
                int maxRows;

                if (PUZZLE_ROWS > totalRightSideHeight)
                {
                    maxRows = PUZZLE_ROWS;
                }
                else
                {
                    maxRows = totalRightSideHeight;
                }


                Console.Write("   ");
                for (int j = 0; j < PUZZLE_COLS; j++)
                {
                    if ((j + 1) % 2 == 0)
                    {
                        Console.Write((j + 1) % 10);
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                }
                Console.WriteLine();

                for (int row = 0; row < maxRows; row++)
                {

                    if (row < PUZZLE_ROWS)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        int rowNum = row + 1;
                        if (rowNum % 2 == 0)
                        {
                            Console.Write((rowNum % 10) + " ");
                        }
                        else
                        {
                            Console.Write("  ");
                        }
                        Console.ResetColor();

                        for (int col = 0; col < PUZZLE_COLS; col++)
                        {
                            bool isPreview = false;
                            if (!placedPieces[selectedPieceIndex])
                            {
                                for (int pi = 0; pi < 5; pi++)
                                {
                                    for (int pj = 0; pj < 5; pj++)
                                    {
                                        if (activePiece[pi, pj] == 1 && row == cursorRow + pi && col == cursorCol + pj)
                                        {
                                            isPreview = true;
                                        }
                                    }
                                }
                            }


                            if (isPreview)
                            {
                                if (puzzle[row, col] != '.' || row >= PUZZLE_ROWS || col >= PUZZLE_COLS)
                                {
                                    Console.BackgroundColor = ConsoleColor.DarkRed;
                                }
                                else
                                {
                                    Console.BackgroundColor = ConsoleColor.Gray;
                                }

                                Console.ForegroundColor = ConsoleColor.Black;
                                Console.Write('#');
                                Console.ResetColor();
                            }
                            else if (puzzle[row, col] != '.')
                            {

                                if (godMode)
                                {
                                    int pIdx = puzzle[row, col] - 'A';
                                    Console.ForegroundColor = pieceColors[pIdx % pieceColors.Length];
                                    Console.Write(puzzle[row, col]);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.Write('█');
                                }
                                Console.ResetColor();
                            }
                            else if (!godMode && targetSilhouette[row, col] == 'X')
                            {

                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                Console.Write('X');
                                Console.ResetColor();
                            }
                            else if (godMode && solutionPuzzle[row, col] != '.')
                            {

                                int pIdx = solutionPuzzle[row, col] - 'A';
                                Console.ForegroundColor = pieceColors[pIdx % pieceColors.Length];
                                Console.Write(solutionPuzzle[row, col]);
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.Write('.');
                            }
                        }
                    }
                    else
                    {
                        Console.Write(new string(' ', 3 + PUZZLE_COLS));
                    }

                    Console.Write("     ||     ");
                    
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


                                if (pieceIndex == selectedPieceIndex)
                                {
                                    Console.BackgroundColor = ConsoleColor.DarkGray;
                                }

                                for (int j = 0; j < 5; j++)
                                {
                                    if (pieces[squares, pieceIndex, lineInPiece, j] == 1)
                                    {
                                        if (placedPieces[pieceIndex])
                                        {
                                            Console.ForegroundColor = ConsoleColor.DarkGreen;
                                            Console.Write("✔");
                                        }
                                        else
                                        {
                                            Console.ForegroundColor = pieceColors[pieceIndex % pieceColors.Length];
                                            Console.Write((char)('A' + pieceIndex));
                                        }
                                        Console.ResetColor();
                                    }
                                    else
                                    {
                                        Console.Write(".");
                                    }
                                }
                                Console.ResetColor();
                                Console.Write("  ");
                            }
                        }
                    }
                    Console.WriteLine();
                }


                key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.UpArrow && cursorRow > -4)
                {
                    cursorRow--;
                }
                else if (key == ConsoleKey.DownArrow && cursorRow < PUZZLE_ROWS - 1)
                {
                    cursorRow++;
                }
                else if (key == ConsoleKey.LeftArrow && cursorCol > -4)
                {
                    cursorCol--;
                }
                else if (key == ConsoleKey.RightArrow && cursorCol < PUZZLE_COLS - 1)
                {
                    cursorCol++;
                }
                else if (key == ConsoleKey.Tab)
                {
                    int start = selectedPieceIndex;
                    do
                    {
                        selectedPieceIndex = (selectedPieceIndex + 1) % numberOfPieces;
                    } while (placedPieces[selectedPieceIndex] && selectedPieceIndex != start);
                    activePiece = playerPieces[selectedPieceIndex];
                }
                else if (key == ConsoleKey.R)
                {
                    int[,] tmp = new int[5, 5];
                    Rotate90(activePiece, tmp);
                    Normalize(tmp);
                    activePiece = tmp;
                }
                else if (key == ConsoleKey.F)
                {
                    int[,] tmp = new int[5, 5];
                    ReverseLR(activePiece, tmp);
                    Normalize(tmp);
                    activePiece = tmp;
                }
                else if (key == ConsoleKey.G)
                {
                    godMode = !godMode;
                }
                else if (key == ConsoleKey.U)
                {
                    if (lastPlacedPiece != -1 && placedPieces[lastPlacedPiece])
                    {
                        int p = lastPlacedPiece;
                        RemovePieceByLetter((char)('A' + p));
                        placedPieces[p] = false;
                        selectedPieceIndex = p;
                        activePiece = playerPieces[p];
                        lastPlacedPiece = -1;
                    }
                }
                else if (key == ConsoleKey.Enter)
                {
                    if (!placedPieces[selectedPieceIndex])
                    {
                        if (CanPlace(activePiece, cursorRow, cursorCol))
                        {
                            PlacePiece(activePiece, cursorRow, cursorCol, (char)('A' + selectedPieceIndex));
                            placedPieces[selectedPieceIndex] = true;
                            lastPlacedPiece = selectedPieceIndex;


                            for (int k = 0; k < numberOfPieces; k++)
                            {
                                if (!placedPieces[k])
                                {
                                    selectedPieceIndex = k;
                                    activePiece = playerPieces[k];
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (key == ConsoleKey.Escape)
                {
                    return false;
                }


                bool allPlaced = true;
                for (int i = 0; i < numberOfPieces; i++)
                {
                    if (!placedPieces[i])
                    {
                        allPlaced = false;
                    }
                }

                if (allPlaced)
                {
                    if (ValidateSolution(puzzle, targetSilhouette))
                    {
                        return true;
                    }
                    else
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nShape Mismatch! Although pieces fit, the shape is wrong.");
                        Console.ResetColor();
                        Console.WriteLine("Press any key to undo last move...");
                        Console.ReadKey();
                        RemovePieceByLetter((char)('A' + lastPlacedPiece));
                        placedPieces[lastPlacedPiece] = false;
                    }
                }
            }
            return false;
        }
        
        static bool GeneratePuzzle(int[] square, int numberOfPieces, double minReg, double maxReg)
        {
            if (minReg > maxReg)
            {
                double t = minReg;
                minReg = maxReg;
                maxReg = t;
            }

            int[] sortedIndices = new int[numberOfPieces];
            for (int i = 0; i < numberOfPieces; i++)
            {
                sortedIndices[i] = i;
            }


            for (int i = 0; i < numberOfPieces - 1; i++)
            {
                for (int j = 0; j < numberOfPieces - i - 1; j++)
                {
                    if (square[sortedIndices[j]] < square[sortedIndices[j + 1]])
                    {
                        int temp = sortedIndices[j];
                        sortedIndices[j] = sortedIndices[j + 1];
                        sortedIndices[j + 1] = temp;
                    }
                }
            }

            int maxTries = 5000;
            bool success = false;
            int tryCount = 0;

            char[,] bestPuzzleSoFar = new char[PUZZLE_ROWS, PUZZLE_COLS];
            double bestRegSoFar = 0.0;
            double bestDiffSoFar = double.MaxValue;

            int centerR = PUZZLE_ROWS / 2;
            int centerC = PUZZLE_COLS / 2;

            Console.WriteLine("Generating Puzzle (High Compactness Mode)...");

            while (!success && tryCount < maxTries)
            {
                tryCount++;
                puzzle = new char[PUZZLE_ROWS, PUZZLE_COLS];
                for (int i = 0; i < PUZZLE_ROWS; i++)
                {
                    for (int j = 0; j < PUZZLE_COLS; j++)
                    {
                        puzzle[i, j] = '.';
                    }
                }

                bool allPiecesPlaced = true;

                for (int k = 0; k < numberOfPieces; k++)
                {
                    int idx = sortedIndices[k];
                    int[,] piece = LoadPiece(square[idx], idx);

                    int bestRow = -1, bestCol = -1;
                    int[,] bestOrientation = null;
                    double minScoreFound = double.MaxValue;
                    bool foundSpot = false;

                    int candidateTries = 400;

                    for (int t = 0; t < candidateTries; t++)
                    {
                        int[,] orientation = RandomOrientation(piece);
                        Normalize(orientation);

                        int row, col;
                        if (k == 0)
                        {
                            row = centerR - 2;
                            col = centerC - 2;
                        }
                        else
                        {

                            int r = 8;
                            row = centerR + random.Next(-r, r);
                            col = centerC + random.Next(-r, r);
                        }

                        if (!CanPlace(orientation, row, col))
                        {
                            continue;
                        }
                        if (k > 0 && !TouchesExisting(orientation, row, col))
                        {
                            continue;
                        }

                        PlacePiece(orientation, row, col, 'X');

                        int currentPerimeter = CalculatePerimeter(puzzle);
                        double dist = CalculateDistanceToCenter(orientation, row, col);
                        double score = currentPerimeter + (dist * 2.0);

                        if (score < minScoreFound)
                        {
                            minScoreFound = score;
                            bestRow = row;
                            bestCol = col;
                            bestOrientation = orientation;
                            foundSpot = true;
                        }

                        RemovePiece(orientation, row, col);
                        if (k == 0)
                        {
                            break;
                        }
                    }

                    if (foundSpot)
                    {
                        PlacePiece(bestOrientation, bestRow, bestCol, (char)('A' + idx));
                    }
                    else
                    {
                        allPiecesPlaced = false;
                        break;
                    }
                }

                if (allPiecesPlaced)
                {
                    int per = CalculatePerimeter(puzzle);
                    int tot = 0;
                    for (int i = 0; i < numberOfPieces; i++)
                    {
                        tot = tot + square[i];
                    }
                    double reg = CalculateRegularityScore(tot, per);

                    if (reg >= minReg && reg <= maxReg)
                    {
                        success = true;
                    }
                    else
                    {
                        double diff;
                        if (reg < minReg)
                        {
                            diff = minReg - reg;
                        }
                        else
                        {
                            diff = reg - maxReg;
                        }

                        if (diff < bestDiffSoFar)
                        {
                            bestDiffSoFar = diff;
                            bestRegSoFar = reg;
                            Array.Copy(puzzle, bestPuzzleSoFar, puzzle.Length);
                        }
                    }
                }
                if (tryCount % 500 == 0)
                {
                    Console.Write(".");
                }
            }

            if (!success && bestRegSoFar > 0)
            {
                Array.Copy(bestPuzzleSoFar, puzzle, bestPuzzleSoFar.Length);
                Console.WriteLine("\n[Info] Exact range not found. Best match: " + bestRegSoFar.ToString("F4"));
                success = true;
            }

            if (success)
            {
                solutionPuzzle = new char[PUZZLE_ROWS, PUZZLE_COLS];
                targetSilhouette = new char[PUZZLE_ROWS, PUZZLE_COLS];
                for (int i = 0; i < PUZZLE_ROWS; i++)
                {
                    for (int j = 0; j < PUZZLE_COLS; j++)
                    {
                        solutionPuzzle[i, j] = puzzle[i, j];
                        if (puzzle[i, j] == '.')
                        {
                            targetSilhouette[i, j] = '.';
                        }
                        else
                        {
                            targetSilhouette[i, j] = 'X';
                        }
                    }
                }
                return true;
            }
            return false;
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

        static void PrintGameScreen(char[,] board, int[] square, int numberOfPieces, bool isSolution)
        {
            if (board == null)
            {
                return;
            }

            int piecesPerRow = 5; //Her satırda gösterilecek parça sayısı
            int pieceGroupHeight = 6; //Her parça grubunun yüksekliği (5 satır parça + 1 boşluk)
            int totalPieceRows = (int)Math.Ceiling((double)numberOfPieces / piecesPerRow); //Ekranda gösterilecek toplam parça satırı sayısı
            int totalRightSideHeight = totalPieceRows * pieceGroupHeight; //Sağ tarafta gösterilecek toplam yükseklik
            int maxRows; //Ekranda gösterilecek maksimum gereken satır sayısı

            if (PUZZLE_ROWS > totalRightSideHeight) //Puzzle için yeterli satır varsa
            {
                maxRows = PUZZLE_ROWS; //Puzzle satır sayısını kullan
            }
            else
            {
                maxRows = totalRightSideHeight; //Yoksa sağ tarafın yüksekliğini kullan
            }

            Console.Write("   ");
            for (int j = 0; j < PUZZLE_COLS; j++)
            {
                if ((j + 1) % 2 == 0)
                {
                    Console.Write((j + 1) % 10);
                }
                else
                {
                    Console.Write(" ");
                }
            }
            Console.WriteLine();

            for (int i = 0; i < maxRows; i++)
            {
                if (i < PUZZLE_ROWS)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    int rowNum = i + 1;
                    if (rowNum % 2 == 0)
                    {
                        Console.Write((rowNum % 10) + " ");
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                    Console.ResetColor();

                    for (int j = 0; j < PUZZLE_COLS; j++)
                    {
                        char c = board[i, j];
                        if (isSolution && c != '.' && c != 'X')
                        {
                            Console.ForegroundColor = pieceColors[(c - 'A') % pieceColors.Length];
                            Console.Write(c);
                            Console.ResetColor();
                        }
                        else if (c == 'X')
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                            Console.Write('X');
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.Write(c);
                        }
                    }
                }
                else
                {
                    Console.Write(new string(' ', 3 + PUZZLE_COLS));
                }

                Console.Write("     ||     ");

                int currentGroupRow = i / pieceGroupHeight;
                int lineInPiece = i % pieceGroupHeight;
                if (lineInPiece < 5)
                {
                    for (int pCol = 0; pCol < piecesPerRow; pCol++)
                    {
                        int pIdx = (currentGroupRow * piecesPerRow) + pCol;
                        if (pIdx < numberOfPieces)
                        {
                            int sz = square[pIdx];
                            for (int j = 0; j < 5; j++)
                            {
                                if (pieces[sz, pIdx, lineInPiece, j] == 1)
                                {
                                    Console.ForegroundColor = pieceColors[pIdx % pieceColors.Length];
                                    Console.Write((char)('A' + pIdx));
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

        static bool ValidateSolution(char[,] playerBoard, char[,] targetBoard)
        {
            for (int i = 0; i < PUZZLE_ROWS; i++)
            {
                for (int j = 0; j < PUZZLE_COLS; j++)
                {
                    bool p = (playerBoard[i, j] != '.');
                    bool t = (targetBoard[i, j] == 'X');
                    if (p != t)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        static void GetPieceSizesFromUser(int n, int[] sq, int[] cnt)
        {
            int[] maxP = { 0, 0, 1, 2, 5, 12, 35, 108, 369, 1285, 4655, 17073, 63600 };
            for (int i = 0; i < n; i++)
            {
                bool v = false;
                while (!v)
                {
                    Console.Write("Enter number of squares for piece " + (i + 1) + " (2–12): ");
                    int s = Convert.ToInt32(Console.ReadLine());
                    if (s < 2 || s > 12)
                    {
                        Console.WriteLine("ERROR: 2-12 only.");
                        continue;
                    }
                    if (cnt[s] >= maxP[s])
                    {
                        Console.WriteLine("Limit reached.");
                        continue;
                    }
                    sq[i] = s;
                    cnt[s]++;
                    v = true;
                }
            }
        }

        static void GenerateAndValidatePieces(int n, int[] sq)
        {
            for (int i = 0; i < n; i++)
            {
                bool d;
                do
                {
                    GeneratePiece(sq[i], i);
                    NormalizePieceIn4D(sq[i], i);
                    d = false;
                    for (int p = 0; p < i; p++)
                    {
                        if (PiecesEqual(LoadPiece(sq[i], i), LoadPiece(sq[p], p)))
                        {
                            d = true;
                            break;
                        }
                    }
                } while (d);
               
            }
        }

        static double CalculateRegularityScore(int totalSquares, int perimeter)
        {
            if (perimeter == 0)
            {
                return 0;
            }
            double d = perimeter / 4.0;
            return totalSquares / (d * d);
        }

        static int CalculatePerimeter(char[,] p)
        {
            int per = 0;
            for (int i = 0; i < PUZZLE_ROWS; i++)
            {
                for (int j = 0; j < PUZZLE_COLS; j++)
                {
                    if (p[i, j] != '.')
                    {
                        if (i == 0 || p[i - 1, j] == '.') per++;
                        if (i == PUZZLE_ROWS - 1 || p[i + 1, j] == '.') per++;
                        if (j == 0 || p[i, j - 1] == '.') per++;
                        if (j == PUZZLE_COLS - 1 || p[i, j + 1] == '.') per++;
                    }
                }
            }
            return per;
        }

        static double CalculateDistanceToCenter(int[,] piece, int row, int col)
        {
            double cX = PUZZLE_ROWS / 2.0;
            double cY = PUZZLE_COLS / 2.0;
            double tR = 0;
            double tC = 0;
            int cnt = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        tR = tR + (row + i);
                        tC = tC + (col + j);
                        cnt++;
                    }
                }
            }
            if (cnt == 0)
            {
                return 0;
            }
            return Math.Sqrt(Math.Pow((tR / cnt) - cX, 2) + Math.Pow((tC / cnt) - cY, 2));
        }

        static void GeneratePiece(int s, int idx)
        {
            int[,] p = new int[5, 5];
            p[random.Next(0, 5), random.Next(0, 5)] = 1;
            int placed = 1;
            while (placed < s)
            {
                int n = AddNeighbor(p, placed);
                if (n != placed)
                {
                    placed = n;
                }
            }
            Normalize(p);
            SaveTo4D(p, s, idx);
        }

        static int AddNeighbor(int[,] p, int c)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (p[i, j] == 1)
                    {
                        int d = random.Next(0, 4);
                        int ni = i;
                        int nj = j;
                        if (d == 0) ni = i - 1;
                        if (d == 1) ni = i + 1;
                        if (d == 2) nj = j - 1;
                        if (d == 3) nj = j + 1;

                        if (ni >= 0 && ni < 5 && nj >= 0 && nj < 5)
                        {
                            if (p[ni, nj] == 0)
                            {
                                p[ni, nj] = 1;
                                return c + 1;
                            }
                        }
                    }
                }
            }
            return c;
        }

        static void Normalize(int[,] p)
        {
            int mr = 5;
            int mc = 5;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (p[i, j] == 1)
                    {
                        if (i < mr) mr = i;
                        if (j < mc) mc = j;
                    }
                }
            }
            int[,] t = new int[5, 5];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (p[i, j] == 1)
                    {
                        t[i - mr, j - mc] = 1;
                    }
                }
            }
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    p[i, j] = t[i, j];
                }
            }
        }

        static void SaveTo4D(int[,] p, int s, int idx)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    pieces[s, idx, i, j] = p[i, j];
                }
            }
        }

        static void NormalizePieceIn4D(int s, int idx)
        {
            int[,] p = LoadPiece(s, idx);
            Normalize(p);
            SaveTo4D(p, s, idx);
        }

        static int[,] LoadPiece(int s, int idx)
        {
            int[,] p = new int[5, 5];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    p[i, j] = pieces[s, idx, i, j];
                }
            }
            return p;
        }

        static bool CanPlace(int[,] piece, int row, int column)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (piece[i, j] == 1)
                    {
                        if (row + i >= PUZZLE_ROWS || column + j >= PUZZLE_COLS || row + i < 0 || column + j < 0)
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

        static void RemovePieceByLetter(char letter)
        {
            for (int i = 0; i < PUZZLE_ROWS; i++)
            {
                for (int j = 0; j < PUZZLE_COLS; j++)
                {
                    if (puzzle[i, j] == letter)
                    {
                        puzzle[i, j] = '.';
                    }
                }
            }
        }

        static int[,] ExtractPieceFromBoard(char[,] board, char letter) // Undo function için
        {
            int[,] piece = new int[5, 5];
            int minR = PUZZLE_ROWS;
            int minC = PUZZLE_COLS;
            for (int i = 0; i < PUZZLE_ROWS; i++)
            {
                for (int j = 0; j < PUZZLE_COLS; j++)
                {
                    if (board[i, j] == letter)
                    {
                        if (i < minR) minR = i;
                        if (j < minC) minC = j;
                    }
                }
            }
            for (int i = 0; i < PUZZLE_ROWS; i++)
            {
                for (int j = 0; j < PUZZLE_COLS; j++)
                {
                    if (board[i, j] == letter)
                    {
                        int r = i - minR;
                        int c = j - minC;
                        if (r < 5 && c < 5)
                        {
                            piece[r, c] = 1;
                        }
                    }
                }
            }
            return piece;
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
                        if (r > 0 && puzzle[r - 1, c] != '.') return true;
                        if (r < PUZZLE_ROWS - 1 && puzzle[r + 1, c] != '.') return true;
                        if (c > 0 && puzzle[r, c - 1] != '.') return true;
                        if (c < PUZZLE_COLS - 1 && puzzle[r, c + 1] != '.') return true;
                    }
                }
            }
            return false;
        }

        static int[,] RandomOrientation(int[,] p)
        {
            int[,] t = new int[5, 5];
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    t[i, j] = p[i, j];
                }
            }
            int r = random.Next(0, 4);
            for (int k = 0; k < r; k++)
            {
                int[,] tmp = new int[5, 5];
                Rotate90(t, tmp);
                t = tmp;
            }
            int f = random.Next(0, 3);
            if (f == 1)
            {
                int[,] tmp = new int[5, 5];
                ReverseLR(t, tmp);
                t = tmp;
            }
            else if (f == 2)
            {
                int[,] tmp = new int[5, 5];
                ReverseUD(t, tmp);
                t = tmp;
            }
            return t;
        }

        static bool PiecesEqual(int[,] p1, int[,] p2)
        {
            int[,] c = p1;
            for (int f = 0; f < 2; f++)
            {
                for (int r = 0; r < 4; r++)
                {
                    if (isSame(c, p2)) return true;
                    int[,] n = new int[5, 5];
                    Rotate90(c, n);
                    c = n;
                    Normalize(c);
                }
                int[,] fl = new int[5, 5];
                ReverseLR(c, fl);
                c = fl;
                Normalize(c);
            }
            return false;
        }

        static bool isSame(int[,] p1, int[,] p2)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (p1[i, j] != p2[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        static void Rotate90(int[,] s, int[,] d)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    d[j, 4 - i] = s[i, j];
                }
            }
        }

        static void ReverseLR(int[,] s, int[,] d)
        {
            for (int i = 0; i < 5; i++)
            {;
                for (int j = 0; j < 5; j++)
                {
                    d[i, 4 - j] = s[i, j];
                }
            }
        }

        static void ReverseUD(int[,] s, int[,] d)
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    d[4 - i, j] = s[i, j];
                }
            }
        }
    }
}
 

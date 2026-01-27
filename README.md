# Squares Project

**Squares** is a sophisticated, single-player algorithmic puzzle game developed in **C#**. [cite_start]It challenges spatial reasoning and geometric optimization through a system that focuses on procedural generation and strict regularity constraints to create a unique logic-based experience. [cite: 1, 2, 3]

## 🕹️ Core Concept
[cite_start]In **Squares**, players interact with a system that generates complex puzzles based on mathematical "regularity". [cite: 2, 13] [cite_start]The objective is to reconstruct a target silhouette—formed by merging various pieces—using a specific set of randomly generated and normalized geometric pieces. [cite: 3, 12, 19]

## 🚀 Key Technical Features

### 1. Procedural Piece Generation
* [cite_start]**Dynamic Geometry**: Pieces are procedurally generated within a 5x5 grid, consisting of 2 to 12 interconnected squares. [cite: 4]
* [cite_start]**Strict Adjacency**: The algorithm ensures squares are connected via edges (Up, Down, Left, Right); diagonal connections are strictly prohibited. [cite: 5]
* [cite_start]**Normalization Engine**: Every generated piece undergoes a "shift" operation to its leftmost and topmost position to ensure standardized processing and comparison. [cite: 6]



### 2. Intelligent Comparison & Orientation
[cite_start]To maintain a high-quality puzzle pool and ensure every piece is unique, the system performs rigorous checks: [cite: 7]
* [cite_start]**Symmetry Analysis**: Each piece is evaluated against existing pieces by calculating rotations (90°, 180°, 270°). [cite: 8]
* [cite_start]**Reflection Mapping**: The system checks mirrored (reversed) versions of pieces to prevent redundant gameplay elements. [cite: 8]
* [cite_start]**Post-Transformation Normalization**: Pieces are re-normalized after every rotation or reversal before the final comparison. [cite: 8]

### 3. The Regularity Algorithm
[cite_start]The engine uses a specific mathematical formula to determine the "regularity" or compactness of the generated puzzles: [cite: 13]

[cite_start]$$Regularity = \frac{TotalNumberOfSquares}{(\frac{TotalLengthOfPerimeter}{4})^2}$$ [cite: 13]

* [cite_start]**Environment**: Puzzles are formed within a 20x30 coordinate system. [cite: 10]
* [cite_start]**Capacity**: Supports a maximum of 20 unique pieces and a total density of up to 160 squares per puzzle. [cite: 11]


### 4. Advanced Scoring System
[cite_start]The scoring logic rewards players based on the complexity and regularity of the completed shape: [cite: 20]
[cite_start]$$Score = TotalSquares \times (4 \times Regularity)^4$$ [cite: 23]
## 🚀 Getting Started (How to Run)

### Prerequisites
* **.NET SDK** (6.0 or later recommended)
* An IDE like **Visual Studio**, **VS Code**, or **JetBrains Rider**.

### Installation & Execution
1. **Clone the Repository:**
   ```bash
   git clone [https://github.com/senainspace/Squares.git](https://github.com/senainspace/Squares.git)
   cd Squares
2. **Build & Run:**
   ```bash
     dotnet run

## 📂 Project Structure

This project is implemented as a cohesive C# application, logically organized into functional segments to handle the game's algorithmic complexity:

```text
Squares/
├── Squares.cs                  # Main application containing all game logic
├── Squares.sln                 # C# Solution file
└── README.md                   # Project documentation

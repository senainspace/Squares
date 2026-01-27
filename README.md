
# Squares Project

**Squares** is a sophisticated, single-player algorithmic puzzle game developed in **C#**. It   challenges spatial reasoning and geometric optimization through a system that focuses on procedural generation and strict regularity constraints to create a unique logic-based experience.

---

## 🕹️ Core Concept
In **Squares**, players interact with a system that generates complex puzzles based on mathematical "regularity". The objective is to reconstruct a target silhouette—formed by merging various pieces—using a specific set of randomly generated and normalized geometric pieces. 

---

## 🚀 Key Technical Features

### 1. Procedural Piece Generation
* **Dynamic Geometry**: Pieces are procedurally generated within a 5x5 grid, consisting of 2 to 12 interconnected squares. 
* **Strict Adjacency**: The algorithm ensures squares are connected via edges (Up, Down, Left, Right); diagonal connections are strictly prohibited. 
* **Normalization Engine**: Every generated piece undergoes a "shift" operation to its leftmost and topmost position to ensure standardized processing and comparison. 

---


### 2. Intelligent Comparison & Orientation
To maintain a high-quality puzzle pool and ensure every piece is unique, the system performs rigorous checks: 
* **Symmetry Analysis**: Each piece is evaluated against existing pieces by calculating rotations (90°, 180°, 270°). 
* **Reflection Mapping**: The system checks mirrored (reversed) versions of pieces to prevent redundant gameplay elements. 
* **Post-Transformation Normalization**: Pieces are re-normalized after every rotation or reversal before the final comparison. 

---

### 3. The Regularity Algorithm
The engine uses a specific mathematical formula to determine the "regularity" or compactness of the generated puzzles: 

Regularity = (TotalNumberOfSquares) / [(TotalLengthOfPerimeter) x (4)]^2

* **Environment**: Puzzles are formed within a 20x30 coordinate system. 
* **Capacity**: Supports a maximum of 20 unique pieces and a total density of up to 160 squares per puzzle. 


### 4. Advanced Scoring System
The scoring logic rewards players based on the complexity and regularity of the completed shape: 
$$Score = TotalSquares \times (4 \times Regularity)^4$$ 
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

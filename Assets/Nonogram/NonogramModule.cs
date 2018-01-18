using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using KMBombInfoExtensions;
using System.Collections;

public class NonogramModule : MonoBehaviour {

    private static int _moduleIdCounter = 1;
    private int _moduleId = 0;

    public KMAudio sound;
    public KMBombModule module;
    public KMBombInfo bombInfo;

    public KMSelectable[] gridButtons;
    public KMSelectable[] gridColorsSelect;

    public KMSelectable submitButton;
    public KMSelectable dotButton;
    public KMSelectable toggleButton;
    public KMSelectable clearButton;

    public MeshRenderer[] gridColors;
    public MeshRenderer[] gridButtonsFill;
    public MeshRenderer[] gridButtonsDot;
    public MeshRenderer dotButtonStatus;
    public MeshRenderer clearButtonStatus;

    public Material blackGridButton;
    public Material whiteGridButton;
    public Material greenGridButton;
    public Material redGridButton;
    public Material backgroundGrid;
    public Material statusOff;
    public Material statusOn;
    public Material statusConfirm;
    public Material offGridColor;
    public Material redGridColor;
    public Material blueGridColor;
    public Material greenGridColor;
    public Material yellowGridColor;
    public Material orangeGridColor;
    public Material purpleGridColor;

    string gridLog;
    int solutionAmountSquares;
    int generationAttempts = 0;
    bool evenSerialNumber;
    bool puzzleGenerated = false;
    bool moduleActivated = false;
    bool canInteract = false;
    bool isDotActive = false;
    bool isSecondaryColor = false;
    bool onClearConfirm = false;
    List<string> clues = new List<string>();
    List<string> colors = new List<string>();
    List<int> solution = new List<int>();
    List<int> currentGrid = new List<int>(Enumerable.Repeat(0, 25).ToArray());

    int[][] fallbackPuzzles = {
        new int[] { 1, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 17, 18, 19, 20, 21 },
        new int[] { 0, 1, 2, 3, 5, 6, 7, 9, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 22, 23 },
        new int[] { 0, 4, 5, 6, 8, 9, 10, 11, 12, 15, 16, 21, 22, 23 },
        new int[] { 2, 3, 5, 6, 7, 8, 9, 10, 12, 15, 16, 18, 19, 20, 21, 22, 23 },
        new int[] { 1, 2, 4, 5, 6, 8, 10, 12, 13, 14, 17, 18, 19, 20, 22 },
        new int[] { 0, 1, 3, 4, 5, 6, 10, 11, 13, 14, 15, 16, 18, 19, 21, 22, 23 },
        new int[] { 0, 1, 2, 3, 5, 6, 7, 9, 10, 11, 12, 13, 14, 15, 17, 19, 20, 21, 22, 24 },
        new int[] { 0, 2, 3, 4, 5, 7, 8, 9, 11, 16, 21, 24 },
        new int[] { 0, 1, 2, 3, 5, 6, 8, 9, 10, 11, 12, 13, 15, 16, 19, 20, 21, 22 },
        new int[] { 0, 1, 2, 3, 4, 6, 8, 9, 13, 14, 15, 16, 18, 22, 23 },
        new int[] { 0, 1, 3, 5, 6, 8, 11, 15, 16, 17, 18, 19, 20, 23 },
        new int[] { 1, 2, 3, 4, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 20, 22, 24 },
        new int[] { 1, 3, 5, 6, 7, 9, 10, 11, 13, 14, 16, 17, 18, 19, 22, 23 },
        new int[] { 0, 1, 2, 4, 5, 7, 8, 9, 10, 11, 13, 14, 15, 16, 17, 18, 20, 22, 23, 24 },
        new int[] { 0, 1, 2, 3, 4, 5, 7, 8, 9, 10, 13, 14, 15, 17, 18, 21, 22, 23 },
        new int[] { 0, 2, 3, 6, 9, 10, 12, 13, 16, 17, 18, 19, 20, 21, 22, 23 },
        new int[] { 0, 1, 3, 4, 5, 6, 7, 8, 9, 11, 12, 14, 16, 17, 19, 20, 22, 23, 24 },
        new int[] { 0, 2, 3, 5, 6, 8, 10, 11, 13, 19, 20, 21, 22 },
        new int[] { 0, 2, 3, 4, 5, 6, 9, 10, 11, 13, 14, 15, 16, 17, 18, 19, 21, 22, 24 },
        new int[] { 0, 2, 3, 4, 5, 6, 7, 11, 14, 20, 21, 22, 23 },
        new int[] { 0, 2, 3, 4, 5, 6, 7, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22 },
        new int[] { 0, 1, 5, 10, 11, 13, 14, 16, 17, 20, 21, 22 },
        new int[] { 1, 3, 5, 6, 7, 10, 11, 13, 14, 16, 18, 20, 21, 22, 23 },
        new int[] { 1, 4, 8, 10, 11, 12, 13, 15, 17, 18, 19, 20, 22, 23, 24 },
        new int[] { 0, 1, 3, 4, 8, 11, 13, 14, 16, 19, 21, 24 },
        new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 16, 18, 19, 20, 21, 24 },
        new int[] { 0, 3, 4, 5, 6, 8, 10, 11, 12, 13, 15, 16, 18, 20, 22, 23, 24 },
        new int[] { 1, 2, 3, 4, 5, 7, 10, 11, 12, 15, 17, 19, 20, 23 },
        new int[] { 0, 1, 2, 3, 4, 5, 6, 9, 10, 11, 13, 14, 15, 16, 17, 18, 20, 21, 22, 23 },
        new int[] { 2, 3, 5, 6, 7, 9, 10, 12, 13, 17, 22, 23 },
        new int[] { 2, 3, 5, 6, 7, 9, 12, 13, 14, 17, 18, 22, 23 },
        new int[] { 0, 1, 2, 4, 5, 6, 7, 9, 10, 11, 12, 15, 16, 17, 18, 19, 20, 21, 22, 24 },
        new int[] { 0, 1, 3, 4, 5, 6, 9, 11, 12, 13, 14, 15, 16, 17, 18, 19, 21, 22, 23, 24 },
        new int[] { 0, 1, 2, 3, 4, 5, 7, 8, 9, 10, 12, 13, 14, 15, 16, 18, 20, 21, 22 },
        new int[] { 0, 1, 2, 5, 7, 8, 9, 10, 12, 13, 14, 15, 17, 19, 22, 23, 24 },
        new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 19, 20, 22, 23 },
        new int[] { 2, 3, 4, 5, 6, 8, 9, 10, 11, 12, 13, 14, 16, 17, 18, 21, 22, 24 },
        new int[] { 2, 3, 5, 8, 12, 13, 14, 15, 16, 17, 19, 22, 24 },
        new int[] { 0, 1, 6, 10, 11, 13, 15, 17, 20, 21, 23, 24 },
        new int[] { 0, 2, 4, 5, 6, 7, 8, 9, 10, 11, 13, 16, 17, 18, 21, 24 },
        new int[] { 0, 1, 2, 3, 4, 6, 7, 8, 10, 11, 12, 14, 15, 17, 19, 21, 23, 24 },
        new int[] { 0, 1, 5, 6, 8, 9, 11, 12, 14, 15, 16, 17, 19, 21, 23, 24 },
        new int[] { 0, 3, 8, 9, 10, 12, 13, 14, 15, 16, 17, 19, 22, 23, 24 },
        new int[] { 0, 1, 2, 5, 7, 8, 10, 11, 17, 18, 19, 20, 22, 24 },
        new int[] { 1, 3, 7, 10, 11, 12, 13, 14, 15, 18, 20, 21, 24 },
        new int[] { 2, 3, 4, 7, 9, 11, 12, 13, 14, 15, 16, 17, 18, 20 },
        new int[] { 0, 1, 3, 5, 7, 8, 9, 11, 12, 13, 15, 16, 18, 19, 20, 21, 22, 23, 24 },
        new int[] { 1, 2, 3, 4, 5, 6, 7, 9, 11, 14, 15, 16, 17, 18, 19, 20, 22, 23, 24 },
        new int[] { 0, 2, 3, 4, 5, 8, 9, 10, 11, 14, 16, 17, 19, 22, 24 },
        new int[] { 0, 1, 2, 3, 4, 6, 8, 10, 13, 14, 15, 17, 19, 23, 24 },
        new int[] { 0, 1, 2, 4, 5, 7, 8, 9, 12, 13, 15, 16, 17, 18, 19, 22, 24 },
        new int[] { 0, 1, 2, 4, 5, 7, 8, 10, 12, 14, 15, 16, 17, 20, 21, 22, 23, 24 },
        new int[] { 0, 1, 3, 6, 7, 9, 10, 11, 13, 15, 16, 17, 18, 19, 20, 21, 22, 23 },
        new int[] { 0, 1, 2, 5, 6, 8, 9, 10, 11, 12, 13, 14, 16, 19, 20, 21, 24 },
        new int[] { 5, 6, 7, 8, 9, 11, 12, 13, 15, 17, 18, 19, 20, 23 },
        new int[] { 0, 2, 5, 6, 7, 8, 10, 13, 14, 17, 19, 20, 21, 23 },
        new int[] { 1, 3, 4, 5, 6, 7, 10, 11, 13, 14, 15, 16, 17, 18, 20, 21, 22, 24 },
        new int[] { 0, 1, 2, 3, 4, 5, 6, 9, 10, 11, 12, 13, 14, 15, 16, 18, 19, 20, 22, 24 },
        new int[] { 0, 3, 4, 6, 7, 8, 9, 11, 12, 13, 14, 17, 18, 19, 20, 21, 22 },
        new int[] { 0, 1, 2, 7, 8, 12, 13, 14, 15, 16, 17, 20, 22, 23 },
        new int[] { 1, 2, 3, 4, 6, 7, 8, 9, 10, 11, 13, 14, 15, 17, 18, 19, 20, 21, 22, 23 },
        new int[] { 1, 2, 3, 4, 5, 6, 7, 11, 12, 13, 14, 16, 18, 19, 23, 24 },
        new int[] { 2, 4, 5, 6, 8, 9, 10, 11, 16, 17, 18, 19, 21, 24 },
        new int[] { 0, 2, 4, 5, 6, 7, 11, 12, 13, 14, 15, 17, 18, 19, 22, 23 },
        new int[] { 0, 2, 3, 4, 5, 7, 11, 12, 14, 16, 17, 18 },
        new int[] { 2, 3, 4, 5, 6, 8, 9, 10, 11, 12, 13, 14, 15, 17, 18, 19, 20, 21, 24 },
        new int[] { 0, 1, 3, 4, 5, 7, 9, 10, 11, 12, 14, 15, 18, 19, 20, 21, 22, 23 },
        new int[] { 0, 1, 2, 4, 5, 6, 12, 15, 16, 17, 18, 22, 23, 24 }
    };

    int[][] colAndRows = new int[][] {
        new int[] { 0, 5, 10, 15, 20 },
        new int[] { 1, 6, 11, 16, 21 },
        new int[] { 2, 7, 12, 17, 22 },
        new int[] { 3, 8, 13, 18, 23 },
        new int[] { 4, 9, 14, 19, 24 },
        new int[] { 0, 1, 2, 3, 4 },
        new int[] { 5, 6, 7, 8, 9 },
        new int[] { 10, 11, 12, 13, 14 },
        new int[] { 15, 16, 17, 18, 19 },
        new int[] { 20, 21, 22, 23, 24 }
    };

    string[] gridIds = new string[] {
        "A1", "B1", "C1", "D1", "E1",
        "A2", "B2", "C2", "D2", "E2",
        "A3", "B3", "C3", "D3", "E3",
        "A4", "B4", "C4", "D4", "E4",
        "A5", "B5", "C5", "D5", "E5"
    };

    string[] colorIds = new string[] {
        "A", "B", "C", "D", "E",
        "1", "2", "3", "4", "5"
    };

    Material getGridColor(string color, bool second) {
        char c = second ? color[0] : color[2];
        
        return c == 'N' ? offGridColor :
            c == 'R' ? redGridColor :
            c == 'B' ? blueGridColor :
            c == 'G' ? greenGridColor :
            c == 'Y' ? yellowGridColor :
            c == 'O' ? orangeGridColor :
            c == 'P' ? purpleGridColor : null;
    }

    void chooseFallbackPuzzle() {
        int choice = UnityEngine.Random.Range(0, fallbackPuzzles.Length);
        solution = new List<int>(fallbackPuzzles[choice]);

        for (int i = 0; i < colAndRows.Length; i++) {
            bool[] active = new bool[5];

            for (int j = 0; j < 5; j++)
                active[j] = solution.Contains(colAndRows[i][j]);

            clues.Add(getClue(active));
        }

        if (moduleActivated)
            prepareModule();

        puzzleGenerated = true;
    }

    IEnumerator submitModule(bool pass) {
        canInteract = false;

        for (int i = 0; i < currentGrid.Count; i++) {
            sound.PlaySoundAtTransform(currentGrid[i] == 2 ? "fill" : "blank", gridButtons[i].transform);

            gridButtonsDot[i].material = backgroundGrid;
            gridButtonsFill[i].material = backgroundGrid;

            if (i > 0) {
                gridButtonsDot[i - 1].material = currentGrid[i - 1] > 0 ? blackGridButton : whiteGridButton;
                gridButtonsFill[i - 1].material = currentGrid[i - 1] > 1 ? blackGridButton : whiteGridButton;
            }

            yield return new WaitForSeconds(0.1f);
        }

        gridButtonsDot[24].material = currentGrid[24] > 0 ? blackGridButton : whiteGridButton;
        gridButtonsFill[24].material = currentGrid[24] > 1 ? blackGridButton : whiteGridButton;

        sound.PlaySoundAtTransform(pass ? "disarm" : "strike", gridButtons[13].transform);

        if (pass) {
            dotButtonStatus.material = statusOff;
            clearButtonStatus.material = statusOff;

            for (int i = 0; i < gridColors.Length; i++)
                gridColors[i].material = offGridColor;

            module.HandlePass();

            for (int i = 0; i < gridButtons.Length; i++) {
                gridButtonsDot[i].material = currentGrid[i] > 0 ? greenGridButton : whiteGridButton;
                gridButtonsFill[i].material = currentGrid[i] > 1 ? greenGridButton : whiteGridButton;
            }
        } else {
            module.HandleStrike();

            for (int i = 0; i < gridButtons.Length; i++) {
                gridButtonsDot[i].material = currentGrid[i] > 0 ? redGridButton : whiteGridButton;
                gridButtonsFill[i].material = currentGrid[i] > 1 ? redGridButton : whiteGridButton;
            }

            yield return new WaitForSeconds(2);

            for (int i = 0; i < gridButtons.Length; i++) {
                gridButtonsDot[i].material = currentGrid[i] > 0 ? blackGridButton : whiteGridButton;
                gridButtonsFill[i].material = currentGrid[i] > 1 ? blackGridButton : whiteGridButton;
            }

            canInteract = true;
        }
    }

    IEnumerator checkSolution() {
        string grid = new String('_', 25);
        int[][] l = colAndRows;
        int index = -1;
        int cycle = 0;

        while (grid.Contains("_") && cycle < 11) {
            index = index > 8 ? 0 : index + 1;
            cycle++;

            string row = "" + grid[l[index][0]] + grid[l[index][1]] + grid[l[index][2]] + grid[l[index][3]] + grid[l[index][4]];
            string oldGrid = grid;

            if (clues[index] == "0") {
                if (row != ":::::")
                    cycle = 0;

                grid = grid.Substring(0, l[index][0]) + ":" + grid.Substring(l[index][0] + 1);
                grid = grid.Substring(0, l[index][1]) + ":" + grid.Substring(l[index][1] + 1);
                grid = grid.Substring(0, l[index][2]) + ":" + grid.Substring(l[index][2] + 1);
                grid = grid.Substring(0, l[index][3]) + ":" + grid.Substring(l[index][3] + 1);
                grid = grid.Substring(0, l[index][4]) + ":" + grid.Substring(l[index][4] + 1);
                continue;
            }

            string expression1 = "^([:_]*)" + (
                clues[index].Length == 5 ? "([#_]{" + clues[index][0] + "})([:_]+)([#_]{" + clues[index][2] + "})([:_]+)([#_]{" + clues[index][4] + "})" :
                clues[index].Length == 3 ? "([#_]{" + clues[index][0] + "})([:_]+)([#_]{" + clues[index][2] + "})" :
                clues[index].Length == 1 ? "([#_]{" + clues[index] + "})" : null) + "([:_]*)$";

            string expression2 = "^([:_]*?)" + (
                clues[index].Length == 5 ? "([#_]{" + clues[index][0] + "})([:_]+?)([#_]{" + clues[index][2] + "})([:_]+?)([#_]{" + clues[index][4] + "})" :
                clues[index].Length == 3 ? "([#_]{" + clues[index][0] + "})([:_]+?)([#_]{" + clues[index][2] + "})" :
                clues[index].Length == 1 ? "([#_]{" + clues[index] + "})" : null) + "([:_]*?)$";

            Match regex1 = new Regex(expression1).Match(row);
            Match regex2 = new Regex(expression2).Match(row);

            string str1 = "", str2 = "";
            string ids1 = "", ids2 = "";

            char mode = '#';
            for (int i = 1; i < regex1.Groups.Count; i++) {
                mode = mode == '#' ? ':' : '#';
                for (int j = 0; j < regex1.Groups[i].Length; j++) {
                    str1 += mode;
                    ids1 += i;
                }
            }

            mode = '#';
            for (int i = 1; i < regex2.Groups.Count; i++) {
                mode = mode == '#' ? ':' : '#';
                for (int j = 0; j < regex2.Groups[i].Length; j++) {
                    str2 += mode;
                    ids2 += i;
                }
            }

            for (int i = 0; i < str1.Length; i++) {
                if (str1[i] == str2[i] && ids1[i] == ids2[i])
                    grid = grid.Substring(0, l[index][i]) + str1[i] + grid.Substring(l[index][i] + 1);
            }

            if (grid != oldGrid)
                cycle = 0;

            yield return null;
        }

        if (grid.Contains("_"))
            StartCoroutine(generatePuzzle());
        else {
            Debug.LogFormat("[Nonogram #{0}] Found solution-unique puzzle after {1} generation{2}.", _moduleId,
                generationAttempts, generationAttempts > 1 ? "s" : "");

            if (moduleActivated)
                prepareModule();

            puzzleGenerated = true;
        }
    }

    IEnumerator generatePuzzle() {
        if (generationAttempts >= 1000) {
            Debug.LogFormat("[Nonogram #{0}] Generated {1} puzzles; none of them were solution-unique.", _moduleId, generationAttempts);
            Debug.LogFormat("[Nonogram #{0}] Giving up and using a fallback puzzle instead...", _moduleId);

            chooseFallbackPuzzle();

            yield break;
        }

        generationAttempts++;

        List<int> selects = Enumerable.Range(0, 25).ToList();
        solutionAmountSquares = UnityEngine.Random.Range(12, 21);

        solution.Clear();
        clues.Clear();

        for (int i = 0; i < solutionAmountSquares; i++) {
            int index = UnityEngine.Random.Range(0, selects.Count);
            solution.Add(selects[index]);
            selects.RemoveAt(index);
        }

        solution.Sort();

        for (int i = 0; i < colAndRows.Length; i++) {
            bool[] active = new bool[5];

            for (int j = 0; j < 5; j++)
                active[j] = solution.Contains(colAndRows[i][j]);

            clues.Add(getClue(active));
        }

        yield return null;

        StartCoroutine(checkSolution());
    }

    string randReverse(string original) {
        return UnityEngine.Random.Range(0, 2) == 0 ?
            new string(original.ToCharArray().Reverse().ToArray()) : original;
    }

    string logGrid(int status) {
        return status > 1 ? "■" : " ";
    }

    string logGridSolution(int index) {
        return solution.Contains(index) ? "■" : " ";
    }

    string getFullColor(string original) {
        char[] order = { 'R', 'B', 'G', 'Y', 'O', 'P' };
        original = Array.IndexOf(order, original[0]) > Array.IndexOf(order, original[2]) ?
            original[2] + " & " + original[0] : original[0] + " & " + original[2];

        return original
            .Replace("R", "Red")
            .Replace("B", "Blue")
            .Replace("G", "Green")
            .Replace("Y", "Yellow")
            .Replace("O", "Orange")
            .Replace("P", "Purple");
    }

    string getClue(bool[] active) {
        string sequence = "";
        
        for (int i = 0; i < active.Length; i++)
            sequence += active[i] ? "o" : " ";

        string[] sqArray = sequence.Split(' ');
        string result = "";

        for (int j = 0; j < sqArray.Length; j++)
            result += sqArray[j].Length > 0 ? " " + sqArray[j].Length : "";

        return result.Length > 0 ? result.Substring(1) : "0";
    }

    void Start() {
        _moduleId = _moduleIdCounter++;

        Debug.LogFormat("[Nonogram #{0}] Generating a solution-unique puzzle...", _moduleId);
        StartCoroutine(generatePuzzle());

        module.OnActivate += delegate () {
            if (puzzleGenerated)
                prepareModule();

            moduleActivated = true;
        };

        gridLog += "┌───┬───┬───┬───┬───┐\n";
        gridLog += "│ {0} │ {1} │ {2} │ {3} │ {4} │\n";
        gridLog += "├───┼───┼───┼───┼───┤\n";
        gridLog += "│ {5} │ {6} │ {7} │ {8} │ {9} │\n";
        gridLog += "├───┼───┼───┼───┼───┤\n";
        gridLog += "│ {10} │ {11} │ {12} │ {13} │ {14} │\n";
        gridLog += "├───┼───┼───┼───┼───┤\n";
        gridLog += "│ {15} │ {16} │ {17} │ {18} │ {19} │\n";
        gridLog += "├───┼───┼───┼───┼───┤\n";
        gridLog += "│ {20} │ {21} │ {22} │ {23} │ {24} │\n";
        gridLog += "└───┴───┴───┴───┴───┘";
    }

    void Awake() {
        for (int i = 0; i < gridButtons.Length; i++) {
            int j = i;
            gridButtons[i].OnInteract += delegate () {
                onGridClick(j);
                return false;
            };
        }

        for (int i = 0; i < gridColorsSelect.Length; i++) {
            int j = i;
            gridColorsSelect[i].OnInteract += delegate () {
                onColorClick(j);
                return false;
            };
        }

        toggleButton.OnInteract += onLightToggle;
        dotButton.OnInteract += onDotToggle;
        clearButton.OnInteract += onClear;
        submitButton.OnInteract += onSubmit;
    }

    void prepareModule() {
        evenSerialNumber = bombInfo.GetSerialNumber().Last() % 2 == 0;

        for (int i = 0; i < clues.Count; i++) {
            switch (clues[i]) {
                case "1": colors.Add(evenSerialNumber ? randReverse("B O") : randReverse("Y O")); break;
                case "2": colors.Add(evenSerialNumber ? randReverse("R B") : randReverse("G P")); break;
                case "3": colors.Add(evenSerialNumber ? randReverse("Y O") : randReverse("B O")); break;
                case "4": colors.Add(evenSerialNumber ? randReverse("R G") : randReverse("B Y")); break;
                case "5": colors.Add(evenSerialNumber ? randReverse("G Y") : randReverse("R G")); break;
                case "1 1": colors.Add(evenSerialNumber ? randReverse("O P") : randReverse("R O")); break;
                case "1 2": colors.Add(evenSerialNumber ? randReverse("G O") : randReverse("B G")); break;
                case "1 3": colors.Add(evenSerialNumber ? randReverse("G P") : randReverse("B P")); break;
                case "2 1": colors.Add(evenSerialNumber ? randReverse("Y P") : randReverse("Y P")); break;
                case "2 2": colors.Add(evenSerialNumber ? randReverse("B P") : randReverse("G Y")); break;
                case "3 1": colors.Add(evenSerialNumber ? randReverse("R O") : randReverse("R B")); break;
                case "1 1 1": colors.Add(evenSerialNumber ? randReverse("R P") : randReverse("R Y")); break;
                default:
                    int random = UnityEngine.Random.Range(1, 4);
                    colors.Add(
                        random == 1 ? evenSerialNumber ? randReverse("R Y") : randReverse("R P") :
                        random == 2 ? evenSerialNumber ? randReverse("B G") : randReverse("G O") :
                        random == 3 ? evenSerialNumber ? randReverse("B G") : randReverse("O P") : null
                    );
                    break;
            }

            gridColors[i].material = getGridColor(colors[i], false);
        }

        string solutionGrid = string.Format(gridLog,
            logGridSolution(0), logGridSolution(1), logGridSolution(2), logGridSolution(3), logGridSolution(4),
            logGridSolution(5), logGridSolution(6), logGridSolution(7), logGridSolution(8), logGridSolution(9),
            logGridSolution(10), logGridSolution(11), logGridSolution(12), logGridSolution(13), logGridSolution(14),
            logGridSolution(15), logGridSolution(16), logGridSolution(17), logGridSolution(18), logGridSolution(19),
            logGridSolution(20), logGridSolution(21), logGridSolution(22), logGridSolution(23), logGridSolution(24)
        );

        Debug.LogFormat("[Nonogram #{0}] The last digit of the serial number is {1}.", _moduleId, evenSerialNumber ? "EVEN" : "ODD");
        Debug.LogFormat("[Nonogram #{0}] Clue for Column A is \"{2}\" ({1})", _moduleId, getFullColor(colors[0]), clues[0]);
        Debug.LogFormat("[Nonogram #{0}] Clue for Column B is \"{2}\" ({1})", _moduleId, getFullColor(colors[1]), clues[1]);
        Debug.LogFormat("[Nonogram #{0}] Clue for Column C is \"{2}\" ({1})", _moduleId, getFullColor(colors[2]), clues[2]);
        Debug.LogFormat("[Nonogram #{0}] Clue for Column D is \"{2}\" ({1})", _moduleId, getFullColor(colors[3]), clues[3]);
        Debug.LogFormat("[Nonogram #{0}] Clue for Column E is \"{2}\" ({1})", _moduleId, getFullColor(colors[4]), clues[4]);
        Debug.LogFormat("[Nonogram #{0}] Clue for Row 1 is \"{2}\" ({1})", _moduleId, getFullColor(colors[5]), clues[5]);
        Debug.LogFormat("[Nonogram #{0}] Clue for Row 2 is \"{2}\" ({1})", _moduleId, getFullColor(colors[6]), clues[6]);
        Debug.LogFormat("[Nonogram #{0}] Clue for Row 3 is \"{2}\" ({1})", _moduleId, getFullColor(colors[7]), clues[7]);
        Debug.LogFormat("[Nonogram #{0}] Clue for Row 4 is \"{2}\" ({1})", _moduleId, getFullColor(colors[8]), clues[8]);
        Debug.LogFormat("[Nonogram #{0}] Clue for Row 5 is \"{2}\" ({1})", _moduleId, getFullColor(colors[9]), clues[9]);
        Debug.LogFormat("[Nonogram #{0}] Generated solution is:\n{1}", _moduleId, solutionGrid);

        canInteract = true;
    }

    void onGridClick(int position) {
        if (!canInteract)
            return;

        onClearConfirm = false;
        clearButtonStatus.material = statusOff;

        if (currentGrid[position] == 0) {
            currentGrid[position] = isDotActive ? 1 : 2;
            gridButtonsDot[position].material = blackGridButton;
            gridButtonsFill[position].material = isDotActive ? whiteGridButton : blackGridButton;
            sound.PlaySoundAtTransform(currentGrid[position] == 1 ? "dot" : "fill", gridButtons[position].transform);
        } else if (currentGrid[position] == 1 && isDotActive || currentGrid[position] == 2 && !isDotActive) {
            currentGrid[position] = 0;
            gridButtonsDot[position].material = whiteGridButton;
            gridButtonsFill[position].material = whiteGridButton;
            sound.PlaySoundAtTransform("blank", gridButtons[position].transform);
        }
    }

    void onColorClick(int position) {
        if (!canInteract)
            return;

        onClearConfirm = false;
        clearButtonStatus.material = statusOff;

        int[] b = colAndRows[position];
        int[] g = currentGrid.ToArray();
        bool somethingHasChanged = false;

        int newState = g[b[0]] > 0 && g[b[1]] > 0 && g[b[2]] > 0 && g[b[3]] > 0 && g[b[4]] > 0 ? 0 : isDotActive ? 1 : 2;

        for (int i = 0; i < b.Length; i++) {
            if (g[b[i]] == 0 || g[b[i]] == (isDotActive ? 1 : 2)) {
                currentGrid[b[i]] = newState;
                gridButtonsDot[b[i]].material = newState > 0 ? blackGridButton : whiteGridButton;
                gridButtonsFill[b[i]].material = newState > 1 ? blackGridButton : whiteGridButton;
                somethingHasChanged = true;
            }
        }

        if (somethingHasChanged && newState == 0)
            sound.PlaySoundAtTransform("blank", gridButtons[position].transform);
        else if (somethingHasChanged && newState == 1)
            sound.PlaySoundAtTransform("dot", gridButtons[position].transform);
        else if (somethingHasChanged && newState == 2)
            sound.PlaySoundAtTransform("fill", gridButtons[position].transform);
    }

    bool onLightToggle() {
        sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, toggleButton.transform);
        toggleButton.AddInteractionPunch(0.25f);

        if (!canInteract)
            return false;

        onClearConfirm = false;
        clearButtonStatus.material = statusOff;

        for (int i = 0; i < colors.Count; i++)
            gridColors[i].material = getGridColor(colors[i], !isSecondaryColor);

        isSecondaryColor = !isSecondaryColor;

        return false;
    }

    bool onDotToggle() {
        sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, dotButton.transform);
        toggleButton.AddInteractionPunch(0.25f);

        if (!canInteract)
            return false;

        onClearConfirm = false;
        clearButtonStatus.material = statusOff;

        dotButtonStatus.material = isDotActive ? statusOff : statusOn;
        isDotActive = !isDotActive;

        return false;
    }

    bool onClear() {
        sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, dotButton.transform);
        toggleButton.AddInteractionPunch(0.25f);

        if (!canInteract)
            return false;

        clearButtonStatus.material = onClearConfirm ? statusOff : statusConfirm;
        onClearConfirm = !onClearConfirm;

        if (!onClearConfirm) {
            if (currentGrid.Contains(1) || currentGrid.Contains(2))
                sound.PlaySoundAtTransform("blank", gridButtons[12].transform);
            currentGrid = new List<int>(Enumerable.Repeat(0, 25).ToArray());
            for (int i = 0; i < gridButtons.Length; i++) {
                gridButtonsDot[i].material = whiteGridButton;
                gridButtonsFill[i].material = whiteGridButton;
            }
        }

        return false;
    }

    bool onSubmit() {
        sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, submitButton.transform);
        toggleButton.AddInteractionPunch();

        if (!canInteract)
            return false;

        onClearConfirm = false;
        clearButtonStatus.material = statusOff;

        int[] grid = currentGrid.ToArray();
        bool strike = false;

        for (int i = 0; i < colAndRows.Length; i++) {
            string currentHint = getClue(new bool[] {
                grid[colAndRows[i][0]] > 1,
                grid[colAndRows[i][1]] > 1,
                grid[colAndRows[i][2]] > 1,
                grid[colAndRows[i][3]] > 1,
                grid[colAndRows[i][4]] > 1
            });

            if (clues[i] != currentHint) {
                strike = true;
                break;
            }
        }

        string submittedGrid = string.Format(gridLog,
            logGrid(grid[0]), logGrid(grid[1]), logGrid(grid[2]), logGrid(grid[3]), logGrid(grid[4]),
            logGrid(grid[5]), logGrid(grid[6]), logGrid(grid[7]), logGrid(grid[8]), logGrid(grid[9]),
            logGrid(grid[10]), logGrid(grid[11]), logGrid(grid[12]), logGrid(grid[13]), logGrid(grid[14]),
            logGrid(grid[15]), logGrid(grid[16]), logGrid(grid[17]), logGrid(grid[18]), logGrid(grid[19]),
            logGrid(grid[20]), logGrid(grid[21]), logGrid(grid[22]), logGrid(grid[23]), logGrid(grid[24])
        );

        Debug.LogFormat("[Nonogram #{0}] Submitted {1} answer:\n{2}", _moduleId, strike ? "incorrect" : "correct", submittedGrid);

        StartCoroutine(submitModule(!strike));

        return !strike;
    }

    public string TwitchHelpMessage = "Switch colors with !{0} toggle. " +
        "Clear the grid with !{0} clear. " +
        "Fill the squares at A2, D5 and the entire column C with !{0} fill A2 C D5. " +
        "Mark a dot at the squares A5, E4 and the entire row 2 with !{0} dot A5 E4 2. " +
        "Submit your answer with !{0} submit.";

    IEnumerator ProcessTwitchCommand(string command) {
        command = command.ToUpperInvariant().Trim();
        string[] args = command.Split(' ');

        if (command == "TOGGLE") {
            onLightToggle();
            yield return null;
        }

        else if (command == "CLEAR") {
            onClear();
            onClear();
            yield return null;
        }

        else if (command == "SUBMIT")
            yield return onSubmit() ? "solve" : "strike";

        else if (args[0] == "FILL" || args[0] == "DOT") {
            for (int i = 0; i < args.Length; i++) {
                if (args[i] == "FILL" && isDotActive || args[i] == "DOT" && !isDotActive)
                    onDotToggle();
                else if (gridIds.Contains(args[i])) {
                    int index = Array.IndexOf(gridIds, args[i]);
                    onGridClick(index);
                } else if (colorIds.Contains(args[i])) {
                    int index = Array.IndexOf(colorIds, args[i]);
                    onColorClick(index);
                } else if (args[i] != "FILL" && args[i] != "DOT") {
                    yield return "sendtochat Argument #" + (i + 1) + " (\"" + args[i] + "\") is invalid, command process will not continue.";
                    yield break;
                }

                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}

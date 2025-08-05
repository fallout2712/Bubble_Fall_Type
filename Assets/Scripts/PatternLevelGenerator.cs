using System.Collections.Generic;
using UnityEngine;

public class PatternLevelGenerator
{
    // private CellColors[] allColors = new CellColors[]
    // {
    //     CellColors.Red,
    //     CellColors.Green,
    //     CellColors.Yellow,
    //     CellColors.Blue
    // };

    public int Width;
    public int Height;
    public int BlockSize = 3; // размер цветового блока
    public float noiseChance = 0.2f;

    public int AvailableSpritesCount = 4; // передай при создании из Ball.BubbleSprites.Length

    public PatternLevelGenerator(int width, int height, int availableSpritesCount)
    {
        Width = width;
        Height = height;
        AvailableSpritesCount = availableSpritesCount;
    }

    public int[,] GenerateGrid()
    {
        int[,] grid = new int[Width, Height];

        for (int y = 0; y < Height; y += BlockSize)
        {
            for (int x = 0; x < Width; x += BlockSize)
            {
                int blockSprite = Random.Range(0, AvailableSpritesCount);

                for (int by = 0; by < BlockSize; by++)
                {
                    for (int bx = 0; bx < BlockSize; bx++)
                    {
                        int gx = x + bx;
                        int gy = y + by;

                        if (gx >= Width || gy >= Height)
                            continue;

                        if (Random.value < noiseChance)
                            grid[gx, gy] = GetRandomDifferentSprite(blockSprite);
                        else
                            grid[gx, gy] = blockSprite;
                    }
                }
            }
        }

        return grid;
    }

    private int GetRandomDifferentSprite(int exclude)
    {
        int newSprite;
        do
        {
            newSprite = Random.Range(0, AvailableSpritesCount);
        }
        while (newSprite == exclude);
        return newSprite;
    }

    // public PatternLevelGenerator(int width, int height)
    // {
    //     Width = width;
    //     Height = height;
    // }


    // public CellColors[,] GenerateGrid()
    // {
    //     CellColors[,] grid = new CellColors[Width, Height];

    //     for (int y = 0; y < Height; y += BlockSize)
    //     {
    //         for (int x = 0; x < Width; x += BlockSize)
    //         {
    //             CellColors blockColor = allColors[Random.Range(0, allColors.Length)];

    //             for (int by = 0; by < BlockSize; by++)
    //             {
    //                 for (int bx = 0; bx < BlockSize; bx++)
    //                 {
    //                     int gx = x + bx;
    //                     int gy = y + by;

    //                     if (gx >= Width || gy >= Height)
    //                         continue;

    //                     if (Random.value < noiseChance)
    //                         grid[gx, gy] = GetRandomDifferentColor(blockColor);
    //                     else
    //                         grid[gx, gy] = blockColor;
    //                 }
    //             }
    //         }
    //     }

    //     return grid;
    // }

    // private CellColors GetRandomDifferentColor(CellColors exclude)
    // {
    //     List<CellColors> others = new List<CellColors>(allColors);
    //     others.Remove(exclude);
    //     return others[Random.Range(0, others.Count)];
    // }
}

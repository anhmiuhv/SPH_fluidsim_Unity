//   SPH Fluid Dynamics for Unity3d.
//
//   Modul:             Fluid physics
//
//   Description:       Implementation of a evenly spaced spatial grid, used for fast fluid simulation


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct grid
{
    public Vector2 pos;
    public float value;
}
public class Grid {
    private List<int>[] neightbors;
	public const int NeighbourCount = 8;
	private List<int>[][] m_grid ;
    private List<mParticle> m_p;
	public float CellSpace;
	public Rect Domain;
	public int Width;
	public int Height;
    public int GPUVoxel
    {
        get
        {
            return (Width * Constants.resolution + 1) * (Height * Constants.resolution + 1);
        }
    }

    public int Voxel
    {
        get
        {
            return (Width * 3 + 1) * (Height * 3 + 1);
        }
    }

    grid[] voxelArray;
	public int Count {
		get { return m_grid.Length; }
	}
	
	public Grid() {
		CellSpace = 8;
		Domain = new Rect(0, 0, 256, 256);
		initialize();
	}
	
	public Grid(float cellSpace, Rect domain) {
		CellSpace = cellSpace;
		Domain = domain;
		initialize();
        neightbors = new List<int>[Constants.MAX_PARTICLES];
        for (int i = 0; i < neightbors.Length; i++)
        {
            neightbors[i] = new List<int>();
        }
        List<mParticle> zerol = new List<mParticle>();
		Refresh(ref zerol);
		zerol = null;
	}
	
	public Grid(float cellSpace, Rect domain, ref List<mParticle> particles) {
		CellSpace       = cellSpace;
		Domain          = domain;
		initialize();
        neightbors = new List<int>[particles.Count];
        for (int i =0; i < neightbors.Length; i++)
        {
            neightbors[i] = new List<int>();
        }
        Refresh(ref particles);
	}
	
	private void initialize() {
		Width           = (int)Mathf.Ceil(Domain.width / this.CellSpace);
		Height          = (int)Mathf.Ceil(Domain.height / this.CellSpace);
		m_grid = new List<int>[Width][];
        voxelArray = new grid[Voxel];
        setUpArray(voxelArray);
    }

    public void Refresh(ref List<mParticle> particles)
    {
        m_p = particles;
        m_grid = null;
        m_grid = new List<int>[Width][];
        for (int i = 0; i < neightbors.Length; i++)
        {
            neightbors[i].Clear();
        }
        if (particles != null)
        {
            for (int i = 0; i < particles.Count; i++)
            {
                mParticle p = particles[i];
                int gridIndexX = GetGridIndexX(ref p);
                int gridIndexY = GetGridIndexY(ref p);
                p.grid_index = new Vector2(gridIndexX, gridIndexY);
                // Add particle to list
                if (m_grid[gridIndexX] == null)
                {
                    m_grid[gridIndexX] = new List<int>[Height];
                }
                if (m_grid[gridIndexX][gridIndexY] == null)
                {
                    m_grid[gridIndexX][gridIndexY] = new List<int>();
                }
                m_grid[gridIndexX][gridIndexY].Add(i);
            }
        }
        for (int i = 0; i < particles.Count; i++)
        {
            for (int xOff = -1; xOff < 2; xOff++)
            {
                for (int yOff = -1; yOff < 2; yOff++)
                {

                    int x = (int) particles[i].grid_index.x + xOff;
                    int y = (int) particles[i].grid_index.y + yOff;
                    // Clamp
                    if (x > -1 && x < this.Width && y > -1 && y < this.Height)
                    {
                        if (m_grid[x] != null)
                        {
                            List<int> idxList = m_grid[x][y];
                            if (idxList != null)
                            {
                                // Return neighbours index

                                neightbors[i].AddRange(idxList);
                                
                            }
                        }
                    }

                }
            }
        }
    }
	
	private int GetGridIndexX(ref mParticle particle) {
		
		int gridIndexX = (int)((particle.Position.x - Constants.SimulationDomain.xMin) / CellSpace);
		// Clamp X
		if (gridIndexX < 0)
		{
            gridIndexX = 0;
		}
		if (gridIndexX >= Width)
		{
            gridIndexX = Width - 1;
		}
		return gridIndexX;
	}

	private int GetGridIndexY(ref mParticle particle) {
		
		int gridIndexY = (int)((particle.Position.y- Constants.SimulationDomain.yMin) / CellSpace);
		// Clamp Y
		if (gridIndexY < 0)
		{
            gridIndexY = 0;
		}
		if (gridIndexY >= Height)
		{
            gridIndexY = Height - 1;
		}
		return gridIndexY;
	}
	
	public void GetNeighbourIndex(ref mParticle particle, out List<int> neighbours) {
		neighbours = null;
		int x_ori = GetGridIndexX(ref particle);
		int y_ori = GetGridIndexY(ref particle);
		for (int xOff = -1; xOff < 2; xOff++) {
            for (int yOff = -1; yOff < 2; yOff++) {
				// Own index
				// Neighbour index
				int x = x_ori + xOff;
				int y = y_ori + yOff;
				// Clamp
				if (x > -1 && x < this.Width && y > -1 && y < this.Height) {
					if (m_grid[x] != null) {
						List<int> idxList = m_grid[x][y];
						if (idxList != null) {
							// Return neighbours index

							neighbours.AddRange(idxList);
							return;
						}
					}
				}
            }
		}
	}
	
	public List<int> GetNeighbourIndex(mParticle particle, int index) {

        return neightbors[index];
	}

    private IEnumerable getAllParticleInGrid(int i, int j)
    {
        if (m_grid[i] != null && m_grid[i][j] != null)
        {
            foreach (int ind in m_grid[i][j])
            {
                yield return m_p[ind];
            }
        }
        
    }
	public int getParticleCountAt(int col, int row) {
		if (Time.realtimeSinceStartup < 10)
			return 0;
		if (m_grid [col] == null || m_grid [col] [row] == null)
			return 0;
		
		return m_grid [col] [row].Count;
	}

	public int[,] getFluidMapCount() {
		float r = Domain.xMax;
		float l = Domain.xMin;
		// Rectangle contains coordinates inverse on y
		float t = Domain.yMax;
		float b = Domain.yMin;
		float[,] map = new float[Width * 3+ 1, Height * 3+ 1];
		int[,] field = new int[Width * 3 + 1, Height * 3 + 1];
		for (int i = 0; i <= Width * 3; i++) {
			for (int j = 0; j <= Height * 3; j++) {
				foreach (mParticle p in m_p) {
					map [i, j] += Helper.ImplicitField (voxelArray[i + j * (Width * 3 + 1)].pos, p.Position);
				}
			}
		}

		for (int i = 0; i <= Width * 3; i++) {
			for (int j = 0; j <= Height * 3; j++) {
				if (map [i, j] > 0) {
					field [i, j] = 1;
				}
			}
		}

		return field;
	}

    public void setUpArray(grid[] gridArray)
    {
        float r = Domain.xMax;
        float l = Domain.xMin;
        // Rectangle contains coordinates inverse on y
        float t = Domain.yMax;
        float b = Domain.yMin;
        for (int i = 0; i <= Width * 3; i++)
        {
            for (int j = 0; j <= Height * 3; j++)
            {

                gridArray[i + j * (Width * 3 + 1)].pos = new Vector2(l + Constants.CellSpace * i / 3, b + Constants.CellSpace / 3 * j);
                gridArray[i + j * (Width * 3 + 1)].value = 0;

            }
        }
    }

    public void setUpGPUArray(grid[] gridArray)
    {
        float r = Domain.xMax;
        float l = Domain.xMin;
        // Rectangle contains coordinates inverse on y
        float t = Domain.yMax;
        float b = Domain.yMin;
        for (int i = 0; i <= Width * Constants.resolution; i++)
        {
            for (int j = 0; j <= Height * Constants.resolution; j++)
            {

				gridArray[i + j * (Width * Constants.resolution + 1)].pos = new Vector2(l + Constants.CellSpace * i / Constants.resolution, b + Constants.CellSpace / Constants.resolution * j);
                gridArray[i + j * (Width * Constants.resolution + 1)].value = 0;

            }
        }
    }



}


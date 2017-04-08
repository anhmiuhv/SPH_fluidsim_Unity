//   SPH Fluid Dynamics for Unity3d.//
//   Modul:             Fluid physics
//
//   Description:       Implementation of a evenly spaced spatial grid, used for fast fluid simulation


using System;
using System.Collections;
using UnityEngine;

public class IndexGrid {
	public const int NeighbourCount = 8;
	private ArrayList[][] m_grid;
	public float CellSpace;
	public Rect Domain;
	public int Width;
	public int Height;
	public int Count {
		get { return m_grid.Length; }
	}
	
	public IndexGrid() {
		CellSpace = 8;
		Domain = new Rect(0, 0, 256, 256);
		initialize();
	}
	
	public IndexGrid(float cellSpace, Rect domain) {
		CellSpace = cellSpace;
		Domain = domain;
		initialize();
		ArrayList zerol = new ArrayList();
		Refresh(ref zerol);
		zerol = null;
	}
	
	public IndexGrid(float cellSpace, Rect domain, ref ArrayList particles) {
		CellSpace       = cellSpace;
		Domain          = domain;
		initialize();
		Refresh(ref particles);
	}
	
	private void initialize() {
		Width           = (int)(Domain.width / this.CellSpace);
		Height          = (int)(Domain.height / this.CellSpace);
		m_grid = new ArrayList[Width][];
	}
	
	public void Refresh(ref ArrayList particles) {
		
		m_grid = null;
		m_grid = new ArrayList[Width][];
		
		if (particles != null) {
            for (int i = 0; i < particles.Count; i++) {
				FluidParticle p = (FluidParticle) particles[i];
				int gridIndexX = GetGridIndexX(ref p);
				int gridIndexY = GetGridIndexY(ref p);
				
				// Add particle to list
				if (m_grid[gridIndexX] == null) {
					m_grid[gridIndexX] = new ArrayList[Height];
				}
				if (m_grid[gridIndexX][gridIndexY] == null) {
					m_grid[gridIndexX][gridIndexY] = new ArrayList();
				}
				m_grid[gridIndexX][gridIndexY].Add(i);
            }
		}
	}
	
	private int GetGridIndexX(ref FluidParticle particle) {
		
		int gridIndexX = (int)(particle.Position.x / CellSpace);
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

	private int GetGridIndexY(ref FluidParticle particle) {
		
		int gridIndexY = (int)(particle.Position.y / CellSpace);
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
	
	public void GetNeighbourIndex(ref FluidParticle particle, out ArrayList neighbours) {
		neighbours = null;
		for (int xOff = -1; xOff < 2; xOff++) {
            for (int yOff = -1; yOff < 2; yOff++) {
				// Own index
				// Neighbour index
				int x = GetGridIndexX(ref particle) + xOff;
				int y = GetGridIndexY(ref particle) + yOff;
				// Clamp
				if (x > -1 && x < this.Width && y > -1 && y < this.Height) {
					if (m_grid[x] != null) {
						ArrayList idxList = m_grid[x][y];
						if (idxList != null) {
							// Return neighbours index
							neighbours = (ArrayList)idxList.Clone();
							return;
						}
					}
				}
            }
		}
	}
	
	public IEnumerable GetNeighbourIndex(FluidParticle particle) {
		for (int xOff = -1; xOff < 2; xOff++) {
            for (int yOff = -1; yOff < 2; yOff++) {
				// Own index
				// Neighbour index
				int x = GetGridIndexX(ref particle) + xOff;
				int y = GetGridIndexY(ref particle) + yOff;
				// Clamp
				if (x > -1 && x < this.Width && y > -1 && y < this.Height) {
					if (m_grid[x] != null) {
						if (m_grid[x][y] != null) {
							ArrayList idxList = (ArrayList) m_grid[x][y].Clone();
							if (idxList != null) {
								// Return neighbours index
								foreach (int idx in m_grid[x][y]) {
									yield return idx;
								}
								idxList.Clear();
							}
						}
					}
				}
            }
		}
	}
	
}


//   SPH Fluid Dynamics for Unity3d.
//
//   Modul:             Fluid physics
//
//   Description:       Implementation of a evenly spaced spatial grid, used for fast fluid simulation


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid {
	public const int NeighbourCount = 8;
	private List<int>[][] m_grid;
	public float CellSpace;
	public Rect Domain;
	public int Width;
	public int Height;
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
		List<mParticle> zerol = new List<mParticle>();
		Refresh(ref zerol);
		zerol = null;
	}
	
	public Grid(float cellSpace, Rect domain, ref List<mParticle> particles) {
		CellSpace       = cellSpace;
		Domain          = domain;
		initialize();
		Refresh(ref particles);
	}
	
	private void initialize() {
		Width           = (int)(Domain.width / this.CellSpace);
		Height          = (int)(Domain.height / this.CellSpace);
		m_grid = new List<int>[Width][];
	}
	
	public void Refresh(ref List<mParticle> particles) {
		
		m_grid = null;
		m_grid = new List<int>[Width][];
		
		if (particles != null) {
            for (int i = 0; i < particles.Count; i++) {
				mParticle p =  particles[i];
				int gridIndexX = GetGridIndexX(ref p);
				int gridIndexY = GetGridIndexY(ref p);

				// Add particle to list
				if (m_grid[gridIndexX] == null) {
					m_grid[gridIndexX] = new List<int>[Height];
				}
				if (m_grid[gridIndexX][gridIndexY] == null) {
					m_grid[gridIndexX][gridIndexY] = new List<int>();
				}
				m_grid[gridIndexX][gridIndexY].Add(i);
            }
		}
	}
	
	private int GetGridIndexX(ref mParticle particle) {
		
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

	private int GetGridIndexY(ref mParticle particle) {
		
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
	
	public void GetNeighbourIndex(ref mParticle particle, out List<int> neighbours) {
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
	
	public IEnumerable GetNeighbourIndex(mParticle particle) {
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
							List<int> idxList = new List<int> (m_grid[x][y]);
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


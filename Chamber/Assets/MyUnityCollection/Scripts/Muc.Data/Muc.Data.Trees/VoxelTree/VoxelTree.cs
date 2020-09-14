

namespace Muc.Data.Trees {

  using System;
  using System.Linq;
  using System.Collections.Generic;
  using UnityEngine;
  using System.Collections;

  public static class VoxelTree {
    /// <summary> Creates a 1*1*1 size VoxelTree (0 debth, 1 voxel) </summary>
    public static VoxelTree<T> Create1<T>() => new VoxelTree<T>(0);
    /// <summary> Creates a 2*2*2 size VoxelTree (1 debth, 8 maximum voxels) </summary>
    public static VoxelTree<T> Create2<T>() => new VoxelTree<T>(1);
    /// <summary> Creates a 4*4*4 size VoxelTree (2 debth, 64 maximum voxels) </summary>
    public static VoxelTree<T> Create4<T>() => new VoxelTree<T>(2);
    /// <summary> Creates an 8*8*8 size VoxelTree (3 debth, 512 maximum voxels) </summary>
    public static VoxelTree<T> Create8<T>() => new VoxelTree<T>(3);
    /// <summary> Creates a 16*16*16 size VoxelTree (4 debth, 4,096 maximum voxels) </summary>
    public static VoxelTree<T> Create16<T>() => new VoxelTree<T>(4);
    /// <summary> Creates a 32*32*32 size VoxelTree (5 debth, 32,768 maximum voxels) </summary>
    public static VoxelTree<T> Create32<T>() => new VoxelTree<T>(5);
    /// <summary> Creates a 64*64*64 size VoxelTree (6 debth, 262,144 maximum voxels) </summary>
    public static VoxelTree<T> Create64<T>() => new VoxelTree<T>(6);
    /// <summary> Creates a 128*128*128 size VoxelTree (7 debth, 2,097,152 maximum voxels) </summary>
    public static VoxelTree<T> Create128<T>() => new VoxelTree<T>(7);
    /// <summary> Creates a 256*256*256 size VoxelTree (8 debth, 16,777,216 maximum voxels) </summary> 
    public static VoxelTree<T> Create256<T>() => new VoxelTree<T>(8);
    /// <summary> Creates a 512*512*512 size VoxelTree (9 debth, 134,217,728 maximum voxels) </summary>
    public static VoxelTree<T> Create512<T>() => new VoxelTree<T>(9);
    /// <summary> Creates a 1024*1024*1024 size VoxelTree (10 debth, 1,073,741,824 maximum voxels) </summary>
    public static VoxelTree<T> Create1024<T>() => new VoxelTree<T>(10);
    /// <summary> Creates a 2048*2048*2048 size VoxelTree (11 debth, 8,589,934,592 maximum voxels) </summary>
    public static VoxelTree<T> Create2048<T>() => new VoxelTree<T>(11);
    /// <summary> Creates a 4096*4096*4096 size VoxelTree (12 debth, 68,719,476,736 maximum voxels) </summary>
    public static VoxelTree<T> Create4096<T>() => new VoxelTree<T>(12);
    /// <summary> Creates an 8192*8192*8192 size VoxelTree (13 debth, 549,755,813,888 maximum voxels) </summary>
    public static VoxelTree<T> Create8192<T>() => new VoxelTree<T>(13);
  }

  public partial class VoxelTree<T> : VoxCell<T>, ITree<T> {

    public int debth { get; }
    public int length { get; }
    public ulong maxTotal { get; }
    public bool dataIsNullable { get; }

    ITreeEnumerator<ICell> ITree.GetEnumerator() => (ITreeEnumerator<ICell>)GetEnumerator();
    ITreeEnumerator<ICell<T>> ITree<T>.GetEnumerator() => (ITreeEnumerator<ICell<T>>)GetEnumerator();

    public T this[int x, int y, int z] {
      get => Get(x, y, z);
      set => Set(x, y, z, value);
    }


    public VoxelTree(int debth) {
      if (debth < 0) throw new ArgumentOutOfRangeException(nameof(debth), $"Argument {nameof(debth)} must be non-negative.");
      if (debth >= 31) throw new ArgumentOutOfRangeException(nameof(debth), $"Argument {nameof(debth)} must not be more than 31.");
      this.debth = debth;
      this.length = (int)Mathf.Pow(2, debth);
      this.maxTotal = (ulong)this.length * (ulong)this.length * (ulong)this.length;
      this.dataIsNullable = typeof(T).IsClass;
      Debug.Log($"length = {this.length}");
      Debug.Log($"maxTotal = {this.maxTotal}");
    }

    private T Get(int x, int y, int z) {
      if (Math.Max(Math.Max(x, y), z) >= length) throw OobError();

      var e = this.GetEnumerator();
      var currentLength = length;
      while (currentLength > 1) {
        var signs = Vector3Int.zero;
        currentLength /= 2;
        if (currentLength <= x) { x -= currentLength; signs.x = 1; }
        if (currentLength <= y) { y -= currentLength; signs.y = 1; }
        if (currentLength <= z) { z -= currentLength; signs.z = 1; }
        var index = Octree.SignsToIndex(signs);
        if (!e.MoveDown(index)) return default;
      }
      return e.Current.data;
    }

    private void Set(int x, int y, int z, T value) {
      if (Math.Max(Math.Max(x, y), z) >= length) throw OobError();

      var e = this.GetEnumerator();
      var currentLength = length;
      while (currentLength > 1) {
        var signs = Vector3Int.zero;
        currentLength /= 2;
        if (currentLength <= x) { x -= currentLength; signs.x = 1; }
        if (currentLength <= y) { y -= currentLength; signs.y = 1; }
        if (currentLength <= z) { z -= currentLength; signs.z = 1; }
        var index = Octree.SignsToIndex(signs);
        e.Current.Split();
        e.MoveDown(index);
      }
      e.Current.data = value;
    }

    private IndexOutOfRangeException OobError(string x = "x", string y = "y", string z = "z")
      => new IndexOutOfRangeException($"One or more of the index arguments {x}, {y} or {z} was outside the bounds of the {nameof(VoxelTree)}");

  }
}
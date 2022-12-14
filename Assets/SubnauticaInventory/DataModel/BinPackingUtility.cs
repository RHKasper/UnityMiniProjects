using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SubnauticaInventory.DataModel
{
	public static class BinPackingUtility
	{
		public static bool ItemsFitInBin(List<ItemData> items, int binWidth, int binHeight)
		{
			return GenerateBinPack(items, binWidth, binHeight) != null;
		}
		
		/// <summary>
		/// Evaluates whether two items in different inventories can be swapped
		/// </summary>
		public static bool ItemsCanSwap(Inventory inv1, ItemData item1, Inventory inv2, ItemData item2)
		{
				List<ItemData> newInv1 = inv1.Items.Where(i => i != item1).ToList();
				newInv1.Add(item2);
			
				List<ItemData> newInv2 = inv2.Items.Where(i => i != item2).ToList();
				newInv2.Add(item1);

				return ItemsFitInBin(newInv1, inv1.Width, inv1.Height) && ItemsFitInBin(newInv2, inv2.Width, inv2.Height);
		}

		/// <summary>
		/// Generates a valid bin pack for the given parameters, if there is one.
		/// Source: https://codeincomplete.com/articles/bin-packing/
		/// </summary>
		/// <returns> null if there is no valid pack, otherwise returns a valid pack for the given <see cref="items"/>
		/// into a bin with a given <see cref="binWidth"/> and <see cref="binHeight"/></returns>
		public static Dictionary<ItemData, Vector2Int> GenerateBinPack(List<ItemData> items, int binWidth, int binHeight)
		{
			// Initialize Dictionary
			Dictionary<ItemData, Vector2Int> binPack = new(); 

			// Sort by height, using width as a tiebreaker, then sorting objects with the same shape/size alphabetically
			items.Sort(delegate(ItemData a, ItemData b)
			{
				if (a.height != b.height) return b.height.CompareTo(a.height);
				if (a.width != b.width) return b.width.CompareTo(a.width);
				if (a.name != b.name) return String.Compare(a.name, b.name, StringComparison.Ordinal);
				
				return a.InstanceID.CompareTo(b.InstanceID);
			});

			// Track open spaces as rects and start with the whole bin open
			LinkedList<IntRect> openSpaces = new();
			openSpaces.AddFirst(new IntRect(0, 0, binWidth, binHeight));

			// Iterate through items and pack each in the first free space that can fit it, starting with top-most spaces.
			foreach (var itemData in items)
			{
				if(itemData.width > itemData.height)
					Debug.LogError("Items with width greater than height are not supported.");
				
				// Check right spaces first so rows can be completed
				foreach (IntRect openSpace in openSpaces)
				{
					if (openSpace.CanFitItem(itemData))
					{
						binPack[itemData] = new Vector2Int(openSpace.X, openSpace.Y);
						
						// Remove this space and replace it with resulting spaces.
						openSpace.SplitAroundItem(itemData, out IntRect? right, out IntRect? below);
						openSpaces.Remove(openSpace);

						if (right.HasValue)
							AddOpenSpace(right.Value, openSpaces);
						if (below.HasValue)
							AddOpenSpace(below.Value, openSpaces);
						break;
					}
				}
				
				// If no spot is found for an item, this is not a valid pack.
				if (!binPack.ContainsKey(itemData))
					return null;
			}

			return binPack;
		}

		/// <summary>
		/// Adds an open space such that <see cref="openSpaces"/> remains sorted from least to greatest Y position
		/// todo: make this work when we add an item with smaller y than any current element.
		/// </summary>
		public static void AddOpenSpace(IntRect spaceToAdd, LinkedList<IntRect> openSpaces)
		{
			if (openSpaces.Count == 0 || openSpaces.Last.Value.Y <= spaceToAdd.Y)
				openSpaces.AddLast(spaceToAdd);
			else if (openSpaces.First.Value.Y >= spaceToAdd.Y)
				openSpaces.AddFirst(spaceToAdd);
			else
			{
				LinkedListNode<IntRect> target = openSpaces.Last;

				// check the previous one to see if it's less than space to add
				while (target.Previous!.Value.Y >= spaceToAdd.Y)
					target = target.Previous;

				openSpaces.AddBefore(target, spaceToAdd);
			}
		}

		
	}
}
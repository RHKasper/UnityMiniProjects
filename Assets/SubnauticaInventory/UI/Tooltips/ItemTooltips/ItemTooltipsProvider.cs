﻿using UnityEngine.EventSystems;

namespace SubnauticaInventory.UI.Tooltips.ItemTooltips
{
	public class ItemTooltipsProvider : AbstractTooltipsProvider<ItemTooltipController, ItemViewController>
	{
		public void UpdatePosition(ItemViewController itemView, PointerEventData eventData)
		{
			if(ActiveTooltips.ContainsKey(itemView))
				ActiveTooltips[itemView].UpdatePositionInternal(eventData);
		}
	}
}
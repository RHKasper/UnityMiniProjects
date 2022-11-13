﻿using UnityEngine.EventSystems;

namespace SubnauticaInventory.UI.Tooltips.ItemTooltips
{
	public class ItemTooltipProvider : AbstractTooltipsProvider<ItemTooltipController, ItemViewController>
	{
		public void UpdatePosition(ItemViewController itemView, PointerEventData eventData)
		{
			if(HasActive(itemView))
				ActiveTooltips[itemView].UpdatePositionInternal(eventData);
		}
	}
}
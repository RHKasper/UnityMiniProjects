﻿using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SubnauticaInventory.UI.Tooltips
{
	public class ItemTooltipController : SimpleUiBehavior
	{
		[Header("Settings")]
		[SerializeField] private Vector2 offsetFromPointer = new(24,-30);
		
		[Header("Internal References")]
		[SerializeField] private TextMeshProUGUI nameText;
		[SerializeField] private TextMeshProUGUI descriptionText;

		private ItemViewController _itemView;

		public void ShowInternal(ItemViewController itemView)
		{
			_itemView = itemView;
			
			nameText.text = itemView.ItemData.name;
			descriptionText.text = itemView.ItemData.description;
			
			transform.SetParent(itemView.PdaOverlayCanvas);
			transform.localScale = Vector3.one;

			gameObject.SetActive(true);
		}

		public void UpdatePositionInternal(PointerEventData eventData)
		{
			var raycastHit = eventData.pointerCurrentRaycast;
			RectTransform.anchoredPosition = offsetFromPointer + (Vector2) _itemView.PdaOverlayCanvas.InverseTransformPoint(raycastHit.worldPosition);
		}
	}
}
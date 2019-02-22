using System;
using Android.Content;
using Android.Support.V7.Widget;
using Object = Java.Lang.Object;

namespace Xamarin.Forms.Platform.Android
{
	public class SelectableItemsViewAdapter : ItemsViewAdapter
	{
		protected readonly SelectableItemsView SelectableItemsView;

		internal SelectableItemsViewAdapter(SelectableItemsView selectableItemsView,
			Func<IVisualElementRenderer, Context, global::Android.Views.View> createView = null) : base(selectableItemsView, createView)
		{
			SelectableItemsView = selectableItemsView;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			base.OnBindViewHolder(holder, position);

			if (!(holder is SelectableViewHolder selectable))
			{
				return;
			}

			// Watch for clicks so the user can select the item held by this ViewHolder
			selectable.Clicked += SelectableOnClicked;

			// Make sure that if this item is one of the selected items, it's marked as selected
			selectable.IsSelected = PostionIsSelected(position);
		}

		public override void OnViewRecycled(Object holder)
		{
			if (holder is SelectableViewHolder selectable)
			{
				selectable.Clicked -= SelectableOnClicked;
				selectable.IsSelected = false;
			}

			base.OnViewRecycled(holder);
		}

		int[] GetSelectedPositions()
		{
			switch (SelectableItemsView.SelectionMode)
			{
				case SelectionMode.None:
					return new int[0];

				case SelectionMode.Single:
					var selectedItem = SelectableItemsView.SelectedItem;
					if (selectedItem == null)
					{
						return new int[0];
					}

					return new int[1] { GetPositionForItem(selectedItem) };

				case SelectionMode.Multiple:
					var selectedItems = SelectableItemsView.SelectedItems;
					var result = new int[selectedItems.Count];

					for (int n = 0; n < result.Length; n++)
					{
						result[n] = GetPositionForItem(selectedItems[n]);
					}

					return result;
			}

			return new int[0];
		}

		bool PostionIsSelected(int position)
		{
			foreach (var selectedPosition in GetSelectedPositions())
			{
				if (selectedPosition == position)
				{
					return true;
				}
			}

			return false;
		}

		void SelectableOnClicked(object sender, int adapterPosition)
		{
			UpdateFormsSelection(adapterPosition);
		}

		void UpdateFormsSelection(int adapterPosition)
		{
			var mode = SelectableItemsView.SelectionMode;

			switch (mode)
			{
				case SelectionMode.None:
					// Selection's not even on, so there's nothing to do here
					return;
				case SelectionMode.Single:
					SelectableItemsView.SelectedItem = ItemsSource[adapterPosition];
					return;
				case SelectionMode.Multiple:
					var item = ItemsSource[adapterPosition];
					var selectedItems = SelectableItemsView.SelectedItems;

					if (selectedItems.Contains(item))
					{
						selectedItems.Remove(item);
					}
					else
					{
						selectedItems.Add(item);
					}
					return;
			}
		}
	}
}
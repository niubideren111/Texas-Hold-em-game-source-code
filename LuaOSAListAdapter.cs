/*
 * * * * This bare-bones script was auto-generated * * * *
 * The code commented with "/ * * /" demonstrates how data is retrieved and passed to the adapter, plus other common commands. You can remove/replace it once you've got the idea
 * Complete it according to your specific use-case
 * Consult the Example scripts if you get stuck, as they provide solutions to most common scenarios
 * 
 * Main terms to understand:
 *		Model = class that contains the data associated with an item (title, content, icon etc.)
 *		Views Holder = class that contains references to your views (Text, Image, MonoBehavior, etc.)
 * 
 * Default expected UI hiererchy:
 *	  ...
 *		-Canvas
 *		  ...
 *			-MyScrollViewAdapter
 *				-Viewport
 *					-Content
 *				-Scrollbar (Optional)
 *				-ItemPrefab (Optional)
 * 
 * Note: If using Visual Studio and opening generated scripts for the first time, sometimes Intellisense (autocompletion)
 * won't work. This is a well-known bug and the solution is here: https://developercommunity.visualstudio.com/content/problem/130597/unity-intellisense-not-working-after-creating-new-1.html (or google "unity intellisense not working new script")
 * 
 * 
 * Please read the manual under "Assets/OSA/Docs", as it contains everything you need to know in order to get started, including FAQ
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using frame8.Logic.Misc.Other.Extensions;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomParams;
using Com.TheFallenGames.OSA.DataHelpers;
using XLua;

// The date was temporarily included in the namespace to prevent duplicate class names
// You should modify the namespace to your own or - if you're sure there will be no conflicts - remove it altogether
namespace Runtime
{
	// There are 2 important callbacks you need to implement: CreateViewsHolder() and UpdateViewsHolder()
	// See explanations below
	public class LuaOSAListAdapter : OSA<BaseParamsWithPrefab, LuaListItemViewsHolder>
	{
		// Helper that stores data and notifies the adapter when items count changes
		// Can be iterated and can also have its elements accessed by the [] operator
		SimpleDataHelper<LuaListItemModel> _Data = null;
		public SimpleDataHelper<LuaListItemModel> Data { 
			get { 
				if (_Data == null) {
					_Data = new SimpleDataHelper<LuaListItemModel>(this);
				}
				return _Data;
		}}


		#region OSA implementation
		protected override void Start()
		{
			// Data = new SimpleDataHelper<LuaListItemModel>(this);
  
			// Calling this initializes internal data and prepares the adapter to handle item count changes
			base.Start();

			// Retrieve the models from your data source and set the items count
			/*
			RetrieveDataAndUpdate(500);
			*/
			// RetrieveDataAndUpdate(500);
		}

		// This is called initially, as many times as needed to fill the viewport, 
		// and anytime the viewport's size grows, thus allowing more items to be displayed
		// Here you create the "ViewsHolder" instance whose views will be re-used
		// *For the method's full description check the base implementation
		protected override LuaListItemViewsHolder CreateViewsHolder(int itemIndex)
		{
			var instance = new LuaListItemViewsHolder();

			// Using this shortcut spares you from:
			// - instantiating the prefab yourself
			// - enabling the instance game object
			// - setting its index 
			// - calling its CollectViews()
			var itemPrefab = _Params.ItemPrefab;
			if (luaAdapterDelegate != null) {
				OSAListCreateViewsHolder createHolder = luaAdapterDelegate.Get<OSAListCreateViewsHolder>("osaListCreateViewsHolder");
				if (createHolder != null) {
					var prefab = createHolder(luaAdapterDelegate, itemIndex);
					if (prefab != null) {
						itemPrefab = prefab.GetComponent<RectTransform>();
					}
				}
			}

			instance.Init(itemPrefab, _Params.Content, itemIndex);

			return instance;
		}

		// This is called anytime a previously invisible item become visible, or after it's created, 
		// or when anything that requires a refresh happens
		// Here you bind the data from the model to the item's views
		// *For the method's full description check the base implementation
		protected override void UpdateViewsHolder(LuaListItemViewsHolder newOrRecycled)
		{
			// In this callback, "newOrRecycled.ItemIndex" is guaranteed to always reflect the
			// index of item that should be represented by this views holder. You'll use this index
			// to retrieve the model from your data set
			/*
			LuaListItemModel model = Data[newOrRecycled.ItemIndex];

			newOrRecycled.backgroundImage.color = model.color;
			newOrRecycled.titleText.text = model.title + " #" + newOrRecycled.ItemIndex;
			*/
			LuaListItemModel model = Data[newOrRecycled.ItemIndex];
			
			if (luaAdapterDelegate != null) {
				OSAListUpdateViewsHolder updateHolder = luaAdapterDelegate.Get<OSAListUpdateViewsHolder>("osaListUpdateViewsHolder");
				if (updateHolder != null) {
					updateHolder(luaAdapterDelegate, newOrRecycled, model.luaData);
				}
			}
			
			if (model.HasPendingVisualSizeChange) {
				newOrRecycled.MarkForRebuild();
				ScheduleComputeVisibilityTwinPass(true);
			}
		}

		/// <summary>
		/// Item高度变化后回调，仅在垂直滚动时有效
		/// </summary>
		/// <param name="vh"></param>
		protected override void OnItemHeightChangedPreTwinPass(LuaListItemViewsHolder vh)
		{
			base.OnItemHeightChangedPreTwinPass(vh);
			Data[vh.ItemIndex].HasPendingVisualSizeChange = false;
			//vh.ContentFitPending = false;
			//vh.ContentSizeFitter.enabled = false;
		}

		/// <summary>
		/// Item宽度变化后回调，仅在水平滚动时有效
		/// </summary>
		/// <param name="vh"></param>
		protected override void OnItemWidthChangedPreTwinPass(LuaListItemViewsHolder vh) { 
			base.OnItemHeightChangedPreTwinPass(vh);
			Data[vh.ItemIndex].HasPendingVisualSizeChange = false;
		}

		/// <summary>
		/// ScrollView尺寸发生变化时调用。
		/// </summary>
		protected override void RebuildLayoutDueToScrollViewSizeChange()
		{
			foreach (var model in Data)
				model.HasPendingVisualSizeChange = true;

			base.RebuildLayoutDueToScrollViewSizeChange();
		}

		// This is the best place to clear an item's views in order to prepare it from being recycled, but this is not always needed, 
		// especially if the views' values are being overwritten anyway. Instead, this can be used to, for example, cancel an image 
		// download request, if it's still in progress when the item goes out of the viewport.
		// <newItemIndex> will be non-negative if this item will be recycled as opposed to just being disabled
		// *For the method's full description check the base implementation

		protected override void OnBeforeRecycleOrDisableViewsHolder(LuaListItemViewsHolder inRecycleBinOrVisible, int newItemIndex)
		{
			base.OnBeforeRecycleOrDisableViewsHolder(inRecycleBinOrVisible, newItemIndex);
			inRecycleBinOrVisible.Clear();
		}

		// You only need to care about this if changing the item count by other means than ResetItems, 
		// case in which the existing items will not be re-created, but only their indices will change.
		// Even if you do this, you may still not need it if your item's views don't depend on the physical position 
		// in the content, but they depend exclusively to the data inside the model (this is the most common scenario).
		// In this particular case, we want the item's index to be displayed and also to not be stored inside the model,
		// so we update its title when its index changes. At this point, the Data list is already updated and 
		// shiftedViewsHolder.ItemIndex was correctly shifted so you can use it to retrieve the associated model
		// Also check the base implementation for complementary info
		/*
		protected override void OnItemIndexChangedDueInsertOrRemove(LuaListItemViewsHolder shiftedViewsHolder, int oldIndex, bool wasInsert, int removeOrInsertIndex)
		{
			base.OnItemIndexChangedDueInsertOrRemove(shiftedViewsHolder, oldIndex, wasInsert, removeOrInsertIndex);

			shiftedViewsHolder.titleText.text = Data[shiftedViewsHolder.ItemIndex].title + " #" + shiftedViewsHolder.ItemIndex;
		}
		*/
		#endregion

		// These are common data manipulation methods
		// The list containing the models is managed by you. The adapter only manages the items' sizes and the count
		// The adapter needs to be notified of any change that occurs in the data list. Methods for each
		// case are provided: Refresh, ResetItems, InsertItems, RemoveItems
		#region data manipulation
		// Lua数据
		public void InsertItemsAt(int index, LuaTable[] datas)
		{
			List<LuaListItemModel> models = new List<LuaListItemModel>();
			for (int i = 0; i < datas.Length; ++i) {
				models.Add(new LuaListItemModel{ luaData = datas[i] });
			}
			Data.InsertItems(index, models);
		}

		public void AppendItems(LuaTable[] datas, bool freezeEndEdge = false)
		{
			List<LuaListItemModel> models = new List<LuaListItemModel>();
			for (int i = 0; i < datas.Length; ++i) {
				models.Add(new LuaListItemModel{ luaData = datas[i] });
			}
			AppendItems(models.ToArray(), freezeEndEdge);
		}

		public void AppendItems(LuaListItemModel[] newItems, bool freezeEndEdge = false)
		{
			if (_Params.effects.LoopItems)
			{
				Data.ResetItems(newItems, freezeEndEdge);
			}
			else
			{
				Data.InsertItemsAtEnd(newItems, freezeEndEdge);
			}
		
		}

		public void RemoveItemsFrom(int index, int count = 1)
		{
			// Commented: the below 2 lines exemplify how you can use a plain list to manage the data, instead of a DataHelper, in case you need full control
			//YourList.RemoveRange(index, count);
			//RemoveItems(index, count);
			Data.RemoveItems(index, count);
		}

		/// <summary>
		/// 获取数据数量
		/// </summary>
		/// <value>当前数量</value>
		public int Count { get { return Data.Count; } }

		public void ResetItems(LuaTable[] datas)
		{
			// Commented: the below 3 lines exemplify how you can use a plain list to manage the data, instead of a DataHelper, in case you need full control
			//YourList.Clear();
			//YourList.AddRange(items);
			//ResetItems(YourList.Count);
			
			var newItems = new List<LuaListItemModel>();
			for (int i = 0; i < datas.Length; ++i) {
				var model = new LuaListItemModel();
				model.luaData = datas[i];
				newItems.Add(model);
			}
			Data.ResetItems(newItems);
		}

		public LuaListItemModel[] GetDatas()
		{
			return Data.List.ToArray();
		}
		#endregion


		// Here, we're requesting <count> items from the data source
		public void RetrieveDataAndUpdate(int count = 0)
		{
			// 不可见时，无需进行提取资源
			if (!gameObject.activeSelf) return;
			StartCoroutine(FetchMoreItemsFromDataSourceAndUpdate(count));
		}

		// Retrieving <count> models from the data source and calling OnDataRetrieved after.
		// In a real case scenario, you'd query your server, your database or whatever is your data source and call OnDataRetrieved after
		IEnumerator FetchMoreItemsFromDataSourceAndUpdate(int count)
		{
			// Simulating data retrieving delay
			yield return new WaitForSeconds(.2f);

			if (luaAdapterDelegate != null) {
				OSAListRetrieveDatas datasFunc = luaAdapterDelegate.Get<OSAListRetrieveDatas>("osaListRetrieveDatas");
				if (datasFunc != null) {
					LuaTable[] datas = datasFunc(luaAdapterDelegate, count);
					var newItems = new LuaListItemModel[datas.Length];
					for (int i = 0; i < datas.Length; ++i)
					{
						var model = new LuaListItemModel();
						model.luaData = datas[i];
						newItems[i] = model;
					}
					AppendItems(newItems);
				}
			}
		}

		/// <summary>
		/// 跳到指定ItemIndex位置上
		/// </summary>
		/// <param name="itemIndex"></param>
		/// <param name="normalizedOffsetFromViewportStart"></param>
		/// <param name="normalizedPositionOfItemPivotToUse"></param>
		public void SkipTo(int itemIndex, float normalizedOffsetFromViewportStart = 0f, float normalizedPositionOfItemPivotToUse = 0f) 
		{
			ScrollTo(itemIndex, normalizedOffsetFromViewportStart, normalizedPositionOfItemPivotToUse);
		}

		/// <summary>
		/// 滚动到指定Item位置上
		/// </summary>
		/// <param name="itemIndex">item索引</param>
		/// <param name="duration">动画表现时间(default=0.5)</param>
		public void ScrollTo(int itemIndex, float duration = 0.5f)
		{
			SmoothScrollTo(itemIndex, duration);
		}

		/// <summary>
		/// 滚到到最后一个元素
		/// </summary>
		/// <param name="duration">动画表现时间(default=0.5)</param>
		public void ScrollToLast(float duration = 0.5f)
		{
			SmoothScrollTo(Data.Count-1, duration);
		}

		/// <summary>
		/// 滚到到第一个元素
		/// </summary>
		/// <param name="duration">动画表现时间(default=0.5)</param>
		public void ScrollToFirst(float duration = 0.5f)
		{
			SmoothScrollTo(0, duration);
		}

		#region Lua
		public delegate LuaTable[] OSAListRetrieveDatas(LuaTable self, int count);
		public delegate LuaTable[] OSAListUpdateViewsHolder(LuaTable self, LuaListItemViewsHolder newOrRecycled, LuaTable data);
		public delegate GameObject OSAListCreateViewsHolder(LuaTable self, int itemIndex);
 		LuaTable luaAdapterDelegate = null;
		public void SetLuaDelegate(LuaTable luaDelegate)
		{
			luaAdapterDelegate = luaDelegate;
		}
		#endregion

		private new void OnDestroy() {
			base.OnDestroy();
			luaAdapterDelegate = null;
		}
	}

	// Class containing the data associated with an item
	public class LuaListItemModel
	{
		/*
		public string title;
		public Color color;
		*/
		public XLua.LuaTable luaData;
		public bool HasPendingVisualSizeChange = true;
	}


	// This class keeps references to an item's views.
	// Your views holder should extend BaseItemViewsHolder for ListViews and CellViewsHolder for GridViews
	public class LuaListItemViewsHolder : BaseItemViewsHolder
	{
		/*
		public Text titleText;
		public Image backgroundImage;
		*/

		// Rebuild的事件
		public delegate void OSAItemMarkForRebuild(LuaTable self);
		public delegate void OSAItemClear(LuaTable self);
		public delegate void OSAItemBeforeDestroy(LuaTable self);

		/// <summary>
		/// Lua端包装LuaListItemViewsHolder对象
		/// 用于ui元素缓存和更新
		/// 防止频繁去GetComponent<>
		/// </summary>
		public LuaTable luaItemWarpper = null;

		// Retrieving the views from the item's root GameObject
		public override void CollectViews()
		{
			base.CollectViews();

			// GetComponentAtPath is a handy extension method from frame8.Logic.Misc.Other.Extensions
			// which infers the variable's component from its type, so you won't need to specify it yourself
			/*
			root.GetComponentAtPath("TitleText", out titleText);
			root.GetComponentAtPath("BackgroundImage", out backgroundImage);
			*/
		}

		// Override this if you have children layout groups. They need to be marked for rebuild when this callback is fired
		
		public override void MarkForRebuild()
		{
			base.MarkForRebuild();

			if (luaItemWarpper != null) {
				OSAItemMarkForRebuild cb = luaItemWarpper.Get<OSAItemMarkForRebuild>("onMarkForRebuild");
				if (cb != null) {
					cb(luaItemWarpper);
				}
			}
		}

		public void Clear() {
			if (luaItemWarpper != null) {
				OSAItemClear cb = luaItemWarpper.Get<OSAItemClear>("onClear");
				if (cb != null) {
					cb(luaItemWarpper);
				}
			}
		}

		public override void OnBeforeDestroy() {
			if (luaItemWarpper != null) {
				OSAItemBeforeDestroy cb = luaItemWarpper.Get<OSAItemBeforeDestroy>("onBeforeDestroy");
				if (cb != null) {
					cb(luaItemWarpper);
				}
			}
		}
	}
}

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

using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Com.TheFallenGames.OSA.CustomAdapters.TableView;
using Com.TheFallenGames.OSA.CustomAdapters.TableView.Basic;
using Com.TheFallenGames.OSA.Core;
using Com.TheFallenGames.OSA.CustomAdapters.TableView.Extra;
using Com.TheFallenGames.OSA.CustomAdapters.TableView.Tuple;

// You should modify the namespace to your own or - if you're sure there won't ever be conflicts - remove it altogether
namespace Runtime
{
	/// <summary>
	/// Template demonstrating the use of a <see cref="TableAdapter{TParams, TTupleViewsHolder, THeaderTupleViewsHolder}"/>.
	/// </summary>
	public class LuaOSATableAdapter : TableAdapter<TableParams, LuaOSATupleViewsHolder, TupleViewsHolder>
	{
		List<string> _Indexes = new List<string>();

		protected override void Start()
		{
			base.Start();
		}

		public void ReloadData(List<IColumnInfo> headers, ITuple[] tuples, List<string> indexes, bool columnSortingSupported = false)
		{
			Columns = new BasicTableColumns(headers);
			Tuples = new BasicTableData(Columns, tuples, columnSortingSupported);
			_Indexes = indexes;
			ResetTableWithCurrentData();
		}

		protected override void UpdateViewsHolder(LuaOSATupleViewsHolder newOrRecycled)
		{
			var tuple = Tuples.GetTuple(newOrRecycled.ItemIndex);
			newOrRecycled.UpdateViews(tuple, Columns);

			if (_Indexes.Count > 0 && newOrRecycled.ItemIndex < _Indexes.Count) {
				newOrRecycled.UpdateIndex(_Indexes[newOrRecycled.ItemIndex]);
			}
		}
	}

	public class LuaOSATableHelper
	{
		public static BasicTuple CreateTupleWithEmptyValues(int length)
		{
			return TableViewUtil.CreateTupleWithEmptyValues<BasicTuple>(length);
		}
	}

	public class LuaOSATupleViewsHolder : TupleViewsHolder
	{
		public ITupleAdapter Adapter { get; private set; }

		public override void CollectViews()
		{
			base.CollectViews();
		}

		public virtual void UpdateViews(ITuple tuple, ITableColumns columns)
		{
			base.UpdateViews(tuple, columns);
		}

		public void UpdateIndex(string newIndex)
		{
			if (_IndexText != null) {
				_IndexText.text = newIndex;
			}
		}
	}

	public class LuaOSATableHeader : IColumnInfo
	{
		public string Name
		{
			get { return _Name; }
			set
			{
				if (_Name == value)
					return;

				_Name = value;
				ReconstructDisplayName();
			}
		}
		public string DisplayName { get; set; }
		public TableValueType ValueType { get; private set; }
		public Type EnumValueType { get; private set; }
		string _Name;

		public LuaOSATableHeader(string name, TableValueType valueType, Type enumValueType = null)
		{
			ValueType = valueType;
			EnumValueType = enumValueType;

			// Setting it last, so the display name will be reconstructed using the other properties
			Name = name;
		}

		void ReconstructDisplayName()
		{
			DisplayName = _Name;
		}
	}
}
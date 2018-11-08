using System;
using System.Collections;
using System.Collections.Generic;
using Rotorz.ReorderableList;
using UnityEngine;
using UnityEditor;

namespace DigitalSky.Tracker
{
    public class RecordPanelItem
    {
        /// <summary>
        /// the story node data reference
        /// </summary>
        public GameObject recordGameObject;
        /// <summary>
        /// the node rect in story panel
        /// </summary>
        public Rect itemRect;

        public RecordPanelItem(GameObject obj)
        {
            this.recordGameObject = obj;
        }
    }

    public class RecordPanelViewList
    {
        // panelItem UI对象列表
        private List<RecordPanelItem> _panelItemList;
        public List<RecordPanelItem> panelItemList
        {
            get { return _panelItemList; }
        }

        // recordList数据源列表
        private List<GameObject> _recordList;
        public List<GameObject> recordList
        {
            get { return _recordList; }
        }

        public RecordPanelViewList(List<GameObject> recordList)
        {
            _recordList = recordList;
            _panelItemList = new List<RecordPanelItem>();
            for (int i = 0; i < _recordList.Count; i++)
            {
                Add(_recordList[i]);
            }
        }

        public void Add(GameObject obj)
        {
            if(obj != null)
            {
                // 添加对应的panelItem UI对象
                RecordPanelItem panelItem = new RecordPanelItem(obj);
                _panelItemList.Add(panelItem);
            }
        }

        public void Remove(int index)
        {
            RecordPanelItem item = _panelItemList[index];

            // 移除对应的panelItem UI对象
            _panelItemList.Remove(item);
        }

        public void Clear()
        {
            _panelItemList.Clear();
        }

        public RecordPanelItem Get(int index)
        {
            if (index < 0 || index >= _panelItemList.Count)
                return null;

            return _panelItemList[index];
        }

        public RecordPanelItem FindPanelItem(GameObject obj)
        {
            RecordPanelItem item = null;

            for(int i = 0; i < _panelItemList.Count; i++)
            {
                if (_panelItemList[i].recordGameObject == obj)
                {
                    item = _panelItemList[i];
                    break;
                }                  
            }

            return item;
        }

        public GameObject FindSourceObject(RecordPanelItem item)
        {
            GameObject obj = null;

            if (item.recordGameObject == null)
                return obj;

            for (int i = 0; i < _recordList.Count; i++)
            {
                if (_recordList[i] == item.recordGameObject)
                {
                    obj = _recordList[i];
                    break;
                }
            }

            return obj;
        }

    }

    public class RecordObjectListAdaptor : GenericListAdaptor<RecordPanelItem>, IReorderableListDropTarget
    {
        public const float mouseDragThresholdInPixels = 0.6f;
        private RecordPanelViewList _panelViewList;
        // Static reference to the list adaptor of the selected item.
        private RecordObjectListAdaptor _selectedList;
        // Static reference limits selection to one item in one list.
        private RecordPanelItem _selectedItem;
        // Position in GUI where mouse button was anchored before dragging occurred.
        private Vector2 _mouseDownPosition;

        // 列表选中回调函数
        public Action<GameObject> onItemSelect;
        // 列表删除回调函数
        public Action<GameObject> onItemRemove;
        // 列表添加回调函数
        public Action<GameObject> onItemAdded;

        // Holds data representing the item that is being dragged.
        private class DraggedItem
        {
            public static readonly string TypeName = typeof(DraggedItem).FullName;
            public readonly RecordObjectListAdaptor sourceListAdaptor;
            public readonly int Index;
            public readonly RecordPanelItem dragItem;

            public DraggedItem(RecordObjectListAdaptor sourceList, int index, RecordPanelItem item)
            {
                sourceListAdaptor = sourceList;
                Index = index;
                dragItem = item;
            }
        }

        public RecordObjectListAdaptor(RecordPanelViewList panelViewList) : base(panelViewList.panelItemList, null, 25f)
        {
            _panelViewList = panelViewList;
        }

        public void Init()
        {
            if (List.Count > 0)
                Select(0);
        }

        public void OnUpdate()
        {
            if (_panelViewList.recordList == null)
                _panelViewList.Clear();

            // 数据源添加更新检测
            for (int i = 0; i < _panelViewList.recordList.Count; i++)
            {
                RecordPanelItem item = _panelViewList.FindPanelItem(_panelViewList.recordList[i]);

                if (item == null)
                {
                    // 源数据有添加新对象， 添加一个新的panelItem UI对象
                    _panelViewList.Add(_panelViewList.recordList[i]);

                    if (onItemAdded != null)
                        onItemAdded(_panelViewList.recordList[i]);
                }
            }

            for (int i = 0; i < _panelViewList.panelItemList.Count; i++)
            {
                GameObject obj = _panelViewList.FindSourceObject(_panelViewList.panelItemList[i]);

                if (obj == null)
                {
                    // 源数据有删除对象， 删除对应的panelItem UI对象
                    _panelViewList.Remove(i);

                    Select(-1);
                }
            }
        }

        public override void DrawItemBackground(Rect position, int index)
        {
            if (this == _selectedList && List[index] == _selectedItem)
            {
                Color restoreColor = GUI.color;
                //GUI.color = ReorderableListStyles.SelectionBackgroundColor;
                GUI.color = new Color(0, 183 / 255f, 238 / 255f);
                GUI.DrawTexture(position, EditorGUIUtility.whiteTexture);
                GUI.color = restoreColor;
            }
        }

        public override void DrawItem(Rect position, int index)
        {
            RecordPanelItem storyItem = List[index];
            storyItem.itemRect = position;

            int controlID = GUIUtility.GetControlID(FocusType.Passive);

            switch (Event.current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    Rect totalItemPosition = ReorderableListGUI.CurrentItemTotalPosition;
                    if (totalItemPosition.Contains(Event.current.mousePosition))
                    {
                        // Select this list item.
                        _selectedList = this;
                        _selectedItem = storyItem;

                        OnSelect(index);
                    }

                    // Calculate rectangle of draggable area of the list item.
                    // This example excludes the grab handle at the left.
                    Rect draggableRect = totalItemPosition;
                    draggableRect.x = position.x;
                    draggableRect.width = position.width;

                    if (Event.current.button == 0 && draggableRect.Contains(Event.current.mousePosition))
                    {
                        // Select this list item.
                        _selectedList = this;
                        _selectedItem = storyItem;

                        // Lock onto this control whilst left mouse button is held so
                        // that we can start a drag-and-drop operation when user drags.
                        GUIUtility.hotControl = controlID;
                        _mouseDownPosition = Event.current.mousePosition;
                        Event.current.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;

                        // Begin drag-and-drop operation when the user drags the mouse
                        // pointer across the threshold. This threshold helps to avoid
                        // inadvertently starting a drag-and-drop operation.
                        if (Vector2.Distance(_mouseDownPosition, Event.current.mousePosition) >= mouseDragThresholdInPixels)
                        {
                            // Prepare data that will represent the item.
                            var item = new DraggedItem(this, index, storyItem);

                            // Start drag-and-drop operation with the Unity API.
                            DragAndDrop.PrepareStartDrag();
                            // Need to reset `objectReferences` and `paths` because `PrepareStartDrag`
                            // doesn't seem to reset these (at least, in Unity 4.x).
                            DragAndDrop.objectReferences = new UnityEngine.Object[0];
                            DragAndDrop.paths = new string[0];

                            DragAndDrop.SetGenericData(DraggedItem.TypeName, item);
                            DragAndDrop.StartDrag(storyItem.recordGameObject.name);
                        }

                        // Use this event so that the host window gets repainted with
                        // each mouse movement.
                        Event.current.Use();
                    }
                    break;

                case EventType.Repaint:
                    position.y += 5;

                    if (storyItem != null && storyItem.recordGameObject != null)
                    {
                        EditorStyles.label.Draw(position, storyItem.recordGameObject.name, false, false, false, false);
                    }
                    break;
            }
        }

        public bool CanDropInsert(int insertionIndex)
        {
            if (!ReorderableListControl.CurrentListPosition.Contains(Event.current.mousePosition))
                return false;

            // Drop insertion is possible if the current drag-and-drop operation contains
            // the supported type of custom data.
            return DragAndDrop.GetGenericData(DraggedItem.TypeName) is DraggedItem;
        }

        public void ProcessDropInsertion(int insertionIndex)
        {
            if (Event.current.type == EventType.DragPerform)
            {
                var draggedItem = DragAndDrop.GetGenericData(DraggedItem.TypeName) as DraggedItem;

                // Are we just reordering within the same list?
                if (draggedItem.sourceListAdaptor == this)
                {
                    Move(draggedItem.Index, insertionIndex);
                }
                else
                {
                    // Nope, we are moving the item!
                    List.Insert(insertionIndex, draggedItem.dragItem);
                    draggedItem.sourceListAdaptor.Remove(draggedItem.Index);

                    // Ensure that the item remains selected at its new location!
                    _selectedList = this;
                }
            }
        }

        public override void Add()
        {
            //_panelViewList.Add();
            //Select(_panelViewList.panelItemList.Count - 1);
        }

        public override void Remove(int index)
        {
            RecordPanelItem item = _panelViewList.Get(index);

            if (onItemRemove != null && item != null)
                onItemRemove(item.recordGameObject);

            /*_panelViewList.Remove(index);

            if (index > 0)
            {
                // if we remove item that is not first one, we select the last one
                Select(index - 1);
            }
            else if (index == 0 && List.Count > 0)
            {
                // if we remove item that is first one and not the only one in the list, we select the next one
                Select(index + 1);
            }
            else
            {
                Select(-1);
            }*/
        }

        void OnSelect(int index)
        {
            Select(index);
        }

        void Select(int index)
        {
            RecordPanelItem item = _panelViewList.Get(index);

            if (index >= 0 && index < List.Count)
            {
                _selectedItem = List[index];
                _selectedList = this;

                if (onItemSelect != null && item != null)
                    onItemSelect(item.recordGameObject);
            }else
            {
                if (onItemSelect != null)
                    onItemSelect(null);
            }
        }
    }
}

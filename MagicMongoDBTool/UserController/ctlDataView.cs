﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MagicMongoDBTool.Module;
using MongoDB.Bson;
using System.IO;

namespace MagicMongoDBTool.UserController
{
    public partial class ctlDataView : UserControl
    {

        #region"Main"
        /// <summary>
        /// 这里需要控制3中不同的数据类型，普通的Collection，GFS，USER。
        /// 图标复用方式来处理不同的类型。
        /// </summary>

        public ctlDataView(MongoDBHelper.DataViewInfo _DataViewInfo)
        {
            InitializeComponent();
            mDataViewInfo = _DataViewInfo;
        }
        /// <summary>
        /// Control for show Data
        /// </summary>
        public List<Control> _dataShower = new List<Control>();
        /// <summary>
        /// DataView信息
        /// </summary>
        public MongoDBHelper.DataViewInfo mDataViewInfo;
        /// <summary>
        /// 关闭Tab事件
        /// </summary>
        public event EventHandler CloseTab;
        /// <summary>
        /// 是否为Admin数据库
        /// </summary>
        private Boolean IsAdminDB;
        /// <summary>
        /// 是否为系统数据集
        /// </summary>
        private Boolean IsSystemCollection;
        /// <summary>
        /// 节点类型
        /// </summary>
        private String strNodeType;
        /// <summary>
        /// 节点路径
        /// </summary>
        private String strNodeData;
        /// <summary>
        /// 加载数据集控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ctlDataView_Load(object sender, EventArgs e)
        {
            this.cmbRecPerPage.SelectedIndex = 1;
            mDataViewInfo.LimitCnt = 100;
            strNodeType = mDataViewInfo.strDBTag.Split(":".ToCharArray())[0];
            strNodeData = mDataViewInfo.strDBTag.Split(":".ToCharArray())[1];

            String[] DataList = strNodeData.Split("/".ToCharArray());
            if (DataList[(int)MongoDBHelper.PathLv.DatabaseLV] == MongoDBHelper.DATABASE_NAME_ADMIN)
            {
                IsAdminDB = true;
            }
            IsSystemCollection = MongoDBHelper.IsSystemCollection(DataList[(int)MongoDBHelper.PathLv.DatabaseLV],
                                                                  DataList[(int)MongoDBHelper.PathLv.CollectionLV]);

            if (strNodeType == MongoDBHelper.COLLECTION_TAG)
            {
                _dataShower.Add(lstData);
                _dataShower.Add(trvData);
                _dataShower.Add(txtData);
            }
            else
            {
                _dataShower.Add(lstData);
                this.tabDataShower.Controls.Remove(tabTreeView);
                this.tabDataShower.Controls.Remove(tabTextView);
            }

            this.lstData.MouseClick += new MouseEventHandler(lstData_MouseClick);
            this.lstData.MouseDoubleClick += new MouseEventHandler(lstData_MouseDoubleClick);
            this.lstData.SelectedIndexChanged += new EventHandler(lstData_SelectedIndexChanged);
            this.trvData.DatatreeView.MouseClick += new MouseEventHandler(trvData_MouseClick_Top);
            this.trvData.DatatreeView.AfterSelect += new TreeViewEventHandler(trvData_AfterSelect_Top);
            this.trvData.DatatreeView.KeyDown += new KeyEventHandler(trvData_KeyDown);
            this.trvData.DatatreeView.AfterExpand += new TreeViewEventHandler(trvData_AfterExpand);
            this.trvData.DatatreeView.AfterCollapse += new TreeViewEventHandler(trvData_AfterCollapse);


            this.tabDataShower.SelectedIndexChanged += new EventHandler(
                //If tabpage changed,the selected data in dataview will disappear,set delete selected record to false
                (x, y) =>
                {
                    this.DelSelectRecordToolStripMenuItem.Enabled = false;
                    if (IsNeedRefresh)
                    {
                        RefreshGUI(sender, e);
                    }
                }
            );
            if (!SystemManager.IsUseDefaultLanguage())
            {
                //数据显示区
                this.tabTreeView.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Tab_Tree);
                this.tabTableView.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Tab_Table);
                this.tabTextView.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Tab_Text);

                this.NewDocumentToolStripMenuItem.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_Operation_DataCollection_AddDocument);
                this.NewDocumentStripButton.Text = this.NewDocumentToolStripMenuItem.Text;
                this.OpenDocInEditorStripButton.Text = SystemManager.mStringResource.GetText(StringResource.TextType.OpenInNativeEditor);
                this.DelSelectRecordToolStripMenuItem.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_Operation_DataCollection_DropDocument);
                this.DelSelectRecordToolStripButton.Text = this.DelSelectRecordToolStripMenuItem.Text;

                this.PrePageStripButton.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_DataView_Previous);
                this.NextPageStripButton.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_DataView_Next);
                this.FirstPageStripButton.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_DataView_First);
                this.LastPageStripButton.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_DataView_Last);
                this.QueryStripButton.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_DataView_Query);
                this.FilterStripButton.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_DataView_DataFilter);

                this.AddElementToolStripMenuItem.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_Operation_DataDocument_AddElement);
                this.DropElementToolStripMenuItem.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_Operation_DataDocument_DropElement);
                this.ModifyElementToolStripMenuItem.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_Operation_DataDocument_ModifyElement);
                
                this.CopyElementToolStripMenuItem.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_Operation_DataDocument_CopyElement);
                this.CopyElementStripButton.Text = this.CopyElementToolStripMenuItem.Text;
                this.CutElementToolStripMenuItem.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_Operation_DataDocument_CutElement);
                this.CutElementStripButton.Text = this.CutElementToolStripMenuItem.Text;
                this.PasteElementToolStripMenuItem.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_Operation_DataDocument_PasteElement);
                this.PasteElementStripButton.Text = this.PasteElementToolStripMenuItem.Text;

                this.DeleteFileToolStripMenuItem.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_Operation_FileSystem_DelFile);
                this.DeleteFileStripButton.Text = this.DeleteFileToolStripMenuItem.Text;

                this.UploadFileToolStripMenuItem.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_Operation_FileSystem_UploadFile);
                this.UploadFileStripButton.Text = this.UploadFileToolStripMenuItem.Text;

                this.UploadFolderToolStripMenuItem.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_Operation_FileSystem_UploadFolder);
                this.UpLoadFolderStripButton.Text = this.UploadFolderToolStripMenuItem.Text;

                this.DownloadFileToolStripMenuItem.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_Operation_FileSystem_Download);
                this.DownloadFileStripButton.Text = this.DownloadFileToolStripMenuItem.Text;

                this.OpenFileToolStripMenuItem.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_Operation_FileSystem_OpenFile);
                this.OpenFileStripButton.Text = this.OpenFileToolStripMenuItem.Text;

                this.RefreshStripButton.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Common_Refresh);
                this.CloseStripButton.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Common_Close);
                this.ExpandAllStripButton.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Common_Expansion);
                this.CollapseAllStripButton.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Common_Collapse);

                this.AddUserStripButton.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_Operation_Database_AddUser);
                this.AddUserToolStripMenuItem.Text = this.AddUserStripButton.Text;
                this.ChangePasswordStripButton.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Common_ChangePassword);
                this.ChangePasswordToolStripMenuItem.Text = this.ChangePasswordStripButton.Text;
                this.RemoveUserStripButton.Text = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_Operation_Database_DelUser);
                this.RemoveUserToolStripMenuItem.Text = this.RemoveUserStripButton.Text;
            }
            InitControlsVisiableAndEvent();
            InitControlsEnable();
            //加载数据
            List<BsonDocument> datalist = MongoDBHelper.GetDataList(ref mDataViewInfo);
            MongoDBHelper.FillDataToControl(datalist,_dataShower,mDataViewInfo);
            //数据导航
            SetDataNav();
        }
        /// <summary>
        /// 将所有的按钮和右键菜单无效化
        /// </summary>
        private void InitControlsVisiableAndEvent()
        {
            foreach (var item in this.contextMenuStripMain.Items)
            {
                if (item is ToolStripMenuItem)
                {
                    ((ToolStripMenuItem)item).Visible = false;
                }
            }
            foreach (var item in ViewtoolStrip.Items)
            {
                if (item is ToolStripButton)
                {
                    ((ToolStripButton)item).Visible = false;
                }
            }
            switch (strNodeType)
            {
                case MongoDBHelper.COLLECTION_TAG:
                    NewDocumentStripButton.Visible = true;
                    NewDocumentToolStripMenuItem.Visible = true;
                    NewDocumentToolStripMenuItem.Click += new EventHandler(NewDocumentStripButton_Click);

                    OpenDocInEditorStripButton.Visible = true;
                    OpenDocInEditorStripMenuItem.Visible = true;
                    OpenDocInEditorStripMenuItem.Click += new EventHandler(OpenDocInEditorDocStripButton_Click);

                    DelSelectRecordToolStripButton.Visible = true;
                    DelSelectRecordToolStripMenuItem.Visible = true;
                    DelSelectRecordToolStripMenuItem.Click += new EventHandler(DelSelectRecordToolStripButton_Click);

                    ExpandAllStripButton.Visible = true;
                    CollapseAllStripButton.Visible = true;

                    CutElementStripButton.Visible = true;
                    CutElementStripButton.Click += new EventHandler(CutElementToolStripMenuItem_Click);
                    CutElementToolStripMenuItem.Visible = true;

                    CopyElementStripButton.Visible = true;
                    CopyElementStripButton.Click += new EventHandler(CopyElementToolStripMenuItem_Click);
                    CopyElementToolStripMenuItem.Visible = true;

                    PasteElementStripButton.Visible = true;
                    PasteElementStripButton.Click += new EventHandler(PasteElementToolStripMenuItem_Click);
                    PasteElementToolStripMenuItem.Visible = true;

                    AddElementToolStripMenuItem.Visible = true;
                    DropElementToolStripMenuItem.Visible = true;
                    ModifyElementToolStripMenuItem.Visible = true;

                    break;
                case MongoDBHelper.GRID_FILE_SYSTEM_TAG:
                    OpenFileStripButton.Visible = true;
                    OpenFileToolStripMenuItem.Visible = true;
                    OpenFileToolStripMenuItem.Click += new EventHandler(OpenFileStripButton_Click);

                    DownloadFileStripButton.Visible = true;
                    DownloadFileToolStripMenuItem.Visible = true;
                    DownloadFileToolStripMenuItem.Click += new EventHandler(DownloadFileStripButton_Click);

                    UploadFileStripButton.Visible = true;
                    UploadFileToolStripMenuItem.Visible = true;
                    UploadFileToolStripMenuItem.Click += new EventHandler(UploadFileStripButton_Click);

                    UpLoadFolderStripButton.Visible = true;
                    UploadFolderToolStripMenuItem.Visible = true;
                    UploadFolderToolStripMenuItem.Click += new EventHandler(UpLoadFolderStripButton_Click);

                    DeleteFileStripButton.Visible = true;
                    DeleteFileToolStripMenuItem.Visible = true;
                    DeleteFileToolStripMenuItem.Click += new EventHandler(DeleteFileStripButton_Click);

                    SeperateBar1.Visible = false;
                    SeperateBarForMenuItem1.Visible = false;
                    SeperateBarForMenuItem2.Visible = false;

                    break;

                case MongoDBHelper.USER_LIST_TAG:
                    AddUserStripButton.Visible = true;
                    AddUserStripButton.Enabled = true;
                    AddUserToolStripMenuItem.Visible = true;
                    AddUserToolStripMenuItem.Click += new EventHandler(AddUserStripButton_Click);

                    RemoveUserStripButton.Visible = true;
                    RemoveUserToolStripMenuItem.Visible = true;
                    RemoveUserToolStripMenuItem.Click += new EventHandler(RemoveUserStripButton_Click);

                    ChangePasswordStripButton.Visible = true;
                    ChangePasswordToolStripMenuItem.Visible = true;
                    ChangePasswordToolStripMenuItem.Click += new EventHandler(ChangePasswordStripButton_Click);

                    SeperateBar1.Visible = false;
                    SeperateBarForMenuItem1.Visible = false;
                    SeperateBarForMenuItem2.Visible = false;

                    break;
                default:
                    break;
            }

            FirstPageStripButton.Visible = true;
            PrePageStripButton.Visible = true;
            NextPageStripButton.Visible = true;
            LastPageStripButton.Visible = true;
            this.FilterStripButton.Visible = true;
            this.QueryStripButton.Visible = true;

            GotoStripButton.Visible = true;
            RefreshStripButton.Visible = true;
            RefreshStripButton.Enabled = true;
            CloseStripButton.Visible = true;
            CloseStripButton.Enabled = true;
        }
        private void InitControlsEnable()
        {
            foreach (var item in this.contextMenuStripMain.Items)
            {
                if (item is ToolStripMenuItem)
                {
                    ((ToolStripMenuItem)item).Enabled = false;
                }
            }
            foreach (var item in ViewtoolStrip.Items)
            {
                if (item is ToolStripButton)
                {
                    ((ToolStripButton)item).Enabled = false;
                }
            }

            switch (strNodeType)
            {
                case MongoDBHelper.COLLECTION_TAG:
                    OpenDocInEditorStripButton.Enabled = true;
                    OpenDocInEditorStripMenuItem.Enabled = true;
                    if (!mDataViewInfo.IsReadOnly)
                    {
                        this.NewDocumentStripButton.Enabled = true;
                        this.NewDocumentToolStripMenuItem.Enabled = true;
                    }
                    ExpandAllStripButton.Enabled = true;
                    CollapseAllStripButton.Enabled = true;

                    break;
                case MongoDBHelper.GRID_FILE_SYSTEM_TAG:
                    UploadFileStripButton.Enabled = true;
                    UploadFileToolStripMenuItem.Enabled = true;

                    UpLoadFolderStripButton.Enabled = true;
                    UploadFolderToolStripMenuItem.Enabled = true;

                    break;

                case MongoDBHelper.USER_LIST_TAG:
                    AddUserStripButton.Enabled = true;
                    AddUserToolStripMenuItem.Enabled = true;
                    break;
                default:
                    break;
            }

            PrePageStripButton.Enabled = mDataViewInfo.HasPrePage;
            NextPageStripButton.Enabled = mDataViewInfo.HasNextPage;
            FirstPageStripButton.Enabled = mDataViewInfo.HasPrePage;
            LastPageStripButton.Enabled = mDataViewInfo.HasNextPage;

            this.FilterStripButton.Checked = mDataViewInfo.IsUseFilter;
            this.FilterStripButton.Enabled = true;
            this.QueryStripButton.Enabled = true;

            GotoStripButton.Enabled = true;
            RefreshStripButton.Enabled = true;
            CloseStripButton.Enabled = true;
        }
        /// <summary>
        /// 关闭本Tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseStripButton_Click(object sender, EventArgs e)
        {
            if (CloseTab != null)
            {
                CloseTab(sender, e);
            }
        }

        #endregion

        #region"数据展示区操作"
        /// <summary>
        /// Is Need Refresh after the element is modify
        /// </summary>
        public Boolean IsNeedRefresh = false;
        /// <summary>
        /// 数据列表选中索引变换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstData_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SystemManager.GetCurrentCollection().Name)
            {
                case MongoDBHelper.COLLECTION_NAME_GFS_FILES:
                    //文件系统
                    this.UploadFileToolStripMenuItem.Enabled = true;
                    this.UploadFileStripButton.Enabled = true;

                    this.UpLoadFolderStripButton.Enabled = true;
                    this.UploadFolderToolStripMenuItem.Enabled = true;
                    switch (lstData.SelectedItems.Count)
                    {
                        case 0:
                            //禁止所有操作
                            this.OpenFileStripButton.Enabled = false;
                            this.OpenFileToolStripMenuItem.Enabled = false;

                            this.DownloadFileToolStripMenuItem.Enabled = false;
                            this.DownloadFileStripButton.Enabled = false;

                            this.DeleteFileStripButton.Enabled = false;
                            this.DeleteFileToolStripMenuItem.Enabled = false;

                            lstData.ContextMenuStrip = null;
                            break;
                        case 1:
                            //可以进行所有操作
                            this.OpenFileStripButton.Enabled = true;
                            this.OpenFileToolStripMenuItem.Enabled = true;
                            this.DownloadFileToolStripMenuItem.Enabled = true;
                            this.DownloadFileStripButton.Enabled = true;
                            if (!mDataViewInfo.IsReadOnly)
                            {
                                this.DeleteFileStripButton.Enabled = true;
                                this.DeleteFileToolStripMenuItem.Enabled = true;
                            }
                            break;
                        default:
                            //可以删除多个文件
                            this.OpenFileStripButton.Enabled = false;
                            this.OpenFileToolStripMenuItem.Enabled = false;

                            this.DownloadFileToolStripMenuItem.Enabled = false;
                            this.DownloadFileStripButton.Enabled = false;
                            if (!mDataViewInfo.IsReadOnly)
                            {
                                this.DeleteFileStripButton.Enabled = true;
                                this.DeleteFileToolStripMenuItem.Enabled = true;
                            }
                            break;
                    }
                    break;
                case MongoDBHelper.COLLECTION_NAME_USER:
                    //用户数据库
                    if (lstData.SelectedItems.Count > 0 && !mDataViewInfo.IsReadOnly)
                    {
                        this.AddUserToolStripMenuItem.Enabled = true;
                        this.AddUserStripButton.Enabled = true;

                        this.RemoveUserStripButton.Enabled = true;
                        this.RemoveUserToolStripMenuItem.Enabled = true;

                        if (this.lstData.SelectedItems.Count == 1)
                        {
                            this.ChangePasswordStripButton.Enabled = true;
                            this.ChangePasswordToolStripMenuItem.Enabled = true;
                        }
                    }
                    break;
                default:
                    //数据系统
                    if (lstData.SelectedItems.Count > 0 && !IsSystemCollection && !mDataViewInfo.IsReadOnly)
                    {
                        DelSelectRecordToolStripMenuItem.Enabled = true;
                        DelSelectRecordToolStripButton.Enabled = true;
                    }
                    else
                    {
                        DelSelectRecordToolStripButton.Enabled = false;
                        DelSelectRecordToolStripMenuItem.Enabled = false;
                    }
                    if (this.lstData.SelectedItems.Count == 1)
                    {
                        OpenDocInEditorStripButton.Enabled = true;
                        OpenDocInEditorStripMenuItem.Enabled = true;
                    }
                    break;
            }
        }
        /// <summary>
        /// 双击列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstData_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            switch (strNodeType)
            {
                case MongoDBHelper.GRID_FILE_SYSTEM_TAG:
                    OpenFileStripButton_Click(sender, e);
                    break;
                case MongoDBHelper.USER_LIST_TAG:
                    ChangePasswordStripButton_Click(sender, e);
                    break;
                case MongoDBHelper.COLLECTION_TAG:
                    OpenDocInEditorDocStripButton_Click(sender, e);
                    break;
            }
        }
        /// <summary>
        /// 数据列表右键菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstData_MouseClick(object sender, MouseEventArgs e)
        {
            SystemManager.SelectObjectTag = mDataViewInfo.strDBTag;
            if (lstData.SelectedItems.Count > 0)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    lstData.ContextMenuStrip = this.contextMenuStripMain;
                    contextMenuStripMain.Show(lstData.PointToScreen(e.Location));
                }
            }
        }

        /// <summary>
        /// 数据树形被选择后(TOP)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trvData_AfterSelect_Top(object sender, TreeViewEventArgs e)
        {
            InitControlsEnable();
            SystemManager.SelectObjectTag = mDataViewInfo.strDBTag;
            if (trvData.DatatreeView.SelectedNode.Level == 0)
            {
                //顶层可以删除的节点
                if (!mDataViewInfo.IsReadOnly)
                {
                    if (!MongoDBHelper.IsSystemCollection(SystemManager.GetCurrentCollection()) && !SystemManager.GetCurrentCollection().IsCapped())
                    {
                        //普通数据
                        //在顶层的时候，允许添加元素,不允许删除元素和修改元素(删除选中记录)
                        DelSelectRecordToolStripMenuItem.Enabled = true;
                        DelSelectRecordToolStripButton.Enabled = true;
                        AddElementToolStripMenuItem.Enabled = true;
                        if (MongoDBHelper.CanPasteAsElement)
                        {
                            PasteElementToolStripMenuItem.Enabled = true;
                            PasteElementStripButton.Enabled = true;
                        }
                    }
                    else
                    {
                        DelSelectRecordToolStripMenuItem.Enabled = false;
                        DelSelectRecordToolStripButton.Enabled = false;
                    }
                }
            }
            else
            {
                //非顶层元素
                trvData_AfterSelect_NotTop(sender, e);
            }
        }
        /// <summary>
        /// 数据树形被选择后(非TOP)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trvData_AfterSelect_NotTop(object sender, TreeViewEventArgs e)
        {
            //非顶层可以删除的节点
            switch (SystemManager.GetCurrentCollection().Name)
            {
                case MongoDBHelper.COLLECTION_NAME_GFS_FILES:
                case MongoDBHelper.COLLECTION_NAME_USER:
                default:
                    if (!MongoDBHelper.IsSystemCollection(SystemManager.GetCurrentCollection()) &&
                        !mDataViewInfo.IsReadOnly &&
                        !SystemManager.GetCurrentCollection().IsCapped())
                    {
                        //普通数据:允许添加元素,不允许删除元素
                        DropElementToolStripMenuItem.Enabled = true;
                        CopyElementToolStripMenuItem.Enabled = true;
                        CopyElementStripButton.Enabled = true;
                        CutElementToolStripMenuItem.Enabled = true;
                        CutElementStripButton.Enabled = true;
                        if (trvData.DatatreeView.SelectedNode.Nodes.Count != 0)
                        {
                            //父节点
                            //1. 以Array_Mark结尾的数组
                            //2. Document
                            if (trvData.DatatreeView.SelectedNode.FullPath.EndsWith(MongoDBHelper.Array_Mark))
                            {
                                //列表的父节点
                                if (MongoDBHelper.CanPasteAsValue)
                                {
                                    PasteElementToolStripMenuItem.Enabled = true;
                                    PasteElementStripButton.Enabled = true;
                                }
                            }
                            else
                            {
                                //文档的父节点
                                if (MongoDBHelper.CanPasteAsElement)
                                {
                                    PasteElementToolStripMenuItem.Enabled = true;
                                    PasteElementStripButton.Enabled = true;
                                }
                            }
                            AddElementToolStripMenuItem.Enabled = true;
                            ModifyElementToolStripMenuItem.Enabled = false;
                        }
                        else
                        {
                            //子节点
                            //1.简单元素
                            //2.空的Array
                            //3.空的文档
                            //4.Array中的Value
                            BsonValue t;
                            if (trvData.DatatreeView.SelectedNode.Tag is BsonElement)
                            {
                                //子节点是一个元素，获得子节点的Value
                                t = ((BsonElement)trvData.DatatreeView.SelectedNode.Tag).Value;
                                if (t.IsBsonDocument || t.IsBsonArray)
                                {
                                    //2.空的Array
                                    //3.空的文档
                                    ModifyElementToolStripMenuItem.Enabled = false;
                                    AddElementToolStripMenuItem.Enabled = true;
                                    if (t.IsBsonDocument)
                                    {
                                        //3.空的文档
                                        if (MongoDBHelper.CanPasteAsElement)
                                        {
                                            PasteElementToolStripMenuItem.Enabled = true;
                                            PasteElementStripButton.Enabled = true;
                                        }

                                    }
                                    if (t.IsBsonArray)
                                    {
                                        //3.Array
                                        if (MongoDBHelper.CanPasteAsValue)
                                        {
                                            PasteElementToolStripMenuItem.Enabled = true;
                                            PasteElementStripButton.Enabled = true;
                                        }
                                    }
                                }
                                else
                                {
                                    //1.简单元素
                                    ModifyElementToolStripMenuItem.Enabled = true;
                                    AddElementToolStripMenuItem.Enabled = false;
                                }
                            }
                            else
                            {
                                //子节点是一个Array的Value，获得Value
                                //4.Array中的Value
                                t = (BsonValue)trvData.DatatreeView.SelectedNode.Tag;
                                ModifyElementToolStripMenuItem.Enabled = true;
                                if (t.IsBsonArray || t.IsBsonDocument)
                                {
                                    //当这个值是一个数组或者文档时候，仍然允许其添加子元素
                                    AddElementToolStripMenuItem.Enabled = true;
                                }
                                else
                                {
                                    AddElementToolStripMenuItem.Enabled = false;
                                }
                            }
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// 是否需要改变选中节点
        /// </summary>
        private Boolean IsNeedChangeNode = true;
        /// <summary>
        /// 展开节点后的动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void trvData_AfterExpand(object sender, TreeViewEventArgs e)
        {
            trvData.DatatreeView.SelectedNode = e.Node;
            IsNeedChangeNode = false;
            SystemManager.SetCurrentDocument(e.Node);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void trvData_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            trvData.DatatreeView.SelectedNode = e.Node;
            IsNeedChangeNode = false;
            SystemManager.SetCurrentDocument(e.Node);
        }
        /// <summary>
        /// 鼠标动作（顶层）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trvData_MouseClick_Top(object sender, MouseEventArgs e)
        {
            if (IsNeedChangeNode)
            {
                //在节点展开和关闭后，不能使用这个方法来重新设定SelectedNode
                trvData.DatatreeView.SelectedNode = this.trvData.DatatreeView.GetNodeAt(e.Location);
            }
            IsNeedChangeNode = true;
            if (trvData.DatatreeView.SelectedNode == null)
            {
                return;
            }
            SystemManager.SetCurrentDocument(trvData.DatatreeView.SelectedNode);
            if (trvData.DatatreeView.SelectedNode.Level == 0)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    this.contextMenuStripMain = new ContextMenuStrip();
                    ///允许删除
                    this.contextMenuStripMain.Items.Add(this.DelSelectRecordToolStripMenuItem.Clone());
                    ///允许添加
                    this.contextMenuStripMain.Items.Add(this.AddElementToolStripMenuItem.Clone());
                    ///允许粘贴
                    this.contextMenuStripMain.Items.Add(this.PasteElementToolStripMenuItem.Clone());
                    trvData.DatatreeView.ContextMenuStrip = this.contextMenuStripMain;
                    contextMenuStripMain.Show(trvData.DatatreeView.PointToScreen(e.Location));
                }
            }
            else
            {
                //非顶层元素
                trvData_MouseClick_NotTop(sender, e);
            }
        }
        /// <summary>
        /// 鼠标动作（非顶层）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trvData_MouseClick_NotTop(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.contextMenuStripMain = new ContextMenuStrip();
                this.contextMenuStripMain.Items.Add(this.AddElementToolStripMenuItem.Clone());
                this.contextMenuStripMain.Items.Add(this.ModifyElementToolStripMenuItem.Clone());
                this.contextMenuStripMain.Items.Add(this.DropElementToolStripMenuItem.Clone());
                this.contextMenuStripMain.Items.Add(this.CopyElementToolStripMenuItem.Clone());
                this.contextMenuStripMain.Items.Add(this.CutElementToolStripMenuItem.Clone());
                this.contextMenuStripMain.Items.Add(this.PasteElementToolStripMenuItem.Clone());
                trvData.DatatreeView.ContextMenuStrip = this.contextMenuStripMain;
                contextMenuStripMain.Show(trvData.DatatreeView.PointToScreen(e.Location));
            }
        }
        /// <summary>
        /// 键盘动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void trvData_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    if (DelSelectRecordToolStripMenuItem.Enabled)
                    {
                        //DelSelectedRecord_Click(sender, e);
                    }
                    else
                    {
                        if (this.DropElementToolStripMenuItem.Enabled)
                        {
                            DropElementToolStripMenuItem_Click(sender, e);
                        }
                    }
                    break;
                case Keys.F2:
                    if (this.ModifyElementToolStripMenuItem.Enabled)
                    {
                        ModifyElementToolStripMenuItem_Click(sender, e);
                    }
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region"管理：元素操作"

        /// <summary>
        /// Add New Document
        /// </summary>
        private void NewDocumentStripButton_Click(object sender, EventArgs e)
        {
            SystemManager.OpenForm(new frmNewDocument());
            RefreshGUI(null, null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenDocInEditorDocStripButton_Click(object sender, EventArgs e)
        {
            MongoDBHelper.SaveAndOpenStringAsFile(txtData.Text);
        }
        /// <summary>
        /// Delete Selected Documents
        /// </summary>
        private void DelSelectRecordToolStripButton_Click(object sender, EventArgs e)
        {
            String strTitle = "Delete Document";
            String strMessage = "Are you sure to delete selected document(s)?";
            if (!SystemManager.IsUseDefaultLanguage())
            {
                strTitle = SystemManager.mStringResource.GetText(StringResource.TextType.Drop_Data);
                strMessage = SystemManager.mStringResource.GetText(StringResource.TextType.Drop_Data_Confirm);
            }
            if (MyMessageBox.ShowConfirm(strTitle, strMessage))
            {
                if (tabDataShower.SelectedTab == tabTableView)
                {
                    //lstData
                    String StrErrormsg = String.Empty;
                    foreach (ListViewItem item in lstData.SelectedItems)
                    {
                        if (!MongoDBHelper.DropDocument(SystemManager.GetCurrentCollection(), item.Tag))
                        {
                            StrErrormsg += "Delete Error Key is:" + item.Tag.ToString() + System.Environment.NewLine;
                        };
                    }
                    MyMessageBox.ShowMessage("Delete Error", StrErrormsg);
                    lstData.ContextMenuStrip = null;
                }
                else
                {
                    if (!MongoDBHelper.DropDocument(SystemManager.GetCurrentCollection(), trvData.DatatreeView.SelectedNode.Tag))
                    {
                        MyMessageBox.ShowMessage("Delete Error", "Delete Error Key is:" + trvData.DatatreeView.SelectedNode.Tag.ToString());
                    }
                    trvData.DatatreeView.ContextMenuStrip = null;
                }
                DelSelectRecordToolStripMenuItem.Enabled = false;
                RefreshGUI(null, null);
            }
        }
        /// <summary>
        /// 添加元素
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Boolean IsElement = true;
            BsonValue t;
            if (trvData.DatatreeView.SelectedNode.Tag is BsonElement)
            {
                t = ((BsonElement)trvData.DatatreeView.SelectedNode.Tag).Value;
            }
            else
            {
                t = (BsonValue)trvData.DatatreeView.SelectedNode.Tag;
            }
            if (t.IsBsonArray)
            {
                IsElement = false;
            }
            SystemManager.OpenForm(new frmElement(false, trvData.DatatreeView.SelectedNode, IsElement));
            IsNeedRefresh = true;
        }
        /// <summary>
        /// 删除元素
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DropElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trvData.DatatreeView.SelectedNode.Level == 1 & trvData.DatatreeView.SelectedNode.PrevNode == null)
            {
                MyMessageBox.ShowMessage("Error", "_id Can't be delete");
                return;
            }
            if (trvData.DatatreeView.SelectedNode.Parent.Text.EndsWith(MongoDBHelper.Array_Mark))
            {
                MongoDBHelper.DropArrayValue(trvData.DatatreeView.SelectedNode.FullPath, trvData.DatatreeView.SelectedNode.Index);
            }
            else
            {
                MongoDBHelper.DropElement(trvData.DatatreeView.SelectedNode.FullPath, (BsonElement)trvData.DatatreeView.SelectedNode.Tag);
            }
            trvData.DatatreeView.Nodes.Remove(trvData.DatatreeView.SelectedNode);
            IsNeedRefresh = true;
        }
        /// <summary>
        /// 修改元素
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModifyElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trvData.DatatreeView.SelectedNode.Level == 1 & trvData.DatatreeView.SelectedNode.PrevNode == null)
            {
                MyMessageBox.ShowMessage("Error", "_id can't be modify");
                return;
            }
            if (trvData.DatatreeView.SelectedNode.Parent.Text.EndsWith(MongoDBHelper.Array_Mark))
            {
                SystemManager.OpenForm(new frmElement(true, trvData.DatatreeView.SelectedNode, false));
            }
            else
            {
                SystemManager.OpenForm(new frmElement(true, trvData.DatatreeView.SelectedNode,true));
            }
            IsNeedRefresh = true;
        }
        /// <summary>
        /// Copy Element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MongoDBHelper._ClipElement = trvData.DatatreeView.SelectedNode.Tag;
            if (trvData.DatatreeView.SelectedNode.Parent.Text.EndsWith(MongoDBHelper.Array_Mark))
            {
                MongoDBHelper.CopyValue((BsonValue)trvData.DatatreeView.SelectedNode.Tag);
            }
            else
            {
                MongoDBHelper.CopyElement((BsonElement)trvData.DatatreeView.SelectedNode.Tag);
            }
        }
        /// <summary>
        /// Paste Element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PasteElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trvData.DatatreeView.SelectedNode.FullPath.EndsWith(MongoDBHelper.Array_Mark))
            {
                MongoDBHelper.PasteValue(trvData.DatatreeView.SelectedNode.FullPath);
                TreeNode NewValue = new TreeNode(MongoDBHelper.ConvertToString((BsonValue)MongoDBHelper._ClipElement));
                NewValue.Tag = MongoDBHelper._ClipElement;
                trvData.DatatreeView.SelectedNode.Nodes.Add(NewValue);
            }
            else
            {
                String PasteMessage = MongoDBHelper.PasteElement(trvData.DatatreeView.SelectedNode.FullPath);
                if (String.IsNullOrEmpty(PasteMessage))
                {
                    //GetCurrentDocument()的第一个元素是ID
                    MongoDBHelper.AddBsonDocToTreeNode(trvData.DatatreeView.SelectedNode,
                                                       new BsonDocument().Add((BsonElement)MongoDBHelper._ClipElement));
                }
                else
                {
                    MyMessageBox.ShowMessage("Exception", PasteMessage);
                }
            }
            IsNeedRefresh = true;
        }
        /// <summary>
        /// Cut Element
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CutElementToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (trvData.DatatreeView.SelectedNode.Level == 1 & trvData.DatatreeView.SelectedNode.PrevNode == null)
            {
                MyMessageBox.ShowMessage("Error", "_id can't be cut");
                return;
            }
            if (trvData.DatatreeView.SelectedNode.Parent.Text.EndsWith(MongoDBHelper.Array_Mark))
            {
                MongoDBHelper.CutValue(trvData.DatatreeView.SelectedNode.FullPath, trvData.DatatreeView.SelectedNode.Index, (BsonValue)trvData.DatatreeView.SelectedNode.Tag);
            }
            else
            {
                MongoDBHelper.CutElement(trvData.DatatreeView.SelectedNode.FullPath, (BsonElement)trvData.DatatreeView.SelectedNode.Tag);
            }
            trvData.DatatreeView.Nodes.Remove(trvData.DatatreeView.SelectedNode);
            IsNeedRefresh = true;
        }
        #endregion

        #region"管理：GFS"
        /// <summary>
        /// Upload File
        /// </summary>
        private void UploadFileStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog upfile = new OpenFileDialog();
            MongoDBHelper.UpLoadFileOption opt = new MongoDBHelper.UpLoadFileOption();
            if (upfile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                frmGFSOption frm = new frmGFSOption();
                SystemManager.OpenForm(frm, false);
                opt.FileNameOpt = frm.filename;
                opt.AlreadyOpt = frm.option;
                frm.Dispose();
                MongoDBHelper.UpLoadFile(upfile.FileName, opt);
                RefreshGUI(null, null);
            }
        }
        /// <summary>
        /// 上传文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpLoadFolderStripButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog upfolder = new FolderBrowserDialog();
            MongoDBHelper.UpLoadFileOption opt = new MongoDBHelper.UpLoadFileOption();

            if (upfolder.ShowDialog() == DialogResult.OK)
            {
                frmGFSOption frm = new frmGFSOption();
                SystemManager.OpenForm(frm, false);
                opt.FileNameOpt = frm.filename;
                opt.AlreadyOpt = frm.option;
                opt.IgnoreSubFolder = frm.ignoreSubFolder;
                frm.Dispose();
                DirectoryInfo uploadDir = new DirectoryInfo(upfolder.SelectedPath);
                int count = 0;
                UploadFolder(uploadDir, ref count, opt);
                MyMessageBox.ShowMessage("Upload", "Upload Completed! Upload Files Count: " + count.ToString());
                RefreshGUI(null, null);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uploadDir"></param>
        /// <param name="fileCount"></param>
        /// <param name="opt"></param>
        /// <returns>是否继续执行后续的所有操作</returns>
        private Boolean UploadFolder(DirectoryInfo uploadDir, ref int fileCount, MongoDBHelper.UpLoadFileOption opt)
        {

            foreach (FileInfo file in uploadDir.GetFiles())
            {
                MongoDBHelper.UploadResult rtn = MongoDBHelper.UpLoadFile(file.FullName, opt);
                switch (rtn)
                {
                    case MongoDBHelper.UploadResult.Complete:
                        fileCount++;
                        break;
                    case MongoDBHelper.UploadResult.Skip:
                        if (opt.AlreadyOpt == MongoDBHelper.enumGFSAlready.Stop)
                        {
                            ///这个操作返回为False，停止包括父亲过程在内的所有操作
                            return false;
                        }
                        break;
                    case MongoDBHelper.UploadResult.Exception:
                        return MyMessageBox.ShowConfirm("Upload Exception", "Is Continue?");
                    default:
                        break;
                }
            }
            if (!opt.IgnoreSubFolder)
            {
                foreach (DirectoryInfo dir in uploadDir.GetDirectories())
                {
                    ///递归文件夹操作，如果下层有任何停止的意愿，则立刻停止，并且使上层也立刻停止
                    Boolean IsContinue = UploadFolder(dir, ref fileCount, opt);
                    if (!IsContinue) { return false; }
                }
            }
            return true;
        }
        /// <summary>
        /// DownLoad File
        /// </summary>
        public void DownloadFileStripButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog downfile = new SaveFileDialog();
            String strFileName = lstData.SelectedItems[0].Text;
            //For Winodws,Linux user DirectorySeparatorChar Replace with @"\"
            downfile.FileName = strFileName.Split(System.IO.Path.DirectorySeparatorChar)[strFileName.Split(System.IO.Path.DirectorySeparatorChar).Length - 1];
            if (downfile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                MongoDBHelper.DownloadFile(downfile.FileName, strFileName);
            }
            RefreshGUI(null, null);
        }
        /// <summary>
        /// Open File
        /// </summary>
        private void OpenFileStripButton_Click(object sender, EventArgs e)
        {
            String strFileName = lstData.SelectedItems[0].Text;
            MongoDBHelper.OpenFile(strFileName);
        }
        /// <summary>
        /// Delete File
        /// </summary>
        public void DeleteFileStripButton_Click(object sender, EventArgs e)
        {
            String strTitle = "Delete Files";
            String strMessage = "Are you sure to delete selected File(s)?";
            if (!SystemManager.IsUseDefaultLanguage())
            {
                strTitle = SystemManager.mStringResource.GetText(StringResource.TextType.Drop_Data);
                strMessage = SystemManager.mStringResource.GetText(StringResource.TextType.Drop_Data_Confirm);
            }
            if (MyMessageBox.ShowConfirm(strTitle, strMessage))
            {
                foreach (ListViewItem item in lstData.SelectedItems)
                {
                    MongoDBHelper.DelFile(item.Text);
                }
                RefreshGUI(null, null);
            }
        }
        #endregion

        #region"用户"
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddUserStripButton_Click(object sender, EventArgs e)
        {
            if (IsAdminDB)
            {
                SystemManager.OpenForm(new frmUser(true));
            }
            else
            {
                SystemManager.OpenForm(new frmUser(false));
            }
            RefreshGUI(sender, e);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveUserStripButton_Click(object sender, EventArgs e)
        {
            if (IsAdminDB)
            {
                RemoveUserFromAdminToolStripMenuItem_Click(sender, e);
            }
            else
            {
                RemoveUserToolStripMenuItem_Click(sender, e);
            }
            RefreshGUI(sender, e);
        }
        /// <summary>
        /// Drop User from Admin Group
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveUserFromAdminToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String strTitle = "Drop User";
            String strMessage = "Are you sure to delete user(s) from Admin Group?";
            if (!SystemManager.IsUseDefaultLanguage())
            {
                strTitle = SystemManager.mStringResource.GetText(StringResource.TextType.Drop_User);
                strMessage = SystemManager.mStringResource.GetText(StringResource.TextType.Drop_User_Confirm);
            }

            //这里也可以使用普通的删除数据的方法来删除用户。
            if (MyMessageBox.ShowConfirm(strTitle, strMessage))
            {
                if (tabDataShower.SelectedTab == tabTableView)
                {
                    //lstData
                    foreach (ListViewItem item in lstData.SelectedItems)
                    {
                        MongoDBHelper.RemoveUserFromSvr(item.SubItems[1].Text);
                    }
                    lstData.ContextMenuStrip = null;
                }
                else
                {
                    MongoDBHelper.RemoveUserFromSvr(trvData.DatatreeView.SelectedNode.Tag.ToString());
                    trvData.DatatreeView.ContextMenuStrip = null;
                }
                RefreshGUI(sender, e);
            }
        }
        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String strTitle = "Drop User";
            String strMessage = "Are you sure to delete user(s) from this database";
            if (!SystemManager.IsUseDefaultLanguage())
            {
                strTitle = SystemManager.mStringResource.GetText(StringResource.TextType.Drop_User);
                strMessage = SystemManager.mStringResource.GetText(StringResource.TextType.Drop_User_Confirm);
            }
            if (MyMessageBox.ShowConfirm(strTitle, strMessage))
            {
                if (tabDataShower.SelectedTab == tabTableView)
                {
                    //lstData
                    foreach (ListViewItem item in lstData.SelectedItems)
                    {
                        MongoDBHelper.RemoveUserFromDB(item.SubItems[1].Text);
                    }
                    lstData.ContextMenuStrip = null;
                }
                else
                {
                    MongoDBHelper.RemoveUserFromDB(trvData.DatatreeView.SelectedNode.Tag.ToString());
                    trvData.DatatreeView.ContextMenuStrip = null;
                }
                RemoveUserToolStripMenuItem.Enabled = false;
                RefreshGUI(sender, e);
            }
        }
        /// <summary>
        /// 密码变更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChangePasswordStripButton_Click(object sender, EventArgs e)
        {
            if (mDataViewInfo.strDBTag.EndsWith(MongoDBHelper.DATABASE_NAME_ADMIN + "/" + MongoDBHelper.COLLECTION_NAME_USER))
            {
                SystemManager.OpenForm(new frmUser(true, lstData.SelectedItems[0].SubItems[1].Text));
            }
            else
            {
                SystemManager.OpenForm(new frmUser(false, lstData.SelectedItems[0].SubItems[1].Text));
            }
            RefreshGUI(sender, e);
        }
        #endregion

        #region"数据导航"
        /// <summary>
        /// 换页操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmbRecPerPage_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            switch (cmbRecPerPage.SelectedIndex)
            {
                case 0:
                    mDataViewInfo.LimitCnt = 50;
                    break;
                case 1:
                    mDataViewInfo.LimitCnt = 100;
                    break;
                case 2:
                    mDataViewInfo.LimitCnt = 200;
                    break;
                default:
                    mDataViewInfo.LimitCnt = 100;
                    break;
            }
            ReloadData();
        }
        /// <summary>
        /// 指定起始位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GotoStripButton_Click(object sender, EventArgs e)
        {
            if (txtSkip.Text.IsNumeric())
            {
                int skip = Convert.ToInt32(txtSkip.Text);
                skip--;
                if (skip >= 0)
                {
                    if (mDataViewInfo.CurrentCollectionTotalCnt <= skip)
                    {
                        mDataViewInfo.SkipCnt = mDataViewInfo.CurrentCollectionTotalCnt - 1;
                        if (mDataViewInfo.SkipCnt == -1)
                        {
                            ///CurrentCollectionTotalCnt可能为0
                            mDataViewInfo.SkipCnt = 0;
                        }
                    }
                    else
                    {
                        mDataViewInfo.SkipCnt = skip;
                    }
                    ReloadData();
                }
            }
            else
            {
                txtSkip.Text = string.Empty;
            }
        }
        /// <summary>
        /// 第一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FirstPage_Click(object sender, EventArgs e)
        {
            MongoDBHelper.PageChanged(MongoDBHelper.PageChangeOpr.FirstPage, ref mDataViewInfo, _dataShower);
            SetDataNav();
        }
        /// <summary>
        /// 前一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrePage_Click(object sender, EventArgs e)
        {
            MongoDBHelper.PageChanged(MongoDBHelper.PageChangeOpr.PrePage, ref mDataViewInfo, _dataShower);
            SetDataNav();
        }
        /// <summary>
        /// 下一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextPage_Click(object sender, EventArgs e)
        {
            MongoDBHelper.PageChanged(MongoDBHelper.PageChangeOpr.NextPage, ref mDataViewInfo, _dataShower);
            SetDataNav();
        }
        /// <summary>
        /// 最后页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LastPage_Click(object sender, EventArgs e)
        {
            MongoDBHelper.PageChanged(MongoDBHelper.PageChangeOpr.LastPage, ref mDataViewInfo, _dataShower);
            SetDataNav();
        }
        /// <summary>
        /// 展开所有
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExpandAll_Click(object sender, EventArgs e)
        {
            trvData.DatatreeView.ExpandAll();
        }
        /// <summary>
        /// 折叠所有
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollapseAll_Click(object sender, EventArgs e)
        {
            trvData.DatatreeView.CollapseAll();
        }
        /// <summary>
        /// 清除所有数据
        /// </summary>
        private void clear()
        {
            lstData.Clear();
            txtData.Text = String.Empty;
            trvData.DatatreeView.Nodes.Clear();
            lstData.ContextMenuStrip = null;
            trvData.DatatreeView.ContextMenuStrip = null;
        }

        /// <summary>
        /// 设置导航可用性
        /// </summary>
        private void SetDataNav()
        {

            PrePageStripButton.Enabled = mDataViewInfo.HasPrePage;
            NextPageStripButton.Enabled = mDataViewInfo.HasNextPage;
            FirstPageStripButton.Enabled = mDataViewInfo.HasPrePage;
            LastPageStripButton.Enabled = mDataViewInfo.HasNextPage;
            this.FilterStripButton.Checked = mDataViewInfo.IsUseFilter;
            this.QueryStripButton.Enabled = true;
            String strTitle = "Records";
            if (!SystemManager.IsUseDefaultLanguage())
            {
                strTitle = SystemManager.mStringResource.GetText(StringResource.TextType.Main_Menu_DataView);
            }
            if (mDataViewInfo.CurrentCollectionTotalCnt == 0)
            {
                this.DataNaviToolStripLabel.Text = strTitle + "：0/0";
            }
            else
            {
                this.DataNaviToolStripLabel.Text = strTitle + "：" + (mDataViewInfo.SkipCnt + 1).ToString() + "/" + mDataViewInfo.CurrentCollectionTotalCnt.ToString();
            }
            txtSkip.Text = (mDataViewInfo.SkipCnt + 1).ToString();
        }
        /// <summary>
        /// Refresh Data
        /// </summary>
        public void RefreshGUI(object sender, EventArgs e)
        {
            this.clear();
            mDataViewInfo.SkipCnt = 0;
            SystemManager.SelectObjectTag = mDataViewInfo.strDBTag;
            List<BsonDocument> datalist = MongoDBHelper.GetDataList(ref mDataViewInfo);
            MongoDBHelper.FillDataToControl(datalist, _dataShower, mDataViewInfo);
            InitControlsEnable();
            SetDataNav();
            IsNeedRefresh = false;
        }
        private void ReloadData()
        {
            this.clear();
            SystemManager.SelectObjectTag = mDataViewInfo.strDBTag;
            List<BsonDocument> datalist = MongoDBHelper.GetDataList(ref mDataViewInfo);
            MongoDBHelper.FillDataToControl(datalist, _dataShower, mDataViewInfo);
            SetDataNav();
            IsNeedRefresh = false;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QueryStripButton_Click(object sender, EventArgs e)
        {
            SystemManager.OpenForm(new frmQuery(mDataViewInfo));
            this.FilterStripButton.Enabled = mDataViewInfo.IsUseFilter;
            this.FilterStripButton.Checked = mDataViewInfo.IsUseFilter;
            //重新展示数据
            RefreshGUI(sender, e);
        }
        /// <summary>
        /// 过滤器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FilterStripButton_Click(object sender, EventArgs e)
        {
            mDataViewInfo.IsUseFilter = !mDataViewInfo.IsUseFilter;
            this.FilterStripButton.Checked = mDataViewInfo.IsUseFilter;
            //过滤变更后，重新刷新
            RefreshGUI(sender, e);
        }
        #endregion

    }
}

﻿using System;
using System.Windows.Forms;

namespace MagicMongoDBTool.Module
{
    public partial class ctlMongodump : UserControl
    {
        public EventHandler<TextChangeEventArgs> CommandChanged;
        private MongodbDosCommand.StruMongoDump MongodumpCommand = new MongodbDosCommand.StruMongoDump();
        public ctlMongodump()
        {
            InitializeComponent();
        }

        private void ctlMongodump_Load(object sender, EventArgs e)
        {
            this.ctllogLvT.LoglvChanged += new ctllogLv.LogLvChangedHandler(ctllogLvT_LoglvChanged);
            this.ctlFilePickerOutput.PathChanged += new ctlFilePicker.PathChangedHandler(ctlFilePickerOutput_PathChanged);
            if (!SystemManager.IsUseDefaultLanguage())
            {
                lblCollectionName.Text = SystemManager.mStringResource.GetText(MagicMongoDBTool.Module.StringResource.TextType.DosCommand_Tab_Backup_DCName);
                lblDBName.Text = SystemManager.mStringResource.GetText(MagicMongoDBTool.Module.StringResource.TextType.DosCommand_Tab_Backup_DBName);
                lblHostAddr.Text = SystemManager.mStringResource.GetText(MagicMongoDBTool.Module.StringResource.TextType.DosCommand_Tab_Backup_Server);
                lblPort.Text = SystemManager.mStringResource.GetText(MagicMongoDBTool.Module.StringResource.TextType.DosCommand_Tab_Backup_Port);
                ctlFilePickerOutput.Title = SystemManager.mStringResource.GetText(MagicMongoDBTool.Module.StringResource.TextType.DosCommand_Tab_Backup_Path);
            }
        }
        protected virtual void OnCommandChange(TextChangeEventArgs e)
        {
            e.Raise(this, ref CommandChanged);
        }
        void ctlFilePickerOutput_PathChanged(String FilePath)
        {
            MongodumpCommand.OutPutPath = FilePath;
            OnCommandChange(new TextChangeEventArgs(String.Empty, MongodbDosCommand.GetMongodumpCommandLine(MongodumpCommand)));
        }
        void ctllogLvT_LoglvChanged(MongodbDosCommand.MongologLevel logLv)
        {
            MongodumpCommand.LogLV = logLv;
            OnCommandChange(new TextChangeEventArgs(String.Empty, MongodbDosCommand.GetMongodumpCommandLine(MongodumpCommand)));
        }
        private void txtHostAddr_TextChanged(object sender, EventArgs e)
        {
            MongodumpCommand.HostAddr = txtHostAddr.Text;
            OnCommandChange(new TextChangeEventArgs(String.Empty, MongodbDosCommand.GetMongodumpCommandLine(MongodumpCommand)));
        }

        private void txtDBName_TextChanged(object sender, EventArgs e)
        {
            MongodumpCommand.DBName = txtDBName.Text;
            OnCommandChange(new TextChangeEventArgs(String.Empty, MongodbDosCommand.GetMongodumpCommandLine(MongodumpCommand)));
        }

        private void txtCollectionName_TextChanged(object sender, EventArgs e)
        {
            MongodumpCommand.CollectionName = txtCollectionName.Text;
            OnCommandChange(new TextChangeEventArgs(String.Empty, MongodbDosCommand.GetMongodumpCommandLine(MongodumpCommand)));
        }

        private void numPort_ValueChanged(object sender, EventArgs e)
        {
            MongodumpCommand.Port = (int)numPort.Value;
            OnCommandChange(new TextChangeEventArgs(String.Empty, MongodbDosCommand.GetMongodumpCommandLine(MongodumpCommand)));
        }

    }
}

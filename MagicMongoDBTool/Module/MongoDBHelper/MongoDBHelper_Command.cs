﻿using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MagicMongoDBTool.Module
{
    public static partial class MongoDBHelper
    {

        #region"Collection Command"
        /// <summary>
        /// Compact
        /// </summary>
        /// <see cref="http://www.mongodb.org/display/DOCS/Compact+Command"/>
        public static MongoCommand Compact_Command = new MongoCommand("compact", PathLv.CollectionLV);

        #endregion

        #region"DataBase Command"

        ///数据库命令 http://www.mongodb.org/display/DOCS/List+of+Database+Commands

        /// <summary>
        /// 修复数据库
        /// http://www.mongodb.org/display/DOCS/Durability+and+Repair
        /// </summary>
        public static MongoCommand repairDatabase_Command = new MongoCommand("repairDatabase", PathLv.DatabaseLV);

        #endregion

        #region"Server Command"
        /// <summary>
        /// 服务器状态
        /// http://www.mongodb.org/display/DOCS/serverStatus+Command
        /// </summary>
        public static MongoCommand serverStatus_Command = new MongoCommand("serverStatus", PathLv.ServerLV);

        /// <summary>
        /// 副本状态
        //http://www.mongodb.org/display/DOCS/Replica+Set+Commands
        /// </summary>
        public static MongoCommand replSetGetStatus_Command = new MongoCommand("replSetGetStatus", PathLv.ServerLV);

        /// <summary>
        /// Slave强制同步
        //http://www.mongodb.org/display/DOCS/Master+Slave
        /// </summary>
        public static MongoCommand resync_Command = new MongoCommand("resync", PathLv.ServerLV);

        /// <summary>
        /// 增加数据分片
        /// </summary>
        /// <param name="routeSvr"></param>
        /// <param name="replicaSetName"></param>
        /// <param name="lstAddress"></param>
        /// <remarks>注意：有个命令可能只能用在mongos上面</remarks>
        /// <returns></returns>
        public static CommandResult AddSharding(MongoServer routeSvr, String replicaSetName, List<String> lstAddress, String Name, Decimal MaxSize)
        {
            // replset/host:port,host:port
            String cmdPara = replicaSetName == String.Empty ? String.Empty : (replicaSetName + "/");
            foreach (String item in lstAddress)
            {
                cmdPara += item + ",";
            }
            cmdPara = cmdPara.TrimEnd(",".ToCharArray());
            CommandDocument mongoCmd = new CommandDocument();
            mongoCmd.Add("addshard", cmdPara);
            if (MaxSize != 0)
            {
                mongoCmd.Add("maxSize", (BsonValue)MaxSize);
            }
            if (Name != String.Empty)
            {
                mongoCmd.Add("name", Name);
            }

            return ExecuteMongoSvrCommand(mongoCmd, routeSvr);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routeSvr"></param>
        /// <param name="ShardName"></param>
        /// <returns></returns>
        public static CommandResult RemoveSharding(MongoServer routeSvr, String ShardName)
        {
            CommandDocument mongoCmd = new CommandDocument();
            mongoCmd.Add("removeshard", ShardName);
            return ExecuteMongoSvrCommand(mongoCmd, routeSvr);
        }
        /// <summary>
        /// 数据库分片
        /// </summary>
        /// <param name="routeSvr"></param>
        /// <param name="shardingDB"></param>
        /// <returns></returns>
        public static CommandResult EnableSharding(MongoServer routeSvr, String shardingDB)
        {
            CommandDocument mongoCmd = new CommandDocument();
            mongoCmd = new CommandDocument();
            mongoCmd.Add("enablesharding", shardingDB);
            return ExecuteMongoSvrCommand(mongoCmd, routeSvr);
        }
        /// <summary>
        /// 数据集分片
        /// </summary>
        /// <param name="routeSvr"></param>
        /// <param name="sharingCollection"></param>
        /// <param name="shardingKey"></param>
        /// <returns></returns>
        public static CommandResult ShardCollection(MongoServer routeSvr, String sharingCollection, BsonDocument shardingKey)
        {
            CommandDocument mongoCmd = new CommandDocument();
            mongoCmd.Add("shardcollection", sharingCollection);
            mongoCmd.Add("key", shardingKey);
            return ExecuteMongoSvrCommand(mongoCmd, routeSvr);
        }
        /// <summary>
        /// 初始化副本
        /// </summary>
        /// <param name="mongoSvr">副本组主服务器</param>
        /// <param name="replicaSetName">副本名称</param>
        /// <param name="HostList">从属服务器列表</param>
        public static CommandResult InitReplicaSet(String replicaSetName, String HostList)
        {
            //第一台服务器作为Primary服务器
            MongoServerSettings PrimarySetting = new MongoServerSettings();
            PrimarySetting.Server = new MongoServerAddress(SystemManager.ConfigHelperInstance.ConnectionList[HostList].Host,
                                                           SystemManager.ConfigHelperInstance.ConnectionList[HostList].Port);
            //如果不设置的话，会有错误：不是Primary服务器，SlaveOK 是 False
            PrimarySetting.SlaveOk = true;

            MongoServer PrimarySvr = new MongoServer(PrimarySetting);
            BsonDocument config = new BsonDocument();
            BsonArray hosts = new BsonArray();
            BsonDocument cmd = new BsonDocument();
            BsonDocument host = new BsonDocument();
            //生成命令
            host = new BsonDocument();
            host.Add(KEY_ID, 1);
            host.Add("host", SystemManager.ConfigHelperInstance.ConnectionList[HostList].Host + ":" + SystemManager.ConfigHelperInstance.ConnectionList[HostList].Port.ToString());
            hosts.Add(host);
            config.Add(KEY_ID, replicaSetName);
            config.Add("members", hosts);
            cmd.Add("replSetInitiate", config);

            CommandDocument mongoCmd = new CommandDocument() { cmd };
            return ExecuteMongoSvrCommand(mongoCmd, PrimarySvr);
        }
        #endregion
    }
}

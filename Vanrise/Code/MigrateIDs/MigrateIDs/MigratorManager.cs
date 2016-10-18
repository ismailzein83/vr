using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace MigrateIDs
{
    public enum MigratorOptions
    {
        BusinessEntity = 1,
        RecordType = 2,
        BusinessEntityDefinition = 3,
        BELookupRuleDefinition = 4,
        GenericRuleDefinition = 5,
        DataTransfomationDefinition = 6,
        ServiceType = 7,
        SummaryDataTransfomationDefinition = 8,
        DataRecordStorage = 9,
        AnalyticReport = 10,
        SchedulerTaskTriggerType = 11,
        PartDefinition = 12,
        SchedulerTaskActionType = 13,
        AnalyticItemConfig = 16,
        ExtensibleBEItem = 17,
        ScheduleTask = 18,
        ViewType = 19,
        Setting = 20,
        View = 21,
        DataStore = 22,
        BPBusinessRuleDefinition = 23,
        BPBusinessRuleAction= 24,
        BPDefinition = 25,
        BPTaskType = 26,
        BPBusinessRuleSet = 27,
        ReprocessDefinition =28,
        QueueExecutionFlow = 29,
        QueueInstance =30,
        ExecutionFlowDefinition = 31

    }
    public class MigratorManager:BaseSQLDataManager
    {
        public MigratorManager()
            : base(GetConnectionStringName("MainDBConnStringKey", "ConfigurationDBConnString"))
        {
        }

        public string BuildAddOldColumnQuery(MiratorParameter input)
        {
            StringBuilder stringBuider = new StringBuilder();
            stringBuider.Append(@"
If  EXISTS(SELECT Top 1 DATA_TYPE   FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '#MAINTABLENAME#' AND TABLE_SCHEMA = '#SCHEMANAME#' AND COLUMN_NAME = '#IDCOLUMNNAME#' AND DATA_TYPE !='uniqueidentifier')
    BEGIN
    ALTER TABLE #SCHEMANAME#.[#MAINTABLENAME#]
    ADD Old#IDCOLUMNNAME# #IDCOLUMNTYPE# 
    END 
                ");
            stringBuider.Replace("#MAINTABLENAME#", input.MainTableName);
            stringBuider.Replace("#IDCOLUMNNAME#", input.IDColumnName);
            stringBuider.Replace("#SCHEMANAME#", input.SchemaName);
            stringBuider.Replace("#IDCOLUMNTYPE#", input.IDColumnType);
            return stringBuider.ToString();
        }
        public string BuildUpdateOldIdsQuery(MiratorParameter input)
        {
            StringBuilder stringBuider = new StringBuilder();
            stringBuider.Append(@"
If  EXISTS(SELECT Top 1 DATA_TYPE   FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '#MAINTABLENAME#' AND TABLE_SCHEMA = '#SCHEMANAME#' AND COLUMN_NAME = '#IDCOLUMNNAME#' AND DATA_TYPE !='uniqueidentifier')
    BEGIN
    Update #SCHEMANAME#.[#MAINTABLENAME#] Set Old#IDCOLUMNNAME# = #IDCOLUMNNAME#;
    END ");
            stringBuider.Replace("#MAINTABLENAME#", input.MainTableName);
            stringBuider.Replace("#IDCOLUMNNAME#", input.IDColumnName);
            stringBuider.Replace("#SCHEMANAME#", input.SchemaName);
            return stringBuider.ToString();
        }
        public string BuildDropOldIdAndAddNewOneColumnQuery(MiratorParameter input)
        {
            StringBuilder stringBuider = new StringBuilder();
            stringBuider.Append(@"
If  EXISTS(SELECT Top 1 DATA_TYPE   FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '#MAINTABLENAME#' AND TABLE_SCHEMA = '#SCHEMANAME#' AND COLUMN_NAME = '#IDCOLUMNNAME#' AND DATA_TYPE !='uniqueidentifier')
    BEGIN
     
                ");
            if (input.IsPrimaryColumn)
            {

                
                                stringBuider.Append(@"
                    ALTER TABLE #SCHEMANAME#.[#MAINTABLENAME#]
                    DROP CONSTRAINT pk_#MAINTABLENAME#
                    ALTER TABLE #SCHEMANAME#.[#MAINTABLENAME#]
                    DROP COLUMN  #IDCOLUMNNAME# 
                    ALTER TABLE #SCHEMANAME#.[#MAINTABLENAME#]
                    ADD #IDCOLUMNNAME# uniqueidentifier Not Null DEFAULT(NEWID())

                ALTER TABLE #SCHEMANAME#.[#MAINTABLENAME#]
                ADD CONSTRAINT pk_#MAINTABLENAME# PRIMARY KEY (#IDCOLUMNNAME#)
                ");
            }else
            {
                stringBuider.Append(@"
                ALTER TABLE #SCHEMANAME#.[#MAINTABLENAME#]
                DROP COLUMN  #IDCOLUMNNAME# 

                ALTER TABLE #SCHEMANAME#.[#MAINTABLENAME#]
                ADD #IDCOLUMNNAME# uniqueidentifier 
            ");
            }

            stringBuider.Append(@"
               END
            ");
            stringBuider.Replace("#MAINTABLENAME#", input.MainTableName);
            stringBuider.Replace("#IDCOLUMNNAME#", input.IDColumnName);
            stringBuider.Replace("#SCHEMANAME#", input.SchemaName);
            return stringBuider.ToString();
        }
        public string BuildQuery(MiratorParameter input)
        {
            StringBuilder stringBuider = new StringBuilder();
            stringBuider.Append(@"

                    DECLARE @Cursor CURSOR;
                    DECLARE @Field int;
                    DECLARE @ID uniqueidentifier;
                        BEGIN
                          SET @Cursor = CURSOR FOR
                          select #IDCOLUMNNAME#, #OLDIDCOLUMNNAME# from #SCHEMANAME#.#MAINTABLENAME# order by #OLDIDCOLUMNNAME# desc

                         OPEN @Cursor 
                         FETCH NEXT FROM @Cursor 
                         INTO @ID,@Field

                        WHILE @@FETCH_STATUS = 0
                        BEGIN
                        DECLARE @IDString varchar(100); 
                        set @IDString= CAST(@ID as VARCHAR(36));
                ");
            for (var i = 0; i < input.UpdatedTables.Count;i++ )
            {
                var table = input.UpdatedTables[i];

                string tableName = null;
                if (table.ConnectionStringKey != null)
                {
                    string connectionStringName = ConfigurationManager.AppSettings[table.ConnectionStringKey];
                    if (connectionStringName != null)
                    {
                        string connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
                        if (connectionString != null)
                        {
                            System.Data.SqlClient.SqlConnectionStringBuilder builder = new System.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
                            if (builder != null && builder.InitialCatalog != null)
                                tableName = string.Format("{0}.{1}", builder.InitialCatalog, table.TableName);
                        }

                    }
                }
                else
                {
                    tableName = table.TableName;
                }
              
                if(tableName != null)
                {
                     stringBuider.Append(@"
                       DECLARE @String#INDEX# varchar(100); 
                        ");

                    if (table.UseQuotationIdentifier)
                    {
                        stringBuider.Append(@"
                       set @String#INDEX# = N'#IDENTIFIER#:""'+CAST(@Field as nvarchar(30))+'""';
                        "); 
                    }else
                    {
                        stringBuider.Append(@"
                       set @String#INDEX# = N'#IDENTIFIER#:'+CAST(@Field as nvarchar(30))+'';
                        "); 
                    }
                    
                    stringBuider.Append(@"DECLARE @ReplaceString#INDEX# varchar(100);
	                   set @ReplaceString#INDEX# = N'#IDENTIFIER#:""'+@IDString+'""';
	                        UPDATE  #TABLENAME# 
	                        SET    #COLUMNNAME# = replace( #COLUMNNAME# , @String#INDEX# , @ReplaceString#INDEX# )
	                        from #TABLENAME# 
	                        where  #COLUMNNAME# like '%'+ @String#INDEX# +'%'
                    ");
                    stringBuider.Replace("#TABLENAME# ", tableName);
                    stringBuider.Replace("#IDENTIFIER#", table.Identifier);
                    stringBuider.Replace("#COLUMNNAME#", table.ColumnName);
                    stringBuider.Replace("#INDEX#", i.ToString());
                }
              
            }
            stringBuider.Append(@"
                     FETCH NEXT FROM @Cursor
                            INTO @ID,@Field 
                            END; 

                             CLOSE @Cursor ;
                            DEALLOCATE @Cursor;
                        END;  
                    ");
            stringBuider.Replace("#SCHEMANAME#", input.SchemaName);
            stringBuider.Replace("#MAINTABLENAME#", input.MainTableName);
            stringBuider.Replace("#OLDIDCOLUMNNAME#", input.OldColumnName);
            stringBuider.Replace("#IDCOLUMNNAME#", input.IDColumnName);
            return stringBuider.ToString();
        }
        public bool Migrate(string query)
        {
            ExecuteNonQueryText(query,null);
            return true;
        }
    }

}

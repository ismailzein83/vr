CREATE TABLE [CRMFixedOper].[WorkOrderGroup] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [WorkOrderTypeID]  UNIQUEIDENTIFIER NULL,
    [AreaID]           BIGINT           NULL,
    [GroupID]          INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_WorkOrderGroup] PRIMARY KEY CLUSTERED ([ID] ASC)
);


CREATE TABLE [NIM].[PhoneNumber] (
    [Id]               BIGINT           IDENTITY (1, 1) NOT NULL,
    [PhoneNumber]      NVARCHAR (255)   NULL,
    [StatusID]         UNIQUEIDENTIFIER NULL,
    [CategoryID]       UNIQUEIDENTIFIER NULL,
    [NodeID]           BIGINT           NULL,
    [LocalAreaCodeID]  BIGINT           NULL,
    [NodeLACID]        BIGINT           NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


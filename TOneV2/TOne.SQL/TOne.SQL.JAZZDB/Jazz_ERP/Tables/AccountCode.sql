CREATE TABLE [Jazz_ERP].[AccountCode] (
    [ID]                UNIQUEIDENTIFIER NOT NULL,
    [Name]              NVARCHAR (255)   NULL,
    [SwitchId]          INT              NULL,
    [TransactionTypeId] UNIQUEIDENTIFIER NULL,
    [Code]              VARCHAR (40)     NULL,
    [CreatedBy]         INT              NULL,
    [CreatedTime]       DATETIME         CONSTRAINT [DF_AccountCode_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedBy]    INT              NULL,
    [LastModifiedTime]  DATETIME         NULL,
    [Carriers]          NVARCHAR (MAX)   NULL,
    [timestamp]         ROWVERSION       NULL,
    CONSTRAINT [PK_AccountCode] PRIMARY KEY CLUSTERED ([ID] ASC)
);


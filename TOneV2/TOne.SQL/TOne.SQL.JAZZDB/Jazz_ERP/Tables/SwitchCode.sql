CREATE TABLE [Jazz_ERP].[SwitchCode] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Code]             VARCHAR (40)     NULL,
    [CeatedBy]         INT              NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_SwitchCode_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    [SwitchId]         INT              NULL,
    [Name]             NVARCHAR (255)   NULL,
    CONSTRAINT [PK_SwitchCode] PRIMARY KEY CLUSTERED ([ID] ASC)
);


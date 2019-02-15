CREATE TABLE [Jazz_ERP].[CustomerType] (
    [ID]              UNIQUEIDENTIFIER NOT NULL,
    [Name]            NVARCHAR (255)   NULL,
    [Code]            VARCHAR (40)     NULL,
    [CreatedBy]       INT              NULL,
    [CreatedTime]     DATETIME         CONSTRAINT [DF_CustomerType_CreatedTime] DEFAULT (getdate()) NULL,
    [LastCreatedBy]   INT              NULL,
    [LastCreatedTime] DATETIME         NULL,
    [timestamp]       ROWVERSION       NULL,
    CONSTRAINT [PK_CustomerType] PRIMARY KEY CLUSTERED ([ID] ASC)
);


CREATE TABLE [Jazz_ERP].[Market] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [Code]             VARCHAR (40)     NULL,
    [ProductServiceId] UNIQUEIDENTIFIER NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedBy]   INT              NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_Market_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);




CREATE TABLE [Jazz_ERP].[Region] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [Code]             VARCHAR (40)     NULL,
    [CreatedBy]        INT              NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_Region_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_Region] PRIMARY KEY CLUSTERED ([ID] ASC)
);


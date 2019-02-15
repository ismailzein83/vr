CREATE TABLE [Jazz_ERP].[TaxCode] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [SwitchId]         INT              NULL,
    [Code]             VARCHAR (40)     NULL,
    [Direction]        INT              NULL,
    [CreatedBy]        INT              NOT NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_Tax_CreatedTime] DEFAULT (getdate()) NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_TaxCode] PRIMARY KEY CLUSTERED ([ID] ASC)
);


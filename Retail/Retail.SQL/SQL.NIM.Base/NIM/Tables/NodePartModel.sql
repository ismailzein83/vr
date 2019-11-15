CREATE TABLE [NIM].[NodePartModel] (
    [ID]               INT              IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [NodePartTypeID]   UNIQUEIDENTIFIER NULL,
    [VendorID]         BIGINT           NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_NodePartModel] PRIMARY KEY CLUSTERED ([ID] ASC)
);


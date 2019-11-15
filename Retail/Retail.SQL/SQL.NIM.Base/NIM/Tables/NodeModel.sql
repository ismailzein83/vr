CREATE TABLE [NIM].[NodeModel] (
    [ID]               INT              IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [NodeTypeID]       UNIQUEIDENTIFIER NULL,
    [VendorID]         BIGINT           NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_NodeModel] PRIMARY KEY CLUSTERED ([ID] ASC)
);


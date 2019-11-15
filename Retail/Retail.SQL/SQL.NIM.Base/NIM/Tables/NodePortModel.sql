CREATE TABLE [NIM].[NodePortModel] (
    [ID]               INT              IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [TypeID]           UNIQUEIDENTIFIER NULL,
    [VendorID]         BIGINT           NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    PRIMARY KEY CLUSTERED ([ID] ASC)
);


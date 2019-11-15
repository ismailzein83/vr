CREATE TABLE [NIM].[ConnectionModel] (
    [ID]               INT              IDENTITY (1, 1) NOT NULL,
    [Name]             NVARCHAR (255)   NULL,
    [TypeID]           UNIQUEIDENTIFIER NULL,
    [VendorID]         BIGINT           NULL,
    [CreatedTime]      DATETIME         NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK__Connecti__3214EC2760C757A0] PRIMARY KEY CLUSTERED ([ID] ASC)
);


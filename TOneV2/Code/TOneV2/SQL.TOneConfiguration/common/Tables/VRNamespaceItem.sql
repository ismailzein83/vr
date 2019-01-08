CREATE TABLE [common].[VRNamespaceItem] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [VRNamespaceId]    UNIQUEIDENTIFIER NOT NULL,
    [Name]             VARCHAR (255)    NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_VRNamespaceItem_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    [LastModifiedBy]   INT              NULL,
    [timestamp]        ROWVERSION       NULL,
    CONSTRAINT [PK_VRNamespaceItem] PRIMARY KEY CLUSTERED ([ID] ASC)
);


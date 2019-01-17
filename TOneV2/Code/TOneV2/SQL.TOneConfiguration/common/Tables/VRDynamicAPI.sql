CREATE TABLE [common].[VRDynamicAPI] (
    [ID]               UNIQUEIDENTIFIER NOT NULL,
    [Name]             VARCHAR (255)    NULL,
    [ModuleId]         UNIQUEIDENTIFIER NULL,
    [Settings]         NVARCHAR (MAX)   NULL,
    [timestamp]        ROWVERSION       NULL,
    [CreatedTime]      DATETIME         CONSTRAINT [DF_VRDynamicAPI_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT              NULL,
    [LastModifiedBy]   INT              NULL,
    [LastModifiedTime] DATETIME         NULL,
    CONSTRAINT [PK_VRDynamicAPI] PRIMARY KEY CLUSTERED ([ID] ASC)
);




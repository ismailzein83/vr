CREATE TABLE [common].[VREventHandler] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NOT NULL,
    [Settings]    NVARCHAR (MAX)   NOT NULL,
    [BED]         DATETIME         NOT NULL,
    [EED]         DATETIME         NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_VREventHandler_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION       NULL,
    CONSTRAINT [PK_VREventHandler] PRIMARY KEY CLUSTERED ([ID] ASC)
);


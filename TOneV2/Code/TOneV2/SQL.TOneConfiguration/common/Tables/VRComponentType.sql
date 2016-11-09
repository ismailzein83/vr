CREATE TABLE [common].[VRComponentType] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NOT NULL,
    [ConfigID]    UNIQUEIDENTIFIER NOT NULL,
    [Settings]    NVARCHAR (MAX)   NOT NULL,
    [timestamp]   ROWVERSION       NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_VRComponentType_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_VRComponentType] PRIMARY KEY CLUSTERED ([ID] ASC)
);


CREATE TABLE [common].[VRAppVisibility] (
    [ID]          UNIQUEIDENTIFIER NOT NULL,
    [Name]        NVARCHAR (255)   NOT NULL,
    [Settings]    NVARCHAR (MAX)   NULL,
    [IsCurrent]   BIT              NULL,
    [CreatedTime] DATETIME         CONSTRAINT [DF_VRAppVisibility_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION       NULL,
    CONSTRAINT [PK_VRAppVisibility] PRIMARY KEY CLUSTERED ([ID] ASC)
);




CREATE TABLE [Retail].[Package] (
    [ID]          INT             IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255)  NULL,
    [Description] NVARCHAR (1000) NULL,
    [Settings]    NVARCHAR (MAX)  NULL,
    [CreatedTime] DATETIME        CONSTRAINT [DF_Package_CreatedTime] DEFAULT (getdate()) NULL,
    [timestamp]   ROWVERSION      NULL,
    CONSTRAINT [PK_Package] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_Package_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);




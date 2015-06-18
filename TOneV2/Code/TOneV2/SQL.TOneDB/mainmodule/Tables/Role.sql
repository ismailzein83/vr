CREATE TABLE [mainmodule].[Role] (
    [ID]          INT             IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255)  NOT NULL,
    [Description] NVARCHAR (1000) NULL,
    [CreatedTime] DATETIME        CONSTRAINT [DF_Role_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Role_1] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [IX_Table_Name] UNIQUE NONCLUSTERED ([Name] ASC)
);


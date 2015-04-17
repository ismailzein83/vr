CREATE TABLE [dbo].[SysParameters] (
    [ID]             INT            NOT NULL,
    [Name]           NVARCHAR (50)  NOT NULL,
    [Description]    NVARCHAR (500) NULL,
    [Value]          NVARCHAR (MAX) NOT NULL,
    [ValueTypeID]    INT            NOT NULL,
    [LastUpdateDate] DATETIME2 (0)  CONSTRAINT [DF_Table_1_CreationDate] DEFAULT (getdate()) NOT NULL,
    [LastUpdatedBy]  INT            NOT NULL,
    CONSTRAINT [PK_SysParameters] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SysParameters_ValueTypes] FOREIGN KEY ([ValueTypeID]) REFERENCES [dbo].[ValueTypes] ([ID]),
    CONSTRAINT [FK_SystemParameters_Users] FOREIGN KEY ([LastUpdatedBy]) REFERENCES [dbo].[Users] ([ID])
);


CREATE TABLE [dbo].[CodeGroup] (
    [Code]       VARCHAR (20)   NOT NULL,
    [Name]       NVARCHAR (100) NOT NULL,
    [UserID]     INT            NULL,
    [timestamp]  ROWVERSION     NULL,
    [DS_ID_auto] INT            IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_CodeGroup] PRIMARY KEY CLUSTERED ([Code] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ZoneGroup_Code]
    ON [dbo].[CodeGroup]([Code] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_ZoneGroup_Name]
    ON [dbo].[CodeGroup]([Name] ASC);


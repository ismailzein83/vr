CREATE TABLE [dbo].[CarrierGroup] (
    [CarrierGroupID]   INT            IDENTITY (1, 1) NOT NULL,
    [CarrierGroupName] NVARCHAR (255) NOT NULL,
    [ParentID]         INT            NULL,
    [ParentPath]       VARCHAR (255)  NULL,
    [Path]             AS             (case when [parentID] IS NULL then [CarrierGroupName] else ([ParentPath]+'/')+[CarrierGroupName] end),
    [timestamp]        ROWVERSION     NOT NULL,
    CONSTRAINT [PK_CarrierGroup] PRIMARY KEY CLUSTERED ([CarrierGroupID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_CarrierGroup]
    ON [dbo].[CarrierGroup]([ParentPath] ASC);


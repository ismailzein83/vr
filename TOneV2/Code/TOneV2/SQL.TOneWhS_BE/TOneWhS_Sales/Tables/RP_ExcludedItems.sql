CREATE TABLE [TOneWhS_Sales].[RP_ExcludedItems] (
    [ID]                BIGINT         IDENTITY (1, 1) NOT NULL,
    [ItemID]            NVARCHAR (50)  NOT NULL,
    [ItemType]          INT            NOT NULL,
    [ItemName]          NVARCHAR (MAX) NOT NULL,
    [Description]       NVARCHAR (MAX) NOT NULL,
    [ParentId]          INT            NULL,
    [ProcessInstanceId] BIGINT         NOT NULL,
    CONSTRAINT [PK_RP_ExcludedItems] PRIMARY KEY CLUSTERED ([ID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_ProcessInstanceId]
    ON [TOneWhS_Sales].[RP_ExcludedItems]([ProcessInstanceId] ASC);


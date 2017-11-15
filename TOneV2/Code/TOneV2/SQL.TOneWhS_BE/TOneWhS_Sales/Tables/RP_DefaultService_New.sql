CREATE TABLE [TOneWhS_Sales].[RP_DefaultService_New] (
    [ID]                BIGINT         NULL,
    [ProcessInstanceID] BIGINT         NOT NULL,
    [Services]          NVARCHAR (MAX) NOT NULL,
    [BED]               DATETIME       NOT NULL,
    [EED]               DATETIME       NULL
);




GO
CREATE CLUSTERED INDEX [IX_RP_DefaultService_New_ProcessInstanceID]
    ON [TOneWhS_Sales].[RP_DefaultService_New]([ProcessInstanceID] ASC);


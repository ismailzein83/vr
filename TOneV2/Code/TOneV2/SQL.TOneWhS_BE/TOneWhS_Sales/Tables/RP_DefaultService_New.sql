CREATE TABLE [TOneWhS_Sales].[RP_DefaultService_New] (
    [ID]                BIGINT         NULL,
    [ProcessInstanceID] BIGINT         NOT NULL,
    [Services]          NVARCHAR (MAX) NOT NULL,
    [BED]               DATETIME       NOT NULL,
    [EED]               DATETIME       NULL
);


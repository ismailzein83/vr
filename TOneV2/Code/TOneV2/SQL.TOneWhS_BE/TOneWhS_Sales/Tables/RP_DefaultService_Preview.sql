CREATE TABLE [TOneWhS_Sales].[RP_DefaultService_Preview] (
    [ProcessInstanceID]         BIGINT         NOT NULL,
    [CurrentServices]           NVARCHAR (MAX) NULL,
    [IsCurrentServiceInherited] BIT            NULL,
    [NewServices]               NVARCHAR (MAX) NULL,
    [EffectiveOn]               DATETIME       NOT NULL,
    [EffectiveUntil]            DATETIME       NULL
);




GO
CREATE CLUSTERED INDEX [IX_RP_DefaultService_Preview_ProcessInstanceID]
    ON [TOneWhS_Sales].[RP_DefaultService_Preview]([ProcessInstanceID] ASC);


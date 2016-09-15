CREATE TABLE [TOneWhS_Sales].[RP_DefaultService_Preview] (
    [ProcessInstanceID]         BIGINT         NOT NULL,
    [CurrentServices]           NVARCHAR (MAX) NULL,
    [IsCurrentServiceInherited] BIT            NULL,
    [NewServices]               NVARCHAR (MAX) NULL,
    [EffectiveOn]               DATETIME       NOT NULL,
    [EffectiveUntil]            DATETIME       NULL
);


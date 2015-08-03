CREATE TABLE [dbo].[SaleZoneException] (
    [Id]               INT         IDENTITY (1, 1) NOT NULL,
    [ExceptionTypeId]  INT         NOT NULL,
    [CarrierAccountId] VARCHAR (5) NOT NULL,
    [ZoneId]           INT         NOT NULL,
    CONSTRAINT [PK_ZoneLockOnAndExclusion] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_CarrierAccountId_ZoneId]
    ON [dbo].[SaleZoneException]([CarrierAccountId] ASC, [ZoneId] ASC);


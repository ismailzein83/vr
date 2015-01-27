CREATE TABLE [dbo].[Zone] (
    [ZoneID]             INT            IDENTITY (1, 1) NOT NULL,
    [CodeGroup]          VARCHAR (20)   NULL,
    [Name]               NVARCHAR (255) NOT NULL,
    [SupplierID]         VARCHAR (5)    NOT NULL,
    [ServicesFlag]       SMALLINT       NULL,
    [IsMobile]           CHAR (1)       NULL,
    [IsProper]           CHAR (1)       NULL,
    [IsSold]             CHAR (1)       NULL,
    [BeginEffectiveDate] SMALLDATETIME  NOT NULL,
    [EndEffectiveDate]   SMALLDATETIME  NULL,
    [IsEffective]        AS             (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
    [UserID]             INT            NULL,
    [timestamp]          ROWVERSION     NULL,
    CONSTRAINT [PK_Zone] PRIMARY KEY CLUSTERED ([ZoneID] ASC),
    CONSTRAINT [FK_Zone_CarrierAccount] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_Zone_CodeGroup]
    ON [dbo].[Zone]([CodeGroup] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Zone_Name]
    ON [dbo].[Zone]([Name] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Zone_Dates]
    ON [dbo].[Zone]([BeginEffectiveDate] DESC, [EndEffectiveDate] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_Zone_SupplierID]
    ON [dbo].[Zone]([SupplierID] ASC);


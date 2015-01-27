CREATE TABLE [dbo].[Code] (
    [ID]                 BIGINT        IDENTITY (1, 1) NOT NULL,
    [Code]               VARCHAR (20)  NOT NULL,
    [ZoneID]             INT           NOT NULL,
    [BeginEffectiveDate] SMALLDATETIME NOT NULL,
    [EndEffectiveDate]   SMALLDATETIME NULL,
    [IsEffective]        AS            (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
    [UserID]             INT           NULL,
    [timestamp]          ROWVERSION    NULL,
    CONSTRAINT [PK_Code] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Code_Zone] FOREIGN KEY ([ZoneID]) REFERENCES [dbo].[Zone] ([ZoneID])
);


GO
CREATE NONCLUSTERED INDEX [IX_Code_EED]
    ON [dbo].[Code]([EndEffectiveDate] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_Code_BED]
    ON [dbo].[Code]([BeginEffectiveDate] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_Code]
    ON [dbo].[Code]([Code] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Code_ZoneID]
    ON [dbo].[Code]([ZoneID] ASC);


CREATE TABLE [dbo].[RouteBlock] (
    [RouteBlockID]        INT           IDENTITY (1, 1) NOT NULL,
    [CustomerID]          VARCHAR (5)   NULL,
    [SupplierID]          VARCHAR (5)   NULL,
    [ZoneID]              INT           NULL,
    [Code]                VARCHAR (30)  NULL,
    [BeginEffectiveDate]  SMALLDATETIME NULL,
    [EndEffectiveDate]    SMALLDATETIME NULL,
    [BlockType]           TINYINT       NULL,
    [IsEffective]         AS            (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
    [UserID]              INT           NULL,
    [timestamp]           ROWVERSION    NULL,
    [Reason]              VARCHAR (250) NULL,
    [RouteChangeHeaderID] INT           NULL,
    [UpdateDate]          SMALLDATETIME NULL,
    [IncludeSubCodes]     CHAR (1)      CONSTRAINT [DF_RouteBlock_IncludeSubCodes] DEFAULT ('N') NULL,
    [ExcludedCodes]       VARCHAR (250) NULL,
    CONSTRAINT [PK_RouteBlock] PRIMARY KEY CLUSTERED ([RouteBlockID] ASC),
    CONSTRAINT [FK_RouteBlock_CarrierAccount] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID]),
    CONSTRAINT [FK_RouteBlock_CarrierAccount1] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
);


GO
CREATE NONCLUSTERED INDEX [IX_RouteBlock_Zone]
    ON [dbo].[RouteBlock]([ZoneID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RouteBlock_Dates]
    ON [dbo].[RouteBlock]([BeginEffectiveDate] DESC, [EndEffectiveDate] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_RouteBlock_Supplier]
    ON [dbo].[RouteBlock]([SupplierID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_RouteBlock_Customer]
    ON [dbo].[RouteBlock]([CustomerID] ASC);


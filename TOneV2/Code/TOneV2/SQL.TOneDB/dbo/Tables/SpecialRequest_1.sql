CREATE TABLE [dbo].[SpecialRequest] (
    [SpecialRequestID]    INT           IDENTITY (1, 1) NOT NULL,
    [CustomerID]          VARCHAR (5)   NOT NULL,
    [ZoneID]              INT           NULL,
    [Code]                VARCHAR (30)  NULL,
    [SupplierID]          VARCHAR (5)   NOT NULL,
    [Priority]            TINYINT       NULL,
    [NumberOfTries]       TINYINT       NULL,
    [SpecialRequestType]  TINYINT       NOT NULL,
    [BeginEffectiveDate]  SMALLDATETIME NOT NULL,
    [EndEffectiveDate]    SMALLDATETIME NULL,
    [IsEffective]         AS            (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
    [UserID]              INT           NULL,
    [timestamp]           ROWVERSION    NULL,
    [Percentage]          TINYINT       NULL,
    [Reason]              VARCHAR (250) NULL,
    [RouteChangeHeaderID] INT           NULL,
    [IncludeSubCodes]     CHAR (1)      CONSTRAINT [DF_SpecialRequest_IncludeSubCodes] DEFAULT ('N') NULL,
    [ExcludedCodes]       VARCHAR (250) NULL,
    CONSTRAINT [PK_SpecialRequest] PRIMARY KEY CLUSTERED ([SpecialRequestID] ASC),
    CONSTRAINT [FK_SpecialRequest_CarrierAccount] FOREIGN KEY ([CustomerID]) REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID]),
    CONSTRAINT [FK_SpecialRequest_CarrierAccount1] FOREIGN KEY ([SupplierID]) REFERENCES [dbo].[CarrierAccount] ([CarrierAccountID])
);


GO
CREATE NONCLUSTERED INDEX [IX_SpecialRequest]
    ON [dbo].[SpecialRequest]([BeginEffectiveDate] DESC, [EndEffectiveDate] DESC);


GO
CREATE NONCLUSTERED INDEX [IX_SpecialRequest_Customer]
    ON [dbo].[SpecialRequest]([CustomerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SpecialRequest_Supplier]
    ON [dbo].[SpecialRequest]([SupplierID] ASC);


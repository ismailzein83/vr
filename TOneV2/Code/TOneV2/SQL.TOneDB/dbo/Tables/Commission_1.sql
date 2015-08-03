CREATE TABLE [dbo].[Commission] (
    [CommissionID]       INT            IDENTITY (1, 1) NOT NULL,
    [SupplierID]         VARCHAR (10)   NOT NULL,
    [CustomerID]         VARCHAR (10)   NOT NULL,
    [ZoneID]             INT            NULL,
    [FromRate]           DECIMAL (9, 5) NULL,
    [ToRate]             DECIMAL (9, 5) NULL,
    [Percentage]         DECIMAL (9, 5) NULL,
    [Amount]             DECIMAL (9, 5) NULL,
    [BeginEffectiveDate] SMALLDATETIME  NULL,
    [EndEffectiveDate]   SMALLDATETIME  NULL,
    [IsExtraCharge]      CHAR (1)       CONSTRAINT [DF_Commission_IsExtraCharges] DEFAULT ('N') NULL,
    [IsEffective]        AS             (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
    [UserID]             INT            NULL,
    [timestamp]          ROWVERSION     NULL,
    CONSTRAINT [PK_Commission] PRIMARY KEY CLUSTERED ([CommissionID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_Commission_Customer]
    ON [dbo].[Commission]([CustomerID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Commission_Zone]
    ON [dbo].[Commission]([ZoneID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Commission_Supplier]
    ON [dbo].[Commission]([SupplierID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_Commission_Dates]
    ON [dbo].[Commission]([BeginEffectiveDate] DESC, [EndEffectiveDate] DESC);


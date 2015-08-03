CREATE TABLE [dbo].[ZebraCommission] (
    [CommissionID]       INT            IDENTITY (1, 1) NOT NULL,
    [SupplierID]         VARCHAR (5)    NOT NULL,
    [FromRate]           DECIMAL (9, 5) NULL,
    [ToRate]             DECIMAL (9, 5) NULL,
    [IsPercentage]       BIT            NULL,
    [Amount]             DECIMAL (9, 5) NULL,
    [BeginEffectiveDate] SMALLDATETIME  NULL,
    [EndEffectiveDate]   SMALLDATETIME  NULL,
    [IsActive]           BIT            CONSTRAINT [DF_Table_1_IsExtraCharge] DEFAULT ('N') NULL,
    [IsEffective]        AS             (case when getdate()>=[BeginEffectiveDate] AND getdate()<=isnull([EndEffectiveDate],'2020-01-01') then 'Y' else 'N' end),
    [UserID]             INT            NULL,
    [CreationDate]       SMALLDATETIME  NULL,
    [LastUpdated]        SMALLDATETIME  NULL,
    CONSTRAINT [PK_ZebraCommission] PRIMARY KEY CLUSTERED ([CommissionID] ASC)
);


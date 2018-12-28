CREATE TABLE [RA_ICX].[OperatorDeclaration] (
    [ID]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [OperatorId]       BIGINT          NULL,
    [TotalVolumeInMin] DECIMAL (20, 4) NULL,
    [TotalRevenue]     DECIMAL (20, 4) NULL,
    [CreatedTime]      DATETIME        NULL,
    [CreatedBy]        INT             NULL,
    [LastModifiedTime] DATETIME        NULL,
    [LastModifiedBy]   INT             NULL,
    [Services]         VARCHAR (MAX)   NULL,
    [Period]           INT             NULL,
    [CurrencyID]       INT             NULL,
    CONSTRAINT [PK_OperatorDeclarationICX] PRIMARY KEY CLUSTERED ([ID] ASC)
);


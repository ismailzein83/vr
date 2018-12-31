CREATE TABLE [RA_INTL].[OperatorDeclaration] (
    [ID]               BIGINT          IDENTITY (1, 1) NOT NULL,
    [OperatorID]       BIGINT          NULL,
    [TotalVolumeInMin] DECIMAL (20, 4) NULL,
    [TotalRevenue]     DECIMAL (20, 4) NULL,
    [CreatedTime]      DATETIME        CONSTRAINT [DF_OperatorDeclaration_CreatedTime] DEFAULT (getdate()) NULL,
    [CreatedBy]        INT             NULL,
    [LastModifiedTime] DATETIME        NULL,
    [LastModifiedBy]   INT             NULL,
    [timestamp]        ROWVERSION      NULL,
    [Services]         VARCHAR (MAX)   NULL,
    [Period]           INT             NULL,
    [CurrencyID]       INT             NULL,
    CONSTRAINT [PK_OperatorDeclaration] PRIMARY KEY CLUSTERED ([ID] ASC)
);




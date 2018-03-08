CREATE TABLE [TOneWhS_BE].[ANumberSupplierCode] (
    [ID]             BIGINT       NOT NULL,
    [ANumberGroupID] INT          NOT NULL,
    [SupplierID]     INT          NOT NULL,
    [Code]           VARCHAR (20) NOT NULL,
    [BED]            DATETIME     NOT NULL,
    [EED]            DATETIME     NULL,
    [timestamp]      ROWVERSION   NULL,
    [CreatedTime]    DATETIME     CONSTRAINT [DF_ANumberSupplierCode_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_ANumberSupplierCode] PRIMARY KEY CLUSTERED ([ID] ASC)
);


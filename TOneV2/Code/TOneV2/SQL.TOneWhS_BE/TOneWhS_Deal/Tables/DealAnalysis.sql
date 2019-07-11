CREATE TABLE [TOneWhS_Deal].[DealAnalysis] (
    [Id]               INT            IDENTITY (1, 1) NOT NULL,
    [CarrierId]        INT            NULL,
    [Name]             NVARCHAR (255) NULL,
    [SwapDealId]       INT            NULL,
    [FromDate]         DATETIME       NULL,
    [ToDate]           DATETIME       NULL,
    [Inbounds]         NVARCHAR (MAX) NULL,
    [Outbounds]        NVARCHAR (MAX) NULL,
    [AnalysisResult]   NVARCHAR (MAX) NULL,
    [LastModifiedTime] DATETIME       NULL,
    [CreatedTime]      DATETIME       NULL,
    [LastModifiedBy]   INT            NULL,
    [CreatedBy]        INT            NULL,
    [timestamp]        ROWVERSION     NULL,
    CONSTRAINT [PK__DealAnal__3214EC077510A974] PRIMARY KEY CLUSTERED ([Id] ASC)
);






CREATE TABLE [dbo].[RePricingQueue] (
    [ID]                   INT           IDENTITY (1, 1) NOT NULL,
    [QueueDate]            DATETIME      CONSTRAINT [DF_RePricingQueue_QueueDate] DEFAULT (getdate()) NOT NULL,
    [From]                 DATETIME      NOT NULL,
    [To]                   DATETIME      NOT NULL,
    [CustomerID]           VARCHAR (5)   NULL,
    [SupplierID]           VARCHAR (5)   NULL,
    [BatchSize]            INT           CONSTRAINT [DF_RePricingQueue_BatchSize] DEFAULT ((20000)) NULL,
    [DailyChunks]          INT           CONSTRAINT [DF_RePricingQueue_DailyChunks] DEFAULT ((1)) NULL,
    [GenerateTrafficStats] CHAR (1)      CONSTRAINT [DF_RePricingQueue_GenerateTrafficStats] DEFAULT ('Y') NULL,
    [IsFinished]           CHAR (1)      CONSTRAINT [DF_RePricingQueue_IsFinished] DEFAULT ('N') NULL,
    [CurrentProcessing]    CHAR (1)      CONSTRAINT [DF_RePricingQueue_UnderProcessing] DEFAULT ('N') NULL,
    [User]                 NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_RePricingQueue] PRIMARY KEY CLUSTERED ([ID] ASC)
);


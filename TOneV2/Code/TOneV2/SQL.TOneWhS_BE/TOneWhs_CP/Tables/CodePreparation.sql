CREATE TABLE [TOneWhs_CP].[CodePreparation] (
    [ID]                  INT            IDENTITY (1, 1) NOT NULL,
    [SellingNumberPlanId] INT            NOT NULL,
    [Changes]             NVARCHAR (MAX) NOT NULL,
    [Status]              INT            NOT NULL,
    [CreatedTime]         DATETIME       CONSTRAINT [DF_CodePreparation_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_CodePreparation] PRIMARY KEY CLUSTERED ([ID] ASC)
);




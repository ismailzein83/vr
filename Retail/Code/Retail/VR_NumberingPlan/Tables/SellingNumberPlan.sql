CREATE TABLE [VR_NumberingPlan].[SellingNumberPlan] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (255) NOT NULL,
    [timestamp]   ROWVERSION     NULL,
    [CreatedTime] DATETIME       CONSTRAINT [DF_SellingNumberPlan_CreatedTime] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_SellingNumberPlan] PRIMARY KEY CLUSTERED ([ID] ASC)
);


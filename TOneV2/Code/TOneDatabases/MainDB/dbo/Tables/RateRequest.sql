CREATE TABLE [dbo].[RateRequest] (
    [ID]                     INT        IDENTITY (1, 1) NOT NULL,
    [RateRequestStatus]      INT        NOT NULL,
    [ApprovedRate]           FLOAT (53) NULL,
    [RequestedRate]          FLOAT (53) NOT NULL,
    [CurrentRate]            FLOAT (53) NULL,
    [OwnerApprovalRequestID] INT        NULL,
    [ZoneID]                 INT        NOT NULL,
    [EvaluatorRuleID]        INT        NOT NULL,
    CONSTRAINT [PK_RateRequest] PRIMARY KEY CLUSTERED ([ID] ASC)
);


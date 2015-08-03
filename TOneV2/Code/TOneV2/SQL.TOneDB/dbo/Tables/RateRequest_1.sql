CREATE TABLE [dbo].[RateRequest] (
    [ID]                     INT            IDENTITY (1, 1) NOT NULL,
    [RateRequestStatus]      INT            NOT NULL,
    [ApprovedRate]           FLOAT (53)     NULL,
    [RequestedRate]          FLOAT (53)     NOT NULL,
    [CurrentRate]            FLOAT (53)     NULL,
    [OwnerApprovalRequestID] INT            NULL,
    [ZoneID]                 INT            NOT NULL,
    [EvaluatorRuleID]        INT            NOT NULL,
    [Note]                   NVARCHAR (250) CONSTRAINT [DF_RateRequest_Note] DEFAULT ('') NULL,
    CONSTRAINT [PK_RateRequest] PRIMARY KEY CLUSTERED ([ID] ASC)
);


CREATE TABLE [dbo].[ApprovalRequest] (
    [ID]                    INT            IDENTITY (1, 1) NOT NULL,
    [Date]                  DATETIME       NOT NULL,
    [Status]                INT            NOT NULL,
    [OriginatorAmuID]       INT            NOT NULL,
    [AssignedToAmuID]       INT            NOT NULL,
    [ForwarderAmuID]        INT            NULL,
    [RatePlanID]            INT            NOT NULL,
    [Note]                  NVARCHAR (250) NULL,
    [Lasttimeoutactiondate] DATETIME       NULL,
    CONSTRAINT [PK_ApprovalRequest] PRIMARY KEY CLUSTERED ([ID] ASC)
);


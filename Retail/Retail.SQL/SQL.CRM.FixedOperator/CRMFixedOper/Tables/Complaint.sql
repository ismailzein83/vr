CREATE TABLE [CRMFixedOper].[Complaint] (
    [ID]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [TypeID]            UNIQUEIDENTIFIER NULL,
    [ProcessInstanceId] BIGINT           NULL,
    [CustomerID]        BIGINT           NULL,
    [ContractID]        BIGINT           NULL,
    [Title]             NVARCHAR (900)   NULL,
    [StatusID]          UNIQUEIDENTIFIER NULL,
    [PriorityID]        UNIQUEIDENTIFIER NULL,
    [CreatedTime]       DATETIME         NULL,
    [CreatedBy]         INT              NULL,
    [LastModifiedTime]  DATETIME         NULL,
    [LastModifiedBy]    INT              NULL,
    [PhoneNumber]       NVARCHAR (255)   NULL,
    CONSTRAINT [PK__Complain__3214EC27440B1D61] PRIMARY KEY CLUSTERED ([ID] ASC)
);


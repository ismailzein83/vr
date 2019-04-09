CREATE TABLE [NetworkRentalManager].[TelephonyComplaint] (
    [ID]                BIGINT           IDENTITY (1, 1) NOT NULL,
    [ProcessInstanceId] BIGINT           NULL,
    [Customer]          BIGINT           NULL,
    [Contract]          BIGINT           NULL,
    [PhoneNumber]       NVARCHAR (255)   NULL,
    [Title]             NVARCHAR (255)   NULL,
    [Status]            UNIQUEIDENTIFIER NULL,
    [Priority]          INT              NULL,
    [Type]              UNIQUEIDENTIFIER NULL,
    [DamageType]        UNIQUEIDENTIFIER NULL,
    [BrokenCategory]    UNIQUEIDENTIFIER NULL,
    [CreatedTime]       DATETIME         NULL,
    [CreatedBy]         INT              NULL,
    [LastModifiedTime]  DATETIME         NULL,
    [LastModifiedBy]    INT              NULL,
    CONSTRAINT [PK__Telephon__3214EC2704E4BC85] PRIMARY KEY CLUSTERED ([ID] ASC)
);


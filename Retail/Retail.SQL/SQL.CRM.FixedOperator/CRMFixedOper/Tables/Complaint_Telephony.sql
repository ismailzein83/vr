CREATE TABLE [CRMFixedOper].[Complaint_Telephony] (
    [ID]                     BIGINT           NOT NULL,
    [TelephonyComplaintType] UNIQUEIDENTIFIER NULL,
    [DamageType]             UNIQUEIDENTIFIER NULL,
    [BrokenCategory]         UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK_ComplaintTelephony] PRIMARY KEY CLUSTERED ([ID] ASC)
);


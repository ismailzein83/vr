CREATE TABLE [NetworkRentalManager].[TelephonyContractService] (
    [ID]          BIGINT           IDENTITY (1, 1) NOT NULL,
    [Contract]    BIGINT           NULL,
    [Service]     UNIQUEIDENTIFIER NULL,
    [CreatedTime] DATETIME         NULL,
    CONSTRAINT [PK__Telephon__3214EC276EF57B66] PRIMARY KEY CLUSTERED ([ID] ASC)
);


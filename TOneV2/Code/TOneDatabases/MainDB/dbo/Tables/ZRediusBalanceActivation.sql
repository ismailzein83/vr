CREATE TABLE [dbo].[ZRediusBalanceActivation] (
    [CarrierAccountID] NVARCHAR (5)    NOT NULL,
    [R_UserID]         INT             NOT NULL,
    [Balance]          DECIMAL (18, 3) NOT NULL,
    [Threshold]        DECIMAL (18, 3) NOT NULL,
    [ActivationStatus] INT             CONSTRAINT [DF_RediusBalanceActivation_ActivationStatus] DEFAULT ((0)) NOT NULL,
    [TimeStamp]        ROWVERSION      NOT NULL,
    [UserAppID]        INT             NULL
);


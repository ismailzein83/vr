CREATE TABLE [dbo].[BilateralAlert] (
    [AlertID]        INT            IDENTITY (1, 1) NOT NULL,
    [AgreementsID]   INT            NULL,
    [Title]          NVARCHAR (250) NULL,
    [Action]         NVARCHAR (250) NULL,
    [Enabled]        CHAR (1)       NULL,
    [Level]          INT            NULL,
    [ApplyOn]        INT            NULL,
    [ZoneID]         INT            NULL,
    [Period]         INT            NULL,
    [DataBasedOn]    INT            NULL,
    [Type]           INT            NULL,
    [Parameter1]     INT            NULL,
    [Parameter2]     INT            NULL,
    [Parameter3]     INT            NULL,
    [IsSale]         CHAR (10)      NULL,
    [Hours]          INT            NULL,
    [Days]           INT            NULL,
    [Minutes]        INT            NULL,
    [QualityHours]   INT            NULL,
    [EnableSchedule] CHAR (1)       NOT NULL,
    CONSTRAINT [PK_BilateralAlert] PRIMARY KEY CLUSTERED ([AlertID] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_AgreementID]
    ON [dbo].[BilateralAlert]([AgreementsID] ASC);


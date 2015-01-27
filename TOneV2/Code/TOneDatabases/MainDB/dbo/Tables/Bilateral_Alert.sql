CREATE TABLE [dbo].[Bilateral_Alert] (
    [AlertID]      INT            IDENTITY (1, 1) NOT NULL,
    [AgreementsID] INT            NULL,
    [Title]        NVARCHAR (250) NULL,
    [Action]       NVARCHAR (250) NULL,
    [Enabled]      CHAR (1)       NULL,
    [Level]        INT            NULL,
    [ApplyOn]      INT            NULL,
    [ZoneID]       INT            NULL,
    [Period]       INT            NULL,
    [DataBasedOn]  INT            NULL,
    [Type]         INT            NULL,
    [Parameter1]   INT            NULL,
    [Parameter2]   INT            NULL,
    [Parameter3]   INT            NULL,
    [IsSale]       CHAR (10)      CONSTRAINT [DF_AlertingAgreement_IsSale] DEFAULT ('((Y))') NULL,
    [Hours]        INT            NULL,
    [Days]         INT            NULL,
    [Minutes]      INT            NULL,
    CONSTRAINT [PK_AlertingAgreement] PRIMARY KEY CLUSTERED ([AlertID] ASC)
);


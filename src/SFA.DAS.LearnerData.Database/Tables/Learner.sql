CREATE TABLE dbo.Learner
(
    [Id]					BIGINT				IDENTITY (1, 1) NOT NULL,
    ULN                     BIGINT          NOT NULL,
    UKPRN                   BIGINT          NOT NULL,
    FirstName               NVARCHAR(max)      NOT NULL,
    LastName                NVARCHAR(max)      NOT NULL,
    Email                   NVARCHAR(max)      NOT NULL,
    DOB                     DATETIME2          NOT NULL,
    AcademicYear            INT           NOT NULL,
    StartDate               DATETIME2          NOT NULL,
    PlannedEndDate          DATETIME2 NULL,
    PriorLearningPercentage INT DEFAULT 0 NOT NULL,
    EpaoPrice               INT           NOT NULL,
    TrainingPrice           INT           NOT NULL,
    AgreementId             INT           NOT NULL,
    StandardCode            INT           NOT NULL,
    IsFlexiJob              BIT           NOT NULL,
    PlannedOTJTrainingHours INT           NOT NULL,
    CreatedDate             DATETIME2     DEFAULT (getdate()) NOT NULL,
    UpdatedDate             DATETIME2 NULL,
    ReceivedDate            DATETIME2          NOT NULL,
    CONSTRAINT pk_learner PRIMARY KEY CLUSTERED ( Id asc ),
    CONSTRAINT unq_learner UNIQUE (ULN, UKPRN, agreementId, academicYear)
    );
GO

CREATE
NONCLUSTERED INDEX idx_learner ON dbo.Learner ( ULN  asc, UKPRN  asc, AcademicYear );
GO

CREATE INDEX idx_learner_0 ON dbo.Learner (UKPRN, receivedDate DESC);
GO

execute sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The date the record was received in AS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'learner', @level2type=N'COLUMN',@level2name=N'receivedDate';
GO
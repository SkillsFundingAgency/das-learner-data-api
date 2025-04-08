CREATE TABLE [dbo].[LearnerData](
    [Id] [bigint] NOT NULL,
    [ULN] [bigint] NOT NULL,
    [UKPRN] [bigint] NOT NULL,
    [Firstname] [nvarchar](200) NOT NULL,
    [Lastname] [nvarchar](200) NOT NULL,
    [Email] [nvarchar](max) NULL,
    [Dob] [date] NOT NULL,
    [AcademicYear] [int] NOT NULL,
    [StartDate] [date] NOT NULL,
    [PlannedEndDate] [date] NULL,
    [PercentageLearningToBeDelivered] [int] NOT NULL,
    [EpaoPrice] [int] NOT NULL,
    [TrainingPrice] [int] NOT NULL,
    [AgreementId] [nvarchar](max) NOT NULL,
    [StandardCode] [int] NOT NULL,
    [IsFlexiJob] [bit] NOT NULL,
    [PlannedOTJTrainingHours] [int] NOT NULL,
    [CreatedDate] [datetime] NOT NULL,
    [UpdatedDate] [datetime] NULL,
    [ReceivedDate] [date] NOT NULL,
    [CorrelationId] [nvarchar](max) NOT NULL,
    [ConsumerReference] [nvarchar](max) NOT NULL,
    CONSTRAINT [pk_learnerData] PRIMARY KEY CLUSTERED
(
[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
GO

CREATE  NONCLUSTERED INDEX idx_learnerData ON dbo.learnerData ( ULN  asc, UKPRN  asc, firstname  asc, lastname  asc, academicYear );
GO

CREATE  INDEX idx_learnerData_0 ON dbo.learnerData ( UKPRN, receivedDate DESC );
GO

execute sys.sp_addextendedproperty @name=N'MS_Description', @value=N'The date the record was received in AS' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'learnerData', @level2type=N'COLUMN',@level2name=N'receivedDate';
GO
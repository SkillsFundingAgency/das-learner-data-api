/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/


IF OBJECT_ID('dbo.Learner', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.Learner;
END

-- Migrate existing TrainingCode data from StandardCode
UPDATE dbo.LearnerData SET 
    TrainingCode = CAST(StandardCode AS NVARCHAR(20)) 
WHERE TrainingCode IS NULL;

-- update LearningType for existing records - FoundationApprenticeship
UPDATE dbo.LearnerData SET 
    LearningType = 1 
WHERE LearningType IS NULL AND TrainingCode in (805,806,807,808,809,810,811);

-- update LearningType for existing records - Apprenticeship
UPDATE dbo.LearnerData SET 
    LearningType = 0
WHERE LearningType IS NULL;
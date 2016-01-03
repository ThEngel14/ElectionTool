USE [ElectionDB]
GO

/****** Object:  View [dbo].[ErststimmeAggregated]    Script Date: 03.01.2016 13:33:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER View [dbo].[ErststimmeAggregated] as


/* used for special mode: 
comment first command out if computation based on single votes is demanded, 

otherwise use first function*/

-- Based on already aggregated table
select Election_Id, Wahlkreis_Id, Person_Id, Amount from ErststimmeAmount

-- Based on single votes
--select Election_Id, Wahlkreis_Id, Person_Id, count(*) from Erststimme group by Election_Id, Wahlkreis_Id, Person_Id
GO



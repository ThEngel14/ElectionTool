USE [ElectionDB]
GO

/****** Object:  View [dbo].[SeatsPerState]    Script Date: 13.11.2015 08:41:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




--output: which ElectionID, which state, the right to fill how many seats
ALTER view [dbo].[SeatsPerState] as


/*vvvvvnaive Code, no iteration*
with Divisor as ( select round(sum(s.count) / 598, 0) as Divisor, s.Election_Id as Election_Id
					from PopulationBundesland s
					 group by s.Election_Id )

this is now done in dbo.Divisor(Election_ID) */


select poB.Election_Id as Election_Id,  poB.Bundesland_Id as Bundesland_ID, cast(round(poB.Count*1.0/dbo.Divisor(poB.Election_Id), 0) as int) as Seats  -- round to full int, no decimals
from PopulationBundesland poB








GO



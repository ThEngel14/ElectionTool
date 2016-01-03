USE [ElectionDB]
GO

/****** Object:  View [dbo].[SeatsPerState]    Script Date: 03.01.2016 13:49:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO








--output: which ElectionID, which state, the right to fill how many seats
ALTER view [dbo].[SeatsPerState] WITH SCHEMABINDING as




select poB.Election_Id as Election_Id, 
	 poB.Bundesland_Id as Bundesland_ID,
	 cast(round(poB.Count*1.0/dbo.Divisor(poB.Election_Id), 0) as int) as Seats  -- round to full int, no decimals
from dbo.PopulationBundesland poB




GO



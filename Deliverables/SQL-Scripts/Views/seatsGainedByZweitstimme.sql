USE [ElectionDB]
GO

/****** Object:  View [dbo].[seatsGainedByZweitstimme]    Script Date: 03.01.2016 13:46:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO







ALTER view [dbo].[seatsGainedByZweitstimme] as 

(select votes.Election_Id as Election_Id, 
	votes.Bundesland_Id as Bundesland_Id, 
	votes.party_Id as Party_Id, 
	cast(round(1.0*votes.Amount / dbo.divisorState(
													votes.Election_Id,
													votes.Bundesland_Id,
													 (Select poB.Count/s.Seats
															from PopulationBundesland poB
															where poB.Bundesland_Id = votes.Bundesland_Id 
															and poB.Election_Id=votes.Election_Id), --starting value of @tempDivisor
													s.Seats)
		,0) as int) 
	as seats
 from  ZweitstimmenState votes, SeatsPerState s
where 
	s.Election_Id = votes.Election_Id
	and s.Bundesland_ID = votes.Bundesland_Id
	and votes.Party_ID in (select p.Id from Parties5 p
							where p.Election_Id=votes.Election_ID))


				






GO



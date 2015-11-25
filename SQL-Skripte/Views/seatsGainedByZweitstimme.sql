USE [ElectionDB]
GO

/****** Object:  View [dbo].[seatsGainedByZweitstimme]    Script Date: 22.11.2015 14:14:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER view [dbo].[seatsGainedByZweitstimme] as 

(select votes.Election_Id as Election_Id, votes.Bundesland_Id as Bundesland_Id, votes.party_Id as Party_Id, 
	cast(round(1.0*votes.Amount/dbo.divisorState(votes.Election_Id, votes.Bundesland_Id),0) as int)  as seats
 from  ZweitstimmenState votes 
where votes.Party_ID in (select p.Id from Parties5 p
						where p.Election_Id=votes.Election_ID))

GO



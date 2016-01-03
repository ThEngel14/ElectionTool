USE [ElectionDB]
GO

/****** Object:  View [dbo].[ZweitstimmenState]    Script Date: 03.01.2016 14:01:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER View [dbo].[ZweitstimmenState] as

 ( select votes.Election_Id as Election_Id, 
		wk.Bundesland_Id as Bundesland_Id,
		votes.Party_Id as Party_ID, 
		coalesce(sum(votes.Amount),0) as Amount
	from ZweitstimmeAmount votes, Wahlkreis wk 
	where votes.Wahlkreis_Id = wk.Id
	group by votes.Election_Id, wk.Bundesland_Id, votes.Party_Id
		); 
						
GO



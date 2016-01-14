USE [ElectionDB]
GO

/****** Object:  View [dbo].[WinnerZweitstimme]    Script Date: 03.01.2016 13:59:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER View [dbo].[WinnerZweitstimme] as

With ExtendedZweitstimmeAggregated (Election_Id, Wahlkreis_Id, Party_Id, RankAmount) as (
		select Election_Id, Wahlkreis_Id, Party_Id, rank() over (partition by election_id, wahlkreis_id order by amount desc)
		from ZweitstimmeAggregated
)

select e.Election_Id, 
	   e.Wahlkreis_Id,
	   k.Name as Wahlkreis_Name,
	   e.Party_Id,
	   p.Name as Party_Name
from ExtendedZweitstimmeAggregated e 
	left join Party p 
		on e.Party_Id = p.Id
	join Wahlkreis k 
		on e.Wahlkreis_Id = k.Id
where e.RankAmount = 1
GO



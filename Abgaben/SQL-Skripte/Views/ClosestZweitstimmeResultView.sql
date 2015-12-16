Alter View ClosestZweitstimmeResult as

With ExtendedZweitstimmeAggregated (Election_Id, Wahlkreis_Id, Party_Id, Amount, RankAmount) as (
		select Election_Id, Wahlkreis_Id, Party_Id, Amount, rank() over (partition by election_id, wahlkreis_id order by amount desc)
		from zweitstimmeAggregated),
	 BaseZweitstimmeAggregated (Election_Id, Wahlkreis_Id, Party_Id, Amount, RankAmount, RankCompare) as (
		select *, case when RankAmount = 1 then 2 else 1 end
		from ExtendedZweitstimmeAggregated
)

select z.Election_Id, 
	   z.Wahlkreis_Id, k.Name as Wahlkreis_Name,
	   z.Party_Id, p.Name as Party_Name,
	   z.Amount, e.Amount as PreviousAmount, 
	   z.Amount - e.Amount as Diff,
	   abs(z.Amount - e.Amount) as AbsDiff
from BaseZweitstimmeAggregated z join ExtendedZweitstimmeAggregated e on z.Election_Id = e.Election_Id and
															 z.Wahlkreis_Id = e.Wahlkreis_Id and
															 e.RankAmount = z.RankCompare
						     join Wahlkreis k on z.Wahlkreis_Id = k.Id
							 join Party p on z.Party_Id = p.Id
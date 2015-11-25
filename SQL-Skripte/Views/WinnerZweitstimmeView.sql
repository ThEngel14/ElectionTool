Alter View WinnerZweitstimme as

With ExtendedZweitstimmeAggregated (Election_Id, Wahlkreis_Id, Party_Id, RankAmount) as (
		select Election_Id, Wahlkreis_Id, Party_Id, rank() over (partition by election_id, wahlkreis_id order by amount desc)
		from ZweitstimmeAggregated
)

select e.*, p.Name as Party_Name
from ExtendedZweitstimmeAggregated e left join Party p on e.Party_Id = p.Id
where e.RankAmount = 1

Alter View ZweitstimmeWahlkreisOverview as

With OverallVotes (Election_Id, Wahlkreis_Id, Overall) as (
		select election_id, wahlkreis_id, sum(amount) as Overall
		from ZweitstimmeAggregated
		group by election_id, wahlkreis_id),
	 Overview as (
		select z.Election_Id, 
			   z.Wahlkreis_Id, k.Name as Wahlkreis_Name,
			   z.Party_Id, p.Name as Party_Name,
			   z.Amount as Votes, o.Overall, 1.0*z.Amount / o.Overall as PercentVotes
		from ZweitstimmeAggregated z join OverallVotes o on z.Election_Id = o.Election_Id and z.Wahlkreis_Id = o.Wahlkreis_Id
								 join Wahlkreis k on z.Wahlkreis_Id = k.Id
								 left join Party p on z.Party_Id = p.Id
)

select o.*, o.PercentVotes - o2.PercentVotes as Previous
from Overview o left join Overview o2 on o.Election_Id - 1 = o2.Election_Id and
										 o.Wahlkreis_Id = o2.Wahlkreis_Id and
										 o.Party_Id = o2.Party_Id
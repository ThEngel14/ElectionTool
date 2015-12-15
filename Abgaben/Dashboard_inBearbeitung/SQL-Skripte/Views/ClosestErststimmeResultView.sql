Alter View ClosestErststimmeResult as

With ExtendedErststimmeAggregated (Election_Id, Wahlkreis_Id, Person_Id, Amount, RankAmount) as (
		select Election_Id, Wahlkreis_Id, Person_Id, Amount, rank() over (partition by election_id, wahlkreis_id order by amount desc)
		from ErststimmeAggregated),
	 BaseErststimmeAggregated (Election_Id, Wahlkreis_Id, Person_Id, Amount, RankAmount, RankCompare) as (
		select *, case when RankAmount = 1 then 2 else 1 end
		from ExtendedErststimmeAggregated
)

select e.Election_Id, 
	   e.Wahlkreis_Id, k.Name as Wahlkreis_Name,
	   e.Person_Id, p.Title, p.Firstname, p.Lastname,
	   af.Party_Id, pa.Name as Party_Name,
	   e.Amount, ex.Amount as PreviousAmount, 
	   e.Amount - ex.Amount as Diff,
	   abs(e.Amount - ex.Amount) as AbsDiff
from BaseErststimmeAggregated e join ExtendedErststimmeAggregated ex on e.Election_Id = ex.Election_Id and
															    e.Wahlkreis_Id = ex.Wahlkreis_Id and
															    ex.RankAmount = e.RankCompare
						     join Wahlkreis k on e.Wahlkreis_Id = k.Id
							 join Person p on e.Person_Id = p.Id
							 left join PartyAffiliation af on p.Id = af.Person_Id and e.Election_Id = af.Election_Id
							 left join Party pa on af.Party_Id = pa.Id